using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class soldier : MonoBehaviour
{
    private StateMachineEngine soldier_fsm;

    private float maxSpeed = 1;
    private float steerStrength = 4;
    private float wanderStrength = 0.20f;
    private int wide = 2, high = 2;
    private float patrol_dist_w, patrol_dist_h;

    Vector2 position;
    Vector2 enemyPos;
    Vector2 colony;
    Vector2 velocity;
    Vector2 desiredDirection;

    private bool enemyAssigned = false;
    private bool inColonyArea = true;
    private bool enemyKilled = false;

    private void Start()
    {
        colony = transform.position;
        patrol_dist_w = colony.x + wide;
        patrol_dist_h = colony.y + high;
        CreateStateMachine();
    }
    private void Update()
    {
        soldier_fsm.Update();
        Debug.Log("State: " + soldier_fsm.GetCurrentState().Name);

        if (enemyAssigned)
        {
            soldier_fsm.Fire("EnemyDetected");
            enemyAssigned = false;
        }

        if (Random.Range(0, 100.0f) > 99.95f & soldier_fsm.GetCurrentState().Name == "patrolState")
            soldier_fsm.Fire("Watch");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "enemy")
        {
            Debug.Log("ENEMY!");
            enemyAssigned = true;
            enemyPos = collision.gameObject.transform.position;
        }
    }

    bool inColony()
    {
        return (position.x < patrol_dist_w & position.y < patrol_dist_h & position.x > -patrol_dist_w & position.y > -patrol_dist_h);
    }

    void CreateStateMachine()
    {
        soldier_fsm = new StateMachineEngine(BehaviourEngine.IsNotASubmachine);

        State idleState = soldier_fsm.CreateEntryState("idleState");
        State patrolState = soldier_fsm.CreateState("patrolState", Patrol);
        State returnColonyState = soldier_fsm.CreateState("returnColonyState", Return);
        State attackState = soldier_fsm.CreateState("attackState", Attack);
        State watchState = soldier_fsm.CreateState("watchState");

        Perception nearColony = soldier_fsm.CreatePerception<AndPerception>(() => inColonyArea);
        Perception enemyDetected = soldier_fsm.CreatePerception<PushPerception>();
        Perception watchTerrain = soldier_fsm.CreatePerception<PushPerception>();
        Perception enemysBeenKilled = soldier_fsm.CreatePerception<ValuePerception>(() => enemyKilled);
        Perception enemysntBeenKilled = soldier_fsm.CreatePerception<ValuePerception>(() => !enemyKilled);
        Perception watchTime = soldier_fsm.CreatePerception<TimerPerception>(3);

        soldier_fsm.CreateTransition("Idle", idleState, nearColony, patrolState);
        soldier_fsm.CreateTransition("Patrolling", patrolState, nearColony, patrolState);
        soldier_fsm.CreateTransition("Watch", patrolState, watchTerrain, watchState);
        soldier_fsm.CreateTransition("Watching", watchState, watchTime, patrolState);
        soldier_fsm.CreateTransition("EnemyDetected", patrolState, enemyDetected, attackState);
        soldier_fsm.CreateTransition("ReachingEnemy", attackState, enemysntBeenKilled, attackState);
        soldier_fsm.CreateTransition("EnemyKilled", attackState, enemysBeenKilled, returnColonyState);
    }

    void Patrol()
    {
        desiredDirection = (desiredDirection + Random.insideUnitCircle * wanderStrength).normalized;

        Vector2 desiredVelocity = desiredDirection * maxSpeed;
        Vector2 desiredSteeringForce = (desiredVelocity - velocity) * steerStrength;
        Vector2 acceleration = Vector2.ClampMagnitude(desiredSteeringForce, steerStrength) / 1;

        velocity = Vector2.ClampMagnitude(velocity + acceleration * Time.deltaTime, maxSpeed);
        position += velocity * Time.deltaTime;

        float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
        
        if (inColony())
        {
            transform.SetPositionAndRotation(position, Quaternion.Euler(0.0f, 0.0f, angle));
        }
        else
        {
            desiredDirection = colony;
            position = transform.position;
        }
    }

    private void Return()
    {
        transform.position = Vector2.MoveTowards(transform.position, colony, 1.5f * Time.deltaTime);
    }

    private void Attack()
    {
        transform.position = Vector2.MoveTowards(transform.position, enemyPos, 1.5f * Time.deltaTime);
        if (new Vector2(transform.position.x, transform.position.y) == enemyPos)
        {
            enemyKilled = true;
            enemyAssigned = false;
        }
            
    }
}