//sound
datablock AudioProfile(wateringCanHitSound)
{
   filename    = "base/data/sound/exitWater.wav";
   description = AudioClosest3d;
   preload = true;
};

//effects
datablock ParticleData(wateringCanTrailParticle)
{
   dragCoefficient      = 4;
   gravityCoefficient   = 2;
   inheritedVelFactor   = 0;
   constantAcceleration = 0.0;
   lifetimeMS           = 1200;
   lifetimeVarianceMS   = 100;
   textureName          = "base/data/particles/cloud";
	
	useInvAlpha = false;
	spinSpeed		= 20.0;
	spinRandomMin		= -20.0;
	spinRandomMax		= 20.0;

   colors[0]     = "0.3 0.3 1.0 1.0";
   colors[1]     = "0.3 0.3 1.0 1.0";
   colors[2]     = "0.3 0.3 1.0 0.0";
   sizes[0]      = 0.15;
   sizes[1]      = 0.15;
   sizes[2]      = 0.00;

   times[0]	= 0.1;
   times[1] = 0.5;
   times[2] = 1.0;

   useInvAlpha = true;
};

datablock ParticleEmitterData(wateringCanTrailEmitter)
{
   ejectionPeriodMS = 12;
   periodVarianceMS = 0;
   ejectionVelocity = 7;
   velocityVariance = 0.0;
   ejectionOffset   = 0.0;
   thetaMin         = 0;
   thetaMax         = 10;
   phiReferenceVel  = 0;
   phiVariance      = 360;
   overrideAdvance = false;
   particles = wateringCanTrailParticle;

   uiName = "Thin Water Stream";
};

datablock ParticleData(wateringCanExplosionParticle)
{
   dragCoefficient      = 4;
   gravityCoefficient   = 2;
   inheritedVelFactor   = 0.2;
   constantAcceleration = 0.0;
   lifetimeMS           = 300;
   lifetimeVarianceMS   = 50;
   textureName          = "base/data/particles/cloud";

	spinSpeed		= 50.0;
	spinRandomMin		= -500.0;
	spinRandomMax		= 500.0;

   colors[0]     = "0.3 0.3 1.0 0.75";
   colors[1]     = "0.0 0.0 0.0 0.0";
   sizes[0]      = 0.15;
   sizes[1]      = 0.2;

   useInvAlpha = true;
};

datablock ParticleEmitterData(wateringCanExplosionEmitter)
{
   ejectionPeriodMS = 1;
   periodVarianceMS = 0;
   ejectionVelocity = 2;
   velocityVariance = 1.0;
   ejectionOffset   = 0.0;
   thetaMin         = 0;
   thetaMax         = 95;
   phiReferenceVel  = 0;
   phiVariance      = 360;
   overrideAdvance = false;
   particles = wateringCanExplosionParticle;

   uiName = "Small Splash";
};

datablock ExplosionData(wateringCanExplosion)
{
   //explosionShape = "";
   lifeTimeMS = 400;
   
   particleEmitter = wateringCanExplosionEmitter;
   particleDensity = 40;
	particleRadius = 0.3;
   
   faceViewer     = true;
   explosionScale = "1 1 1";

   soundProfile = wateringCanHitSound;

   
   shakeCamera = false;
   cameraShakeFalloff = false;
   camShakeFreq = "2.0 3.0 1.0";
   camShakeAmp = "1.0 1.0 1.0";
   camShakeDuration = 2.5;
   camShakeRadius = 0.0001;

   // Dynamic light
   lightStartRadius = 0;
   lightEndRadius = 0;
   lightStartColor = "0.0 0.0 0.0";
   lightEndColor = "0 0 0";
};


//projectile
datablock ProjectileData(wateringCanProjectile)
{
   projectileShapeName = "base/data/shapes/empty.dts";
   directDamage        = 0;
   impactImpulse       = 0;
   verticalImpulse     = 0;
   explosion           = wateringCanExplosion;

   muzzleVelocity      = 3;
   velInheritFactor    = 0;

   armingDelay         = 0;
   lifetime            = 1000;
   fadeDelay           = 70;
   bounceElasticity    = 0;
   bounceFriction      = 0;
   isBallistic         = true;
   gravityMod = 1.0;

   hasLight    = false;
   lightRadius = 3.0;
   lightColor  = "0 0 0.5";

   uiName = "";
};


