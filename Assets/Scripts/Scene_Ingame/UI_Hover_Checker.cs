using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Hover_Checker : MonoBehaviour
{
    private Ingame_Input input_sc;

    void Start()
    {
        input_sc = GameObject.Find("UI").GetComponent<Ingame_Input>();
    }

    public void MouseOverUI()
    {
        input_sc.mouseOverUI = true;
    }
    public void MouseNotOverUI()
    {
        input_sc.mouseOverUI = false;
    }
}
