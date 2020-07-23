using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class Shield : BuffBase
{
    protected override void OnCast()
    {
        disposable.Add(target.beforeDamageSubject.Subscribe(damageInfo =>
        {
            if(damageInfo.damageType == GameDefine.DamageType.Physical)
            {
                damageInfo.damage = 0;
                GameLogger.AddLog($"{target.name} mute damage, shield dispeared");
                EndBuff();
            }
        }));
    }
}
