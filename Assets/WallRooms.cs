using UnityEngine;
using System.Collections;

public class WallRooms : MonoBehaviour {

	protected int[] rooms;
	protected MapGen MapGenerator;

	void Awake ()
	{
		MapGenerator = GameObject.FindWithTag("GameController").GetComponent<MapGen>();
		rooms = new int[2];
		MapGenerator.pushWall(this);
	}
	/*public void UpdateRooms()
	{
		if (gameObject.transform.eulerAngles.y == 0)
		{
			rooms[0] = MapGenerator.getRoom(transform.position.x + 1f, transform.position.z);
			rooms[1] = MapGenerator.getRoom(transform.position.x - 1f, transform.position.z);
		}
		else
		{
			rooms[0] = MapGenerator.getRoom(transform.position.x, transform.position.z + 1f);
			rooms[1] = MapGenerator.getRoom(transform.position.x, transform.position.z - 1f);
		}
	}*/
	public void UpdateRooms()//fix this at some point
	{
		rooms[0] = MapGenerator.getRoom(transform.position.x + 1f, transform.position.z);
		rooms[1] = MapGenerator.getRoom(transform.position.x - 1f, transform.position.z);
		if(rooms[0] == rooms[1])
		{
			rooms[0] = MapGenerator.getRoom(transform.position.x, transform.position.z + 1f);
			rooms[1] = MapGenerator.getRoom(transform.position.x, transform.position.z - 1f);
		}
	}
	public bool Borders(int i)
	{
		if (i == rooms[0] || i == rooms[1])
			return true;
		return false;
	}
	public bool Borders(int i, int y)
	{
		if (i == y && GetBorder(0) != GetBorder(1))
			return false;
		if (Borders(i) && Borders(y))
			return true;
		return false;
	}
	public int GetBorder(int i)
	{
		if (i == 0 || i == 1)
			return rooms[i];
		else
			throw new System.Exception("Trying to get a wall out of range: index accessed " + i);
	}
}
