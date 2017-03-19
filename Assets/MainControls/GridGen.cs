using UnityEngine;
using System.Collections.Generic;

public class GridGen : MonoBehaviour {

	private float nodeHeight = 3.3f;
	private List<HackNode> Nodes;

	public GameObject TurretHackNode;
	public GameObject DoorHackNode;
	public GameObject StartHackNode;
	public GameObject[] HackNodePrefabs;
	public GameObject Connector;

	public void GridGenerate(int startX, int startZ)
	{
		Nodes = new List<HackNode>();

		//substitute node points with actual nodes
		foreach(GameObject node in GameObject.FindGameObjectsWithTag("EmptyHackNode"))
		{
			if (Random.value < 0.6f)
			{
				GameObject tempNode = Instantiate(HackNodePrefabs[Random.Range(0, HackNodePrefabs.Length)], node.transform.position + new Vector3(0,0.3f,0), Quaternion.Euler(90, 0, 0)) as GameObject;
				Nodes.Add(tempNode.GetComponent<HackNode>());
			}
			Destroy(node);
		}

		//add nodes next to doors
		foreach(GameObject Door in GameObject.FindGameObjectsWithTag("Door"))
		{
			GameObject tempNode = Instantiate(DoorHackNode, new Vector3(Door.transform.position.x + 1f, nodeHeight, Door.transform.position.z + 1f), Quaternion.Euler(90, 0, 0)) as GameObject;
			tempNode.GetComponent<DoorHackNodeScript>().AssignDoor(Door);
			Nodes.Add(tempNode.GetComponent<HackNode>());
		}

		//add nodes next to turrets
		foreach (GameObject Turret in GameObject.FindGameObjectsWithTag("Turret"))
		{
			GameObject tempNode = Instantiate(TurretHackNode, new Vector3(Turret.transform.position.x + 1f, nodeHeight, Turret.transform.position.z + 1f), Quaternion.Euler(90, 0, 0)) as GameObject;
			tempNode.GetComponent<TurretHackNodeScript>().AssignTurret(Turret.GetComponent<TurretBehaviour>());
			Nodes.Add(tempNode.GetComponent<HackNode>());
		}

		//connect nodes
		List<HackNode> DisNodes = new List<HackNode>(), ConNodes = new List<HackNode>();
		DisNodes.AddRange(Nodes);
		ConNodes.Add((Instantiate(StartHackNode, new Vector3(startX, nodeHeight, startZ), Quaternion.Euler(90, 0, 0)) as GameObject).GetComponent<HackNode>());//insert start node
		while (DisNodes.Count != 0)//quicksort by distance to speed up a little and delete first foreach?
		{
			float minDis = float.MaxValue, tempDis;
			HackNode minDisNode = DisNodes[0], minConNode = ConNodes[0];
			foreach(HackNode Node in DisNodes)
			{
				tempDis = Vector3.Distance(Node.gameObject.transform.position, ConNodes[0].gameObject.transform.position);
				if (tempDis < minDis)
				{
					minDis = tempDis;
					minDisNode = Node;
				}
			}
			DisNodes.Remove(minDisNode);
			foreach(HackNode Node in ConNodes)
			{
				tempDis = Vector3.Distance(Node.gameObject.transform.position, minDisNode.gameObject.transform.position);
				if (tempDis < minDis)
				{
					minDis = tempDis;
					minConNode = Node;
				}
			}
			ConNodes.Add(minDisNode);
			GameObject tempCon = Instantiate(Connector) as GameObject;
			tempCon.transform.localScale = new Vector3(0.15f, 0.15f, minDis);
			tempCon.transform.position = (minDisNode.gameObject.transform.position + minConNode.gameObject.transform.position) / 2 - new Vector3(0, 0.15f, 0);
			tempCon.transform.rotation = Quaternion.LookRotation(minConNode.gameObject.transform.position - minDisNode.gameObject.transform.position);
			minDisNode.PushConnectedNode(minConNode);
			minConNode.PushConnectedNode(minDisNode);
			minDisNode.PushConnection(tempCon);
			minConNode.PushConnection(tempCon);
		}
		ConNodes[0].SetHackable(true);
		ConNodes[0].Hack(100000000f);//quality programming right here;
	}

	public List<DoorHackNodeScript> GetNodesOfTypeDoor()//implement generic version when back online
	{
		List<DoorHackNodeScript> Ret = new List<DoorHackNodeScript>();

		foreach(HackNode Node in Nodes)
		{
			if(Node.GetType() == typeof(DoorHackNodeScript))
			{
				Ret.Add((DoorHackNodeScript)Node);
			}
		}

		return Ret;
	}
}
