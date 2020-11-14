using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveRequest
{
    public int c_coord_x { get; set; }
    public int c_coord_y { get; set; }
    public int d_coord_x { get; set; }
    public int d_coord_y { get; set; }

    public void Setup(Character character, Hex destination)
    {
        Utility.GridCoord charCoords = GameMain.inst.gridManager.Get_GridCoord_ByHex(character.hex);
        c_coord_x = charCoords.coord_x;
        c_coord_y = charCoords.coord_y;

        Utility.GridCoord destCoords = GameMain.inst.gridManager.Get_GridCoord_ByHex(destination);
        d_coord_x = destCoords.coord_x;
        d_coord_y = destCoords.coord_y;
    }
}
