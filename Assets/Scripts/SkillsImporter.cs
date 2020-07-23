using System.Collections.Generic;
using UnityEngine;
using System.IO;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using UnityEditor;
using System;

public static class SkillsImporter
{
    private static List<HSSFWorkbook> workbooks;
    private static List<FileStream> sheetFSs;
    public static void OpenExcel(string path)
    {
        Debug.Log("loading : " + path);
        if(sheetFSs == null)
            sheetFSs = new List<FileStream>();
        var sheetFS = new FileStream(path, FileMode.Open, FileAccess.Read);
        if (sheetFS == null) return;
        sheetFSs.Add(sheetFS);
        if(workbooks == null)
            workbooks = new List<HSSFWorkbook>();
        var workbook = new HSSFWorkbook(sheetFS);
        workbooks.Add(workbook);
    }

    public static SkillBase LoadSkill(int skillId, CharacterLogic character)
    {
        ISheet sheet = workbooks[skillId / 10].GetSheet(skillId.ToString());
        string skillLogicScript = sheet.GetRow(1).GetCell(1).GetString();
        Type skillType = Type.GetType(skillLogicScript);
        string skillName = sheet.GetRow(2).GetCell(1).GetString();
        int coolDown = sheet.GetRow(3).GetCell(1).GetInt();
        bool selectable = sheet.GetRow(4).GetCell(1).GetBoolean();
        string description = sheet.GetRow(5).GetCell(1).GetString();
        try
        {
            SkillBase skill = Activator.CreateInstance(skillType, skillId, skillName, coolDown, selectable, character, description) as SkillBase;
            skill.LoadCustomProperty(sheet);
            return skill;
        }
        catch(Exception e)
        {
            Debug.LogError("skill " + skillId.ToString());
            Debug.LogError(e);
        }
        return null; 
    }

    public static void Close()
    {
        foreach (var item in workbooks)
        {
            item.Close();
        }
        foreach (var item in sheetFSs)
        {
            item.Close();
        }
    }
}
