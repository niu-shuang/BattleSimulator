using System;
using System.Collections.Generic;

[Serializable]
public class CharacterInfo
{
    public int characterId;
    public string script;
    public string characterName;
    public string icon;
    public int hp;
    public int atk;
    public int def;
    public List<int> skills;
    public string characterType;
    //千分数
    public int dodgeRate;
}
