using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GameDefine
{
    public static float[] ATKMap = { 1.2f, 1f, .8f, .6f };
    //public static float GetDefPercentage(int def) => def * .06f / (def * .06f + 1);

    public static float GetAttackFix(int atk, int def) => 300 * atk / (atk + 4 * def);

    public static int DEFAULT_MANA_PER_TURN = 4;

    public static int CARD_GENERATE_NUM = 5;

    public static int SKILL_CUSTOM_PROPERTY_START_ROW = 7;
    
    /// <summary>
    /// 千分之单位的100%
    /// </summary>
    public static int PERCENTAGE_MAX = 1000;

    public enum BuffTickType
    {
        Turn,
        Attack,
        Damage,
    }

    public enum CharacterType
    {
        Biological,
        Mechanical
    }

    public enum DamageType
    {
        Physical,
        Magical,
    }
}

public static partial class BattleExtensionMethods
{
    public static Team GetOpposite(this Team origin)
    {
        if (origin == Team.Team1)
            return Team.Team2;
        else
            return Team.Team1;
    }
}
