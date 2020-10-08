using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Belt : Item
{
    public Item_Belt()
    {
        base.itemId = 1;
        base.itemName = "Belt";
        base.itemImage = Resources.Load<Sprite>("Items/Belt");

        base.itemBuffs.Add(new Buff_Hp_up_10());
    }
}
