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

    public int overrideDrawNum;
    private int registeredSkill;


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
        registeredSkill = 0;
        overrideDrawNum = 0;
        disposable = new CompositeDisposable();
        disposable.Add(GameManager.Instance.turnBeginSubject.Subscribe(turn =>
        {
            if (!cardAdded)
            {
                foreach (var item in team1Skills)
                {
                    team1UsedCardPool.Add(item.Clone());
                }
                foreach (var item in team2Skills)
                {
                    team2UsedCardPool.Add(item.Clone());
                }
                cardAdded = true;
            }
            GenerateDeck();
            ShowDeck(currentTeam);
        }));
        disposable.Add(GameManager.Instance.turnEndSubject.Subscribe(turn =>
        {
            /*
            foreach (var item in currentDeckTeam1)
            {
                team1UsedCardPool.Add(item);
            }
            currentDeckTeam1.Clear();
            foreach (var item in currentDeckTeam2)
            {
                team2UsedCardPool.Add(item);
            }
            currentDeckTeam2.Clear();*/
        }));
        disposable.Add(team1CardPool.ObserveCountChanged(true).Subscribe(_ => OnCardPoolCountChanged()));
        disposable.Add(team2CardPool.ObserveCountChanged(true).Subscribe(_ => OnCardPoolCountChanged()));
        disposable.Add(team1UsedCardPool.ObserveCountChanged(true).Subscribe(_ => OnUsedCardPoolCountChanged()));
        disposable.Add(team2UsedCardPool.ObserveCountChanged(true).Subscribe(_ => OnUsedCardPoolCountChanged()));
        disposable.Add(currentDeckTeam1.ObserveAdd()
            .Subscribe(addEvent =>
            {
                addEvent.Value.OnAddToDeck();
            }));
        disposable.Add(currentDeckTeam1.ObserveRemove()
            .Subscribe(removeEvent =>
            {
                removeEvent.Value.OnRemovedFromDeck();
            }));
        disposable.Add(currentDeckTeam2.ObserveAdd()
            .Subscribe(addEvent =>
            {
                addEvent.Value.OnAddToDeck();
            }));
        disposable.Add(currentDeckTeam2.ObserveRemove()
            .Subscribe(removeEvent =>
            {
                removeEvent.Value.OnRemovedFromDeck();
            }));
    }

    public void ShowDeck(Team team)
    {
        ReactiveCollection<SkillBase> currentDeck = team == Team.Team1 ? currentDeckTeam1 : currentDeckTeam2;
        for (int i = 0; i < skillCardViews.Count; i++)
        {
            if (i < currentDeck.Count)
                skillCardViews[i].SetData(currentDeck[i], true);
            else
                skillCardViews[i].SetEmpty();
        }
        currentTeam = team;
        OnCardPoolCountChanged();
        OnUsedCardPoolCountChanged();
    }

    public void OnCharacterDead(CharacterLogic character)
    {
        var deck = character.team == Team.Team1 ? currentDeckTeam1 : currentDeckTeam2;
        var cardPool = character.team == Team.Team1 ? team1CardPool : team2CardPool;
        var usedCardPool = character.team == Team.Team1 ? team1UsedCardPool : team2UsedCardPool;
        var cardToKeep = deck.Where(i => i.caster != character);
        ReactiveCollection<SkillBase> newDeck = new ReactiveCollection<SkillBase>();
        foreach (var item in cardToKeep)
        {
            newDeck.Add(item.Clone());
        }
        
        ReactiveCollection<SkillBase> newCardPool = new ReactiveCollection<SkillBase>();
        cardToKeep = cardPool.Where(i => i.caster != character);
        foreach (var item in cardToKeep)
        {
            newCardPool.Add(item.Clone());
        }
        ReactiveCollection<SkillBase> newUsedCardPool = new ReactiveCollection<SkillBase>();
        cardToKeep = usedCardPool.Where(i => i.caster != character);
        foreach (var item in cardToKeep)
        {
            newUsedCardPool.Add(item);
        }
        if (character.team == Team.Team1)
        {
            currentDeckTeam1 = newDeck;
            team1CardPool = newCardPool;
            team1UsedCardPool = newUsedCardPool;
        }
        else
        {
            currentDeckTeam2 = newDeck;
            team2CardPool = newCardPool;
            team2UsedCardPool = newUsedCardPool;
        }
        ShowDeck(Team.Team1);
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
        if(team == Team.Team1)
        {
            if(team1CardPool.Count == 0)
            {
                team1CardPool = Shuffle(team1CardPool, team1UsedCardPool);
            }
                
            var rand = Random.Range(0, team1CardPool.Count);
            var card = team1CardPool[rand];
            currentDeckTeam1.Add(card);
            team1CardPool.Remove(card);
        }
        else
        {
            if (team2CardPool.Count == 0)
            {
                team2CardPool = Shuffle(team2CardPool, team2UsedCardPool);
            }
                
            var rand = Random.Range(0, team2CardPool.Count);
            var card = team2CardPool[rand];
            currentDeckTeam2.Add(card);
            team2CardPool.Remove(card);
        }
    }

    public void ChargeCardPool(Team team)
    {
        if(team == Team.Team1)
        {
            team1CardPool = Shuffle(team1CardPool, team1UsedCardPool);
        }
        else
        {
            team2CardPool = Shuffle(team2CardPool, team2UsedCardPool);
        }
    }


    private void GenerateDeck()
    {
        {
            var allCardNum = team1CardPool.Count + currentDeckTeam1.Count + team1UsedCardPool.Count;
            int drawNum = overrideDrawNum > 0 ? overrideDrawNum : GameDefine.DRAW_CARD_NUM;
            var diff = drawNum;
            if(GameDefine.DECK_MAX_NUM - currentDeckTeam1.Count < drawNum)
            {
                diff = GameDefine.DECK_MAX_NUM - currentDeckTeam1.Count;
            }
            if(GameManager.Instance.turn == 1)
            {
                diff = GameDefine.DECK_INIT_NUM;
            }
            //var loopTime = GameDefine.CARD_GENERATE_NUM < allCardNum ? GameDefine.CARD_GENERATE_NUM : allCardNum;
            for (int i = 0; i < diff; i++)
            {
                if (team1CardPool.Count == 0)
                {
                    team1CardPool = Shuffle(team1UsedCardPool, team1CardPool);
                    currentDeckTeam1.Add(team1CardPool[0]);
                    team1CardPool.RemoveAt(0);
                }
                else
                {
                    currentDeckTeam1.Add(team1CardPool[0]);
                    team1CardPool.RemoveAt(0);
                }
            }
            overrideDrawNum = 0;
        }
        {
            var allCardNum = team2CardPool.Count + currentDeckTeam2.Count + team2UsedCardPool.Count;
            var loopTime = GameDefine.DECK_MAX_NUM < allCardNum ? GameDefine.DECK_MAX_NUM : allCardNum;
            for (int i = 0; i < loopTime; i++)
            {
                if (team2CardPool.Count == 0)
                {
                    team2CardPool = Shuffle(team2UsedCardPool, team2CardPool);
                    currentDeckTeam2.Add(team2CardPool[0]);
                    team2CardPool.RemoveAt(0);
                }
                else
                {
                    currentDeckTeam2.Add(team2CardPool[0]);
                    team2CardPool.RemoveAt(0);
                }
            }
        }        
    }

    private ReactiveCollection<SkillBase> Shuffle(ReactiveCollection<SkillBase> usedCardPool, ReactiveCollection<SkillBase> cardPool)
    {
        ReactiveCollection<SkillBase> gen = new ReactiveCollection<SkillBase>();
        while(usedCardPool.Count > 0)
        {
            var rand = Random.Range(0, usedCardPool.Count);
            gen.Add(usedCardPool[rand].Clone());
            usedCardPool.RemoveAt(rand);
        }
        foreach (var item in cardPool)
        {
            gen.Add(item.Clone());
        }
        cardPool.Clear();
        return gen;
    }

    public void OnUseSkill(SkillBase skill)
    {
        skill.Reset();
        if (skill.caster.team == Team.Team1)
        {
            team1UsedCardPool.Add(skill);
            var tempDeck = new ReactiveCollection<SkillBase>();
            foreach (var item in currentDeckTeam1)
            {
                if(item != skill)
                {
                    tempDeck.Add(item);
                }
            }
            currentDeckTeam1 = tempDeck;
        }
        else
        {
            team2UsedCardPool.Add(skill);
            currentDeckTeam2.Remove(skill);
        }
        ShowDeck(Team.Team1);
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

    public int GetUniqueId()
    {
        return registeredSkill++;
    }

    protected override void SingletonOnDestroy()
    {
        disposable?.Dispose();
    }
}