//////////
// item //
//////////
datablock ItemData(wateringCanItem)
{
	category = "Tools";  // Mission editor category
	className = "Weapon"; // For inventory system

	 // Basic Item Properties
	shapeFile = "./watercan.dts";
	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = true;

	//gui properties
	uiName = "Watering Can";
	// iconName = "./";
	doColorShift = true;
	colorShiftColor = "0.7 0.7 0.7 1";
	

	 // Dynamic properties defined by the scripts
	image = wateringCanImage;
	canDrop = true;
};

////////////////
//weapon image//
////////////////
datablock ShapeBaseImageData(wateringCanImage)
{
   // Basic Item properties
   shapeFile = "./waterCan.dts";
   emap = true;

   // Specify mount point & offset for 3rd person, and eye offset
   // for first person rendering.
   mountPoint = 0;
   offset = "0 0 0";
   //eyeOffset = "0.7 1.2 -0.15";

   // When firing from a point offset from the eye, muzzle correction
   // will adjust the muzzle vector to point to the eye LOS point.
   // Since this weapon doesn't actually fire from the muzzle point,
   // we need to turn this off.  
   correctMuzzleVector = false;

   // Add the WeaponImage namespace as a parent, WeaponImage namespace
   // provides some hooks into the inventory system.
   className = "WeaponImage";

   // Projectile && Ammo.
   item = wateringCanItem;
   ammo = " ";
   projectile = wateringCanProjectile;
   projectileType = Projectile;

   //melee particles shoot from eye node for consistancy
   melee = false;
   doRetraction = false;
   //raise your arm up or not
   armReady = true;

   //casing = " ";

   doColorShift = true;
   colorShiftColor = "0.7 0.7 0.7 1";

   // Images have a state system which controls how the animations
   // are run, which sounds are played, script callbacks, etc. This
   // state system is downloaded to the client so that clients can
   // predict state changes and animate accordingly.  The following
   // system supports basic ready->fire->reload transitions as
   // well as a no-ammo->dryfire idle state.

   // Initial start up state
	stateName[0]                     = "Activate";
	stateTimeoutValue[0]             = 0.0;
	stateTransitionOnTimeout[0]       = "Ready";

	stateName[1]                     = "Ready";
	stateTransitionOnTriggerDown[1]  = "PreFire";
	stateAllowImageChange[1]         = true;

	stateName[2]			= "PreFire";
	stateScript[2]                  = "onPreFire";
	stateAllowImageChange[2]        = true;
	stateTimeoutValue[2]            = 0.01;
	stateTransitionOnTimeout[2]     = "Fire";
	stateEmitterNode[2]				= "muzzlePoint";
	stateEmitterTime[2]				= 0.01;
	stateEmitter[2] 				= "wateringCanTrailEmitter";

	stateName[3]                    = "Fire";
	stateTransitionOnTimeout[3]     = "Fire";
	stateTimeoutValue[3]            = 0.2;
	stateFire[3]                    = true;
	stateAllowImageChange[3]        = true;
	stateSequence[3]                = "Fire";
	stateScript[3]                  = "onFire";
	stateWaitForTimeout[3]			= true;
	stateSequence[3]				= "Fire";
    stateTransitionOnTriggerUp[3]	= "StopFire";
    stateSound[3] = "";
	stateEmitterNode[3]				= "muzzlePoint";
	stateEmitterTime[3]				= 0.2;
	stateEmitter[3] 				= "wateringCanTrailEmitter";
   
	stateName[4]			= "CheckFire";
	stateTransitionOnTriggerUp[4]	= "StopFire";
	stateTransitionOnTriggerDown[4]	= "Fire";
    stateSound[4] = "";

	stateName[5]                    = "StopFire";
	stateTransitionOnTimeout[5]     = "Ready";
	stateTimeoutValue[5]            = 0.2;
	stateAllowImageChange[5]        = true;
	stateWaitForTimeout[5]		= true;
	stateSequence[5]                = "StopFire";
	stateScript[5]                 = "onStopFire";
};

