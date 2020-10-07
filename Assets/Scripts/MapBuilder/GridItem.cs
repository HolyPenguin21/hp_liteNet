using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GridItem
{
    public Vector3 worldPos;
    public int coord_x;
    public int coord_y;
    public Hex hex;

    public GridItem (Vector3 worldPos, int coord_x, int coord_y)
    {
        this.worldPos = worldPos;
        this.coord_x = coord_x;
        this.coord_y = coord_y;
        this.hex = null;
    }
}
