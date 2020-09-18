using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class SmallDrone : SummonedCharacter
{
    public SmallDrone(int characterId, Vector2Int pos, string name, int maxHp, Team team, int atk, int def, GameDefine.CharacterType characterType, int dodgeRate, int aliveTime) : base(characterId, pos, name, maxHp, team, atk, def, characterType, dodgeRate, aliveTime)
    {
        disposable.Add(GameManager.Instance.turnEndSubject.Subscribe(turn =>
        {
            if (isStun) return;
            if (!isDead.Value)
            {
                for(int i = 0; i < 3; i++)
                {
                    AutoAttack();
                }
            }
        }));
    }
}
