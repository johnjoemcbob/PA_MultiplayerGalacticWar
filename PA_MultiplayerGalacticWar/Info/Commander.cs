// Matthew Cormack
// JSON format for ---: Commanders
// 18/03/16

#region Includes
using System;
using Newtonsoft.Json.Linq;
#endregion

namespace PA_MultiplayerGalacticWar
{
	//	"client":{
	//		"ui":{
	//			"image":"/ui/main/shared/img/commanders/img_imperial_invictus.png",
	//			"thumb_image":"/ui/main/shared/img/commanders/thumbs/img_imperial_invictus_thumb.png",
	//			"profile_image":"/ui/main/shared/img/commanders/profiles/profile_imperial_invictus.png"
	//		}
	//	}

	#region Extra Data Structures
	struct PAUI
	{
		public string image;
		public string thumb_image;
		public string profile_image;
	}

	struct PAClient
	{
		public PAUI ui;
	}

	struct PAReconObvervable
	{
		public bool always_visible;
	}

	struct PAReconObververItem
	{
		public string layer;
		public string channel;
		public string shape;
		public float radius;
	}

    struct PAReconObverver
	{
		public PAReconObververItem[] items;
	}

	struct PARecon
	{
		public PAReconObvervable observable;
        public PAReconObverver observer;
    }

	struct PAResource
	{
		public float energy;
		public float metal;
	}

	struct PANavigation
	{
		public string type;
		public float acceleration;
		public float brake;
		public float move_speed;
		public float turn_speed;
		public float turn_accel;
		public bool turn_in_place;
	}
	#endregion

	class Commander
	{
		#region Variable Declaration
		public String base_spec;
		public String display_name;
		public String description;
		public float max_health;
		public Object attachable; // ?????
		public String si_name;
		public Object model; // ?????
		public PARecon recon;
		public String buildable_types;
		public bool show_in_orbital_layer;
		public String armor_type;
		public PAResource production; // ?????
		public PAResource storage; // ?????
		public float[] mesh_bounds;
		public String[] unit_types;
		public String[] command_caps;
		public PANavigation navigation;
		public Object tools; // ?????
		public String catalog_object_name;
		public Object client; // ?????
		#endregion

		#region Cards
		static public void AddCard( JObject commander, JObject card )
		{
			AddIndividualCard( ref commander, ref card, "max_health" );
			AddIndividualCard( ref commander, ref card, "production", "energy" );
			AddIndividualCard( ref commander, ref card, "production", "metal" );
			AddIndividualCard( ref commander, ref card, "storage", "energy" );
			AddIndividualCard( ref commander, ref card, "storage", "metal" );
			AddIndividualCard( ref commander, ref card, "navigation", "move_speed" );

			//if ( ( card.recon.observer.items != null ) && ( card.recon.observer.items.Length != 0 ) )
			//{
			//	for ( int id = 0; id < card.recon.observer.items.Length; id++ )
			//	{
			//                 if ( card.recon.observer.items[id].radius != 0 )
			//		{
			//			recon.observer.items[id].radius += recon.observer.items[id].radius / 100 * card.recon.observer.items[id].radius;
			//                 }
			//	}
			//}
		}

		static public void AddIndividualCard( ref JObject commander, ref JObject card, string key )
		{
			if ( ( commander[key] != null ) && ( card[key] != null ) )
			{
				commander[key] = float.Parse( commander[key].ToString() ) + ( ( float.Parse( commander[key].ToString() ) / 100.0f * float.Parse( card[key].ToString() ) ) );
			}
		}

		static public void AddIndividualCard( ref JObject commander, ref JObject card, string key, string key2 )
		{
			if ( ( commander[key] != null ) && ( card[key] != null ) && ( commander[key][key2] != null ) && ( card[key][key2] != null ) )
			{
				commander[key][key2] = float.Parse( commander[key][key2].ToString() ) + ( ( float.Parse( commander[key][key2].ToString() ) / 100.0f * float.Parse( card[key][key2].ToString() ) ) );
			}
		}
		#endregion
	}
}
