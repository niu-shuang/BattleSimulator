using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;
using J;
using Cysharp.Threading.Tasks;
using UniRx;

public partial class PopupManager : SingletonMonoBehaviour<PopupManager>
{
    /// <summary>
    /// アラートポップアップのタイプ.
    /// </summary>
    public enum AlertType
    {
        TitleAndMessageYesNo,
        TitleAndMessageClose,
        MessageYesNo,
        MessageClose
    }

    /// <summary>
    /// アラートボタンのタイプ.
    /// </summary>
    public enum AlertButtonType
    {
        Negative, // 否定形ボタン.
        Neutral,  // 通常ボタン.
        Positive  // 肯定形ボタン.
    }

    public struct ButtonSetting
    {
        public AlertButtonType buttonType;
        public string label;
        public ButtonSetting(AlertButtonType buttonType, string label)
        {
            this.buttonType = buttonType;
            this.label = label;
        }
    }

    [SerializeField]
    private GameObject _titleAndMessagePopupPrefab;
    [SerializeField]
    private GameObject _messagePopupPrefab;

    private static string DEFAULT_POSITIVE_BUTTON_LABEL = "はい";
    private static string DEFAULT_NEGATIVE_BUTTON_LABEL = "いいえ";
    private static string DEFAULT_NEUTRAL_BUTTON_LABEL = "OK";

    /// <summary>
    /// 通用のポップアップです
    /// </summary>
    /// <param name="type"></param>
    /// <param name="title"></param>
    /// <param name="message"></param>
    /// <param name="buttonSettings"></param>
    /// <returns></returns>
    public async UniTask<AlertButtonType> ShowAlertAsync(AlertType type,
                                                    string title = "",
                                                    string message = "",
                                                    List<ButtonSetting> buttonSettings = null,
                                                    bool dontClosePopup = false,
                                                    int backKeyButtonIndex = -1)
    {
        if (type == AlertType.TitleAndMessageClose || type == AlertType.TitleAndMessageYesNo)
            return await CreateAndShowTitleAndMessageAlert(type, title, message, buttonSettings, dontClosePopup, backKeyButtonIndex);
        else
            return await CreateAndShowMessageAlert(type, message, buttonSettings, dontClosePopup, backKeyButtonIndex);
    }

    /// <summary>
    /// 確認用のalert
    /// </summary>
    /// <param name="title"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    public async UniTask<Unit> ShowConfirmAlertAsync(string title, string message, bool dontClosePopup = false, int backKeyButtonIndex = -1)
    {
        var settings = new List<ButtonSetting>();
        settings.Add(new ButtonSetting(AlertButtonType.Neutral, DEFAULT_NEUTRAL_BUTTON_LABEL));
        await ShowAlertAsync(AlertType.TitleAndMessageClose, title, message, settings, dontClosePopup, backKeyButtonIndex);
        return Unit.Default;
    }

    /// <summary>
    /// Yes Noだけのalert
    /// </summary>
    /// <param name="title"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    public async UniTask<bool> ShowYesNoAlertAsync(string title, string message, int backKeyButtonIndex = -1)
    {
        var result = await ShowAlertAsync(AlertType.TitleAndMessageYesNo, title, message, backKeyButtonIndex: backKeyButtonIndex);
        return result == AlertButtonType.Positive;
    }

    /// <summary>
    /// 通常コールバック登録バージョンの通用ポップアップです
    /// </summary>
    /// <param name="type"></param>
    /// <param name="title"></param>
    /// <param name="message"></param>
    /// <param name="buttonSettings"></param>
    /// <param name="callbackYes"></param>
    /// <param name="callbackNo"></param>
    public void ShowAlert(AlertType type,
                                    string title = "",
                                    string message = "",
                                    List<ButtonSetting> buttonSettings = null,
                                    UnityAction callbackYes = null,
                                    UnityAction callbackNo = null,
                                    int backKeyButtonIndex = -1)
    {
        ShowAlertAsync(type, title, message, buttonSettings, backKeyButtonIndex: backKeyButtonIndex).ToObservable()
            .Subscribe(result =>
            {
                switch (result)
                {
                    case AlertButtonType.Negative:
                        callbackNo?.Invoke();
                        break;
                    case AlertButtonType.Neutral:
                        callbackYes?.Invoke();
                        break;
                    case AlertButtonType.Positive:
                        callbackYes?.Invoke();
                        break;
                    default:
                        break;
                }
            });
    }

