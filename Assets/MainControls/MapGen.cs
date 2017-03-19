using UnityEngine;
using System.Collections.Generic;

public class MapGen : MonoBehaviour {

	public GameObject[] roomTemplates;
	public GameObject DoorTemplate;
	private List<DoorNav> Doors;
	private int[,] rooms;
	public int roomHeight, roomWidth, startX, startZ;
	private int roomNum = 0;

	//temp var
	private List<WallRooms> Walls;

	public int getRoom(Vector3 pos)
	{
		return getRoom(pos.x, pos.z);
	}
	public int getRoom(float x, float z)
	{
		int xx = Mathf.FloorToInt(x / 4), zz = Mathf.FloorToInt(z / 4);
		if (xx < 0 || xx >= roomWidth || zz < 0 || zz >= roomHeight)
			return -1;
		else return rooms[xx, zz];
	}
	public int getRoomByIndex(int x, int z)
	{
		return rooms[x, z];
	}

	public int getRoomsTotal()
	{
		return roomNum;
	}

	public List<DoorNav> GetDoorNavsInRoom(int roomID)
	{
		return Doors.FindAll(x => x.Borders(roomID) == true);
	}

	public bool AreRoomsConnected(int roomID1 , int roomID2)
	{
		foreach(DoorNav Door in Doors)
		{
			if(Door.Borders(roomID1, roomID2))
			{
				return true;
			}
		}
		return false;
	}

	public void MapGenerate()
	{
		//initialise DTs
		rooms = new int[roomWidth, roomHeight];
		Walls = new List<WallRooms>();
		Doors = new List<DoorNav>();
		List<WallRooms> rem;
		List<WallRooms> uniqueWalls = new List<WallRooms>();

		//prepare DTs
		for (int X = 0; X < roomWidth; X++)
			for (int Y = 0; Y < roomHeight; Y++)
				rooms[X, Y] = -1;

		int randRoom;
		int rot;
		int[] size;
		//create rooms from templates
		for (int X = 0; X < roomWidth; X++)
			for (int Y = 0; Y < roomHeight; Y++)//create seperate rooms
			{
				while(rooms[X, Y] == -1)//if -1 (no room), then try to add new room
				{
					randRoom = Random.Range(0, roomTemplates.Length);
					rot = Random.Range(0, 4);
					size = roomTemplates[randRoom].GetComponent<RoomSize>().getSize(rot);
					if (spaceIsFree(X, Y, X + size[1] - 1, Y + size[0] - 1))//check if free
					{
						AddRoom(X, Y, roomTemplates[randRoom], size, rot);
					}
				}
			}

		foreach(WallRooms wall in Walls)//choose unique walls
		{
			if(!uniqueWalls.Exists(x => x.transform.position == wall.transform.position)) uniqueWalls.Add(wall);
		}
		DeleteWall(new List<WallRooms>(), listDifference<WallRooms>(Walls, uniqueWalls));//destroy all walls that are not unique
		Walls = null;//get rid of references

		UpdateRooms(uniqueWalls);//update walls to reflect seperated rooms

		uniqueWalls = listDifference<WallRooms>(uniqueWalls, GetWallsNext(uniqueWalls, -1));

		for (int I = 0; I < Mathf.CeilToInt(0.1f * roomNum); I++)//merge some rooms
		{
			WallRooms randWall = uniqueWalls[Random.Range(0, uniqueWalls.Count)];
			SetRoom(randWall.GetBorder(0), randWall.GetBorder(1));
			DeleteWall(uniqueWalls, GetWallsNext(uniqueWalls, randWall.GetBorder(0), randWall.GetBorder(1)));
			UpdateRooms(uniqueWalls);//update walls to reflect seperated rooms
		}

		//add doors
		bool[] access = new bool[roomNum];//initialises to false
		access[getRoom(startX, startZ)] = true;//set starting room
		while (!arrayTrue(access))//while not all rooms accessible
		{
			rem = new List<WallRooms>();
			foreach (WallRooms wall in uniqueWalls)
			{
				if (access[wall.GetBorder(0)] != access[wall.GetBorder(1)])//accessible next to inaccessible
				{
					if (Random.value < 0.3f)
					{
						access[wall.GetBorder(0)] = true;
						access[wall.GetBorder(1)] = true;
						Instantiate(DoorTemplate, wall.gameObject.transform.position, wall.gameObject.transform.rotation);
						Destroy(wall.gameObject);
						rem.Add(wall);
					}
				}
				else if (access[wall.GetBorder(0)])
				{
					if (Random.value < 0.08f)
					{
						Instantiate(DoorTemplate, wall.gameObject.transform.position, wall.gameObject.transform.rotation);
						Destroy(wall.gameObject);
					}
					rem.Add(wall);
				}
			}
			uniqueWalls = listDifference<WallRooms>(uniqueWalls, rem);
		}
	}

