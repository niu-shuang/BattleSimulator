using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameLogger : MonoBehaviour
{
    [SerializeField]
    private Text output;
    private static GameLogger instance;

    private void Awake()
    {
        instance = this;
    }

    public static void AddLog(string log)
    {
        instance.output.text += log + Environment.NewLine;
    }

    public static void ClearLog()
    {
        instance.output.text = string.Empty;
    }
}
