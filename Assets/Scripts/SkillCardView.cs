using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class SkillCardView : MonoBehaviour
{
    [SerializeField]
    private Text cost;
    [SerializeField]
    private Image casterIcon;
    [SerializeField]
    private Text description;
    private SkillBase skill;

    public void SetData(SkillBase skillLogic)
    {
        if(skillLogic.casted)
        {
            SetEmpty();
            return;
        }
        cost.text = $"Cost:{skillLogic.cost}";
        casterIcon.sprite = GameManager.Instance.characterIcons[skillLogic.caster];
        description.text = skillLogic.description;
        skill = skillLogic;
        skill.SetView(this);
    }

    public void OnClick()
    {
        if(skill != null && skill.canCast)
        {
            GameManager.Instance.OnClickSkill(skill);
        } 
    }

    public void SetEmpty()
    {
        cost.text = "";
        description.text = "";
        casterIcon.sprite = null;
        skill = null;
    }
}
