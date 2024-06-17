using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// インゲームのUIに対してアタッチをする
/// </summary>
public class InGameUIManager : MonoBehaviour
{
    private List<GameObject> _hpBars = new List<GameObject>();
    private GameObject _komashiraUITemplate;

    /// <summary>
    /// HPバーを追加して、このクラス内のHPバーのインデックスを返す
    /// </summary>
    public int AddHPBar()
    {
        if (_komashiraUITemplate is null)
        {
            _komashiraUITemplate = Resources.Load<GameObject>("Prefabs/UI/KomashiraHP");
        }

        var bar = GameObject.Instantiate(_komashiraUITemplate, transform);
        _hpBars.Add(bar);
        return _hpBars.Count - 1;
    }

    public void UpdateHPBarPos(int index, Vector3 pos)
    {
        if (_hpBars[index] is not null)
        {
            if (pos.z > 0)
            {
                _hpBars[index].SetActive(true);
                _hpBars[index].transform.position = pos;
            }
            else
            {
                _hpBars[index].SetActive(false);
            }
        }
    }
}
