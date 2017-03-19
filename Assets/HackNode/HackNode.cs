using UnityEngine;
using System.Collections.Generic;

public abstract class HackNode : MonoBehaviour {

	protected float hackComp, hackProg;
	protected List<GameObject> Connections;
	protected List<HackNode> ConnectedNodes;
	private bool hacked, hackable;

	private OctaProgBar ProgBar;
	private ServerSystem SerSys;
	protected MainDoc Main;

	private float raiseHeight = 4f;

	public GameObject ProgBarPrefab;

	protected virtual void Awake()
	{
		SerSys = GameObject.FindGameObjectWithTag("ServerControl").GetComponent<ServerSystem>();
		Main = GameObject.FindGameObjectWithTag("GameController").GetComponent<MainDoc>();
		ConnectedNodes = new List<HackNode>();
		Connections = new List<GameObject>();
		hacked = false;
		hackable = false;
		hackComp = 1000f;
		ProgBar = (Instantiate(ProgBarPrefab, transform.position + Vector3.up, Quaternion.identity) as GameObject).GetComponent<OctaProgBar>();

		ProgBar.SetComp(hackComp);
	}
	void OnMouseDown()
	{
		if (hacked)
			OnPress();
		if(hackable)
			SerSys.AssignHackNode(this);
	}

	public bool IsHacked()
	{
		return hacked;
	}
	public bool IsHackable()
	{
		return hackable;
	}
	public void Hack(float points)
	{
		if (!HackConditionSatisfied())
			return;
		hackProg += points;
		ProgBar.AddProg(points);
		if (hackProg > hackComp)
		{
			hacked = true;
			RaiseNode(this);
			foreach (GameObject conn in Connections)
				RaiseConnection(conn);
			foreach (HackNode node in ConnectedNodes)
				RaiseNode(node);
			OnHack();
		}
	}
	public void SetHackable(bool canHack)
	{
		hackable = canHack;
	}

	private void RaiseConnection(GameObject conn)
	{
		Vector3 newPos = conn.transform.position;
		conn.transform.position = new Vector3(newPos.x, raiseHeight, newPos.z);
	}
	private void RaiseNode(HackNode node)
	{
		Vector3 newPos = node.transform.position;
		node.transform.position = new Vector3(newPos.x, raiseHeight + 0.3f, newPos.z);
		node.SetHackable(true);
	}

	public void PushConnection(GameObject Obj)
	{
		Connections.Add(Obj);
	}
	public void PushConnectedNode(HackNode Node)
	{
		ConnectedNodes.Add(Node);
	}

	protected virtual bool HackConditionSatisfied()
	{
		return true;
	}

	protected abstract void OnHack();

	protected abstract void OnPress();
}
