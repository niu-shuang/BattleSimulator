using Sirenix.Utilities;
using System.Collections;
using System.Collections.Generic;
using UIAnimation;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class CharacterPanel : MonoBehaviour
{
    [SerializeField]
    private Text characterName;
    [SerializeField]
    private Text characterTeam;
    [SerializeField]
    private Text characterHp;
    [SerializeField]
    private Text characterAtk;
    [SerializeField]
    private Text characterDef;
    [SerializeField]
    private Button atkButton;
    [SerializeField]
    private Button[] skillButtons;
    [SerializeField]
    private Text nextTurnText;
    [SerializeField]
    private UITweenSequence nextTurnAnimation;

    private CompositeDisposable disposable;

    public void Init()
    {
        disposable = new CompositeDisposable();
        GameManager.Instance.turnBeginSubject.Subscribe(turn =>
        {
            nextTurnText.text = $"Turn {turn} Start";
            GameManager.Instance.beginTurnTasks.Add(nextTurnAnimation.Play());
        });
    }

    public void SetChara(CharacterLogic character)
    {
        disposable?.Dispose();
        disposable = new CompositeDisposable();
        characterName.text = character.name;
        characterTeam.text = character.team.ToString();
        disposable.Add(character.Hp.Subscribe(hp =>
        {
            characterHp.text = $"{hp} / {character.maxHp.Value}";
        }));
        disposable.Add(character.atk.Subscribe(atk =>
        {
            characterAtk.text = atk.ToString();
        }));
        disposable.Add(character.def.Subscribe(def =>
        {
            characterDef.text = def.ToString();
        }));
        disposable.Add(character.isDead.Subscribe(isDead=>
        {
            if(isDead)
            {
                atkButton.interactable = false;
            }
            else
            {
                atkButton.interactable = true;
            }
        }));
        for(int i = 0; i < 3; i++)
        {
            if(i < character.skills.Count)
            {
                skillButtons[i].gameObject.SetActive(true);
                
                skillButtons[i].onClick.RemoveAllListeners();
                SkillBase skill = character.skills[i];
                Button skillButton = skillButtons[i];
                skillButton.interactable = skill.canCast;
                disposable.Add(GameManager.Instance.turnBeginSubject.Subscribe(turn =>
                {
                    skillButton.interactable = skill.canCast;
                }));
                skillButtons[i].onClick.AddListener(()=>
                {
                    GameManager.Instance.OnClickSkill(skill);
                    skillButton.interactable = skill.canCast;
                });
                skillButtons[i].transform.GetChild(0).GetComponent<Text>().text = skill.skillName;
            }
            else
            {
                skillButtons[i].gameObject.SetActive(false);
            }
        }
    }

    public void OnClickAttack()
    {
        GameManager.Instance.OnClickAttack();
    }
}
