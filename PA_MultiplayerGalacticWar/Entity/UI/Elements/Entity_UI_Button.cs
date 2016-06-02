// Matthew Cormack
// Basic button UI element
// 30/03/16

using Otter;

namespace PA_MultiplayerGalacticWar.Entity
{
	delegate void MouseEvent( Entity_UI_Button self );

	class Entity_UI_Button : Otter.Entity
	{
		#region Variable Declaration
		// Label string to display on top of the button
		public string Label = "DEFAULT TEXT";

		// Button positioning
		public Vector2 Offset = Vector2.Zero;
		public Vector2 Origin = new Vector2( 0.5f, 0.5f );
		public Vector2 Center = new Vector2( Game.Instance.HalfWidth, Game.Instance.HalfHeight );

		// Clickable boundary
        public Vector4 ButtonBounds = new Vector4();
		public Vector4 ButtonCameraBounds = new Vector4();

		// Button callbacks
		public MouseEvent OnPressed = delegate ( Entity_UI_Button self ) { };
		public MouseEvent OnUnPressed = delegate ( Entity_UI_Button self ) { };
		public MouseEvent OnReleased = delegate ( Entity_UI_Button self ) { };
		public MouseEvent OnHover = delegate ( Entity_UI_Button self ) { };
		public MouseEvent WhileHover = delegate ( Entity_UI_Button self ) { };
		public MouseEvent OnUnHover = delegate ( Entity_UI_Button self ) { };

		// Button graphic camera scroll
		public Vector2 Scroll = new Vector2( 0, -0.1f );

		// Button event change colours
		public Color Colour_Default = Color.White;
		public Color Colour_Hover = Color.Green;

		// Button status
		public bool Hover = false;
		protected bool Pressed = false;

		// Button individual elements
		public Entity_Image Image;
		protected Text Text_Label;
		#endregion

		#region Intialise
		// Constructor: Initialise base mouse hover callbacks
		public Entity_UI_Button()
		{
			OnHover = delegate ( Entity_UI_Button self )
			{
				Image.image.Color = Colour_Hover;
				AudioManager.PlaySound( "resources/audio/ui_hover.wav" );
			};
			OnUnHover = delegate ( Entity_UI_Button self )
			{
				Image.image.Color = Colour_Default;
			};
		}

		// Added To Scene: Initialise the button graphics
		public override void Added()
		{
			base.Added();

            Image = new Entity_Image( 0, 0, "", false );
			{
				int width = (int) ( ButtonBounds.Z );
				int height = (int) ( ButtonBounds.W );
				Image.NineSlice( Program.PATH_PA + "media/ui/main/shared/img/buttons/btn_lrg_std.png", 82, 50, width, height );

				Image.image.Color = Colour_Default;
                Image.SetTarget( Center + new Vector2( ButtonBounds.X, ButtonBounds.Y ) );
				Image.image.ScrollX = Scroll.X;
				Image.image.ScrollY = Scroll.Y;

				Image.Layer = Helper.Layer_UI;
			}
			Scene.Instance.Add( Image );

			Text_Label = new Text( Label, Program.Font, 24 );
            {
				Text_Label.CenterOrigin();
				Text_Label.SetPosition( new Vector2( 0, -( ButtonBounds.W / 4 ) ) );
				Text_Label.ScrollX = Scroll.X;
				Text_Label.ScrollY = Scroll.Y;
			}
			Image.AddGraphic( Text_Label );

			Layer = Helper.Layer_UI;
		}
		#endregion

		#region Update
		// Update In Scene: Move the button clickable boundaries with the camera & run click logic
		public override void Update()
		{
			base.Update();

			// Can't click buttons when the game is loading
			if ( Helper.IsLoading() ) return;

			UpdateBounds();
			UpdateClick();
		}

		// Update In Scene: Update the bounds of the clickable button as the camera moves around
		private void UpdateBounds()
		{
			ButtonCameraBounds = new Vector4(
				Image.image.X + ButtonBounds.X - ( Image.image.ScaledWidth * ( 0.5f + Offset.X ) ),
				Image.image.Y + ButtonBounds.Y - ( Image.image.ScaledHeight * ( 1.5f + Offset.Y ) ),
				Image.image.ScaledWidth,
				Image.image.ScaledHeight
			);
			{
				ButtonCameraBounds += new Vector4(
					Scene.Instance.CameraCenterX * ( 1 - Scroll.X ),
					Scene.Instance.CameraCenterY * ( 1 - Scroll.Y ),
					0,
					0
				);
			}
			{
				ButtonCameraBounds += new Vector4(
					0,
					0,
					ButtonCameraBounds.X,
					ButtonCameraBounds.Y
				);
			}
		}

		// Update In Scene: Mouse clickable/hoverable logic
		private void UpdateClick()
		{
			Vector2 mouse = new Vector2( Scene.Instance.MouseRawX, Scene.Instance.MouseRawY );
			if ( Helper.PointWithinRect( mouse, ButtonCameraBounds ) )
			{
				if ( Program.Clicked )
				{
					OnPressed( this );
					Pressed = true;
					Program.Clicked = false;
				}
				else if ( Game.Instance.Input.MouseButtonReleased( MouseButton.Left ) )
				{
					OnReleased( this );
					Hover = false;
				}
				if ( !Hover )
				{
					OnHover( this );
					Hover = true;
				}
			}
			else if ( Hover )
			{
				Hover = false;
				OnUnHover( this );
			}
			if ( ( !Program.Clicked ) && Pressed )
			{
				OnUnPressed( this );
				Pressed = false;
			}
			if ( Hover )
			{
				WhileHover( this );
			}
		}

		// Called only once ever each update - even between many buttons: For clickable logic
		public static void GlobalUpdate()
		{
			Program.Clicked = Scene.Instance.Input.MouseButtonPressed( MouseButton.Left );
		}
		#endregion

		#region Cleanup
		// Removed From Scene: Remove the button image (which is not properly attached to this button)
		public override void Removed()
		{
			base.Removed();

			Scene.Instance.Remove( Image );
		}
		#endregion

		#region Alter Visuals
		// Called from outside: To amend the button visuals with any recent variable changes
		public void UpdateGraphic()
		{
			if ( Text_Label == null ) return;

			// Text
			Text_Label.String = Label;
			Text_Label.CenterOrigin();

			// Colour
			Image.image.Color = Colour_Default;
		}
		#endregion
	}
}
