function serverCmdMoney(%this, %name) {
	if(%this.isAdmin && isObject(%target = findClientByName(%name))) {
		%user = GTAData.collection("users").document(%target.getDocumentName());

		// if(%user.get("money") $= "" || %user.get("money") < 0) {
		// 	%this.setMoney(0);
		// }
		// if(%user.get("money") > 99999999) {
		// 	%this.setMoney(99999999);
		// }
		messageClient(%this, '', "\c6" @ %target.name SPC "currently has \c2$" @ %user.get("money") @ "\c6.");
	}
}

function serverCmdGiveMoney(%this, %amt, %name) {
	%user = GTAData.collection("users").document(%this.getDocumentName());

	if(%amt !$= mFloor(%amt)) {
		return;
	}
	if(%amt > %user.get("money")) {
		%amt = %user.get("money");
	}
	if(isObject(%target = findClientByName(%name)) && %amt > 0) {
		%userTarget = GTAData.collection("users").document(%target.getDocumentName());
		messageClient(%this, '', "\c6You gave \c2$" SPC %amt SPC "\c6to" SPC %target.name);
		messageClient(%target, '', "\c6" @ %this.name SPC "has given you \c2$" SPC %amt @ "\c6.");
		%this.addMoney(-%amt);
		%target.addMoney(%amt);
	}
}

function serverCmdGrantMoney(%this, %amt, %name) {
	%user = GTAData.collection("users").document(%this.getDocumentName());

	if(%amt !$= mFloor(%amt)) {
		return;
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
	if(%this.isAdmin && isObject(%target = findClientByName(%name))) {
		%show = %amt;
		if(%show < 0) {
			%show = mAbs(%show);
			if(%user.get("money") < %show) {
				%show = %user.get("money");
			}
		}
		if(%target == %this) {
			messageClient(%this, '', "\c6You" SPC (%add ? "added" : "substracted") SPC "\c2$" @ %show SPC "\c6" @ (%add ? "to" : "from") SPC "yourself.");
			%this.addMoney(%amt);
			if(%user.get("money") < 0) {
				%this.setMoney(0);
			}
			if(%user.get("money") > 99999999) {
				%this.setMoney(99999999);
			}
			return;
		}
		%userTarget = GTAData.collection("users").document(%target.getDocumentName());
		messageClient(%this, '', "\c6You" SPC (%add ? "added" : "substracted") SPC "\c2$" @ %show SPC "\c6" @ (%add ? "to" : "from") SPC %target.name);
		messageClient(%target, '', "\c6" @ %this.name SPC "has" SPC (%add ? "added" : "substracted") SPC "\c2$" @ %show @ "\c6.");
		%target.addMoney(%amt);
		if(%userTarget.get("money") < 0) {
			%target.setMoney(0);
		}
		if(%userTarget.get("money") > 99999999) {
			%target.setMoney(99999999);
		}
	}
}

function serverCmdSetMoney(%this, %amt, %name) {
	%user = GTAData.collection("users").document(%this.getDocumentName());

	if(%amt !$= mFloor(%amt)) {
		return;
	}
	if(isObject(%target = findClientByName(%name)) && %amt >= 0) {
		%userTarget = GTAData.collection("users").document(%target.getDocumentName());
		messageClient(%this, '', "\c6You set" SPC %target.name @ "'s money to \c2$" @ %amt);
		messageClient(%target, '', "\c6" @ %this.name SPC "has set your money to \c2$" @ %amt @ "\c6.");
		%target.setMoney(%amt);
	}
}

function serverCmdDropMoney(%this, %amt) {
	if(!isObject(%obj = %this.player)) {
		messageClient(%this, '', "\c6You must be alive to use this command.");
		return;
	}
	if(%amt <= 0 || %amt !$= mFloor(%amt)) {
		messageClient(%this, '', "\c6Invalid amount!");
		return;
	}
	%user = GTAData.collection("users").document(%this.getDocumentName());

	if(%user.get("money") <= 0) {
		messageClient(%this, '', "\c6You dont have any money!");
		return;
	}
	if(%amt > %user.get("money")) {
		%amt = %user.get("money");
	}

	%pos = %obj.getPosition();

	%money = new Item() {
		dataBlock = moneyItem;
		position = %pos;
		amt = %amt;
	};

	%money.setShapeName("$" @ %money.amt);
	%money.setShapeNameColor("0 1 0");
	%money.setShapeNameDistance(25);
	%money.setTransform(%obj.getEyePoint());
	%money.setVelocity(VectorAdd(%obj.getVelocity(), VectorScale(%obj.getEyeVector(), 7)));
	%money.setCollisionTimeout(%this.player);
	%this.addMoney(-%amt);

	%money.schedule( 29000, "fadeOut" );
	%money.schedule( 30000, "delete" );
}