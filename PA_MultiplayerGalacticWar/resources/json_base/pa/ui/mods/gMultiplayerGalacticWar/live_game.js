// Matthew Cormack (@johnjoemcbob)
// 29/03/16
//
// Multiplayer Galactic War mod (PAMGW)
// Requires standalone application to play
// Repo: https://github.com/johnjoemcbob/PA_MultiplayerGalacticWar
//
// Thanks to mikeyh for WhoDeletedThat and UnitSelector, both of which this mod is losely based on
// https://forums.uberent.com/threads/rel-server-lobby-live-game-unit-selector.67177/
// Using this as a reference for how to change the build menus of individual commanders

var PAMGW_PlayerID = -1;
if ( sessionStorage.uberId == "\"7420260152538080746\"" ) // MATTHEW
{
	PAMGW_PlayerID = 0;
}
else if ( sessionStorage.uberId == "\"6884095390246756552\"" ) // JORDAN
{
	PAMGW_PlayerID = 1;
}
else if ( sessionStorage.uberId == "\"16024636805495278681\"" ) // SEAN?
{
	PAMGW_PlayerID = 2;
}
else //"friends":"[\"11035761434068835310\",\"16024636805495278681\",\"6884095390246756552\"]",
{
	PAMGW_PlayerID = 3;
}

function PAMGW_AlterOnBuild()
{
	model.default_executeStartBuild = model.executeStartBuild;
	model.executeStartBuild = function( params )
	{
		// Add player id to the unit's name to change to the commander specific variant
		params.item = params.item.substring( 0, params.item.length - 5 ) + PAMGW_PlayerID + ".json";

		model.default_executeStartBuild( params );
	}
}
PAMGW_AlterOnBuild();

function PAMGW_AlterBuildUI()
{
	model.default_unit_specs = handlers.unit_specs;

	handlers.unit_specs = function( payload )
	{
		try
		{
			var payload_ids = _.keys( payload );

			var unit_ids_to_remove = [];

			if ( PAMGW_PlayerID == 0 )
			{
				unit_ids_to_remove = [
					"/pa/units/air/titan_air/titan_air.json",
					"/pa/units/land/titan_bot/titan_bot.json",
					"/pa/units/land/titan_vehicle/titan_vehicle.json",
					"/pa/units/land/unit_cannon/unit_cannon.json",
					"/pa/units/sea/naval_factory_adv/naval_factory_adv.json",
					"/pa/units/air/air_factory_adv/air_factory_adv.json",
					"/pa/units/land/bot_factory_adv/bot_factory_adv.json",
					"/pa/units/orbital/orbital_launcher/orbital_launcher.json",
					"/pa/units/sea/naval_factory/naval_factory.json",
					"/pa/units/air/air_factory/air_factory.json",
					"/pa/units/land/bot_factory/bot_factory.json",

					"/pa/units/land/tank_armor/tank_armor.json",
					"/pa/units/land/land_scout/land_scout.json",
					"/pa/units/land/aa_missile_vehicle/aa_missile_vehicle.json",
					"/pa/units/land/tank_hover/tank_hover.json"
				];
			}
			else if ( PAMGW_PlayerID == 1 )
			{
				unit_ids_to_remove = [
					"/pa/units/land/vehicle_factory/vehicle_factory.json"
				];
			}
			else if ( PAMGW_PlayerID == 2 )
			{
				unit_ids_to_remove = [
					"/pa/units/land/air_factory/air_factory.json"
				];
			}
			else if ( PAMGW_PlayerID == 3 ) // JORDAN
			{
				unit_ids_to_remove = [
					"/pa/units/land/bot_factory/bot_factory.json"
				];
			}

			for( var i=0, len = payload_ids.length; i < len; i++)
			{
				var unit_key = payload_ids[ i ];

				var unit = payload[ unit_key ];

				// Unit has no builds to begin with
				if ( typeof payload[ unit_key ][ 'build' ] == 'undefined' )
				{
					continue;
				}

				payload[ unit_key ][ 'build' ] = _.difference( payload[ unit_key ][ 'build' ], unit_ids_to_remove );
			}

			api.panels.build_bar.message( 'unit_specs', payload );
		}
		catch ( e )
		{
			console.log( e );
		}

		model.default_unit_specs( payload );
	}
}
PAMGW_AlterBuildUI();