using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_HealthPotion : Item
{
    public Item_HealthPotion()
    {
        base.itemId = 2;
        base.itemName = "Health potion";
        base.itemImage = Resources.Load<Sprite>("Items/RedPotion3");
        base.itemActive = new Buff_HealhPotion_10();
        base.itemOneTime = true;
    }
}
