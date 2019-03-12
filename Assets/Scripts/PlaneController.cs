using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ChartAndGraph;

public class PlaneController : MonoBehaviour
{

    public double cWing;
    public double alpha;
    public double ro;
    public double N;
    public double sWing;
    public double cPlane;
    public double sPlane;
    public double q;

    const double g = 9.81;

    public double mPlane;
    public double mFuel;

    private double initialFuel;

    public double x = 0;
    private double y = 0;

    private double v = 0;
    private double vx = 0;
    private double vy = 0;

    public GraphChart Graph;

    public Slider powerSlider;
    public Slider angleSlider;

    public Toggle engineState;
    public Toggle graphState;

    public Image airspeedArrow;
    public Image vertspeedArrow;
    public Image fuelArrow;
    public Image altArrow;
    public Image altArrowSmall;
    public Image aviaHorizont;

    public GameObject MainCamera;
    public GameObject crashOverlay;

    public Text flewText;
    public Text goalText;

    public GameObject finishPoint;

    private bool gameOver = false;

    // Start is called before the first frame update
    void Start()
    {
        if (Graph == null)
            return;
        Graph.DataSource.StartBatch(); 
        Graph.DataSource.ClearCategory("Player 1");
        Graph.DataSource.EndBatch();
        Graph.DataSource.AddPointToCategoryRealtime("Player 1", -1000, 1, Time.deltaTime);
        Graph.DataSource.AddPointToCategoryRealtime("Player 1", -999, 0, Time.deltaTime);

        initialFuel = mFuel;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameOver)
            return;

        ro = 1.2255 * Mathf.Pow((float)(1 - 6.5 * (y / 1000) / 288), 4.255f);
        MovePlane();

        engineState.transform.Find("Background").gameObject.SetActive(!engineState.isOn);
        Graph.gameObject.SetActive(graphState.isOn);
    }

    void MovePlane()
    {
        double dt = Time.deltaTime;

        double c = alpha * cWing * angleSlider.value;
        double FY = c * ro * vx * vx * sWing / 2;
        double FYm = (mPlane + mFuel) * g;
        double FX = 0;
        if (mFuel > 0 && engineState.isOn)
            FX = N * powerSlider.value * (ro / 1.2255);
        double FXr = 0.5 * cPlane * ro * sPlane * vx * vx;

        double FYr = 0.5 * ro * (sWing + 210) * vy * vy;

        if (vy < 0)
            FYr *= -1;

        double dFX = FX - FXr;
        double dFY = FY - FYr - FYm;

        double m = mPlane + mFuel;

        double dx = vx * dt + (dFX * dt * dt) / (2 * m + m);

        x += dx;
        y += vy * dt + (dFY * dt * dt) / (2 * m + m);

        vx += (dFX * dt) / m;
        vy += (dFY * dt) / m;

        if (y <= 0)
        {
            if(vy < -5)
            {
                crashOverlay.SetActive(true);
                MainCamera.transform.Find("Explosion").gameObject.SetActive(true);
                gameOver = true;
                return;
            }

            y = 0;
            vy = 0;
        }

        double A = dx * FX;
        double dm = (A / 0.45) / q;
        mFuel -= dm;
        if (mFuel < 0)
            mFuel = 0;


        MainCamera.transform.position = new Vector3(0, (float)y, (float)x);
        MainCamera.transform.rotation = Quaternion.Euler(-Mathf.Atan2((float)vy, (float)vx) / Mathf.PI * 180f, 0, 0);
  
        Graph.DataSource.AddPointToCategoryRealtime("Player 1", x, y, dt);

        airspeedArrow.transform.rotation = Quaternion.Euler(0, 0, -((float)(vx * 1.94384) * 360f / 600f));
        vertspeedArrow.transform.rotation = Quaternion.Euler(0, 0, -((float)(-90f + vy * 360f / 300f)));
        fuelArrow.transform.rotation = Quaternion.Euler(0, 0, -((float)(-144f + (mFuel / initialFuel) * 288f)));
        altArrowSmall.transform.rotation = Quaternion.Euler(0, 0, -((float)(y / 1000) * 360f / 20f));
        altArrow.transform.rotation = Quaternion.Euler(0, 0, -((float)(y % 1000 / 100) * 360f / 10f));
        aviaHorizont.rectTransform.localPosition = new Vector3(0, (-Mathf.Atan2((float)vy, (float)vx) / Mathf.PI * 180f) * 1.6f, 0);

        flewText.text = "Flew: " + Mathf.Floor((float)(0 + x)) + "m";
        goalText.text = Mathf.Floor((float)(finishPoint.transform.position.z - x)) + "m to goal";
        
        Debug.Log("X: " + x + " Y: " + y + " FX: " + dFX + " FY: " + dFY + " VX: " + (vx * 1.94384) + " VY: " + vy + " Ro: " + ro + " Fuel: " + mFuel);
    }
}
