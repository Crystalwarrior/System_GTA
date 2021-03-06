// ============================================================
// Section 1 : Datablocks
// ============================================================
if(!isObject(taserItem))
{
	AddDamageType("Taser",   '<bitmap:Add-Ons/System_GTA/res/shapes/ci/taser> %1',    '%2 <bitmap:Add-Ons/System_GTA/res/shapes/ci/taser> %1', 0.5, 1);
	
	datablock AudioProfile(taserExplosionSound)
	{
		filename	 = "Add-Ons/Projectile_Radio_Wave/radioWaveExplosion.wav";
		description = AudioClosest3d;
		preload = true;
	};
	
	datablock ParticleData(taserDischargeParticle)
	{
		dragCoefficient			= 8;
		gravityCoefficient		= 0;
		inheritedVelFactor		= 0.2;
		constantAcceleration	= 0.0;
		lifetimeMS			  	= 500;
		lifetimeVarianceMS		= 100;
		textureName			 	= "Add-ons/Projectile_Radio_wave/bolt";
		spinSpeed				= 3380.0;
		spinRandomMin			= -50.0;
		spinRandomMax			= 50.0;
		colors[0]	 	= "1 1 0.0 0.6";
		colors[1]	 	= "1 1 1 0";
		sizes[0]		= 0.51;
		sizes[1]		= 0.26;
	
		useInvAlpha = true;
	};
	datablock ParticleEmitterData(taserDischargeEmitter)
	{
		ejectionPeriodMS	= 1;
		periodVarianceMS	= 0;
		ejectionVelocity	= 5;
		velocityVariance	= 0.6;
		ejectionOffset		= 0.8;
		thetaMin			= 0;
		thetaMax			= 0;
		phiReferenceVel		= 0;
		phiVariance			= 30;
		overrideAdvance		= false;
		particles			= "taserDischargeParticle";
	};
	
	datablock ParticleData(taserInduceParticle)
	{
		dragCoefficient		= 1;
		gravityCoefficient	= 0;
		inheritedVelFactor	= 0.2;
		constantAcceleration = 0.0;
		lifetimeMS			  = 700;
		lifetimeVarianceMS	= 400;
		textureName			 = "Add-ons/Projectile_Radio_wave/bolt";
		spinSpeed		= 10.0;
		spinRandomMin		= -50.0;
		spinRandomMax		= 50.0;
		colors[0]	  = "0.3 0.6 0.8 0.4";
		colors[1]	  = "1 1 0 0.1";
		colors[2]	  = "1 1 1 0.0";
		sizes[0]		= 0.15;
		sizes[1]		= 0.35;
		sizes[1]		= 0.45;
	
		useInvAlpha = true;
	};
	
	datablock ParticleEmitterData(taserInduceEmitter)
	{
		ejectionPeriodMS = 6;
		periodVarianceMS = 0;
		ejectionVelocity = -1.5;
		velocityVariance = 1.0;
		ejectionOffset	= 1.0;
		thetaMin			= 40;
		thetaMax			= 70;
		phiReferenceVel  = 360;
		phiVariance		= 360;
		overrideAdvance = false;
		particles = "taserInduceParticle";
	};
	
	datablock ProjectileData(taserProjectile)
	{
		projectileShapeName = "Add-Ons/Weapon_Gun/bullet.dts";

		directDamage		  = 0;
		directDamageType	 = $DamageType::Taser;
		radiusDamageType	 = $DamageType::Taser;
	
		brickExplosionRadius = 0;
		brickExplosionImpact = true;
		brickExplosionForce  = 10;
		brickExplosionMaxVolume = 1;
		brickExplosionMaxVolumeFloating = 2;
	
		impactImpulse		= 0;
		verticalImpulse		= 0;
		explosion			= gunExplosion;
		particleEmitter		= taserDischargeEmitter;
	
		muzzleVelocity		= 60;
		velInheritFactor	= 0;
	
		armingDelay			= 00;
		lifetime			= 500;
		fadeDelay			= 250;
		bounceElasticity	= 0.5;
		bounceFriction		= 0.20;
		isBallistic			= true;
		gravityMod = 1;
	
		hasLight	 = true;
		lightRadius = 1.0;
		lightColor  = "1.0 1.0 0.5";
	
		uiName = "Taser Discharge";
	};
	
	datablock ItemData(taserItem)
	{
		category = "Weapon";
		className = "Weapon";
		
		shapeFile = "Add-Ons/System_GTA/res/shapes/taser.dts";
		rotate = false;
		mass = 1;
		density = 0.2;
		elasticity = 0.2;
		friction = 0.6;
		emap = true;
	
		
		uiName = "Taser";
		iconName = "Add-Ons/System_GTA/res/shapes/ItemIcons/taser";
		doColorShift = false;
		colorShiftColor = "0.25 0.25 0.25 1.000";
		
		image = taserImage;
		canDrop = true;
		
		// CityRPG Properties
		canArrest = true;
	};
	
	datablock ShapeBaseImageData(taserImage)
	{
		shapeFile = "Add-Ons/System_GTA/res/shapes/taser.dts";
		emap = true;
		
		mountPoint = 0;
		offset = "0 0 0";
		eyeOffset = 0;
		rotation = eulerToMatrix( "0 0 0" );
		
		correctMuzzleVector = true;
		
		className = "WeaponImage";
		
		item = BowItem;
		ammo = " ";
		projectile			= taserProjectile;
		projectileType		= Projectile;
	
		//casing				= taserShellDebris;
		shellExitDir		= "1.0 -1.3 1.0";
		shellExitOffset		= "0 0 0";
		shellExitVariance	= 0.0;	
		shellVelocity		= 15.0;
		minShotTime			= 2500;
		
		melee = false;
		armReady = true;
	
		doColorShift		= true;
		colorShiftColor 	= taserItem.colorShiftColor;
		
		stateName[0]					= "Activate";
		stateTimeoutValue[0]			= 0.10;
		stateTransitionOnTimeout[0]		= "Ready";
		stateSound[0]					= weaponSwitchSound;
	
		stateName[1]					= "Ready";
		stateTransitionOnTriggerDown[1] = "Fire";
		stateAllowImageChange[1]		= true;
		stateSequence[1]				= "Ready";
	
		stateName[2]					= "Fire";
		stateTransitionOnTimeout[2]	 	= "Recharge";
		stateTimeoutValue[2]			= 0.10;
		stateFire[2]					= true;
		stateAllowImageChange[2]		= false;
		stateSequence[2]				= "Fire";
		stateScript[2]					= "onFire";
		stateWaitForTimeout[2]			= true;
		stateEmitter[2]					= taserDischargeEmitter;
		stateEmitterTime[2]				= 0.05;
		stateEmitterNode[2]				= "muzzleNode";
		stateSound[2]					= TaserExplosionSound;
		stateEjectShell[2]				= true;
	
		stateName[3]					= "Recharge";
		stateEmitterTime[3]				= 2.50;
		stateEmitter[3]					= taserInduceEmitter;
		stateEmitterNode[3]				= "muzzleNode";
		stateTimeoutValue[3]			= 2.50;
		stateTransitionOnTimeout[3]	 	= "Reload";
	
		stateName[4]					= "Reload";
		stateSequence[4]				= "Reload";
		stateTransitionOnTriggerUp[4]	= "Ready";
		stateSequence[4]				= "Ready";
	
	};
}

