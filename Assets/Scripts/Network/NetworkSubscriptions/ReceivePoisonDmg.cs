﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReceivePoisonDmg
{
    public int coord_x { get; set; }
    public int coord_y { get; set; }
    public int amount { get; set; }
    public int hpLeft { get; set; }

    public IEnumerator Setup(Hex charHex)
    {
        Utility.GridCoord gridCoord = GameMain.inst.gridManager.Get_GridCoord_ByHex(charHex);
        coord_x = gridCoord.coord_x;
        coord_y = gridCoord.coord_y;
        amount = Utility.villageHeal;
        hpLeft = charHex.character.charHp.hp_cur - amount; if (hpLeft <= 0) hpLeft = 1;

        yield return null;
    }

    public IEnumerator Implementation()
    {
        Character character = GameMain.inst.gridManager.Get_GridItem_ByCoords(coord_x, coord_y).hex.character;
        character.RecieveDmg(amount, hpLeft);

        yield return null;
    }
}
