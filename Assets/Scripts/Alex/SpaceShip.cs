using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class Spaceship2D : MonoBehaviour {
    [Header("Movement Settings")]
    [SerializeField] private float maxSpeed = 10f;
    [SerializeField] private float acceleration = 5f;
    [SerializeField] private float rotationSpeed = 180f;
    [SerializeField] private float stopDistance = 0.5f;
    [SerializeField] private float arrivalDistance = 0.5f;

    [Header("Visual Settings")]
    [SerializeField] private Transform visualTransform;

    private Vector3 targetPosition;
    private Rigidbody2D rb;
    private StateMachine stateMachine;

    // Публічні властивості для доступу зі станів
    public Vector3 TargetPosition => targetPosition;
    public Rigidbody2D Rigidbody => rb;
    public float MaxSpeed => maxSpeed;
    public float Acceleration => acceleration;
    public float RotationSpeed => rotationSpeed;
    public float ArrivalDistance => arrivalDistance;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null) {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }

        rb.gravityScale = 0f;
        rb.linearDamping = 1f;
        rb.angularDamping = 3f;

        if (visualTransform == null) {
            visualTransform = transform;
        }

        var stateFactory = new SpaceshipStateFactory(this, stateMachine);
        stateMachine = new StateMachine(stateFactory);
        stateMachine.ChangeState<IdleSpaceShipState>();
    }

    private void Update() {
        stateMachine?.UpdateState();
    }

    public void SetTarget(Vector3 target) {
        targetPosition = target;
        stateMachine.ChangeState<MovingToTargetState>();
    }

    public void Stop() {
        stateMachine.ChangeState<IdleSpaceShipState>();
    }

    public bool HasReachedTarget() {
        return Vector2.Distance(transform.position, targetPosition) < arrivalDistance;
    }

    private void OnDrawGizmos() {
        if (stateMachine?.CurrentState is MovingToTargetState) {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(targetPosition, arrivalDistance);
            Gizmos.DrawLine(transform.position, targetPosition);
        }
    }
}

public class SpaceshipStateFactory : IStateFactory {
    private readonly Spaceship2D spaceship;
    private readonly StateMachine stateMachine;

    public SpaceshipStateFactory(Spaceship2D spaceship, StateMachine stateMachine) {
        this.spaceship = spaceship;
        this.stateMachine = stateMachine;
    }

    IState IStateFactory.CreateState<T>() {
        if (typeof(T) == typeof(IdleSpaceShipState))
            return new IdleSpaceShipState(stateMachine, spaceship);
        if (typeof(T) == typeof(MovingToTargetState))
            return new MovingToTargetState(stateMachine, spaceship);

        throw new System.Exception($"State {typeof(T).Name} not found");
    }
}

public class IdleSpaceShipState : State {
    private readonly Spaceship2D spaceship;

    public IdleSpaceShipState(IStateMachine stateMachine, Spaceship2D spaceship) : base(stateMachine) {
        this.spaceship = spaceship;
    }

    public override void Enter() {
        spaceship.Rigidbody.linearVelocity = Vector2.zero;
    }

    public override void Exit() { }
}

public class MovingToTargetState : State {
    private readonly Spaceship2D spaceship;
    private float currentSpeed = 0f;

    public MovingToTargetState(IStateMachine stateMachine, Spaceship2D spaceship) : base(stateMachine) {
        this.spaceship = spaceship;
    }

    public override void Enter() {
        currentSpeed = 0f;
    }

    public override void Update() {
        Vector2 currentPosition = spaceship.transform.position;
        Vector2 directionToTarget = (Vector2)spaceship.TargetPosition - currentPosition;
        float distanceToTarget = directionToTarget.magnitude;

        if (distanceToTarget < spaceship.ArrivalDistance) {
            StateMachine.ChangeState<IdleSpaceShipState>();
            return;
        }

        // Обчислення кута
        float targetAngle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg - 90f;
        float currentAngle = spaceship.transform.eulerAngles.z;
        float angleDifference = Mathf.DeltaAngle(currentAngle, targetAngle);

        // Поворот
        float rotationStep = spaceship.RotationSpeed * Time.deltaTime;
        float newAngle = currentAngle + Mathf.Clamp(angleDifference, -rotationStep, rotationStep);
        spaceship.transform.rotation = Quaternion.Euler(0f, 0f, newAngle);

        // Рух
        float alignmentThreshold = 30f;
        if (Mathf.Abs(angleDifference) < alignmentThreshold) {
            currentSpeed = Mathf.Min(currentSpeed + spaceship.Acceleration * Time.deltaTime, spaceship.MaxSpeed);

            if (distanceToTarget < spaceship.ArrivalDistance * 3f) {
                float slowdownFactor = distanceToTarget / (spaceship.ArrivalDistance * 3f);
                currentSpeed *= slowdownFactor;
            }
        } else {
            currentSpeed = Mathf.Max(currentSpeed - spaceship.Acceleration * Time.deltaTime * 2f, 0f);
        }

        Vector2 forward = spaceship.transform.up;
        spaceship.Rigidbody.linearVelocity = forward * currentSpeed;
    }

    public override void Exit() {
        spaceship.Rigidbody.linearVelocity = Vector2.zero;
    }
}