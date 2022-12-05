using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrindState : StateMachineBase
{
    public override void EnterState(StateManager character)
    {
        Debug.Log("Hello from the EnterState");
    }


    public override void UpdateState(StateManager character)
    {
        {
            character.SwitchState(character.movingState);
        }

    }

    public override void OnCollisionEnter(StateManager character, Collision collision)
    {

    }
}
