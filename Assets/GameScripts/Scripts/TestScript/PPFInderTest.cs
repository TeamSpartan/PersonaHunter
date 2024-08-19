using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PPFInderTest : MonoBehaviour
{
    private Volume _volume;
    DifferenceOfGaussian _differenceOfGaussian ;
    
    private void Start()
    {
        _volume = GameObject.FindObjectOfType<Volume>();
        _volume.profile.TryGet(out _differenceOfGaussian);
    }

    private void Update()
    {
        _differenceOfGaussian.center.Override(new Vector2(0.5f,0.5f));
    }
}
