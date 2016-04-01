// Matthew Cormack
// JSON format for ---: DefaultUnits (could be a building or a unit)
// 27/03/16

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PA_MultiplayerGalacticWar
{
	struct PAPhysics
	{
		public bool underwater;
		public string shape;
		public float radius;
		public float gravity_scalar;
		public string collision_layers;
    };

    struct PASelectionIcon
	{
		public string diameter;
	};

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
		public PASelectionIcon selection_icon;
		public float[] mesh_bounds;
		public float[] placement_size;
		public string area_build_type;
		public float area_build_separation;
		public float TEMP_texelinfo;
		public PAPhysics physics;
		public float guard_radius;
		public string guard_layer;
		public PANavigation navigation;
		public bool maintain_priority_target;
		public float gravwell_velocity_multiplier;
		public float wreckage_health_frac;
		public object attachable;
		public object transportable;
		public string[] feature_requirements;
		public bool force_snap_to_feature_orientation;
		public PAResource production;
		public PAResource consumption;
		public float energy_efficiency_requirement;
        public string[] replaceable_units;
		public string[] buildable_projectiles;
		public object factory;
		public bool flip_drag_orientation;
		public object orders;
		public object teleporter;
		public object useable;
    }
}
