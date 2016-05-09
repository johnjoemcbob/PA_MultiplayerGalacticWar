// Matthew Cormack
// Star system instance entity
// 18/03/16

using Otter;
using System;
using System.Collections.Generic;

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
		public int Owner = -1; // Player Owner Index

		// Team colour
		public Color Colour = new Color( 0.6f, 0.5f, 0.5f );
		public Entity_PlayerArmy HasPlayerArmy = null;

		// Images
		private Entity_Image Image_SelectCircle;
		private Entity_Image Image_Owner;
		private Entity_Image Image_Star;
		private Entity_UIPanel_StarSystem UI;
		// Lerp
		private float Time_Lerp;
		// Selection
		private bool Selected = false;
		private bool Hover = false;

		private List<Entity_StarSystem> Neighbours = new List<Entity_StarSystem>();

		private List<Entity_PlayerArmy> VisibleBy = new List<Entity_PlayerArmy>();

		public Entity_StarSystem( Scene scene, float x, float y, string path_pa, string path_mod ) : base( x, y )
		{
			ID = StarSystemInfos.Types.IndexOf( Type );

			string file;

			file = Helper.FindFile( new string[] { "resources/selected.png" } );
			if ( file != null )
			{
				Image_SelectCircle = new Entity_Image( x, y, file );
				{
					Image_SelectCircle.image.Scale = 0.5f;
					Image_SelectCircle.image.Alpha = 0;
					Image_SelectCircle.image.Scroll = 1;
				}
				AddGraphics( Image_SelectCircle.Graphic );
			}

			Image_Owner = new Entity_Image( x, y, "resources/owner.png" );
			{
				Image_Owner.image.Scale = 0.5f;
				Image_Owner.image.Color = Colour;
				Image_Owner.image.Scroll = 1;
			}
			AddGraphics( Image_Owner.Graphic );
			//Image_Owner.ClearGraphics();

			Image_Star = new Entity_Image( 0, 0, "resources/star.png" );
			{
				Image_Star.image.Scale = 0.5f;
				Image_Star.image.Alpha = 0.95f;
				Image_Star.image.Color = new Color( 0.9f, 0.9f, 0.9f );
				Image_Star.image.Scroll = 1;
			}
			AddGraphics( Image_Star.Graphic );

			UI = new Entity_UIPanel_StarSystem( Entity_UIPanel_StarSystem.HalfWidth, Entity_UIPanel_StarSystem.HalfHeight - 8, Name, Type );
			{
				UI.System = this;
			}

			Time_Lerp = 0;

			// Add button press collider
			AddCollider<BoxCollider>( new BoxCollider( 48, 48 ) );
			Collider.SetPosition( -24, -24 );

			Visible = false;

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
			if ( Program.Clicked && Visible )
			{
				bool uihit = ( UI.Collider.Overlap( 0.5f, 0.5f, x, y ) && Helper.AnyInScene<Entity_UIPanel_StarSystem>( Scene.Instance.GetEntities<Entity_UIPanel_StarSystem>() ) );
                if ( !uihit && Collider.Overlap( 0.5f, 0.5f, x, y ) )
				{
					Select();
				}
				else if ( !uihit )
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
				Image_Star.Graphic.Scale = 0.5f + ( lerp * dist );
				// Hover
				dist = 0.01f;
				Image_Star.Graphic.OriginY = Image_Star.Graphic.Height * ( 0.5f + ( lerp * dist ) );
				// Colour
				dist = 0.1f;
				Image_Star.Graphic.Color.R = 0.9f - ( lerp * dist );
				Image_Star.Graphic.Color.G = 0.9f - ( lerp * dist );
			}
		}

		public override void Removed()
		{
			base.Removed();

			Image_SelectCircle = null;
			Image_Star = null;
			Scene.Instance.Remove( UI );
        }

		private void Select()
		{
			Selected = true;
			Image_SelectCircle.image.Alpha = 1;

			Scene.Instance.GetEntity<Entity_Galaxy>().UpdateSelection( Index, true );

			Scene.Instance.Add( UI );
		}

		private void Deselect()
		{
			Selected = false;
			Image_SelectCircle.image.Alpha = 0;

			Scene.Instance.GetEntity<Entity_Galaxy>().UpdateSelection( Index, false );

			Scene.Instance.Remove( UI );
		}

		public void AddConnection( Entity_StarSystem other )
		{
			Neighbours.Add( other );
		}

		public void AddPlayerArmy( Entity_PlayerArmy army )
		{
			HasPlayerArmy = army;
			AddVisibleBy( army );

			// Update visibility
			foreach ( Entity_StarSystem system in Neighbours )
			{
				system.AddVisibleBy( army );
			}

			// Update UI
			UI.UpdatePlayerArmy( army );
		}

		public void RemovePlayer()
		{
			if ( HasPlayerArmy == null ) return;

			// Update visibility
			foreach ( Entity_StarSystem system in Neighbours )
			{
				system.RemoveVisibleBy( HasPlayerArmy );
			}
			RemoveVisibleBy( HasPlayerArmy );

			// Update UI
			UI.UpdatePlayerArmy( null );

			HasPlayerArmy = null;
		}

		public void AddVisibleBy( Entity_PlayerArmy army )
		{
			if ( !VisibleBy.Contains( army ) )
			{
				VisibleBy.Add( army );
			}

			CheckVisibility();
		}

		public void RemoveVisibleBy( Entity_PlayerArmy army )
		{
			VisibleBy.Remove( army );

			CheckVisibility();
		}

		public void CheckVisibility()
		{
			bool visible = false;
			{
				foreach ( Entity_PlayerArmy army in VisibleBy )
				{
					if ( army.Player == Program.ThisPlayer )
					{
						visible = true;
						break;
					}
				}
			}
			Visible = visible;
		}

		public void SetOwner( int owner )
		{
			Owner = owner;

			Colour = new Otter.Color( ( (Scene_Game) Scene.Instance ).CurrentPlayers.ToArray()[Owner].Commander.Colour );
			Image_Owner.image.Color = Colour;
		}
	}
}
