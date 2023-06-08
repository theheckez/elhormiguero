using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArriveToDestination : Perception {

    #region variables

    private Vector3 position;
    private Vector3 destination;

    #endregion variables

    public override bool Check()
    {
        if(Vector3.Distance(position, destination) < 0.3f) {
            return true;
        }
        else {
            return false;
        }
    }

    public void SetPosition(Vector3 position)
    {
        this.position = position;
    }

    public void SetDestination(Vector3 destination)
    {
        this.destination = destination;
    }
}