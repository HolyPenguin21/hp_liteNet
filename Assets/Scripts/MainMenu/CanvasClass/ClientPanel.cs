using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClientPanel : UI_MainMenu_CanvasPanel
{
    public ClientPanel (GameObject canvasObj)
    {
        base.canvasObj = canvasObj;
    }

    public override void ConnectToServer(GameObject clientPrefab, Text clientName_text, string ipAddress, UI_MainMenu ui_MainMenu)
    {
        try
        {
            GameMain.inst.client = InstantiateClient(clientPrefab);
            GameMain.inst.client.Connect(Set_ClientName(clientName_text), ipAddress);
        }
        catch (Exception e)
        {
            Debug.Log("Error on client creation : " + e.Message);
        }
    }

    public override void DestroyClient()
    {
        GameMain.inst.client = null;

        Client[] clients = GameObject.FindObjectsOfType<Client>();
        if(clients == null) return;

        foreach(Client client in clients)
        {
            client.StopClient();
            MonoBehaviour.Destroy(client.gameObject);
        }
    }

    private Client InstantiateClient(GameObject clientPrefab)
    {
        return MonoBehaviour.Instantiate(clientPrefab).GetComponent<Client>();
    }

    private string Set_ClientName(Text clientName_text)
    {
        string clientName = "Default_Client";

        if (clientName_text.text != "")
            clientName = clientName_text.text + "_Client";
        
        return clientName;
    }
}
