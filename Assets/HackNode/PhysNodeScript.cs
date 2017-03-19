using UnityEngine;

public class PhysNodeScript : HackNode {

	public Material MatLight;

	protected override void OnHack()
	{
		gameObject.GetComponent<MeshRenderer>().material = MatLight;
		return;
	}

	protected override void OnPress()
	{
		return;
	}

	protected override bool HackConditionSatisfied()
	{
		return Main.WithinRadius(false, 2f, transform.position - Vector3.up * 4);
	}
}