	private bool spaceIsFree(int dx,int dy, int ux, int uy)
	{
		if (ux >= roomWidth || dx < 0 || uy >= roomHeight || ux < 0) return false;
		for(int I=dy;I<=uy;I++)
			for(int M=dx;M<=ux;M++)
			{
				if (rooms[M, I] != -1) return false;
			}
		return true;
	}
	private void AddRoom(int dx, int dy, GameObject room, int[] size, int rot)
	{
		for (int I = dy; I <= dy + size[0] - 1; I++)
			for (int M = dx; M <= dx + size[1] - 1; M++)
			{
				rooms[M, I] = roomNum;//debugging for 30mins because you wrote rooms[dx,dy]. Good job, m8. How did it even work all that time?
			}
		Instantiate(room, new Vector3(4f * (float)(dx + 0.5 * size[1]), -0.2f, 4f * (float)(dy + 0.5 * size[0])), Quaternion.AngleAxis(rot * 90, Vector3.up));
		roomNum++;
	}
	private void SetRoom(int r1, int r2)
	{
		int target = Mathf.Max(r1, r2), to = Mathf.Min(r1, r2);
		roomNum--;
		for(int I=0;I<roomHeight;I++)
			for(int K=0;K<roomWidth;K++)
			{
				if (rooms[K, I] == target)
					rooms[K, I] = to;
				else if (rooms[K, I] > target)
					rooms[K, I]--;
			}
	}
	private List<WallRooms> GetWallsNext(List<WallRooms> ls, int a, int b)
	{
		List<WallRooms> ret = new List<WallRooms>();
		foreach(WallRooms wall in ls)
			if (wall.Borders(a, b))
				ret.Add(wall);
		return ret;
	}
	private List<WallRooms> GetWallsNext(List<WallRooms> ls, int a)
	{
		List<WallRooms> ret = new List<WallRooms>();
		foreach (WallRooms wall in ls)
			if (wall.Borders(a))
				ret.Add(wall);
		return ret;
	}
	private void UpdateRooms(List<WallRooms> ls)
	{
		foreach (WallRooms wall in ls)
			wall.UpdateRooms();
	}

	private void DeleteWall(List<WallRooms> ls, WallRooms a)
	{
		ls.Remove(a);
		Destroy(a.gameObject);
	}
	private void DeleteWall(List<WallRooms> ls, List<WallRooms> del)
	{
		foreach (WallRooms wall in del)
		{
			DeleteWall(ls, wall);
		}
	}
	public void pushWall(WallRooms wall)
	{
		Walls.Add(wall);
	}
	public void PushDoor(DoorNav door)
	{
		Doors.Add(door);
	}
	private List<T> listDifference<T>(List<T> target, List<T> removal)
	{
		List<T> left = new List<T>();

		foreach (T obj in target)
			if (!removal.Contains(obj))
				left.Add(obj);

		return left;
	}
	private bool arrayTrue(bool[] arr)
	{
		for(int I=0; I<arr.Length; I++)
		{
			if (!arr[I])
			{
				return false;
			}
		}
		return true;
	}
}
