package GTAPackage
{
	function gameConnection::spawnPlayer(%this)
	{
		Parent::spawnPlayer(%this);

		%this.GTATimeStatusLoop(); //Also updates display regardless of daycycle.
		%user = GTAData.collection("users").document(%this.getDocumentName());

		if(%user.get("demerits") >= $GTA::Demerits::wantedLevel)
		{
			%this.player.setShapeNameColor("1 0 0");
			%user.set("record", 0); //dirtify their record yo

			if(%user.get("job") $= "cop")
			{
				messageClient(%this, '', "\c6You have been demoted.");

				%user.set("job", "");
				%this.applyBodyParts();
				%this.applyBodyColors();
			}
		}
			else
			%this.player.setShapeNameColor("0 1 0");

		if(%user.get("jailtimer") > 0) {
			%this.jailTimerSchedule();
			for(%i = 0; %i < %this.player.getDatablock().maxTools; %i++)
			{
				%this.player.tool[%i] = "";
				messageClient(%this, 'MsgItemPickup', "", %i, "");
			}
		}

		if(%user.get("job") $= "cop") {
			%this.player.setShapeNameColor("0 0.5 1");
			%tools = "BatonItem TaserItem";
			for(%i = 0; %i < %this.player.getDatablock().maxTools; %i++)
			{
				if(%a >= getWordCount(%tools)) return;
				if(%this.player.tool[%i] <= 0)
				{
					%this.player.tool[%i] = nameToID(getWord(%tools, %a));
					messageClient(%this, 'MsgItemPickup', "", %i, nameToID(getWord(%tools, %a)));
					%a++;
				}
			}
		}
	}
	function serverCmdEnvGui_SetVar(%this, %variable, %value) {
		parent::serverCmdEnvGui_SetVar(%this, %variable, %value);
		if(%variable $= "DayCycleEnabled") {
			for (%i = 0; %i < ClientGroup.getCount(); %i++) {
				%current = ClientGroup.getObject(%i);
				%current.GTATimeStatusLoop();
			}
		}
	}

	function GameConnection::applyBodyParts(%this) {
		Parent::applyBodyParts(%this);
		%user = GTAData.collection("users").document(%this.getDocumentName());
		if(%user.get("jailtimer") > 0)
		{
			%this.player.applyJailAppearance();
			return;
		}
		if(%user.get("job") $= "cop") {
			%this.player.applyCopAppearance();
		}
	}
	function GameConnection::applyBodyColors(%this) {
		Parent::applyBodyColors(%this);
		%user = GTAData.collection("users").document(%this.getDocumentName());
		if(%user.get("jailtimer") > 0)
		{
			%this.player.applyJailAppearance();
			return;
		}
		if(%user.get("job") $= "cop") {
			%this.player.applyCopAppearance();
		}
	}

	function fxDTSBrick::onActivate(%this, %player, %client, %pos, %vec)
	{
		%data = %this.getDataBlock();
		if(%data.activateTrig)
		{
			%data.onActivateTrig(%this, %player);
		}
		parent::onActivate(%this, %player, %client, %pos, %vec);
	}

	function serverCmdSuicide(%this)
	{
		// if(%this.)
		parent::serverCmdSuicide(%this);
	}
};
activatePackage(GTAPackage);