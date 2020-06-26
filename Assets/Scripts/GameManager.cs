using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using J;
using System;
using UniRx;
using UnityEngine.UI;
using System.IO;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    public enum GamePhase
    {
        Prepare,
        SelectChara,      //选择角色
        Attack,　　　　　 //选择攻击对象
        SpecSkill,             //选择施法对象
        EndTurn           //结束turn
    }

    public ReactiveProperty<GamePhase> phase;
    public int turn { get; private set; }
    public Subject<int> turnEndSubject;
    public Subject<int> turnBeginSubject;
    public List<UniTask> endTurnTasks;
    public List<UniTask> beginTurnTasks;


    private List<CharacterLogic> team1;
    private List<CharacterLogic> team2;
    [SerializeField]
    private GridHelper grids;
    [SerializeField]
    private CharacterPanel characterPanel;
    [SerializeField]
    private Text info;

    private CharacterLogic currentChara;
    private SkillBase currentSkill;
    private CompositeDisposable disposable;

    protected override void Awake()
    {
        base.Awake();
        turn = -1;
        turnEndSubject = new Subject<int>();
        endTurnTasks = new List<UniTask>();
        beginTurnTasks = new List<UniTask>();
        turnBeginSubject = new Subject<int>();
        phase = new ReactiveProperty<GamePhase>(GamePhase.Prepare);
        team1 = new List<CharacterLogic>();
        team2 = new List<CharacterLogic>();
        disposable = new CompositeDisposable();
        characterPanel.Init();
    }

    public void OnClickImportCharacter()
    {
        CharacterImporter.OpenExcel();
    }

    public void OnImportCharacterSuc(Dictionary<Vector2Int, CharacterInfo> team1Info, Dictionary<Vector2Int, CharacterInfo> team2Info)
    {
        SkillsImporter.OpenExcel(Path.Combine(CharacterImporter.path,"Skills.xls"));
        Dictionary<Vector2Int, KeyValuePair<CharacterLogic, Sprite>> team1 = new Dictionary<Vector2Int, KeyValuePair<CharacterLogic, Sprite>>();
        Dictionary<Vector2Int, KeyValuePair<CharacterLogic, Sprite>> team2 = new Dictionary<Vector2Int, KeyValuePair<CharacterLogic, Sprite>>();
        foreach (var item in team1Info)
        {
            CharacterLogic logic = new CharacterLogic(item.Value.characterId, item.Key, item.Value.characterName, item.Value.hp, Team.Team1, item.Value.atk, item.Value.def);
            foreach (var skillId in item.Value.skills)
            {
                if(skillId > 0)
                {
                    var skill = SkillsImporter.LoadSkill(skillId, logic);
                    logic.AddSkill(skill);
                }
            }
            Sprite sprite = Resources.Load<Sprite>($"Icons/{ item.Value.icon }");
            this.team1.Add(logic);
            team1[item.Key] = new KeyValuePair<CharacterLogic, Sprite>(logic, sprite);
        }
        foreach (var item in team2Info)
        {
            CharacterLogic logic = new CharacterLogic(item.Value.characterId, item.Key, item.Value.characterName, item.Value.hp, Team.Team2, item.Value.atk, item.Value.def);
            Sprite sprite = Resources.Load<Sprite>($"Icons/{ item.Value.icon }");
            this.team2.Add(logic);
            team2[item.Key] = new KeyValuePair<CharacterLogic, Sprite>(logic, sprite);
        }
        SkillsImporter.Close();
        grids.Init(team1, team2);
        disposable.Add(phase.Subscribe(OnPhaseChanged));
        NextTurnProcess();
    }


    public void OnPhaseChanged(GamePhase phase)
    {
        switch (phase)
        {
            case GamePhase.Prepare:
                break;
            case GamePhase.SelectChara:
                info.text = "Select a Chara";
                break;
            case GamePhase.Attack:
                info.text = "Select a target to attack";
                break;
            case GamePhase.SpecSkill:
                info.text = "Select a target to spell";
                break;
            case GamePhase.EndTurn:
                break;
            default:
                break;
        }
    }

    public void AddSummonCharacter(Team team, Vector2Int pos, CharacterInfo characterInfo, int aliveTime)
    {
        SummonedCharacter character = new SummonedCharacter(-1, pos, characterInfo.characterName, characterInfo.hp, team, characterInfo.atk, characterInfo.def, aliveTime);
        Sprite sprite = Resources.Load<Sprite>($"Icons/{ characterInfo.icon }");
        grids.AddCharacter(character, sprite, pos);

    }
    public void OnClickGrid(Vector2Int pos, Team team)
    {
        switch (phase.Value)
        {
            case GamePhase.Prepare:
                break;
            case GamePhase.SelectChara:
                {
                    var chara = grids.GetCharacter(team, pos);
                    if (chara != null)
                    {
                        characterPanel.SetChara(chara);
                        currentChara = chara;
                    }
                }
                break;
            case GamePhase.Attack:
                {
                    if (team == currentChara.team) return;
                    var target = grids.GetAttackTarget(team, pos.x);
                    currentChara.Attack(target);
                    phase.Value = GamePhase.SelectChara;
                }
                break;
            case GamePhase.SpecSkill:
                {
                    currentSkill.Cast(pos);
                    phase.Value = GamePhase.SelectChara;
                }
                break;
            case GamePhase.EndTurn:
                break;
            default:
                break;
        }
    }

    public CharacterLogic GetCharacter(Vector2Int pos, Team team)
    {
        return grids.GetCharacter(team, pos);
    }
    public void OnClickCancel()
    {
        switch (phase.Value)
        {
            case GamePhase.SpecSkill:
                phase.Value = GamePhase.SelectChara;
                break;
            default:
                break;
        }
    }

    public async void NextTurnProcess()
    {
        endTurnTasks.Clear();
        beginTurnTasks.Clear();
        turnEndSubject.OnNext(turn);
        for(int i = 0; i < endTurnTasks.Count; i++)
        {
            await endTurnTasks[i];
        }
        turn++;
        turnBeginSubject.OnNext(turn);
        for(int i = 0; i < beginTurnTasks.Count; i++)
        {
            await beginTurnTasks[i];
        }
        phase.Value = GamePhase.SelectChara;
        info.text = "Select a Chara";
    }

    public void OnClickAttack()
    {
        phase.Value = GamePhase.Attack;
    }

    public void OnClickSkill(SkillBase skill)
    {
        currentSkill = skill;
        if(currentSkill.selectable)
        {
            phase.Value = GamePhase.SpecSkill;
        }
        else
        {
            currentSkill.Cast(Vector2Int.zero);
            phase.Value = GamePhase.SelectChara; 
        }
    }

    protected override void SingletonOnDestroy()
    {
        base.SingletonOnDestroy();
        turnEndSubject.OnCompleted();
        turnBeginSubject.OnCompleted();
        disposable.Dispose();
    }
}
