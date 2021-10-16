using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerHandle
{
	public static void WelcomeReceived(int fromClient, Packet packet)
	{
		int clientIdCheck = packet.ReadInt();
		string username = packet.ReadString();

		Debug.Log(
			$"{Server.Instance.clients[fromClient].tcp.socket.Client.RemoteEndPoint} connected successfully and is now player {fromClient}" +
			$" with the nickname {username}"
		);

		if (fromClient != clientIdCheck)
		{
			Debug.Log($"Player \"{username}\" (ID: {fromClient}) has assumed the wrong client ID ({clientIdCheck})!");
		}

		Server.Instance.clients[fromClient].SendIntoGame(username);
	}

	public static void PlayerMovement(int fromClient, Packet packet)
	{
		bool[] inputs = new bool[packet.ReadInt()];

		for (int i = 0; i < inputs.Length; i++)
		{
			inputs[i] = packet.ReadBool();
		}

		Quaternion rotation = packet.ReadQuaternion();

		Server.Instance.clients[fromClient].player.SetInput(inputs, rotation);
	}
}
