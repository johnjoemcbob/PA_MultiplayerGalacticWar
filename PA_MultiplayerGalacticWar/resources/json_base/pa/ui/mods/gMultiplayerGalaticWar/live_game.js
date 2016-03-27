// Currently based on https://palobby.com/mods/gUnitSelector/ui/mods/gUnitSelector/live_game.js
// https://forums.uberent.com/threads/rel-server-lobby-live-game-unit-selector.67177/
//
// Using this as a reference for how to change the build menus of individual commanders

console.log( 'gUnitSelector live_game last tested on 77443' );

model.gUnitSelector_handlers_unit_specs = handlers.unit_specs;

handlers.unit_specs = function (payload)
{
  console.log( sessionStorage.displayName );
  console.log( 'gUnitSelector live_game unit_specs' );
  
  try
  {
    var payload_ids = _.keys( payload );
  
    var unit_ids_to_remove = [];
	if ( sessionStorage.displayName == '"Prof. Bespoke"' )
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

    console.log( unit_ids_to_remove );
  
    for( var i=0, len = payload_ids.length; i < len; i++)
    {
      var unit_key = payload_ids[ i ];
      
      var unit = payload[ unit_key ];
   
      console.log( payload[ unit_key ][ 'build' ] );
      payload[ unit_key ][ 'build' ] = _.difference( payload[ unit_key ][ 'build' ], unit_ids_to_remove ) ;
  
    }
  
    api.panels.build_bar.message( 'unit_specs', payload );
  }
  catch ( e )
  {
    console.log( e );
  }
  
  model.gUnitSelector_handlers_unit_specs( payload );
  
}