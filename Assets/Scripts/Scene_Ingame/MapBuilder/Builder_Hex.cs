using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Builder_Hex : MonoBehaviour
{
    private Builder_Grid gridBuilder;
    private Hex hex;

    private void Start()
    {
        gridBuilder = GameObject.Find("GridManager").GetComponent<Builder_Grid>();
        hex = GetComponent<Hex>();
    }
    void Update()
    {
        transform.position = gridBuilder.Get_ClosestGridPos(hex);
    }
}
