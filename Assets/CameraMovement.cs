using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour {

	public Transform Pos;
	public float speed;
	
	void Update () {
		if(Input.GetKey("up"))Pos.position+=(new Vector3(0f,0f,speed*Time.deltaTime));
		if(Input.GetKey("down"))Pos.position+=(new Vector3(0f,0f,-speed*Time.deltaTime));
		if(Input.GetKey("left"))Pos.position+=(new Vector3(-speed*Time.deltaTime,0f,0f));
		if(Input.GetKey("right"))Pos.position+=(new Vector3(speed*Time.deltaTime,0f,0f));
	}
}
