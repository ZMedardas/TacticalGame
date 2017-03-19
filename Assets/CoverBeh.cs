using UnityEngine;
using System.Collections.Generic;

public class CoverBeh : MonoBehaviour {

	public MainDoc GameControl;
	public MapGen MapGenerator;
	public CoverControl CoverCont;

	private int roomNum;
	private bool taken;
	private List<Vector3> coverPoints;

	void Start ()
	{
		GameControl = GameObject.FindWithTag("GameController").GetComponent<MainDoc>();//get reference to GameController
		MapGenerator = GameObject.FindWithTag("GameController").GetComponent<MapGen>();
		CoverCont = GameObject.FindWithTag("GameController").GetComponent<CoverControl>();
		roomNum = MapGenerator.getRoom(transform.position.x, transform.position.z);
		CoverCont.pushCover(this);
		taken = false;
		coverPoints = new List<Vector3>();
		foreach (Transform child in transform){
			if(child.name.Contains("CoverPoint"))coverPoints.Add(child.gameObject.transform.position);
		}
	}

	public int getRoom()
	{
		return roomNum;
	}
	public bool isTaken()
	{
		return taken;
	}
	public void takeCover()
	{
		if (taken)
			print("COVER ALREADY TAKEN");
		else
			taken = true;
	}
	public void releaseCover()
	{
		if (!taken)
			print("COVER ALREADY FREE");
		else
			taken = false;
	}

	public int coverExposedTo()
	{
		int minNum = 10000, minTemp;

		foreach(Vector3 pos in coverPoints)
		{
			minTemp = GameControl.positionSeenBy(pos);
			if (minTemp < minNum)
			{
				minNum = minTemp;
			}
		}
		return minNum;
	}
	public Vector3 getCoverPos()
	{
		int minNum = 10000, minTemp;
		Vector3 minPos = new Vector3();
		foreach (Vector3 pos in coverPoints)
		{
			minTemp = GameControl.positionSeenBy(pos);
			if (minTemp < minNum)
			{
				minNum = minTemp;
				minPos = pos;
			}
		}
		return minPos;
	}
}
