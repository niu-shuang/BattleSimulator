using J;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

public class SkillCardManager : SingletonMonoBehaviour<SkillCardManager>
{
    [SerializeField]
    private List<SkillCardView> skillCardViews;
    [SerializeField]
    private Text cardPoolCount;
    [SerializeField]
    private Text usedCardPoolCount;
    [SerializeField]
    private GameObject skillCardsPopupPrefab;

    private List<SkillBase> team1Skills;
    private List<SkillBase> team2Skills;

    public ReactiveCollection<SkillBase> team1CardPool;
    public ReactiveCollection<SkillBase> team2CardPool;
    public ReactiveCollection<SkillBase> currentDeckTeam1;
    public ReactiveCollection<SkillBase> currentDeckTeam2;
    public ReactiveCollection<SkillBase> team1UsedCardPool;
    public ReactiveCollection<SkillBase> team2UsedCardPool;
    private CompositeDisposable disposable;
    private Team currentTeam;
    private bool cardAdded = false;


    public void Init()
    {
        team1Skills = new List<SkillBase>();
        team2Skills = new List<SkillBase>();
        team1CardPool = new ReactiveCollection<SkillBase>();
        team2CardPool = new ReactiveCollection<SkillBase>();
        team1UsedCardPool = new ReactiveCollection<SkillBase>();
        team2UsedCardPool = new ReactiveCollection<SkillBase>();
        currentDeckTeam1 = new ReactiveCollection<SkillBase>();
        currentDeckTeam2 = new ReactiveCollection<SkillBase>();

        disposable = new CompositeDisposable();
        disposable.Add(GameManager.Instance.turnBeginSubject.Subscribe(turn =>
        {
            if (!cardAdded)
            {
                foreach (var item in team1Skills)
                {
                    team1CardPool.Add(item.Clone());
                }
                foreach (var item in team2Skills)
                {
                    team2CardPool.Add(item.Clone());
                }
                cardAdded = true;
            }
            GenerateDeck(team1CardPool, currentDeckTeam1, team1UsedCardPool);
            GenerateDeck(team2CardPool, currentDeckTeam2, team2UsedCardPool);
            ShowDeck(currentTeam);
        }));
        disposable.Add(GameManager.Instance.turnEndSubject.Subscribe(turn =>
        {
            foreach (var item in currentDeckTeam1)
            {
                team1UsedCardPool.Add(item);
            }
            currentDeckTeam1.Clear();
            foreach (var item in currentDeckTeam2)
            {
                team2UsedCardPool.Add(item);
            }
            currentDeckTeam2.Clear();
        }));
        disposable.Add(team1CardPool.ObserveCountChanged(true).Subscribe(_ => OnCardPoolCountChanged()));
        disposable.Add(team2CardPool.ObserveCountChanged(true).Subscribe(_ => OnCardPoolCountChanged()));
        disposable.Add(team1UsedCardPool.ObserveCountChanged(true).Subscribe(_ => OnUsedCardPoolCountChanged()));
        disposable.Add(team2UsedCardPool.ObserveCountChanged(true).Subscribe(_ => OnUsedCardPoolCountChanged()));
    }

    public void ShowDeck(Team team)
    {
        ReactiveCollection<SkillBase> currentDeck = team == Team.Team1 ? currentDeckTeam1 : currentDeckTeam2;
        for (int i = 0; i < skillCardViews.Count; i++)
        {
            if (i < currentDeck.Count)
                skillCardViews[i].SetData(currentDeck[i]);
            else
                skillCardViews[i].SetEmpty();
        }
        currentTeam = team;
        OnCardPoolCountChanged();
        OnUsedCardPoolCountChanged();
    }

    private void OnCardPoolCountChanged()
    {
        if (currentTeam == Team.Team1)
        {
            cardPoolCount.text = team1CardPool.Count.ToString();
        }
        else
        {
            cardPoolCount.text = team2CardPool.Count.ToString();
        }
    }

