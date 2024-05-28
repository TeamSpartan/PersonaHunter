using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

public class IntroMovie : MonoBehaviour
{
    [SerializeField, Range(0f, 30f, order = 100)]
    private float _fadeInOutTime;

    [SerializeField] private GameObject _fadeInOutPanel;

    [SerializeField] private TextMeshProUGUI _textBox;
    
    [SerializeField] private List<string> _text;
    
    private bool _isFading;
    private int _cIndex;

    private void Start()
    {
        _cIndex = 0;
        _textBox.text = _text[_cIndex];
    }

    /// <summary>
    /// フェードイン・アウトを行う
    /// </summary>
    public void StartFadeInOut()
    {
        if (_isFading) return;

        var fade_in = _fadeInOutPanel.GetComponent<Image>().DOFade(1f, _fadeInOutTime / 2);
        fade_in.OnComplete(() => { _fadeInOutPanel.GetComponent<Image>().DOFade(0f, _fadeInOutTime / 2); });
    }

    public void NextText()
    {
        _cIndex = _cIndex < _text.Count - 1 ? _cIndex + 1 : 0;
        _textBox.text = _text[_cIndex];
    }
}
