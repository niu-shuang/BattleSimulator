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
    private RectTransform infoTextRect;

    private CompositeDisposable disposable;
    public void Init(CharacterLogic character, Sprite iconSprite)
    {
        disposable = new CompositeDisposable();
        icon.sprite = iconSprite;
        icon.color = Color.white;
        hpBar.transform.parent.gameObject.SetActive(true);
        hpBar.fillAmount = 1;
        infoTextRect.localScale = Vector3.zero;
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
            switch (info.type)
            {
                case CharacterLogType.Damage:
                    infoText.text = info.value.ToString();
                    infoText.color = Color.red;
                    break;
                case CharacterLogType.Heal:
                    infoText.text = info.value.ToString();
                    infoText.color = Color.green;
                    break;
                default:
                    break;
            }
            
            animationPhase = AnimationPhase.Phase1;
            startTime = Time.time;
        }));
    }

    private enum AnimationPhase
    {
        None,
        Phase1,
        Phase2,
        Phase3,
    }
    private AnimationPhase animationPhase = AnimationPhase.None;
    float startTime;
    float runTime;
    const float phase1Duration = .2f;
    const float phase2Duration = .7f;
    const float phase3Duration = .3f;

    private void Update()
    {
        runTime = Time.time;
        if(animationPhase == AnimationPhase.Phase1)
        {
            infoTextRect.localScale = Vector3.one * (runTime - startTime) / phase1Duration;
            if(runTime - startTime >= phase1Duration)
            {
                animationPhase = AnimationPhase.Phase2;
                startTime = Time.time;
            }
        }
        else if(animationPhase == AnimationPhase.Phase2)
        {
            if (runTime - startTime >= phase2Duration)
            {
                animationPhase = AnimationPhase.Phase3;
                startTime = Time.time;
            }
        }
        else if(animationPhase == AnimationPhase.Phase3)
        {
            infoText.color = new Color(infoText.color.r, infoText.color.g, infoText.color.b, (1 - (runTime - startTime) / phase3Duration));
            if (runTime - startTime >= phase2Duration)
            {
                animationPhase = AnimationPhase.None;
                infoTextRect.localScale = Vector3.zero;
                infoText.color = Color.white;
                startTime = Time.time;
            }
        }
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
        infoTextRect = transform.GetChild(2).GetComponent<RectTransform>();
    }
}
