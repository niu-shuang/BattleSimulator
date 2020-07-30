using NPOI.SS.UserModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Threaten : SkillBase
{
    public int aliveTime { get; private set; }
    public Threaten(int id, string skillName, SkillType skillType, int cost, bool selectable, CharacterLogic caster, string description) : base(id, skillName, skillType, cost, selectable, caster, description)
    {
    }

    public override void LoadCustomProperty(ISheet sheet)
    {
        aliveTime = sheet.GetRow(GameDefine.SKILL_CUSTOM_PROPERTY_START_ROW).GetCell(1).GetInt();
    }

    public override bool Cast(Vector2Int targetPos, Team team)
    {
        var charaList = GameManager.Instance.GetCharacters(caster.team.GetOpposite());
        List<CharacterLogic> targetList = new List<CharacterLogic>();
        foreach (var item in charaList)
        {
            if(item.characterType == GameDefine.CharacterType.Biological)
            {
                targetList.Add(item);
            }
        }
        if(targetList.Count > 0)
        {
            var rand = Random.Range(0, targetList.Count);
            var target = targetList[rand];
            ModifyDef buff = new ModifyDef(GameDefine.PERCENTAGE_MAX - 500);
            buff.Init(target, caster, GameDefine.BuffTickType.Turn, false, aliveTime);
            target.AddBuff(buff);
            return base.Cast(targetPos, team);
        }
        else
        {
            return false;
        }   
    }
}
