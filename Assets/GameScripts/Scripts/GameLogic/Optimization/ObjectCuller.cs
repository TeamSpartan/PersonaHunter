using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectCuller : MonoBehaviour
{
    [SerializeField, Tooltip("この値より遠い場合に見えなくする")]
    private float _distanceToImbisible;

    private Transform _target;

    private MeshRenderer _renderer;

    private bool _logicIsRunning = true;

    void Start()
    {
        if (TryGetComponent<MeshRenderer>(out var c) is false)
        {
            _renderer = GetComponentInChildren<MeshRenderer>();
        }
        else
        {
            _renderer = c;
        }

        if (_target is not null)
        {
            _target = GameObject.Find("Player").transform;
            StartCoroutine(nameof(Task));
        }
    }

    private void OnApplicationQuit()
    {
        _logicIsRunning = false;
    }

    private IEnumerator Task()
    {
        while (true)
        {
            yield return null;
            if (Vector3.Distance(transform.position, _target.position) > _distanceToImbisible)
            {
                _renderer.enabled = false;
            }
            else
            {
                _renderer.enabled = true;
            }

            yield return null;
        }
    }
}
