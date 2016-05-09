// Matthew Cormack
// Star System UI element
// 02/04/16

using Otter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PA_MultiplayerGalacticWar.Entity
{
	class Entity_UIPanel_StarSystem : Otter.Entity
	{
		public static int Width = 256;
		public static int Height = 212;
		public static int HalfWidth = Width / 2;
		public static int HalfHeight = Height / 2;

		public string Label = "";
		public string SystemType = "";
		public Entity_StarSystem System = null;

		private Entity_Image Image_Background;
		private Text Text_Label;
		private Text Text_SystemType;
		private Text Text_SystemInfo;
		private Entity_UI_Button Button_Action;

		private Entity_PlayerArmy Army;

		public Entity_UIPanel_StarSystem( float x, float y, string name, string systemtype )
		{
			X = x;
			Y = y;

			Label = name;
			SystemType = systemtype;

			// Panel image
			Image_Background = new Entity_Image( X, Y, "", false );
			{
				Image_Background.NineSlice( Program.PATH_PA + "media/ui/main/shared/img/panel/img_menu_panel.png", 24, 24, Width, Height );

				Image_Background.image.Scroll = 0;
				//Image_Background.SetTarget( center );

				Image_Background.Layer = Helper.Layer_UI;
			}
			// System name
			Text_Label = new Text( Label, Program.Font, 20 );
			{
				Text_Label.CenterOrigin();
				Text_Label.SetPosition( new Vector2( 0, -80 ) );
				Text_Label.Scroll = 0;
				//Text_Label.Color = Color.Green;
			}
			Image_Background.AddGraphic( Text_Label );
			// System type (i.e. map)
			Text_SystemType = new Text( "(" + SystemType + ")", Program.Font, 16 );
			{
				Text_SystemType.CenterOrigin();
				Text_SystemType.SetPosition( new Vector2( 0, -62 ) );
				Text_SystemType.Scroll = 0;
				Text_SystemType.Color = Color.Gray;
			}
			Image_Background.AddGraphic( Text_SystemType );
			// System information (i.e. number of planets)
			Text_SystemInfo = new Text( "(Planets: N/A)", Program.Font, 12 );
			{
				Text_SystemInfo.CenterOrigin();
				Text_SystemInfo.SetPosition( new Vector2( 0, -48 ) );
				Text_SystemInfo.Scroll = 0;
				Text_SystemInfo.Color = Color.Shade( 0.7f );
			}
			Image_Background.AddGraphic( Text_SystemInfo );
			// Button Action
			Button_Action = new Entity_UI_Button();
			{
				Vector2 pos = new Vector2( X - Game.Instance.HalfWidth, Y - Game.Instance.HalfHeight + 72 );
				Button_Action.Offset.Y = -1;
				Button_Action.ButtonBounds = new Vector4( pos.X, pos.Y, 128, 48 );
				Button_Action.Scroll = new Vector2( 0, 0 );

				Button_Action.Layer = Helper.Layer_UI;
			}
			UpdateButton();

			// Add ui press collider
			AddCollider<BoxCollider>( new BoxCollider( Width, Height ) );
			Collider.CenterOrigin();
            Collider.SetPosition( -Game.Instance.HalfWidth, -Game.Instance.HalfHeight );

			Layer = Helper.Layer_UI;
        }

		public override void Added()
		{
			base.Added();

			Scene.Instance.Add( Image_Background );
			Scene.Instance.Add( Button_Action );

			Text_SystemInfo.String = "(Planets: " + StarSystemInfos.Infos[System.ID].Planets + ")";
        }

		public override void Update()
		{
			base.Update();

			// Update the collider to follow the camera
			Collider.SetPosition( -Game.Instance.HalfWidth + Scene.Instance.CameraCenterX, -Game.Instance.HalfHeight + Scene.Instance.CameraCenterY );
		}

		public override void Removed()
		{
			base.Removed();

			Scene.Instance.Remove( Image_Background );
			Scene.Instance.Remove( Button_Action );
		}

		private void UpdateButton()
		{
			// Move
			if ( ( Army == null ) || ( Army.Player != Program.ThisPlayer ) )
			{
				Button_Action.Colour_Default = Color.Green;
				Button_Action.Colour_Hover = Color.Green * Color.Shade( 0.8f );
				Button_Action.Label = "Move To";

				Button_Action.OnPressed = delegate ( Entity_UI_Button self )
				{
					self.Image.image.Color = self.Colour_Hover * Color.Gray;
					AudioManager.PlaySound( "resources/audio/ui_click.wav" );
				};
				Button_Action.OnReleased = delegate ( Entity_UI_Button self )
				{
					Entity_PlayerArmy.GetAllByPlayer( Program.ThisPlayer )[0].MoveToSystem( System );
					AudioManager.PlaySound( "resources/audio/ui_click.wav" );
				};
			}
			// WAR
			else
			{
				Button_Action.Colour_Default = Color.Red;
				Button_Action.Colour_Hover = Color.Red * Color.Shade( 0.8f );
				Button_Action.Label = "WAR";

				Button_Action.OnPressed = delegate ( Entity_UI_Button self )
				{
					self.Image.image.Color = self.Colour_Hover * Color.Gray;
					AudioManager.PlaySound( "resources/audio/ui_click.wav" );
				};
				Button_Action.OnReleased = delegate ( Entity_UI_Button self )
				{
					Console.WriteLine( "WAR " + Label );
					( (Scene_Game) Scene.Instance ).SaveGame();
					AudioManager.PlaySound( "resources/audio/ui_click.wav" );
				};
			}
		}

		public void UpdatePlayerArmy( Entity_PlayerArmy army )
		{
			// Update Flag
			Army = army;

			// Check if in scene, if so; update actual buttons
			UpdateButton();
		}
	}
}
