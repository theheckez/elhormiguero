using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectCar : Perception {

    #region variables

    private Vector3 pointToLook;
    private GameObject radar;
    private Color ligthColor;
    private float carSpeed;

    #endregion variables

    public DetectCar(GameObject radar, Vector3 pointToLook)
    {
        this.radar = radar;
        this.pointToLook = pointToLook;
    }

    public override bool Check()
    {
        Ray ray = new Ray(radar.transform.position, -radar.transform.TransformPoint(pointToLook));

        if(Physics.Raycast(ray, out RaycastHit hit, 50) && hit.collider.tag == "Car") {
            carSpeed = hit.collider.gameObject.GetComponent<VehicleFSM>().GetSpeed();

            if(carSpeed > 20) {
                ligthColor = new Color(1, 0, 0);
                return true;
            }
            else {
                ligthColor = new Color(0, 1, 0);
                return true;
            }
        }

        return false;
    }

    public Color GetLightColor()
    {
        return ligthColor;
    }

    public float GetCarSpeed()
    {
        return carSpeed;
    }
}