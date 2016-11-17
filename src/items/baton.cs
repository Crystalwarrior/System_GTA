// ============================================================
// Section 1 : Datablocks
// ============================================================
AddDamageType("Baton",   '<bitmap:Add-Ons/System_GTA/res/shapes/ci/baton> %1',    '%2 <bitmap:Add-Ons/System_GTA/res/shapes/ci/baton> %1', 0.5, 1);
datablock itemData(BatonItem : hammerItem)
{
	category = "Weapon";
	uiName = "Baton";
	image = BatonImage;

	shapeFile = "Add-Ons/System_GTA/res/shapes/baton.dts";
	colorShiftColor = "0.4 0.4 0.6 1.000";
	
	// Properties
	noSpawn = true;
	canArrest = true;
};

datablock shapeBaseImageData(BatonImage : hammerImage)
{
	// SpaceCasts
	raycastWeaponRange = 5;
	raycastWeaponTargets = $TypeMasks::PlayerObjectType
						|$TypeMasks::StaticObjectType
						|$TypeMasks::TerrainObjectType
						|$TypeMasks::VehicleObjectType
						|$TypeMasks::FXBrickObjectType;
	raycastDirectDamage = 25;
	raycastDirectDamageType = $DamageType::Baton;
	raycastExplosionProjectile = hammerProjectile;
	raycastExplosionSound = hammerHitSound;
	minshottime = 300;
	
	shapeFile = "Add-Ons/System_GTA/res/shapes/baton.dts";
	item = BatonItem;
	projectile = hammerProjectile;
	colorShiftColor = BatonItem.colorShiftColor;
	eyeOffset	= "0 0 0";
	showBricks = 0;

	stateName[0]					= "Activate";
	stateTimeoutValue[0]			= 0.5;
	stateTransitionOnTimeout[0]		= "Ready";

	stateName[1]					= "Ready";
	stateTransitionOnTriggerDown[1]	= "PreFire";
	stateAllowImageChange[1]		= true;

	stateName[2]					= "PreFire";
	stateTransitionOnTimeout[2]		= "Fire";
	stateTimeoutValue[2]			= 0.15;
	stateAllowImageChange[2]		= false;

	stateName[3]					= "Fire";
	stateTransitionOnTimeout[3]		= "CheckFire";
	stateTimeoutValue[3]			= 0.2;
	stateFire[3]					= true;
	stateScript[3]					= "onFire";
	stateWaitForTimeout[3]			= true;
	stateSequence[3]				= "";

	stateName[4]					= "CheckFire";
	stateTransitionOnTriggerUp[4]	= "StopFire";
	stateTransitionOnTriggerDown[4]	= "PreFire";

	stateName[5]					= "StopFire";
	stateTransitionOnTimeout[5]		= "Ready";
	stateTimeoutValue[5]			= 0.2;
	stateAllowImageChange[5]		= false;
	stateWaitForTimeout[5]			= true;
	stateSequence[5]				= "";
	stateScript[5]					= "onStopFire";
};

// ============================================================
// Section 2 : Functions
// ============================================================

// Section 2.1 : Visual Functionality
function BatonImage::onPreFire(%this, %obj, %slot)
{
	%obj.playThread(2, "shiftDown");
}

function BatonImage::onHitObject(%this, %obj, %slot, %col, %pos, %normal)
{
	if(%col.getClassName() $= "Player")
	{
		%client = %obj.client;
		if((%col.getType() & $typeMasks::playerObjectType) && isObject(%col.client))
		{
			if(%col.client.getWantedLevel())
			{
				if(%col.getDatablock().maxDamage - (%col.getDamageLevel() + 25) < %this.raycastDirectDamage)
				{
					%col.setDamageLevel(%this.raycastDirectDamage + 1);
					%reward = %col.client.getWantedLevel() * 100;
					for(%i = 0; %i < %col.getDatablock().maxTools; %i++)
					{
						if(%col.tool[%i].isDrug)
						{
							switch$(%col.tool[%i].getName())
							{
								case "OpiumBagItem": %reward += 100;
								case "MarijuanaItem": %reward += 20;
								case "HeroinItem": %reward += 350;
								case "HempBagItem": %reward += 60;
							}
						}
					}
					%client.addMoney(%reward);
					%client.extendCenterprint("<font:calibri:30>\c3You have recieved a \c2$" @ %reward SPC "\c3reward.", 3);
					%col.client.jail();
				}
				else
				{
					%client.extendCenterprint("<font:calibri:30>\c3" @ %col.client.name SPC "\c6has resisted arrest!", 3);
				}
			}
			else
			{
				%doNoEvil = true;
			}
		}
	}
	else if(%col.getClassname() $= "WheeledVehicle")
	{
		for (%i = 0; %i < %col.getDataBlock().numMountPoints; %i++)
		{
			%pl = %col.getMountedObject(%i);
			if (isObject(%pl))
			{
				%pl.dismount();
			}
		}
	}
	else if(%col.getClassname() $= "fxDTSBrick")
	{
		if(!%col.getDatablock().isDrug)
			return;
		switch$(%col.getDatablock().Drug)
		{
			case "Weed":
				%col.client.addDemerits(25, 1);
				%money = 60;
			case "Opium":
				%col.client.addDemerits(75, 1);
				%money = 350;
			default:
				%col.client.addDemerits(25, 1); //Second var is bool. Means no wanted flash/witness checks.
				%money = 25;
		}
		if(%col.client != %obj.client)
		{
			%obj.client.addMoney(%money);
		}

		messageClient(%col.client, '', "\c3Your" SPC %col.getDatablock().drug SPC "plant was destroyed!");
		%col.killBrick();
	}

	if(%doNoEvil) { %this.raycastDirectDamage = 0; }
	parent::onHitObject(%this, %obj, %slot, %col, %pos, %normal);
	if(%doNoEvil) { %this.raycastDirectDamage = 25; }
}

function BatonItem::onPickup(%this, %item, %obj)
{
	if(isObject(%obj.client) && GTAData.collection("users").document(%obj.client.getDocumentName()).get("job") $= "cop") {
		parent::onPickup(%this, %item, %obj);
		return;
	}
}