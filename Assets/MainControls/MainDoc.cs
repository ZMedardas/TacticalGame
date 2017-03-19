using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainDoc : MonoBehaviour
{
	public CoverControl CoverCont;
	public MapGen MapGenerator;
	public GridGen GridGenerator;
	public NavigationControl NavCon;
	public FogOfWar Fog;

	public int unitCount, enemyCount, turretCount;
	private int curSel;
	private UnitBehaviour[] unitRef;
	private EnemyBehaviour[] enemyRef;
	private TurretBehaviour[] turretRef;
	private Ray ray;
	private RaycastHit rayHit;
	public LayerMask layerMaskUnit;
	public LayerMask layerMaskEnemy;
	public LayerMask layerMaskCover;
	public GameObject Unit, Enemy, Turret;
	public Transform ZeroPoint;
	public GameObject Surface;

	private void InitUnits()
	{
		unitRef = new UnitBehaviour[unitCount];
		for (int I=0; I<unitCount; I++)
		{
			unitRef[I]=(Instantiate(Unit, new Vector3(2f+0.5f*I,0f,2f+0.5f*I), Quaternion.identity) as GameObject).GetComponent<UnitBehaviour>();
			unitRef[I].SetStats(true,10,1.5f,35,100f,4f,10f,"Enemy(Clone)","Unit(Clone)");
		}
	}
	private void InitEnemies()
	{
		enemyRef = new EnemyBehaviour[enemyCount];
		for (int I=0; I<enemyCount; I++)
		{
			enemyRef[I]=(Instantiate(Enemy, new Vector3((MapGenerator.roomWidth-1)*4f*Random.value+3f,0f, (MapGenerator.roomHeight-1) * 4f * Random.value+3f), Quaternion.identity) as GameObject).GetComponent<EnemyBehaviour>();
			enemyRef[I].SetStats(false,5,2f,25,100f,6f,15f,"Unit(Clone)","Enemy(Clone)");
		}
	}
	private void InitTurrets()
	{
		List<GameObject> EmptyNodes = new List<GameObject>();
		EmptyNodes.AddRange(GameObject.FindGameObjectsWithTag("EmptyHackNode"));
		if ((EmptyNodes.Count / 2) - 1 < turretCount)
			turretCount = (EmptyNodes.Count / 2) - 1;
		if (turretCount < 0)
			turretCount = 0;
		print(turretCount);
		turretRef = new TurretBehaviour[turretCount];
		for (int I = 0; I < turretCount; I++)
		{
			int ENID = Random.Range(0, EmptyNodes.Count);
			turretRef[I] = (Instantiate(Turret, EmptyNodes[ENID].transform.position - Vector3.up * 2.7f, Quaternion.identity) as GameObject).GetComponent<TurretBehaviour>();
			turretRef[I].SetStats(false, 20, 0.3f, 10, 300f, 20f, "Unit(Clone)", "Enemy(Clone)");
			turretRef[I].transform.name = "Enemy(Clone)";
			EmptyNodes[ENID].tag = "Untagged";
			Destroy(EmptyNodes[ENID]);
			print(EmptyNodes[ENID]);
			EmptyNodes.RemoveAt(ENID);
		}
	}
	void Start ()
	{
		MapGenerator.MapGenerate();
		CoverCont.initStructures();
		InitUnits();
		InitEnemies();
		InitTurrets();
		NavCon.Init();
		GridGenerator.GridGenerate(MapGenerator.startX, MapGenerator.startZ);
		Fog.Init(MapGenerator.roomWidth, MapGenerator.roomHeight);
	}

	void Update ()
	{
		if (Input.GetKey (KeyCode.Keypad1))//select unit
			curSel = 0;
		else if (Input.GetKey (KeyCode.Keypad2))
			curSel = 1;
		else if (Input.GetKey (KeyCode.Keypad3))
			curSel = 2;
		else if (Input.GetKey (KeyCode.Keypad4))
			curSel = 3;

		if (curSel >= unitCount)
			curSel = unitCount - 1;

		//NavCon.PrintStatesAround(unitRef[curSel].GetRoom());
	}

	//control

	public void MoveActiveUnitTo(Vector3 pos)
	{
		NavMeshPath Path = new NavMeshPath();
		unitRef[curSel].GetComponent<NavMeshAgent>().CalculatePath(pos, Path);
		unitRef[curSel].GetComponent<NavMeshAgent>().SetPath(Path);
	}

	//room related

	public int getRoomEnemy(GameObject enemy)
	{
		return GetRoomGenBeh(enemy);
	}

	public int GetRoomGenBeh(GameObject obj)
	{
		return MapGenerator.getRoom(obj.transform.position);
		//maybe just use getRoom in the first place...
	}

	//units related

	public int positionSeenBy(Vector3 pos)
	{
		int seenBy = 0;
		for(int I = 0; I < unitCount; I++)
		{
			if (unitRef[I].GetRoom() == MapGenerator.getRoom(pos.x, pos.z) && !Physics.Linecast(pos, unitRef[I].transform.position + new Vector3(0f, 1f, 0f), layerMaskCover))
			{
				seenBy++;
			}
		}
		for (int I = 0; I < turretCount; I++)
		{
			if (turretRef[I].transform.name == "Unit(Clone)" && turretRef[I].GetRoom() == MapGenerator.getRoom(pos.x, pos.z) && !Physics.Linecast(pos, turretRef[I].transform.position + new Vector3(0f, 1f, 0f), layerMaskCover))
			{
				seenBy++;
			}
		}
		return seenBy;
	}

	public bool WithinRadius(bool enemy, float radius, Vector3 pos)
	{
		if (enemy)
		{
			return false;
		}
		else
		{
			for(int I=0;I<unitCount;I++)
			{
				if (Vector3.Distance(unitRef[I].transform.position, pos) <= radius)
					return true;
			}
			return false;
		}
	}

	//combat

	public bool GenBehPresent(bool enemy,GameObject n)
	{
		return GenBehPresent(enemy, MapGenerator.getRoom(n.transform.position));
	}
	public bool GenBehPresent(bool enemy, int room)
	{
		if (enemy)
		{
			for (int I = 0; I < enemyCount; I++)
			{
				if (enemyRef[I].GetRoom() == room)
					return true;
			}
			for (int I = 0; I < turretCount; I++)
			{
				if (turretRef[I].transform.name == "Enemy(Clone)" && turretRef[I].GetRoom() == room)
					return true;
			}
			return false;
		}
		else
		{
			for (int I = 0; I < unitCount; I++)
			{
				if (unitRef[I].GetRoom() == room)
					return true;
			}
			for (int I = 0; I < turretCount; I++)
			{
				if (turretRef[I].transform.name == "Unit(Clone)" && turretRef[I].GetRoom() == room)
					return true;
			}
			return false;
		}
	}

	public GameObject GetTarget(bool enemy,GameObject n)//picks and returns target semi-randomly
	{//REWRITE, seriously...
		int tR = MapGenerator.getRoom(n.transform.position);
		float returnChance = 0.75f;
		GameObject temp=null;
		if (enemy) {
			for (int I=0; I< enemyCount; I++)
			{
				if(enemyRef[I].GetRoom() == tR)
				{
					RaycastHit hit;
					Physics.Raycast(n.transform.position+Vector3.up,(enemyRef[I].transform.position - n.transform.position), out hit,50,layerMaskUnit);
					if(hit.collider.gameObject.name=="Enemy(Clone)")
					{
						temp=enemyRef[I].gameObject;
						if(Random.value>returnChance)return temp;
					}
				}
			}
			for (int I = 0; I < turretCount; I++)
			{
				if (turretRef[I].GetRoom() == tR && turretRef[I].transform.name == "Enemy(Clone)")
				{
					RaycastHit hit;
					Physics.Raycast(n.transform.position + Vector3.up, (turretRef[I].transform.position - n.transform.position), out hit, 50, layerMaskUnit);
					if (hit.collider.gameObject.name == "Enemy(Clone)")
					{
						temp = turretRef[I].gameObject;
						if (Random.value > returnChance) return temp;
					}
				}
			}
			return temp;
		}
		else {
			for (int I=0; I<unitCount; I++){
				if(unitRef[I].GetRoom() == tR)
				{
					RaycastHit hit;
					Physics.Raycast(n.transform.position+Vector3.up,(unitRef[I].transform.position - n.transform.position), out hit,50,layerMaskEnemy);
					if(hit.collider.gameObject.name=="Unit(Clone)")
					{
						temp=unitRef[I].gameObject;
						if(Random.value>returnChance)return temp;
					}
				}
			}
			for (int I = 0; I < turretCount; I++)
			{
				if (turretRef[I].GetRoom() == tR && turretRef[I].transform.name == "Unit(Clone)")
				{
					RaycastHit hit;
					Physics.Raycast(n.transform.position + Vector3.up, (turretRef[I].transform.position - n.transform.position), out hit, 50, layerMaskEnemy);
					if (hit.collider.gameObject.name == "Unit(Clone)")
					{
						temp = turretRef[I].gameObject;
						if (Random.value > returnChance) return temp;
					}
				}
			}
			return temp;
		}
	}
	public void Remove(GameObject obj)
	{
		for (int I=0; I<enemyCount; I++) {
			if(enemyRef[I].gameObject == obj)
			{
				enemyCount--;
				enemyRef[I]=enemyRef[enemyCount];
				return;
			}
		}
		for (int I=0; I<unitCount; I++) {
			if(unitRef[I].gameObject == obj)
			{
				unitCount--;
				unitRef[I]=unitRef[unitCount];
				return;
			}
		}
		for (int I = 0; I < turretCount; I++)
		{
			if (turretRef[I].gameObject == obj)
			{
				turretCount--;
				turretRef[I] = turretRef[turretCount];
				return;
			}
		}
	}
}