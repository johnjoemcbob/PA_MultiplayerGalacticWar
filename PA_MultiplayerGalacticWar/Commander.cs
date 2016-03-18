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
	struct PAResource
	{
		public float energy;
		public float metal;
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
		public Object recon; // ?????
		public String buildable_types;
		public bool show_in_orbital_layer;
		public String armor_type;
		public PAResource production; // ?????
		public PAResource storage; // ?????
		public float[] mesh_bounds;
		public String[] unit_types;
		public String[] command_caps;
		public Object navigation; // ?????
		public Object tools; // ?????
		public String catalog_object_name;
		public Object client; // ?????
    }
}