datablock ItemData(HempBagItem)
{
	category = "Weapon";  // Mission editor category;
	className = "Weapon"; // For inventory system

	 // Basic Item Properties
	shapeFile = "./DrugBag.dts";
	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = true;

	//gui stuff
	uiName = "Hemp Seeds";
	iconName = "";

	doColorShift = true;
	colorShiftColor = "0.09 0.529 0.274 1.0";

	 // Dynamic properties defined by the scripts
	image = HempBagImage;
	canDrop = true;
	isDrug = true;
};

datablock ShapeBaseImageData(HempBagImage)
{
	// Basic Item properties
	shapeFile = "./DrugBag.dts";
	emap = true;

	// Specify mount point & offset for 3rd person, and eye offset
	// for first person rendering.
	mountPoint = 0;
	offset = "0 0 0";

	// When firing from a point offset from the eye, muzzle correction
	// will adjust the muzzle vector to point to the eye LOS point.
	// Since this weapon doesn't actually fire from the muzzle point,
	// we need to turn this off.  
	correctMuzzleVector = false;

	//eyeOffset = "0.7 1.2 -0.25";

	// Add the WeaponImage namespace as a parent, WeaponImage namespace
	// provides some hooks into the inventory system.
	className = "WeaponImage";

	// Projectile && Ammo.
	item = HempBagItem;
	ammo = " ";
	projectile = "";
	projectileType = Projectile;

	doColorShift = true;
	colorShiftColor = "0.09 0.529 0.274 1.0";

	//melee particles shoot from eye node for consistancy
	melee = true;
	doRetraction = false;
	//raise your arm up or not
	armReady = true;

	//casing = " ";

	// Images have a state system which controls how the animations
	// are run, which sounds are played, script callbacks, etc. This
	// state system is downloaded to the client so that clients can
	// predict state changes and animate accordingly.  The following
	// system supports basic ready->fire->reload transitions as
	// well as a no-ammo->dryfire idle state.

	// Initial start up state
	stateName[0] = "Activate";
	stateTimeoutValue[0] = 0.15;
	stateTransitionOnTimeout[0] = "Ready";
	stateSound[0] = weaponSwitchSound;

	stateName[1] = "Ready";
	stateTransitionOnTriggerDown[1] = "Fire";
	stateAllowImageChange[1] = true;
	stateSequence[1] = "Ready";

	stateName[2] = "Fire";
	stateScript[2] = "onFire";
	stateTimeoutValue[2] = 0.1;
	stateTransitionOnTimeout[2] = "Ready";
};

datablock ShapeBaseImageData(OpiumBagImage : HempBagImage)
{
	item = OpiumBagItem;

	doColorShift = true;
	colorShiftColor = "0.560 0.529 0.454 1.0";
};

datablock ItemData(OpiumBagItem : HempBagItem)
{
	uiName = "Opium Seeds";
	iconName = "";

	doColorShift = true;
	colorShiftColor = "0.560 0.529 0.454 1.0";

	image = OpiumBagImage;
};

datablock ShapeBaseImageData(MarijuanaImage : HempBagImage)
{
	shapeFile = "./MarijuanaImage.dts";

	item = MarijuanaItem;

	doColorShift = true;
	colorShiftColor = "0.352 0.388 0.258 1.0";
};

datablock ItemData(MarijuanaItem : HempBagItem)
{
	shapeFile = "./MarijuanaItem.dts";

	uiName = "Marijuana";
	iconName = "";

	doColorShift = true;
	colorShiftColor = "0.352 0.388 0.258 1.0";

	image = MarijuanaImage;
};

datablock ShapeBaseImageData(HeroinImage : HempBagImage)
{
	shapeFile = "./HeroinImage.dts";

	doColorShift = false;
	item = HeroinItem;
};

datablock ItemData(HeroinItem : HempBagItem)
{
	shapeFile = "./HeroinItem.dts";

	uiName = "Heroin";
	iconName = "";


	doColorShift = false;
	image = HeroinImage;
};