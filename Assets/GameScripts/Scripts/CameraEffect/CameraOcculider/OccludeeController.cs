using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OccludeeController : MonoBehaviour
{
    [SerializeField, Tooltip("1フレームにどれだけa値を変化させる")]
    float _step = 0.01f;
    /// <summary>変化させるa値 </summary>
    float _targetAlpha = 1.0f;
    /// <summary>初期のa値 </summary>
    float _originAlpha = 1.0f;
    Material _material;
 
    void Start()
    {
        Renderer r = GetComponent<Renderer>();
        if(r)
        {
            _material = r.material;
        }

        if(_material)
        {
            _originAlpha = _material.color.a;
        }
    }

    /// <summary>
    /// alpha値を初期値に戻す
    /// </summary>
    public void ChangeAlphaOriginal()
    {
        ChangeAlpha(_originAlpha);
    }

    /// <summary>
    /// alpha値を変更
    /// </summary>
    /// <param name="targetAlpha">変更したいalpha値</param>
    public void ChangeAlpha(float targetAlpha)
    {
        _targetAlpha = targetAlpha;
        if( _material)
        {
            StartCoroutine(ChangeAlpha());
        }
    }

    /// <summary>
    /// alpha値を徐々に変更する
    /// </summary>
    /// <returns></returns>
    IEnumerator ChangeAlpha()
    {
        if(_material.color.a > _targetAlpha)
        {
            while(_material.color.a > _targetAlpha)
            {
                Color c = _material.color;
                c.a -= _step;
                _material.color = c;
                yield return new WaitForEndOfFrame();
            }
        }
        else
        {
            while(_material.color.a < _targetAlpha)
            {
                Color c = _material.color;
                c.a += _step;
                _material.color = c;
                yield return new WaitForEndOfFrame();
            }
        }
    }
}
