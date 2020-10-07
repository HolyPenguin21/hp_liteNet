using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using UnityEditor; // COMMENT

// [ExecuteInEditMode] // COMMENT
public class Builder_Grid : MonoBehaviour
{
    private GridManager manager;
    public GameObject hexVisual_pref;
    public int grid_width;
    public int grid_height;

    private void Awake()
    {
        manager = GetComponent<GridManager>();

        // Must be OFF / COMMENTED at game start
        // CreateGrid(); // COMMENT
        // Assign_HexToGrid(); // COMMENT
        // Assign_Neighbors(); // COMMENT
    }

    private void CreateGrid()
    {
        manager.grids = new GridItem[grid_height * grid_width];

        int current = 0;
        for (int x = 0; x < grid_width; x++)
        {
            for (int y = 0; y < grid_height; y++)
            {
                string coords = x + "," + y;

                float posX = (x + y * Utility.hex_size - y / 2) * 3.0f / 2.0f * Utility.hex_width;
                float posY = y * Utility.hex_height / 2;

                // Grid visual
                Vector3 worldPos = new Vector3(posX, 0, posY);
                GameObject hexVisual = Instantiate(hexVisual_pref, worldPos, Quaternion.identity, transform);
                hexVisual.name = coords;
                hexVisual.transform.Find("grid_Pos_Text").GetComponent<TextMesh>().text = coords;

                manager.grids[current] = new GridItem(worldPos, x, y);

                current++;
            }
        }
    }
    // Called from Hex script, to position the hex
    public Vector3 Get_ClosestGridPos(Hex hex)
    {
        float curDist = 10000f;
        Vector3 closestPos = Vector3.zero;
        for (int x = 0; x < manager.grids.Length; x++)
        {
            float dist = Vector3.Distance(hex.transform.position, manager.grids[x].worldPos);
            if (dist < curDist)
            {
                curDist = dist;
                closestPos = manager.grids[x].worldPos;
            }
        }

        return closestPos;
    }

    // After grid and hexes are placed
    private void Assign_HexToGrid()
    {
        for (int x = 0; x < manager.grids.Length; x++)
        {
            manager.grids[x].hex = Get_ClosestHex(manager.grids[x].worldPos);
            manager.grids[x].hex.gameObject.name = manager.grids[x].coord_x + "," + manager.grids[x].coord_y;

            if (manager.grids[x].hex.isStartPoint)
                manager.startPoints.Add(manager.grids[x]);

            if (manager.grids[x].hex.isVillage)
                manager.villages.Add(manager.grids[x]);
            
            if (manager.grids[x].hex.neutralsSpawner)
                manager.neutralsSpawners.Add(manager.grids[x]);
        }
        Debug.Log("Builder > Hexes assigned to grid");
    }
    
    private Hex Get_ClosestHex(Vector3 pos)
    {
        GameObject[] hexes = GameObject.FindGameObjectsWithTag("Hex");

        Hex closestHex = null;
        float curDist = 10000f;

        for (int x = 0; x < hexes.Length; x++)
        {
            float dist = Vector3.Distance(hexes[x].transform.position, pos);
            if (dist < curDist)
            {
                curDist = dist;
                closestHex = hexes[x].GetComponent<Hex>();
            }
        }

        return closestHex;
    }

    private void Assign_Neighbors()
    {
        GameObject[] hexes = GameObject.FindGameObjectsWithTag("Hex");

        for (int x = 0; x < manager.grids.Length; x++)
        {
            manager.grids[x].hex.neighbors.Clear();
            manager.grids[x].hex.neighbors = new List<Hex>();

            for (int y = 0; y < manager.grids.Length; y++)
            {
                if (manager.grids[y].hex == manager.grids[x].hex) continue;

                float dist = Vector3.Distance(manager.grids[x].hex.transform.position, manager.grids[y].hex.transform.position);
                if (dist < Utility.distHexes)
                {
                    manager.grids[x].hex.neighbors.Add(manager.grids[y].hex);
                }
            }

            // EditorUtility.SetDirty(manager.grids[x].hex); // COMMENT
        }
        Debug.Log("Builder > Neighbors are setted up");
    }
}
