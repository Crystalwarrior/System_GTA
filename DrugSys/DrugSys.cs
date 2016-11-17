if(!isObject("brickDrugBagData"))
{
	exec("./bricks.cs");
	exec("./items/items.cs");
}

package DrugMod
{
	function Gameconnection::centerPrint(%client, %msg, %delay)
	{
		Parent::centerPrint(%client, %msg, %delay);

		%client.centerPrint = %msg;

		if(%delay > 0)
		schedule(1000 * %delay, 0, eval, %client @ ".centerPrint = \"\";");
	}

	function Gameconnection::extendCenterprint(%client, %msg, %delay)
	{
		cancel(%client.extendSchedule);
		if(%client.centerPrint !$= "")
		{
			commandToClient(%client, 'centerPrint', (%msg @ %client.centerPrint));
			%client.extendSchedule = schedule(1000 * %delay, 0, commandToClient, %client, 'centerPrint', %client.centerPrint);
		}
			else
		%client.centerPrint(%msg, %delay);
	}

	function fxDTSBrick::onActivate(%brick, %player, %loc, %rot)
	{
		Parent::onActivate(%brick, %player, %loc, %rot);

		%user = GTAData.collection("users").document(%player.client.getDocumentName());

		if(%brick.getDatablock().getName() $= "brickDrugBagData")
		{
			if(%brick.getName() $= "_Weed")
			{
				if(%user.get("money") >= 20)
				commandToClient(%player.client, 'MessageBoxYesNo', "Buy this product?", "Are you sure you'd like to purchase Marijuana seeds for $20?", 'buyDrug');
			else
				commandToClient(%player.client, 'MessageBoxOK', "Insufficent funds!", "<just:center>This bag of Marijuana seeds costs $20.");
			}

			if(%brick.getName() $= "_Opium")
			{
				if(%user.get("money") >= 100)
				commandToClient(%player.client, 'MessageBoxYesNo', "Buy this product?", "Are you sure you'd like to purchase Opium seeds for $100?", 'buyDrug');
			else
				commandToClient(%player.client, 'MessageBoxOK', "Insufficent funds!", "<just:center>This bag of Opium seeds costs $100.");
			}
		}

		if(%brick.getDatablock().getName() $= "brickDrugCrateData")
		{
			%player.client.extendCenterprint("<color:D9B700>If you have a proccessed drug, you can come here to sell it.", 2);
		}

		if(%brick.getDatablock().isDrug)
		{
			%data = %brick.getDatablock().getName();

			if(%data $= "brickWeedAData" || %data $= "brickOpiumAData")
			{
				%elapsedTime = getSimTime() - %brick.spawnTime;
				%percentage = mFloatLength(%elapsedTime / (%data $= "brickWeedAData" ? 9000 : 12000), 1);

				%player.client.extendCenterprint("<color:D9B700>" @ ( %elapsedTime > (%data $= "brickWeedAData" ? 450000 : 600000) ? "50.0" : %percentage ) @ "%", 2);

				return;
			}

			if(%data $= "brickWeedBData" || %data $= "brickOpiumBData")
			{
				if(!%brick.watered)
				{
					%player.client.extendCenterprint("<color:1069FF>Brick needs to be watered!", 2);
				}
			else
				{
					%elapsedTime = getSimTime() - %brick.spawnTime;
					%percentage = mFloatLength(%elapsedTime / (%data $= "brickWeedBData" ? 9000 : 12000)+50, 1);

					%player.client.extendCenterprint("<color:D9B700>" @ ( %elapsedTime > (%data $= "brickWeedAData" ? 450000 : 600000) ? "100.0" : %percentage ) @ "%", 2);
				}

				return;
			}

			if(%data $= "brickWeedCData" || %data $= "brickOpiumCData")
			{
				for(%i=0;%i<%player.getDatablock().maxTools;%i++)
				{
					if(%player.tool[%i] == 0)
					break;
				}

				if(%i == %player.getDatablock().maxTools)
				{
					%player.client.extendCenterprint("Your inventory is full!",1);
					return;
				}

				%image = (%data $= "brickWeedCData" ? MarijuanaItem.getID() : HeroinItem.getID());

				%player.tool[%i] = %image;
				%player.weaponCount++;

				messageClient(%player.client,'MsgItemPickup','',%i,%image);

				%brick.killBrick();
				%player.client.extendCenterprint("<color:D9B700>Drug harvested.", 2);

				return;
			}
		}
	}

	function serverCmdBuyDrug(%client)
	{
		%player = %client.player;

		%mask = $TypeMasks::FxBrickObjectType;
		%start = %player.getEyePoint();
		%aimVec = %player.getEyeVector();
		%range = 5;
		%end = vectorAdd(%start, vectorScale(%aimVec, %range));

		%brick = getWord(containerRaycast(%start, %end, %mask, %player), 0);

		if(isObject(%brick))
		{
			if(%brick.getDatablock().getName() $= "brickDrugBagData")
			{
				for(%i=0;%i<%player.getDatablock().maxTools;%i++)
				{
					if(%player.tool[%i] == 0)
					break;
				}

				if(%i == %player.getDatablock().maxTools)
				{
					%client.extendCenterprint("Your inventory is full!",1);
					return;
				}

				%user = GTAData.collection("users").document(%player.client.getDocumentName());

				if(%brick.getName()  $= "_Weed")
				{
					if(%user.get("money") >= 20)
					{
						%player.client.addMoney(-20);
						%image = HempBagItem.getID();

						%player.tool[%i] = %image;
						%player.weaponCount++;

						messageClient(%client,'MsgItemPickup','',%i,%image);
						return;
					}
				}

				if(%brick.getName()  $= "_Opium")
				{
					if(%user.get("money") >= 100)
					{
						%player.client.addMoney(-100);
						%image = OpiumBagItem.getID();

						%player.tool[%i] = %image;
						%player.weaponCount++;

						messageClient(%client,'MsgItemPickup','',%i,%image);
						return;
					}
				}
			}
		}
	}

	function Player::plantTempDrug(%player, %client, %drug)
	{
		if(isObject(%player))
		{
			cancel(%player.updateTempDrug);

			%masks = $TypeMasks::FxBrickObjectType | $TypeMasks::TerrainObjectType;

			%start = %player.getEyePoint();
			%aimVec = %player.getEyeVector();
			%range = 5;
			%end = vectorAdd(%start, vectorScale(%aimVec, %range));

			%ray = containerRaycast(%start, %end, %masks, %player);

			%obj = getWord(%ray, 0);
			%pos = getWords(%ray, 1, 2) SPC getWord(%ray, 3)+1;

			if(isObject(%obj))
			{
				%temp = %player.tempBrick;

				if(isObject(%temp))	
				if(%temp.getDatablock().isDrug)
				if(%temp.getDatablock().Drug $= %drug)
				%temp.setTransform(%pos);
			else
				%temp.delete();

				if(!isObject(%temp))
				{
					%player.tempBrick = new fxDTSBrick()
					{
						datablock = "brick" @ %drug @ "CData";
						position = %pos;
						client = %client;
						rotation = %temp.rotation;
					};

					("BrickGroup_" @ %client.bl_id ).add(%player.tempBrick);
					%player.tempBrick.setTrusted(1);
				}
			}
		}
	}

	function OpiumBagImage::onFire(%this, %player, %slot)
	{
		%player.plantTempDrug(%player.client, "Opium");
	}

	function HempBagImage::onFire(%this, %player, %slot)
	{
		%player.plantTempDrug(%player.client, "Weed");
	}

	function MarijuanaImage::onFire(%this, %player, %slot)
	{
		%player.sellDrug("Marijuana");
	}

	function HeroinImage::onFire(%this, %player, %slot)
	{
		%player.sellDrug("Heroin");
	}

	function Player::sellDrug(%player, %drug)
	{
		%mask = $TypeMasks::FxBrickObjectType;
		%start = %player.getEyePoint();
		%aimVec = %player.getEyeVector();
		%range = 5;
		%end = vectorAdd(%start, vectorScale(%aimVec, %range));

		%brick = getWord(containerRaycast(%start, %end, %mask, %player), 0);

		if(isObject(%brick))
		{
			if(%brick.getDatablock().getName() $= "brickDrugCrateData")
			{
				%player.tool[%player.currTool] = 0;
				messageClient(%player.client, 'MsgItemPickup', '', %player.currTool ,0);

				%player.updateArm(0);
				%player.unMountImage(0);

				if(%drug $= "Marijuana")
				{
					%player.client.addMoney(60);
					DrugCrateRegistry_Randomize();
				}

				if(%drug $= "Heroin")
				{
					%player.client.addMoney(350);
					DrugCrateRegistry_Randomize();
				}
			}
		}
	}

	function serverCmdUseTool(%client, %slot)
	{
		Parent::serverCmdUseTool(%client, %slot);

		%temp = (%client.player).tempBrick;

		if(isObject(%temp))
		{
			if(%temp.getDatablock().isDrug)
			%temp.delete();
		}
	}

	function serverCmdunUseTool(%client)
	{
		Parent::serverCmdunUseTool(%client);

		%temp = (%client.player).tempBrick;

		if(isObject(%temp))
		{
			if(%temp.getDatablock().isDrug)
			%temp.delete();
		}
	}

	function serverCmdShiftBrick(%client, %x, %y, %z)
	{
		%temp = (%client.player).tempBrick;

		if(isObject(%temp))
		{
			if(%temp.getDatablock().isDrug)
			{
				%client.extendCenterprint("You can't shift a drug brick.", 2);
				return;
			}
		}
		Parent::serverCmdShiftBrick(%client, %x, %y, %z);
	}

	function serverCmdSuperShiftBrick(%client, %x, %y, %z)
	{
		%temp = (%client.player).tempBrick;

		if(isObject(%temp))
		{
			if(%temp.getDatablock().isDrug)
			{
				%client.extendCenterprint("You can't shift a drug brick.", 2);
				return;
			}
		}
		Parent::serverCmdSuperShiftBrick(%client, %x, %y, %z);
	}

	function serverCmdPlantBrick(%client)
	{
		%temp = (%client.player).tempBrick;

		if(isObject(%temp))
		{
			if(%temp.getDatablock().isDrug)
			{
				initContainerBoxSearch(%temp.position, "0.4 0.4 1.9", $TypeMasks::FxBrickAlwaysObjectType);

				while(containerSearchNext())
				{
					messageClient(%client, 'MsgPlantError_Overlap');
					return;
				}

				%brick = new fxDtsBrick()
				{
					client = %client;

					dataBlock = "brick" @ %temp.getDatablock().Drug @ "AData";
					position = %temp.position;
					rotation = %temp.rotation;

					isPlanted = 1;
					stackBL_ID = %temp.stackBL_ID;
				};

				if(%err = %brick.plant())
				{
					%brick.delete();

					switch(%err)
					{
						case 1: %cmd = 'MsgPlantError_Overlap';
						case 2: %cmd = 'MsgPlantError_Float';
						case 3: %cmd = 'MsgPlantError_Stuck';
						case 4: %cmd = 'MsgPlantError_Unstable';
						case 5: %cmd = 'MsgPlantError_Buried';
						case 6: %cmd = 'MsgPlantError_TooFar';
						case 7: %cmd = 'MsgPlantError_TooLoud';
						case 8: %cmd = 'MsgPlantError_Limit';
					}

					messageClient(%client, %cmd);

					return;
				}

				("Brickgroup_" @ %client.bl_id ).add(%brick);
				%brick.setTrusted(1);
				%temp.delete();
				%client.playSound("brickPlantSound");

				%client.player.tool[%client.player.currTool] = 0;
				messageClient(%client, 'MsgItemPickup', '', %client.player.currTool ,0);

				%client.player.updateArm(0);
				%client.player.unMountImage(0);
			}
		}

		Parent::serverCmdPlantBrick(%client);
	}

	function fxDTSBrick::onPlant(%brick, %a)
	{
		Parent::onPlant(%brick);

		if(%brick.getDatablock().isDrug)
		{
			if(%brick.getDatablock().getName() $= "brickWeedAData") //15 minutes total
			{
				%brick.spawnTime = getSimTime();
				%brick.schedule(450000, growDrug);
			}

			if(%brick.getDatablock().getName() $= "brickOpiumAData") //20 minutes total
			{
				%brick.spawnTime = getSimTime();
				%brick.schedule(600000, growDrug);
			}
		}

		if(%brick.getDatablock().getName() $= "brickDrugCrateData")
		DrugCrateRegistry_Add(%brick);
	}

	function fxDTSBrick::onLoadPlant(%brick)
	{
		Parent::onLoadPlant(%brick);

		if(%brick.getDatablock().getName() $= "brickDrugCrateData")
		%brick.onPlant(%brick);
	}

	function fxDTSBrick::onDeath(%brick)
	{
		Parent::onDeath(%brick);

		if(%brick.getDatablock().getName() $= "brickDrugCrateData")
		DrugCrateRegistry_Remove(%brick);
	}

	function fxDTSBrick::onRemove(%brick)
	{
		Parent::onRemove(%brick);

		if(%brick.getDatablock().getName() $= "brickDrugCrateData")
		DrugCrateRegistry_Remove(%brick);
	}

	function DrugCrateRegistry_Add(%brick)
	{
		if(!isObject(DrugCrateRegistry))
		new ScriptObject(DrugCrateRegistry);

		DrugCrateRegistry.brick[%brick] = %brick;
	}

	function DrugCrateRegistry_Remove(%brick)
	{
		if(isObject(DrugCrateRegistry))
		{
			if(DrugCrateRegistry.brick[%brick] == %brick)
			DrugCrateRegistry.brick[%brick] = "";
		}
	}

	function DrugCrateRegistry_Randomize()
	{
		if(isObject(DrugCrateRegistry))
		{
			for(%i=0;%i<DrugCrateRegistry.getTagCount();%i++)
			{
				%brick = getField(DrugCrateRegistry.getTaggedField(%i), 1);

				%brick.setRaycasting(0);
				%brick.setColliding(0);
				%brick.setRendering(0);
			}

			for(%i=0;%i<5;%i++)
			{
				%rBrick = getField(DrugCrateRegistry.getTaggedField(getRandom(0, DrugCrateRegistry.getTagCount()-1)), 1);
				%rBrick.setRaycasting(1);
				%rBrick.setColliding(1);
				%rBrick.setRendering(1);
			}
		}
	}

	function fxDTSBrick::onProjectileHit(%brick, %projectile, %client)
	{
		Parent::onProjectileHit(%brick, %projectile, %client);

		if(%brick.getDatablock().isDrug)
		{
			if(%projectile.getDatablock().getName() $= "wateringCanProjectile")
			{
				if(%brick.getDatablock().getName() $= "brickWeedBData")
				{
					if(!%brick.watered)
					{
						%brick.watered = 1;
						%client.extendCenterprint("<color:1069FF>Plant watered!", 2);

						%brick.spawnTime = getSimTime();
						%brick.schedule(450000, growDrug);
					}
				}

				if(%brick.getDatablock().getName() $= "brickOpiumBData")
				{
					if(!%brick.watered)
					{
						%brick.watered = 1;
						%client.extendCenterprint("<color:1069FF>Plant watered!", 2);

						%brick.spawnTime = getSimTime();
						%brick.schedule(600000, growDrug);
					}
				}
			}
		}
	}

	function fxDTSBrick::growDrug(%brick)
	{
		%data = %brick.getDatablock().getName();

		if(%data $= "brickWeedAData")
		%brick.setDatablock("brickWeedBData");

		if(%data $= "brickWeedBData")
		%brick.setDatablock("brickWeedCData");

		if(%data $= "brickOpiumAData")
		%brick.setDatablock("brickOpiumBData");

		if(%data $= "brickOpiumBData")
		%brick.setDatablock("brickOpiumCData");
	}
};
activatePackage(DrugMod);