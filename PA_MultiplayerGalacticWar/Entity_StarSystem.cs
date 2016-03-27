// Matthew Cormack
// Star system instance entity
// 18/03/16

using Otter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Add random system map selector
// Add lines connecting to others

namespace PA_MultiplayerGalacticWar
{
	class Entity_StarSystem : Entity
	{
		// System name and type (i.e. map)
		public string Name = StarSystemInfos.Names.RandomElement() + StarSystemInfos.Name_Suffix.RandomElement();
		public string Type = StarSystemInfos.Types.RandomElement();
		// Team colour
		public Color Colour = new Color( 0.6f, 0.5f, 0.5f );

		// Images
		private Entity_Image SelectCircle;
		private Entity_Image Owner;
		private Entity_Image Star;
		// Lerp
		private float Time_Lerp;
		// Selection
		private bool Selected = false;
		private bool Hover = false;

		public Entity_StarSystem( Scene scene, float x, float y, string path_pa, string path_mod ) : base( x, y )
		{
			SelectCircle = new Entity_Image( x, y, "resources/selected.png" );
			{
				SelectCircle.image.Scale = 0.5f;
				SelectCircle.image.Alpha = 0;
				SelectCircle.image.Scroll = 1;
			}
			AddGraphics( SelectCircle.Graphic );

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

			Time_Lerp = 0;

			// Add button press collider
			AddCollider<BoxCollider>( new BoxCollider( 48, 48 ) );
			Collider.SetPosition( -24, -24 );
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
				else
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

		private void Select()
		{
			Selected = true;
			SelectCircle.image.Alpha = 1;
			Console.WriteLine( Name + ": "+ Type );
		}

		private void Deselect()
		{
			Selected = false;
			SelectCircle.image.Alpha = 0;
		}
	}
}
