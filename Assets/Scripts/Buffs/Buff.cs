using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Buff
{
    public int buffId;
    public string buffName;
    public string buffDescription;
    public Utility.buff_Type buffType;

    public abstract void Buff_Activate(Character character);

    public abstract void Buff_Remove(Character character);
}
