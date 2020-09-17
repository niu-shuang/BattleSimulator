using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GameDefine
{
    public static float[] ATKMap = { 1f, 1f, 1f, 1f };
    //public static float GetDefPercentage(int def) => def * .06f / (def * .06f + 1);

    public static float GetAttackFix(int atk, int def) => DamageCoefficient1 * atk / (atk + DamageCoefficient2 * def);

    public static int DECK_INIT_NUM = 5;
    public static int DRAW_CARD_NUM = 3;
    public static int DECK_MAX_NUM = 10;

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


    public static int MAX_MANA_LIMIT = 15;
    public static List<int> MaxManaTable = new List<int>() { 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
    public static int MAX_MANA_PER_TURN(int turn)
    {
        int value = 0;
        if (MaxManaTable.Count >= turn)
        {
            value = MaxManaTable[turn - 1];
        }
        else
            value = MAX_MANA_LIMIT;
        
        return value;
    }

    public static int RECOVER_MANA_LIMIT = 12;
    public static List<int> RecoverManaTable = new List<int>() { 3, 4, 5, 6, 7, 8, 9, 10, 11, 12};
    public static int RECOVER_MANA_PER_TURN(int turn)
    {
        int value = 0;
        if (RecoverManaTable.Count >= turn)
            value = RecoverManaTable[turn - 1];
        else
            value = RECOVER_MANA_LIMIT;
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
