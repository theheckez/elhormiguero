using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour
{

    StateMachineBase currentState;
    
    public IdleState idleState = new IdleState();
    public MovingState movingState = new MovingState();

    // Start is called before the first frame update
    void Start()
    {
        currentState = idleState;

        currentState.EnterState(this);
    }

    void OnCollisionEnter(Collision collision) {
        currentState.OnCollisionEnter(this, collision);
    }
    // Update is called once per frame
    void Update()
    {
        currentState.UpdateState(this);
    }

    public void SwitchState(StateMachineBase state)
    {
        currentState = state;
        state.EnterState(this);
    }
}
