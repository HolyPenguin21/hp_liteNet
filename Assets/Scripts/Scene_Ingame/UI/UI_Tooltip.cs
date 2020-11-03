using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Tooltip : MonoBehaviour
{
    private GameObject tooltipPrefab;
    public GameObject tooltipCanvas;
    public RectTransform tooltipRect;
    public Text tooltipText;

    private float width;
    private float widthTreshold;
    private float height;
    private float heightTreshold;
    private Vector2 tooltipPos;

    private void Start()
    {
        width = Screen.width;
        widthTreshold = width * 0.11f;
        height = Screen.height;
        heightTreshold = height * 0.1f;

        Hide_Tooltip();
    }

    private void Update()
    {
        if (!tooltipCanvas.activeInHierarchy) return;

        Vector2 mouse = Input.mousePosition;

        if (mouse.x < width / 2 && mouse.y >= height / 2) tooltipPos = new Vector2(mouse.x + widthTreshold, mouse.y - heightTreshold);
        if (mouse.x >= width / 2 && mouse.y >= height / 2) tooltipPos = new Vector2(mouse.x - widthTreshold, mouse.y - heightTreshold);
        if (mouse.x < width / 2 && mouse.y < height / 2) tooltipPos = new Vector2(mouse.x + widthTreshold, mouse.y + heightTreshold);
        if (mouse.x >= width / 2 && mouse.y < height / 2) tooltipPos = new Vector2(mouse.x - widthTreshold, mouse.y + heightTreshold);

        tooltipRect.position = tooltipPos;
    }

    public void Show_Tooltip(string textId)
    {
        bool show = true;
        string tooltip = "";

        switch (textId)
        {
            case "skill_1":
                Spell s1 = GameObject.Find("UI").GetComponent<Ingame_Input>().selectedHex.character.charSpell_1;
                if (s1 == null)
                {
                    show = false;
                    break;
                }
                tooltip = s1.spellName + " : " +
                          "\n  " + s1.description +
                          "\nSpell range : " + s1.spellCastRange +
                          "\nSpell cooldown : " + s1.cooldown_max;

                tooltipRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 200f);
                break;

            case "skill_2":
                Spell s2 = GameObject.Find("UI").GetComponent<Ingame_Input>().selectedHex.character.charSpell_2;
                if (s2 == null)
                {
                    show = false;
                    break;
                }
                tooltip = s2.spellName + " : " +
                          "\n  " + s2.description +
                          "\nSpell range : " + s2.spellCastRange +
                          "\nSpell cooldown : " + s2.cooldown_max;

                tooltipRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 200f);
                break;

            case "char_info":
                Ingame_Input ingameInput = GameObject.Find("UI").GetComponent<Ingame_Input>();
                UI_Ingame uIIngame = GameObject.Find("UI").GetComponent<UI_Ingame>();

                Character c = null;

                if(uIIngame.recruit_Canvas.activeInHierarchy) c = uIIngame.charToRecruit;
                else if (ingameInput.selectedHex != null) c = ingameInput.selectedHex.character;

                if (c == null)
                {
                    show = false;
                    break;
                }
                tooltip = c.charName + " :" +
                          "\n Slash resistance : " + c.charDef.blade_resistance +
                          "\n Pierce resistance : " + c.charDef.pierce_resistance +
                          "\n Blunt resistance : " + c.charDef.impact_resistance +
                          "\n Magic resistance : " + c.charDef.magic_resistance;

                if (c.charSpell_1 != null)
                    tooltip = tooltip + "\n - " + c.charSpell_1.spellName;

                if (c.charSpell_2 != null)
                    tooltip = tooltip + "\n - " + c.charSpell_2.spellName;

                tooltipRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 275f);
                break;
        }

        tooltipText.text = tooltip;
        if (show) tooltipCanvas.SetActive(true);
    }
    public void Hide_Tooltip()
    {
        tooltipCanvas.SetActive(false);
    }
}
