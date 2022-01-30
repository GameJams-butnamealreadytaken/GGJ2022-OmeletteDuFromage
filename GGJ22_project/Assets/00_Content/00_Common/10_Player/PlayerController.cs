using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour
{
	public Weapon currentWeapon;

	[Header("Properties")]
	[SerializeField] private int m_playerStartLifePoints = 5;
	[SerializeField] private bool m_isTheMole = false;

	[Header("GUI")]
	[SerializeField] [Tooltip("The image of the life points")]
	private Image m_lifePointsImage;
	[SerializeField]
	[Tooltip("The lose UI")]
	private GameObject m_loseScreenUI;
	[SerializeField]
	[Tooltip("The win UI")]
	private GameObject m_winScreenUI;

	[Header("Sounds")]
	[SerializeField]
	[Tooltip("The audio clips when walking on the grass (farmer)")]
	private List<AudioClip> m_stepsStepsAudioClips = new List<AudioClip>();

	[SerializeField]
	[Tooltip("The audio clips when hitting")]
	private List<AudioClip> m_swingAudioClips = new List<AudioClip>();

	[SerializeField]
	[Tooltip("The hurt clips")]
	private List<AudioClip> m_hurtAudioClips = new List<AudioClip>();

	[SerializeField]
	[Tooltip("The dead clips")]
	private List<AudioClip> m_deadAudioClips = new List<AudioClip>();

	[SerializeField] private AudioClip m_victoryAudioClip;

	private readonly float moveSpeed = 300.0f;
	private Vector3 moveDirection = Vector3.zero;

	private Rigidbody rb;
	private Animator animator;

	private int m_playerLifePoints;

	private bool avoidProcessingNextAttack = false; // Used to avoid attack animation when joining the game with attack button
	private bool isGameOver;
	private bool m_hasWon = false;
	private bool m_inLeaderboardScreen = false;

	public bool HasWon => m_hasWon;
	public bool IsTheMole => m_isTheMole;

	private void Awake()
	{
		rb = GetComponent<Rigidbody>();
		animator = GetComponent<Animator>();

		//
		// Init life points
		m_playerLifePoints = m_playerStartLifePoints;

		//
		// Display tutorial
		GetComponentInChildren<PlayerUI>().ActivateTutorial(GetComponent<PlayerInput>());
	}

	private void FixedUpdate()
	{
		if (isGameOver)
        {
			return;
        }

		bool isAttacking = animator.GetCurrentAnimatorStateInfo(0).IsName("Attack");
		if (isAttacking)
		{
			if (currentWeapon.resetVelocityDuringAttack)
			{
				rb.velocity = Vector2.zero;
			}

			if (!currentWeapon.canMoveDuringAttack)
			{
				return;
			}
		}

		// Move
		Vector3 verticalMove = Vector3.forward * moveDirection.x;
		Vector3 horizontalMove = Vector3.right * moveDirection.z;
		Vector3 move = verticalMove + horizontalMove;
		move *= moveSpeed * Time.fixedDeltaTime;
		rb.velocity = move;

		// Rotation
		if (moveDirection.sqrMagnitude > 0.0f)
		{
			Quaternion rotation = Quaternion.LookRotation(move, gameObject.transform.up);
			transform.rotation = rotation;
		}
	}

	private void Update()
	{
		//
		// update the fill amount of the life points image
		m_lifePointsImage.fillAmount = ((float)m_playerStartLifePoints / (float)m_playerLifePoints);
	}

	public void OnMove(InputValue value)
	{
		if (isGameOver)
			return;

		Vector2 dir = value.Get<Vector2>();

		moveDirection.x = dir.x;
		moveDirection.z = dir.y;

		// Analog to normalized digital since webgl doesn't handle it on z axis for gamepad..
		if (moveDirection.z >= 0.25f)
		{
			if (moveDirection.x >= 0.25f || moveDirection.x <= -0.25f)
				moveDirection.x = 0.71f;
			else
				moveDirection.x = 1.0f;
		}
		else if (moveDirection.z <= -0.25f)
		{
			if (moveDirection.x >= 0.25f || moveDirection.x <= -0.25f)
				moveDirection.x = -0.71f;
			else
				moveDirection.x = -1.0f;
		}
		else
		{
			moveDirection.x = 0.0f;
		}

		if (dir.x >= 0.25f)
		{
			if (moveDirection.z >= 0.25f || moveDirection.z <= -0.25f)
				moveDirection.z = 0.71f;
			else
				moveDirection.z = 1.0f;
		}
		else if (dir.x <= -0.25f)
		{
			if (moveDirection.z >= 0.25f || moveDirection.z <= -0.25f)
				moveDirection.z = -0.71f;
			else
				moveDirection.z = -1.0f;
		}
		else
		{
			moveDirection.z = 0.0f;
		}

		animator.SetBool("Move", dir.SqrMagnitude() != 0.0f);
	}

	public void OnAttack(InputValue value)
	{
		if (avoidProcessingNextAttack)
        {
			// Ignroe the first attack to avoid attack animation when joining
			avoidProcessingNextAttack = false;
			return;
        }

		if (!isGameOver && !animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
		{
			animator.SetTrigger("Attack");
			GetComponentInChildren<AudioSource>().PlayOneShot(m_swingAudioClips[Random.Range(0, m_swingAudioClips.Count)]);
		}
	}

	public void OnStart()
	{
		if (isGameOver && m_inLeaderboardScreen)
		{
			CharacterSelectionManager.Instance.EndLeaderboardAndGoToMainMenu();
		}
	}

	public void OnAttackStarted()
	{
		// Not used anymore
	}

	public void OnAttackEnded()
	{
		currentWeapon.Attack();
	}

	public void OnWon()
	{
		animator.SetTrigger("Win");
		isGameOver = true;
		m_hasWon = true;
		GetComponentInChildren<AudioSource>().PlayOneShot(m_victoryAudioClip);
	}

	public void OnLost()
	{
		animator.SetTrigger("Loose");
		isGameOver = true;
		m_hasWon = false;
	}

	public void TakeDamage(int damageValue)
	{
		//
		// Do nothing if we are game over
		if (isGameOver)
		{
			return;
		}

		m_playerStartLifePoints -= damageValue;

		//
		// Emit sound
		GetComponentInChildren<AudioSource>().PlayOneShot(m_hurtAudioClips[Random.Range(0, m_hurtAudioClips.Count)]);

		//
		// Handle game over (only if game is still running)
		if (CharacterSelectionManager.Instance.IsGameRunning)
		{
			if (m_playerStartLifePoints <= 0)
			{
				CharacterSelectionManager.Instance.NotifyPlayerLost(this.GetComponent<PlayerInput>());
				GetComponentInChildren<AudioSource>().PlayOneShot(m_deadAudioClips[Random.Range(0, m_deadAudioClips.Count)]);
			}
		}
	}

	public void DisplayLoseScreen()
	{
		OnLost();
		m_loseScreenUI.SetActive(true);
	}

	public void DisplayWinScreen()
	{
		OnWon();
		m_winScreenUI.SetActive(true);
	}

	public void HideLoseScreen()
	{
		animator.Play("Idle");
		m_loseScreenUI.SetActive(false);
	}
	
	public void HideWinScreen()
	{
		animator.Play("Idle");
		m_winScreenUI.SetActive(false);
	}

	public void ActivateLeaderboard()
	{
		m_inLeaderboardScreen = true;
		if (m_hasWon)
		{
			animator.Play("Win");
		}
		else
		{
			animator.Play("Idle");
		}
	}

	public void HideTutorial()
	{
		GetComponentInChildren<PlayerUI>().DeactivateTutorial();
	}
}
