// Matthew Cormack
// JSON format for ---: Commanders
// 18/03/16

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PA_MultiplayerGalacticWar
{
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
		public bool turn_in_place;
	}

	class Commander
	{
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

		public void AddCard( Commander card )
		{
			if ( card.max_health != 0 )
			{
				max_health += max_health / 100 * card.max_health;
			}
			if ( card.production.energy != 0 )
			{
				production.energy += production.energy / 100 * card.production.energy;
			}
			if ( card.production.metal != 0 )
			{
				production.metal += production.metal / 100 * card.production.metal;
			}
			if ( card.storage.energy != 0 )
			{
				storage.energy += storage.energy / 100 * card.storage.energy;
			}
			if ( card.storage.metal != 0 )
			{
				storage.metal += storage.metal / 100 * card.storage.metal;
			}
			if ( card.navigation.move_speed != 0 )
			{
				navigation.move_speed += navigation.move_speed / 100 * card.navigation.move_speed;
			}
			if ( ( card.recon.observer.items != null ) && ( card.recon.observer.items.Length != 0 ) )
			{
				for ( int id = 0; id < card.recon.observer.items.Length; id++ )
				{
                    if ( card.recon.observer.items[id].radius != 0 )
					{
						recon.observer.items[id].radius += recon.observer.items[id].radius / 100 * card.recon.observer.items[id].radius;
                    }
				}
			}
		}
    }
}
