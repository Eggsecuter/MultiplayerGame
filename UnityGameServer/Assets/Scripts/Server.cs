using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class Server
{
	#region Singleton
	private static Server _instance;
	public static Server Instance
	{
		get
		{
			if (_instance is null)
			{
				_instance = new Server();
			}

			return _instance;
		}
	}

	private Server() { }
	#endregion

	public int MaxPlayers { get; private set; }
	public int Port { get; private set; }
	public Dictionary<int, Client> clients = new Dictionary<int, Client>();
	public delegate void PacketHandler(int fromClient, Packet packet);
	public Dictionary<int, PacketHandler> packetHandlers;

	private TcpListener tcpListener;
	private UdpClient udpListener;

	public void Start(int maxPlayers, int port)
	{
		MaxPlayers = maxPlayers;
		Port = port;

		Debug.Log("Starting server...");
		InitializeServerData();

		tcpListener = new TcpListener(IPAddress.Any, Port);
		tcpListener.Start();
		tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);

		udpListener = new UdpClient(Port);
		udpListener.BeginReceive(UDPReceiveCallback, null);

		Debug.Log($"Server started on {Port}.");
	}

	public void Stop()
	{
		tcpListener.Stop();
		udpListener.Close();
	}

	private void TCPConnectCallback(IAsyncResult result)
	{
		TcpClient client = tcpListener.EndAcceptTcpClient(result);
		tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);
		Debug.Log($"Incoming connection from {client.Client.RemoteEndPoint}...");

		for (int i = 1; i <= MaxPlayers; i++)
		{
			if (clients[i].tcp.socket == null)
			{
				clients[i].tcp.Connect(client);
				return;
			}
		}

		Debug.Log($"{client.Client.RemoteEndPoint} failed to connect: Server full!");
	}

	private void UDPReceiveCallback(IAsyncResult result)
	{
		try
		{
			IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
			byte[] data = udpListener.EndReceive(result, ref clientEndPoint);
			udpListener.BeginReceive(UDPReceiveCallback, null);

			if (data.Length < 4)
			{
				return;
			}

			using (Packet packet = new Packet(data))
			{
				int clientId = packet.ReadInt();

				if (clientId == 0)
				{
					return;
				}

				if (clients[clientId].udp.endPoint == null)
				{
					clients[clientId].udp.Connect(clientEndPoint);
					return;
				}

				if (clients[clientId].udp.endPoint.ToString() == clientEndPoint.ToString())
				{
					clients[clientId].udp.HandleData(packet);
				}
			}
		}
		catch (Exception ex)
		{
			Debug.Log($"Error receiving UDP data: {ex}");
		}
	}

	public void SendUDPData(IPEndPoint clientEndPoint, Packet packet)
	{
		try
		{
			if (clientEndPoint != null)
			{
				udpListener.BeginSend(packet.ToArray(), packet.Length(), clientEndPoint, null, null);

			}
		}
		catch (Exception ex)
		{
			Debug.Log($"Error sending data to {clientEndPoint} via UDP: {ex}");
		}
	}

	private void InitializeServerData()
	{
		for (int i = 1; i <= MaxPlayers; i++)
		{
			clients.Add(i, new Client(i));
		}

		packetHandlers = new Dictionary<int, PacketHandler>()
			{
				{ (int)ClientPackets.welcomeReceived, ServerHandle.WelcomeReceived },
				{ (int)ClientPackets.playerMovement, ServerHandle.PlayerMovement }
			};
	}
}
