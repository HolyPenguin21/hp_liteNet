using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPanel : UI_MainMenu_CanvasPanel
{
    public LobbyPanel (GameObject canvasObj)
    {
        base.canvasObj = canvasObj;
    }

    public override void Setup_RacePicker(bool load, Text player1Name_Text, Text player2Name_Text, Text chat_Text, Dropdown player1RaceDropdown, Dropdown player2RaceDropdown, Button startGame_button)
    {
        player1Name_Text.text = "Waiting ...";
        player2Name_Text.text = "Waiting ...";
        
        chat_Text.text = "Chat log ...";
        if(load) chat_Text.text = "Loading saved game ..."+ "\n" + chat_Text.text;
        else chat_Text.text = "Starting new game ..."+ "\n" + chat_Text.text;

        player1RaceDropdown.interactable = false;
        player2RaceDropdown.interactable = false;
        startGame_button.interactable = false;

        if (Utility.IsServer())
        {
            startGame_button.interactable = true;

            player1Name_Text.text = GameMain.inst.server.players[0].name;
            if (GameMain.inst.server.players.Count > 1)
                player2Name_Text.text = GameMain.inst.server.players[1].name;
            player1RaceDropdown.interactable = true;
        }
        else
        {
            player1Name_Text.text = GameMain.inst.client.players[0].name;
            player2Name_Text.text = GameMain.inst.client.players[1].name;
            player2RaceDropdown.interactable = true;
        }
    }
}
