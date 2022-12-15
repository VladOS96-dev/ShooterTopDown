using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum NPC_EnemyState
{
	IDLE_STATIC,
	IDLE_ROAMER,
	IDLE_PATROL,
	INSPECT,
	ATTACK, FIND_WEAPON,
	KNOCKED_OUT, DEAD, NONE
}
public class Enemy : MonoBehaviour, ICharacter
{
	public float inspectTimeout;

	[SerializeField] private NPC_EnemyState idleState = NPC_EnemyState.IDLE_ROAMER;
	public TypeCharacter typeCharacter { get => TypeCharacter.Enemy; }
	[SerializeField] private LayerMask hitTestLayer;
	[SerializeField] private Transform weaponPivot;
	[SerializeField] private Bullet proyectilePrefab;
	[SerializeField] private float powerWeapon = 2000f;
	[SerializeField] private float speed = 7;
	private System.Action _initState;
	private System.Action _updateState;
	private System.Action _endState;
	private UnityEngine.AI.NavMeshAgent navMeshAgent;
	[SerializeField] private NPC_EnemyState currentState = NPC_EnemyState.NONE;
	private Vector3 targetPos;
	private float weaponRange = 20.0f;
	private float weaponActionTime = 0.025f;
	private float weaponTime = 0.05f;
	private Timer idleTimer = new Timer();
	private Timer idleRotateTimer = new Timer();
	private bool idleWaiting, idleMoving;
	private Timer inspectTimer = new Timer();
	private Timer inspectTurnTimer = new Timer();
	private bool inspectWait;
	private Timer attackActionTimer = new Timer();
	private bool actionDone;




	void Start()
	{
		navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
		SetState(idleState);
	}

	void Update()
	{
		_updateState();

	}
	public void SetState(NPC_EnemyState newState)
	{
		if (currentState != newState)
		{
			if (_endState != null)
				_endState();
			switch (newState)
			{
				case NPC_EnemyState.IDLE_ROAMER: _initState = StateInit_IdleRoamer; _updateState = StateUpdate_IdleRoamer; break;
				case NPC_EnemyState.INSPECT: _initState = StateInit_Inspect; _updateState = StateUpdate_Inspect; break;
				case NPC_EnemyState.ATTACK: _initState = StateInit_Attack; _updateState = StateUpdate_Attack; break;
			}
			_initState();
			currentState = newState;
		}
	}

	///////////////////////////////////////////////////////// STATE: IDLE ROAMER

	void StateInit_IdleRoamer()
	{
		navMeshAgent.speed = speed;
		idleTimer.StartTimer(Random.Range(2.0f, 4.0f));
		RandomRotate();
		AdvanceIdle();
		idleWaiting = false;
		idleMoving = true;
	}
	void StateUpdate_IdleRoamer()
	{

		idleTimer.UpdateTimer();

		if (idleMoving)
		{
			if (HasReachedMyDestination())
			{
				AdvanceIdle();

			}
		}
		else if (idleWaiting)
		{
			idleRotateTimer.UpdateTimer();
			if (idleRotateTimer.IsFinished())
			{
				RandomRotate();
				idleRotateTimer.StartTimer(Random.Range(1.5f, 3.25f));
			}

		}
		if (idleTimer.IsFinished())
		{
			if (idleMoving)
			{
				navMeshAgent.isStopped = true;
				float waitTime = Random.Range(2.5f, 6.5f);
				float randomTurnTime = waitTime / 2.0f;
				idleRotateTimer.StartTimer(randomTurnTime);
				idleTimer.StartTimer(waitTime);


			}
			else if (idleWaiting)
			{
				idleTimer.StartTimer(Random.Range(2.0f, 4.0f));

				AdvanceIdle();
			}

			idleMoving = !idleMoving;
			idleWaiting = !idleMoving;

		}

	}

	void AdvanceIdle()
	{

		RaycastHit hit = new RaycastHit();
		Physics.Raycast(transform.position, transform.forward * 5.0f, out hit, 50.0f, hitTestLayer);

		if (hit.distance < 3.0f)
		{
			Vector3 dir = hit.point - transform.position;
			Vector3 reflectedVector = Vector3.Reflect(dir, hit.normal);
			Physics.Raycast(transform.position, reflectedVector, out hit, 50.0f, hitTestLayer);
		}
		navMeshAgent.Resume();
		navMeshAgent.SetDestination(hit.point);

	}
	///////////////////////////////////////////////////////// STATE: INSPECT

