using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrategyCam : MonoBehaviour 
{
    public float camMoveSpeed = 0.05f;
    //public float[] BoundsX = new float[] { -4f, 10f };
    //public float[] BoundsZ = new float[] { -4f, 4f };

    private Camera s_Camera;
    private IngameUI_Camera IngameUI_Camera;

    private void Awake()
    {
        s_Camera = GetComponent<Camera>();
        IngameUI_Camera = GetComponent<IngameUI_Camera>();
    }

    void LateUpdate()
    {
        if (!Settings.inputPc) return;

        Vector3 curPos = transform.position;
        curPos += new Vector3(Input.GetAxis("Horizontal") * camMoveSpeed, 0, Input.GetAxis("Vertical") * camMoveSpeed);

        curPos.x = Mathf.Clamp(curPos.x, IngameUI_Camera.BoundsX[0], IngameUI_Camera.BoundsX[1]);
        curPos.z = Mathf.Clamp(curPos.z, IngameUI_Camera.BoundsZ[0], IngameUI_Camera.BoundsZ[1]);
        transform.position = curPos;
    }
}
