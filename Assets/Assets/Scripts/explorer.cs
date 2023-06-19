using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class explorer : MonoBehaviour
{
    private StateMachineEngine explorer_fsm;
    private StateMachineEngine danger_fsm;

    State exploreState;
    State spottingState;
    State returningState;
    State runState;

    Perception inDanger;

    State runningStateSM;

    Perception inDangerSM;
    Perception notInDangerSM;

    private float maxSpeed = 2;
    private float steerStrength = 2;
    private float wanderStrength = 0.15f;

    Vector2 position;
    Vector2 colony;
    Vector2 velocity;
    Vector2 desiredDirection;
    
    private Vector2 foodPos;
    private Vector2 enemyPos;
    private bool foodSpotted = false;
    private bool headColony = false;
    private bool endExp = false;
    public bool enemySpotted = false;
    private bool enemyStill = false;

    void Start()
    {
        colony = transform.position;
        
        CreateSubMachine();
        CreateStateMachine();

        danger_fsm.CreateExitTransition("EnemyLost", runningStateSM, notInDangerSM, exploreState);
    }

    // Update is called once per frame
    void Update()
    {
        explorer_fsm.Update();
        if (explorer_fsm.GetCurrentState().Name.Equals("runState"))
        {
            danger_fsm.Update();
        // Debug.Log("    >Danger:" + danger_fsm.GetCurrentState().Name);
        }

        // Debug.Log(">Explorer:" + explorer_fsm.GetCurrentState().Name);

        if (enemySpotted)
        {
            explorer_fsm.Fire("EnemySpotted");
            enemySpotted = false;
        }

        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "food")
        {
            foodSpotted = true;
            foodPos = collision.gameObject.transform.position;
        }
        if (collision.gameObject.tag == "enemy")
        {
            enemySpotted = true;
            enemyPos = collision.gameObject.transform.position;
            desiredDirection = -enemyPos;
            //Debug.Log("Ant scared");
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "enemy")
        {
            enemyStill = true;
            enemyPos = collision.gameObject.transform.position;
            desiredDirection = -enemyPos;
            //Debug.Log("Running away!");
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "enemy")
        {
            enemyStill = false;
            //Debug.Log("Free!");
        }
    }

    private void CreateStateMachine()
    {
        explorer_fsm = new StateMachineEngine(BehaviourEngine.IsNotASubmachine);

        State idleState = explorer_fsm.CreateEntryState("idleState");
        exploreState = explorer_fsm.CreateState("exploreState", Explore);
        spottingState = explorer_fsm.CreateState("spottingState", ReachFood);
        returningState = explorer_fsm.CreateState("returningState", ReturnColony);
        runState = explorer_fsm.CreateSubStateMachine("runState", danger_fsm);

        Perception foodNeed = explorer_fsm.CreatePerception<AndPerception>(() => !headColony, () => !foodSpotted);
        Perception foodNotSpotted = explorer_fsm.CreatePerception<ValuePerception>(() => !foodSpotted);
        Perception foodSight = explorer_fsm.CreatePerception<ValuePerception>(() => foodSpotted);
        Perception unreached = explorer_fsm.CreatePerception<ValuePerception>(() => !headColony);
        Perception spotted = explorer_fsm.CreatePerception<ValuePerception>(() => headColony);
        Perception endExploration = explorer_fsm.CreatePerception<ValuePerception>(() => endExp);
        inDanger = explorer_fsm.CreatePerception<PushPerception>();

        explorer_fsm.CreateTransition("FoodNecessity", idleState, foodNeed, exploreState);
        explorer_fsm.CreateTransition("FoodNotOnSight", exploreState, foodNotSpotted, exploreState);
        explorer_fsm.CreateTransition("FoodOnSight", exploreState, foodSight, spottingState);
        explorer_fsm.CreateTransition("FoodSpotting", spottingState, unreached, spottingState);
        explorer_fsm.CreateTransition("FoodSpotted", spottingState, spotted, returningState);
        explorer_fsm.CreateTransition("ReturnState", returningState, spotted, returningState);
        explorer_fsm.CreateTransition("EndOfExploration", returningState, endExploration, exploreState);
        explorer_fsm.CreateTransition("EnemySpotted", exploreState, inDanger, runState);
    }

    private void CreateSubMachine()
    {
        danger_fsm = new StateMachineEngine(BehaviourEngine.IsASubmachine);

        runningStateSM = danger_fsm.CreateEntryState("runningStateSM", Run);

        inDangerSM = danger_fsm.CreatePerception<ValuePerception>(() => enemyStill);
        notInDangerSM = danger_fsm.CreatePerception<ValuePerception>(() => !enemyStill);

        danger_fsm.CreateTransition("NotLost", runningStateSM, inDangerSM, runningStateSM);
    }

    void Run()
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
