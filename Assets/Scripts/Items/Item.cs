using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item
{
    public int itemId;
    public string itemName;
    public Sprite itemImage;
    public List<Buff> itemBuffs = new List<Buff>();
    public Buff itemActive = null;
    public bool itemOneTime = false;

    public void Item_OnEquip(Character character)
    {
        for(int x = 0; x < itemBuffs.Count; x++)
        {
            if(itemBuffs[x].buffType != Utility.buff_Type.onEquip) continue;
            Buff buff = itemBuffs[x];

            buff.Buff_Activate(character);
        }
    }

    public void Item_OnRemove(Character character)
    {
        for(int x = 0; x < itemBuffs.Count; x++)
        {
            Buff buff = itemBuffs[x];
            buff.Buff_Remove(character);
        }
    }
}
