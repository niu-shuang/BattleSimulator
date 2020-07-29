using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using J;
using UniRx;
using Cysharp.Threading.Tasks;
using UnityEngine.Events;

public partial class PopupManager : SingletonMonoBehaviour<PopupManager>
{
    [SerializeField]
    private RectTransform _root;
    [SerializeField]
    private RectTransform _inActiveRoot;

    private Dictionary<string, PopupBase> _popupList;
    private Dictionary<string, PopupBase> _permanentPopupList;

    private PopupBase _currentPopup;

    private Stack<PopupBase> _popupBehind;

    public bool existActivePopup => _currentPopup != null;

    public bool canUseBackKey => _currentPopup != null ? _currentPopup.canUseBackKey : false;

    public void Init()
    {
        _popupList = new Dictionary<string, PopupBase>();
        _permanentPopupList = new Dictionary<string, PopupBase>();
        _currentPopup = null;
        _popupBehind = new Stack<PopupBase>();
    }

    /// <summary>
    /// ポップアップを作成してコンポ念とを返すだけ
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="prototypeComponent"></param>
    /// <returns></returns>
    public T Create<T>(T prototypeComponent) where T : PopupBase
    {
        if (_currentPopup != null && _currentPopup != prototypeComponent)
        {
            _currentPopup.transform.SetParent(_inActiveRoot);
            _popupBehind.Push(_currentPopup);
        }
        var typeName = typeof(T).ToString();
        if (!_popupList.ContainsKey(typeName))
        {
            var go = Instantiate(prototypeComponent.gameObject, _root);
            var component = go.GetComponent<T>();
            _popupList.Add(typeName, component);
            return component;

        }
        else
        {
            if (_popupList[typeName].transform.parent != _root) _popupList[typeName].transform.SetParent(_root);
            return _popupList[typeName] as T;
        }
    }

    /// <summary>
    /// ボタンの番号をコールバックに渡す
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="prototypeComponent"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    public T CreateAndShow<T>(T prototypeComponent, UnityAction<int> callback) where T : PopupBase
    {
        T component = Create<T>(prototypeComponent);
        _currentPopup = component;
        component.OpenAsObservable()
            .Subscribe(index => callback?.Invoke(index));
        return component;
    }

    public T CreateAndShow<T>(T prototypeComponent, UnityAction<bool> callback) where T : PopupBase
    {
        return CreateAndShow(prototypeComponent, (int index) =>
        {
            callback?.Invoke(index > 0);
        });
    }

    public T CreateAndShow<T>(T prototypeComponent, UnityAction callback) where T : PopupBase
    {
        return CreateAndShow(prototypeComponent, (int index) => callback?.Invoke());
    }

    public GameObject Create(GameObject prefab)
    {
        if (_currentPopup != null && !_popupList.ContainsKey(prefab.name))
        {
            _currentPopup.transform.SetParent(_inActiveRoot);
            _popupBehind.Push(_currentPopup);
        }

        if (_popupList.ContainsKey(prefab.name))
        {
            if (_popupList[prefab.name].transform.parent != _root)
            {
                _popupList[prefab.name].transform.SetParent(_root);
            }
            return _popupList[prefab.name].gameObject;
        }
        else
        {
            var go = Instantiate(prefab, _root);
            var script = go.GetComponent<PopupBase>();
            _popupList[prefab.name] = script;
            return go;

        }
    }

    public T Create<T>(GameObject prefab) where T : PopupBase
    {
        Create(prefab);
        return _popupList[prefab.name] as T;
    }

    /// <summary>
    /// ポップアップを表示してボタンの番号を返すまで待つ
    /// </summary>
    /// <param name="popupType"></param>
    /// <returns></returns>
    public async UniTask<int> ShowAsync(System.Type popupType)
    {
        var popup = _popupList[popupType.ToString()];
        _currentPopup = popup;
        if (popup.transform.parent != _root) popup.transform.SetParent(_root);
        return await popup.OpenAsync();
    }

