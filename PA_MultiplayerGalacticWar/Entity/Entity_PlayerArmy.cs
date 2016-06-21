// Matthew Cormack
// Individual Player Army, for moving around the galactic map
// 09/05/16

#region Includes
using Otter;
using System;
using System.Collections.Generic;
#endregion

namespace PA_MultiplayerGalacticWar.Entity
{
	class Entity_PlayerArmy : Otter.Entity
	{
		#region Variable Declaration
		public int Player = 0;
		public int PlayerArmyID = 0;
		public Entity_StarSystem System = null;

		private Vector2 Target;
		private bool TargetSet = false;

		private List<Vector2> TrailPoints = new List<Vector2>();
		private int MaxTrailPoints = 50;
		private float MinTrailDistance = 0;
		private float TrailMaxWidth = 16;
        private float NextTrailTime = 0;
		private float BetweenTrailTime = 2;

		private Image Icon;

		private Info_Player Info;

		private Sound Sound_Move;
		private float PitchModifier = 1;
		#endregion

		#region Initialise
		public override void Added()
		{
			base.Added();

			Info = ( (Scene_Game) Scene ).CurrentPlayers.ToArray()[Player];

            string file = Helper.FindFile( new string[] { Program.PATH_PA + "media/ui/main/game/galactic_war/shared/img/icon_faction_" + Info.Commander.FactionID + ".png" } );
			if ( file != null )
			{
				Icon = new Image( file );
				{
					Icon.Scale = 0.5f;
					Icon.CenterOrigin();
					Icon.Scroll = 1;
				}
				Icon.Color = new Otter.Color( Info.Commander.Colour );
			}

			Sound_Move = AudioManager.Instance.PlaySound( "resources/audio/player_move_loop.wav", true );
			Sound_Move.Volume = 0.1f;

			Visible = false;

			Layer = Helper.Layer_Player;
        }
		#endregion

		#region Update
		public override void Update()
		{
			base.Update();

			if ( Helper.IsLoading() ) return;

			// Add the old position to the trail points list
			Vector2 pos = new Vector2( X, Y );
			if ( ( TrailPoints.Count == 0 ) || ( Vector2.Distance( pos, TrailPoints.ToArray()[TrailPoints.Count - 1] ) >= MinTrailDistance ) )
			{
				TrailPoints.Add( pos );
				NextTrailTime = Game.Instance.Timer + BetweenTrailTime;

				if ( TrailPoints.Count > MaxTrailPoints )
				{
					// Pop the oldest point off
					TrailPoints.RemoveAt( 0 );
				}
			}
			else if ( NextTrailTime < Game.Instance.Timer )
			{
				// Pop the oldest point off
				TrailPoints.RemoveAt( 0 );
				NextTrailTime = Game.Instance.Timer + BetweenTrailTime;
            }

			// Lerp
			float speed = 0.05f;
			float changex = ( Target.X - X ) * Game.Instance.DeltaTime * speed;
			float changey = ( Target.Y - Y ) * Game.Instance.DeltaTime * speed;
            X += changex;
			Y += changey;

			// Play audio if moving
			if ( Vector2.Distance( Target, new Vector2( X, Y ) ) < 0.5f )
			{
				Sound_Move.Volume = 0.1f;
			}
			else
			{
				Sound_Move.Volume = 1.0f;
				Sound_Move.Pitch = Math.Abs( changex + changey ) * PitchModifier;
            }

			// Only show to own player unless they have been scouted
			bool scouted = false;
			{
				foreach ( Entity_StarSystem system in System.GetNeighbours() )
				{
					if ( ( system.HasPlayerArmy != null ) && ( system.HasPlayerArmy.Player == Program.ThisPlayer ) )
					{
						scouted = true;
						break;
					}
				}
			}
			if ( ( Player == Program.ThisPlayer ) || scouted || ( Helper.DEBUG ) )
			{
				Visible = true;
			}
			else
			{
				Visible = false;
			}
		}
		#endregion

		#region Render
		public override void Render()
		{
			base.Render();

			// Loop backwards through the trail points, getting progressively smaller
			int length = TrailPoints.Count - 1;
			float width = TrailMaxWidth;
            for ( int point = length; point > 0; point-- )
			{
				if ( Vector2.Distance( TrailPoints[point], TrailPoints[point - 1] ) < 0.5f ) continue;

				// Draw wobbly
				float radius = 16;
				float speed = 0.05f;
				float offset = point;
				Vector2 start = GetWobblePoint( TrailPoints[point], radius / MaxTrailPoints * point, speed, offset * radius );
				Vector2 end = GetWobblePoint( TrailPoints[point - 1], radius / MaxTrailPoints * ( point - 1 ), speed, offset * radius );
				Draw.RoundedLine( start.X, start.Y, end.X, end.Y, Icon.Color, width / MaxTrailPoints * point );
			}

			// Manual draw on top of the trail
			Icon.Render( X, Y );
        }
		#endregion

		private Vector2 GetWobblePoint( Vector2 point, float radius, float speed, float offset )
		{
			Vector2 wobble = new Vector2();
			{
				float time = Game.Instance.Timer;

				wobble.X = point.X + ( (float) Math.Sin( ( time * speed ) + offset ) * radius );
				wobble.Y = point.Y + ( (float) Math.Cos( ( time * speed ) + offset ) * radius );
			}
			return wobble;
		}

		public void MoveToSystem( Entity_StarSystem system )
		{
			// Remove player from old system
			if ( System != null )
			{
				System.RemovePlayer();
			}

			// Move player
			System = system;

			// Add player to new system
			if ( System != null )
			{
				// Lerp towards new system, unless it is the first
				if ( !TargetSet )
				{
					X = System.X;
					Y = System.Y;
					TargetSet = true;
                }
				Target = new Vector2( System.X, System.Y );

				System.AddPlayerArmy( this );
			}
		}

		#region Getters
		static public Entity_PlayerArmy[] GetAllByPlayer( int player )
		{
			// Find all army entities in the scene and sort through to find this player's
			List<Entity_PlayerArmy> armies = new List<Entity_PlayerArmy>();
			{
				foreach ( Entity_PlayerArmy army in Scene.Instance.GetEntities<Entity_PlayerArmy>() )
				{
					if ( army.Player == player )
					{
						armies.Add( army );
					}
				}
			}
			return armies.ToArray();
		}
		#endregion
	}
}
