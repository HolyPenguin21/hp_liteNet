using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConnectedPanel : UI_MainMenu_CanvasPanel
{
    public ConnectedPanel(GameObject canvasObj)
    {
        base.canvasObj = canvasObj;
    }

    public override void UpdateList(Text text)
    {
        text.text = "";
        if (Utility.IsServer())
        {
            for (int x = 0; x < GameMain.inst.server.players.Count; x++)
                text.text += x + 1 + ". " + GameMain.inst.server.players[x].name + "\n";
        }
        else
        {
            for (int x = 0; x < GameMain.inst.client.players.Count; x++)
                text.text += x + 1 + ". " + GameMain.inst.client.players[x].name + "\n";
        }
    }
}
