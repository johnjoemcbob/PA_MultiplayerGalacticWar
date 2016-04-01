// Matthew Cormack
// Various helper functions
// 30/03/16

using Otter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PA_MultiplayerGalacticWar
{
	class Helper
	{
		// Basic collision detection; rect expects upper left and lower right points
		public static bool PointWithinRect( Vector2 point, Vector4 rect )
		{
			if (
				( point.X < rect.X ) ||
				( point.X > rect.Z ) ||
				( point.Y < rect.Y ) ||
				( point.Y > rect.W )
			)
			{
				return false;
			}
			return true;
		}
		public static bool CollisionDetection( Vector2 point, Vector4 rect )
		{
			return PointWithinRect( point, rect );
		}

		// Positioning helpers
		public static Vector2 PositionByTopLeft( Vector2 pos )
		{
			return PositionOffset( pos, new Vector2( -0.5f, -0.5f ) );
		}
		public static Vector2 PositionByTopRight( Vector2 pos )
		{
			return PositionOffset( pos, new Vector2( 0.5f, -0.5f ) );
		}
		public static Vector2 PositionByBottomLeft( Vector2 pos )
		{
			return PositionOffset( pos, new Vector2( -0.5f, 0.5f ) );
		}
		public static Vector2 PositionByBottomRight( Vector2 pos )
		{
			return PositionOffset( pos, new Vector2( 0.5f, 0.5f ) );
		}
		public static Vector2 PositionOffset( Vector2 pos, Vector2 off )
		{
			Vector2 screenoff = new Vector2( Game.Instance.Width * off.X, Game.Instance.Height * off.Y );
			return ( pos + screenoff );
		}
    }
}
