using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider), typeof(Rigidbody))]
public class OccluderController : MonoBehaviour
{
    [SerializeField, Tooltip("どのくらい透明にするか"), Range(0f, 1f)] float _transparency = 0.2f;

    private void OnTriggerEnter(Collider other)
    {
        OccludeeController occuluController = other.gameObject.GetComponent<OccludeeController>();
        if(occuluController)
        {
            occuluController.ChangeAlpha(_transparency);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        OccludeeController occuluController = other.gameObject.GetComponent<OccludeeController>();
        if(occuluController)
        {
            occuluController.ChangeAlphaOriginal();
        }
    }
}
