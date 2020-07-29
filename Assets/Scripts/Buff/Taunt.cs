using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Taunt : BuffBase
{
    protected override void OnCast()
    {
        GameLogger.AddLog($"{target.name} taunt now");
        target.isTaunt.Value = true;
    }

    protected override void EndBuff()
    {
        target.isTaunt.Value = false;
        GameLogger.AddLog($"{target.name} end taunt");
        base.EndBuff();
    }
}
