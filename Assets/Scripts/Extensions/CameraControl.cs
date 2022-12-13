using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class Bounder
{
    public float maxX;
    public float minX;

    public float maxY;
    public float minY;

    public Bounder(float maxX, float minX, float maxY, float minY)
    {
        this.maxX = maxX;
        this.minX = minX;
        this.maxY = maxY;
        this.minY = minY;
    }
}

public class CameraControl : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private Slider slider;

    private float maxSizeBase = 6.2f;
    private float minSizeBase = 3f;

    // x 6.7 4.3
    //y -9 -1.8 


    //x 17 4.4
    //- 3.3 3.3
    private Bounder isoBounder = new Bounder(17f, 4.4f, 3.3f, -3.3f);
    private Bounder baseBounder = new Bounder(6.7f, 4.3f, -1.8f, -9f);

    public void Start()
    {
        this.slider.value = 1;
    }
    public void OnSliderChange()
    {
        float silderVal = slider.value;
        this.cam.orthographicSize = silderVal * (maxSizeBase - minSizeBase) + minSizeBase;
    }

    public void OnClickReset()
    {
        this.slider.value = 1;
        switch (GridManager.Instance.CurMapCrd)
        {
            case EnMapCrd.Base:
                this.cam.transform.position = new Vector3(5.5f, -5.5f, -10);
                break;
            case EnMapCrd.Iso:
                this.cam.transform.position = new Vector3(11f, 0f, -10);
                break;
            default:
                break;
        }
    }

    private Vector3 Origin;
    private Vector3 Diference;
    private bool Drag = false;

    void LateUpdate()
    {
        if (Input.GetMouseButton(1))
        {
            Diference = (cam.ScreenToWorldPoint(Input.mousePosition)) - cam.transform.position;
            if (Drag == false)
            {
                Drag = true;
                Origin = cam.ScreenToWorldPoint(Input.mousePosition);
            }
        }
        else
        {
            Drag = false;
        }
        if (Drag == true)
        {
            cam.transform.position = Origin - Diference;

            var bounder = GridManager.Instance.CurMapCrd == EnMapCrd.Base ? baseBounder : isoBounder;

            //clamp
            cam.transform.position = new Vector3    //if 'Player' crossed the movement borders, returning him back 
            (
            Mathf.Clamp(cam.transform.position.x, bounder.minX, bounder.maxX),
            Mathf.Clamp(cam.transform.position.y, bounder.minY, bounder.maxY),
            -10
            );
        }
    }
}
    