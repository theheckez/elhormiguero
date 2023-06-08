using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleFSM : MonoBehaviour {

    #region variables

    [SerializeField] private float speed;

    private Rigidbody rbody;
    private StateMachineEngine vehicleFSM;
    private GameObject radar;

    #endregion variables

    // Start is called before the first frame update
    private void Start()
    {
        rbody = GetComponent<Rigidbody>();
        speed = Random.Range(15, 30);
        radar = GameObject.FindGameObjectWithTag("Radar");

        CreateStateMachine();
    }

    private void CreateStateMachine()
    {
        vehicleFSM = new StateMachineEngine(false);

        // Perceptions
        Perception radarBroken = vehicleFSM.CreatePerception<IsInStatePerception>(radar.GetComponent<RadarFSM>().GetRadarFSM(), "Broken");
        Perception radarWorking = vehicleFSM.CreatePerception<IsInStatePerception>(radar.GetComponent<RadarFSM>().GetRadarFSM(), "Working");
        Perception direct = vehicleFSM.CreatePerception<PushPerception>();

        // States
        State runningState = vehicleFSM.CreateEntryState("Running", OnRunning);
        State speedUpState = vehicleFSM.CreateState("Speed up", SpeedUp);
        State slowDownState = vehicleFSM.CreateState("Slow down", SlowDown);

        // Transitions
        vehicleFSM.CreateTransition("Radar is broken", runningState, radarBroken, speedUpState);
        vehicleFSM.CreateTransition("Radar is working", speedUpState, radarWorking, slowDownState);
        vehicleFSM.CreateTransition("To running", slowDownState, direct, runningState);
    }

    // Update is called once per frame
    private void Update()
    {
        vehicleFSM.Update();
    }

    private void OnRunning()
    {
        rbody.velocity = transform.forward * speed;
    }

    private void SpeedUp()
    {
        speed += 10;
    }

    private void SlowDown()
    {
        speed -= 10;
        vehicleFSM.Fire("To running");
    }

    public float GetSpeed()
    {
        return speed;
    }
}