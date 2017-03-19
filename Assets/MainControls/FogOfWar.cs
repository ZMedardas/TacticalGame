using UnityEngine;
using System.Collections.Generic;

public class FogOfWar : MonoBehaviour {

	protected List<GameObject>[] FogTiles;
	protected int numOfRooms;
	protected bool[] skipRoom;

	public GameObject FogTilePrefab;

	public MapGen Map;
	public MainDoc Main;

	public void Init(int roomWidth, int roomHeight)
	{
		numOfRooms = Map.getRoomsTotal();
		FogTiles = new List<GameObject>[numOfRooms];
		skipRoom = new bool[numOfRooms];

		for (int I=0; I < numOfRooms; I++)
		{
			FogTiles[I] = new List<GameObject>();
			skipRoom[I] = false;
		}
		for(int I = 0; I < roomHeight; I++)
		{
			for(int K = 0; K < roomWidth; K++)
			{
				FogTiles[Map.getRoom(2f + K * 4f, 2f + I * 4f)].Add(Instantiate(FogTilePrefab, new Vector3(2f + K * 4f, 3.45f, 2f + I * 4f), Quaternion.identity) as GameObject);
			}
		}
	}
	
	void Update ()
	{
		bool fogPresent;
		for(int I = 0; I < numOfRooms && !skipRoom[I]; I++)
		{
			fogPresent = !Main.GenBehPresent(false, I);
			foreach (GameObject Tile in FogTiles[I])
			{
				Tile.SetActive(fogPresent);
				if (!fogPresent)
					Tile.transform.position = new Vector3(Tile.transform.position.x, 2.75f, Tile.transform.position.z);
			}
		}
	}

	public void SetAlwaysSeen(int roomId)
	{
		skipRoom[roomId] = true;
		foreach (GameObject Tile in FogTiles[roomId])
		{
			Tile.SetActive(false);
		}
	}
}
