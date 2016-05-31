﻿// Matthew Cormack
// Main game scene; play the current game
// 30/03/16

using Otter;
using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using Newtonsoft.Json;

using PA_MultiplayerGalacticWar.Entity;

namespace PA_MultiplayerGalacticWar
{
	class Scene_Game : Scene
	{
		public Info_Game CurrentGame;
		public List<Info_Player> CurrentPlayers = new List<Info_Player>();

		// File input & output
		private string Filename = "";
		private string FileToLoad = "";
		private string JSONToLoad = "";
		private Thread Thread_Load = null;
		private Thread Thread_Save = null;
		private bool FileToSave = true;
		private bool IsNewGame = false;

		// Networking
		private string IPToConnect = "";

		// Flag to quit after pressing escape & after saving
		private bool Quit = false;

		private List<Entity_UIPanel_Card> PossibleCards = new List<Entity_UIPanel_Card>();

		// UI
		private Entity_UIPanel_TurnSwitch UI_TurnSwitch = new Entity_UIPanel_TurnSwitch();
		private Entity_UIPanel_FileIO UI_Load = null;

		private Entity_Galaxy Galaxy;

		// Zooming
		private float Zoom = 1;
		private float ZoomTarget = 1;

		public Scene_Game( string load = "" )
		{
			Filename = load;
			FileToLoad = load;
		}
		public Scene_Game( string ip, bool join )
		{
			IPToConnect = ip;
		}

		public override void Begin()
		{
			base.Begin();

			// Setup galaxy & star system information: BEFORE LOADING
			Galaxy = new Entity_Galaxy( this, Program.PATH_PA, Program.PATH_MOD );

			// Temp
			PossibleCards.Add( new Entity_UIPanel_Card(
				-256, 0,
				"Foreman Commander",
				"Your commander is specialised in\nproducing metal.\n\nGain a 100% increase to base metal\nproduction."
			) );
			PossibleCards.Add( new Entity_UIPanel_Card( 0, 0, "Tech Storage", "Gain an extra slot to store new\ntechnology in." ) );
			PossibleCards.Add( new Entity_UIPanel_Card( 256, 0, "Powerhouse Commander", "Your commander is specialised in\nproducing energy.\n\nGain a 50% increase to base energy\nproduction." ) );
			foreach ( Entity_UIPanel_Card card in PossibleCards )
			{
				Add( card );
			}

			// Load thread
			if ( FileToLoad != "" )
			{
				Thread_Load = new Thread( new ThreadStart( this.ThreadLoadGame ) );
				FileToSave = true;
			}
			// Connect to server
			if ( IPToConnect != "" )
			{
				CurrentGame = new Info_Game();
				NetworkManager.Connect( IPToConnect );
			}

			Game.Instance.QuitButton.Clear();

			// Cursor last
			Add( new Entity_Cursor( Program.PATH_PA + "media/ui/main/shared/img/icons/cursor.png" ) );
		}

		private void TryInitialise()
		{
			if ( Galaxy.IsInScene ) return;

			Add( Galaxy );
		}

		public override void UpdateFirst()
		{
			base.UpdateFirst();

			Entity_UI_Button.GlobalUpdate();
		}

		public override void Update()
		{
			base.Update();

			NetworkManager.Update();

			UpdateFileIO();
			//UpdateZoom();

			if ( Game.Instance.Input.KeyPressed( Key.Escape ) )
			{
				// Launch saving thread, scene will be switched when the thread joins
				SaveGame();
				Quit = true;
			}
		}

		private void UpdateFileIO()
		{
			// Loading thread & UI
			if ( ( ( Thread_Load != null ) && Thread_Load.IsAlive ) || ( ( Thread_Save != null ) && Thread_Save.IsAlive ) ) return;
			if ( ( FileToLoad == null ) && UI_Load.IsInScene )
			{
				Remove( UI_Load );
				if ( Thread_Save != null )
				{
					Thread_Save.Abort();
				}
			}
			if ( !FileToSave )
			{
				Remove( UI_Load );

				if ( Quit )
				{
					// Change state
					Game.Instance.RemoveScene();
					Game.Instance.AddScene( new Scene_ChooseGame() );
				}
			}
			TryInitialise();

			TryInitialLoadGame();
		}

		private void UpdateZoom()
		{
			// Update the camera's zoom
			if ( Input.MouseWheelDelta != 0 )
			{
				ZoomTarget += ( Input.MouseWheelDelta / 10 );
				ZoomTarget = Math.Max( 1, Math.Min( 2, ZoomTarget ) );
			}
			Zoom += ( ZoomTarget - Zoom ) * Game.Instance.DeltaTime;
			Scene.Instance.CameraZoom = Zoom;
		}

