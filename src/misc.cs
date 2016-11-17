function dayCycleUpdate() {
	cancel($dayCycleUpdate);

	%time = getDayCycleTime();
	%stage = getDayCycleStage(%time);
	%stageName = getDayCycleStageName(%stage);

	if (%stage == 0) { // Dawn
		//reduce demerits ok
	}

	$dayCycleUpdate = schedule(getMSToNextDayCycleStage(%time) + 16, 0, dayCycleUpdate);
}

function Player::applyCopAppearance(%obj) {
	%obj.setNodeColor("chest", "0 0.141 0.333 1");
	%obj.setNodeColor("rarm", "0 0.141 0.333 1");
	%obj.setNodeColor("larm", "0 0.141 0.333 1");
	for(%i = 0;$hat[%i] !$= "";%i++)
	{
		%obj.hideNode($hat[%i]);
		%obj.hideNode($accent[%i]);
	}
	%obj.setNodeColor("cophat", "0 0.141 0.333 1");
	%obj.unHideNode("cophat");
	%decal = "Mod-Police";
	%obj.setDecalName(%decal);
}

function Player::applyJailAppearance(%obj) {
	%obj.setNodeColor("chest", "0.902 0.341 0.078 1");
	%obj.setNodeColor("pants", "0.902 0.341 0.078 1");
	%obj.setNodeColor("rarm", "0.902 0.341 0.078 1");
	%obj.setNodeColor("larm", "0.902 0.341 0.078 1");
	%decal = "Mod-Prisoner";
	%obj.setDecalName(%decal);
}

function serverCmdHelp(%this, %category, %subcategory) {
	switch$(%category) {
		case "commands": 
			if(%subcategory $= "admin") { // /help commands admin
				%i = -1;
				%text[%i++] = "\c3----";
				%textcount = %i;
			}
			else { // /help commands
				%i = -1;
				%text[%i++] = "\c3----";
				%textcount = %i;
			}

		case "police":
			%i = -1;
			%text[%i++] = "\c6   You can become a policeman by using \c3/becop\c6 or \c3/retire\c6.";
			%text[%i++] = "\c6Police arrest criminals and get rewarded.";
			%text[%i++] = "\c6If you're arrested, you'll be taken to jail. ";
			%text[%i++] = "\c6Please note that you will get demoted if you die with any amount of wanted stars.";
			%text[%i++] = "\c6/Record to check your record";
			%textcount = %i;

		case "vehicles":
			%i = -1;
			%text[%i++] = "\c6   Vehicles spawn all over the map. You can easily steal one by clicking to enter it.";
			%text[%i++] = "\c6Vehicles can damage each other by ramming, or get damaged from ramming into bricks.";
			%text[%i++] = "\c6Be careful where you drive unless you want your \"hard-earned\" car go to waste!";
			%textcount = %i;

		case "money":
			%i = -1;
			%text[%i++] = "\c6   The following are the commands you will need.";
			%text[%i++] = "\c3/dropmoney amount";
			%text[%i++] = "\c3/givemoney amount ID";
			%textcount = %i;

		case "weapons":
			%i = -1;
			%text[%i++] = "\c6   You can purchase weapons in the stores located across the world.";
			%text[%i++] = "\c6This isnt a deathmatch, you will get wanted if you kill randomly!";
			%textcount = %i;
		default:
		%i = -1;
		%text[%i++] = "\c6   Welcome to Furdle's Grand Theft Auto! Help Commands:";
		%text[%i++] = "\c3/help police";
		%text[%i++] = "\c3/help vehicles";
		%text[%i++] = "\c3/help money";
		%text[%i++] = "\c3/help weapons";
		%textcount = %i;
	}
	if(%textcount $= "" || %textcount < 0) {
		return;
	}
	for(%i = 0; %i <= %textcount; %i++) {
		messageClient(%this, '', %text[%i]);
	}
}