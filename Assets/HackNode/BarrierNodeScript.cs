using UnityEngine;

public class BarrierNodeScript : HackNode {

	public Material MatLight;

	protected override void OnHack()
	{
		gameObject.GetComponent<MeshRenderer>().material = MatLight;
	}

	protected override void OnPress()
	{
		return;
	}
}
