using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestarButtonNoKey : MonoBehaviour {

    #region variables

    [SerializeField] private GameObject key;
    [SerializeField] private GameObject testBoy;
    [SerializeField] private Transform spawnPoint;

    #endregion variables

    public void Restart()
    {
        GameObject oldPlayer = GameObject.FindGameObjectWithTag("Player");
        if(oldPlayer != null) {
            Destroy(oldPlayer);
        }
        Instantiate(testBoy, spawnPoint.position, spawnPoint.rotation);

        GameObject oldKey = GameObject.FindGameObjectWithTag("Key");
        if(oldKey != null) {
            Destroy(oldKey);
        }
        Instantiate(key, new Vector3(-20, 1.7f, 2), new Quaternion(0, -70, 0, 0));
    }
}