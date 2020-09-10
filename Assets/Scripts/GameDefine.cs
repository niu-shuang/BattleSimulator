using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GameDefine
{
    public static float[] ATKMap = { 1f, 1f, 1f, 1f };
    //public static float GetDefPercentage(int def) => def * .06f / (def * .06f + 1);

    public static float GetAttackFix(int atk, int def) => DamageCoefficient1 * atk / (atk + DamageCoefficient2 * def);

    public static int CARD_GENERATE_NUM = 10;

    public static int SKILL_CUSTOM_PROPERTY_START_ROW = 7;

    public static int DamageCoefficient1 = 300;
    public static int DamageCoefficient2 = 4;

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
        Mechanical,
        Android
    }

    public enum DamageType
    {
        Physical,
        Magical,
    }

    public static int INIT_MAX_MANA = 6;
    public static int MAX_MANA_PER_TURN(int turn)
    {
        int value = INIT_MAX_MANA + turn - 1;
        if (value > 15) value = 15;
        return value;
    }

    public static int INIT_RECOVER_MANA = 3;
    public static int RECOVER_MANA_PER_TURN(int turn)
    {
        int value = INIT_RECOVER_MANA + turn - 1;
        if (value > 12) value = 12;
        return value;
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

    public static string GetName(this GameDefine.CharacterType type)
    {
        string value = string.Empty;
        switch (type)
        {
            case GameDefine.CharacterType.Biological:
                value = "バイオ";
                break;
            case GameDefine.CharacterType.Mechanical:
                value = "機械";
                break;
            case GameDefine.CharacterType.Android:
                value = "アンドロイド";
                break;
            default:
                break;
        }
        return value;
    }
}
