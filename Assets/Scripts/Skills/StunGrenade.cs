using NPOI.SS.UserModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunGrenade : SkillBase
{
    public int hitRate { get; private set; }
    public int continuousTurn { get; private set; }
    public StunGrenade(int id, string skillName, SkillType skillType, int cost, bool selectable, CharacterLogic caster, string description) : base(id, skillName, skillType, cost, selectable, caster, description)
    {
    }

    public override void LoadCustomProperty(ISheet sheet)
    {
        hitRate = sheet.GetRow(GameDefine.SKILL_CUSTOM_PROPERTY_START_ROW).GetCell(1).GetInt();
        continuousTurn = sheet.GetRow(GameDefine.SKILL_CUSTOM_PROPERTY_START_ROW + 1).GetCell(1).GetInt();
    }

    public override bool Cast(Vector2Int targetPos, Team team)
    {
        var targets = GameManager.Instance.GetCharactersInOneRow(targetPos.y, team);
        var rand = Random.Range(0, GameDefine.PERCENTAGE_MAX);
        if (rand <= hitRate)
        {
            foreach (var item in targets)
            {
                Stune buff = new Stune();
                buff.Init(item, caster, GameDefine.BuffTickType.Turn, false, continuousTurn);
                item.AddBuff(buff);
            }
        }
        
        return base.Cast(targetPos, team);
    }
}
