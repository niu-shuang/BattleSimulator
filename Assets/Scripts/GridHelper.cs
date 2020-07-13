using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridHelper : MonoBehaviour
{
    [SerializeField]
    private List<Grid> team1Grid;
    [SerializeField]
    private List<Grid> team2Grid;

    private const int COL_NUM = 3;

    public CharacterLogic GetAttackTarget(Team team, int col)
    {
        if (team == Team.Team1)
        {
            for (int i = 0; i < 2; i++)
            {
                if (team1Grid[col + i * COL_NUM].hasCharacter)
                {
                    return team1Grid[col + i * COL_NUM].characterLogic;
                }
            }
        }
        else
        {
            for (int i = 0; i < 3; i++)
            {
                if (team2Grid[col + i * COL_NUM].hasCharacter)
                {
                    return team2Grid[col + i * COL_NUM].characterLogic;
                }
            }
        }
        return null;
    }

    public CharacterLogic GetCharacter(Team team, Vector2Int pos)
    {
        if (team == Team.Team1)
        {
            return team1Grid[pos.x + pos.y * COL_NUM].characterLogic;
        }
        else
        {
            return team2Grid[pos.x + pos.y * COL_NUM].characterLogic;
        }
    }

    public void Init(Dictionary<Vector2Int, KeyValuePair<CharacterLogic, Sprite>> team1, Dictionary<Vector2Int, KeyValuePair<CharacterLogic, Sprite>> team2)
    {
        for (int i = 0; i < COL_NUM; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                Vector2Int currentPos = new Vector2Int(i, j);
                int currentIndex = currentPos.x + currentPos.y * COL_NUM;
                if (team1.ContainsKey(currentPos))
                {
                    team1Grid[currentIndex].SetCharacter(team1[currentPos].Key, team1[currentPos].Value, currentPos, Team.Team1);
                }
                else
                {
                    team1Grid[currentIndex].SetEmpty(currentPos, Team.Team1);
                }

                if (team2.ContainsKey(currentPos))
                {
                    team2Grid[currentIndex].SetCharacter(team2[currentPos].Key, team2[currentPos].Value, currentPos, Team.Team2);
                }
                else
                {
                    team2Grid[currentIndex].SetEmpty(currentPos, Team.Team2);
                }
            }
        }
    }

    public void AddCharacter(CharacterLogic character, Sprite sprite, Vector2Int pos)
    {
        if (character.team == Team.Team1)
        {
            int index = pos.x + pos.y * COL_NUM;
            team1Grid[index].SetCharacter(character, sprite, pos, Team.Team1);
        }
        else
        {
            int index = pos.x + pos.y * COL_NUM;
            team2Grid[index].SetCharacter(character, sprite, pos, Team.Team2);
        }
    }

    public void RemoveCharacter(Vector2Int pos, Team team)
    {
        if (team == Team.Team1)
        {
            int index = pos.x + pos.y * COL_NUM;
            var chara = team1Grid[index].characterLogic;
            chara.Hp.Value = 0;
            team1Grid[index].SetEmpty(pos, team);
        }
        else
        {
            int index = pos.x + pos.y * COL_NUM;
            var chara = team2Grid[index].characterLogic;
            chara.Hp.Value = 0;
            team2Grid[index].SetEmpty(pos, team);
        }
    }
}
