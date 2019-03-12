using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneModel : MonoBehaviour
{

    public double cWing;
    public double alpha;
    public double ro;
    public double N;
    public double sWing;
    public double cPlane;
    public double sPlane;
    public double q;

    private const double g = 9.81;

    public double mPlane;
    public double mFuel;

    private double initialFuel;

    public double InitialFuel
    {
        get
        {
            return initialFuel;
        }
        set
        {
            initialFuel = value;
        }
    }

    private double _x = 0;
    private double _y = 0;

    public double x
    {
        get;
        set;
    }
    public double y
    {
        get;
        set;
    }

    private double v = 0;
    private double _vx = 0;
    private double _vy = 0;

    public double vx
    {
        get;
        set;
    }
    public double vy
    {
        get;
        set;
    }

    private bool _crashed = false;
    public bool crashed
    {
        get;
        set;
    }

    // Start is called before the first frame update
    void Start()
    {
        initialFuel = mFuel;
    }

    public void MovePlane(float angleSlider, float powerSlider, bool engineState)
    {
        ro = 1.2255 * Mathf.Pow((float)(1 - 6.5 * (y / 1000) / 288), 4.255f);

        double dt = Time.deltaTime;

        double c = alpha * cWing * angleSlider;
        double FY = c * ro * vx * vx * sWing / 2;
        double FYm = (mPlane + mFuel) * g;
        double FX = 0;
        if (mFuel > 0 && engineState)
            FX = N * powerSlider * (ro / 1.2255);
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
            if (vy < -5)
            {
                crashed = true;
            }

            y = 0;
            vy = 0;
        }

        double A = dx * FX;
        double dm = (A / 0.45) / q;
        mFuel -= dm;
        if (mFuel < 0)
            mFuel = 0;

        
        Debug.Log("X: " + x + " Y: " + y + " FX: " + dFX + " FY: " + dFY + " VX: " + (vx * 1.94384) + " VY: " + vy + " Ro: " + ro + " Fuel: " + mFuel);
    }
}
