using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fog
{
	private Server s;
	private Client c;
	private GridManager g;

	public Fog(Server server, Client client, GridManager gridManager)
	{
		s = server;
		c = client;
		g = gridManager;
	}

	public void UpdateFog_PlayerView()
	{
		for (int i = 0; i < g.grids.Length; i++)
		{
			Hex hex = g.grids[i].hex;
			hex.Show_Fog();

			for (int x = 0; x < GameMain.inst.allCharacters.Count; x++)
			{
				Character character = GameMain.inst.allCharacters[x];
				if (Utility.IsMyCharacter(character) && Utility.IsHexVisibleForChar(hex, character))
					hex.Hide_Fog();
			}
		}
	}

	public void UpdateFog_CharacterView(Character character)
	{
		List<Hex> hexesInRange = new List<Hex>();
		List<Hex> moveHexes = new List<Hex>();

		int range_cur = character.charMovement.movePoints_cur;

		for (int i = 0; i < g.grids.Length; i++)
		{
			Hex hex = g.grids[i].hex;

			if (Utility.IsHexVisibleForChar(hex, character))
			{
				hex.Hide_Fog();
				hexesInRange.Add(g.grids[i].hex);
			}
		}

		for (int i = 0; i < hexesInRange.Count; i++)
		{
			if (hexesInRange[i].character != null) continue;

			List<Hex> path = GameMain.inst.pathfinding.Get_Path(character.hex, hexesInRange[i]);
			if (path == null) continue;
			int pathCost = GameMain.inst.pathfinding.Get_PathCost(character, path);

			if (pathCost <= range_cur)
				moveHexes.Add(hexesInRange[i]);
		}

		for (int i = 0; i < g.grids.Length; i++)
		{
			if (!moveHexes.Contains(g.grids[i].hex))
				g.grids[i].hex.Show_MoveFog();
		}

		character.hex.Hide_Fog();
	}

	public void Hide_Fog()
	{
		for (int i = 0; i < g.grids.Length; i++)
		{
			Hex hex = g.grids[i].hex;
			hex.Hide_Fog();
			if (hex.character != null)
			{
				hex.character.tr.gameObject.SetActive(true);
			}
		}
	}
}
