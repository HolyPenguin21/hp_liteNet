using System.Collections.Generic;
using UnityEngine;
// using UnityEditor; // COMMENT

// [ExecuteInEditMode] // COMMENT
public class GridManager : MonoBehaviour
{
    public GridItem[] grids;
    public List<GridItem> startPoints = new List<GridItem>();
    public List<GridItem> villages = new List<GridItem>();
    public List<GridItem> neutralsSpawners = new List<GridItem>();

    #region Ingame Methods
    public GridItem Get_StartGridItem()
    {
        GridItem randGridPos = startPoints[Random.Range(0, startPoints.Count)];
        startPoints.Remove(randGridPos);
        return randGridPos;
    }

    public GridItem Get_GridItem_ByCoords(int posX, int posY)
    {
        for (int x = 0; x < grids.Length; x++)
        {
            if (grids[x].coord_x == posX && grids[x].coord_y == posY)
                return grids[x];
        }

        return grids[0];
    }

    public GridItem Get_GridItem_ByTransform(Transform tr)
    {
        for (int x = 0; x < grids.Length; x++)
        {
            if (grids[x].hex.transform == tr)
                return grids[x];
        }

        return grids[0];
    }

    public GridItem Get_GridItem_ByHex(Hex hex)
    {
        for (int x = 0; x < grids.Length; x++)
        {
            if (grids[x].hex == hex)
                return grids[x];
        }

        return grids[0];
    }

    public List<Utility.GridCoord> Get_CoordPath(List<Hex> hexPath)
    {
        if (hexPath == null) return null;

        List<Utility.GridCoord> coordPath = new List<Utility.GridCoord>();

        for (int x = 0; x < hexPath.Count; x++)
        {
            GridItem gridItem = Get_GridItem_ByHex(hexPath[x]);
            Utility.GridCoord coord;
            coord.coord_x = gridItem.coord_x;
            coord.coord_y = gridItem.coord_y;
            coordPath.Add(coord);
        }

        return coordPath;
    }

    public Utility.GridCoord Get_GridCoord_ByHex(Hex hex)
    {
        Utility.GridCoord coord;
        coord.coord_x = -1;
        coord.coord_y = -1;

        for (int x = 0; x < grids.Length; x++)
        {
            if (grids[x].hex == hex)
            {
                coord.coord_x = grids[x].coord_x;
                coord.coord_y = grids[x].coord_y;
            }
        }
        return coord;
    }
    #endregion
}
