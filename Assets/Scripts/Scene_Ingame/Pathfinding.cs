using System.Collections.Generic;
using UnityEngine;

public class Pathfinding
{
    private List<Transform> hex_PathVisuals;

    public Pathfinding(List<Transform> path_Visuals)
    {
        hex_PathVisuals = new List<Transform>(path_Visuals);
    }

    public void Show_Path(Hex startHex, Hex endHex)
    {
        if(startHex == endHex)
        {
            Hide_Path();
            return;
        }

        List<Hex> path = Get_Path(startHex, endHex);

        if (path == null)
        {
            Hide_Path();
            return;
        }
        path.RemoveAt(0);

        int movePointsLeft = startHex.character.charMovement.movePoints_cur;

        Hex current = startHex;
        Hex next = path[0];

        for (int i = 0; i < path.Count; i++)
        {
            Transform someVisual = Get_DisabledPathVisual();
            someVisual.position = path[i].transform.position + Vector3.up * 0.15f;

            movePointsLeft -= path[i].moveCost;
            if(Utility.EnemyInNeighbors(startHex.character, current) && Utility.EnemyInNeighbors(startHex.character, next))
                movePointsLeft -= Utility.enemyHexValue;

            someVisual.Find("PathValue").GetComponent<TextMesh>().text = "" + movePointsLeft;

            SpriteRenderer rend = someVisual.Find("Image").GetComponent<SpriteRenderer>();
            if (movePointsLeft >= 0)
                rend.color = Color.green;
            else
                rend.color = Color.red;
            
            if (i != path.Count - 1)
            {
                current = path[i];
                next = path[i + 1];
            }
        }
    }

    // will return shortest path from hex to hex
    public List<Hex> Get_Path(Hex startHex, Hex endHex)
    {
        if(startHex == null) return null;
        
        bool pathComplete = false;
        List<Hex> finalPath = new List<Hex>();

        Queue<Hex> groupToVisit = new Queue<Hex>();
        groupToVisit.Enqueue(startHex);

        Dictionary<Hex, int> costSoFar = new Dictionary<Hex, int>();
        costSoFar[startHex] = 0;

        Dictionary<Hex, Hex> cameFrom = new Dictionary<Hex, Hex>();
        cameFrom[startHex] = startHex;

        while (groupToVisit.Count > 0)
        {
            Hex current = groupToVisit.Dequeue();

            foreach (Hex next in current.neighbors)
            {
                if (next != endHex && next.character != null) continue;

                int newCost = costSoFar[current] + next.moveCost;
                if (Utility.EnemyInNeighbors(startHex.character, current) && Utility.EnemyInNeighbors(startHex.character, next))
                    newCost += Utility.enemyHexValue;

                switch (startHex.character.charMovement.moveType)
                {
                    case Utility.char_moveType.ground:
                        if (next.groundMove && (!costSoFar.ContainsKey(next) || newCost < costSoFar[next]))
                        {
                            costSoFar[next] = newCost;
                            cameFrom[next] = current;
                            groupToVisit.Enqueue(next);
                        }
                        break;
                    case Utility.char_moveType.air:
                        if (next.airMove && (!costSoFar.ContainsKey(next) || newCost < costSoFar[next]))
                        {
                            costSoFar[next] = newCost;
                            cameFrom[next] = current;
                            groupToVisit.Enqueue(next);
                        }
                        break;
                }
            }
        }

        if (cameFrom.ContainsKey(endHex)) pathComplete = true;
        if (!pathComplete) return null;

        finalPath.Add(endHex);

        Hex rebuildPoint = cameFrom[endHex];
        while (rebuildPoint != startHex)
        {
            finalPath.Add(rebuildPoint);

            rebuildPoint = cameFrom[rebuildPoint];
        }

        finalPath.Add(startHex);

        finalPath = Utility.Swap_ListItems(finalPath);

        return finalPath;
    }

    // Will return actual path that character can move on this turn
    public List<Hex> Get_RealPath(List<Hex> somePath)
    {
        if (somePath == null) return null;

        List<Hex> realPath = new List<Hex>();

        Character someCharacter = somePath[0].character;

        int movePointsLeft = someCharacter.charMovement.movePoints_cur;
        Hex current = somePath[0];
        Hex next = somePath[1];

        realPath.Add(somePath[0]);
        for (int x = 1; x < somePath.Count; x++)
        {
            movePointsLeft -= somePath[x].moveCost;
            if (Utility.EnemyInNeighbors(someCharacter, current) && Utility.EnemyInNeighbors(someCharacter, next))
                movePointsLeft -= Utility.enemyHexValue;

            if (movePointsLeft >= 0)
                realPath.Add(somePath[x]);

            if (x != somePath.Count - 1)
            {
                current = somePath[x];
                next = somePath[x + 1];
            }
        }

        return realPath;
    }

    public int Get_PathCost_FromStart(List<Hex> somePath)
    {
        if (somePath == null) return 0;

        int cost = 0;

        for (int x = 0; x < somePath.Count; x++)
            cost += somePath[x].moveCost;

        return cost;
    }
    public int Get_PathCost_FromNext(List<Hex> somePath)
    {
        if (somePath == null) return 0;

        int cost = 0;

        for (int x = 1; x < somePath.Count; x++)
            cost += somePath[x].moveCost;

        return cost;
    }
    public int Get_PathCost_Between(List<Hex> somePath)
    {
        if (somePath == null) return 0;
        if (somePath.Count == 2) return 0;

        int cost = 0;

        for (int x = 1; x < somePath.Count - 1; x++)
            cost += somePath[x].moveCost;

        return cost;
    }

    public void Hide_Path()
    {
        for (int x = 0; x < hex_PathVisuals.Count; x++)
            hex_PathVisuals[x].gameObject.SetActive(false);
    }

    private Transform Get_DisabledPathVisual()
    {
        Transform someVisual = null;

        while (someVisual == null)
        {
            Transform visual = hex_PathVisuals[UnityEngine.Random.Range(0, hex_PathVisuals.Count)];
            if (!visual.gameObject.activeInHierarchy)
            {
                someVisual = visual;
                someVisual.gameObject.SetActive(true);
            }
        }

        return someVisual;
    }
}
