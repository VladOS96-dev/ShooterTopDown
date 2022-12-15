using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour, ICharacter
{
	public TypeCharacter typeCharacter { get => TypeCharacter.Player; }
	[SerializeField] private Transform hitTestPivot;
	[SerializeField] private Transform gunPivot;
	[SerializeField] private GameObject mousePointer;
	[SerializeField] private Bullet bulletPrefab;	
	[SerializeField] private float moveSpeed = 10.0f;
	[SerializeField] private float powerWeapon=2000f;
	private float attackTime = 0.1f;
	private Timer attackTimer = new Timer();
	private Vector3 movement;
	private Rigidbody _rigidBody;

	void Start()
	{
		_rigidBody = GetComponent<Rigidbody>();
		attackTimer.StartTimer(0.1f);
	}


	void Update()
	{

		float horizontal = Input.GetAxis("Horizontal");
		float vertical = Input.GetAxis("Vertical");
		movement = transform.TransformDirection(vertical, 0, -horizontal).normalized;
		_rigidBody.velocity = movement * moveSpeed * Time.deltaTime;

		if (Input.GetMouseButtonDown(0) && attackTimer.IsFinished())
		{
			Attack();
		}

		attackTimer.UpdateTimer();
		UpdateAim();
	}

	public void DamagePlayer()
	{
		GlobalEventsGame.InvokeResetPositionCharacter();
		GlobalEventsUI.InvokeOnHit(TypeCharacter.Enemy);

	}
	void UpdateAim()
	{
		Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		mousePos.y = transform.position.y;
		mousePointer.transform.position = mousePos;
		float deltaY = mousePos.z - transform.position.z;
		float deltaX = mousePos.x - transform.position.x;
		float angleInDegrees = Mathf.Atan2(deltaY, deltaX) * 180 / Mathf.PI;
		transform.eulerAngles = new Vector3(0, -angleInDegrees, 0);
	}
	public void Attack()
	{
		Bullet bullet;
		if (GlobalPoolBullet.DisactiveBullets.Count == 0)
		{
			bullet = Instantiate(bulletPrefab, gunPivot.position, gunPivot.rotation);
			GlobalPoolBullet.AddActiveBullet(bullet.gameObject);
		}
		else
		{
			bullet = GlobalPoolBullet.DisactiveBullets[0].GetComponent<Bullet>();
			bullet.gameObject.SetActive(true);
			bullet.transform.position = gunPivot.position;
			bullet.transform.rotation = gunPivot.rotation;
			GlobalPoolBullet.AddActiveBullet(bullet.gameObject);
		}

		bullet.transform.LookAt(mousePointer.transform);
		bullet.transform.Rotate(0, Random.Range(-7.5f, 7.5f), 0);
		bullet.SetPower(powerWeapon, typeCharacter);
		bullet.SetMaterial(GetComponent<MeshRenderer>().material);
		bullet.SetParentIgnoreCollision(GetComponent<Collider>());
		AlertEnemies();
		attackTimer.StartTimer(attackTime);

	}
	void AlertEnemies()
	{
		RaycastHit[] hits = Physics.SphereCastAll(hitTestPivot.position, 20.0f, hitTestPivot.up);
		foreach (RaycastHit hit in hits)
		{
			if (hit.collider != null && hit.collider.tag == "Enemy")
			{
				hit.collider.GetComponent<Enemy>().SetAlertPos(transform.position);
			}
		}
	}

	public void Damage()
	{
		GlobalEventsGame.InvokeResetPositionCharacter();
		GlobalEventsUI.InvokeOnHit(TypeCharacter.Enemy);
	}

    public void ResetPosition()
    {
		if (transform.parent != null)
		{
			transform.position = Vector3.zero;
		}
    }
}
