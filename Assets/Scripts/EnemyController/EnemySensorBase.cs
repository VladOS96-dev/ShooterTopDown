using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySensorBase : MonoBehaviour
{
	public Enemy npcBase;
	protected List<GameObject> sensedObjects = new List<GameObject>();

	void Start()
	{
		if (npcBase == null)
			npcBase = gameObject.GetComponent<Enemy>();

	}

	void Update()
	{
		UpdateSensor();
	}

	protected virtual void UpdateSensor() { }
}
