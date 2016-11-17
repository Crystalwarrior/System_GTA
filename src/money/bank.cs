datablock fxDTSBrickData( atmBrickData : brick2x2rampPrintData )
{
	category = "Special";
	subcategory = "GTA";
	
	uiName = "ATM Monitor";

	adminOnly = true;
	activateTrig = true;
};

function atmBrickData::onActivateTrig(%this, %brick, %obj)
{
	if(!isObject(%client = %obj.client))
	{
		return;
	}
	%user = GTAData.collection("users").document(%client.getDocumentName());
	messageClient(%client, '', "\c2ATM\c6: Welcome to ATM," SPC %client.name @ ". You currently have \c2$" @ %user.get("bank") @ "\c6 in the bank.");
	messageClient(%client, '', "\c2ATM\c6: \c3/deposit amt\c6 to deposit cash.");
	messageClient(%client, '', "\c2ATM\c6: \c3/withdraw amt\c6 to withdraw cash.");

	%obj.currentBank = %brick;
	%obj.bankSchedule(%brick);
}

function Player::bankSchedule(%this, %brick)
{
	cancel(%this.bankSchedule);
	if(%this.getState() $= "Dead" || !isObject(%brick))
		return;

	if(vectorDist(%this.getPosition(), %brick.getPosition()) > 5)
	{
		messageClient(%this.client, '', "\c2ATM\c6: \c6Thanks, have a nice day.");
		%this.currentBank = "";
		return;
	}

	%this.bankSchedule = %this.schedule(100, bankSchedule, %brick);
}

function serverCmdDeposit(%this, %amt)
{	
	if(!isObject(%this.player) || %this.player.getState() $= "Dead")
		return;
	%user = GTAData.collection("users").document(%this.getDocumentName());

	if(%amt !$= mFloor(%amt)) {
		return;
	}
	if(%amt > %user.get("money")) {
		%amt = %user.get("money");
	}

	if(isObject(%this.player.currentBank))
	{
		if(%amt <= 0)
		{
			messageClient(%this, '', "\c2ATM\c6: \c6Insufficent funds!");
			return;
		}
		%user.set("bank", %user.get("bank") + %amt);
		%this.addMoney(-%amt);
		messageClient(%this, '', "\c2ATM\c6: \c6You have succesfuly deposited \c2$" @ %amt @ "\c6. You now have \c2$" @ %user.get("bank") @ "\c6 in the bank.");
	}
}

function serverCmdWithdraw(%this, %amt)
{
	if(!isObject(%this.player) || %this.player.getState() $= "Dead")
		return;
	%user = GTAData.collection("users").document(%this.getDocumentName());

	if(%amt !$= mFloor(%amt)) {
		return;
	}
	if(%amt > %user.get("bank")) {
		%amt = %user.get("bank");
	}

	if(isObject(%this.player.currentBank))
	{
		if(%amt <= 0)
		{
			messageClient(%this, '', "\c2ATM\c6: \c6You don't have any money deposited.");
			return;
		}
		%user.set("bank", %user.get("bank") - %amt);
		%this.addMoney(%amt);
		messageClient(%this, '', "\c2ATM\c6: \c6You have succesfuly withdrawn \c2$" @ %amt @ "\c6. You now have \c2$" @ %user.get("bank") @ "\c6 in the bank.");
	}
}