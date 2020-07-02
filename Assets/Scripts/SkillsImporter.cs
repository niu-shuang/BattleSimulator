using System.Collections.Generic;
using UnityEngine;
using System.IO;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using UnityEditor;
using System;

public static class SkillsImporter
{
    private static HSSFWorkbook workbook;
    private static FileStream sheetFS;
    public static void OpenExcel(string path)
    {
        Debug.Log("loading : " + path);
        sheetFS = new FileStream(path, FileMode.Open, FileAccess.Read);
        if (sheetFS == null) return;
        workbook = new HSSFWorkbook(sheetFS);
    }

    public static SkillBase LoadSkill(int skillId, CharacterLogic character)
    {
        ISheet sheet = workbook.GetSheet(skillId.ToString());
        string skillLogicScript = sheet.GetRow(1).GetCell(1).GetString();
        Type skillType = Type.GetType(skillLogicScript);
        string skillName = sheet.GetRow(2).GetCell(1).GetString();
        int coolDown = sheet.GetRow(3).GetCell(1).GetInt();
        bool selectable = sheet.GetRow(4).GetCell(1).GetBoolean();
        string description = sheet.GetRow(5).GetCell(1).GetString();
        SkillBase skill = Activator.CreateInstance(skillType, skillId, skillName, coolDown, selectable, character, description) as SkillBase;
        skill.LoadCustomProperty(sheet);
        return skill;
    }

    public static void Close()
    {
        workbook.Close();
        sheetFS.Close();
    }
}
