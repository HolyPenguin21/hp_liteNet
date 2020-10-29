using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_BlinkScroll : Item
{
    public Item_BlinkScroll()
    {
        base.itemId = 3;
        base.itemName = "Blink scroll";
        base.itemImage = Resources.Load<Sprite>("Items/Scroll");
        
        base.itemActive = new Buff_Blink();
        base.itemOneTime = true;
    }
}
