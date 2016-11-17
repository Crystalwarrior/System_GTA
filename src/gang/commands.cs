function serverCmdGang(%this, %action, %arg1, %arg2) {
	switch$(%action) {
		case "create":
			%this.gang = CreateGang(%this, %arg1);
			if(isJSONObject(%this.gang)) {
				messageClient(%client, '', "\c6Succesfuly created \c3\"" @ %arg1 @ "\"\c6 gang.");
			}
		case "disband":
			if(!isJSONObject(%this.gang) || %this.gang.get("leader") != %this.bl_id) {
				messageClient(%client, '', "\c6You don't have a gang you lead!");
				return;
			}
			RemoveGang(%this.gang);
			%this.gang = "";
		case "invite":
			if(!isJSONObject(%this.gang) || %this.gang.get("leader") != %this.bl_id) {
				messageClient(%client, '', "\c6You don't have a gang you lead!");
				return;
			}
			if(!isObject(findClientByName(%arg1))) {
				messageClient(%client, '', "\c6\"" @ %arg1 @ "\" is either not present on the server or he doesnt exist.");
				return;
			}
			InviteGangMember(%this.gang, %arg1);
		case "kick":
			if(!isJSONObject(%this.gang) || %this.gang.get("leader") != %this.bl_id) {
				messageClient(%client, '', "\c6You don't have a gang you lead!");
				return;
			}
			if(!isObject(findClientByName(%arg1))) {
				messageClient(%client, '', "\c6\"" @ %arg1 @ "\" is either not present on the server or he doesnt exist. In this case, use /gang kick_blid.");
				return;
			}
			%exists = RemoveGangMember(%this.gang, findClientByName(%arg1).bl_id);
			if(!%exists) {
				messageClient(%client, '', "\c6" @ findClientByName(%arg1).getPlayerName() SPC "does not belong to your gang.");
			}
		case "kick_blid":
			if(!isJSONObject(%this.gang) || %this.gang.get("leader") != %this.bl_id) {
				messageClient(%client, '', "\c6You don't have a gang you lead!");
				return;
			}
			if(%arg1 $= "") {
				messageClient(%client, '', "\c6You must specify the bl_id of who you want to kick.");
				return;
			}
			%exists = RemoveGangMember(%this.gang, %arg1);
			if(!%exists) {
				messageClient(%client, '', "\c6User with specified bl_id was not found in your gang!");
			}
		case "accept": %this.gangAccept();
		case "decline": %this.gangDecline();
		case "members": //return a list of members. TODO
		default: %this.GangHelp();
	}
}