box(100, 70, 30, 2, 5, 2);
translate([25, 15, 0])
pillars(50, 40, 10, 20, 4, 0.8);
*translate([0, 0, 30])
cap(100, 70, 2, 5, 3, 5, 2, 0.4);



module box(L, W, HBox, T, extPinRad, intPinRad, needInternalPins)
{
	difference()
	{
		union()
		{
			difference()
			{
				cube([L, W, HBox]);
				translate([T, T, T])
				cube([L - 2 * T, W - 2 * T, HBox]);
			}
			for (edge = [0 : 3]) pinEdgePlacer(edge, HBox, extPinRad, intPinRad, 0, T, 0, L, W, 0, 0); //Цилиндр по четырём углам
		}
		//Вырез оставшихся треугольников по четырём углам.
		for (edge = [0 : 3]) excessTriangle(edge, HBox, extPinRad, L, W);
		if (!needInternalPins)
		{
			//Вырез внутренних цилиндров по четырём углам.
			for(edge = [0 : 3]) cutInternalPins(edge, HBox, extPinRad, L, W, T);
		}
	}
}

module cap(L, W, T, HCapExt, HCapInt, extPinRad, intPinRad, capGap, boxHasInternalPins)
{
	difference()
	{
		difference()
		{
			union()
			{
				cube([L, W, HCapExt]);
			}
			for (edge = [0 : 3]) pinEdgePlacer(edge, HCapExt, extPinRad, intPinRad, 0, 0, 2, L, W, 0, 0); //Цилиндр по четырём углам
		}
		for (edge = [0 : 3]) excessTriangle(edge, HCapExt, extPinRad, L, W); //Вырез оставшихся треугольников по четырём углам.
	}
	difference()
	{
		translate([T + capGap, T + capGap, -HCapInt]) //Часть крышки, что заходит внутрь.
		cube([L - 2 * (T + capGap), W - 2 * (T + capGap), HCapInt]);
		if (boxHasInternalPins)
		{
			translate([0, 0, -HCapInt])
			for (edge = [0 : 3]) pinEdgePlacer(edge, HCapInt, extPinRad + capGap, 0, 0, 0, 1, L, W, 0, 0);
		}
	}
}

//Размещает пины (цилиндры с дыркой) по 4-м сторонам прямоугольника, заданного width, length, x, y. Угол задаётся edge.
//Размеры прямоугольника соответствуют краям цилиндров с радиусом er.
//Если радиуса er не достаточно, то можно увеличить его добавкой addToExtRad. Это увеличит внешний радиус без смещения центра пина.
module pinEdgePlacer(edge, hh, er, ir, addToExtRad, holeFromBottom, pinSelect, length, width, deltaX, deltaY)
{
	x = (edge == 1 || edge == 3) ? length - er : er;
	y = (edge == 2 || edge == 3) ? width - er : er;
	translate([x + deltaX, y + deltaY, 0])
	pin(pinSelect, er + addToExtRad, ir, hh, holeFromBottom);
}

//Пины для прикручивания (цилиндры с дыркой).
module pin(pinSelect, er, ir, hh, holeFromBottom) //pinSelect: 0 - цилиндр с дыркой, 1 - только цилиндр, 2 - только дырка в виде цилиндра.
{
	difference()
	{
		if (pinSelect == 0 || pinSelect == 1) cylinder(r = er, h = hh);
		if (pinSelect == 0 || pinSelect == 2)
		{
			translate([0, 0, holeFromBottom])
			cylinder(r = ir, h = hh - holeFromBottom);
		}
	}
}

//Вырез оставшихся треугольников по четырём углам.
module excessTriangle(edge, hh, er, L, W)
{
	xCyl = (edge == 1 || edge == 3) ? L - er : er;
	yCyl = (edge == 2 || edge == 3) ? W - er : er;
	xCube = (edge == 1 || edge == 3) ? L - er: 0;
	yCube = (edge == 2 || edge == 3) ? W - er : 0;
	difference()
	{
		translate([xCube, yCube, 0])
		cube([er, er, hh]);
		translate([xCyl, yCyl, 0])
		cylinder(r = er, h = hh);
	}
}

module cutInternalPins(edge, hh, er, L, W, T)
{
	xCube = (edge == 1 || edge == 3) ? L - T - er * 2 : T;
	yCube = (edge == 2 || edge == 3) ? W - T - er * 2 : T;
	translate([xCube, yCube, T])
	cube([er * 2, er * 2, hh]);
}

//Столбики с вырезом - держалки для чего-либо.
module pillar(wPillar, hPillar, hFromPlate, cut)
{
	difference()
	{
		cube([wPillar, wPillar, hPillar]);
		translate([wPillar * (1 - cut), wPillar * (1 - cut), hFromPlate])
		cube([wPillar * cut, wPillar * cut, hPillar - hFromPlate]);
	}
}

//Размещает столбики-держалки по четырём углам.
module pillars(L, W, wPillar, hPillar, hFromPlate, cut) //W - по x ширина, L - по y длина - расстояние между стойками
{
	delta = (1 - cut) * wPillar;
	render()
	for (edge = [0 : 3])
	{
		x = (edge == 1 || edge == 3) ? L + delta : 0 - delta;
		y = (edge == 2 || edge == 3) ? W + delta : 0 - delta;
		alpha = (edge == 0 ? 0 : (edge == 1 ? 90 : (edge == 2 ? -90 : 180)));
		translate([x, y, 0])
		rotate([0, 0, alpha])
		pillar(wPillar, hPillar, hFromPlate, cut);
	}
}

//Делает скруглённые отверстия.
module roundedHole(L, W, H)
{
	linear_extrude(height = H)
	hull()
	{
		translate([W / 2, 0, 0])
		circle(d = W);
		translate([L - W / 2, 0, 0])
		circle(d = W);
	}
}