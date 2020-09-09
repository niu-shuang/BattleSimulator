using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class Decoy : SummonedCharacter
{
    public Decoy(int characterId, Vector2Int pos, string name, int maxHp, Team team, int atk, int def, GameDefine.CharacterType characterType, int dodgeRate, int aliveTime) : base(characterId, pos, name, maxHp, team, atk, def, characterType, dodgeRate, aliveTime)
    {
        disposable.Add(GameManager.Instance.turnEndSubject.Subscribe(turn =>
        {
            if (!isDead.Value)
            {
                AutoAttack();
            }
        }));
        Taunt buff = new Taunt();
        buff.Init(this, this, GameDefine.BuffTickType.Turn, true, -1);
        AddBuff(buff);
    }    
}
