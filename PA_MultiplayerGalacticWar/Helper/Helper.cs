// Matthew Cormack
// Various helper functions
// 30/03/16

using Otter;
using System;
using System.IO;
using System.Collections.Generic;

namespace PA_MultiplayerGalacticWar
{
	class Helper
	{
		// Flags
		static public bool DEBUG = false;

		// Colours
        static public Color Colour_Unowned = new Color( 0.2f, 0.4f, 0.7f, 1 );

		// Layers
		static public int Layer_Background = 1000;
		static public int Layer_StarRoute = 900;
		static public int Layer_Star = 800;
        static public int Layer_Player = 700;
		static public int Layer_UI = 600;
		static public int Layer_Cursor = 0;

		// Action Types
		public const int ACTION_MOVE = 0;
		public const int ACTION_WAR = 1;

		// Ensure all directories in path exist
		static public void CreateDirectory( string path )
		{
			string[] dirs = path.Split( '/' );
			string currentdir = "";
			foreach ( string dir in dirs )
			{
				currentdir += dir + "/";
				if ( ( !Directory.Exists( currentdir ) ) && ( !dir.Contains( "." ) ) )
				{
					Directory.CreateDirectory( currentdir );
				}
            }
		}

		// Load files as strings
		static public string ReadFile( string file )
		{
			try
			{   // Open the text file using a stream reader.
				using ( StreamReader sr = new StreamReader( file ) )
				{
					// Read the stream to a string, and write the string to the console.
					return sr.ReadToEnd();
				}
			}
			catch ( Exception e )
			{
				Console.WriteLine( "The file could not be read:" );
				Console.WriteLine( e.Message );
			}
			return "";
		}

		// Return the first existing file OR null
		static public string FindFile( string[] files )
		{
			foreach ( string file in files )
			{
				if ( File.Exists( file ) )
				{
					return file;
				}
			}
			return null;
		}

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

		// Entity checking helpers
		public static bool AnyInScene<T>( List<T> entities )
		{
			foreach ( T entity in entities )
			{
				Otter.Entity ent = (Otter.Entity) (object) entity;
				if ( ent.IsInScene )
				{
					return true;
				}
			}
			return false;
		}
		public static bool IsLoading()
		{
			return AnyInScene<Entity.Entity_UIPanel_FileIO>( Otter.Scene.Instance.GetEntities<Entity.Entity_UIPanel_FileIO>() );
		}

		public static Scene_Game GetGameScene()
		{
			return ( (Scene_Game) Scene.Instance );
		}

		public static Entity.Entity_PlayerArmy GetPlayerArmy( int player, int armyid )
		{
			foreach ( Entity.Entity_PlayerArmy army in Scene.Instance.GetEntities<Entity.Entity_PlayerArmy>() )
			{
				if ( ( army.Player == player ) && ( army.PlayerArmyID == armyid ) )
				{
					return army;
				}
			}
			return null;
		}

		// From: http://stackoverflow.com/questions/20879457/remove-junk-value-from-string
		public static string TrimNullTerminatedString( string s )
		{
			if ( s == null )
				throw new NotImplementedException();
			int i = s.IndexOf( '\0' );
			if ( i >= 0 )
				return s.Substring( 0, i );
			return s;
		}
	}
}
