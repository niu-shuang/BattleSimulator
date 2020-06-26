using System.Collections.Generic;
using UnityEngine;
using System.IO;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using System;
using SFB;

public static class CharacterImporter
{
    public static string path { get; private set; }
    public static void OpenExcel()
    {
        ExtensionFilter[] filter = new ExtensionFilter[1] { new ExtensionFilter("xls", "xls") };
        StandaloneFileBrowser.OpenFilePanelAsync("Choose xls file", Application.streamingAssetsPath, filter, false, OnSuccess);
    }

    private static void OnSuccess(string[] paths)
    {
        if (paths.Length < 1 || paths[0] != null)
        {
            path = Path.GetDirectoryName(paths[0]);
            LoadExcel(paths[0]);
        }
    }

    private static void LoadExcel(string path)
    {
        Debug.Log("loading : " + path);
        FileStream sheetFS = new FileStream(path, FileMode.Open, FileAccess.Read);
        if (sheetFS == null) return;
        HSSFWorkbook workbook = new HSSFWorkbook(sheetFS);
        ISheet configSheet = workbook.GetSheet("Config");
        Dictionary<Vector2Int, CharacterInfo> team1 = new Dictionary<Vector2Int, CharacterInfo>();
        Dictionary<Vector2Int, CharacterInfo> team2 = new Dictionary<Vector2Int, CharacterInfo>();
        int index = 1;
        #region team1
        while (true)
        {
            var row = configSheet.GetRow(index++);
            if (row == null) break;
            var cell = row.GetCell(0);
            if (cell == null) break;
            CharacterInfo charaInfo = new CharacterInfo();
            charaInfo.characterId = cell.GetInt();
            if (charaInfo.characterId == 0) break;
            string posStr = row.GetCell(1).GetString();
            string[] splited = posStr.Split(',');
            int x = int.Parse(splited[0].Substring(1));
            int y = int.Parse(splited[1].Substring(0, splited[1].Length - 1));
            ISheet charaSheet = workbook.GetSheet(charaInfo.characterId.ToString());
            LoadCharacter(charaSheet, charaInfo);
            team1[new Vector2Int(x, y)] = charaInfo;
        }
        #endregion
        #region team2
        index = 1;
        while (true)
        {
            var row = configSheet.GetRow(index++);
            if (row == null) break;
            var cell = row.GetCell(2);
            if (cell == null) break;
            CharacterInfo charaInfo = new CharacterInfo();
            charaInfo.characterId = cell.GetInt();
            if (charaInfo.characterId == 0) break;
            string posStr = row.GetCell(3).GetString();
            string[] splited = posStr.Split(',');
            int x = int.Parse(splited[0].Substring(1));
            int y = int.Parse(splited[1].Substring(0, splited[1].Length - 1));
            ISheet charaSheet = workbook.GetSheet(charaInfo.characterId.ToString());
            LoadCharacter(charaSheet, charaInfo);
            team2[new Vector2Int(x, y)] = charaInfo;
        }
        #endregion
        workbook.Close();
        sheetFS.Close();
        GameManager.Instance.OnImportCharacterSuc(team1, team2);
    }

    private static void LoadCharacter(ISheet charaSheet, CharacterInfo info)
    {
        var row = charaSheet.GetRow(0);
        if (row == null) Debug.LogError("read charaName failed");
        var cell = row.GetCell(0);
        if (cell == null) Debug.LogError("read charaName failed");
        info.characterName = row.GetCell(1).GetString();
        info.icon = charaSheet.GetRow(1).GetCell(1).GetString();
        info.hp = charaSheet.GetRow(2).GetCell(1).GetInt();
        info.atk = charaSheet.GetRow(3).GetCell(1).GetInt();
        info.def = charaSheet.GetRow(4).GetCell(1).GetInt();
        int[] skills = new int[3];
        skills[0] = charaSheet.GetRow(5).GetCell(1).GetInt();
        skills[1] = charaSheet.GetRow(6).GetCell(1).GetInt();
        skills[2] = charaSheet.GetRow(7).GetCell(1).GetInt();
        info.skills = skills;
    }

}

public partial class GlobalExtensionMethods
{
    public static string GetString(this ICell cell)
    {
        if (cell == null) return string.Empty;
        switch (cell.CellType)
        {
            case CellType.Unknown:
                return string.Empty;
            case CellType.Numeric:
                return cell.NumericCellValue.ToString();
            case CellType.String:
                return cell.StringCellValue;
            case CellType.Formula:
                return cell.CellFormula;
            case CellType.Boolean:
                if (cell.BooleanCellValue) return "TRUE";
                else return "FALSE";
            //return cell.BooleanCellValue.ToString();
            case CellType.Blank:
            case CellType.Error:
                return string.Empty;
            default:
                return string.Empty;
        }
    }

    public static int GetInt(this ICell cell)
    {
        if (cell == null) return 0;
        switch (cell.CellType)
        {
            case CellType.Unknown:
                return 0;
            case CellType.Numeric:
                return (int)cell.NumericCellValue;
            case CellType.String:
                return int.Parse(cell.StringCellValue);
            case CellType.Formula:
                return 0;
            case CellType.Boolean:
                if (cell.BooleanCellValue) return 1;
                else return 0;
            //return cell.BooleanCellValue.ToString();
            case CellType.Blank:
            case CellType.Error:
            default:
                return 0;
        }
    }

    public static bool GetBoolean(this ICell cell)
    {
        if (cell == null) return false;
        switch (cell.CellType)
        {
            case CellType.Unknown:
                return false ;
            case CellType.Numeric:
                return (int)cell.NumericCellValue > 0;
            case CellType.String:
                return Boolean.Parse(cell.StringCellValue);
            case CellType.Formula:
                return false;
            case CellType.Boolean:
                return cell.BooleanCellValue;
            //return cell.BooleanCellValue.ToString();
            case CellType.Blank:
            case CellType.Error:
            default:
                return false;
        }
    }
}
