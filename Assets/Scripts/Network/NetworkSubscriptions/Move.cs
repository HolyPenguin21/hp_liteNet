using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move
{
    public int coord_x { get; set; }
    public int coord_y { get; set; }
    public int mpLeft { get; set; }
    public string pathData { get; set; }

    public void Setup(Character character, List<Hex> path)
    {
        Utility.GridCoord charCoords = GameMain.inst.gridManager.Get_GridCoord_ByHex(character.hex);
        coord_x = charCoords.coord_x;
        coord_y = charCoords.coord_y;

        mpLeft = character.charMovement.movePoints_cur - GameMain.inst.pathfinding.Get_PathCost(character, path);

        List<Utility.GridCoord> pathData = GameMain.inst.gridManager.Get_CoordPath(path);
        for (int i = 0; i < pathData.Count; i++)
        {
            this.pathData += "|";
            this.pathData += pathData[i].coord_x + ";";
            this.pathData += pathData[i].coord_y;
        }

        if (this.pathData[0].ToString() == "|") this.pathData = this.pathData.Substring(1);
    }

    public IEnumerator Server_Implementation(Character character, List<Hex> path)
    {
        character.charMovement.movePoints_cur = mpLeft;
        yield return character.Move(path);
    }

    public IEnumerator Client_Implementation(Move move)
    {
        Character character = GameMain.inst.gridManager.Get_GridItem_ByCoords(move.coord_x, move.coord_y).hex.character;
        character.charMovement.movePoints_cur = move.mpLeft;

        string[] pathData = move.pathData.Split('|');
        List<Hex> path = new List<Hex>();
        for (int j = 0; j < pathData.Length; j++)
        {
            string[] hexCoords = pathData[j].Split(';');
            int posX = int.Parse(hexCoords[0]);
            int posY = int.Parse(hexCoords[1]);
            path.Add(GameMain.inst.gridManager.Get_GridItem_ByCoords(posX, posY).hex);
        }

        yield return character.Move(path);
    }
}
