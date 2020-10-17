using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Utility
{
    public static float hex_size = 0.5f;
    public static float hex_width = 2 * hex_size;
    public static float hex_height = Mathf.Sqrt(3) * hex_size;

    public static float distHexes = 0.867f;
    public static int enemyHexValue = 3; // move cost near enemy character

    public static int villageHeal = 5;
    public static int villageIncome = 2;

    public enum dayTime { dawn, day1, day2, evening, night1, night2 };
    public enum char_moveType { ground, air };
    public enum char_attackType { none, melee, ranged };
    public enum char_attackDmgType { slash, pierce, blunt, magic };
    public enum char_Type { day, night, neutral };

    public enum buff_Type { onEquip, onRemove, onTurn};
    public enum spell_Area { single, circle, cone};

    public struct GridCoord
    {
        public int coord_x;
        public int coord_y;
    }

    [System.Serializable]
    public struct InstanceReady
    {
        public bool ready;
        public int taskId;
    }

    [System.Serializable]
    public struct char_Attack
    {
        public char_attackType attackType;
        public char_attackDmgType attackDmgType;
        public int attackCount;
        public int attackDmg_base;
        public int attackDmg_cur;
    }

    [System.Serializable]
    public struct char_Defence
    {
        public int dodgeChance;
        public float slash_resistance;
        public float pierce_resistance;
        public float blunt_resistance;
        public float magic_resistance;
    }

    [System.Serializable]
    public struct char_Hp
    {
        public int hp_cur;
        public int hp_max;
    }

    [System.Serializable]
    public struct char_Exp
    {
        public int exp_cur;
        public int exp_max;
    }

    [System.Serializable]
    public struct char_Move
    {
        public char_moveType moveType;
        public int movePoints_cur;
        public int movePoints_max;
    }

    public static bool IsServer()
    {
        if (GameMain.inst.server != null)
            return true;
        return false;
    }

    public static void Set_OwnerColor(Transform tr, Player owner)
    {
        if (IsServer())
        {
            if (owner == GameMain.inst.server.players[0])
                tr.Find("playerColor").GetComponent<SpriteRenderer>().color = Color.blue;

            if (owner == GameMain.inst.server.players[1])
                tr.Find("playerColor").GetComponent<SpriteRenderer>().color = Color.red;
        }
        else
        {
            if (owner == GameMain.inst.client.players[0])
                tr.Find("playerColor").GetComponent<SpriteRenderer>().color = Color.blue;

            if (owner == GameMain.inst.client.players[1])
                tr.Find("playerColor").GetComponent<SpriteRenderer>().color = Color.red;
        }
    }

    public static bool EnemyInNeighbors(Character character, Hex current)
    {
        for (int x = 0; x < current.neighbors.Count; x++)
        {
            if (current.neighbors[x].character != null && current.neighbors[x].character.tr.gameObject.activeInHierarchy && character.owner != current.neighbors[x].character.owner)
                return true;
        }
        return false;
    }

    public static bool HexIsVisible(Hex hex)
    {
        if (hex.fog.activeInHierarchy)
            return false;

        return true;
    }
    public static bool CharacterIsVisible(Character character)
    {
        if (character.tr.gameObject.activeInHierarchy)
            return true;

        return false;
    }

    public static bool IsMyCharacter(Character someCharacter)
    {
        if (IsServer())
        {
            if (someCharacter.owner.name == GameMain.inst.server.player.name)
                return true;
        }
        else
        {
            if (someCharacter.owner.name == GameMain.inst.client.player.name)
                return true;
        }
        return false;
    }

    public static bool IsEmeny(Character char1, Character char2)
    {
        if (char1.owner != char2.owner)
        {
            return true;
        }
        return false;
    }

    public static bool IsMyTurn()
    {
        if (IsServer())
        {
            if (GameMain.inst.currentTurn.name != GameMain.inst.server.player.name)
                return false;
        }
        else
        {
            if (GameMain.inst.currentTurn.name != GameMain.inst.client.player.name)
                return false;
        }

        return true;
    }

    public static bool InAttackRange(Hex hex_a, Hex hex_b)
    {
        float dist = Vector3.Distance(hex_a.transform.position, hex_b.transform.position);

        if (dist > distHexes) return false;

        return true;
    }

    public static List<T> Swap_ListItems<T>(List<T> initialList)
    {
        int listHalf = Convert.ToInt32(initialList.Count / 2);

        for (int x = 0; x < listHalf; x++)
        {
            T tempItem = initialList[x];
            initialList[x] = initialList[initialList.Count - 1 - x];
            initialList[initialList.Count - 1 - x] = tempItem;
        }

        return initialList;
    }

    public static Player Get_Client_byString(string someName, List<Player> initialList)
    {
        return initialList.Find(x => x.name == someName);
    }

    public static bool AreAllPlayersAvailable()
    {
        for (int x = 0; x < GameMain.inst.server.players.Count; x++)
        {
            Player somePlayer = GameMain.inst.server.players[x];
            if (somePlayer.isServer || somePlayer.isNeutral) continue;

            if (!somePlayer.isAvailable) return false;
        }

        return true;
    }

    //public static void LoadScene_byString(string sceneName)
    //{
    //    SceneManager.LoadScene(sceneName);
    //}
}
