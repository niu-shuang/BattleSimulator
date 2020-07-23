using NPOI.SS.UserModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorruptionPlus : SkillBase
{
    public int defDownRate { get; private set; }
    public int hpDownRate { get; private set; }
    public CorruptionPlus(int id, string skillName, int cost, bool selectable, CharacterLogic caster, string description) : base(id, skillName, cost, selectable, caster, description)
    {
    }

    public override void LoadCustomProperty(ISheet sheet)
    {
        defDownRate = sheet.GetRow(GameDefine.SKILL_CUSTOM_PROPERTY_START_ROW).GetCell(1).GetInt();
        hpDownRate = sheet.GetRow(GameDefine.SKILL_CUSTOM_PROPERTY_START_ROW + 1).GetCell(1).GetInt();
    }

    public override bool Cast(Vector2Int targetPos, Team team)
    {
        var target = GameManager.Instance.GetCharacter(targetPos, team);
        if (target.team != caster.team && target.characterType == GameDefine.CharacterType.Mechanical)
        {
            ModifyDef buff = new ModifyDef(GameDefine.PERCENTAGE_MAX - defDownRate);
            buff.Init(target, caster, GameDefine.BuffTickType.Turn, false, 1);
            target.AddBuff(buff);
            var finalDamage = target.maxHpModifier.finalValue.Value * hpDownRate / GameDefine.PERCENTAGE_MAX;
            if(finalDamage > target.hpModifier.finalValue.Value)
            {
                finalDamage = target.hpModifier.finalValue.Value;
            }
            target.hpModifier.AddValueDirectly(-finalDamage);
            GameLogger.AddLog($"corrupt {target.name} defDown { defDownRate / 10 }% And cause {finalDamage} damage");
            return base.Cast(targetPos, team);
        }
        else
        {
            return false;
        }
    }
}
