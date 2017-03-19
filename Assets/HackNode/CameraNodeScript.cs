using System;
using UnityEngine;

public class CameraNodeScript : HackNode
{
	public Material MatLight;

	protected override void Awake()
	{
		base.Awake();
		GameObject.FindGameObjectWithTag("GameController").GetComponent<NavigationControl>().PushCamera(this);
	}
	protected override void OnHack()
	{
		gameObject.GetComponent<MeshRenderer>().material = MatLight;
		GameObject.FindGameObjectWithTag("GameController").GetComponent<FogOfWar>().SetAlwaysSeen(
			GameObject.FindGameObjectWithTag("GameController").GetComponent<MapGen>().getRoom(transform.position));
		GameObject.FindGameObjectWithTag("GameController").GetComponent<NavigationControl>().RemoveCamera(this);
	}

	protected override void OnPress()
	{
		return;
	}
}
