function mAdd(%a, %b) { //for big numbers. Doesn't go apeshit with numbers above 999999.
	return ((%a | 0) + (%b | 0)) | 0;
}

function strFirstUpr(%string) {
	return strUpr(getSubStr(%string, 0, 1)) @ getSubStr(%string, 1, strLen(%string));
}