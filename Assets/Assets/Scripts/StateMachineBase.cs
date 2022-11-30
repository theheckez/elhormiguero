using UnityEngine;

public abstract class StateMachineBase 
{
    public abstract void EnterState(StateManager character);

    public abstract void UpdateState(StateManager character);

    public abstract void OnCollisionEnter(StateManager character, Collision collision);
}
