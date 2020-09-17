using NPOI.SS.UserModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class Regeneration : SkillBase
{
    public Regeneration(int id, string skillName, SkillType skillType, int cost, bool selectable, CharacterLogic caster, string description) : base(id, skillName, skillType, cost, selectable, caster, description)
    {
    }

    public override void LoadCustomProperty(ISheet sheet)
    {
        
    }

    public override void OnAddToDeck()
    {
        Debug.Log("on add to deck");
        DoHeal();
        disposable.Add(GameManager.Instance.turnBeginSubject
            .Subscribe(turn => DoHeal()));
    }

    private void DoHeal()
    {
        var targets = GameManager.Instance.GetCharacters(caster.team);
        foreach (var target in targets)
        {
            var healInfo = new HealInfo(caster, target, target.Hp.Value * 50 / GameDefine.PERCENTAGE_MAX);
            healInfo.DoHeal();
        }
    }

    public override void OnRemovedFromDeck()
    {
        disposable.Dispose();
        Debug.Log("on remove from deck");
    }

}
