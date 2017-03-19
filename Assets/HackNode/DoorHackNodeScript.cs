using UnityEngine;

public class DoorHackNodeScript : HackNode
{
	private GameObject Door;
	private DoorNav Nav;
	private bool passable;

	public Material MatLight, MatDark;

	protected override void OnHack()
	{
		passable = false;
		Nav = Door.AddComponent<DoorNav>();
		Nav.PushScript(this);
		Nav.PosSet(passable);
		gameObject.GetComponent<MeshRenderer>().material = MatLight;
	}

	protected override void OnPress()
	{
		passable = !passable;
		Nav.PosSet(passable);
	}

	public bool IsPassable()
	{
		return passable;
	}

	public void AssignDoor(GameObject obj)
	{
		Door = obj.transform.GetChild(0).gameObject;
	}
}
