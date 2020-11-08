using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CharVars
{
    public enum char_Type { day, night, neutral };
    public enum char_moveType { ground, air };
    public enum char_attackType { none, Melee, Ranged };
    public enum char_attackDmgType { Blade, Pierce, Impact, Magic };

    [System.Serializable]
    public struct char_Attack
    {
        public char_attackType attackType;
        public char_attackDmgType attackDmgType;
        public int attackCount;
        public int attackDmg_base;
        public int attackDmg_cur;
        public ABuff attackBuff;
    }

    [System.Serializable]
    public struct char_Hp
    {
        public int hp_cur;
        public int hp_max;
        public char_Hp(int hp_max)
        {
            this.hp_max = hp_max;
            hp_cur = this.hp_max;
        }
    }

    [System.Serializable]
    public struct char_Defence
    {
        public int dodgeChance;
        public float blade_resistance;
        public float pierce_resistance;
        public float impact_resistance;
        public float magic_resistance;
    }

    [System.Serializable]
    public struct char_Exp
    {
        public int exp_cur;
        public int exp_max;
        public char_Exp(int exp_max)
        {
            this.exp_max = exp_max;
            exp_cur = 0;
        }
    }

    [System.Serializable]
    public struct char_Move
    {
        public char_moveType moveType;
        public int movePoints_cur;
        public int movePoints_max;
    }
}
