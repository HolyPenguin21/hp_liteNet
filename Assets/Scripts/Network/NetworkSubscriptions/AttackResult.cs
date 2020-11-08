using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class AttackResult
{
    public int a_coord_x { get; set; }
    public int a_coord_y { get; set; }
    public int t_coord_x { get; set; }
    public int t_coord_y { get; set; }
    public int attackId { get; set; }
    public string attackData { get; set; }

    public void AttackData_Calculation(Character attacker, Character target, int attackId)
    {
        Utility.char_Attack a_Attack = attacker.charAttacks[attackId];
        int a_AttackCount = a_Attack.attackCount;
        int a_Health = attacker.charHp.hp_cur;
        int a_HealthMax = attacker.charHp.hp_max;

        List<Utility.char_Attack> t_Attacks = target.charAttacks;
        int t_AttackCount = 0;
        if (t_Attacks.Count > attackId) t_AttackCount = t_Attacks[attackId].attackCount;
        int t_Health = target.charHp.hp_cur;
        int t_HealthMax = attacker.charHp.hp_max;

        while (a_AttackCount > 0 || t_AttackCount > 0)
        {
            if (a_AttackCount > 0)
            {
                int a_buff = 0;
                if (a_Attack.attackBuff != null) a_buff = a_Attack.attackBuff.buffId;

                int dmg = Dmg_Calculation(a_buff, a_Attack, target.charDef, target.hex);

                a_Health = Buff_HealthModification(a_Health, a_HealthMax, dmg, a_buff);
                if (dmg != -1) t_Health -= dmg;

                string charBuffs = Get_CharacterBuffs(attacker);

                attackData += "a;" + a_Health + ";" + a_buff + ";" + dmg + ";" + t_Health + ";" + charBuffs + "]";

                if (t_Health <= 0) break;

                a_AttackCount--;
            }

            if (t_AttackCount > 0)
            {
                Utility.char_Attack t_Attack = t_Attacks[attackId];
                int t_buff = 0;
                if (t_Attack.attackBuff != null) t_buff = t_Attack.attackBuff.buffId;

                int a_buff = 0;
                if (a_Attack.attackBuff != null) a_buff = a_Attack.attackBuff.buffId;

                int dmg = Dmg_Calculation(t_buff, t_Attack, attacker.charDef, attacker.hex);
                //if (a_buff == 3) dmg = dmg * 2;  // Charge Buff - remake this

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

        Character attacker = GameMain.inst.gridManager.Get_GridItem_ByCoords(a_coord_x, a_coord_y).hex.character;
        List<int> attackerCharBuffs = new List<int>();
        Character target = GameMain.inst.gridManager.Get_GridItem_ByCoords(t_coord_x, t_coord_y).hex.character;

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
                yield return attacker.AttackAnimation(target.hex, attackId);                
                if (attackBuff != null && dmg > 0)
                {
                    attacker.Recieve_ABuff_AsAttacker(attackBuff, a_health);
                    target.Recieve_ABuff_AsTarget(attackBuff);
                }
                target.RecieveDmg(dmg, t_health);

                if (Utility.IsServer())
                    if (t_health <= 0)
                    {
                        yield return GameMain.inst.Server_Die(target.hex);
                        yield return GameMain.inst.Server_AddExp(attacker.hex, 3);
                        break;
                    }
            }
            else
            {
                yield return target.AttackAnimation(attacker.hex, attackId);
                if (attackBuff != null && dmg > 0)
                {
                    target.Recieve_ABuff_AsAttacker(attackBuff, a_health);
                    attacker.Recieve_ABuff_AsTarget(attackBuff);
                }                
                attacker.RecieveDmg(dmg, t_health);

                if (Utility.IsServer())
                    if (t_health <= 0)
                    {
                        yield return GameMain.inst.Server_Die(attacker.hex);
                        yield return GameMain.inst.Server_AddExp(target.hex, 3);
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

    private int Dmg_Calculation(int a_buff, Utility.char_Attack attack, Utility.char_Defence defence, Hex targetHex)
    {
        int dmg = attack.attackDmg_cur;
        //if (a_buff == 3) dmg = dmg * 2;  // Charge Buff - remake this

        // hit or miss
        int dodge = defence.dodgeChance + targetHex.dodge;
        if (UnityEngine.Random.Range(0, 101) < dodge) return -1;

        switch (attack.attackDmgType)
        {
            case Utility.char_attackDmgType.Blade:
                return Convert.ToInt32(dmg - dmg * defence.blade_resistance);
            case Utility.char_attackDmgType.Pierce:
                return Convert.ToInt32(dmg - dmg * defence.pierce_resistance);
            case Utility.char_attackDmgType.Impact:
                return Convert.ToInt32(dmg - dmg * defence.impact_resistance);
            case Utility.char_attackDmgType.Magic:
                return Convert.ToInt32(dmg - dmg * defence.magic_resistance);
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

        if(charBuffs != "") if (charBuffs[charBuffs.Length - 1].ToString() == ";") charBuffs = charBuffs.Remove(charBuffs.Length - 1);

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
