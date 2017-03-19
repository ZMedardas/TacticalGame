using UnityEngine;
using System.Collections;

public class UnitBehaviour : GeneralBehaviour
{
	private void Update ()
	{
		timer+=Time.deltaTime;
		RecoverHP ();
		if (Control.GenBehPresent(true, gameObject))
		{
			Shoot();
		}
	}

	public override void hit(int dmg)
	{
		health -= dmg;
		if (health < 0)
		{
			Control.Remove(gameObject);
			Destroy(gameObject);
		}
		adjustLight();
	}
}
