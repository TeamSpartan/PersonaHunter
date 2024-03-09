using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class FocusSine : MonoBehaviour
{
    //used for sinewave
    public LineRenderer myLineRenderer;
    public int points;
    public float amplitude = 1;
    public float sinewidth = 1;
    public float wavespeed = 1;

    public FocusGuageController FocusGuageController;
    private float amplitudemultiplier;

    //used to convert sinwave to canvas



    // Start is called before the first frame update
    void Start()
    {
        myLineRenderer = GetComponent<LineRenderer>();
    }

    void Draw()
    {
        float xStart = 0;
        float Tau = 2 * Mathf.PI;
        float xFinish = Tau;

        myLineRenderer.positionCount = points;
        for(int currentPoint = 0; currentPoint < points; currentPoint++)
        {
            float progress = (float)currentPoint / (points - 1);
            float x = Mathf.Lerp(xStart, xFinish, progress);
            float y = (amplitudemultiplier*amplitude)*Mathf.Sin(sinewidth*(x+Time.timeSinceLevelLoad*wavespeed));
            myLineRenderer.SetPosition(currentPoint, new Vector3(x, y, 0));
        }
    }

    // Update is called once per frame
    void Update()
    {
        amplitudemultiplier = 1 - FocusGuageController.FocusGuageValue;
        gameObject.GetComponent<LineRenderer>().startWidth = 2.5f - FocusGuageController.FocusGuageValue;
        gameObject.GetComponent<LineRenderer>().endWidth = 2.5f - FocusGuageController.FocusGuageValue;

        Draw();
    }
}
