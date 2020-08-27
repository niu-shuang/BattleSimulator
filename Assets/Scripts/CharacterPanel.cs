using Cysharp.Threading.Tasks;
using Sirenix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UIAnimation;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class CharacterPanel : MonoBehaviour
{
    [SerializeField]
    private Text characterName;
    [SerializeField]
    private Text characterType;
    [SerializeField]
    private Text characterHp;
    [SerializeField]
    private Text characterAtk;
    [SerializeField]
    private Text characterDef;
    [SerializeField]
    private Text nextTurnText;
    //[SerializeField]
    //public UITweenSequence nextTurnAnimation;
    [SerializeField]
    private Text manaText;

    private CompositeDisposable disposable;
    private IDisposable turnBeginTaskDisposable;

    public void Init()
    {
        disposable = new CompositeDisposable();
        turnBeginTaskDisposable = GameManager.Instance.turnBeginSubject.Subscribe(turn =>
        {
            nextTurnText.text = $"Turn {turn}";
            //GameManager.Instance.beginTurnTasks.Add(()=> DoNextTurnAnimation());
        });
    }

    /*
    private async UniTask DoNextTurnAnimation()
    {  
        nextTurnText.color = new Color(nextTurnText.color.r, nextTurnText.color.g, nextTurnText.color.b, 1);
        await UniTask.Delay(250);
        nextTurnText.DOFade(0, .25f);
        await UniTask.Delay(250);
    }*/

    public void SetChara(CharacterLogic character)
    {
        disposable?.Dispose();
        disposable = new CompositeDisposable();
        characterName.text = character.name;
        if (characterType != null)
        {
            characterType.text = character.characterType.GetName();
        }

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
        disposable.Add(character.isDead.Subscribe(isDead =>
        {
        }));
        disposable.Add(GameManager.Instance.mana[(int)character.team].Subscribe(mana =>
        {
            manaText.text = $"Mana: {mana}/{GameManager.Instance.maxMana[(int)character.team].Value}";
        }));
        SkillCardManager.Instance.ShowDeck(character.team);

    }

    public void Dispose()
    {
        disposable?.Dispose();
        turnBeginTaskDisposable?.Dispose();
    }
}
