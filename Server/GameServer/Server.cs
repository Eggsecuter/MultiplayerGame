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
		private UdpClient udpListener;

		public void Start(int _maxPlayers, int _port)
		{
			MaxPlayers = _maxPlayers;
			Port = _port;

			Console.WriteLine("Starting server...");
			InitializeServerData();

			tcpListener = new TcpListener(IPAddress.Any, Port);
			tcpListener.Start();
			tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);

			udpListener = new UdpClient(Port);
			udpListener.BeginReceive(UDPReceiveCallback, null);

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

		private void UDPReceiveCallback(IAsyncResult _result)
		{
			try
			{
				IPEndPoint _clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
				byte[] _data = udpListener.EndReceive(_result, ref _clientEndPoint);
				udpListener.BeginReceive(UDPReceiveCallback, null);

				if (_data.Length < 4)
				{
					return;
				}

				using (Packet _packet = new Packet(_data))
				{
					int _clientId = _packet.ReadInt();

					if (_clientId == 0)
					{
						return;
					}

					if (clients[_clientId].udp.endPoint == null)
					{
						clients[_clientId].udp.Connect(_clientEndPoint);
						return;
					}

					if (clients[_clientId].udp.endPoint.ToString() == _clientEndPoint.ToString())
					{
						clients[_clientId].udp.HandleData(_packet);
					}
				}
			}
			catch (Exception _ex)
			{
				Console.WriteLine($"Error receiving UDP data: {_ex}");
			}
		}

		public void SendUDPData(IPEndPoint _clientEndPoint, Packet _packet)
		{
			try
			{
				if (_clientEndPoint != null)
				{
					udpListener.BeginSend(_packet.ToArray(), _packet.Length(), _clientEndPoint, null, null);

				}
			}
			catch (Exception _ex)
			{
				Console.WriteLine($"Error sending data to {_clientEndPoint} via UDP: {_ex}");
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
				{ (int)ClientPackets.udpTestReceived, ServerHandle.UDPTestReceived }
			};
		}
	}
}
