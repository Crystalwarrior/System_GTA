exec("./DrugSys/DrugSys.cs");
exec("./prefs.cs");

exec("./lib/misc.cs");
exec("./lib/fov.cs");
exec("./lib/daycycle.cs");
exec("./lib/jettison.cs");
exec("./lib/raycastingWeapons.cs");
exec("./lib/database.cs");
DocumentStorage(GTAData, "config/server/GTA/GTAData.dat");
GTAData.export();

exec("./src/package.cs");
exec("./src/display.cs");
exec("./src/datablocks.cs");
exec("./src/misc.cs");
exec("./src/events.cs");

exec("./src/money/main.cs");
exec("./src/crime/main.cs");

exec("./src/items/baton.cs");
exec("./src/items/taser.cs");

package GTAData
{
	function GameConnection::autoAdminCheck(%this) {
		%user = GTAData.collection("users").document(%this.getDocumentName());
		%user.setDefault("money", 0);
		%user.setDefault("bank", 200); //Bank cash.
		%user.setDefault("demerits", 0);
		%user.setDefault("jailtimer", 0);
		%user.setDefault("record", 1);
		return parent::autoAdminCheck(%this);
	}
};
activatePackage(GTAData);