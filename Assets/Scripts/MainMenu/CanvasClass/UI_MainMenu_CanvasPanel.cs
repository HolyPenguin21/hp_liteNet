using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class UI_MainMenu_CanvasPanel
{
    public GameObject canvasObj;

    public void OpenPanel()
    {
        canvasObj.SetActive(true);
    }

    public void ClosePanel()
    {
        canvasObj.SetActive(false);
    }

    public void InteractablePanel()
    {
        Component[] buttons = canvasObj.GetComponentsInChildren(typeof(Button));
        Component[] inputs = canvasObj.GetComponentsInChildren(typeof(InputField));

        if (buttons != null)
        {
            foreach (Button button in buttons)
                button.interactable = true;
        }

        if (inputs != null)
        {
            foreach (InputField input in inputs)
                input.interactable = true;
        }
    }

    public void NonInteractablePanel()
    {
        Component[] buttons = canvasObj.GetComponentsInChildren(typeof(Button));
        Component[] inputs = canvasObj.GetComponentsInChildren(typeof(InputField));

        if (buttons != null)
        {
            foreach (Button button in buttons)
                button.interactable = false;
        }

        if (inputs != null)
        {
            foreach (InputField input in inputs)
                input.interactable = false;
        }
    }

    #region HostPanel
    public virtual void CreateServer(GameObject serverPrefab, Text clientName_text)
    {
        Debug.Log("Sys > Used by HostPanel class only");
        return;
    }

    public virtual void DestroyServer()
    {
        Debug.Log("Sys > Used by HostPanel class only");
        return;
    }
    #endregion

    #region ClientPanel
    public virtual void ConnectToServer(GameObject serverPrefab, Text clientName_text, string ipAddress, UI_MainMenu ui_MainMenu)
    {
        Debug.Log("Sys > Used by ClientPanel class only");
        return;
    }

    public virtual void DestroyClient()
    {
        Debug.Log("Sys > Used by ClientPanel class only");
        return;
    }
    #endregion

    #region ConnectedList
    public virtual void UpdateList(Text text)
    {
        Debug.Log("Sys > Used by ConnectedListPanel class only");
        return;
    }
    #endregion

    #region LobbyPanel
    public virtual void Setup_RacePicker(bool load, Text player1Name_Text, Text player2Name_Text, Text chat_Text, Dropdown player1RaceDropdown, Dropdown player2RaceDropdown, Button startGame_button)
    {
        Debug.Log("Sys > Used by LobbyPanel class only");
        return;
    }
    #endregion
}
