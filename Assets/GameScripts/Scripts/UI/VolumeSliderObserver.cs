using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class VolumeSliderObserver : MonoBehaviour
{
    [FormerlySerializedAs("_spriteRenderer")] [SerializeField] private Image _image;
    
    [SerializeField] private Sprite _iconWhenMuted;
    
    [SerializeField] private Sprite _iconWhenNotMuted;

    private Slider _slider;

    private void Start()
    {
        if (TryGetComponent<Slider>(out var slider))
        {
            _slider = slider;
            _slider.onValueChanged.AddListener(OnChangedValue);
        }
    }

    private void OnChangedValue(float val)
    {
        _image.sprite = val == _slider.minValue ? _iconWhenMuted : _iconWhenNotMuted;
    }
}
