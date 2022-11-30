using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingState : StateMachineBase
{
  public override void EnterState(StateManager character)
    {
        Debug.Log("Hello From the moving State");
    }

  
    public override void UpdateState(StateManager character)
    {
        
    }

    public override void OnCollisionEnter(StateManager character, Collision collision){

    }
}
