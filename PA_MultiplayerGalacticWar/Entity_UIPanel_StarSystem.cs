// Matthew Cormack
// Star System UI element
// 02/04/16

using Otter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PA_MultiplayerGalacticWar
{
	class Entity_UIPanel_StarSystem : Entity
	{
		public static int Width = 196;
		public static int Height = 212;
		public static int HalfWidth = Width / 2;
		public static int HalfHeight = Height / 2;

		public string Label = "";
		public string SystemType = "";
		public int SystemID = 0;

		private Entity_Image Image_Background;
		private Text Text_Label;
		private Text Text_SystemType;
		private Text Text_SystemInfo;
		private Entity_UI_Button Button_War;

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
			Text_SystemInfo = new Text( "(Planets: " + StarSystemInfos.Infos[SystemID].Planets + ")", Program.Font, 12 );
			{
				Text_SystemInfo.CenterOrigin();
				Text_SystemInfo.SetPosition( new Vector2( 0, -48 ) );
				Text_SystemInfo.Scroll = 0;
				Text_SystemInfo.Color = Color.Shade( 0.7f );
			}
			Image_Background.AddGraphic( Text_SystemInfo );
			// Button WAR
			Button_War = new Entity_UI_Button();
			{
				Button_War.Colour_Default = Color.Red;
				Button_War.Colour_Hover = Color.Red * Color.Shade( 0.8f );
				Button_War.Label = "WAR";

				Vector2 pos = new Vector2( X - Game.Instance.HalfWidth, Y - Game.Instance.HalfHeight + 72 );
				Button_War.Offset.Y = -1;
				Button_War.ButtonBounds = new Vector4( pos.X, pos.Y, 128, 48 );
				Button_War.Scroll = new Vector2( 0, 0 );

				Button_War.OnPressed = delegate ( Entity_UI_Button self )
				{
					self.Image.image.Color = self.Colour_Hover * Color.Gray;
					AudioManager.PlaySound( "resources/audio/ui_click.wav" );
				};
				Button_War.OnReleased = delegate ( Entity_UI_Button self )
				{
					Console.WriteLine( "WAR " + Label );
					( (Scene_Game) Scene.Instance ).SaveGame();
					AudioManager.PlaySound( "resources/audio/ui_click.wav" );
				};
			}

			// Add ui press collider
			AddCollider<BoxCollider>( new BoxCollider( Width, Height ) );
			Collider.CenterOrigin();
            Collider.SetPosition( -Game.Instance.HalfWidth, -Game.Instance.HalfHeight );
		}

		public override void Added()
		{
			base.Added();

			Scene.Instance.Add( Image_Background );
			Scene.Instance.Add( Button_War );
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
			Scene.Instance.Remove( Button_War );
		}
	}
}
