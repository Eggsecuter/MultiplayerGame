﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	public int id;
	public string username;
	[HideInInspector]
	public CharacterController controller;
	public Transform shootOrigin;
	public float gravity = -9.81f;
	public float moveSpeed = 5f;
	public float jumpSpeed = 5f;
	public float health;
	public float maxHealth = 100f;

	private bool[] inputs;
	private float yVelocity = 0;

	private void Start()
	{
		controller = GetComponent<CharacterController>();
		gravity *= Time.fixedDeltaTime * Time.fixedDeltaTime;
		moveSpeed *= Time.fixedDeltaTime;
		jumpSpeed *= Time.fixedDeltaTime;
	}

	public void Initialize(int id, string username)
	{
		this.id = id;
		this.username = username;
		health = maxHealth;

		inputs = new bool[5];
	}

	public void FixedUpdate()
	{
		if (health <= 0f)
		{
			return;
		}

		Vector2 inputDirection = Vector2.zero;

		if (inputs[0])
		{
			inputDirection.y += 1;
		}
		if (inputs[1])
		{
			inputDirection.y -= 1;
		}
		if (inputs[2])
		{
			inputDirection.x -= 1;
		}
		if (inputs[3])
		{
			inputDirection.x += 1;
		}

		Move(inputDirection);
	}

	private void Move(Vector2 inputDirection)
	{
		Vector3 moveDirection = transform.right * inputDirection.x + transform.forward * inputDirection.y;
		moveDirection *= moveSpeed;

		if (controller.isGrounded)
		{
			yVelocity = 0f;

			if (inputs[4])
			{
				yVelocity = jumpSpeed;
			}
		}

		yVelocity += gravity;

		moveDirection.y = yVelocity;
		controller.Move(moveDirection);

		ServerSend.Instance.PlayerPosition(this);
		ServerSend.Instance.PlayerRotation(this);
	}

	public void SetInput(bool[] inputs, Quaternion rotation)
	{
		this.inputs = inputs;
		transform.rotation = rotation;
	}

	public void Shoot(Vector3 viewDirection)
	{
		if (Physics.Raycast(shootOrigin.position, viewDirection, out RaycastHit hit, 25f))
		{
			if (hit.collider.CompareTag("Player"))
			{
				hit.collider.GetComponent<Player>().TakeDamage(50f);
			}
		}
	}

	public void TakeDamage(float damage)
	{
		if (health <= 0f)
		{
			return;
		}

		health -= damage;

		if (health <= 0f)
		{
			health = 0f;
			controller.enabled = false;
			transform.position = new Vector3(0f, 25f, 0f);
			ServerSend.Instance.PlayerPosition(this);
			StartCoroutine(Respawn());
		}

		ServerSend.Instance.PlayerHealth(this);
	}

	private IEnumerator Respawn()
	{
		yield return new WaitForSeconds(5f);

		health = maxHealth;
		controller.enabled = true;
		ServerSend.Instance.PlayerRespawned(this);
	}
}
