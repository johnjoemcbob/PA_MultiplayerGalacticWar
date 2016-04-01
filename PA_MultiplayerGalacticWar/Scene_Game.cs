// Matthew Cormack
// Main game scene; play the current game
// 30/03/16

using Otter;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PA_MultiplayerGalacticWar
{
	class Scene_Game : Scene
	{
		public Info_Game CurrentGame;

		private string Filename = "";
		private string FileToLoad = "";

		public Scene_Game( string load = "" )
		{
			Filename = load;
			FileToLoad = load;
		}

		public override void Begin()
		{
			base.Begin();

			Add( new Entity_Galaxy( this, Program.PATH_PA, Program.PATH_MOD ) );

			// Temp
			Add( new Entity_Card( -256, 0, "Foreman Commander", "Your commander is specialised in\nproducing metal.\n\nGain a 100% increase to base metal\nproduction." ) );
			Add( new Entity_Card( 0, 0, "Tech Storage", "Gain an extra slot to store new\ntechnology in." ) );
			Add( new Entity_Card( 256, 0, "Powerhouse Commander", "Your commander is specialised in\nproducing energy.\n\nGain a 50% increase to base energy\nproduction." ) );

			Game.Instance.QuitButton.Clear();
		}

		public override void End()
		{
			base.End();

			SaveGame();
		}

		public override void UpdateFirst()
		{
			base.UpdateFirst();

			Entity_UI_Button.GlobalUpdate();
		}

		public override void Update()
		{
			base.Update();

			LoadGame();

			if ( Game.Instance.Input.KeyPressed( Key.Escape ) )
			{
				Game.Instance.RemoveScene();
				Game.Instance.AddScene( new Scene_ChooseGame() );
			}
		}

		private void LoadGame()
		{
			if ( FileToLoad == null ) return;

			if ( FileToLoad == "" )
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
				FileToLoad = null;
			}
			else
			{
				if ( File.Exists( FileToLoad ) )
				{
					// Load
					CurrentGame = JsonConvert.DeserializeObject<Info_Game>( Program.ReadJSON( FileToLoad ) );
				}
				else
				{
					// Return to menu
					Game.Instance.RemoveScene();
					Game.Instance.AddScene( new Scene_ChooseGame() );
				}
				FileToLoad = null;
			}

			ApplyCards();
		}

		private void ApplyCards()
		{
			foreach ( CommanderType commander in CurrentGame.Commanders )
			{
				Program.SetupCommander( Program.PATH_COMMANDER[commander.ModelID], commander.CommanderCards );
				Program.SetupArmy( Program.PATH_PA + "media/pa/units/", commander.ModelID, commander.Cards );
            }
			Program.EndSetupArmy( Program.PATH_PA + "media/pa/units/" );
        }

		private void SaveGame()
		{
			if ( !Directory.Exists(  "data/" ) )
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
		}
	}
}
