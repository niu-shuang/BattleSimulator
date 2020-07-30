using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System;
using Cysharp.Threading.Tasks;
using UnityEngine.Events;
using UIAnimation;

public class PopupBase : MonoBehaviour
{
    [SerializeField]
    protected List<Button> buttons;

    public Subject<int> state;
    [SerializeField]
    protected UITweenSequence _animationSequence;
    protected int _backKeyButtonIndex = -1;

    public bool canUseBackKey { get; protected set; }

    /// <summary>
    /// ポップアップを開く、結果をobserve
    /// </summary>
    /// <returns></returns>
    public IObservable<int> OpenAsObservable()
    {
        state = new Subject<int>();
        gameObject.SetActive(true);
        Observable.NextFrame()
            .Subscribe(_ => OnAfterOpenPopup());
        /*
        _animationSequence.Play(() =>
        {
            OnAfterOpenPopup();
        }).Forget();*/
        return state.LastOrDefault();
    }

    /// <summary>
    /// ポップアップを開くasync版
    /// </summary>
    /// <returns></returns>
    public async UniTask<int> OpenAsync()
    {
        return await OpenAsObservable();
    }

    /// <summary>
    /// ポップアップが開いた時に呼ばれる.
    /// </summary>
    protected virtual void OnAfterOpenPopup() { }

    /// <summary>
    /// ポップアップを閉じる
    /// </summary>
    /// <param name="callback"></param>
    public void ClosePopup(UnityAction callback)
    {
        OnBeforeClosePopup();
        Observable.NextFrame()
            .Subscribe(_ =>
            {
                gameObject.SetActive(false);
                callback?.Invoke();
            });
        /*
        _animationSequence.PlayReverse(() =>
        {
            
        }).Forget();*/
    }

    /// <summary>
    /// ポップアップのUIを変えずにsubjectを再発行
    /// </summary>
    /// <returns></returns>
    public IObservable<int> ReCreateObserver()
    {
        state = new Subject<int>();
        return state.LastOrDefault();
    }

    /// <summary>
    /// ポップアップを閉じるasync版
    /// </summary>
    /// <returns></returns>
    public async UniTask<Unit> ClosePopupAsync()
    {
        OnBeforeClosePopup();
        await _animationSequence.PlayReverse();
        gameObject.SetActive(false);
        return Unit.Default;
    }

    /// <summary>
    /// ポップアップが閉じる直前に呼ばれる.
    /// </summary>
    protected virtual void OnBeforeClosePopup() { }

    public void OnClickButton(int index)
    {
        state.OnNext(index);
        state.OnCompleted();
        state.Dispose();
        state = null;
    }

    /// <summary>
    /// androidのバックキーが押下した時
    /// </summary>
    public virtual void OnPressBackKey()
    {
        if (canUseBackKey)
        {
            if (_backKeyButtonIndex > -1)
            {
                OnClickButton(_backKeyButtonIndex);
            }
            else
            {
                OnClickButton(0);
            }
        }
    }

    /// <summary>
    /// ボタンのラベルを手動で設定するメソッド
    /// </summary>
    /// <param name="labels"></param>
    public void SetLabel(List<string> labels)
    {
        int count = labels.Count;
        for (int i = 0; i < count; i++)
        {
            if (buttons.Count > i)
            {
                buttons[i].GetComponentInChildren<Text>().text = labels[i];
            }
        }
    }

    /// <summary>
    /// 削除される前にもしSubjectが存在する場合OnCompleteとDisposeする
    /// </summary>
    protected virtual void OnDestroy()
    {
        if (state != null)
        {
            state.Dispose();
        }
    }
}