    /// <summary>
    /// ポップアップを表示してボタンの番号を返すまで待つ
    /// </summary>
    /// <param name="popupType"></param>
    /// <returns></returns>
    public async UniTask<bool> ShowAsyncYesOrNo(System.Type popupType)
    {
        var popup = _popupList[popupType.ToString()];
        _currentPopup = popup;
        if (popup.transform.parent != _root) popup.transform.SetParent(_root);
        int index = await popup.OpenAsync();
        return index > 0;
    }

    /// <summary>
    /// ポップアップを閉じてcallbackを実行
    /// </summary>
    /// <param name="callback"></param>
    public void Hide(UnityAction callback)
    {
        if (_currentPopup != null)
        {
            _currentPopup.ClosePopup(() =>
            {
                callback?.Invoke();
                _currentPopup.transform.SetParent(_inActiveRoot);
                _currentPopup = null;
                if (_popupBehind.Count > 0)
                {
                    _currentPopup = _popupBehind.Pop();
                    _currentPopup.transform.SetParent(_root);
                }
            });
        }
    }

    /// <summary>
    /// ポップアップを閉じるasync版
    /// </summary>
    /// <returns></returns>
    public async UniTask<Unit> HideAsync()
    {
        if (_currentPopup != null)
            await _currentPopup.ClosePopupAsync();
        _currentPopup = null;
        if (_popupBehind.Count > 0)
        {
            _currentPopup = _popupBehind.Pop();
            if (_currentPopup != null)
            {
                //シーン切り替え・プレイボタン押された時DestroyされたやつをAccessしようとするケースがあるので保険として入れる...
                if (_currentPopup.transform != null)
                    _currentPopup.transform.SetParent(_root);
            }
        }
        return Unit.Default;
    }

    /// <summary>
    /// ポップアップ閉じてから削除
    /// </summary>
    /// <param name="callback"></param>
    public void HideAndDestroy(System.Type popupType, UnityAction callback)
    {
        string key = popupType.Name;
        if (_permanentPopupList.ContainsKey(key))
        {
            Debug.LogError("Can not Destroy popup in permanent list!");
            callback?.Invoke();
            return;
        }
        if (_currentPopup != null)
        {
            _currentPopup.ClosePopup(() =>
            {
                callback?.Invoke();
                _popupList.Remove(key);
                Destroy(_currentPopup.gameObject);
                _currentPopup = null;
                if (_popupBehind.Count > 0)
                {
                    _currentPopup = _popupBehind.Pop();
                    if (_currentPopup != null)
                    {
                            //シーン切り替え・プレイボタン押された時DestroyされたやつをAccessしようとするケースがあるので保険として入れる...
                            if (_currentPopup.transform != null)
                            _currentPopup.transform.SetParent(_root);
                    }
                }
            });
        }

    }

    /// <summary>
    /// ポップアップ閉じてから削除
    /// </summary>
    /// <param name="callback"></param>
    public async UniTask<Unit> HideAndDestroyAsync(System.Type popupType)
    {
        string key = popupType.ToString();
        var popupToDestory = _currentPopup;
        await HideAsync();
        if (_permanentPopupList.ContainsKey(key))
        {
            Debug.LogError("Can not Destroy popup in permanent list!");
            return Unit.Default;
        }
        _popupList.Remove(key);
        Destroy(popupToDestory.gameObject);
        _currentPopup = null;
        if (_popupBehind.Count > 0)
        {
            _currentPopup = _popupBehind.Pop();
            _currentPopup.transform.SetParent(_root);
        }
        return Unit.Default;
    }

    public void OnPressBackkey()
    {
        _currentPopup.OnPressBackKey();
    }

    /// <summary>
    /// Sceneを閉じる時ポップアップをクリア
    /// </summary>
    public void OnEndScene()
    {
        if(_popupList != null)
        {
            foreach (var item in _popupList.Values)
            {
                Destroy(item.gameObject);
            }
            _currentPopup = null;
            _popupList.Clear();
        }
    }

    protected override void SingletonOnDestroy()
    {
        base.SingletonOnDestroy();
        OnEndScene();
    }
}
