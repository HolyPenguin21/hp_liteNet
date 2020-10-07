using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_MainMenu : MonoBehaviour
{
    // UI
    public GameObject mainMenu_obj;
    private UI_MainMenu_CanvasPanel mainMenu_canvas;
    public GameObject hostMenu_obj;
    private UI_MainMenu_CanvasPanel hostMenu_canvas;
    public GameObject connectedMenu_obj;
    private UI_MainMenu_CanvasPanel connectedMenu_canvas;
    public GameObject clientMenu_obj;
    private UI_MainMenu_CanvasPanel clientMenu_canvas;
    public GameObject lobbyMenu_obj;
    private UI_MainMenu_CanvasPanel lobbyMenu_canvas;

    public Text clientName_Text;
    public InputField ipAddress_Input;
    public Text connectedPlayersList_Text;
    public Text player1Name_Text;
    public Text player2Name_Text;
    public Dropdown player1RaceDropdown;
    public Dropdown player2RaceDropdown;
    public InputField chatMessage_Input;
    public Text chat_Text;
    public Button startGame_Button;

    // Prefabs
    public GameObject gameMainPrefab;
    public GameObject serverPrefab;
    public GameObject clientPrefab;

    private void Awake()
    {
        Instantiate(gameMainPrefab);

        mainMenu_canvas = new MainMenuPanel(mainMenu_obj);
        hostMenu_canvas = new HostPanel(hostMenu_obj);
        connectedMenu_canvas = new ConnectedPanel(connectedMenu_obj);
        clientMenu_canvas = new ClientPanel(clientMenu_obj);
        lobbyMenu_canvas = new LobbyPanel(lobbyMenu_obj);
    }

    private void Start()
    {
        hostMenu_canvas.ClosePanel();
        clientMenu_canvas.ClosePanel();
        connectedMenu_canvas.ClosePanel();
        lobbyMenu_canvas.ClosePanel();        
    }

    private void Update()
    {
        // send chat message with "Enter" key
        if (Input.GetKeyDown(KeyCode.Return))
            //if (chatMessage_Input.text != "")
                Button_SendMessage();
    }

    public void Button_Host()
    {
        mainMenu_canvas.NonInteractablePanel();
        
        hostMenu_canvas.OpenPanel();

        GameMain.inst.loadGame = false;
    }

    public void Button_NewGame()
    {
        hostMenu_canvas.NonInteractablePanel();

        hostMenu_canvas.CreateServer(serverPrefab, clientName_Text);

        lobbyMenu_canvas.OpenPanel();
        connectedMenu_canvas.OpenPanel();
        Update_ConnectedList();

        Setup_RacePicker();
    }

    public void Button_LoadGame()
    {
        hostMenu_canvas.NonInteractablePanel();

        GameMain.inst.loadGame = true;

        hostMenu_canvas.CreateServer(serverPrefab, clientName_Text);

        lobbyMenu_canvas.OpenPanel();
        Setup_RacePicker();
    }

    public void Button_Client()
    {
        mainMenu_canvas.NonInteractablePanel();
        clientMenu_canvas.OpenPanel();
    }

    public void Button_Cancel()
    {
        GameMain.inst.loadGame = false;

        hostMenu_canvas.DestroyServer();
        clientMenu_canvas.DestroyClient();

        hostMenu_canvas.ClosePanel();
        clientMenu_canvas.ClosePanel();
        connectedMenu_canvas.ClosePanel();
        lobbyMenu_canvas.ClosePanel();

        mainMenu_canvas.InteractablePanel();
        hostMenu_canvas.InteractablePanel();
        clientMenu_canvas.InteractablePanel();
    }

    public void Button_Connect()
    {
        clientMenu_canvas.ConnectToServer(clientPrefab, clientName_Text, ipAddress_Input.text, this);
    }

    public void Client_OnConnect()
    {
        lobbyMenu_canvas.OpenPanel();
        connectedMenu_canvas.OpenPanel();
        Update_ConnectedList();

        clientMenu_canvas.NonInteractablePanel();
    }
    public void Client_OnDidNotConnect()
    {
        clientMenu_canvas.DestroyClient();
        clientMenu_canvas.InteractablePanel();
    }

    public void Update_ConnectedList()
    {
        connectedMenu_canvas.UpdateList(connectedPlayersList_Text);
    }
    public void Setup_RacePicker()
    {
        lobbyMenu_canvas.Setup_RacePicker(GameMain.inst.loadGame, player1Name_Text, player2Name_Text, chat_Text, player1RaceDropdown, player2RaceDropdown, startGame_Button);
    }

    public void OnRaceChange(int clientId)
    {
        RaceChange raceChange = new RaceChange();

        if (clientId == 0)
        {
            raceChange.playerName = GameMain.inst.server.player.name;
            raceChange.raceId = player1RaceDropdown.value;
            StartCoroutine(GameMain.inst.Server_RaceChange(raceChange));
        }

        if (clientId == 1)
        {
            raceChange.playerName = GameMain.inst.client.player.name;
            raceChange.raceId = player2RaceDropdown.value;
            GameMain.inst.Request_RaceChange(raceChange);
        }
    }

    public void Button_SendMessage()
    {
        ChatMessage chatMessage = new ChatMessage();
        chatMessage.message = chatMessage_Input.text;

        if (Utility.IsServer())
        {
            chatMessage.name = GameMain.inst.server.player.name;

            StartCoroutine(GameMain.inst.Server_ChatMessage(chatMessage));
        }
        else
        {
            chatMessage.name = GameMain.inst.client.player.name;

            GameMain.inst.Request_ChatMessage(chatMessage);
        }

        chatMessage_Input.text = String.Empty;
    }

    public void Button_StartGame()
    {
        StartCoroutine(GameMain.inst.Server_LoadScene("Scene_Map_Test"));
    }

    public void Button_Quit()
    {
        Application.Quit();
    }
}
