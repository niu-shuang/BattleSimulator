using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UIAnimation;
using Cysharp.Threading.Tasks;

public class CharacterView : MonoBehaviour
{
    [SerializeField]
    private Image icon;
    [SerializeField]
    private Image hpBar;
    [SerializeField]
    private Text infoText;
    [SerializeField]
    private UITweenSequence infoTextAnimation;
    private CharacterLogic logic;

    private CompositeDisposable disposable;
    public void Init(CharacterLogic character, Sprite iconSprite)
    {
        disposable = new CompositeDisposable();
        logic = character;
        icon.sprite = iconSprite;
        icon.color = Color.white;
        hpBar.transform.parent.gameObject.SetActive(true);
        disposable.Add(character.isDead.Subscribe(isDead =>
        {
            if(isDead == true)
            {
                SetEmpty();
                disposable.Dispose();
            }
        }));
        disposable.Add(character.Hp.Subscribe(hp =>
        {
            hpBar.fillAmount = hp / (float)character.maxHp.Value;
        }));
        disposable.Add(character.info.Subscribe(info =>
        {
            if(info != string.Empty)
            {
                infoText.text = info;
                infoTextAnimation.Play().Forget();
            }
        }));
    }

    public void SetEmpty()
    {
        icon.color = new Color(0, 0, 0, 0);
        hpBar.transform.parent.gameObject.SetActive(false);
        disposable?.Dispose();
    }

    private void Reset()
    {
        icon = transform.GetChild(0).GetComponent<Image>();
        hpBar = transform.GetChild(1).GetChild(0).GetComponent<Image>();
        infoText = transform.GetChild(2).GetComponent<Text>();
        infoTextAnimation = transform.GetChild(2).GetComponent<UITweenSequence>();
    }
}
