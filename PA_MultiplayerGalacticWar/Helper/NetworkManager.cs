using Otter;
using System;
using System.Net;
using System.Net.NetworkInformation;
using Newtonsoft.Json;
using Lidgren.Network;

namespace PA_MultiplayerGalacticWar
{
	struct NetworkTurnType
	{
		public int Action;
		public int System;
		public int Player;
		public int Army;
	};

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
			if ( IsPortInUse( Port ) )
			{
				Console.WriteLine( "Port " + Port + " is unavailable!" );
				return;
			}

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
			if ( NetworkHandler == null ) return;

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

		// Called by cleanup to clear any connections and sockets
		static public void Cleanup()
		{
			if ( NetworkHandler == null ) return;

			NetworkHandler.Shutdown( "Left Game" );
			NetworkHandler = null;
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
			NetworkHandler.Recycle( message );
		}

		static private void ParseMessage_Data( NetIncomingMessage message )
		{
			// Handle custom messages
			int id = message.ReadInt32();
			string datastr;

			// Parse message
			switch ( id )
			{
				case MESSAGE_INITIAL:
					datastr = message.ReadString();
					( (Scene_Game) Scene.Instance ).LoadFromJSON( datastr );

					break;
				case MESSAGE_PLAYERID:
					int playerid = message.ReadInt16();
					Program.ThisPlayer = playerid;

					break;
				case MESSAGE_TURN_REQUEST:
					datastr = message.ReadString();
					NetworkTurnType turn = JsonConvert.DeserializeObject<NetworkTurnType>( datastr );
					SendTurnConfirm( turn.Action, turn.System, turn.Player, turn.Army );

					break;
				case MESSAGE_TURN_CONFIRM:
					datastr = message.ReadString();
					NetworkTurnType turnconfirm = JsonConvert.DeserializeObject<NetworkTurnType>( datastr );
					Helper.GetGameScene().DoTurn( turnconfirm.Action, turnconfirm.System, turnconfirm.Player, turnconfirm.Army );

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

		static public void SendMessage( NetConnection sendto, NetOutgoingMessage msg )
		{
			NetworkHandler.SendMessage( msg, sendto, NetDeliveryMethod.ReliableOrdered );
		}

		static public NetOutgoingMessage StartDefaultMessage( int id )
		{
			NetOutgoingMessage msg = NetworkHandler.CreateMessage();
			{
				msg.Write( id );
			}
			return msg;
		}

		static public void SendString( NetConnection sendto, int id, string data )
		{
			NetOutgoingMessage msg = StartDefaultMessage( id );
			{
                msg.Write( (string) data );
			}
			SendMessage( sendto, msg );
        }

		static public void SendInitialState( NetConnection sendto )
		{
			Info_Game game = ( (Scene_Game) Scene.Instance ).CurrentGame;

			// Send the player ID
			NetOutgoingMessage msg_playerid = StartDefaultMessage( MESSAGE_PLAYERID );
			{
				msg_playerid.Write( NetworkHandler.ConnectionsCount );
            }
			SendMessage( sendto, msg_playerid );

			// Send the initial world state
			SendString( sendto, MESSAGE_INITIAL, game.GetNetworkString() );
        }

		static public void SendTurnRequest( int action, int system, int player, int playerarmy )
		{
			if ( NetworkHandler == null ) return;

			if ( Server )
			{
				// Server host has no need to confirm, skip straight to sending the confirmed turns to all clients
				Console.WriteLine( "HOST is skipping turn request" );
				SendTurnConfirm( action, system, player, playerarmy );
            }
			else
			{
				// Client sends to server to confirm action
				Console.WriteLine( "CLIENT is doing turn request" );
				NetworkTurnType turn = new NetworkTurnType();
				{
					turn.Action = action;
					turn.System = system;
					turn.Player = player;
					turn.Army = playerarmy;
				}
				string turndata = JsonConvert.SerializeObject( turn );
                foreach ( NetConnection connection in NetworkHandler.Connections )
				{
					SendString( connection, MESSAGE_TURN_REQUEST, turndata );
				}
			}
		}

		static public void SendTurnConfirm( int action, int system, int player, int playerarmy )
		{
			// Extra logic on server host to also confirm the turn on their own game client
			if ( Server )
			{
				Helper.GetGameScene().DoTurn( action, system, player, playerarmy );
            }
			NetworkTurnType turn = new NetworkTurnType();
			{
				turn.Action = action;
				turn.System = system;
				turn.Player = player;
				turn.Army = playerarmy;
			}
			string turndata = JsonConvert.SerializeObject( turn );
			foreach ( NetConnection connection in NetworkHandler.Connections )
			{
				SendString( connection, MESSAGE_TURN_CONFIRM, turndata );
			}
		}

		// From: https://softwarebydefault.com/2013/02/22/port-in-use/
		// Used to determine if a port is available, to stop crashes when hosting on an unavailable port
		public static bool IsPortInUse( int port )
		{
			bool inuse = false;
			{
				IPGlobalProperties ipproperties = IPGlobalProperties.GetIPGlobalProperties();
				IPEndPoint[] ipendpoints = ipproperties.GetActiveUdpListeners();

				foreach ( IPEndPoint endPoint in ipendpoints )
				{
					if ( endPoint.Port == port )
					{
						inuse = true;
						break;
					}
				}
			}
			return inuse;
		}
	}
}
