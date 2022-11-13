using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsoGrid : MonoBehaviour
{
	[SerializeField] private int IsoW = 40;
	[SerializeField] private int IsoH = 20;

	[SerializeField] private int IsoX;
	[SerializeField] private int IsoY;


	int IsoToScreenX(int localX, int localY)
	{
		return IsoX + (localX - localY) * IsoW;
	}
	int IsoToScreenY(int localX, int localY)
	{
		return IsoY + (localX + localY) * IsoH;
	}
	int ScreenToIsoX(int globalX, int globalY)
	{
		return ((globalX - IsoX) / IsoW + (globalY - IsoY) / IsoH) / 2;
	}
	int ScreenToIsoY(int globalX, int globalY)
	{
		return ((globalY - IsoY) / IsoH - (globalX - IsoX) / IsoW) / 2;
	}

	// Draws an isometric tile at the given coordinates
	void DrawIsoTile(int x, int y)
	{
	}

}
