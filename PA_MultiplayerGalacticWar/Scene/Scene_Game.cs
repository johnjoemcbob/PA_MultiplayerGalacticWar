// Matthew Cormack
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
		private Thread Thread_Load = null;
		private Thread Thread_Save = null;
		private bool FileToSave = true;

		// Flag to quit after pressing escape & after saving
		private bool Quit = false;

		private List<Entity_UIPanel_Card> PossibleCards = new List<Entity_UIPanel_Card>();

		private Entity_UIPanel_FileIO LoadUI = null;

		private Entity_Galaxy Galaxy;

		// Zooming
		private float Zoom = 1;
		private float ZoomTarget = 1;

		public Scene_Game( string load = "" )
		{
			Filename = load;
			FileToLoad = load;
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

			// Threading
			Thread_Load = new Thread( new ThreadStart( this.ThreadLoadGame ) );
			FileToSave = true;

			Game.Instance.QuitButton.Clear();

			// Cursor last
			Add( new Entity_Cursor( Program.PATH_PA + "media/ui/main/shared/img/icons/cursor.png" ) );
		}

		private void Initialise()
		{
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
			if ( Thread_Load.IsAlive || ( ( Thread_Save != null ) && Thread_Save.IsAlive ) ) return;
			if ( ( FileToLoad == null ) && LoadUI.IsInScene )
			{
				Remove( LoadUI );
				if ( Thread_Save != null )
				{
					Thread_Save.Abort();
				}
				else
				{
					Initialise();
				}
			}
			if ( !FileToSave && Quit )
			{
				// Change state
				Game.Instance.RemoveScene();
				Game.Instance.AddScene( new Scene_ChooseGame() );
			}

			TryLoadGame();
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
					CommanderType commander;
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
					}
					CurrentGame.Commanders.Add( commander );
				}
			}
			SaveGame();

			SetupGame();
		}

		private void TryLoadGame()
		{
			if ( ( FileToLoad == null ) || ( Thread_Load.IsAlive ) ) return;

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
					LoadUI = new Entity_UIPanel_FileIO();
                    Add( LoadUI );

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
			CurrentGame = JsonConvert.DeserializeObject<Info_Game>( Helper.ReadFile( FileToLoad ) );
			FileToLoad = null;

			SetupGame();

			foreach ( Info_Player player in CurrentPlayers )
			{
				ApplyCards( player, player.Commander.CommanderCards, player.Commander.Cards );
			}
		}

		public void SaveGame()
		{
			if ( ( Thread_Save != null ) && Thread_Save.IsAlive ) return;

			// Add the saving text to the screen
			LoadUI = new Entity_UIPanel_FileIO();
			{
				LoadUI.Label = "Saving";
			}
			Add( LoadUI );

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

			// Write back out
			StreamWriter writer = new StreamWriter( Filename );
			{
				writer.WriteLine( JsonConvert.SerializeObject( CurrentGame ) );
			}
			writer.Close();

			FileToSave = false;
        }

		private void SetupGame()
		{
			Info_Player.SetupAllArmy();
			int playerid = 0;
            foreach ( CommanderType com in CurrentGame.Commanders )
			{
				Entity_StarSystem start = Galaxy.GetRandomSystem();

				Info_Player player = new Info_Player();
				{
					player.Commander = com;
					player.CommanderPath = Program.PATH_COMMANDER[com.ModelID];

					// Add their initial army
					Entity_PlayerArmy army = new Entity_PlayerArmy();
					{
						army.Player = playerid;
						army.MoveToSystem( start );
					}
					player.Armies.Add( army );
				}
				player.Initialise();
                CurrentPlayers.Add( player );
				Scene.Instance.Add( player.Armies[0] );
				start.SetOwner( player.Armies[0].Player );

				playerid++;
            }
		}
	}
}
