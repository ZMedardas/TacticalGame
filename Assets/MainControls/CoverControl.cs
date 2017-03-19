using UnityEngine;
using System.Collections.Generic;

public class CoverControl : MonoBehaviour {

	public MainDoc GameControl;
	public MapGen MapGenerator;

	private List<CoverBeh>[] coverRef;
	public int coverCountMax;

	public void pushCover(CoverBeh cov)
	{
		coverRef[cov.getRoom()].Add(cov);
	}

	public CoverBeh findCover(GameObject re, int cloCo, int eneCo)
	{
		//re reference
		//cloCo priority given to nearby cover
		//eneCo penalty per enemy that has the position in sight
		int bestR = -500000, curR;
		float dis;
		CoverBeh bestCov = null;
		Vector3 pos;

		pos = re.transform.position;
		foreach (CoverBeh coverItem in coverRef[GameControl.getRoomEnemy(re)])
		{
			if (!coverItem.isTaken())
			{
				dis = Vector3.Distance(pos, coverItem.gameObject.transform.position);
				curR = Mathf.RoundToInt(- cloCo * dis - eneCo * coverItem.coverExposedTo());
				if (curR > bestR)
				{
					bestR = curR;
					bestCov = coverItem;
				}
			}
		}
		return bestCov;
	}

	public void RemoveCov(GameObject obj)
	{
		CoverBeh target = obj.GetComponent<CoverBeh>();
		coverRef[target.getRoom()].Remove(target);
	}

	public void initStructures()
	{
		coverRef = new List<CoverBeh>[MapGenerator.getRoomsTotal()];
		for (int I = 0; I < MapGenerator.getRoomsTotal(); I++)
			coverRef[I] = new List<CoverBeh>();
	}
}
