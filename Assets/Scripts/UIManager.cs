using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChartAndGraph;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GraphChart Graph;

    public Image airspeedArrow;
    public Image vertspeedArrow;
    public Image fuelArrow;
    public Image altArrow;
    public Image altArrowSmall;
    public Image aviaHorizont;

    public Text flewText;
    public Text goalText;

    public Toggle graphState;

    // Start is called before the first frame update
    void Start()
    {
        if (Graph == null)
            return;
        Graph.DataSource.StartBatch();
        Graph.DataSource.ClearCategory("Player 1");
        Graph.DataSource.EndBatch();
        Graph.DataSource.AddPointToCategoryRealtime("Player 1", -1000, 1, 0);
        Graph.DataSource.AddPointToCategoryRealtime("Player 1", -999, 0, 0);
    }

    public void UpdateGauges(double x, double y, double dt, double vx, double vy, double mFuel, double initialFuel, double finishPoint)
    {
        Graph.DataSource.AddPointToCategoryRealtime("Player 1", x, y, dt);

        airspeedArrow.transform.rotation = Quaternion.Euler(0, 0, -((float)(vx * 1.94384) * 360f / 600f));
        vertspeedArrow.transform.rotation = Quaternion.Euler(0, 0, -((float)(-90f + vy * 360f / 300f)));
        fuelArrow.transform.rotation = Quaternion.Euler(0, 0, -((float)(-144f + (mFuel / initialFuel) * 288f)));
        altArrowSmall.transform.rotation = Quaternion.Euler(0, 0, -((float)(y / 1000) * 360f / 20f));
        altArrow.transform.rotation = Quaternion.Euler(0, 0, -((float)(y % 1000 / 100) * 360f / 10f));
        aviaHorizont.rectTransform.localPosition = new Vector3(0, (-Mathf.Atan2((float)vy, (float)vx) / Mathf.PI * 180f) * 1.6f, 0);

        flewText.text = "Flew: " + Mathf.Floor((float)(0 + x)) + "m";
        goalText.text = Mathf.Floor((float)(finishPoint - x)) + "m to goal";

        Graph.gameObject.SetActive(graphState.isOn);
    }

}
