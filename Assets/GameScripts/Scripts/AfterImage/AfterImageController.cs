using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterImageController : AfterImageControllerBase
{
    [SerializeField, Header("残像の発生元となるオブジェクト")]
    private Transform _originalTransform = null;
    protected override void SetupParam()
    {
        _param = new SimpleAfterImageParam(_originalTransform);
    }
}
