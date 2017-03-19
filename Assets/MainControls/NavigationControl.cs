using UnityEngine;
using System.Collections.Generic;

public class NavigationControl : MonoBehaviour {

	private IntelNode[] IntelNodes;
	private List<CameraNodeScript> Cameras;
	private int roomWidth, roomHeight;

	private AssaultGroup AssaultGroupActive;

	public GameObject AssaultGroupPrefab;

	public MapGen MapGenerator;
	public MainDoc Main;

	public void Init()
	{
		roomWidth = MapGenerator.roomWidth;
		roomHeight = MapGenerator.roomHeight;
		IntelNodes = new IntelNode[MapGenerator.getRoomsTotal()];
		Cameras = new List<CameraNodeScript>();
		for (int I = 0; I < IntelNodes.Length; I++)
		{
			IntelNodes[I] = new IntelNode(I);
			if (Main.GenBehPresent(true, I))
				IntelNodes[I].SetState(IntelNode.States.EN_K);
			else
				IntelNodes[I].SetState(IntelNode.States.EN_UNK);
		}
		for (int I = 0; I < IntelNodes.Length; I++)//should be recoded later.
		{
			for(int K = MapGenerator.getRoomsTotal() - 1; K >= 0; K--)
			{
				if(MapGenerator.AreRoomsConnected(I, K))
				{
					IntelNodes[I].PushLink(IntelNodes[K]);
				}
			}
		}

		for (int I=0; I < roomHeight; I++)
		{
			for(int K=0; K<roomWidth; K++)
			{
				Vector3 vec = new Vector3(2f + 4f * K, 0f, 2f + 4f * I);
				IntelNodes[MapGenerator.getRoom(vec)].PushPosition(vec);
			}
		}
	}

	void Update()
	{
		UpdateIntelStates();
	}

	private void UpdateIntelStates()
	{
		foreach(IntelNode Node in IntelNodes)
		{
			if (IntelPresent(Node.GetRoom()))
			{
				if (Main.GenBehPresent(false, Node.GetRoom()))
				{
					Node.SetState(IntelNode.States.UN_K);
				}
				else
					Node.SetState(IntelNode.States.EN_K);
			}
			else
			{
				if (Node.GetState() == IntelNode.States.UN_K)
					Node.SetState(IntelNode.States.UN_UNK);
				else if(Node.GetState() == IntelNode.States.EN_K)
					Node.SetState(IntelNode.States.EN_UNK);
			}
		}
	}

	public IntelNode.States GetIntelState(Vector3 pos)
	{
		return GetIntelState(MapGenerator.getRoom(pos));
	}
	public IntelNode.States GetIntelState(int room)
	{
		return IntelNodes[room].GetState();
	}

	public KeyValuePair<string, Vector3> GetJob()
	{
		float r = Random.value;

		if (r < 0.5f)
			return GetPatrol();
		else if (r < 0.8f)
			return GetDefend();
		else
			return GetAssault();
	}

	private KeyValuePair<string, Vector3> GetPatrol()
	{
		string job = "PATROL";
		IntelNode Node = GetNodeOfState(IntelNode.States.EN_UNK);
		if (Node == null)
			return new KeyValuePair<string, Vector3>(job, GetRandomNodePos());
		return new KeyValuePair<string, Vector3>(job, Node.GetRandomPos());
	}
	private KeyValuePair<string, Vector3> GetPatrol(string job)
	{
		return new KeyValuePair<string, Vector3>(job, GetPatrol().Value);
	}
	private KeyValuePair<string, Vector3> GetDefend()
	{
		string job = "DEFEND";
		IntelNode Node;

		for (int I = 0; I < 5; I++)
		{
			Node = GetNodeOfState(IntelNode.States.EN_UNK, IntelNode.States.EN_K);
			if (Node.BordersState(IntelNode.States.UN_K, IntelNode.States.UN_UNK))
				return new KeyValuePair<string, Vector3>(job, Node.GetRandomPos());
		}
		return GetPatrol(job);
	}
	private KeyValuePair<string, Vector3> GetAssault()
	{
		string job = "ATTACK";
		IntelNode Node;

		if (AssaultGroupActive == null)
		{
			Node = GetNodeOfState(IntelNode.States.EN_UNK, IntelNode.States.EN_K);
			if(Node != null)
			{
				AssaultGroupActive = (Instantiate(AssaultGroupPrefab, Node.GetRandomPos(), Quaternion.identity) as GameObject).GetComponent<AssaultGroup>();
				AssaultGroupActive.SetStats(Random.Range(2, Main.enemyCount / 3), this, Main);
			}
			else
				return GetPatrol();
		}
		return new KeyValuePair<string, Vector3>(job, AssaultGroupActive.GetPos());
	}

	public Vector3 GetAttackPos()
	{
		IntelNode Node = GetNodeOfState(IntelNode.States.UN_K, IntelNode.States.UN_UNK);

		if (Node == null)
			return GetRandomNodePos();
		return Node.GetRandomPos();
	}

	public void DeleteAssaultGroup()
	{
		AssaultGroupActive = null;
	}

	public bool IntelPresent(int room1, int room2)
	{
		return (IntelPresent(room1) || IntelPresent(room2));
	}

	public bool IntelPresent(int room)
	{
		return (Main.GenBehPresent(true, room) || CameraPresent(room));
	}

	public bool CameraPresent(int room)
	{
		foreach(CameraNodeScript Camera in Cameras)
		{
			if (MapGenerator.getRoom(Camera.transform.position) == room)
				return true;
		}
		return false;
	}
	public void PushCamera(CameraNodeScript Camera)
	{
		Cameras.Add(Camera);
	}
	public void RemoveCamera(CameraNodeScript Camera)
	{
		Cameras.Remove(Camera);
	}

	public IntelNode GetNodeOfState(IntelNode.States state1)
	{
		return GetNodeOfState(state1, state1);
	}
	public IntelNode GetNodeOfState(IntelNode.States state1, IntelNode.States state2)
	{
		List<IntelNode> NodeList = new List<IntelNode>();

		foreach(IntelNode Node in IntelNodes)
		{
			if (Node.GetState() == state1 || Node.GetState() == state2)
				NodeList.Add(Node);
		}
		if (NodeList.Count == 0)
			return null;
		return NodeList[Random.Range(0, NodeList.Count)];
	}
	public Vector3 GetRandomNodePos()
	{
		return IntelNodes[Random.Range(0, IntelNodes.Length)].GetRandomPos();
	}

	public class IntelNode
	{
		public enum States { UN_K, UN_UNK, EN_K, EN_UNK };

		private States state;
		private List<IntelNode> Links;
		private List<Vector3> Positions;

		private int roomID;

		public IntelNode(int roomID)
		{
			Positions = new List<Vector3>();
			Links = new List<IntelNode>();
			this.roomID = roomID;
		}

		public void SetState(States newState)
		{
			state = newState;
		}
		public States GetState()
		{
			return state;
		}
		public bool BordersState(States st1, States st2)
		{
			return (BordersState(st1) || BordersState(st2));
		}
		public bool BordersState(States st)
		{
			foreach (IntelNode Node in Links)
			{
				if (Node.GetState() == st)
					return true;
			}
			return false;
		}

		public int GetRoom()
		{
			return roomID;
		}

		public List<IntelNode> GetLinks()
		{
			return Links;
		}

		public Vector3 GetRandomPos()
		{
			return Positions[Random.Range(0, Positions.Count)];
		}
		public void PushPosition(Vector3 pos)
		{
			Positions.Add(pos);
		}
		public void PushLink(IntelNode pos)
		{
			Links.Add(pos);
		}
	}
}
