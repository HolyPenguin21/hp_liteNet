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
				Set_HeroOption(ui_mm.player1RaceDropdown, ui_mm.player1HeroDropdown, GameMain.inst.server.players[0]);
			}

			if (GameMain.inst.server.players.Count > 1)
				if (playerName == GameMain.inst.server.players[1].name)
				{
					ui_mm.player2RaceDropdown.value = raceId;
					Set_HeroOption(ui_mm.player2RaceDropdown, ui_mm.player2HeroDropdown, GameMain.inst.server.players[1]);
				}

			Utility.Get_Client_byString(playerName, GameMain.inst.server.players).race = raceId;
		}
		else
		{
			if (playerName == GameMain.inst.client.players[0].name)
			{
				ui_mm.player1RaceDropdown.value = raceId;
				Set_HeroOption(ui_mm.player1RaceDropdown, ui_mm.player1HeroDropdown, GameMain.inst.client.players[0]);
			}

			if (playerName == GameMain.inst.client.players[1].name)
			{
				ui_mm.player2RaceDropdown.value = raceId;
				Set_HeroOption(ui_mm.player2RaceDropdown, ui_mm.player2HeroDropdown, GameMain.inst.client.players[1]);
			}

			Utility.Get_Client_byString(playerName, GameMain.inst.client.players).race = raceId;
		}

		yield return null;
	}

	private void Set_HeroOption(Dropdown race, Dropdown hero, Player player)
	{
		switch (race.value)
		{
			case 0: // Humans
				hero.ClearOptions();
				List<string> humHeroes = new List<string> { "Mage", "Pikeman", "Longbowman" };
				hero.AddOptions(humHeroes);
				
				player.heroId = 10;
				break;
			case 1: // Orcs
				hero.ClearOptions();
				List<string> orcHeroes = new List<string> { "Orc 1", "Orc 2", "Orc 3" };
				hero.AddOptions(orcHeroes);
				break;
			case 2: // Undeads
				hero.ClearOptions();
				List<string> undHeroes = new List<string> { "DarkSorcerer", "Revenant", "Necrophage" };
				hero.AddOptions(undHeroes);

				player.heroId = 17;
				break;
		}
	}
}
