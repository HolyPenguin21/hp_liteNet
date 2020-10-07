﻿using System.Collections;
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

			if(hex.item != null)
				hex.itemObj.SetActive(false);

			if (Utility.IsServer())
			{
				// Hide characters
				if (hex.character != null && hex.character.owner.clientName != s.serverName)
					hex.character.tr.gameObject.SetActive(false);

				for (int x = 0; x < IngameManager.inst.allCharacters.Count; x++)
				{
					Character character = IngameManager.inst.allCharacters[x];
					if (character.owner.clientName == s.serverName &&
					 Vector3.Distance(character.hex.transform.position, hex.transform.position) < Utility.distHexes * (float)character.lookRange)
					{
						hex.Hide_Fog();

						if (hex.character != null)
							hex.character.tr.gameObject.SetActive(true);
						
						if (hex.item != null)
							hex.itemObj.SetActive(true);
					}
				}
			}
			else
			{
				// Hide characters
				if (hex.character != null && hex.character.owner.clientName != c.clientName)
					hex.character.tr.gameObject.SetActive(false);

				for (int x = 0; x < IngameManager.inst.allCharacters.Count; x++)
				{
					Character character = IngameManager.inst.allCharacters[x];
					if (character.owner.clientName == c.clientName && 
					Vector3.Distance(character.hex.transform.position, hex.transform.position) < Utility.distHexes * (float)character.lookRange)
					{
						hex.Hide_Fog();
						if (hex.character != null)
							hex.character.tr.gameObject.SetActive(true);
						
						if (hex.item != null)
							hex.itemObj.SetActive(true);
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
				if(g.grids[i].hex.character != null) continue;
                	hexesInRange.Add(g.grids[i].hex);
            }
		}

		for (int i = 0; i < hexesInRange.Count; i++)
		{
			if(hexesInRange[i].character != null) continue;

			List<Hex> path = IngameManager.inst.pathfinding.Get_Path(character.hex, hexesInRange[i]);

			if(path == null) continue;
			path.RemoveAt(0);
			if(path.Count == 0) continue;

			int pathCost = IngameManager.inst.pathfinding.Get_PathCost_FromStart(path);

			if(pathCost <= range_cur)
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
			if(!moveHexes.Contains(g.grids[i].hex))
				g.grids[i].hex.Show_Fog();
		}

		character.hex.Hide_Fog();
	}
}