    private async UniTask<AlertButtonType> CreateAndShowTitleAndMessageAlert(AlertType type, string title, string message, List<ButtonSetting> buttonsettings = null, bool dontClosePopup = false, int backKeyButtonIndex = -1)
    {
        CommonPopup script;
        if (_permanentPopupList.ContainsKey(_titleAndMessagePopupPrefab.name))
        {
            script = _permanentPopupList[_titleAndMessagePopupPrefab.name] as CommonPopup;
            script.transform.SetParent(_root);
        }
        else
        {
            var go = Instantiate(_titleAndMessagePopupPrefab, _root);
            script = go.GetComponent<CommonPopup>();
            _permanentPopupList[_titleAndMessagePopupPrefab.name] = script;
        }
        if (_currentPopup != null)
        {
            _currentPopup.transform.SetParent(_inActiveRoot);
            _popupBehind.Push(_currentPopup);
        }
        _currentPopup = script;
        script.SetData(title, message);
        if (buttonsettings == null)
        {
            script.SetButton(MakeDefaultButtonSettings(type), backKeyButtonIndex);
        }
        else
        {
            script.SetButton(buttonsettings, backKeyButtonIndex);
        }

        int result = await script.OpenAsync();
        if (!dontClosePopup)
            await HideAsync();
        return (AlertButtonType)result;
    }

    private async UniTask<AlertButtonType> CreateAndShowMessageAlert(AlertType type, string message, List<ButtonSetting> buttonsettings = null, bool dontClosePopup = false, int backKeyButtonIndex = -1)
    {
        CommonPopup script;
        if (_permanentPopupList.ContainsKey(_messagePopupPrefab.name))
        {
            script = _permanentPopupList[_messagePopupPrefab.name] as CommonPopup;
            script.transform.SetParent(_root);
        }
        else
        {
            var go = Instantiate(_messagePopupPrefab, _root);
            script = go.GetComponent<CommonPopup>();
            _permanentPopupList[_titleAndMessagePopupPrefab.name] = script;
        }
        if (_currentPopup != null)
        {
            _currentPopup.transform.SetParent(_inActiveRoot);
            _popupBehind.Push(_currentPopup);
        }
        _currentPopup = script;
        script.SetData(null, message);
        if (buttonsettings == null)
        {
            script.SetButton(MakeDefaultButtonSettings(type), backKeyButtonIndex);
        }
        else
        {
            script.SetButton(buttonsettings, backKeyButtonIndex);
        }

        int result = await script.OpenAsync();
        if (!dontClosePopup)
            await HideAsync();
        return (AlertButtonType)result;
    }



    private List<ButtonSetting> MakeDefaultButtonSettings(AlertType type)
    {
        var settings = new List<ButtonSetting>();
        switch (type)
        {
            case AlertType.TitleAndMessageYesNo:
            case AlertType.MessageYesNo:
                settings.Add(new ButtonSetting(AlertButtonType.Positive, DEFAULT_POSITIVE_BUTTON_LABEL));
                settings.Add(new ButtonSetting(AlertButtonType.Negative, DEFAULT_NEGATIVE_BUTTON_LABEL));
                break;
            case AlertType.TitleAndMessageClose:
            case AlertType.MessageClose:
                settings.Add(new ButtonSetting(AlertButtonType.Neutral, DEFAULT_NEUTRAL_BUTTON_LABEL));
                break;
            default:
                settings.Add(new ButtonSetting(AlertButtonType.Neutral, DEFAULT_NEUTRAL_BUTTON_LABEL));
                break;
        }
        return settings;
    }
}

