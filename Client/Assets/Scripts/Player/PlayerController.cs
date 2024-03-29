﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public Transform cam;

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Mouse0))
		{
			ClientSend.PlayerShoot(cam.forward);
		}
	}

	private void FixedUpdate()
	{
		SendInputToServer();
	}

	private void SendInputToServer()
	{
		bool[] inputs = new bool[]
		{
			Input.GetKey(KeyCode.W),
			Input.GetKey(KeyCode.S),
			Input.GetKey(KeyCode.A),
			Input.GetKey(KeyCode.D),
			Input.GetKey(KeyCode.Space)
		};

		ClientSend.PlayerMovement(inputs);
	}
}
