using NPOI.SS.UserModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashAttack : SkillBase
{
    public FlashAttack(int id, string skillName, int cost, bool selectable, CharacterLogic caster, string description) : base(id, skillName, cost, selectable, caster, description)
    {
    }

    public override void LoadCustomProperty(ISheet sheet)
    {
        
    }
}
