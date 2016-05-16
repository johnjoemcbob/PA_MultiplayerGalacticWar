using Otter;
using System;
using System.Text;
using Lidgren.Network;

namespace PA_MultiplayerGalacticWar
{
	class NetworkManager
	{
		public const int MESSAGE_INITIAL = 0;
		public const int MESSAGE_PLAYERID = 1;
		public const int MESSAGE_TURN_REQUEST = 2;
		public const int MESSAGE_TURN_CONFIRM = 3;

		static public string ApplicationName = "Galactic War";
		static public int Port = 25001;

		static public NetPeerConfiguration Config = null;
		static public NetPeer NetworkHandler = null;
		static public bool Server = false;

		static public void Host()
		{
			Config = new NetPeerConfiguration( ApplicationName );
			{
				Config.Port = Port;
			}
			NetworkHandler = new NetServer( Config );
			NetworkHandler.Start();

			Server = true;
        }

		static public void Connect( string ip )
		{
			Config = new NetPeerConfiguration( ApplicationName );
			NetworkHandler = new NetClient( Config );
			{
				NetworkHandler.Start();
				NetworkHandler.Connect( ip, Port );
			}

			Server = false;
        }

		static public void Update()
		{
			// Check peers for messages
			foreach ( NetConnection connection in NetworkHandler.Connections )
			{
				NetPeer peer = connection.Peer;

				NetIncomingMessage message;
				while ( ( message = peer.ReadMessage() ) != null )
				{
					ParseMessage( message );
				}
			}
		}

		static private void ParseMessage( NetIncomingMessage message )
		{
			Console.WriteLine( "Received message with type: " + message.MessageType );

			switch ( message.MessageType )
			{
				case NetIncomingMessageType.Data:
					ParseMessage_Data( message );
                    break;

				case NetIncomingMessageType.StatusChanged:
					ParseMessage_Status( message );
					break;

				case NetIncomingMessageType.DebugMessage:
					// handle debug messages
					// (only received when compiled in DEBUG mode)
					Console.WriteLine( message.ReadString() );
					break;

				/* .. */
				default:
					Console.WriteLine( "unhandled message with type: "
						+ message.MessageType );
					break;
			}
		}

		static private void ParseMessage_Data( NetIncomingMessage message )
		{
			// Handle custom messages
			var data = message.Data;
			string datastr = Encoding.Default.GetString( data );
            //Console.WriteLine( "Data: " + datastr );

			// Get message id from the first char of the message
			int id = -1;
			{
				// Find message id
				int index = 0;
				bool parsed = false;
				while ( !parsed )
				{
					parsed = int.TryParse( datastr.ToCharArray()[index].ToString(), out id );
					index++;
					if ( index > 10 )
					{
						Console.WriteLine( "Message Parse Failed on: " + datastr );
						return;
					}
				}
				// Clip any garbage from the start
				datastr = datastr.Substring( index - 1 );
			}
			Console.WriteLine( "Data ID: " + id );

			// Parse message
			Console.WriteLine( "" );
			string datacore = datastr.Substring( 2 );
			switch ( id )
			{
				case MESSAGE_INITIAL:
					//Console.WriteLine( "JSON: " + datacore );
					( (Scene_Game) Scene.Instance ).LoadFromJSON( datacore );

					break;
				case MESSAGE_PLAYERID:
					int num = int.Parse( datacore.ToCharArray()[0].ToString() );
                    Console.WriteLine( "Player: " + num );
					Program.ThisPlayer = num;

					break;
				case MESSAGE_TURN_REQUEST:
					

					break;
				default:
					break;
			}
		}

		static private void ParseMessage_Status( NetIncomingMessage message )
		{
			switch ( message.SenderConnection.Status )
			{
				case NetConnectionStatus.Connected:
					// Server sends initial game state
					if ( Server )
					{
						SendInitialState( message.SenderConnection );
					}
					break;
				default:
					break;
					/* .. */
			}
		}

		static private void Send( NetConnection sendto, int id, string data )
		{
			NetOutgoingMessage msg = NetworkHandler.CreateMessage();
			{
				msg.Write( id.ToString() + " " + data );
			}
			NetworkHandler.SendMessage( msg, sendto, NetDeliveryMethod.ReliableOrdered );
		}

		static private void SendInitialState( NetConnection sendto )
		{
			Info_Game game = ( (Scene_Game) Scene.Instance ).CurrentGame;
			Send( sendto, MESSAGE_PLAYERID, NetworkHandler.ConnectionsCount.ToString() );
			Send( sendto, MESSAGE_INITIAL, game.GetNetworkString() );
        }

		static private void SendTurnRequest( NetConnection sendto )
		{
			Info_Game game = ( (Scene_Game) Scene.Instance ).CurrentGame;
			Send( sendto, MESSAGE_INITIAL, game.GetNetworkString() );
		}
	}
}
