using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackRequest
{
    public int a_coord_x { get; set; }
    public int a_coord_y { get; set; }
    public int a_attackId { get; set; }
    public int t_coord_x { get; set; }
    public int t_coord_y { get; set; }
    public int t_attackId { get; set; }

    public void Setup(Character attacker, int a_AttackId, Character target, int t_AttackId)
    {
        Utility.GridCoord a_charCoords = GameMain.inst.gridManager.Get_GridCoord_ByHex(attacker.hex);
        a_coord_x = a_charCoords.coord_x;
        a_coord_y = a_charCoords.coord_y;
        a_attackId = a_AttackId;
        Utility.GridCoord t_charCoords = GameMain.inst.gridManager.Get_GridCoord_ByHex(target.hex);
        t_coord_x = t_charCoords.coord_x;
        t_coord_y = t_charCoords.coord_y;
        t_attackId = t_AttackId;
    }
}
