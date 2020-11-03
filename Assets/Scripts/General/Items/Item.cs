using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item
{
    public int itemId;
    public string itemName;
    public Sprite itemImage;
    
    public Buff itemActive = null;
    public bool itemOneTime = false;

    public List<Buff> itemBuffs = new List<Buff>();

    public void Item_OnEquip(Character character)
    {
        for(int x = 0; x < itemBuffs.Count; x++)
        {
            Buff buff = itemBuffs[x];
            if (buff.buffType != Utility.buff_Type.onEquip) continue;

            GameMain.inst.StartCoroutine(buff.Buff_Activate(character));
        }
    }

    public void Item_OnTurn(Character character)
    {
        for (int x = 0; x < itemBuffs.Count; x++)
        {
            Buff buff = itemBuffs[x];
            if (buff.buffType != Utility.buff_Type.onTurn) continue;
            GameMain.inst.StartCoroutine(buff.Buff_Activate(character));
        }
    }

    public void Item_OnRemove(Character character)
    {
        for(int x = 0; x < itemBuffs.Count; x++)
        {
            Buff buff = itemBuffs[x];
            GameMain.inst.StartCoroutine(buff.Buff_Remove(character));
        }
    }
}
