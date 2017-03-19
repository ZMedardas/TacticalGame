using UnityEngine;
using System.Collections;

public class MoveUnitOnClick : MonoBehaviour {

	public MainDoc Md;

	public GameObject Pointer;

	void OnMouseDown()
	{
		Ray ray;
		RaycastHit rayHit;
		ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		Physics.Raycast(ray, out rayHit, 50);
		Instantiate(Pointer, rayHit.point + 2 * Vector3.up, Quaternion.identity);
		Md.MoveActiveUnitTo(rayHit.point - 4 * Vector3.up);
	}
}
