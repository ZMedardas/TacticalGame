using UnityEngine;

public class AssaultGroup : EnemyGroup {

	private Vector3 AttackPos;
	private int movingIndex;

	protected override void OnFull()
	{
		AttackPos = NavCon.GetAttackPos();
		NavCon.DeleteAssaultGroup();
		Destroy(gameObject.GetComponent<Collider>());
		curState = State.DISPATCHING;
	}

	void Update()
	{
		timer += Time.deltaTime;

		if (curState != State.FIGHTING && GetAttackedEnemy() != null)
		{
			SwitchToFightingState();
		}

		if(curState == State.REGROUPING)
		{
			if (timer > 1f)
			{
				RegroupAll();
				timer = -4f;
			}
			else if(timer > 0f)
			{
				AttackPos = NavCon.GetAttackPos();
				curState = State.DISPATCHING;
			}
		}
		else if (curState == State.FIGHTING)
		{
			if(!Main.GenBehPresent(false, gameObject))
			{
				curState = State.REGROUPING;
				timer = 2f;
			}
		}
		else if(curState == State.MOVING)
		{
			if (Enemies[0].GetComponent<NavMeshAgent>().remainingDistance < 1f)
				DisbandGroup();
			else
				return;
		}
		else if (curState == State.DISPATCHING)
		{
			if(timer > 0)
			{
				if (MoveNextEnemy())
				{
					timer = -delay;
				}
				else
				{
					curState = State.MOVING;
				}
			}
		}
		else if(curState==State.WAITING)
		{
			if (NavCon.GetIntelState(transform.position) == NavigationControl.IntelNode.States.UN_K || timer > 30f)
			{
				DisbandGroup();
			}
		}
	}

	private void SwitchToFightingState()
	{
		EnemyBehaviour TempEne = GetAttackedEnemy();

		MoveAllEnemies(TempEne.transform.position);
		foreach(EnemyBehaviour Enemy in Enemies)
		{
			Enemy.SetDelay(0f);
		}
		transform.position = TempEne.transform.position;
		curState = State.FIGHTING;
	}

	private EnemyBehaviour GetAttackedEnemy()
	{
		foreach(EnemyBehaviour Enemy in Enemies)
		{
			if(Main.GenBehPresent(false, Enemy.gameObject))
			{
				return Enemy;
			}
		}
		return null;
	}

	private bool MoveNextEnemy()
	{
		while (movingIndex < size && Enemies[movingIndex] == null)
		{
			movingIndex++;
		}
		if (movingIndex < size)
		{
			Enemies[movingIndex].SetDestination(5f, AttackPos);
			movingIndex++;
			return true; // there are still enemies to move;
		}
		else
		{
			movingIndex = 0;
			return false; // all enemies moved, restarting;
		}
	}
	private void MoveAllEnemies(Vector3 pos)
	{
		foreach(EnemyBehaviour Enemy in Enemies)
		{
			Enemy.SetDestination(2f, pos);
		}
	}
	private void RegroupAll()
	{
		ResetStandbyPos();
		foreach (EnemyBehaviour Enemy in Enemies)
		{
			Enemy.SetDestination(GetStandbyPos());
		}
	}

	private void DisbandGroup()
	{
		foreach (EnemyBehaviour Enemy in Enemies)
		{
			if (Enemy != null)
				Enemy.SetDelay(0f);
		}
		if (!IsFull()) NavCon.DeleteAssaultGroup();
		Destroy(gameObject);
	}
}
