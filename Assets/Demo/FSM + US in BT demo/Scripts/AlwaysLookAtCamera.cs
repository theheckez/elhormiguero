using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlwaysLookAtCamera : MonoBehaviour {

    #region variables

    [SerializeField] private GameObject target;

    #endregion variables

    // Update is called once per frame
    private void Update()
    {
        transform.LookAt(target.transform.position);
    }
}