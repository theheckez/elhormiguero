using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RadarFSM : MonoBehaviour {

    #region variables

    [SerializeField] private Vector3 pointToLook;
    [SerializeField] private Text speedText;

    private StateMachineEngine radarFSM;
    private StateMachineEngine workingSubFSM;
    private Light radarLight;
    private DetectCar detectCar;
    private State brokenState;
    private bool intensityIncreasing;

    #endregion variables

    // Start is called before the first frame update
    private void Start()
    {
        radarLight = GetComponentInChildren<Light>();

        CreateSubMachine();
        CreateStateMachine();
    }

    private void CreateSubMachine()
    {
        workingSubFSM = new StateMachineEngine(true);

        // Perceptions
        detectCar = workingSubFSM.CreatePerception<DetectCar>(new DetectCar(gameObject, pointToLook));
        Perception carOverSpeed = workingSubFSM.CreatePerception<ValuePerception>(() => detectCar.GetCarSpeed() > 20);
        Perception carOnSpeed = workingSubFSM.CreatePerception<ValuePerception>(() => detectCar.GetCarSpeed() <= 20);
        Perception overSpeedLimit = workingSubFSM.CreateAndPerception<AndPerception>(detectCar, carOverSpeed);
        Perception onSpeedLimit = workingSubFSM.CreateAndPerception<AndPerception>(detectCar, carOnSpeed);
        Perception timeout = workingSubFSM.CreatePerception<TimerPerception>(2);

        // States
        State waitingForCarState = workingSubFSM.CreateEntryState("Waiting for car", OnWaitingForCar);
        State speedingState = workingSubFSM.CreateState("Speeding", OnSpeeding);
        State correctSpeedState = workingSubFSM.CreateState("Correct speed", OnCorrectSpeed);

        // Transitions
        workingSubFSM.CreateTransition("Car over speed limit", waitingForCarState, overSpeedLimit, speedingState);
        workingSubFSM.CreateTransition("Car on speed limit", waitingForCarState, onSpeedLimit, correctSpeedState);
        workingSubFSM.CreateTransition("To waiting for next bad car", speedingState, timeout, waitingForCarState);
        workingSubFSM.CreateTransition("To waiting for next good car", correctSpeedState, timeout, waitingForCarState);
    }

    private void CreateStateMachine()
    {
        radarFSM = new StateMachineEngine(false);

        // Perceptions
        Perception direct = radarFSM.CreatePerception<PushPerception>();
        Perception breakDown = radarFSM.CreatePerception<TimerPerception>(30);
        Perception fix = radarFSM.CreatePerception<TimerPerception>(15);

        // States
        State entryState = radarFSM.CreateEntryState("Entry State");
        State workingState = radarFSM.CreateSubStateMachine("Working", workingSubFSM);
        brokenState = radarFSM.CreateState("Broken", Broken);

        // Transitions
        radarFSM.CreateTransition("Direct", entryState, direct, workingState);
        workingSubFSM.CreateExitTransition("To broken", workingState, breakDown, brokenState);
        radarFSM.CreateTransition("To working", brokenState, fix, workingState);

        radarFSM.Fire("Direct");
    }

    // Update is called once per frame
    private void Update()
    {
        workingSubFSM.Update();
        radarFSM.Update();

        if(radarFSM.GetCurrentState() == brokenState) {
            BlinkingLight();
        }
    }

    private void OnWaitingForCar()
    {
        radarLight.intensity = 2;
        radarLight.color = new Color(0, 0.6f, 1);
    }

    private void OnSpeeding()
    {
        radarLight.intensity = 2;
        radarLight.color = detectCar.GetLightColor();
        speedText.text = (detectCar.GetCarSpeed() + 100).ToString();
    }

    private void OnCorrectSpeed()
    {
        radarLight.intensity = 2;
        radarLight.color = detectCar.GetLightColor();
        speedText.text = (detectCar.GetCarSpeed() + 100).ToString();
    }

    private void Broken()
    {
        radarLight.intensity = 2;
        radarLight.color = new Color(1, 1, 0);
        speedText.text = "---";
    }

    private void BlinkingLight()
    {
        if(radarLight.intensity >= 2) {
            intensityIncreasing = false;
        }
        else if(radarLight.intensity <= 0) {
            intensityIncreasing = true;
        }
        radarLight.intensity += (intensityIncreasing) ? 0.1f : -0.1f;
    }

    public StateMachineEngine GetRadarFSM()
    {
        return radarFSM;
    }
}