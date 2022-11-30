using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : StateMachineBase
{
    int i = 0;

    public override void EnterState(StateManager character)
    {
        Debug.Log("Hello from the IdleState");       
    }

  
    public override void UpdateState(StateManager character)
    {
        Debug.Log("Actualizando desde IdleState");
        i++;
        if(i>4000) {
            character.SwitchState(character.movingState);
        }

    }

    public override void OnCollisionEnter(StateManager character, Collision collision){

    }
}
