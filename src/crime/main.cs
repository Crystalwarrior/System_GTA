exec("./datablocks.cs");
exec("./commands.cs");
exec("./events.cs");

package GTACrimePackage {
	function GameConnection::getSpawnPoint(%this)
	{
		%user = GTAData.collection("users").document(%this.getDocumentName());

		if(!isObject(jailSpawnSet)) {
			new SimSet(jailSpawnSet);
		}
		if(!isObject(copSpawnSet)) {
			new SimSet(copSpawnSet);
		}

		%spawnCount = jailSpawnSet.getCount();
		if(%user.get("jailtimer") > 0 && %spawnCount > 0) {
			%spawn = jailSpawnSet.getObject(getRandom(0,%spawnCount - 1));
			return %spawn.getSpawnPoint();
		}

		%spawnCount = copSpawnSet.getCount();
		if(%user.get("job") $= "cop" && %spawncount > 0) {
			%spawn = copSpawnSet.getObject(getRandom(0,%spawnCount - 1));
			return %spawn.getSpawnPoint();
		}

		return parent::getSpawnPoint(%this);
	}

	function GameConnection::onDeath(%this, %sourcePlayer, %sourceClient, %damageType, %damageArea) {
		if(isObject(%sourceClient) && %sourceClient != %this) {
			%user = GTAData.collection("users").document(%sourceClient.getDocumentName());
			if(%user.get("job") $= "cop" && %this.getWantedLevel() > 0)
			{
				return Parent::onDeath(%this, %sourcePlayer, %sourceClient, %damageType, %damageArea);
			}
			%dems = $GTA::Demerits::Murder;
			if(%this.getWantedLevel() >= 1) {
				%dems = MFloor(%dems * 0.2);
			}
			%sourceClient.addDemerits(%dems);
			%sourceClient.UpdateDisplay();
		}
		Parent::onDeath(%this, %sourcePlayer, %sourceClient, %damageType, %damageArea);
	}

	function Armor::damage(%this, %obj, %src, %pos, %damage, %type) {
		Parent::damage(%this, %obj, %src, %pos, %damage, %type);

		if(isObject(%src.client) && %damage >= 1 && %src.client != %obj.client) {
			%user = GTAData.collection("users").document(%src.client.getDocumentName());
			if(%user.get("job") $= "cop" && %obj.client.getWantedLevel() > 0)
			{
				return;
			}

			if(%damage >= %obj.getDatablock().maxDamage - %obj.getDamageLevel()) {
				%dems = $GTA::Demerits::Murder;
				if(%obj.client.getWantedLevel() >= 1) {
					%dems = MFloor(%dems * 0.2);
				}
				%src.client.addDemerits(%dems);
				%src.client.UpdateDisplay();
			}
			else if(%obj.client.getWantedLevel() <= 0) {
				%src.client.addDemerits($GTA::Demerits::Assault);
				%src.client.UpdateDisplay();
			}
		}
		%obj.client.UpdateDisplay();
	}

	function Player::activateStuff(%this)
	{
		Parent::activateStuff(%this);
		%client = %this.client;
		%user = GTAData.collection("users").document(%client.getDocumentName());
		if(%user.get("job") !$= "cop")
		{
			return;
		}
		%start = %this.getEyePoint();
		%end = vectorAdd(%start, vectorScale(%this.getEyeVector(), 6));

		%mask = $TypeMasks::FxBrickObjectType | $TypeMasks::PlayerObjectType;
		%ray = containerRayCast(%start, %end, %mask, %this);

		if (!%ray || !(%ray.getType() & $TypeMasks::PlayerObjectType))
		{
			return;
		}

		if(%ray.client.getWantedLevel() >= 1)
			return;

		for(%i = 0; %i < %ray.getDatablock().maxTools; %i++)
		{
			if(%ray.tool[%i].isDrug)
			{
				%hippie = true;
			}
		}
		if(%hippie)
		{
			messageClient(%client, '', "\c3" @ %ray.client.getPlayerName() SPC "\c0turns out to have drugs!");
			%ray.client.addDemerits(100);
		}
		else
		{
			messageClient(%client, '', "\c3" @ %ray.client.getPlayerName() SPC "\c2is clean.");
		}
	}
};
activatePackage(GTACrimePackage);

function GameConnection::doWitnessCheck(%this) {
	if(!isObject(%obj = %this.player)) {
		return;
	}

	%seen = 0;
	initContainerRadiusSearch(%obj.getHackPosition(), 100, $TypeMasks::PlayerObjectType);
	while (isObject(%found = containerSearchNext())) {
		if(%found.getState() !$= "Dead" && isObject(%found.client)) {
			if(%found != %obj) {
				%ray = containerRayCast(%found.getEyePoint(), %obj.getHackPosition(),
					$TypeMasks::PlayerObjectType |
					$TypeMasks::fxBrickObjectType,
					%found
				);
				if(isObject(%col = getWord(%ray, 0)) && %col == %obj) {
					%seen = %found.isWithinView(%obj.getHackPosition(), 120);
				}
			}
		}
	}
	return %seen;
}

