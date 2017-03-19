using UnityEngine;
using System.Collections;

public class FlashBehaviour : MonoBehaviour {
	public float timer;
	
	void Update ()
	{
		timer-=Time.deltaTime;
		if (timer <= 0)this.GetComponent<Light> ().intensity = 0;
		if(!this.GetComponent<AudioSource>().isPlaying)Destroy (gameObject);//destroy once audio stops playing
	}
}