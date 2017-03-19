using UnityEngine;
using System.Collections;

public class TurretBehaviour : EnemyBehaviour {

	public override void SetStats(bool isFriend, int am, float del, int dmg, float hp, float rel, float acc, string t, string f)
	{
		print("TURRETS DO NOT RELOAD");
	}
	public void SetStats(bool isFriend, int am, float del, int dmg, float hp, float acc, string t, string f)
	{
		isFriendly = isFriend;
		magFull = am;
		mag = am;
		delay = reload = del;
		damage = dmg;
		health = hp;
		healthMax = hp;
		spread = acc;
		targetName = t;
		friendlyName = f;
	}
	public void SetFriendEnemyProjectile(string t, string f, GameObject proj)
	{
		targetName = t;
		friendlyName = f;
		Projectile = proj;
	}
	public void SetFriend()
	{
		isFriendly = true;
		transform.name = "Unit(Clone)";
		gameObject.layer = 11;
	}

	protected override void Update()
	{
		timer += Time.deltaTime;

		RecoverHP();

		if (Control.GenBehPresent(isFriendly, gameObject))
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