		public void PickCard( Entity_UIPanel_Card pickedcard )
		{
			// Remove all other cards
			foreach ( Entity_UIPanel_Card card in PossibleCards )
			{
				if ( ( card != null ) && ( card.Scene == this ) )
				{
					Remove( card );
				}
			}

			// Apply this card to the winning player
			//Program.AddCard(  );
		}

		private void ApplyCards( Info_Player player, List<string> commandercards, List<string> cards )
		{
			player.AddCommanderCards( commandercards );
			player.AddArmyCards( cards );
		}

		private void FinishCards()
		{
			Info_Player.EndSetupArmy( Program.PATH_PA + "media/pa/units/" );
		}

		private void NewGame()
		{
			// Create new
			CurrentGame = new Info_Game();
			{
				CurrentGame.StartDate = DateTime.Today.ToShortDateString();
				CurrentGame.Players = 2;
				CurrentGame.Commanders = new List<CommanderType>();
				for ( int com = 0; com < CurrentGame.Players; com++ )
				{
					CommanderType commander = new CommanderType();
					{
						commander.PlayerName = "Matthew";
						commander.UberID = "\"7420260152538080746\""; //"friends":"[\"11035761434068835310\",\"16024636805495278681\",\"6884095390246756552\"]",
						commander.FactionName = "Legion";
						commander.ModelID = 0;
						commander.Colour = Color.Random.ColorString;
						commander.CommanderCards = new List<string>();
						{
							commander.CommanderCards.Add( "Miner Upgrade" );
						}
						commander.Cards = new List<string>();
						{
							commander.Cards.Add( "Advanced Bots" );
						}
						commander.Armies = new List<ArmyType>();
						{
							ArmyType army;
							{
								army.Name = "";
								army.Strength = 1;
								army.SystemPosition = Rand.Int( 0, Galaxy.MaxSystems );
							}
							commander.Armies.Add( army );

							// Store initial state
							SetPlayerTurn( com );
							DoTurn( Helper.ACTION_MOVE, army.SystemPosition, com, 0, false );
						}
					}
					CurrentGame.Commanders.Add( commander );

					// Revert back to player 1 having first turn
					SetPlayerTurn( 0 );
				}

				// Generate the initial galaxy
				Galaxy.Generate();
            }
			Galaxy.Initialise();
			Galaxy.Generate_Info();
			SetupGame();

			// Add ownership of the starting locations to those players
			int playerid = 0;
			foreach ( CommanderType player in CurrentGame.Commanders )
			{
				foreach ( ArmyType army in player.Armies )
				{
					Galaxy.GetSystemByID( army.SystemPosition ).SetOwner( playerid );
				}
				playerid++;
			}

			IsNewGame = true;
			SaveGame();
		}

		private void TryInitialLoadGame()
		{
			if ( ( FileToLoad == null ) || ( IPToConnect != "" ) || ( ( Thread_Load != null ) && Thread_Load.IsAlive ) || ( Thread_Save != null ) ) return;

			if ( FileToLoad == "" )
			{
				NewGame();
			}
			else
			{
				// Check for a save
				if ( File.Exists( FileToLoad ) )
				{
					// Add the loading text to the screen
					UI_Load = new Entity_UIPanel_FileIO();
                    Add( UI_Load );

					// Start at thread to load the content
					Thread_Load.Start();
                }
				else
				{
					// Return to menu
					Game.Instance.RemoveScene();
					Game.Instance.AddScene( new Scene_ChooseGame() );
				}
			}
		}

		public void ThreadLoadGame()
		{
			string json = JSONToLoad;
            if ( JSONToLoad == "" )
			{
				json = Helper.ReadFile( FileToLoad );
			}
			CurrentGame = JsonConvert.DeserializeObject<Info_Game>( json );
			FileToLoad = null;

			Galaxy.Initialise( CurrentGame.Galaxy );
			SetupGame();
			Galaxy.InitialisePlayers();

			foreach ( Info_Player player in CurrentPlayers )
			{
				ApplyCards( player, player.Commander.CommanderCards, player.Commander.Cards );
			}
		}

		public void LoadFromJSON( string json )
		{
			// Add the loading text to the screen
			UI_Load = new Entity_UIPanel_FileIO();
			Add( UI_Load );

			// Start at thread to load the content
			JSONToLoad = json;
			if ( Thread_Load == null )
			{
				Thread_Load = new Thread( new ThreadStart( this.ThreadLoadGame ) );
			}
			Thread_Load.Start();
		}

