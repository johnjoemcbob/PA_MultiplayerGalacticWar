// Matthew Cormack
// Unlock Card UI element
// 01/04/16

#region Includes
using Otter;
using System;
using Newtonsoft.Json.Linq;
#endregion

namespace PA_MultiplayerGalacticWar.Entity
{
	class Entity_UIPanel_Card : Otter.Entity
	{
		#region Variable Declaration
		// Card visual information
		public string Label = "Test Card";
		public string Description = "Description Text\nYeah!";
		public string IconFile = "";
		public JObject JSONData = null;

		// Card individual elements
		private Entity_Image Image_Background;
		private Entity_Image Image_Icon;
		private Entity_Image Image_Button_Background;
		private Text Text_Label;
		private RichText Text_Description;
		private Entity_UI_Button Button_Choose;
		#endregion

		#region Initialise
		// Constructor: Position the card & initialise
		public Entity_UIPanel_Card( float x, float y, string label = "Test Card", string description = "Description", string iconfile = "", JObject data = null )
		{
			X = x;
			Y = y;
			Initialise( label, description, iconfile, data );
        }

		// Constructor: Initialise
		public Entity_UIPanel_Card( string label = "Test Card", string description = "Description", string iconfile = "", JObject data = null )
		{
			Initialise( label, description, iconfile, data );
		}

		// Called from constructor: Initialise visual information variables
		private void Initialise( string label, string description, string iconfile, JObject data )
		{
			if ( iconfile == "" )
			{
				iconfile = Program.PATH_PA + "media/ui/main/game/galactic_war/gw_play/img/tech/gwc_storage.png";
			}

			Label = label;
			Description = description;
			IconFile = iconfile;
			JSONData = data;
		}

		// Added To Scene: Initialise individual elements
		public override void Added()
		{
			base.Added();

			// Panel image
			Image_Background = new Entity_Image( X, Y, "", false );
			{
				Image_Background.NineSlice( Program.PATH_PA + "media/ui/main/shared/img/panel/img_menu_panel.png", 24, 24, 256, 364 );

				//Image_Background.SetTarget( center );

				Image_Background.Layer = Helper.Layer_UI;
			}
			Scene.Instance.Add( Image_Background );

			// Icon image
			Image_Icon = new Entity_Image( X, Y - 96, IconFile );
			{
				//Image_Icon.SetTarget( center );
			}
			Scene.Instance.Add( Image_Icon );

			// Card name
			Text_Label = new Text( Label, Program.Font, 20 );
			{
				Text_Label.CenterOrigin();
				Text_Label.SetPosition( new Vector2( 0, 64 ) );
				Text_Label.Color = Color.Green;
			}
			Image_Icon.AddGraphic( Text_Label );

			// Description
			Text_Description = new RichText( Description, Program.Font, 12 );
			{
                Text_Description.SetPosition( new Vector2( -128 + 16, 96 ) );
				Text_Description.TextWidth = 256 - 32;
				Text_Description.WordWrap = true;
			}
			Image_Icon.AddGraphic( Text_Description );

			// Panel image
			Image_Button_Background = new Entity_Image( X, Y + 140, "", false );
			{
				Image_Button_Background.NineSlice( Program.PATH_PA + "media/ui/main/shared/img/panel/img_panel_std_black.png", 120, 86, 326, 156 );

				//Image_Button_Background.SetTarget( center );
			}
			Scene.Instance.Add( Image_Button_Background );

			// Button Choose
			Button_Choose = new Entity_UI_Button();
			{
				Button_Choose.Label = "DOWNLOAD";
				Vector2 pos = new Vector2( X, Y + 134 );
				Button_Choose.Center = Vector2.Zero;
				Button_Choose.Offset.Y = -1;
                Button_Choose.ButtonBounds = new Vector4( pos.X, pos.Y, 196, 48 );
				Button_Choose.Scroll = new Vector2( 1, 1 );
				Button_Choose.OnPressed = delegate ( Entity_UI_Button self )
				{
					self.Image.image.Color = self.Colour_Hover * Color.Gray;
					AudioManager.PlaySound( "resources/audio/ui_click.wav" );
				};
				Button_Choose.OnReleased = delegate ( Entity_UI_Button self )
				{
					NetworkManager.SendPickCard( Label );
					Helper.GetGameScene().HideCards();
					AudioManager.PlaySound( "resources/audio/ui_click.wav" );
				};

				Button_Choose.Layer = Helper.Layer_UI;
			}
			Scene.Instance.Add( Button_Choose );

			Layer = Helper.Layer_UI;
		}
		#endregion

		#region Cleanup
		// Removed From Scene: Remove individual elements (which are not properly attached to this card)
		public override void Removed()
		{
			base.Removed();

			Scene.Instance.Remove( Image_Background );
			Scene.Instance.Remove( Image_Icon );
			Scene.Instance.Remove( Image_Button_Background );
			Scene.Instance.Remove( Button_Choose );
		}
		#endregion
	}
}
