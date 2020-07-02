using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDefine
{
    public static float[] ATKMap = { 1.2f, 1f, .8f, .6f };
    public static float GetDefPercentage(int def) => def * .06f / (def * .06f + 1);

    public static int MANA_PER_TURN = 3;

    public static int DECK_NUM = 5;
}
