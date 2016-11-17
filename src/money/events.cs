//Outputs
registerOutputEvent("fxDTSBrick", "addMoney", "int -99999999 99999999\tbool", 1);
registerOutputEvent("fxDTSBrick", "setMoney", "int -99999999 99999999\tbool", 1);
registerOutputEvent("fxDTSBrick", "checkMoney", "list == 0 >= 1 > 2 <= 3 < 4 != 5\tint -15000 15000\tstring 8 30", 1);

function fxDTSBrick::addMoney(%this, %amt, %msg, %client) {
	%owner = %this.getGroup().client;
	if(!%client.isAdmin) {
		echo(%client.name SPC "attempted to use 'addMoney' event without privelegies.");
		return;
	}

	if(%amt > 99999999) {
		%amt = 99999999;
	}
	if(%amt < -99999999) {
		%amt = -99999999;
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
		if(%user.get("money") < %show) {
			%show = %user.get("money");
		}
	}

	%client.addMoney(%amt);
	if(%user.get("money") < 0) {
		%client.setMoney(0);
	}
	if(%user.get("money") > 99999999) {
		%client.setMoney(99999999);
	}

	if(%msg) {
		messageClient(%client, '', "\c6The brick has" SPC (%add ? "added" : "substracted") SPC "\c2$" @ %show @ "\c6.");
	}
}

function fxDTSBrick::setMoney(%this, %amt, %msg, %client) {
	%owner = %this.getGroup().client;
	if(!%client.isAdmin) {
		echo(%client.name SPC "attempted to use 'setMoney' event without privelegies.");
		return;
	}

	if(%amt < 0) {
		%amt = 0;
	}
	if(%amt > 99999999) {
		%amt = 99999999;
	}

	%user = GTAData.collection("users").document(%client.getDocumentName());
	%client.setMoney(%amt);
	if(%msg) {
		messageClient(%client, '', "\c6The brick has set your money to \c2$" @ %amt @ "\c6.");
	}
}

function fxDTSBrick::checkMoney(%this, %list, %int, %eventnums, %client) {
	%user = GTAData.collection("users").document(%client.getDocumentName());
	switch(%list) {
		case 0:
		if(%user.get("money") == %int) {
			%pass = true;
		}
		case 1:
		if(%user.get("money") >= %int) {
			%pass = true;
		}
		case 2:
		if(%user.get("money") > %int) {
			%pass = true;
		}
		case 3:
		if(%user.get("money") <= %int) {
			%pass = true;
		}
		case 4:
		if(%user.get("money") < %int) {
			%pass = true;
		}
		case 5:
		if(%user.get("money") != %int) {
			%pass = true;
		}
	}
	if(getWordCount(%eventnums) != 2) {
		if(%pass) {
			%this.onMoneyCheckTrue(%client);
		}
		else {
			%this.onMoneyCheckFalse(%client);
		}
		return;
	}

	%eventmin = getWord(%eventnums, 0);
	%eventmax = getWord(%eventnums, 1);
	%max = %this.numEvents;
	for(%i = 0; %i < %max; %i++) {
		%input = %this.eventInput[%i];
		
		if(%this.eventInput[%i] $= "onMoneyCheckTrue" || %this.eventInput[%i] $= "onMoneyCheckFalse") {
			if(%i < %eventmin || %i > %eventmax) {
				%old[%i] = %this.eventEnabled[%i];
				%this.eventEnabled[%i] = 0;
			}
		}
	}

	if(%pass) {
		%this.onMoneyCheckTrue(%client);
	}
	else {
		%this.onMoneyCheckFalse(%client);
	}
	for(%i = 0; %i < %max; %i++) {
		if(%old[%i] !$= "") {
			%this.eventEnabled[%i] = %old[%i];
		}
	}
}

//Inputs
registerInputEvent("fxDTSBrick","onMoneyCheckTrue","Self fxDTSBrick\tPlayer Player\tClient GameConnection\tMiniGame MiniGame");
registerInputEvent("fxDTSBrick","onMoneyCheckFalse","Self fxDTSBrick\tPlayer Player\tClient GameConnection\tMiniGame MiniGame");

function fxDTSBrick::onMoneyCheckTrue(%obj, %client)
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
	%obj.processInputEvent("OnMoneyCheckTrue", %client);
}

function fxDTSBrick::onMoneyCheckFalse(%obj, %client)
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
	%obj.processInputEvent("OnMoneyCheckFalse", %client);
}