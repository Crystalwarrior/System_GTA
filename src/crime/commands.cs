function serverCmdAddDemerits(%this, %amt, %name) {
	if(!%this.isAdmin) {
		return;
	}

	%user = GTAData.collection("users").document(%this.getDocumentName());
	if(%amt !$= mFloor(%amt)) {
		return;
	}
	if(isObject(%target = findClientByName(%name))) {
		messageClient(%this, '', "\c6You added\c3" SPC %amt SPC "\c6demerits to" SPC %target.name);
		%target.addDemerits(%amt);
	}
}

function serverCmdSetDemerits(%this, %amt, %name)
{
	if(!%this.isAdmin)
	return;

	%user = GTAData.collection("users").document(%this.getDocumentName());

	if(%amt !$= mFloor(%amt))
	return;

	if(isObject(%target = findClientByName(%name)))
	{
		messageClient(%this, '', "\c6You set" SPC %target.name @ "'s demerits to\c3" SPC %amt);
		%target.setDemerits(%amt);
	}
}

function serverCmdPardon(%this, %name) {
	if(!%this.isAdmin) {
		return;
	}

	if(isObject(%target = findClientByName(%name))) {
		%user = GTAData.collection("users").document(%target.getDocumentName());
		if(%user.get("jailtimer") <= 0) {
			messageClient(%this, '', "\c6Target" SPC %target.name SPC "is not in jail!");
			return;
		}
		%user.set("jailtimer", 0);
		%target.instantRespawn();
		cancel(%target.jailTimerSchedule);
		messageClient(%this, '', "\c6You have set" SPC %target.name SPC "free.");
		messageClient(%target, '', "\c6" @ %this.name SPC "has set you free!");
	}
}

function serverCmdRecord(%this)
{
	%user = GTAData.collection("users").document(%this.getDocumentName());
	%bleh = (%user.get("record") ? "clean" : "dirty");
	messageClient(%this, '', "\c6Your record is" SPC %bleh @ ".");
}

function serverCmdSetRecord(%this, %name, %tog)
{
	if(!%this.isAdmin) {
		return;
	}
	if(%tog != 0 && %tog != 1)
	{
		messageClient(%this, '', "\c6Invalid input! (Put either 1 or 0)");
		return;
	}

	%bleh = (%tog ? "clean" : "dirty");
	if(isObject(%target = findClientByName(%name))) {
		%user = GTAData.collection("users").document(%target.getDocumentName());
		%user.set("record", %tog);
		messageClient(%target, '', "\c6Your record was set to" SPC %bleh @ ".");
		messageClient(%this, '', "\c6You have set" SPC %target.name @ "'s record to" SPC %bleh @ ".");
	}
	else
	{
		%user = GTAData.collection("users").document(%this.getDocumentName());
		%user.set("record", %tog);
		%bleh = (%tog ? "clean" : "dirty");
		messageClient(%this, '', "\c6Your record was set to" SPC %bleh @ ".");
	}
}

function serverCmdBeCop(%this) {
	%user = GTAData.collection("users").document(%this.getDocumentName());
	if(%user.get("job") $= "cop") {
		messageClient(%this, '', "\c6You are already a cop.");
		return;
	}

	if(%user.get("record") < 1) {
		if(%this.getWantedLevel() > 0) {
			messageClient(%this, '', "\c6You are wanted!");
			return;
		}
		if(%user.get("money") < 1000) {
			messageClient(%this, '', "\c6Your record is dirty. You can only become a cop if you have \c3$1000\c6.");
			return;
		}
		else {
			%this.addMoney(-1000);
		}
	}
	%user.set("job", "cop");
	%user.set("record", 1);
	%this.player.applyCopAppearance();
	messageClient(%this, '', "\c6Congratulations! You have become a cop.");
	%this.instantRespawn();
}

function serverCmdDemote(%this, %name) {
	if(!%this.isAdmin) {
		return;
	}

	if(isObject(%target = findClientByName(%name))) {
		messageClient(%this, '', "\c6You have demoted" SPC %target.name @ ".");
		messageClient(%target, '', "\c6You have been demoted.");
		%user = GTAData.collection("users").document(%target.getDocumentName());
		%user.set("record", 0); //dirtify their record yo
		if(%user.get("job") $= "cop") {
			%user.set("job", "");
		}
		%target.instantRespawn();
	}
}

function serverCmdRetire(%this)
{
	%user = GTAData.collection("users").document(%this.getDocumentName());
	if(%user.get("job") $= "cop")
	{
		%user.set("job", "");
		%this.instantRespawn();
		messageClient(%this, '', "\c6You have retired from the cop job.");
	}
}