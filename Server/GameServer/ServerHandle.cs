using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer
{
	class ServerHandle
	{
		public static void WelcomeReceived(int fromClient, Packet packet)
		{
			int clientIdCheck = packet.ReadInt();
			string username = packet.ReadString();

			Console.WriteLine(
				$"{Server.Instance.clients[fromClient].tcp.socket.Client.RemoteEndPoint} connected successfully and is now player {fromClient}" +
				$" with the nickname {username}"
			);

			if (fromClient != clientIdCheck)
			{
				Console.WriteLine($"Player \"{username}\" (ID: {fromClient}) has assumed the wrong client ID ({clientIdCheck})!");
			}

			// TODO: Send player into game
		}

		public static void UDPTestReceived(int fromClient, Packet packet)
		{
			string msg = packet.ReadString();

			Console.WriteLine($"Received packet via UDP. Contains message: {msg}");
		}
	}
}
