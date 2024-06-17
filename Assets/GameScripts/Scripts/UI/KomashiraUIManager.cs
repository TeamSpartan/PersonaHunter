using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// インゲームのUIに対してアタッチをする
/// </summary>
public class KomashiraUIManager : MonoBehaviour
{
    private List<GameObject> _hpBars = new List<GameObject>();
    private List<Slider> _sliders = new List<Slider>();
    private GameObject _komashiraUITemplate;

    /// <summary>
    /// HPバーを追加して、このクラス内のHPバーのインデックスを返す
    /// </summary>
    public int AddHPBar(float maxHp)
    {
        Debug.Log($"add hp bar");
        if (_komashiraUITemplate is null)
        {
            _komashiraUITemplate = Resources.Load<GameObject>("Prefabs/UI/KomashiraHP");
        }

        var bar = GameObject.Instantiate(_komashiraUITemplate, transform);
        if (bar.TryGetComponent<Slider>(out var c))
        {
            c.maxValue = maxHp;
            _sliders.Add(c);
        }

        _hpBars.Add(bar);
        return _hpBars.Count - 1;
    }

    public void UpdateHPBar(int index, Vector3 pos, float val)
    {
        if (_hpBars[index] is not null)
        {
            if (pos.z > 0)
            {
                _hpBars[index].SetActive(true);
                _hpBars[index].transform.position = pos;
                _sliders[index].value = val;
            }
            else
            {
                _hpBars[index].SetActive(false);
            }
        }
    }

    public void DestroyHpBar(int index)
    {
        var obj = _hpBars[index];
        _hpBars.RemoveAt(index);
        GameObject.Destroy(obj);
    }
}
