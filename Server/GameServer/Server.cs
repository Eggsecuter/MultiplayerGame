using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace GameServer
{
	class Server
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
		public delegate void PacketHandler(int _fromClient, Packet _packet);
		public Dictionary<int, PacketHandler> packetHandlers;

		private TcpListener tcpListener;

		public void Start(int _maxPlayers, int _port)
		{
			MaxPlayers = _maxPlayers;
			Port = _port;

			Console.WriteLine("Starting server...");
			InitializeServerData();

			tcpListener = new TcpListener(IPAddress.Any, Port);
			tcpListener.Start();
			tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);

			Console.WriteLine($"Server started on {Port}.");
		}

		private void TCPConnectCallback(IAsyncResult _result)
		{
			TcpClient _client = tcpListener.EndAcceptTcpClient(_result);
			tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);
			Console.WriteLine($"Incoming connection from {_client.Client.RemoteEndPoint}...");

			for (int i = 1; i <= MaxPlayers; i++)
			{
				if (clients[i].tcp.socket == null)
				{
					clients[i].tcp.Connect(_client);
					return;
				}
			}

			Console.WriteLine($"{_client.Client.RemoteEndPoint} failed to connect: Server full!");
		}

		private void InitializeServerData()
		{
			for (int i = 1; i <= MaxPlayers; i++)
			{
				clients.Add(i, new Client(i));
			}

			packetHandlers = new Dictionary<int, PacketHandler>()
			{
				{ (int)ClientPackets.welcomeReceived, ServerHandle.WelcomeReceived }
			};
		}
	}
}
