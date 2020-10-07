using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrategyCam : MonoBehaviour {

    public int levelArea = 100;
    
    public float dragSpeed = 0.1f;
    public float zoomScale = 1;

    private Camera s_Camera;

    private Vector3 curPos;
    public bool isMoving;

    private void Awake()
    {
        s_Camera = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        // Init camera translation for this frame.
        var translation = Vector3.zero;

        // Move camera with arrow keys
        translation += new Vector3(Input.GetAxis("Horizontal") * dragSpeed, 0, Input.GetAxis("Vertical") * dragSpeed);

        curPos = s_Camera.transform.position;
        if (curPos.x > levelArea) curPos.x = levelArea - 0.1f;
        if (curPos.z > levelArea) curPos.z = levelArea - 0.1f;
        if (curPos.x < -levelArea) curPos.x = -levelArea + 0.1f;
        if (curPos.z < -levelArea) curPos.z = -levelArea + 0.1f;

        curPos += translation;
        transform.position = curPos;

        if (transform.hasChanged)
        {
            transform.hasChanged = false;
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }
    }
}
