using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ingame_Input : MonoBehaviour
{
    public Hex mouseOverHex;
    public Hex selectedHex;

    public bool mouseOverUI = false;

    // Spell
    public bool castingSpell = false;
    public Spell spell_Active;
    public List<Hex> spell_HexConcerned = new List<Hex>();

    // Raycasting
    private Ray mouseRay;
    private RaycastHit mouseHit;

    // References
    private Camera scene_Camera;
    private Pathfinding pathfinding;
    private UI_Ingame uI_Ingame;

    // Prefabs
    public GameObject hexHover_pref;
    private SpriteRenderer hoverImage;
    private SpriteRenderer attackImage;
    private SpriteRenderer moveImage;
    public GameObject hexSelected_pref;

    // Scene objects
    private Transform hexHover_tr;
    private Transform hexSelected_tr;

    private void Awake()
    {
        scene_Camera = Camera.main;
        uI_Ingame = GetComponent<UI_Ingame>();

        hexHover_tr = Instantiate(hexHover_pref, transform).transform;
        hoverImage = hexHover_tr.Find("Hover_Image").GetComponent<SpriteRenderer>();
        attackImage = hexHover_tr.Find("Attack_Image").GetComponent<SpriteRenderer>();
        moveImage = hexHover_tr.Find("Move_Image").GetComponent<SpriteRenderer>();
        hexHover_tr.gameObject.SetActive(false);
        attackImage.enabled = false;
        moveImage.enabled = false;

        hexSelected_tr = Instantiate(hexSelected_pref, transform).transform;
        hexSelected_tr.gameObject.SetActive(false);
    }

    private void Start()
    {
        pathfinding = GameMain.inst.pathfinding;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
            Reset_All();

        if (mouseOverUI) return;

        MouseInput_Constant();
    }

    public void OnClick()
    {
        return;

        // if (mouseOverUI) return;

        // Reset_All();
    }

    public void OnDoubleClick()
    {
        if (mouseOverUI) return;
        if (Utility.IsServer())
        {
            if (!GameMain.inst.server.player.isAvailable) return;
        }
        else
        {
            if (!GameMain.inst.client.player.isAvailable) return;
        }

        MouseInput_DoubleClick();
    }

    public void OnHold()
    {
        if (mouseOverUI) return;
        if (castingSpell) return;
        if (Utility.IsServer())
        {
            if (!GameMain.inst.server.player.isAvailable) return;
        }
        else
        {
            if (!GameMain.inst.client.player.isAvailable) return;
        }

        Hex clickedHex = HittedObject();
        if (clickedHex == null) return;

        MouseInput_Hold();
    }

    private void MouseInput_Constant()
    {
        Hex someHex = HittedObject();
        if (someHex == null || uI_Ingame.somePanelIsOn)
        {
            Reset_Hover();
            return;
        }

        if (someHex == mouseOverHex) return;
        pathfinding.Hide_Path();
        mouseOverHex = someHex;

        Set_HoverPosition(mouseOverHex);

        if(castingSpell)
        {
            Spellcasting_Constant(mouseOverHex);
            return;
        }
        else
        {
            if (selectedHex != null)
            {
                if (selectedHex.character != null && selectedHex.character.canAct)
                {
                    if (mouseOverHex.character != null)
                    {
                        if (Utility.IsMyCharacter(mouseOverHex.character))
                        {
                            Set_HoverImage(0);
                        }
                        else
                        {
                            if(Utility.CharacterIsVisible(mouseOverHex.character))
                            {
                                pathfinding.Show_Path(selectedHex, mouseOverHex);
                                Set_HoverImage(2);
                            }
                            else
                            {
                                pathfinding.Show_Path(selectedHex, mouseOverHex);
                                Set_HoverImage(1);
                            }
                        }
                    }
                    else
                    {
                        if (selectedHex.character.charMovement.movePoints_cur > 0)
                        {
                            if(Utility.CharacterIsVisible(selectedHex.character))
                            {
                                Set_HoverImage(1);
                                pathfinding.Show_Path(selectedHex, mouseOverHex);
                            }
                        }
                        else
                        {
                            Set_HoverImage(0);
                        }
                    }
                }
                else
                {
                    Set_HoverImage(0);
                }
            }
        }
    }

    private void MouseInput_DoubleClick()
    {
        Hex clickedHex = HittedObject();
        if (clickedHex == null)
        {
            Reset_All();
            return;
        }

        if(selectedHex == null)
        {
            SelectHex(clickedHex);
            return;
        }

        if(castingSpell)
        {
            if(GameMain.inst.spellData.InRange(selectedHex, spell_HexConcerned[0], spell_Active))
            {
                //if (Utility.IsServer())
                //    StartCoroutine(GameMain.inst.Server_CastSpell(selectedHex, spell_HexConcerned[0], spell_Active.spellId));
                //else
                //    GameMain.inst.Request_CastSpell(selectedHex, spell_HexConcerned[0], spell_Active.spellId);
            }

            Spellcasting_Cancel();
            return;
        }

        Character character = selectedHex.character;
        if (character != null)
        {
            if (Utility.IsMyCharacter(character))
            {
                if (clickedHex.character != null)
                {
                    if (Utility.IsEmeny(character, clickedHex.character))
					{
                        if(Utility.CharacterIsVisible(clickedHex.character))
                        {
                            if (character.canAct)
                            {
                                //GameMain.inst.Try_Attack(selectedHex, clickedHex);
                                pathfinding.Hide_Path();
                            }
                            else
                            {
                                SelectHex(clickedHex);
                                pathfinding.Hide_Path();
                            }
                        }
                        else
                        {
                            if (character.charMovement.movePoints_cur > 0)
                            {
                                if (Utility.IsServer())
                                    StartCoroutine(GameMain.inst.Server_Move(pathfinding.Get_Path(selectedHex, clickedHex)));
                                else
                                    GameMain.inst.Request_Move(pathfinding.Get_Path(selectedHex, clickedHex));

                                pathfinding.Hide_Path();
                            }
                            else
                            {
                                SelectHex(clickedHex);
                                pathfinding.Hide_Path();
                            }
                        }
					}
					else
					{
						SelectHex(clickedHex);
                        pathfinding.Hide_Path();
					}
                }
                else if (character.charMovement.movePoints_cur > 0)
                {
                    if (Utility.IsServer())
                        StartCoroutine(GameMain.inst.Server_Move(pathfinding.Get_Path(selectedHex, clickedHex)));
                    else
                        GameMain.inst.Request_Move(pathfinding.Get_Path(selectedHex, clickedHex));

                    pathfinding.Hide_Path();
                }
                else
                {
                    SelectHex(clickedHex);
                    pathfinding.Hide_Path();
                }
            }
            else
            {
                SelectHex(clickedHex);
                pathfinding.Hide_Path();
            }
        }
        else
		{
            SelectHex(clickedHex);
            pathfinding.Hide_Path();
		}
    }

    private void MouseInput_Hold()
    {
        Hex clickedHex = HittedObject();
        if (clickedHex == null) return;
        if (castingSpell) return;

        if (Utility.IsServer())
        {
            if (GameMain.inst.currentTurn.name != GameMain.inst.server.player.name) return;
        }
        else
        {
            if (GameMain.inst.currentTurn.name != GameMain.inst.client.player.name) return;
        }        

        if(clickedHex.rootCastle == null) return;
        if(clickedHex.rootCastle.character == null || !clickedHex.rootCastle.character.heroCharacter) return;
        if(clickedHex.character != null) return;

        GameObject.Find("UI").GetComponent<UI_Ingame>().Recruit_OpenMenu(clickedHex);
    }

    private void Spellcasting_Constant(Hex someHex)
    {
        Set_HoverImage(0);
        GameMain.inst.fog.Update_Fog();

        // Clear previos visuals
        foreach(Hex h in spell_HexConcerned)
            h.Hide_Target();
        spell_HexConcerned.Clear();

        // Get new hexes to draw visuals
        spell_HexConcerned = GameMain.inst.spellData.Get_ConcernedHexes(someHex, spell_Active.spellId);

        // Check range
        if(GameMain.inst.spellData.InRange(selectedHex, someHex, spell_Active))
        {
            foreach(Hex h in spell_HexConcerned)
                h.target.transform.GetComponent<SpriteRenderer>().color = Color.white;
        }
        else
        {
            foreach(Hex h in spell_HexConcerned)
                h.target.transform.GetComponent<SpriteRenderer>().color = Color.red;
        }

        // Draw visuals
        foreach(Hex h in spell_HexConcerned)
            h.Show_Target();
    }
    public void Spellcasting_Cancel()
    {
        foreach(Hex h in spell_HexConcerned)
            h.Hide_Target();
        spell_HexConcerned.Clear();

        castingSpell = false;
		spell_Active = null;

        if(selectedHex != null)
            if(Utility.CharacterIsVisible(selectedHex.character))
                GameMain.inst.fog.Show_MoveHexes(selectedHex.character);

        GameObject.Find("UI").GetComponent<UI_Ingame>().cInfo_Spell_Cancel.SetActive(false);
    }

    private Hex HittedObject()
    {
        mouseRay = scene_Camera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(mouseRay, out mouseHit, 50.0f))
        {
            if (mouseHit.collider.CompareTag("Hex"))
                return GameMain.inst.gridManager.Get_GridItem_ByTransform(mouseHit.collider.transform).hex;
        }

        return null;
    }

    public void SelectHex(Hex hex)
    {
        selectedHex = hex;

        hexSelected_tr.gameObject.SetActive(true);
        hexSelected_tr.position = selectedHex.transform.position;

        GameObject.Find("UI").GetComponent<UI_Ingame>().Show_HexInfo(selectedHex);

        if(selectedHex.character != null)
        {
            GameMain.inst.fog.Update_Fog();

            if(Utility.CharacterIsVisible(selectedHex.character))
                GameMain.inst.fog.Show_MoveHexes(selectedHex.character);
        }
    }
    public void SelectHex(Transform clickedHex)
	{
		selectedHex = GameMain.inst.gridManager.Get_GridItem_ByTransform(clickedHex).hex;

		hexSelected_tr.gameObject.SetActive(true);
		hexSelected_tr.position = clickedHex.position;

		GameObject.Find("UI").GetComponent<UI_Ingame>().Show_HexInfo(selectedHex);

        GameMain.inst.fog.Update_Fog();

        if(selectedHex.character != null)
            GameMain.inst.fog.Show_MoveHexes(selectedHex.character);
	}

    public void Reset_All()
    {
        mouseOverHex = null;
        hexHover_tr.gameObject.SetActive(false);

        selectedHex = null;
        hexSelected_tr.gameObject.SetActive(false);

        pathfinding.Hide_Path();

        Set_HoverImage(0);
        GameObject.Find("UI").GetComponent<UI_Ingame>().Hide_HexInfo();

        GameObject.Find("UI").GetComponent<UI_Ingame>().Button_Cancel_Spell();

        mouseOverUI = false;

        GameMain.inst.fog.Update_Fog();
    }
    private void Reset_Hover()
    {
        mouseOverHex = null;
        hexHover_tr.gameObject.SetActive(false);
    }

    private void Set_HoverImage(int id)
    {
        switch (id)
        {
            case 0:
                hoverImage.enabled = true;
                attackImage.enabled = false;
                moveImage.enabled = false;
                break;
            case 1:
                hoverImage.enabled = false;
                attackImage.enabled = false;
                moveImage.enabled = true;
                break;
            case 2:
                hoverImage.enabled = false;
                attackImage.enabled = true;
                moveImage.enabled = false;
                break;
        }
    }

    private void Set_HoverPosition(Hex hex)
    {
        hexHover_tr.gameObject.SetActive(true);
        hexHover_tr.position = hex.transform.position;
    }
}
