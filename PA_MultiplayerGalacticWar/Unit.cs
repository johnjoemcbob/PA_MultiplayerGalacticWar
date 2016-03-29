// Matthew Cormack
// JSON format for ---: DefaultUnits
// 27/03/16

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PA_MultiplayerGalacticWar
{
	class Unit
	{
		public String base_spec;
		public String display_name;
		public String description;
		public float max_health;
		public float build_metal_cost;
		public float atrophy_rate;
		public float atrophy_cool_down;
		public string spawn_layers;
		public string buildable_types;
		public object rolloff_dirs;
		public float wait_to_rolloff_time;
		public float factory_cooldown_time;
		public string[] unit_types;
		public string[] command_caps;
		public object recon;
		public object model;
		public object tools;
		public object events;
		public object audio;
		public object fx_offsets;
		public object headlights;
		public object lamps;
		public object death;
		public object selection_icon;
		public float[] mesh_bounds;
		public float[] placement_size;
		public float area_build_separation;
		public float TEMP_texelinfo;
		public object physics;
    }
}
