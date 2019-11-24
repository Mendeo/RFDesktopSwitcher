use <box.scad>

$fn = 250;

devL = 43.8;
devW = 18.5;
devH = 12.2; //От кромки платы ардуино до верхушки дросселя на приёмнике.
extraSpace = 2;
T = 1;

HPin = 2.5;
intPinRad = 0.8;
arduinoThiknesToHole = 0.5;

hCapInt = T;
hCapExt = T;

usbL = 7.8;
usbW = 2.8;

arduinoPlateThiknes = 1.7;

boxEdgeRad = 1.5;

antennaD = 1;
XfromPlateEdgeToAntennaEdge = 13;
ZfromPlateEdgeToAntennaEdge = 2.8;

extPinRad = intPinRad + arduinoThiknesToHole;
addToExtPinRad = intPinRad + T - extPinRad;

L = devL + extraSpace + 2 * T;
W = devW + extraSpace + 2 * T;
H = HPin + T + devH + hCapInt;

render()
devBox();

*
translate([0, 0, hCapExt])
rotate([180, 0, 0])
render()
cap(L, W, T, hCapExt, hCapInt, boxEdgeRad, 0, 0.4, false);

module devBox()
{

	difference()
	{
		box(L, W, H, T, boxEdgeRad, 0, false);
		//Отверстие под микро USB.
		translate([0, (W - usbL) / 2, usbW / 2 + T + HPin + arduinoPlateThiknes + 0.1])
		rotate([90, 0, 90])
		roundedHole(usbL, usbW, T);
		for (edge = [0 : 3]) pinEdgePlacer(edge, HPin + T, extPinRad, intPinRad, addToExtPinRad, 0, 2, devL, devW, T + extraSpace / 2, T + extraSpace / 2); //Делаем дырки насквозь, чтобы потом они оказаись в пинах.
		//Отверстие под антенну.
		translate([antennaD / 2 + XfromPlateEdgeToAntennaEdge, T, antennaD / 2 + T + HPin + ZfromPlateEdgeToAntennaEdge])
		rotate([90, 0, 0])
		cylinder(h = T, d = antennaD);
	}
	for (edge = [0 : 3]) pinEdgePlacer(edge, HPin + T, extPinRad, intPinRad, addToExtPinRad, 0.4, 0, devL, devW, T + extraSpace / 2, T + extraSpace / 2);
}
