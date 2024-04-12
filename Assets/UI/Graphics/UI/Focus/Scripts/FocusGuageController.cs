using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusGuageController : MonoBehaviour
{
    const float FocusGuageMAX = 1;
    const float FocusGuageMIN = 0;
    public float FocusGuageValue;
    public GameObject FocusWaveUI;
    public GameObject FocusGuage;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        FocusGuageValue = Mathf.Clamp(FocusGuageValue, FocusGuageMIN, FocusGuageMAX);
        

        if (FocusGuageValue < FocusGuageMAX)
        {
            FocusWaveUI.SetActive(true); ;
            FocusGuage.SetActive(false);
        }
        else
        {
            FocusWaveUI.SetActive(false);
            FocusGuage.SetActive(true);
        }
    }
}
