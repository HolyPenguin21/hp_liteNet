using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroChange
{
	public string playerName { get; set; }
	public int optionId { get; set; }

	public IEnumerator Implementation(UI_MainMenu ui_mm)
	{
		if (Utility.IsServer())
		{
			if (playerName == GameMain.inst.server.players[0].name)
			{
				ui_mm.player1HeroDropdown.value = optionId;
				Set_HeroId(ui_mm.player1RaceDropdown, ui_mm.player1HeroDropdown, GameMain.inst.server.players[0]);
			}

			if (GameMain.inst.server.players.Count > 1)
				if (playerName == GameMain.inst.server.players[1].name)
				{
					ui_mm.player2HeroDropdown.value = optionId;
					Set_HeroId(ui_mm.player2RaceDropdown, ui_mm.player2HeroDropdown, GameMain.inst.server.players[1]);
				}
		}
		else
		{
			if (playerName == GameMain.inst.client.players[0].name)
			{
				ui_mm.player1HeroDropdown.value = optionId;
				Set_HeroId(ui_mm.player1RaceDropdown, ui_mm.player1HeroDropdown, GameMain.inst.server.players[0]);
			}

			if (playerName == GameMain.inst.client.players[1].name)
			{
				ui_mm.player2HeroDropdown.value = optionId;
				Set_HeroId(ui_mm.player2RaceDropdown, ui_mm.player2HeroDropdown, GameMain.inst.server.players[1]);
			}
		}

		yield return null;
	}

	private void Set_HeroId(Dropdown race, Dropdown hero, Player player)
	{
		if (race.value == 0) // Humans
		{
			switch (hero.value)
			{
				case 0: // Mage
					player.heroId = 10;
					break;
				case 1: // Pikeman
					player.heroId = 3;
					break;
				case 2: // Longbowman
					player.heroId = 8;
					break;
			}
		}
		else if (race.value == 1) // Orcs
		{
			switch (hero.value)
			{
				case 0: // Orc 1
					player.heroId = 10;
					break;
				case 1: // Orc 2
					player.heroId = 10;
					break;
				case 2: // Orc 3
					player.heroId = 10;
					break;
			}
		}
		else if (race.value == 2) // Undeads
		{
			switch (hero.value)
			{
				case 0: // DarkSorcerer
					player.heroId = 22;
					break;
				case 1: // Revenant
					player.heroId = 13;
					break;
				case 2: // Necrophage
					player.heroId = 15;
					break;
			}
		}
	}
}
