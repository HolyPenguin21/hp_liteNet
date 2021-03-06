﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hex : MonoBehaviour
{
    public Character character;
    [HideInInspector] public GameObject fog;
    [HideInInspector] public SpriteRenderer fogRenderer;
    [HideInInspector] public GameObject target;

    public bool isStartPoint; // set in editor
    public Hex rootCastle; // attach in editor

    public bool neutralsSpawner;
    public bool isVillage;
    public Player villageOwner;

    public bool isMountain;
    public bool isWater;

    public Item item = null;
    public GameObject itemObj;
    private SpriteRenderer itemImage;

    // Pathfinding
    public bool groundMove;
    public bool airMove;
    public List<Hex> neighbors;
    public int moveCost = 1;

    public int dodge;

    private void Start()
    {
        fog = transform.Find("HexFog").gameObject;
        fogRenderer = fog.GetComponent<SpriteRenderer>();
        target = transform.Find("HexTarget").gameObject;
        Hide_Target();

        itemObj = transform.Find("HexItem").gameObject;
        itemImage = itemObj.GetComponent<SpriteRenderer>();
        itemObj.SetActive(false);
    }

    #region Fog
    public void Show_Fog()
    {
        fogRenderer.enabled = true;

        if (item != null) itemObj.SetActive(false);
        if (character != null)
        {
            character.charImageRend.enabled = false;
            character.colorImageRend.enabled = false;
            character.heroImageRend.enabled = false;
            character.itemImageRend.enabled = false;
            character.canMoveImageRend.enabled = false;
        }
    }

    public void Show_MoveFog()
    {
        fogRenderer.enabled = true;
    }

    public void Hide_Fog()
    {
        fogRenderer.enabled = false;

        if (item != null) itemObj.SetActive(true);
        if (character != null)
        {
            character.charImageRend.enabled = true;
            character.colorImageRend.enabled = true;
            character.heroImageRend.enabled = true;
            character.itemImageRend.enabled = true;
            character.canMoveImageRend.enabled = true;
        }
    }
    #endregion

    #region Target
    public void Show_Target()
    {
        target.SetActive(true);
    }

    public void Hide_Target()
    {
        target.SetActive(false);
    }
    #endregion

    #region Items
    public void Add_Item(Item item)
    {
        this.item = item;
        itemObj.SetActive(true);
        itemImage.sprite = item.itemImage;
    }

    public void Remove_Item()
    {
        this.item = null;
        itemObj.SetActive(false);
        itemImage.sprite = null;
    }
    #endregion
}
