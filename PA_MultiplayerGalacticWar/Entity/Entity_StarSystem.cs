// Matthew Cormack
// Star system instance entity
// 18/03/16

using Otter;
using System;

// Add random system map selector
// Add lines connecting to others

namespace PA_MultiplayerGalacticWar.Entity
{
	class Entity_StarSystem : Otter.Entity
	{
		// System name and type (i.e. map)
		public string Name = StarSystemInfos.Names.RandomElement() + StarSystemInfos.Name_Suffix.RandomElement();
		public string Type = StarSystemInfos.Types.RandomElement();
		public int ID = -1; // Map ID
		public int Index = -1; // Array Index

		// Team colour
		public Color Colour = new Color( 0.6f, 0.5f, 0.5f );

		// Images
		private Entity_Image SelectCircle;
		private Entity_Image Owner;
		private Entity_Image Star;
		private Entity_UIPanel_StarSystem UI;
		// Lerp
		private float Time_Lerp;
		// Selection
		private bool Selected = false;
		private bool Hover = false;

		public Entity_StarSystem( Scene scene, float x, float y, string path_pa, string path_mod ) : base( x, y )
		{
			ID = StarSystemInfos.Types.IndexOf( Type );

			string file;

			file = Helper.FindFile( new string[] { "resources/selected.png" } );
			if ( file != null )
			{
				SelectCircle = new Entity_Image( x, y, file );
				{
					SelectCircle.image.Scale = 0.5f;
					SelectCircle.image.Alpha = 0;
					SelectCircle.image.Scroll = 1;
				}
				AddGraphics( SelectCircle.Graphic );
			}

			Owner = new Entity_Image( x, y, "resources/owner.png" );
			{
                Owner.image.Scale = 0.5f;
				Owner.image.Color = Colour;
				Owner.image.Scroll = 1;
			}
			AddGraphics( Owner.Graphic );
			//Owner.ClearGraphics();

			Star = new Entity_Image( 0, 0, "resources/star.png" );
			{
				Star.image.Scale = 0.5f;
				Star.image.Alpha = 0.95f;
				Star.image.Color = new Color( 0.9f, 0.9f, 0.9f );
				Star.image.Scroll = 1;
			}
			AddGraphics( Star.Graphic );

			UI = new Entity_UIPanel_StarSystem( Entity_UIPanel_StarSystem.HalfWidth, Entity_UIPanel_StarSystem.HalfHeight - 8, Name, Type );
			{
				UI.SystemID = ID;
			}

			Time_Lerp = 0;

			// Add button press collider
			AddCollider<BoxCollider>( new BoxCollider( 48, 48 ) );
			Collider.SetPosition( -24, -24 );

			Entity_PlayerArmy army = new Entity_PlayerArmy();
			{
				army.X = X;
				army.Y = Y;
			}
            Scene.Instance.Add( army );

			Layer = Helper.Layer_Star;
		}

		public override void Update()
		{
			base.Update();

			// Logic for hovering
			int x = (int) ( Input.MouseScreenX / Scene.Instance.CameraZoom );
			int y = (int) ( Input.MouseScreenY / Scene.Instance.CameraZoom );
			if ( Collider.Overlap( 0.5f, 0.5f, x, y ) )
			{
				Hover = true;
			}
			else
			{
				Hover = false;
			}
			// Logic for selecting
			if ( Input.MouseButtonPressed( MouseButton.Left ) )
			{
				if ( Collider.Overlap( 0.5f, 0.5f, x, y ) )
				{
					Select();
				}
				else if ( !UI.Collider.Overlap( 0.5f, 0.5f, x, y ) )
                {
					Deselect();
				}
			}
			if ( Input.MouseButtonPressed( MouseButton.Right ) )
			{
				Deselect();
			}

			// Pulse slightly
			float extradist = 2;
			{
				if ( Selected )
				{
					extradist *= 2;
				}
				else if ( !Hover )
				{
					extradist = 1;
				}
			}
			float offset = ( X / Game.Instance.Width ) / 10;
			float speed = 0.03f;
			Time_Lerp += ( Game.Instance.DeltaTime * ( speed * extradist ) ) + offset;
			float lerp = (float) Math.Sin( Time_Lerp + offset );
			{
				// Scale
				float dist = 0.01f;
				Star.Graphic.Scale = 0.5f + ( lerp * dist );
				// Hover
				dist = 0.01f;
				Star.Graphic.OriginY = Star.Graphic.Height * ( 0.5f + ( lerp * dist ) );
				// Colour
				dist = 0.1f;
				Star.Graphic.Color.R = 0.9f - ( lerp * dist );
				Star.Graphic.Color.G = 0.9f - ( lerp * dist );
			}
		}

		public override void Removed()
		{
			base.Removed();

			SelectCircle = null;
			Star = null;
			Scene.Instance.Remove( UI );
        }

		private void Select()
		{
			Selected = true;
			SelectCircle.image.Alpha = 1;

			Scene.Instance.GetEntity<Entity_Galaxy>().UpdateSelection( Index, true );

			Scene.Instance.Add( UI );
		}

		private void Deselect()
		{
			Selected = false;
			SelectCircle.image.Alpha = 0;

			Scene.Instance.GetEntity<Entity_Galaxy>().UpdateSelection( Index, false );

			Scene.Instance.Remove( UI );
		}
	}
}
