using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class Modifier : IDisposable
{
    public struct Item
    {
        public int value;
        public bool isMulti;
    }

    public ReactiveCollection<Item> items;

    public ReactiveProperty<int> finalValue;

    private int baseValue;

    private CompositeDisposable disposable;

    public Modifier(int baseValue)
    {
        items = new ReactiveCollection<Item>();
        finalValue = new ReactiveProperty<int>(baseValue);
        this.baseValue = baseValue;
        disposable = new CompositeDisposable();
        disposable.Add(items.ObserveCountChanged()
            .Subscribe(_=> CalcFinalValue()));
    }

    public void AddItem(Item item)
    {
        items.Add(item);
    }

    public void RemoveItem(Item item)
    {
        items.Remove(item);
    }

    private void CalcFinalValue()
    {
        int tempValue = baseValue;
        List<Item> calcNextRound = new List<Item>();
        foreach(var item in items)
        {
            if (!item.isMulti)
            {
                calcNextRound.Add(item);
            }
            else
            {
                tempValue = (int)(tempValue * item.value / 1000f);
            }
        }
        foreach (var item in calcNextRound)
        {
            tempValue += item.value;
        }
        finalValue.Value = tempValue;
    }

    public void Dispose()
    {
        disposable?.Dispose();
    }
}
