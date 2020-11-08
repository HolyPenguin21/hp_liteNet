using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fog
{
	private Server s;
	private Client c;
	private GridManager g;

	public Fog(GridManager gridManager, Server server, Client client)
	{
		s = server;
		c = client;
		g = gridManager;
	}

	public void Update_Fog()
	{
		for (int i = 0; i < g.grids.Length; i++)
		{
			Hex hex = g.grids[i].hex;
			hex.Show_Fog();

			if (Utility.IsServer())
			{
				for (int x = 0; x < GameMain.inst.allCharacters.Count; x++)
				{
					Character character = GameMain.inst.allCharacters[x];
					if (character.owner.name == s.player.name &&
					Vector3.Distance(character.hex.transform.position, hex.transform.position) < Utility.distHexes * (float)character.charMovement.movePoints_max)
					{
						hex.Hide_Fog();
					}
				}
			}
			else
			{
				for (int x = 0; x < GameMain.inst.allCharacters.Count; x++)
				{
					Character character = GameMain.inst.allCharacters[x];
					if (character.owner.name == c.player.name &&
					Vector3.Distance(character.hex.transform.position, hex.transform.position) < Utility.distHexes * (float)character.charMovement.movePoints_max)
					{
						hex.Hide_Fog();
					}
				}
			}
		}
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

	public void Show_MoveHexes(Character character)
	{
		List<Hex> hexesInRange = new List<Hex>();
		List<Hex> moveHexes = new List<Hex>();

		int range_max = character.charMovement.movePoints_max;
		int range_cur = character.charMovement.movePoints_cur;

		for (int i = 0; i < g.grids.Length; i++)
		{
			float dist = Vector3.Distance(g.grids[i].hex.transform.position, character.hex.transform.position);
			if (dist < Utility.distHexes * range_max)
			{
				if (g.grids[i].hex.character != null) continue;

				hexesInRange.Add(g.grids[i].hex);
			}
		}

		for (int i = 0; i < hexesInRange.Count; i++)
		{
			if (hexesInRange[i].character != null) continue;

			List<Hex> path = GameMain.inst.pathfinding.Get_Path(character.hex, hexesInRange[i]);

			if (path == null) continue;
			path.RemoveAt(0);
			if (path.Count == 0) continue;

			int pathCost = GameMain.inst.pathfinding.Get_PathCost_FromStart(path);

			if (pathCost <= range_cur)
				moveHexes.Add(hexesInRange[i]);

			// string debug = hexesInRange[i].gameObject.name + " c: " + pathCost + " : ";
			// for(int x = 0; x < path.Count; x ++)
			// {
			// 	debug += path[x].gameObject.name + ", ";
			// }
			// Debug.Log(debug);
		}

		// Debug.Log(hexesInRange.Count + " - " + moveHexes.Count);

		for (int i = 0; i < g.grids.Length; i++)
		{
			if (!moveHexes.Contains(g.grids[i].hex))
				g.grids[i].hex.Show_Fog();
		}

		character.hex.Hide_Fog();
	}
}
