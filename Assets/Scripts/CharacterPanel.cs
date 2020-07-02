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
    private Text nextTurnText;
    [SerializeField]
    private UITweenSequence nextTurnAnimation;
    [SerializeField]
    private Text manaText;

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
        }));
        disposable.Add(GameManager.Instance.mana[(int)character.team].Subscribe(mana =>
        {
            manaText.text = $"Mana: {mana}";
        }));
        SkillCardManager.Instance.ShowDeck(character.team);
        
    }
}
