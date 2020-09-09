using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : CharacterLogic
{
    public EnemyBase(int characterId, Vector2Int pos, string name, int maxHp, Team team, int atk, int def, GameDefine.CharacterType characterType, int dodgeRate) : base(characterId, pos, name, maxHp, team, atk, def, characterType, dodgeRate)
    {
    }
}
