using UnityEngine;
using System.Collections;

public class ServerSystem : MonoBehaviour {

	private float[] hackSpeeds;
	private HackNode[] AssignedNodes;

	void Awake () {
		AssignedNodes = new HackNode[1];
		hackSpeeds = new float[1];
		hackSpeeds[0] = 1000f;
	}

	void Update () {
		if (AssignedNodes[0] != null && !AssignedNodes[0].IsHacked() && AssignedNodes[0].IsHackable())
		{
			AssignedNodes[0].Hack(hackSpeeds[0] * Time.deltaTime);
		}
	}

	public void AssignHackNode(HackNode node)
	{
		AssignedNodes[0] = node;
	}
}
