using UnityEngine;

public class DoorNav : WallRooms
{
	private DoorHackNodeScript DoorScript;
	private NavigationControl NavCon;

	private bool PassForEnemies;

	void Awake()
	{
		MapGenerator = GameObject.FindWithTag("GameController").GetComponent<MapGen>();
		NavCon = GameObject.FindWithTag("GameController").GetComponent<NavigationControl>();
		rooms = new int[2];
		MapGenerator.PushDoor(this);
		UpdateRooms();
		PassForEnemies = true;
	}

	void Update()
	{
		if(NavCon.IntelPresent(rooms[0], rooms[1]))
		{
			PosSet(DoorScript.IsPassable());
		}
	}

	public void PosSet(bool passable)
	{
		if(NavCon.IntelPresent(rooms[0], rooms[1]))
		{
			PassForEnemies = passable;
		}
		if (DoorScript.IsHacked())
		{
			if (passable)
			{
				if (PassForEnemies)
					transform.localPosition = new Vector3(transform.localPosition.x, -4f, transform.localPosition.z);
				else
					transform.localPosition = new Vector3(transform.localPosition.x, -2.9f, transform.localPosition.z);
			}
			else
			{
				if (!PassForEnemies)
					transform.localPosition = new Vector3(transform.localPosition.x, 0f, transform.localPosition.z);
				else
					transform.localPosition = new Vector3(transform.localPosition.x, 0.2f, transform.localPosition.z);
			}
		}
	}

	public bool IsPassableForEnemies()
	{
		return PassForEnemies;
	}

	public void PushScript(DoorHackNodeScript obj)
	{
		DoorScript = obj;
	}
}
