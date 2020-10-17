using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkPortalOpen : MonoBehaviour
{
    public float initialScale;
    public float curScale;

    IEnumerator Start()
    {
        initialScale = transform.localScale.x;

        curScale = 0.01f;
        transform.localScale = new Vector3(curScale, 1, curScale);

        while (curScale < initialScale)
        {
            curScale += Time.deltaTime * 4;
            transform.localScale = new Vector3(curScale, 1, curScale);
            yield return null;
        }

        while (curScale > 0.01f)
        {
            curScale -= Time.deltaTime * 2;
            transform.localScale = new Vector3(curScale, 1, curScale);
            yield return null;
        }

        Destroy(gameObject);
    }
}
