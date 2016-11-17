function GameConnection::UpdateDisplay(%this) {
	if(!isObject(%this.player) || %this.player.getState() $= "Dead") {
		return;
	}
	%user = GTAData.collection("users").document(%this.getDocumentName());

	if(%user.get("jailtimer") > 0) {
		%str = "<font:calibri:30><color:00AAFF>" @ %user.get("jailtimer") SPC "time left in jail.";
	}

	%str = %str @ "<just:right><font:calibri:30>";

	if($EnvGuiServer::DayCycleEnabled && isObject(DayCycle)) {
		%time = getDayCycleTime();
		%stage = getDayCycleStageReal(%time);
		%stageName = getDayCycleStageName(%stage);
		%str = %str @ "\c6" @ getDayCycleTimeString(%time); //strFirstUpr(%stageName) @ "," SPC
	}
	%health = (1 - %this.player.getDamagePercent()) * 100;

	// %str = %str @ "\n<color:FF5555>HP:" SPC %health;
	if(%this.displayEffect $= "money_flash") {
		%color = "<color:AACC88>";
	}
	else {
		%color = "<color:66AA55>";
	}
	%str = %str @ "<sbreak>" @ %color @ "$" @ %user.get("money");

	%str = %str @ "\n";
	for (%i = 5; %i > 0; %i--) {
		if(%i <= %this.getWantedLevel() && %this.displayEffect !$= "wanted_blink") {
			%str = %str @ "<color:FFFF00>";
		}
		else {
			%str = %str @ "<color:333333>";
		}
		%str = %str @ "*";
	}
	%this.centerPrint(%str, 0);
}

function GameConnection::GTATimeStatusLoop(%this) {
	cancel(%this.GTATimeStatusLoop);
	%this.UpdateDisplay();
	if(!$EnvGuiServer::DayCycleEnabled || !isObject(DayCycle)) {
		return;
	}
	%delay = 64;//60000 * (DayCycle.dayLength / 86400);
	if(%delay < 16) {
		%delay = 16;
	}
	%this.GTATimeStatusLoop = %this.schedule(%delay, GTATimeStatusLoop);
}