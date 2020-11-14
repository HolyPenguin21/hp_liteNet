using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class AttackResult
{
    public int a_coord_x { get; set; }
    public int a_coord_y { get; set; }
    public int a_attackId { get; set; }
    public int t_coord_x { get; set; }
    public int t_coord_y { get; set; }
    public int t_attackId { get; set; }
    public string attackData { get; set; }

    public void AttackData_Calculation(Character attacker, Character target)
    {
        CharVars.char_Attack a_Attack = attacker.charAttacks[a_attackId];
        int a_AttackCount = a_Attack.attackCount;
        int a_Health = attacker.charHp.hp_cur;
        int a_HealthMax = attacker.charHp.hp_max;

        List<CharVars.char_Attack> t_Attacks = target.charAttacks;
        int t_AttackCount = 0;
        if (t_Attacks.Count > t_attackId) t_AttackCount = t_Attacks[t_attackId].attackCount;
        int t_Health = target.charHp.hp_cur;
        int t_HealthMax = attacker.charHp.hp_max;

        while (a_AttackCount > 0 || t_AttackCount > 0)
        {
            if (a_AttackCount > 0)
            {
                int a_buff = 0;
                if (a_Attack.attackBuff != null) a_buff = a_Attack.attackBuff.buffId;

                int dmg = AttackResult_Calculation(attacker, attacker, target);

                a_Health = Buff_HealthModification(a_Health, a_HealthMax, dmg, a_buff);
                if (dmg != -1) t_Health -= dmg;

                string charBuffs = Get_CharacterBuffs(attacker);

                attackData += "a;" + a_Health + ";" + a_buff + ";" + dmg + ";" + t_Health + ";" + charBuffs + "]";

                if (t_Health <= 0) break;

                a_AttackCount--;
            }

            if (t_AttackCount > 0)
            {
                CharVars.char_Attack t_Attack = t_Attacks[t_attackId];
                int t_buff = 0;
                if (t_Attack.attackBuff != null) t_buff = t_Attack.attackBuff.buffId;

                int a_buff = 0;
                if (a_Attack.attackBuff != null) a_buff = a_Attack.attackBuff.buffId;

                int dmg = AttackResult_Calculation(attacker, target, attacker);

                t_Health = Buff_HealthModification(t_Health, t_HealthMax, dmg, t_buff);
                if (dmg != -1) a_Health -= dmg;

                string charBuffs = Get_CharacterBuffs(target);

                attackData += "t;" + t_Health + ";" + t_buff + ";" + dmg + ";" + a_Health + ";" + charBuffs + "]";

                if (a_Health <= 0) break;

                t_AttackCount--;
            }
        }

        attackData = Regex.Replace(attackData, ";]", "]");
        if (attackData[attackData.Length - 1].ToString() == "]") attackData = attackData.Remove(attackData.Length - 1);
    }

    public IEnumerator Implementation()
    {
        Debug.Log(attackData);

        Hex a_Hex = GameMain.inst.gridManager.Get_GridItem_ByCoords(a_coord_x, a_coord_y).hex;
        Character attacker = a_Hex.character;
        List<int> attackerCharBuffs = new List<int>();

        Hex t_Hex = GameMain.inst.gridManager.Get_GridItem_ByCoords(t_coord_x, t_coord_y).hex;
        Character target = t_Hex.character;

        string[] attackResultData = attackData.Split(']');
        for (int x = 0; x < attackResultData.Length; x++)
        {
            //Debug.Log(attackResultData[x]);
            string[] singleAttackData = attackResultData[x].Split(';');
            int a_health = int.Parse(singleAttackData[1]);                                              // [1] attacker health after attack
            ABuff attackBuff = GameMain.inst.aBuffData.Get_ABuff_byId(int.Parse(singleAttackData[2]));  // [2] attacker attack buff
            int dmg = int.Parse(singleAttackData[3]);                                                   // [3] dmg done
            int t_health = int.Parse(singleAttackData[4]);                                              // [4] target health left after attack

            // character buffs
            if (singleAttackData.Length > 5)                                                            // [5+] buffs that are present on attacker
                for (int y = 5; y < singleAttackData.Length; y++)
                    attackerCharBuffs.Add(int.Parse(singleAttackData[y]));

            // attack iteration
            if (singleAttackData[0] == "a")                                                             // [0] attacker identifier
            {
                yield return attacker.AttackAnimation(target.hex, a_attackId);
                if (attackBuff != null && dmg > 0)
                {
                    attacker.Recieve_ABuff_AsAttacker(attackBuff, a_health);
                    target.Recieve_ABuff_AsTarget(attackBuff);
                }
                if (dmg > 0) GameMain.inst.effectsData.Effect_Damage(t_Hex.transform.position, dmg);
                target.Set_Health(t_health);

                if (Utility.IsServer())
                    if (t_health <= 0)
                    {
                        yield return GameMain.inst.Server_Die(target.hex);
                        yield return GameMain.inst.Server_AddExp(attacker.hex, 7);
                        break;
                    }
            }
            else
            {
                yield return target.AttackAnimation(attacker.hex, t_attackId);
                if (attackBuff != null && dmg > 0)
                {
                    target.Recieve_ABuff_AsAttacker(attackBuff, a_health);
                    attacker.Recieve_ABuff_AsTarget(attackBuff);
                }
                if (dmg > 0) GameMain.inst.effectsData.Effect_Damage(a_Hex.transform.position, dmg);
                attacker.Set_Health(t_health);

                if (Utility.IsServer())
                    if (t_health <= 0)
                    {
                        yield return GameMain.inst.Server_Die(attacker.hex);
                        yield return GameMain.inst.Server_AddExp(target.hex, 7);
                        break;
                    }
            }
        }

        if (!Utility.IsServer()) yield break;

        if (attacker.charHp.hp_cur > 0)
        {
            yield return GameMain.inst.Server_AddExp(attacker.hex, 1);

            if (attacker.charExp.exp_cur >= attacker.charExp.exp_max)
                yield return GameMain.inst.Server_LevelUp(attacker);
        }

        if (target.charHp.hp_cur > 0)
        {
            yield return GameMain.inst.Server_AddExp(target.hex, 1);

            if (target.charExp.exp_cur >= target.charExp.exp_max)
                yield return GameMain.inst.Server_LevelUp(target);
        }
    }

    private int AttackResult_Calculation(Character firstStrikeChar, Character attacker, Character target)
    {
        // hit or miss
        int dodge = target.charDef.dodgeChance + target.hex.dodge;
        if (UnityEngine.Random.Range(0, 101) < dodge) return -1;
        else return DmgCalculation(firstStrikeChar, attacker, target);
    }
    
    public int DmgCalculation(Character firstStrikeChar, Character attacker, Character target)
    {
        CharVars.char_Attack a_Attack = attacker.charAttacks[a_attackId];
        CharVars.char_Defence t_Defence = target.charDef;
        ABuff a_ABuff = a_Attack.attackBuff;
        List<Buff> a_charBuffs = attacker.charBuffs;
        List<Buff> t_charBuffs = target.charBuffs;

        int dmg = a_Attack.attackDmg_cur;

        // Attack Buff
        switch (a_ABuff)
        {
            case ABuff_Charge charge:
                if (firstStrikeChar == attacker)
                {
                    dmg += dmg;
                }
                break;
        }

        // Attacker buffs
        for (int x = 0; x < a_charBuffs.Count; x++)
        {
            switch (a_charBuffs[x])
            {
                case Buff_Berserk berserk:
                    Debug.Log("Berserk");
                    break;
            }
        }

        // Target buffs
        for (int x = 0; x < t_charBuffs.Count; x++)
        {
            switch (t_charBuffs[x])
            {
                case Buff_Defensive defensive:
                    if (firstStrikeChar != target)
                        dmg -= dmg / 2;
                    break;
            }
        }

        switch (a_Attack.attackDmgType)
        {
            case CharVars.char_attackDmgType.Blade:
                return Convert.ToInt32(dmg - dmg * t_Defence.blade_resistance);
            case CharVars.char_attackDmgType.Pierce:
                return Convert.ToInt32(dmg - dmg * t_Defence.pierce_resistance);
            case CharVars.char_attackDmgType.Impact:
                return Convert.ToInt32(dmg - dmg * t_Defence.impact_resistance);
            case CharVars.char_attackDmgType.Magic:
                return Convert.ToInt32(dmg - dmg * t_Defence.magic_resistance);
        }

        return -999; // should not get here
    }

    private string Get_CharacterBuffs(Character attacker)
    {
        string charBuffs = "";
        for (int x = 0; x < attacker.charBuffs.Count; x++)
        {
            if (attacker.charBuffs[x].buffType != Utility.buff_Type.onAttack) continue;

            charBuffs += attacker.charBuffs[x].buffId + ";";
        }

        if (charBuffs != "") if (charBuffs[charBuffs.Length - 1].ToString() == ";") charBuffs = charBuffs.Remove(charBuffs.Length - 1);

        return charBuffs;
    }

    private int Buff_HealthModification(int health, int healthMax, int dmg, int a_buff)
    {
        switch (a_buff)
        {
            case 1:
                if (health < healthMax)
                {
                    health += dmg / 2;
                    if (health > healthMax)
                        health = healthMax;
                }
                break;
        }

        return health;
    }
}
