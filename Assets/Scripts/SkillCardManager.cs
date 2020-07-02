using J;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class SkillCardManager : SingletonMonoBehaviour<SkillCardManager>
{
    [SerializeField]
    private List<SkillCardView> skillCards;

    private List<SkillBase> team1Skills;
    private List<SkillBase> team2Skills;

    private List<SkillBase> currentDeckTeam1;
    private List<SkillBase> currentDeckTeam2;
    private CompositeDisposable disposable;
    private Team currentTeam;

    public void Init()
    {
        team1Skills = new List<SkillBase>();
        team2Skills = new List<SkillBase>();
        disposable = new CompositeDisposable();
        disposable.Add(GameManager.Instance.turnBeginSubject.Subscribe(turn =>
        {
            currentDeckTeam1 = new List<SkillBase>();
            currentDeckTeam2 = new List<SkillBase>();
            GenerateDeck(team1Skills, currentDeckTeam1);
            GenerateDeck(team2Skills, currentDeckTeam2);
            currentDeckTeam1.ForEach(i => i.ResetCastFlag());
            currentDeckTeam2.ForEach(i => i.ResetCastFlag());
            ShowDeck(currentTeam);
        }));
    }

    public void ShowDeck(Team team)
    {
        for(int i = 0; i < GameDefine.DECK_NUM; i++)
        {
            skillCards[i].SetData(team == Team.Team1 ? currentDeckTeam1[i] : currentDeckTeam2[i]);
        }
        currentTeam = team;
    }

    private void GenerateDeck(List<SkillBase> repo, List<SkillBase> deck)
    {
        for(int i = 0; i < GameDefine.DECK_NUM; i++)
        {
            deck.Add(repo[Random.Range(0,repo.Count)].Clone());
        }
    }

    public void AddSkill(SkillBase skill, Team team)
    {
        if(team == Team.Team1)
        {
            team1Skills.Add(skill);
        }
        else
        {
            team2Skills.Add(skill);
        }
    }

    protected override void SingletonOnDestroy()
    {
        disposable?.Dispose();
    }
}
