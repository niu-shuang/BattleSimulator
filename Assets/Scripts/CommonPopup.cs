using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommonPopup : PopupBase
{
    [SerializeField]
    private Text _title;
    [SerializeField]
    private Text _message;

    /// <summary>
    /// ボタンを設定する
    /// </summary>
    /// <param name="buttonSettings"></param>
    public void SetButton(List<PopupManager.ButtonSetting> buttonSettings, int backKeyButtonIndex = -1)
    {
        buttons.ForEach(i => i.gameObject.SetActive(false));
        foreach (var item in buttonSettings)
        {
            int index = (int)item.buttonType;
            buttons[index].gameObject.SetActive(true);
            buttons[index].GetComponentInChildren<Text>().text = item.label;
        }

        canUseBackKey = buttonSettings.Count < 2 || backKeyButtonIndex > -1;
        _backKeyButtonIndex = backKeyButtonIndex;
    }

    /// <summary>
    /// ポップアップの内容を設定する
    /// </summary>
    /// <param name="title"></param>
    /// <param name="message"></param>
    public void SetData(string title, string message)
    {
        if (_title != null)
            _title.text = title;
        _message.text = message;
    }
}