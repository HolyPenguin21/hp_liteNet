using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ABuffData : MonoBehaviour
{
    public ABuff Get_ABuff_byId(int id)
    {
        if (id == 1) return new ABuff_DrainLife();
        else if (id == 2) return new ABuff_PoisonTouch();
        else if (id == 3) return new ABuff_Charge();
        else if (id == 4) return new ABuff_Marksman();

        return null;
    }
}
