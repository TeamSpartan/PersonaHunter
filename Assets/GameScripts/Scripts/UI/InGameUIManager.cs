using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;


/// <summary>
/// インゲームのUIに対してアタッチをする
/// </summary>
public class InGameUIManager : MonoBehaviour
{
    private List<GameObject> _hpBars = new List<GameObject>();
    private List<Slider> _sliders = new List<Slider>();
    private GameObject _barUITemplate;

    public enum HPBarTemplate
    {
        Komashira,
    }

    /// <summary>
    /// HPバーを追加して、このクラス内のHPバーのインデックスを返す
    /// </summary>
    public int AddHPBar(float maxHp, HPBarTemplate template)
    {
        switch (template)
        {
            case HPBarTemplate.Komashira:
            {
                if (_barUITemplate is null)
                {
                    _barUITemplate = Resources.Load<GameObject>("Prefabs/UI/KomashiraHP");
                }

                break;
            }
        }

        var bar = GameObject.Instantiate(_barUITemplate, transform);
        bar.name += _hpBars.Count.ToString();
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
        _hpBars.Remove(obj);
        GameObject.Destroy(obj);
    }

    public void PunchGuage(int index)
    {
        _sliders[index].transform.DOShakePosition(0.5f, 60f, 25);
    }
}