		public void SaveGame()
		{
			if ( ( Thread_Save != null ) && Thread_Save.IsAlive ) return;

			// Add the saving text to the screen
			UI_Load = new Entity_UIPanel_FileIO();
			{
				UI_Load.Label = "Saving";
			}
			Add( UI_Load );

			// Start at thread to load the content
			Thread_Save = new Thread( new ThreadStart( this.ThreadSaveGame ) );
			Thread_Save.Start();
		}

		public void ThreadSaveGame()
		{
			// Save player states
			foreach ( Info_Player player in CurrentPlayers )
			{
				player.SaveCommander();
				player.SaveArmy();
			}

			// Save game state
			if ( !Directory.Exists( "data/" ) )
			{
				Directory.CreateDirectory( "data/" );
			}
			if ( Filename == "" )
			{
				int num = Directory.GetFiles( "data/" ).Length + 1;
				Filename = "data/game" + num + ".json";
			}

			// Get the new info about each player
			CurrentGame.SavePlayers( CurrentPlayers, !IsNewGame );

			// Get the new info about the galaxies
			CurrentGame.SaveGalaxy( Galaxy );

			// Write back out
			StreamWriter writer = new StreamWriter( Filename );
			{
				writer.WriteLine( JsonConvert.SerializeObject( CurrentGame ) );
			}
			writer.Close();

			FileToSave = false;
			IsNewGame = false;
		}

		private void SetupGame()
		{
			Info_Player.SetupAllArmy();
			int playerid = 0;
            foreach ( CommanderType com in CurrentGame.Commanders )
			{
				Info_Player player = new Info_Player();
				{
					player.Commander = com;
					player.CommanderPath = Program.PATH_COMMANDER[com.ModelID];
				}
				player.Initialise();
				CurrentPlayers.Add( player );

				// Load their armies
				if ( player.Commander.Armies != null )
				{
					foreach ( ArmyType armytype in player.Commander.Armies )
					{
						Entity_PlayerArmy army = new Entity_PlayerArmy();
						{
							army.Player = playerid;
							army.MoveToSystem( Galaxy.GetSystemByID( armytype.SystemPosition ) );
						}
						player.Armies.Add( army );
						Scene.Instance.Add( army );
					}
				}

				// NOTE: Depends on player being in the CurrentPlayers list first
				// Load the owned systems from the json into the galaxy map
				if ( player.Commander.OwnedSystems != null )
				{
					foreach ( int systemid in player.Commander.OwnedSystems )
					{
						Entity_StarSystem system = Galaxy.GetSystemByID( systemid );
						system.SetOwner( playerid );
					}
				}

				playerid++;
            }
		}

		public void DoTurn( int action, int systemid, int player, int armyid, bool doturn = true )
		{
			// Actually perform the turn if received confirmation
			if ( doturn )
			{
				Entity_PlayerArmy army = Helper.GetPlayerArmy( player, armyid );
				Entity_StarSystem system = Galaxy.GetSystemByID( systemid );
				switch ( action )
				{
					case Helper.ACTION_MOVE:
						system.Action_Move( army );
						break;

					case Helper.ACTION_WAR:
						system.Action_War( army );
						break;

					default:
						break;
				}
            }

			// Add to turn history
			if ( CurrentGame.TurnHistory == null )
			{
				CurrentGame.TurnHistory = new List<TurnType>();
            }

			TurnType turn;
			{
				turn.ActionID = action;
				turn.Player = player;
				turn.Data = systemid + " " + armyid;
			}
			CurrentGame.TurnHistory.Add( turn );
        }
		public int GetPlayerTurn()
		{
			return CurrentGame.CurrentTurn;
        }
		public bool GetIsPlayerTurn( int player )
		{
			return ( GetPlayerTurn() == player );
		}
		public void SetNextPlayerTurn()
		{
			CurrentGame.CurrentTurn++;
			CurrentGame.Turns++;
			if ( CurrentGame.CurrentTurn >= CurrentGame.Players )
			{
				CurrentGame.CurrentTurn = 0;
			}
			SetPlayerTurn( CurrentGame.CurrentTurn );
        }
		public void SetPlayerTurn( int turn )
		{
			CurrentGame.CurrentTurn = turn;
			PlayTurnSwitch();

			// temp
			//Program.ThisPlayer = CurrentGame.CurrentTurn;
			//foreach ( Entity_StarSystem system in Galaxy.GetSystems() )
			//{
			//	system.CheckVisibility();
			//	system.SetSelected( false );
			//}
		}
		public void PlayTurnSwitch()
		{
			// Show turn switch UI
			if ( UI_TurnSwitch.IsInScene )
			{
				//Remove( UI_TurnSwitch );
			}
			Add( UI_TurnSwitch );
			UI_TurnSwitch.Initialise();
		}
    }
}
