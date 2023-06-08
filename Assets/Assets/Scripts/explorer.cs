using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class explorer : MonoBehaviour
{
    private StateMachineEngine explorer_fsm;

    public float maxSpeed = 2;
    public float steerStrength = 2;
    public float wanderStrength = 0.015f;

    Vector2 position;
    Vector2 colony;
    Vector2 velocity;
    Vector2 desiredDirection;
    
    private Vector2 foodPos;
    private bool foodSpotted = false;
    private bool headColony = false;
    private bool endExp = false;

    void Start()
    {
        colony = transform.position;
        explorer_fsm = new StateMachineEngine();
        CreateStateMachine();
        //Debug.Log(explorer_fsm.GetCurrentState());
    }

    // Update is called once per frame
    void Update()
    {
        explorer_fsm.Update();
        Debug.Log(explorer_fsm.GetCurrentState().Name);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "food")
        {
            foodSpotted = true;
            foodPos = collision.gameObject.transform.position;
            Debug.Log("FOOD POS " + foodPos);
        }
    }

    private void CreateStateMachine()
    {
        Perception foodNeed = explorer_fsm.CreatePerception<AndPerception>(() => !headColony, () => !foodSpotted);
        Perception foodNotSpotted = explorer_fsm.CreatePerception<ValuePerception>(() => !foodSpotted);
        Perception foodSight = explorer_fsm.CreatePerception<ValuePerception>(() => foodSpotted);
        Perception unreached = explorer_fsm.CreatePerception<ValuePerception>(() => !headColony);
        Perception spotted = explorer_fsm.CreatePerception<ValuePerception>(() => headColony);
        Perception endExploration = explorer_fsm.CreatePerception<ValuePerception>(() => endExp);

        State idleState = explorer_fsm.CreateEntryState("idleState");
        State exploreState = explorer_fsm.CreateState("exploreState", Explore);
        State spottingState = explorer_fsm.CreateState("spottingState", ReachFood);
        State returningState = explorer_fsm.CreateState("returningState", ReturnColony);

        explorer_fsm.CreateTransition("FoodNecessity", idleState, foodNeed, exploreState);
        explorer_fsm.CreateTransition("FoodNotOnSight", exploreState, foodNotSpotted, exploreState);
        explorer_fsm.CreateTransition("FoodOnSight", exploreState, foodSight, spottingState);
        explorer_fsm.CreateTransition("FoodSpotting", spottingState, unreached, spottingState);
        explorer_fsm.CreateTransition("FoodSpotted", spottingState, spotted, returningState);
        explorer_fsm.CreateTransition("ReturnState", returningState, spotted, returningState);
        explorer_fsm.CreateTransition("EndOfExploration", returningState, endExploration, exploreState);
    }

    void Explore()
    {
        desiredDirection = (desiredDirection + Random.insideUnitCircle * wanderStrength).normalized;

        Vector2 desiredVelocity = desiredDirection * maxSpeed;
        Vector2 desiredSteeringForce = (desiredVelocity - velocity) * steerStrength;
        Vector2 acceleration = Vector2.ClampMagnitude(desiredSteeringForce, steerStrength) / 1;

        velocity = Vector2.ClampMagnitude(velocity + acceleration * Time.deltaTime, maxSpeed);
        position += velocity * Time.deltaTime;

        float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
        transform.SetPositionAndRotation(position, Quaternion.Euler(0.0f, 0.0f, angle));
    }

    private void ReachFood()
    {
        transform.position = Vector2.MoveTowards(transform.position, foodPos, 1.5f * Time.deltaTime);
        if (new Vector2(transform.position.x, transform.position.y) == foodPos)
        {
            foodSpotted = false;
            headColony = true;
        }
    }

    private void ReturnColony()
    {
        transform.position = Vector2.MoveTowards(transform.position, colony, 1.5f * Time.deltaTime);
        if (new Vector2(transform.position.x, transform.position.y) == colony)
        {
            headColony = false;
            endExp = true;
            position = new Vector2(0.0f,0.0f);
        }
    }
}
