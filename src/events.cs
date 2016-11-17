registerOutputEvent("fxDTSBrick", "checkDayStage", "list == 0 >= 1 > 2 <= 3 < 4 != 5\tlist Dawn 0 Day 1 Dusk 2 Night 3\tstring 8 30", 1);
registerOutputEvent("fxDTSBrick", "checkDayTime", "list == 0 >= 1 > 2 <= 3 < 4 != 5\tstring 5 32\tstring 8 30", 1);

function fxDTSBrick::checkDayStage(%this, %list, %int, %eventnums, %client) {
	%time = getDayCycleTime();
	%stage = getDayCycleStageReal(%time);
	switch(%list) {
		case 0:
		if(%stage == %int) {
			%pass = true;
		}
		case 1:
		if(%stage >= %int) {
			%pass = true;
		}
		case 2:
		if(%stage > %int) {
			%pass = true;
		}
		case 3:
		if(%stage <= %int) {
			%pass = true;
		}
		case 4:
		if(%stage < %int) {
			%pass = true;
		}
		case 5:
		if(%stage != %int) {
			%pass = true;
		}
	}

	if(getWordCount(%eventnums) != 2) {
		if(%pass) {
			%this.onDayCheckTrue(%client);
		}
		else {
			%this.onDayCheckFalse(%client);
		}
		return;
	}

	%eventmin = getWord(%eventnums, 0);
	%eventmax = getWord(%eventnums, 1);
	%max = %this.numEvents;
	for(%i = 0; %i < %max; %i++) {
		%input = %this.eventInput[%i];
		
		if(%this.eventInput[%i] $= "onDayCheckTrue" || %this.eventInput[%i] $= "onDayCheckFalse") {
			if(%i < %eventmin || %i > %eventmax) {
				%old[%i] = %this.eventEnabled[%i];
				%this.eventEnabled[%i] = 0;
			}
		}
	}

	if(%pass) {
		%this.onDayCheckTrue(%client);
	}
	else {
		%this.onDayCheckFalse(%client);
	}
	for(%i = 0; %i < %max; %i++) {
		if(%old[%i] !$= "") {
			%this.eventEnabled[%i] = %old[%i];
		}
	}
}

function fxDTSBrick::checkDayTime(%this, %list, %str, %eventnums, %client) {
	%time = getDayCycleTime();
	%time = getDayCycleTimeString(%time);
	%time = strReplace(%time, ":", " ");
	%str = strReplace(%time, ":", " ");
	%hours = getWord(%time, 0);
	%mins = getWord(%time, 1);
	%strhours = getWord(%str, 0);
	%strmins = getWord(%str, 1);
	switch(%list) {
		case 0:
		if(%hours == %strhours && %mins == %strmins) {
			%pass = true;
		}
		case 1:
		if(%hours >= %strhours && %mins >= %strmins) {
			%pass = true;
		}
		case 2:
		if(%hours > %strhours && %mins > %strmins) {
			%pass = true;
		}
		case 3:
		if(%hours <= %strhours && %mins <= %strmins) {
			%pass = true;
		}
		case 4:
		if(%hours < %strhours && %mins < %strmins) {
			%pass = true;
		}
		case 5:
		if(%hours != %strhours && %mins != %strmins) {
			%pass = true;
		}
	}
	if(getWordCount(%eventnums) != 2) {
		if(%pass) {
			%this.onDayCheckTrue(%client);
		}
		else {
			%this.onDayCheckFalse(%client);
		}
		return;
	}

	%eventmin = getWord(%eventnums, 0);
	%eventmax = getWord(%eventnums, 1);
	%max = %this.numEvents;
	for(%i = 0; %i < %max; %i++) {
		%input = %this.eventInput[%i];
		
		if(%this.eventInput[%i] $= "onDayCheckTrue" || %this.eventInput[%i] $= "onDayCheckFalse") {
			if(%i < %eventmin || %i > %eventmax) {
				%old[%i] = %this.eventEnabled[%i];
				%this.eventEnabled[%i] = 0;
			}
		}
	}

	if(%pass) {
		%this.onDayCheckTrue(%client);
	}
	else {
		%this.onDayCheckFalse(%client);
	}
	for(%i = 0; %i < %max; %i++) {
		if(%old[%i] !$= "") {
			%this.eventEnabled[%i] = %old[%i];
		}
	}
}

//Inputs
registerInputEvent("fxDTSBrick","onDayCheckTrue","Self fxDTSBrick\tPlayer Player\tClient GameConnection\tMiniGame MiniGame");
registerInputEvent("fxDTSBrick","onDayCheckFalse","Self fxDTSBrick\tPlayer Player\tClient GameConnection\tMiniGame MiniGame");

function fxDTSBrick::onDayCheckTrue(%obj, %client)
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
	%obj.processInputEvent("OnDayCheckTrue", %client);
}

function fxDTSBrick::onDayCheckFalse(%obj, %client)
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
	%obj.processInputEvent("OnDayCheckFalse", %client);
}