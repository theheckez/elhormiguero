using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyVehicles : MonoBehaviour {

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Car") {
            Destroy(other.gameObject);
        }
    }
}