using UnityEngine;
using System.Collections;

public class ProjectileBehaviour : MonoBehaviour {

	private string target,friendly;
	private int damage;

	public void SetStats(string t,string f,int dmg)
	{
		target=t;
		friendly = f;
		damage = dmg;
	}
	private void OnCollisionEnter(Collision col)
	{
		if (col.gameObject.name == friendly)
			return;
		if (col.gameObject.name == target) {
			IDamagable victim=(IDamagable)col.gameObject.GetComponent(typeof(IDamagable));
			victim.hit (damage);
		}
		Destroy (gameObject);
	}
}
