// Matthew Cormack
// Star System UI element
// 02/04/16

using Otter;

namespace PA_MultiplayerGalacticWar.Entity
{
	class Entity_UIPanel_StarSystem : Otter.Entity
	{
		#region Variable Declaration
		// Panel sizing
		public static int Width = 256;
		public static int Height = 212;
		public static int HalfWidth = Width / 2;
		public static int HalfHeight = Height / 2;

		// Visual information
		public string Label = "";
		public string SystemType = "";

		// Linked star system
		public Entity_StarSystem System = null;

		// Individual elements
		private Entity_Image Image_Background;
		private Text Text_Label;
		private Text Text_SystemType;
		private Text Text_SystemInfo;
		private Entity_UI_Button Button_Action;
		#endregion

		#region Initialise
		// Constructor: Initialise the individual elements
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
			Text_SystemType = new Text( "(SystemType)", Program.Font, 16 );
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

		// Added To Scene: Add individual elements
		public override void Added()
		{
			base.Added();

			Scene.Instance.Add( Image_Background );
			Scene.Instance.Add( Button_Action );
        }
		#endregion

		#region Update
		// Update In Scene: Update collider position to follow the camera
		public override void Update()
		{
			base.Update();

			// Update the collider to follow the camera
			Collider.SetPosition( -Game.Instance.HalfWidth + Scene.Instance.CameraCenterX, -Game.Instance.HalfHeight + Scene.Instance.CameraCenterY );
		}
		#endregion

		#region Cleanup
		// Removed From Scene: Remove individual elements
		public override void Removed()
		{
			base.Removed();

			Scene.Instance.Remove( Image_Background );
			Scene.Instance.Remove( Button_Action );
		}
		#endregion

		#region Alter Visuals
		// Called when the linked system's state changes (e.g. an army moves there)
		public void UpdateButton()
		{
			if ( System == null ) return;

			// Move
			if ( System.HasPlayerArmy == null )
			{
				Button_Action.Colour_Default = Color.Green;
				Button_Action.Colour_Hover = Color.Green * Color.Shade( 0.8f );
				Button_Action.Label = "MOVE TO";

				Button_Action.OnPressed = delegate ( Entity_UI_Button self )
				{
					self.Image.image.Color = self.Colour_Hover * Color.Gray;
					AudioManager.Instance.PlaySound( "resources/audio/ui_click.wav" );
				};
				Button_Action.OnReleased = delegate ( Entity_UI_Button self )
				{
					System.TryAction_Move();
					AudioManager.Instance.PlaySound( "resources/audio/ui_click.wav" );
				};
			}
			// WAR
			else
			{
				Button_Action.Colour_Default = Color.Red;
				Button_Action.Colour_Hover = Color.Red * Color.Shade( 0.8f );
				Button_Action.Label = "WAR";
				Button_Action.UpdateGraphic();

				Button_Action.OnPressed = delegate ( Entity_UI_Button self )
				{
					self.Image.image.Color = self.Colour_Hover * Color.Gray;
					AudioManager.Instance.PlaySound( "resources/audio/ui_click.wav" );
				};
				Button_Action.OnReleased = delegate ( Entity_UI_Button self )
				{
					System.TryAction_War();
					AudioManager.Instance.PlaySound( "resources/audio/ui_click.wav" );
				};
			}
		}

		// Called when the system visual information is loaded/updated
		public void UpdateText()
		{
			Text_Label.String = Label;
			Text_Label.CenterOrigin();

			Text_SystemType.String = "(" + SystemType + ")";
			Text_SystemType.CenterOrigin();

			Text_SystemInfo.String = "(Planets: " + StarSystemInfos.Infos[System.MapID].Planets + ")";
			Text_SystemInfo.CenterOrigin();
		}
		#endregion
	}
}
