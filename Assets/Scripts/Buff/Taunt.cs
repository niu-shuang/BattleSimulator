using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Taunt : BuffBase
{
    protected override void OnCast()
    {
        target.isTaunt.Value = true;
    }

    protected override void EndBuff()
    {
        target.isTaunt.Value = false;
        base.EndBuff();
    }
}
