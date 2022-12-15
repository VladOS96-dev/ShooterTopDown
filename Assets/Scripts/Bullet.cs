using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

	private TypeCharacter collisionTarget;

	public void SetMaterial(Material material)
	{
		GetComponent<MeshRenderer>().material = material;
		TrailRenderer trail = GetComponent<TrailRenderer>();
		trail.startColor = material.color;
		trail.endColor = material.color;
	}
	public void SetParentIgnoreCollision(Collider colliderParent)
	{
		Physics.IgnoreCollision(colliderParent, GetComponent<Collider>());
	}
	public void SetPower(float power, TypeCharacter collisionTarget)
	{
		GetComponent<Rigidbody>().velocity = Vector3.zero;
		GetComponent<Rigidbody>().AddForce(transform.forward * power * Time.deltaTime, ForceMode.VelocityChange);
		this.collisionTarget = collisionTarget;
	}
	void OnCollisionEnter(Collision collision)
	{
		if (collision.collider.CompareTag("Character"))
		{
			if (collision.collider.GetComponentInChildren<ICharacter>().typeCharacter != collisionTarget)
			{
				collision.gameObject.GetComponentInChildren<ICharacter>().Damage();
			}
		}
		else if (collision.collider.CompareTag("DeadZone"))
		{
			GlobalPoolBullet.RemoveActiveBullet(gameObject);
			gameObject.SetActive(false);
		}
		else if (collision.collider.CompareTag("Obstacle"))
		{
			ChangeDirectionBullet(collision.contacts[0]);
		}

	}
	private void ChangeDirectionBullet(ContactPoint contact)
	{

		Quaternion rot = Quaternion.FromToRotation(Vector3.forward, contact.normal);

		transform.rotation = Quaternion.Euler(0, rot.eulerAngles.y, 0);
	}
	public enum CollisionTarget
	{
		PLAYER,
		ENEMIES
	}
}
