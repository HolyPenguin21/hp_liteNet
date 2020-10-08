using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class ChatMessage
{
    public string name { get; set; }
    public string message { get; set; }

    public IEnumerator Implementation()
    {
        Text chat = GameObject.Find("UI").GetComponent<UI_MainMenu>().chat_Text;
        chat.text = name + " : " + message + "\n" + chat.text;

        yield return null;
    }
}
