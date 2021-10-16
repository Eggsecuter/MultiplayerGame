using System.Collections;
using System.Collections.Generic;

public class ServerSend
{
	#region Singleton
	private static ServerSend _instance;
	public static ServerSend Instance
	{
		get
		{
			if (_instance is null)
			{
				_instance = new ServerSend();
			}

			return _instance;
		}
	}

	private ServerSend() { }
	#endregion

	private void SendTCPData(int toClient, Packet packet)
	{
		packet.WriteLength();
		Server.Instance.clients[toClient].tcp.SendData(packet);
	}

	private void SendUDPData(int toClient, Packet packet)
	{
		packet.WriteLength();
		Server.Instance.clients[toClient].udp.SendData(packet);
	}

	private void BroadcastTCPData(Packet packet)
	{
		packet.WriteLength();

		for (int i = 1; i <= Server.Instance.MaxPlayers; i++)
		{
			Server.Instance.clients[i].tcp.SendData(packet);
		}
	}

	private void BroadcastTCPData(Packet packet, int exeptClient)
	{
		packet.WriteLength();

		for (int i = 1; i <= Server.Instance.MaxPlayers; i++)
		{
			if (i != exeptClient)
			{
				Server.Instance.clients[i].tcp.SendData(packet);
			}
		}
	}

	private void BroadcastUDPData(Packet packet)
	{
		packet.WriteLength();

		for (int i = 1; i <= Server.Instance.MaxPlayers; i++)
		{
			Server.Instance.clients[i].udp.SendData(packet);
		}
	}

	private void BroadcastUDPData(Packet packet, int exeptClient)
	{
		packet.WriteLength();

		for (int i = 1; i <= Server.Instance.MaxPlayers; i++)
		{
			if (i != exeptClient)
			{
				Server.Instance.clients[i].udp.SendData(packet);
			}
		}
	}

	#region Packets
	public void Welcome(int toClient, string msg)
	{
		using (Packet packet = new Packet((int)ServerPackets.welcome))
		{
			packet.Write(msg);
			packet.Write(toClient);

			SendTCPData(toClient, packet);
		}
	}

	public void SpawnPlayer(int toClient, Player player)
	{
		using (Packet packet = new Packet((int)ServerPackets.spawnPlayer))
		{
			packet.Write(player.id);
			packet.Write(player.username);
			packet.Write(player.transform.position);
			packet.Write(player.transform.rotation);

			SendTCPData(toClient, packet);
		}
	}

	public void PlayerPosition(Player player)
	{
		using (Packet packet = new Packet((int)ServerPackets.playerPosition))
		{
			packet.Write(player.id);
			packet.Write(player.transform.position);

			BroadcastUDPData(packet);
		}
	}

	public void PlayerRotation(Player player)
	{
		using (Packet packet = new Packet((int)ServerPackets.playerRotation))
		{
			packet.Write(player.id);
			packet.Write(player.transform.rotation);

			BroadcastUDPData(packet, player.id);
		}
	}
	#endregion
}
