using UnityEngine;

public class ZoneFocusSine : MonoBehaviour
{
    //used for sinewave
    [SerializeField] int points;
    [SerializeField] float amplitude = 1;
    [SerializeField] float sinewidth = 1;
    [SerializeField] float wavespeed = 1;

    LineRenderer _myLineRenderer;
    ZoneGauge _zoneGauge;
    
    private float _amplitudemultiplier;
    
    // Start is called before the first frame update
    void Start()
    {
        _myLineRenderer = GetComponent<LineRenderer>();
        _zoneGauge = GetComponentInParent<ZoneGauge>();
    }

    void Draw()
    {
        float xStart = 0;
        float Tau = 2 * Mathf.PI;
        float xFinish = Tau;

        _myLineRenderer.positionCount = points;
        for(int currentPoint = 0; currentPoint < points; currentPoint++)
        {
            float progress = (float)currentPoint / (points - 1);
            float x = Mathf.Lerp(xStart, xFinish, progress);
            float y = (_amplitudemultiplier*amplitude)*Mathf.Sin(sinewidth*(x+Time.timeSinceLevelLoad*wavespeed));
            _myLineRenderer.SetPosition(currentPoint, new Vector3(x, y, 0));
        }
    }

    // Update is called once per frame
    void Update()
    {
        _amplitudemultiplier = 1 - Mathf.Clamp( _zoneGauge.GetCurrentZoneGaugeValue / _zoneGauge.GetMaxZoneGaugeValue, 0f,1f);
        _myLineRenderer.startWidth = 2.5f - Mathf.Clamp( _zoneGauge.GetCurrentZoneGaugeValue / _zoneGauge.GetMaxZoneGaugeValue, 0f,1f);
        _myLineRenderer.endWidth = 2.5f - Mathf.Clamp( _zoneGauge.GetCurrentZoneGaugeValue / _zoneGauge.GetMaxZoneGaugeValue, 0f,1f);

        Draw();
    }
}
