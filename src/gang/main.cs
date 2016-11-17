exec("./commands.cs");
function CreateGang(%leader, %name) {
	%gang = parseJSON("{}");
	if(%name $= "") {
		error("CreateGang: Invalid name!");
		return;
	}
	%bl_id = %leader.bl_id;
	if(!isObject(%leader)) {
		%bl_id = 0;
	}
	%gang.set("name", %name);
	%gang.set("leader", %bl_id);
	%gang.set("members", parseJSON("[]"));
	for(%i = 0; %i < $gangCount; %i++) {
		if($gangs[%i] $= "") {
			$gangs[%i] = %gang;
			$gangCount++;
			break;
		}
	}
	return %gang;
}

function gangSetDirty(%gang) {
	if(!isJSONObject(%gang)) {
		error("setDirty: Invalid/Nonexistant gang!");
		return;
	}
	%gang.dirty = 1;

	if (%gang.file !$= "" && !isEventPending(%gang.dirtyExportSchedule)) {
		%gang.dirtyExportSchedule = schedule(60000, 0, saveJSON, %gang, "config/server/GTA/gangs/" @ %gang.name);
	}

	return %this;
}

function AddGangMember(%gang, %bl_id) {
	if(!isJSONObject(%gang)) {
		error("AddGangMember: Invalid/Nonexistant gang!");
		return;
	}
	if(%bl_id $= "") {
		error("AddGangMember: Invalid bl_id!");
		return;
	}
	if(!isJSONObject(%gang.members)) {
		echo("AddGangMember: gang didn't have .members array, creating...");
		%gang.set("members", parseJSON("[]"));
	}
	%gang.members.append(%bl_id);
	gangSetDirty(%gang);
}

function RemoveGangMember(%gang, %bl_id) {
	if(!isJSONObject(%gang)) {
		error("RemoveGangMember: Invalid/Nonexistant gang!");
		return;
	}
	if(%bl_id $= "") {
		error("RemoveGangMember: Invalid bl_id!");
		return;
	}
	%array = %gang.members;
	for (%i = 0; %i < %array.length; %i++)
	{
		echo(%array.item[%i]);
		if(%array.get(%i) == %bl_id) {
			%array.remove(%i);
			%exists = true;
			if(isObject(%client = findClientByBL_ID(%bl_id))) {
				messageClient(%client, '', "\c6You have been kicked out of the gang.");
			}
		}
	}
	gangSetDirty(%gang);
	return %exists;
}

function RemoveGang(%gang) {
	if(!isJSONObject(%gang)) {
		error("RemoveGang: Invalid/Nonexistant gang!");
		return;
	}
	%gang.delete();
}

function exportGangData() { //Manual gang saving iguess
	for(%i = 0; %i < $gangCount; %i++) {
		if($gangs[%i] !$= "") {
			saveJSON($gangs[%i], "config/server/GTA/gangs/" @ $gangs[%i].name);
		}
	}
}

function InviteGangMember(%gang, %member) {
	//MEHBEH
}

function GameConnection::gangAccept(%this) {
	if(!%this.gangInvitePending) {
		messageClient(%this, '', "\c6You dont have any pending invites.");
		return;
	}
}