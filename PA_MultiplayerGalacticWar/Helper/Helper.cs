﻿// Matthew Cormack
// Various helper functions
// 30/03/16

#region Includes
using Otter;
using System;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;
#endregion

namespace PA_MultiplayerGalacticWar
{
	class Helper
	{
		#region Variable Declaration
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
		#endregion

		#region File IO
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

		// Copy all directories and files from source to destination, with overwriting
		static public void CopyDirectory( string sourcepath, string destpath )
		{
			foreach ( string sourcedir in Directory.GetDirectories( sourcepath ) )
			{
				// Get appended for destination
				string destdir = sourcedir.Remove( 0, sourcepath.Length );
				// Remove any double slashes
				destdir = destdir.Replace( "//", "/" );
				destdir = destdir.Replace( "\\", "/" );
				// Recurse
				CopyDirectory( sourcedir, destpath + destdir );
			}

			foreach ( string sourcefile in Directory.GetFiles( sourcepath ) )
			{
				// Remove any double slashes
				string file;
				file = sourcefile.Replace( "//", "/" );
				file = sourcefile.Replace( "\\", "/" );

				// Get appended for destination & copy
				string destfile = destpath + file.Remove( 0, sourcepath.Length );

				// Delete the file if it already exists, as File.Copy doesn't handle overwriting
				if ( File.Exists( destfile ) )
				{
					File.Delete( destfile );
				}

				// Copy the source file to the destination
				CreateDirectory( destfile );
				File.Copy( file, destfile );
			}
		}

		// Creates a zip file in a destination from a source directory
		static public void ZipDirectory( string sourcepath, string destpath = "" )
		{
			// Default to same directory
			if ( destpath == "" )
			{
				destpath = sourcepath;
			}

			// Remove last slash and add .zip file extension
			destpath = destpath.Remove( destpath.Length - 1, 1 );
			destpath += ".zip";

			// Delete file if pre-existing
			if ( File.Exists( destpath ) )
			{
				File.Delete( destpath );
			}

			// Compress
			ZipFile.CreateFromDirectory( sourcepath, destpath );
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
		#endregion

		#region Collision & Positioning
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
		#endregion

		#region Check for Entities in Scene
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
		#endregion

		public static Scene_Game GetGameScene()
		{
			return ( (Scene_Game) Scene.Instance );
		}
	}
}
