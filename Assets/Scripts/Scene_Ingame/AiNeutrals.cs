using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiNeutrals
{
    private GameMain manager;

    public AiNeutrals (GameMain manager)
    {
        this.manager = manager;
    }

    public IEnumerator Ai_Logic()
    {
        // Spawn
        for (int x = 0; x < manager.gridManager.neutralsSpawners.Count; x++)
        {
            Hex spawnPoint = manager.gridManager.neutralsSpawners[x].hex;

            if (spawnPoint.character != null) continue;
            if (spawnPoint.isVillage && spawnPoint.villageOwner.name != "" && spawnPoint.villageOwner.name != "Neutrals") continue;

            int spawnChance = Random.Range(1, 101);
            if (spawnChance < 80) continue;

            int charId = Random.Range(1, 15);
            yield return manager.Server_CreateCharacter(spawnPoint, charId, "Neutrals", false); // Server is blocked
        }

        // Movement / Attack
        for (int i = 0; i < manager.allCharacters.Count; i++)
        {
            if (manager.allCharacters[i].owner.name != "Neutrals") continue;
            Character aiChar = manager.allCharacters[i];

            // if(aiChar.charHp.hp_cur == aiChar.charHp.hp_max) continue;

            yield return Attack_NearbyEnemy(aiChar);
            if(aiChar == null) continue;

            if(Get_Enemys_InRange(aiChar).Count > 0)
                yield return Move_ToRandomEnemyInRange(aiChar);
            else
                yield return Move_Random(aiChar);

            yield return Attack_NearbyEnemy(aiChar);
        }

        yield return null;
    }

    #region Logic
    private IEnumerator Attack_NearbyEnemy(Character character)
    {
        if(character.canAct)
		{
			Hex enemyHex = Get_NearbyEnemyHex(character);
            if(enemyHex != null)
            {
                List<Hex> path = new List<Hex>();
                path.Add(character.hex);
                path.Add(enemyHex);

                int attackId = 0;
                int curMaxDmg = 0;
                for(int x = 0; x < character.charAttacks.Count; x++)
                {
                    int maxDmg = character.charAttacks[x].attackCount * character.charAttacks[x].attackDmg_base;
                    if(maxDmg > curMaxDmg)
                    {
                        curMaxDmg = maxDmg;
                        attackId = x;
                    }
                }

                yield return manager.Server_Attack(path, attackId);
            }
		}

        yield return null;
    }

    private IEnumerator Move_ToRandomEnemyInRange(Character character)
    {
        if(!character.canAct || character.charMovement.movePoints_cur == 0) yield break;

        List<Hex> enemysInRange = Get_Enemys_InRange(character);
        if(enemysInRange.Count == 0) yield break;

        Hex randomEnemy = enemysInRange[Random.Range(0, enemysInRange.Count)];
        List<Hex> path = manager.pathfinding.Get_Path(character.hex, randomEnemy);
        path.RemoveAt(path.Count - 1);
        yield return manager.Server_Move(path);
    }

    private IEnumerator Move_Random(Character character)
    {
        if(!character.canAct || character.charMovement.movePoints_cur == 0) yield break;

        yield return manager.Server_Move(manager.pathfinding.Get_Path(character.hex, Get_RandomNeighborMoveHex(character)));
    }
    #endregion

    private Hex Get_RandomNeighborMoveHex(Character character)
    {
        Hex current = character.hex;
        List<Hex> availableHexes = new List<Hex>();

        for(int x = 0; x < current.neighbors.Count; x++)
        {
            if(current.neighbors[x].groundMove && current.neighbors[x].character == null)
                availableHexes.Add(current.neighbors[x]);
        }

        return availableHexes[Random.Range(0, availableHexes.Count)];
    }

    private List<Hex> Get_Enemys_InRange(Character aiChar)
    {
        if(aiChar == null) return null;
        List<Hex> enemyHexes = new List<Hex>();

        for(int x = 0; x < manager.allCharacters.Count; x++)
        {
            if(manager.allCharacters[x].owner == aiChar.owner) continue;

            Character enemyCharacter = manager.allCharacters[x];
            List<Hex> pathToEnemyCharacter = manager.pathfinding.Get_Path(aiChar.hex, enemyCharacter.hex);
            int pathCost = manager.pathfinding.Get_PathCost_Between(pathToEnemyCharacter);

            if(pathCost > aiChar.charMovement.movePoints_cur) continue;

            enemyHexes.Add(enemyCharacter.hex);
        }

        return enemyHexes;
    }

    private Hex Get_NearbyEnemyHex(Character character)
    {
        List<Hex> hexesWithEnemy = new List<Hex>();
        Hex current = character.hex;

        for (int x = 0; x < current.neighbors.Count; x++)
        {
            if (current.neighbors[x].character != null && character.owner != current.neighbors[x].character.owner)
                hexesWithEnemy.Add(current.neighbors[x]);
        }

        if(hexesWithEnemy.Count > 0)
            return hexesWithEnemy[Random.Range(0, hexesWithEnemy.Count)];
        else
            return null;
    }
}
