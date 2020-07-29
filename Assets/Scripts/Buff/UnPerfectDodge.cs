﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnPerfectDodge : BuffBase
{
    protected override void OnBeforeDamage(DamageInfo info)
    {
        var rand = Random.Range(0, GameDefine.PERCENTAGE_MAX);
        if(rand >= 500)
        {
            GameLogger.AddLog($"{target.name} muted damage");
            info.damage = 0;
        }
    }
}