    private void OnUsedCardPoolCountChanged()
    {
        if (currentTeam == Team.Team1)
        {
            usedCardPoolCount.text = team1UsedCardPool.Count.ToString();
        }
        else
        {
            usedCardPoolCount.text = team2UsedCardPool.Count.ToString();
        }
    }

    public void PickSkill(Team team)
    {
        var currentDeck = team == Team.Team1 ? currentDeckTeam1 : currentDeckTeam2;
        var usedCard = team == Team.Team1 ? team1UsedCardPool : team2UsedCardPool;
        var cardPool = team == Team.Team1 ? team1CardPool : team2CardPool;
        int rand = 0;
        if(cardPool.Count == 0)
        {
            rand = Random.Range(0, usedCard.Count);
            var tempCard = usedCard[rand];
            usedCard.Remove(tempCard);
            cardPool.Add(tempCard);
        }
        rand = Random.Range(0, cardPool.Count);
        var card = cardPool[rand];
        currentDeck.Add(card);
        cardPool.Remove(card);
    }


    private void GenerateDeck(ReactiveCollection<SkillBase> cardPool, ReactiveCollection<SkillBase> deck, ReactiveCollection<SkillBase> usedCardPool)
    {
        if (cardPool.Count < GameDefine.CARD_GENERATE_NUM)
        {
            List<SkillBase> tempDeck = new List<SkillBase>();
            for (int i = 0; i < GameDefine.CARD_GENERATE_NUM - cardPool.Count; i++)
            {
                if (usedCardPool.Count > 0)
                {
                    tempDeck.Add(usedCardPool[0].Clone());
                    usedCardPool.RemoveAt(0);
                }
                else
                {
                    break;
                }
            }
            foreach (var item in tempDeck)
            {
                cardPool.Add(item);
            }
        }

        for (int i = 0; i < GameDefine.CARD_GENERATE_NUM; i++)
        {
            if (cardPool.Count > 0)
            {
                int rand = Random.Range(0, cardPool.Count);
                deck.Add(cardPool[rand]);
                cardPool.RemoveAt(rand);
            }
        }
    }

    public void OnUseSkill(SkillBase skill)
    {
        skill.Reset();
        if (skill.caster.team == Team.Team1)
        {
            team1UsedCardPool.Add(skill);
            currentDeckTeam1.Remove(skill);
        }
        else
        {
            team2UsedCardPool.Add(skill);
            currentDeckTeam2.Remove(skill);
        }
    }

    public void AddSkill(SkillBase skill, Team team)
    {
        if (team == Team.Team1)
        {
            team1Skills.Add(skill);
        }
        else
        {
            team2Skills.Add(skill);
        }
    }

    public async void OnClickShowCardPool()
    {
        var popup = PopupManager.Instance.Create<SkillCardsPopup>(skillCardsPopupPrefab);
        if (currentTeam == Team.Team1)
            popup.SetData(team1CardPool.ToList(), "抽牌堆");
        else
            popup.SetData(team2CardPool.ToList(), "抽牌堆");
        await PopupManager.Instance.ShowAsync(typeof(SkillCardsPopup));
        await PopupManager.Instance.HideAsync();
        popup.ClearView();
    }

    public async void OnClickShowUsedCardPool()
    {
        var popup = PopupManager.Instance.Create<SkillCardsPopup>(skillCardsPopupPrefab);
        if (currentTeam == Team.Team1)
            popup.SetData(team1UsedCardPool.ToList(), "弃牌堆");
        else
            popup.SetData(team2UsedCardPool.ToList(), "弃牌堆");
        await PopupManager.Instance.ShowAsync(typeof(SkillCardsPopup));
        await PopupManager.Instance.HideAsync();
        popup.ClearView();
    }

    protected override void SingletonOnDestroy()
    {
        disposable?.Dispose();
    }
}
