registerOutputEvent("GameConnection", "BreakOut", "", 1);

function GameConnection::Breakout(%this)
{
	%user = GTAData.collection("users").document(%this.getDocumentName());
	if(%user.get("jailtimer") > 0)
	{
		cancel(%this.jailTimerSchedule);
		%this.setDemerits(400);
		%user.set("jailtimer", 0);
		%this.applyBodyParts();
		%this.applyBodyColors();
		messageClient(%this, '', "\c3You broke out of jail!");
		announce("Warning:" SPC %this.getPlayerName() SPC "has escaped the jail. He has gained a wanted level of 5 stars.");
	}
}

//Outputs
registerOutputEvent("fxDTSBrick", "addDemerits", "int -400 400\tbool", 1);
registerOutputEvent("fxDTSBrick", "setDemerits", "int -400 400\tbool", 1);
registerOutputEvent("fxDTSBrick", "checkDemerits", "list == 0 >= 1 > 2 <= 3 < 4 != 5\tint -15000 15000\tstring 8 30", 1);

function fxDTSBrick::addDemerits(%this, %amt, %msg, %client) {
	%owner = %this.getGroup().client;
	if(!%client.isAdmin) {
		echo(%client.name SPC "attempted to use 'addDemerits' event without privelegies.");
		return;
	}

	if(%amt > 400) {
		%amt = 400;
	}
	if(%amt < -400) {
		%amt = -400;
	}

	if(%amt > 0) {
		%add = true;
	}
	else {
		if(%amt == 0) {
			return;
		}
		%add = false;
	}

	%user = GTAData.collection("users").document(%client.getDocumentName());

	%show = %amt;
	if(%show < 0) {
		%show = mAbs(%show);
		if(%user.get("Demerits") < %show) {
			%show = %user.get("Demerits");
		}
	}

	%client.addDemerits(%amt);
	if(%user.get("Demerits") < 0) {
		%client.setDemerits(0);
	}
	if(%user.get("Demerits") > 400) {
		%client.setDemerits(400);
	}

	if(%msg) {
		messageClient(%client, '', "\c6The brick has" SPC (%add ? "added" : "substracted") SPC "\c2$" @ %show @ "\c6.");
	}
}

function fxDTSBrick::setDemerits(%this, %amt, %msg, %client) {
	%owner = %this.getGroup().client;
	if(!%client.isAdmin) {
		echo(%client.name SPC "attempted to use 'setDemerits' event without privelegies.");
		return;
	}

	if(%amt < 0) {
		%amt = 0;
	}
	if(%amt > 400) {
		%amt = 400;
	}

	%user = GTAData.collection("users").document(%client.getDocumentName());
	%client.setDemerits(%amt);
	if(%msg) {
		messageClient(%client, '', "\c6The brick has set your Demerits to \c2$" @ %amt @ "\c6.");
	}
}

function fxDTSBrick::checkDemerits(%this, %list, %int, %eventnums, %client) {
	%user = GTAData.collection("users").document(%client.getDocumentName());
	switch(%list) {
		case 0:
		if(%user.get("Demerits") == %int) {
			%pass = true;
		}
		case 1:
		if(%user.get("Demerits") >= %int) {
			%pass = true;
		}
		case 2:
		if(%user.get("Demerits") > %int) {
			%pass = true;
		}
		case 3:
		if(%user.get("Demerits") <= %int) {
			%pass = true;
		}
		case 4:
		if(%user.get("Demerits") < %int) {
			%pass = true;
		}
		case 5:
		if(%user.get("Demerits") != %int) {
			%pass = true;
		}
	}
	if(getWordCount(%eventnums) != 2) {
		if(%pass) {
			%this.onDemeritsCheckTrue(%client);
		}
		else {
			%this.onDemeritsCheckFalse(%client);
		}
		return;
	}

	%eventmin = getWord(%eventnums, 0);
	%eventmax = getWord(%eventnums, 1);
	%max = %this.numEvents;
	for(%i = 0; %i < %max; %i++) {
		%input = %this.eventInput[%i];
		
		if(%this.eventInput[%i] $= "onDemeritsCheckTrue" || %this.eventInput[%i] $= "onDemeritsCheckFalse") {
			if(%i < %eventmin || %i > %eventmax) {
				%old[%i] = %this.eventEnabled[%i];
				%this.eventEnabled[%i] = 0;
			}
		}
	}

	if(%pass) {
		%this.onDemeritsCheckTrue(%client);
	}
	else {
		%this.onDemeritsCheckFalse(%client);
	}
	for(%i = 0; %i < %max; %i++) {
		if(%old[%i] !$= "") {
			%this.eventEnabled[%i] = %old[%i];
		}
	}
}

//Inputs
registerInputEvent("fxDTSBrick","onDemeritsCheckTrue","Self fxDTSBrick\tPlayer Player\tClient GameConnection\tMiniGame MiniGame");
registerInputEvent("fxDTSBrick","onDemeritsCheckFalse","Self fxDTSBrick\tPlayer Player\tClient GameConnection\tMiniGame MiniGame");

function fxDTSBrick::onDemeritsCheckTrue(%obj, %client)
{
	//setup targets
	$InputTarget_["Self"]   = %obj;
	$InputTarget_["Player"] = %client.player;
	$InputTarget_["Client"] = %client;

	if($Server::LAN) {
		$InputTarget_["MiniGame"] = getMiniGameFromObject(%client);
	}
	else {
		if(getMiniGameFromObject(%obj) == getMiniGameFromObject(%client)) {
			$InputTarget_["MiniGame"] = getMiniGameFromObject(%obj);
		}
		else {
			$InputTarget_["MiniGame"] = 0;
		}
	}
	//process the event
	%obj.processInputEvent("OnDemeritsCheckTrue", %client);
}

function fxDTSBrick::onDemeritsCheckFalse(%obj, %client)
{
	//setup targets
	$InputTarget_["Self"]   = %obj;
	$InputTarget_["Player"] = %client.player;
	$InputTarget_["Client"] = %client;

	if($Server::LAN) {
		$InputTarget_["MiniGame"] = getMiniGameFromObject(%client);
	}
	else {
		if(getMiniGameFromObject(%obj) == getMiniGameFromObject(%client)) {
			$InputTarget_["MiniGame"] = getMiniGameFromObject(%obj);
		}
		else {
			$InputTarget_["MiniGame"] = 0;
		}
	}
	//process the event
	%obj.processInputEvent("OnDemeritsCheckFalse", %client);
}