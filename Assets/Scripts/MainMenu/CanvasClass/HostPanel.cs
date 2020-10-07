using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HostPanel : UI_MainMenu_CanvasPanel
{
    public HostPanel (GameObject canvasObj)
    {
        base.canvasObj = canvasObj;
    }

    public override void CreateServer(GameObject serverPrefab, Text clientName_text)
    {
        try
        {
            GameMain.inst.server = InstantiateServer(serverPrefab);
            GameMain.inst.server.Init(Set_ServerName(clientName_text));
        }
        catch (Exception e)
        {
            Debug.Log("Host creation error : " + e.Message);
        }
    }

    public override void DestroyServer()
    {
        GameMain.inst.server = null;

        Server[] servers = GameObject.FindObjectsOfType<Server>();
        if(servers == null) return;

        foreach(Server server in servers)
        {
            server.StopServer();
            MonoBehaviour.Destroy(server.gameObject);
        }
    }

    private Server InstantiateServer(GameObject serverPrefab)
    {
        return MonoBehaviour.Instantiate(serverPrefab).GetComponent<Server>();
    }

    private string Set_ServerName(Text clientName_text)
    {
        string serverName = "Default_Host";

        if (clientName_text.text != "")
            serverName = clientName_text.text + "_Host";
        
        return serverName;
    }
}
