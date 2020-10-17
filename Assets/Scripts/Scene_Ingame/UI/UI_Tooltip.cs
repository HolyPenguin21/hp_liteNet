using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Tooltip : MonoBehaviour
{
    public GameObject tooltipPrefab;
    public GameObject tooltipObj;
    public RectTransform tooltipRect;
    private Text tooltipText;

    private float width;
    private float widthTreshold;
    private float height;
    private float heightTreshold;
    private Vector2 tooltipPos;

    private void Start()
    {
        width = Screen.width;
        widthTreshold = width * 0.1f;
        height = Screen.height;
        heightTreshold = height * 0.1f;

        tooltipObj = Instantiate(tooltipPrefab).transform.Find("Tooltip_Panel").gameObject;
        tooltipRect = tooltipObj.GetComponent<RectTransform>();
        tooltipText = tooltipObj.transform.Find("Text").GetComponent<Text>();

        Hide_Tooltip();
    }

    private void Update()
    {
        if (!tooltipObj.activeInHierarchy) return;

        Vector2 mouse = Input.mousePosition;

        if (mouse.x < width / 2 && mouse.y >= height / 2) tooltipPos = new Vector2(mouse.x + widthTreshold, mouse.y - heightTreshold);
        if (mouse.x >= width / 2 && mouse.y >= height / 2) tooltipPos = new Vector2(mouse.x - widthTreshold, mouse.y - heightTreshold);
        if (mouse.x < width / 2 && mouse.y < height / 2) tooltipPos = new Vector2(mouse.x + widthTreshold, mouse.y + heightTreshold);
        if (mouse.x >= width / 2 && mouse.y < height / 2) tooltipPos = new Vector2(mouse.x - widthTreshold, mouse.y + heightTreshold);

        tooltipRect.position = tooltipPos;
    }

    public void Show_Tooltip(string textId)
    {
        switch (textId)
        {
            case "skill_1":

            break;
        }
        string tooltip = "";

        tooltipText.text = tooltip;
        tooltipObj.SetActive(true);
    }
    public void Hide_Tooltip()
    {
        tooltipObj.SetActive(false);
    }
}
