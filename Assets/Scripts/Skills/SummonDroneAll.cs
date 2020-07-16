using NPOI.SS.UserModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonDroneAll : SummonDrone
{
    public SummonDroneAll(int id, string skillName, int coolDown, bool selectable, CharacterLogic caster, string description) : base(id, skillName, coolDown, selectable, caster, description)
    {
    }

    public override bool Cast(Vector2Int targetPos, Team team)
    {
        var info = CreateCharacterInfo();
        for(int i = 0; i < 3; i++)
        {
            var pos = new Vector2Int(i, caster.pos.y);
            if(pos != caster.pos)
            {
                GameManager.Instance.AddSummonCharacter(caster.team, pos + new Vector2Int(0, -1), info, aliveTime);
            }
        }
        OnCastSuc();
        return true;
    }
}
