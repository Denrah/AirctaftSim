using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlaneControllerNew : MonoBehaviour
{
    public Slider powerSlider;
    public Slider angleSlider;

    public Toggle engineState;

    public GameObject MainCamera;
    public GameObject crashOverlay;

    public GameObject finishPoint;

    private bool gameOver = false;

    public PlaneModel planeModel;
    public UIManager uiManager;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (gameOver)
            return;

        planeModel.MovePlane(angleSlider.value, powerSlider.value, engineState.isOn);

        if (planeModel.crashed)
        {
                crashOverlay.SetActive(true);
                MainCamera.transform.Find("Explosion").gameObject.SetActive(true);
                gameOver = true;
                return;
        }
        

        uiManager.UpdateGauges(planeModel.x, planeModel.y, Time.deltaTime, planeModel.vx, planeModel.vy, planeModel.mFuel, planeModel.InitialFuel, finishPoint.transform.position.z);

        MainCamera.transform.position = new Vector3(0, (float)planeModel.y, (float)planeModel.x);
        MainCamera.transform.rotation = Quaternion.Euler(-Mathf.Atan2((float)planeModel.vy, (float)planeModel.vx) / Mathf.PI * 180f, 0, 0);

        engineState.transform.Find("Background").gameObject.SetActive(!engineState.isOn);
    }
}
