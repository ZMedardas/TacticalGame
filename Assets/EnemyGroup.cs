using UnityEngine;
using System.Collections.Generic;

public abstract class EnemyGroup : MonoBehaviour {

	protected enum State { WAITING, DISPATCHING, FIGHTING, REGROUPING, MOVING };
	protected int size, actualSize;
	protected float delay = 0.5f, timer = 0f;
	protected List<EnemyBehaviour> Enemies;

	protected State curState;

	protected NavigationControl NavCon;
	protected MainDoc Main;

	public void SetStats(int numberOfEnemies, NavigationControl NC, MainDoc MC)
	{
		Enemies = new List<EnemyBehaviour>();
		size = numberOfEnemies;
		actualSize = 0;
		NavCon = NC;
		Main = MC;
		curState = State.WAITING;
	}

	void OnTriggerEnter(Collider col)
	{
		if (col.gameObject.name == "Enemy(Clone)" && col.gameObject.GetComponent<TurretBehaviour>() == null && !IsFull())
		{
			EnemyBehaviour EnemyTemp = col.gameObject.GetComponent<EnemyBehaviour>();
			if (EnemyTemp.InGroup())
				 return;
			Enemies.Add(EnemyTemp);
			EnemyTemp.SetDestination(100000f, GetStandbyPos());
			EnemyTemp.SetAssaultGroup(this);
			actualSize++;
			if (IsFull())
			{
				timer = -5f;
				OnFull();
			}
		}
	}

	public Vector3 GetPos()
	{
		return transform.position;
	}
	protected Vector3 GetStandbyPos()
	{
		return transform.position;
	}
	protected void ResetStandbyPos()
	{
		return;
	}

	public bool IsFull()
	{
		return (size == actualSize);
	}

	public void RemoveEnemyFrom(EnemyBehaviour En)
	{
		Enemies.Remove(En);
	}

	protected abstract void OnFull();
}
