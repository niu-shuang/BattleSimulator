using System;
using System.Collections;
using System.Collections.Generic;
using System.Management.Instrumentation;
using UnityEngine;
using UnityEngine.UI;

public class GameLogger : MonoBehaviour
{
    [SerializeField]
    private Text output;
    [SerializeField]
    private Scrollbar scrollBar;
    private static GameLogger instance;

    private List<string> logStr;

    private void Awake()
    {
        instance = this;
        logStr = new List<string>();
    }

    public static void AddLog(string log)
    {
        instance.logStr.Add(log);
        var count = instance.logStr.Count;
        instance.output.text = string.Empty;
        int maxLoop = 0;
        if (count <= 20)
            maxLoop = count;
        else
            maxLoop = 20;
        for(int i = count - 1; i >= count - maxLoop; i--)
        {
            instance.output.text += instance.logStr[i] + Environment.NewLine;
        }
        instance.scrollBar.value = 1;
    }

    public static void ClearLog()
    {
        instance.output.text = string.Empty;
    }
}
