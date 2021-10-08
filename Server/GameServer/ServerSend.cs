using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer
{
	class ServerSend
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

		private void SendTCPData(int _toClient, Packet _packet)
		{
			_packet.WriteLength();
			Server.Instance.clients[_toClient].tcp.SendData(_packet);
		}

		private void BroadcastTCPData(Packet _packet)
		{
			_packet.WriteLength();
			
			for (int i = 1; i <= Server.Instance.MaxPlayers; i++)
			{
				Server.Instance.clients[i].tcp.SendData(_packet);
			}
		}

		private void BroadcastTCPData(Packet _packet, int _exeptClient)
		{
			_packet.WriteLength();

			for (int i = 1; i <= Server.Instance.MaxPlayers; i++)
			{
				if (i != _exeptClient)
				{
					Server.Instance.clients[i].tcp.SendData(_packet);
				}
			}
		}

		public void Welcome(int _toClient, string _msg)
		{
			using (Packet _packet = new Packet((int)ServerPackets.welcome))
			{
				_packet.Write(_msg);
				_packet.Write(_toClient);

				SendTCPData(_toClient, _packet);
			}
		}
	}
}
