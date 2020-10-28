using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
				Set_HeroId_P1(ui_mm, GameMain.inst.server.players[0]);
			}

			if (GameMain.inst.server.players.Count > 1)
				if (playerName == GameMain.inst.server.players[1].name)
				{
					ui_mm.player2HeroDropdown.value = optionId;
					Set_HeroId_P2(ui_mm, GameMain.inst.server.players[1]);
				}
		}
		else
		{
			if (playerName == GameMain.inst.client.players[0].name)
			{
				ui_mm.player1HeroDropdown.value = optionId;
				Set_HeroId_P1(ui_mm, GameMain.inst.client.players[0]);
			}

			if (playerName == GameMain.inst.client.players[1].name)
			{
				ui_mm.player2HeroDropdown.value = optionId;
				Set_HeroId_P2(ui_mm, GameMain.inst.client.players[1]);
			}
		}

		yield return null;
	}

	private void Set_HeroId_P1(UI_MainMenu ui_mm, Player player)
	{
		if (ui_mm.player1RaceDropdown.value == 0) // Humans
		{
			switch (ui_mm.player1HeroDropdown.value)
			{
				case 0: // Mage
					player.heroId = 10;
					break;
				case 1: // Knight
					player.heroId = 5;
					break;
				case 2: // Hunter
					player.heroId = 9;
					break;
			}
		}
		else if (ui_mm.player1RaceDropdown.value == 1) // Orcs
		{
			switch (ui_mm.player1HeroDropdown.value)
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
		else if (ui_mm.player1RaceDropdown.value == 2) // Undeads
		{
			switch (ui_mm.player1HeroDropdown.value)
			{
				case 0: // Mage
					player.heroId = 17;
					break;
				case 1: // Skeleton
					player.heroId = 13;
					break;
				case 2: // Necromancer
					player.heroId = 22;
					break;
			}
		}
	}

	private void Set_HeroId_P2(UI_MainMenu ui_mm, Player player)
	{
		if (ui_mm.player2RaceDropdown.value == 0) // Humans
		{
			switch (ui_mm.player2HeroDropdown.value)
			{
				case 0: // Mage
					player.heroId = 10;
					break;
				case 1: // Knight
					player.heroId = 5;
					break;
				case 2: // Hunter
					player.heroId = 9;
					break;
			}
		}
		else if (ui_mm.player2RaceDropdown.value == 1) // Orcs
		{
			switch (ui_mm.player2HeroDropdown.value)
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
		else if (ui_mm.player2RaceDropdown.value == 2) // Undeads
		{
			switch (ui_mm.player2HeroDropdown.value)
			{
				case 0: // Mage
					player.heroId = 17;
					break;
				case 1: // Skeleton
					player.heroId = 13;
					break;
				case 2: // Necromancer
					player.heroId = 22;
					break;
			}
		}
	}
}
