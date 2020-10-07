﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickHandler : MonoBehaviour
{
    public int clickCount = 0;
    public float double_timer = 0.0f;
    private float clickDelay = 0.3f;

    public float hold_timer = 0.0f;
    private float holdDelay = 0.5f;
    public bool holdOn = false;

    private Ingame_Input input;

    private void Awake()
    {
        input = GetComponent<Ingame_Input>();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            clickCount++;
            hold_timer = 0.0f;
        }
        
        if(Input.GetKey(KeyCode.Mouse0))
            hold_timer += Time.deltaTime;

        if(Input.GetKeyUp(KeyCode.Mouse0))
        {
            hold_timer = 0.0f;
            holdOn = false;
        }

        if(clickCount == 1)
            double_timer += Time.deltaTime;

        if(clickCount >= 2)
        {
            double_timer = 0.0f;
            clickCount = 0;
            input.OnDoubleClick();
        }

        if(double_timer >= clickDelay)
        {
            double_timer = 0.0f;
            clickCount = 0;

            if(hold_timer == 0.0f && !input.mouseOverUI)
                input.OnClick();
        }

        if(hold_timer >= holdDelay && !holdOn)
        {
            holdOn = true;
            input.OnHold();
        }
    }
}
