using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyBehaviour : GeneralBehaviour{
	protected CoverBeh cover = null;
	protected float moveTimer = 0, idleTimer;
	protected string Job;

	private Vector3 LastSafePos;

	private EnemyGroup EnGr;

	private NavigationControl NavCon;
	private NavMeshAgent NavAgent;

	protected override void Awake()
	{
		base.Awake();
		NavCon = GameObject.FindWithTag("GameController").GetComponent<NavigationControl>();
		NavAgent = gameObject.GetComponent<NavMeshAgent>();
		idleTimer = -15f * Random.value - 1f;
		LastSafePos = transform.position;
		Job = "NEWSPAWN";
	}

	protected virtual void Update ()
	{
		timer += Time.deltaTime;
		moveTimer += Time.deltaTime;

		RecoverHP();

		if (Control.GenBehPresent(false, gameObject))
		{
			Shoot();
			if (!Job.Equals("ATTACK") && !Job.Equals("DEFEND") && !Job.Equals("HELP"))
			{
				if(Job.Equals("FLEE"))
					return;
				NavMeshPath Path = new NavMeshPath();
				NavAgent.CalculatePath(LastSafePos + Vector3.Normalize(LastSafePos - transform.position) * 2, Path);
				NavAgent.SetPath(Path);
				idleTimer = 2f;
				Job = "FLEE";
			}
			else if (moveTimer > 0)
			{
				if (cover != null) cover.releaseCover();
				CoverBeh coverTemp = CoverCont.findCover(gameObject, 1000, 10000);//find suitable cover
				if (coverTemp == null)
				{
					NavAgent.SetDestination(gameObject.transform.position + new Vector3(Random.value * 2f - 1f, 0f, Random.value * 2f - 1f));
					cover = null;
					moveTimer = -0.1f;
				}
				else
				{
					moveTimer = -0.5f;
					cover = coverTemp;
					NavAgent.SetDestination(cover.getCoverPos());
					cover.takeCover();
				}
			}
		}
		else
		{
			LastSafePos = transform.position;
			if (NavAgent.remainingDistance < 0.1f)
			{
				if (idleTimer > 0)
				{
					KeyValuePair<string, Vector3> ret = NavCon.GetJob();
					Job = ret.Key;
					switch(Job)
					{
						case "ATTACK":
							{
								idleTimer = 5f;
								break;
							}
						case "PATROL":
							{
								idleTimer = 8f;
								break;
							}
						case "DEFEND":
							{
								idleTimer = 35f;
								break;
							}
						case "HELP":
							{
								idleTimer = 5f;
								break;
							}
					}
					idleTimer *= -(Random.value + 0.5f);
					NavAgent.SetDestination(ret.Value);
				}
				else
					idleTimer += Time.deltaTime;
			}
		}
	}

	public void SetDestination(float del, Vector3 pos)
	{
		idleTimer = -del * (Random.value + 0.5f);
		NavAgent.SetDestination(pos);
	}
	public void SetDestination(Vector3 pos)
	{
		NavAgent.SetDestination(pos);
	}
	public void SetDelay(float del)
	{
		idleTimer = -del;
	}

	public void SetAssaultGroup(EnemyGroup Gr)
	{
		EnGr = Gr;
	}

	public bool InGroup()
	{
		return (EnGr != null);
	}

	public override void hit(int dmg)
	{
		health -= dmg;
		if (health < 0)
		{
			Control.Remove(gameObject);
			if (cover != null) cover.releaseCover();
			if (EnGr != null) EnGr.RemoveEnemyFrom(this);
			Destroy(gameObject);
			return;
		}
		adjustLight();
	}
}