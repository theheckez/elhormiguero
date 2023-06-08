using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMTest : MonoBehaviour
{
    private StateMachineEngine fsm;
    void Start()
    {
        // Create state machine
        fsm = new StateMachineEngine();

        // Create states
        State redState = fsm.CreateEntryState("Red", RedAction);
        State blueState = fsm.CreateState("Blue", BlueAction);

        // Create perceptions
        Perception mouseClick = fsm.CreatePerception<PushPerception>();
        Perception timeOut = fsm.CreatePerception<TimerPerception>(1);

        // Create transitions
        fsm.CreateTransition("mouse click", redState, mouseClick, blueState);
        fsm.CreateTransition("time out", blueState, timeOut, redState);
    }

    void Update()
    {
        fsm.Update();
        if (Input.GetMouseButtonDown(0))
        {
            fsm.Fire("mouse click");
        }
    }

    private void RedAction()
    {
        GetComponent<Renderer>().material.color = new Color(255, 0, 0);
    }
    private void BlueAction()
    {
        GetComponent<Renderer>().material.color = new Color(0, 0, 255);
    }
}
