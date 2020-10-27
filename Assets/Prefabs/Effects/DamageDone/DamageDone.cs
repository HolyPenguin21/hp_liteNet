using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDone : MonoBehaviour
{
    public float duration = 2f;
    public float speed = 0.5f;

    public Transform textTr;
    public Transform spriteTr;

    // Start is called before the first frame update
    public void Init(int dmg)
    {
        GameObject.Destroy(gameObject, duration);
        textTr.GetComponent<TextMesh>().text = "" + dmg;

        Vector3 pos = new Vector3(Random.Range(textTr.position.x - 0.15f, textTr.position.x + 0.15f), textTr.position.y, textTr.position.z);
        textTr.position = pos;
        spriteTr.position = pos;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.up * speed * Time.deltaTime);
    }
}