// ============================================================
// Section 2 : Functions
// ============================================================

function taserImage::onFire(%this, %obj, %slot)
{
	if($Sim::Time - %obj.lastTaserFire <= 2.6)
		return;
	parent::onFire(%this, %obj, %slot);
	%obj.lastTaserFire = $Sim::Time;
}

function taserProjectile::onCollision(%this, %obj, %col, %fade, %pos, %normal)
{
	if(isObject(%this))
	if(isObject(%col))
	{
		if(miniGameCanDamage(%obj, %col) <= 0 || %col.getDatablock().isInvincible || %col.client.getWantedLevel() <= 0) return;
		if((%col.getType() & $typeMasks::playerObjectType) && isObject(%col.client))
		{
			%col.oldDatablock = %col.getDataBlock();
			%col.setVelocity("0 0" SPC getWord(%col.getVelocity(), 2));
			freezeLoop(%col);
		}
	}
}

function freezeLoop(%this)
{
	cancel(%this.freezeLoop);
	if(%this.loops >= 20)
	{
		%this.setDataBlock(%this.oldDatablock);
		%this.loops = "";
		return;
	}

	if(!isObject(%this))
	{
		return;
	}

	if(%this.getDataBlock() !$= nameToId(playerFrozenArmor))
	{
		%this.setDataBlock(playerFrozenArmor);
	}

	%pos = %this.getTransform();
	%rnd = getRandom(-10, 10) * 0.1;
	%newrot = getWord(%pos, 5) + %rnd;
	%this.setTransform(getWords(%pos, 0, 4) SPC %newrot);

	%this.loops += 1;
	%this.freezeLoop = schedule(100, 0, "freezeLoop", %this);
}

// package CityRPG_TaserPackage
// {
// 	function Armor::damage(%this, %obj, %src, %unk, %dmg, %type)
// 	{
// 		// Taser Abuse Preventitive Measures
// 		if(!(isObject(%src) && %src.getDatablock().getName() $= "deathVehicle"))
// 		{
// 			parent::damage(%this, %obj, %src, %unk, %dmg, %type);
// 		}
// 	}	
// };
// activatePackage(CityRPG_TaserPackage);