	void StateInit_Inspect()
	{
		navMeshAgent.speed = 10.0f;
		navMeshAgent.Resume();
		inspectTimer.StopTimer();
		inspectWait = false;
	}
	void StateUpdate_Inspect()
	{


		if (HasReachedMyDestination() && !inspectWait)
		{
			inspectWait = true;
			inspectTimer.StartTimer(2.0f);
			inspectTurnTimer.StartTimer(1.0f);
		}
		navMeshAgent.SetDestination(targetPos);
		RaycastHit hit = new RaycastHit();
		Physics.Raycast(transform.position, transform.forward, out hit, weaponRange, hitTestLayer);

		if (hit.collider != null && hit.collider.tag == "Character")
		{
			SetState(NPC_EnemyState.ATTACK);
		}
		if (inspectWait)
		{
			inspectTimer.UpdateTimer();
			inspectTurnTimer.UpdateTimer();
			if (inspectTurnTimer.IsFinished())
			{
				RandomRotate();
				inspectTurnTimer.StartTimer(Random.Range(0.5f, 1.25f));
			}
			if (inspectTimer.IsFinished())
				SetState(idleState);
		}
	}

	///////////////////////////////////////////////////////// STATE: ATTACK

	void StateInit_Attack()
	{
		navMeshAgent.isStopped = true;
		navMeshAgent.velocity = Vector3.zero;
		CancelInvoke("AttackAction");
		Invoke("AttackAction", weaponActionTime);
		attackActionTimer.StartTimer(weaponTime);
		actionDone = false;
	}
	void StateUpdate_Attack()
	{
		attackActionTimer.UpdateTimer();
		if (!actionDone && attackActionTimer.IsFinished())
		{
			EndAttack();
			actionDone = true;
		}
	}

	void EndAttack()
	{
		SetState(NPC_EnemyState.INSPECT);
	}
	void AttackAction()
	{
		Bullet bullet;
		if (GlobalPoolBullet.DisactiveBullets.Count == 0)
		{
			bullet = Instantiate(proyectilePrefab, weaponPivot.position, weaponPivot.rotation);
			GlobalPoolBullet.AddActiveBullet(bullet.gameObject);
		}
		else
		{
			bullet = GlobalPoolBullet.DisactiveBullets[0].GetComponent<Bullet>();
			bullet.gameObject.SetActive(true);
			bullet.transform.position = weaponPivot.position;
			bullet.transform.rotation = weaponPivot.rotation;
			GlobalPoolBullet.AddActiveBullet(bullet.gameObject);
		}
		bullet.transform.Rotate(0, Random.Range(-7.5f, 7.5f), 0);
		bullet.SetPower(powerWeapon, typeCharacter);
		bullet.SetMaterial(GetComponent<MeshRenderer>().material);
		bullet.SetParentIgnoreCollision(GetComponent<Collider>());
	}
	////////////////////////// MISC FUNCTIONS //////////////////////////

	void RandomRotate()
	{
		float randomAngle = Random.Range(45, 180);
		float randomSign = Random.Range(0, 2);
		if (randomSign == 0)
			randomAngle *= -1;

		transform.Rotate(0, randomAngle, 0);
	}

	public bool HasReachedMyDestination()
	{
		float dist = Vector3.Distance(transform.position, navMeshAgent.destination);
		if (dist <= 1.5f)
		{
			return true;
		}

		return false;
	}
	////////////////////////// PUBLIC FUNCTIONS //////////////////////////
	public void SetAlertPos(Vector3 newPos)
	{
		if (idleState != NPC_EnemyState.IDLE_STATIC)
		{
			SetTargetPos(newPos);
		}
	}
	public void SetTargetPos(Vector3 newPos)
	{
		targetPos = newPos;
		if (currentState != NPC_EnemyState.ATTACK)
		{
			SetState(NPC_EnemyState.INSPECT);
		}
	}
	public void Damage()
	{
		navMeshAgent.velocity = Vector3.zero;
		GlobalEventsUI.InvokeOnHit(TypeCharacter.Player);
		GlobalEventsGame.InvokeResetPositionCharacter();
	}
	public void ResetPosition()
	{
		SetState(idleState);
	}
}
