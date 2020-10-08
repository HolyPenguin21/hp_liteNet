using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceChange
{
    public string playerName { get; set; }
    public int raceId { get; set; }

	public IEnumerator Implementation()
	{
		if (Utility.IsServer())
		{
			if (playerName == GameMain.inst.server.players[0].name)
				GameObject.Find("UI").GetComponent<UI_MainMenu>().player1RaceDropdown.value = raceId;

			if (GameMain.inst.server.players.Count > 1)
				if (playerName == GameMain.inst.server.players[1].name)
					GameObject.Find("UI").GetComponent<UI_MainMenu>().player2RaceDropdown.value = raceId;

			Utility.Get_Client_byString(playerName, GameMain.inst.server.players).race = raceId;
		}
		else
		{
			if (playerName == GameMain.inst.client.players[0].name)
				GameObject.Find("UI").GetComponent<UI_MainMenu>().player1RaceDropdown.value = raceId;

			if (playerName == GameMain.inst.client.players[1].name)
				GameObject.Find("UI").GetComponent<UI_MainMenu>().player2RaceDropdown.value = raceId;

			Utility.Get_Client_byString(playerName, GameMain.inst.client.players).race = raceId;
		}

		yield return null;
	}
}
