using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestartButton : MonoBehaviour {

    #region variables

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
    }
}