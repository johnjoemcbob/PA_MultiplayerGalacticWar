// Matthew Cormack
// Main menu scene; choose saved game or enter the create new game menu
// 30/03/16

using Otter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PA_MultiplayerGalacticWar
{
	class Scene_ChooseGame : Scene
	{
		Entity_UI_Button Button_New;
		Entity_UI_Button Button_Continue;
		Entity_UI_Button Button_Load;
		Entity_UI_Button Button_Quit;

		public override void Begin()
		{
			base.Begin();

			// Add background
			Add( new Entity_Image_Background( this, Program.PATH_PA, Program.PATH_MOD ) );

			// Add title image
			Entity_Image image_title = new Entity_Image( 0, 0, Program.PATH_PA + "media/ui/main/shared/img/logos/img_pa_logo_start_rest.png" );
			{
				image_title.SetTarget( new Vector2( Game.Instance.HalfWidth, Game.Instance.HalfHeight / 4 ) );
				image_title.image.Scale = 2;
				image_title.image.ScrollX = 0;
				image_title.image.ScrollY = 0.05f;
			}
			Add( image_title );

			// Setup buttons
			Button_New = new Entity_UI_Button();
			{
				Button_New.Label = "NEW";
				Vector2 pos = new Vector2( 0, -100 );
                Button_New.ButtonBounds = new Vector4( pos.X, pos.Y, 256, 48 );
				Button_New.OnPressed = delegate ( Entity_UI_Button self )
				{
					self.Image.image.Color = self.Colour_Hover * Color.Gray;
				};
				Button_New.OnReleased = delegate ( Entity_UI_Button self )
				{
					Game.Instance.RemoveScene();
					Game.Instance.AddScene( new Scene_Game() );
				};
			}
			Add( Button_New );
			Button_Continue = new Entity_UI_Button();
			{
				Button_Continue.Label = "CONTINUE";
				Vector2 pos = new Vector2( 0, -25 );
				Button_Continue.ButtonBounds = new Vector4( pos.X, pos.Y, 256, 48 );
				Button_Continue.OnPressed = delegate ( Entity_UI_Button self )
				{
					self.Image.image.Color = self.Colour_Hover * Color.Gray;
				};
				Button_Continue.OnReleased = delegate ( Entity_UI_Button self )
				{
					Game.Instance.RemoveScene();
					Game.Instance.AddScene( new Scene_Game( "data/game1.json" ) );
				};
			}
			Add( Button_Continue );
			Button_Load = new Entity_UI_Button();
			{
				Button_Load.Label = "LOAD GAME";
				Vector2 pos = new Vector2( 0, 50 );
				Button_Load.ButtonBounds = new Vector4( pos.X, pos.Y, 256, 48 );
				Button_Load.OnPressed = delegate ( Entity_UI_Button self )
				{
					self.Image.image.Color = self.Colour_Hover * Color.Gray;
				};
				Button_Load.OnReleased = delegate ( Entity_UI_Button self )
				{
					Game.Instance.RemoveScene();
					Game.Instance.AddScene( new Scene_Game( "data/game2.json" ) );
				};
			}
			Add( Button_Load );
			Button_Quit = new Entity_UI_Button();
			{
				Button_Quit.Label = "QUIT";
				Vector2 pos = new Vector2( 0, 250 );
				Button_Quit.ButtonBounds = new Vector4( pos.X, pos.Y, 256, 48 );
				Button_Quit.OnPressed = delegate ( Entity_UI_Button self )
				{
					self.Image.image.Color = self.Colour_Hover * Color.Gray;
				};
				Button_Quit.OnReleased = delegate ( Entity_UI_Button self )
				{
					Game.Instance.Close();
				};
			}
			Add( Button_Quit );

			Game.Instance.QuitButton.Clear();
		}

		public override void UpdateFirst()
		{
			base.UpdateFirst();

			Entity_UI_Button.GlobalUpdate();
		}

		public override void Update()
		{
			base.Update();

			if ( Game.Instance.Input.KeyPressed( Key.Escape ) )
			{
				Game.Instance.Close();
			}
        }
	}
}