function GameConnection::doWantedFlash(%this, %flash, %loops) {
	cancel(%this.wantedFlashLoop);
	if(!isObject(%this.player) || %this.player.getState() $= "Dead") {
		%this.displayEffect = "";
		return;
	}
	%user = GTAData.collection("users").document(%this.getDocumentName());
	%loops += 1;
	if(%loops >= 20) {
		%this.displayEffect = "";
		%this.player.setShapeNameColor("1 0 0");
		if(!%this.doWitnessCheck()) {
			%this.player.setShapeNameColor("0 1 0");
			if(%user.get("job") $= "cop") {
				%this.player.setShapeNameColor("0 0.5 1");
			}
			%amt = $GTA::Demerits::wantedLevel * (mClamp(%this.getWantedLevel() - 1, 1, 4));
			%this.setDemerits(%user.get("demerits") - %amt);
		}
		%this.updateDisplay();
		return;
	}

	%flash = !%flash;
	if(%flash) {
		%this.displayEffect = "wanted_blink";
		%this.player.setShapeNameColor("1 0 0");
	}
	else {
		%this.displayEffect = "";
		%this.player.setShapeNameColor("0 1 0");
	}
	%this.updateDisplay();
	%this.wantedFlashLoop = %this.schedule(250, doWantedFlash, %flash, %loops);
}

function GameConnection::addDemerits(%this, %amt, %noflash) {
	%user = GTAData.collection("users").document(%this.getDocumentName());
	%this.setDemerits(%user.get("demerits") + %amt, %noflash);
}

function GameConnection::setDemerits(%this, %amt, %noflash) {
	%user = GTAData.collection("users").document(%this.getDocumentName());
	if(%this.getWantedLevel() != %this.lastWantedLevel) {
		%this.lastWantedLevel = %this.getWantedLevel();
	}
	%user.set("demerits", %amt);
	if(%this.demsMultiplier < 1) {
		%this.demsMultiplier = 1;
	}
	if(%user.get("demerits") < 0) {
		%user.set("demerits", 0);
	}
	
	if(isObject(%this.player)) {
		if(%this.getWantedLevel()) {
			%this.player.setShapeNameColor("1 0 0");
		}
		else {
			%this.player.setShapeNameColor("0 1 0");
		}
	}
	if(%this.getWantedLevel() > %this.lastWantedLevel && !%noflash) {
		%this.doWantedFlash();
	}
	if(%user.get("demerits") > $GTA::Demerits::wantedLevel * %this.demsMultiplier) {
		%this.demsMultiplier = %this.demsMultiplier * 2;
	}
	else if(%user.get("demerits") < $GTA::Demerits::wantedLevel * %this.demsMultiplier) {
		%this.demsMultiplier = %this.demsMultiplier / 2;
	}
	if(%user.get("demerits") > $GTA::Demerits::wantedLevel * 16) {
		%user.set("demerits", 400);
	}

	%this.updateDisplay();
}

function GameConnection::getWantedLevel(%this) {
	%user = GTAData.collection("users").document(%this.getDocumentName());
	%dems = %user.get("demerits");

	if(%dems >= 25) %stars = 1;
	if(%dems >= 50) %stars = 2;
	if(%dems >= 100) %stars = 3;
	if(%dems >= 200) %stars = 4;
	if(%dems >= 400) %stars = 5;

	return %stars;
}

function GameConnection::jailTimerSchedule(%this) {
	cancel(%this.jailTimerSchedule);
	if(!isObject(%this.player) || %this.player.getState() $= "Dead") {
		return;
	}
	%user = GTAData.collection("users").document(%this.getDocumentName());
	if(%user.get("jailtimer") <= 0) {
		%this.instantRespawn();
		%this.addMoney(-100);
		if(%user.get("money") < 0)
			%this.setMoney(0);
		messageClient(%this, '', "\c6The Police Department has taken \c0$100\c6 as a penalty.");
		return;
	}
	%user.set("jailtimer", %user.get("jailtimer") - 1);
	%this.UpdateDisplay();
	%this.jailTimerSchedule = %this.schedule(1000, jailTimerSchedule);
}

function GameConnection::Jail(%this) {
	echo(%this.getWantedLevel());
	if(%this.getWantedLevel() < 1) {
		return;
	}
	%user = GTAData.collection("users").document(%this.getDocumentName());
	%user.set("jailtimer", %this.getWantedLevel() * 120);
	%user.set("demerits", 0);
	%user.set("record", 0); //dirtify their record yo
	if(%user.get("job") $= "cop") {
		%user.set("job", "");
		messageClient(%this, '', "\c6You have been demoted.");
	}
	%this.updateDisplay();
	%this.instantRespawn();
	%this.schedule(1000, jailTimerSchedule);
}