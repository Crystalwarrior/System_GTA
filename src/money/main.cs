exec("./events.cs");
exec("./commands.cs");
exec("./bank.cs");

datablock audioProfile( moneyPickupSound ) {
	fileName = "Add-Ons/System_GTA/res/sounds/cash_pickup.wav";
	description = audioClose3D;
	preload = true;
};

datablock ItemData(moneyItem : swordItem) {
	category = "Weapon";
	className = "Weapon";
	
	shapeFile = "Add-Ons/System_GTA/res/shapes/money.dts";
	mass = 1.1;
	density = 0.2;
	elasticity = 0.3;
	friction = 0.7;
	emap = true;
	
	doColorShift = true;
	colorShiftColor = "0 0.7 0 1";
	image = "";
	canPickup = true;

	uiName = "";

	// lightType = "constantLight";
	// lightColor = "0 1 0 0.5";
};

function canPickup(%obj) {
	%obj.canPickup = true;
}

function gameConnection::setMoney(%this, %value) {
	%user = GTAData.collection("users").document(%this.getDocumentName());
	%user.set("money", %value);
	%this.displayEffect = "money_flash";
	%this.updateDisplay();
	schedule(95, 0, eval, %this @ ".displayEffect = \"\";");
	%this.schedule(100, updateDisplay); //Stop the effect
	%this.setScore(); //Since we packaged "setScore", args can be empty.
}

function gameConnection::addMoney(%this, %value) {
	%user = GTAData.collection("users").document(%this.getDocumentName());
	%this.setMoney(mAdd(%user.get("money"), %value));
}

package moneyItem {
	function gameConnection::onDeath( %this, %source, %killer, %type, %location ) {
		%user = GTAData.collection("users").document(%this.getDocumentName());
		if(isObject( %obj = %this.player ) ) {
			if(%user.get("money") <= 0) {
				return parent::onDeath( %this, %source, %killer, %type, %location );
			}

			%pos = %obj.getPosition();

			%money = new Item() {
				dataBlock = moneyItem;
				position = %pos;
				amt = mClamp(%user.get("money"), 1, 100);
			};

			%money.setShapeName("$" @ %money.amt);
			%money.setShapeNameColor("0 1 0");
			%money.setShapeNameDistance(25);

			%money.setTransform(setWord(%obj.getTransform(), 2, getWord(%obj.getTransform(), 2) + 2));
			%money.setVelocity(VectorScale(%obj.getEyeVector(), 5));
			%this.addMoney(-%money.amt);

			%money.schedule( 29000, "fadeOut" );
			%money.schedule( 30000, "delete" );
		}

		parent::onDeath( %this, %source, %killer, %type, %location );
	}

	function Armor::onCollision( %this, %obj, %col, %thing, %other ) {
		if(!isObject(%obj.client)) {
			parent::onCollision( %this, %obj, %col, %thing, %other );
			return;
		}

		%user = GTAData.collection("users").document(%obj.client.getDocumentName());
		if(%col.getDatablock() == nameToId("moneyItem") ) {
			if(isObject( %col ) && isObject( %obj.client ) ) {
				if(%col.canPickup) {
					serverPlay3D( moneyPickUpSound, %obj.getHackPosition() );
					%obj.client.addMoney(%col.amt);
					%col.delete();
				}
			}
		}
		else {
			parent::onCollision( %this, %obj, %col, %thing, %other );
		}
	}

	function moneyItem::onAdd( %this, %obj ) {
		%obj.rotate = 1;
		Parent::onAdd(%this, %obj );
	}

	function gameConnection::setScore(%this, %score) {
		%user = GTAData.collection("users").document(%this.getDocumentName());
		%score = %user.get("money");
		parent::setScore(%this, %score);
	}
};
activatePackage(moneyItem);