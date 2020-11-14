using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SaveLoad
{
    public static void Save()
    {
        Save_Data sData = new Save_Data();
        sData.hexesData = Get_HexesData();
        sData.gameData = Get_GameData();
        
        string jsonData = JsonUtility.ToJson(sData);

        PlayerPrefs.SetString("gameSave", jsonData);
        PlayerPrefs.Save();
    }
    
    private static Save_HexesData Get_HexesData()
    {
        GridItem[] grids = GameMain.inst.gridManager.grids;
        Save_HexesData hexesData = new Save_HexesData();
        for(int x = 0; x < grids.Length; x ++)
        {
            // Debug.Log(grids[x].coord_x + " " + grids[x].coord_y);
            Save_Hex saveHex = new Save_Hex();

            saveHex.gridPos_x = grids[x].coord_x;
            saveHex.gridPos_y = grids[x].coord_y;
            
            // Character
            if(grids[x].hex.character != null)
            {
                Character character = grids[x].hex.character;
                // ID
                saveHex.charId = character.charId;
                // Owner
                if(character.owner.name == "Neutrals")
                {
                    saveHex.charOwnerId = 0;
                }
                else if(character.owner == GameMain.inst.server.players[0])
                {
                    saveHex.charOwnerId = 1;
                }
                else if(character.owner == GameMain.inst.server.players[1])
                {
                    saveHex.charOwnerId = 2;
                }
                // Is hero character
                saveHex.charIsHero = character.heroCharacter;

                // Character Item
                saveHex.charItemId = 0;
                if(character.charItem != null)
                    saveHex.charItemId = character.charItem.itemId;
                
                // Character vars
                saveHex.charCanAct = character.canAct;
                saveHex.charHp_cur = character.charHp.hp_cur;
                saveHex.charMp_cur = character.charMovement.movePoints_cur;
                saveHex.charExp_cur = character.charExp.exp_cur;
            }
            else
                saveHex.charId = 0;
            
            // Village
            if (grids[x].hex.villageOwner != null)
            {
                if(grids[x].hex.villageOwner.name == "Neutrals")
                {
                    saveHex.villageOwnerId = 0;
                }
                else if(grids[x].hex.villageOwner == GameMain.inst.server.players[0])
                {
                    saveHex.villageOwnerId = 1;
                }
                else if (grids[x].hex.villageOwner == GameMain.inst.server.players[1])
                {
                    saveHex.villageOwnerId = 2;
                }
            }

            // Hex Item
            saveHex.hexItemId = 0;
            if (grids[x].hex.item != null)
            {
                saveHex.hexItemId = grids[x].hex.item.itemId;
            }

            hexesData.hexes.Add(saveHex);
        }

        return hexesData;
    }

    private static Save_GameData Get_GameData()
    {
        Save_GameData gameData = new Save_GameData();

        gameData.currentTurnId = GameMain.inst.server.players.FindIndex((Player x) => x.name == GameMain.inst.currentTurn.name);

        List<Player> clients = GameMain.inst.server.players;
        for(int x = 0; x < clients.Count; x++)
        {
            Save_Client client = new Save_Client();
            client.race = clients[x].race;
            client.gold = clients[x].gold;

            gameData.clients.Add(client);
        }

        return gameData;
    }

    public static IEnumerator Load()
    {
        string jsonData = PlayerPrefs.GetString("gameSave");
        Save_Data save_Data = JsonUtility.FromJson<Save_Data>(jsonData);

        yield return Load_HexData(save_Data);
        yield return Load_GameData(save_Data);

        yield return null;
    }

    private static IEnumerator Load_HexData(Save_Data save_Data)
    {
        for(int x = 0; x < save_Data.hexesData.hexes.Count; x++)
        {
            Save_Hex someHexData = save_Data.hexesData.hexes[x];
            Hex hex = GameMain.inst.gridManager.Get_GridItem_ByCoords(someHexData.gridPos_x, someHexData.gridPos_y).hex;

            // Hex Characters
            if(someHexData.charId != 0)
            {
                // Create character
                int charId = someHexData.charId;
		        string ownerName = "Neutrals";
                if(someHexData.charOwnerId == 1)
                    ownerName = GameMain.inst.server.players[0].name;
                else if(someHexData.charOwnerId == 2)
                    ownerName = GameMain.inst.server.players[1].name;
                bool isHero = someHexData.charIsHero;
                
                //yield return GameMain.inst.Server_CreateCharacter(hex, charId, ownerName, isHero);
		        yield return new WaitUntil(() => GameMain.inst.server.player.isAvailable);

                // Character Items
                if(someHexData.charItemId != 0)
                {
                    int itemId = someHexData.charItemId;

                    //yield return GameMain.inst.Server_CreateItem(hex, itemId); // Server is blocked
                    //yield return GameMain.inst.Server_PickupItem(hex.character); // Server is blocked
                }

                // Set character vars
                bool canAct = someHexData.charCanAct;
                int hp = someHexData.charHp_cur;
                int mp = someHexData.charMp_cur;
                int exp = someHexData.charExp_cur;

                //yield return GameMain.inst.Server_Character_SetVars(hex, canAct, hp, mp, exp); // Server is blocked
            }

            // Hex Items
            if(someHexData.hexItemId != 0)
            {
                int itemId = someHexData.hexItemId;
		        
                //yield return GameMain.inst.Server_CreateItem(hex, itemId); // Server is blocked
            }

            // Villages
            if(hex.isVillage)
            {
                Player villageOwner = null;
                if(someHexData.villageOwnerId == 0)
                {
                    villageOwner = Utility.Get_Client_byString("Neutrals");
                }
                else if(someHexData.villageOwnerId == 1)
                    villageOwner = GameMain.inst.server.players[0];
                else if(someHexData.villageOwnerId == 2)
                    villageOwner = GameMain.inst.server.players[1];

                //yield return GameMain.inst.Server_Village_SetOwner(hex, villageOwner);
            }
        }
        yield return null;
    }

    private static IEnumerator Load_GameData(Save_Data save_Data)
    {
        for(int x = 0; x < GameMain.inst.server.players.Count; x++)
        {
            GameMain.inst.server.players[x].race = save_Data.gameData.clients[x].race;
            GameMain.inst.server.players[x].gold = save_Data.gameData.clients[x].gold;
        }

        GameMain.inst.currentTurn = GameMain.inst.server.players[save_Data.gameData.currentTurnId];
        //yield return GameMain.inst.Server_SetCurTurn(); // Server is blocked

        yield return null;
    }
}

[System.Serializable]
public class Save_Data
{
    public Save_HexesData hexesData;
    public Save_GameData gameData;
}

[System.Serializable]
public class Save_HexesData
{
    public List<Save_Hex> hexes = new List<Save_Hex>();
}

[System.Serializable]
public class Save_GameData
{
    public int currentTurnId;
    public List<Save_Client> clients = new List<Save_Client>();
}

[System.Serializable]
public class Save_Client
{
    public int race;
    public int gold;
}
[System.Serializable]
public class Save_Hex
{
    public int gridPos_x;
    public int gridPos_y;

    public int charId;
    public int charOwnerId;
    public bool charIsHero;
    public int charItemId;
    public bool charCanAct;
    public int charHp_cur;
    public int charMp_cur;
    public int charExp_cur;

    public int villageOwnerId;
    public int hexItemId;
}


