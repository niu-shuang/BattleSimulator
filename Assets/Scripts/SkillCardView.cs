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
    [SerializeField]
    private Text cardName;
    private SkillBase skill;
    private bool canClick;

    public void SetData(SkillBase skillLogic, bool canClick)
    {
        if(skillLogic.casted)
        {
            SetEmpty();
            return;
        }
        this.canClick = canClick;
        gameObject.SetActive(true);
        cardName.text = skillLogic.skillName;
        cost.text = $"Cost:{skillLogic.cost}";
        casterIcon.sprite = GameManager.Instance.characterIcons[skillLogic.caster];
        description.text = skillLogic.description;
        skill = skillLogic;
        skill.SetView(this);
    }

    public void OnClick()
    {
        if(skill != null && skill.canCast && canClick)
        {
            GameManager.Instance.OnClickSkill(skill);
        } 
    }

    public void SetEmpty()
    {
        if(gameObject != null)
            gameObject.SetActive(false);
    }
}
