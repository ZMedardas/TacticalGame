using UnityEngine;
using System.Collections;

public class ScaleIn : MonoBehaviour {

	void Update () {
		transform.localScale -= new Vector3(0, 4f * Time.deltaTime, 0);
		if (transform.localScale.y < 0.1f)
			Destroy(gameObject);
	}
}
