using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySensor : EnemySensorBase
{
	const float SIGHT_DIRECT_ANGLE = 120.0f, SIGHT_MAX_DISTANCE = 20.0f;
	float height = 2.0f;
	public LayerMask hitTestMask;
	float TARGET_LOST_COOLDOWN = 1.0f, ALERTED_COOLDOWN = 10.0f, lastTargetTime = float.MinValue, lastAlertTime = float.MinValue;
	bool alerted = false;
	protected override void UpdateSensor()
	{
		GetTargetInSight();

	}
	bool targetSpotted = false;
	void GetTargetInSight()
	{
		Collider[] overlapedObjects = Physics.OverlapSphere(transform.position, SIGHT_MAX_DISTANCE);

		for (int i = 0; i < overlapedObjects.Length; i++)
		{
			Vector3 direction = overlapedObjects[i].transform.position - transform.position;
			float objAngle = Vector3.Angle(direction, transform.forward);
			if (overlapedObjects[i].tag == "Character")
			{
				if (objAngle < SIGHT_DIRECT_ANGLE && TargetInSight(overlapedObjects[i].transform, SIGHT_MAX_DISTANCE))
				{
					npcBase.SetTargetPos(overlapedObjects[i].transform.position);
				}

			}
		}
		if (alerted && lastAlertTime + ALERTED_COOLDOWN < Time.time)
		{
			alerted = false;
		}
		if (alerted && targetSpotted && lastTargetTime + TARGET_LOST_COOLDOWN < Time.time)
		{
			targetSpotted = false;
		}



	}
	bool TargetInSight(Transform target, float distance)
	{
		Vector3 sightPosition = transform.position;
		sightPosition.y += height;
		RaycastHit hit = new RaycastHit();
		Vector3 dir = target.position - sightPosition;
		Physics.Raycast(sightPosition, dir, out hit, distance, hitTestMask);
		return (hit.collider != null && target.gameObject == hit.collider.gameObject);
	}

}
