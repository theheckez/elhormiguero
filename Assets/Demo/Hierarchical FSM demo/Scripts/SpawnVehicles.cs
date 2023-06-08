using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnVehicles : MonoBehaviour {

    #region variables

    [SerializeField] private GameObject[] vehicles = new GameObject[8];

    #endregion variables

    // Start is called before the first frame update
    private void Start()
    {
        InvokeRepeating("SpawnVehicle", 2, 4);
    }

    private void SpawnVehicle()
    {
        int vehicleIndex = Random.Range(0, vehicles.Length);
        Instantiate(vehicles[vehicleIndex], transform.position, transform.rotation);
    }
}