datablock fxDtsBrickData(jailSpawnPointData : brickSpawnPointData)
{
	category = "Special";
	subcategory = "GTA";
	uiName = "Jail Spawn";
};

datablock fxDtsBrickData(copSpawnPointData : brickSpawnPointData)
{
	category = "Special";
	subcategory = "GTA";
	uiName = "Cop Spawn";
};

package jailSpawnPointPackage {
	function jailSpawnPointData::onAdd(%this,%obj) {
		parent::onPlant(%this, %obj);
		if(!isObject(jailSpawnSet)) {
			new SimSet(jailSpawnSet);
		}
		jailSpawnSet.add(%obj);
	}
	function copSpawnPointData::onAdd(%this,%obj) {
		parent::onPlant(%this, %obj);
		if(!isObject(copSpawnSet)) {
			new SimSet(copSpawnSet);
		}
		copSpawnSet.add(%obj);
	}
};
activatePackage(jailSpawnPointPackage);