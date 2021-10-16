using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	public int id;
	public string username;
	[HideInInspector]
	public CharacterController controller;
	public float gravity = -9.81f;
	public float moveSpeed = 5f;
	public float jumpSpeed = 5f;

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

		inputs = new bool[5];
	}

	public void FixedUpdate()
	{
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
}
