using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OctaProgBar : MonoBehaviour {

	private float progPerStage, prog;
	private int progStage;
	private List<Transform> ProgBars;

	public GameObject ProgBarPrefab;

	void Awake()
	{
		progPerStage = 0f;
		prog = 0f;
		progStage = 0;
		ProgBars = new List<Transform>();
		foreach(Transform Bar in transform)
		{
			ProgBars.Add(Bar);
		}
	}
	public void SetComp(float completeAt)
	{
		if (progPerStage != 0f)
		{
			print("Already set!");
		}
		else
			progPerStage = completeAt / 8;
	}
	public void AddProg(float progress)
	{
		prog += progress;
		while(prog >= progPerStage)
		{
			ProgBars[progStage].localScale = new Vector3(1, 0.2f, 0.25f);
			prog -= progPerStage;
			progStage++;
			if (progStage == 8)
			{
				Destroy(gameObject);
				return;
			}
		}
		ProgBars[progStage].localScale = new Vector3(prog/progPerStage, 0.2f, 0.25f);
	}
}
