using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RaceChange
{
	public string playerName { get; set; }
	public int raceId { get; set; }

	public IEnumerator Implementation(UI_MainMenu ui_mm)
	{
		if (Utility.IsServer())
		{
			if (playerName == GameMain.inst.server.players[0].name)
			{
				ui_mm.player1RaceDropdown.value = raceId;
				Set_HeroOption_P1(ui_mm, GameMain.inst.server.players[0]);
			}

			if (GameMain.inst.server.players.Count > 1)
				if (playerName == GameMain.inst.server.players[1].name)
				{
					ui_mm.player2RaceDropdown.value = raceId;
					Set_HeroOption_P2(ui_mm, GameMain.inst.server.players[1]);
				}

			Utility.Get_Client_byString(playerName, GameMain.inst.server.players).race = raceId;
		}
		else
		{
			if (playerName == GameMain.inst.client.players[0].name)
			{
				ui_mm.player1RaceDropdown.value = raceId;
				Set_HeroOption_P1(ui_mm, GameMain.inst.client.players[0]);
			}

			if (playerName == GameMain.inst.client.players[1].name)
			{
				ui_mm.player2RaceDropdown.value = raceId;
				Set_HeroOption_P2(ui_mm, GameMain.inst.client.players[1]);
			}

			Utility.Get_Client_byString(playerName, GameMain.inst.client.players).race = raceId;
		}

		yield return null;
	}

	private void Set_HeroOption_P1(UI_MainMenu ui_mm, Player player)
	{
		switch (ui_mm.player1RaceDropdown.value)
		{
			case 0: // Humans
				ui_mm.player1HeroDropdown.ClearOptions();
				List<string> humHeroes = new List<string> { "Mage", "Knight", "Hunter" };
				ui_mm.player1HeroDropdown.AddOptions(humHeroes);
				
				player.heroId = 10;
				break;
			case 1: // Orcs
				ui_mm.player1HeroDropdown.ClearOptions();
				List<string> orcHeroes = new List<string> { "Orc 1", "Orc 2", "Orc 3" };
				ui_mm.player1HeroDropdown.AddOptions(orcHeroes);
				break;
			case 2: // Undeads
				ui_mm.player1HeroDropdown.ClearOptions();
				List<string> undHeroes = new List<string> { "Mage", "Skeleton", "Necromancer" };
				ui_mm.player1HeroDropdown.AddOptions(undHeroes);

				player.heroId = 17;
				break;
		}
	}

	private void Set_HeroOption_P2(UI_MainMenu ui_mm, Player player)
	{
		switch (ui_mm.player2RaceDropdown.value)
		{
			case 0: // Humans
				ui_mm.player2HeroDropdown.ClearOptions();
				List<string> humHeroes = new List<string> { "Mage", "Knight", "Hunter" };
				ui_mm.player2HeroDropdown.AddOptions(humHeroes);

				player.heroId = 10;
				break;
			case 1: // Orcs
				ui_mm.player2HeroDropdown.ClearOptions();
				List<string> orcHeroes = new List<string> { "Orc 1", "Orc 2", "Orc 3" };
				ui_mm.player2HeroDropdown.AddOptions(orcHeroes);
				break;
			case 2: // Undeads
				ui_mm.player2HeroDropdown.ClearOptions();
				List<string> undHeroes = new List<string> { "Mage", "Skeleton", "Necromancer" };
				ui_mm.player2HeroDropdown.AddOptions(undHeroes);

				player.heroId = 17;
				break;
		}
	}
}
