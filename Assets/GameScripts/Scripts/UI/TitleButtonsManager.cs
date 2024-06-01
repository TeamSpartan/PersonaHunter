using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

/// <summary>
/// タイトル画面のボタンの管理クラス
/// </summary>
public class TitleButtonsManager : MonoBehaviour
{
    [SerializeField] private List<Button> _buttons;
    
    private BaseInputModule _baseInputModule;
    private int _selectedIndex = 0;

    private void Start()
    {
        _baseInputModule = GameObject.FindFirstObjectByType<BaseInputModule>();
    }

    private void Update()
    {
        
    }
}
