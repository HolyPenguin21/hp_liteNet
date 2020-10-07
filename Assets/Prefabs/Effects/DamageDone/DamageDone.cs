using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDone : MonoBehaviour
{
    public float duration = 2f;
    public float speed = 0.5f;

    // Start is called before the first frame update
    public void Init(int dmg)
    {
        GameObject.Destroy(gameObject, duration);
        transform.Find("Text").GetComponent<TextMesh>().text = "" + dmg;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.up * speed * Time.deltaTime);
    }
}
