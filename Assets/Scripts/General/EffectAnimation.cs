using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectAnimation : MonoBehaviour
{
    public float animationLenght = 1.5f;

    void Start()
    {
        GameObject.Destroy(gameObject, animationLenght);
    }
}
