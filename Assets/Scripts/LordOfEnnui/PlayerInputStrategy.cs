using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputStrategy : ACharacterStrategy {
    
    InputSystem_Actions inputActions;
    InputAction moveAction, lookAction, sprintAction, attackAction;
    
    [SerializeField]
    float inputQueueTime = 0.1f;

    [SerializeField]
    Transform playerInputSpace = default;
    Vector3 right, up;

    [Header("ReadOnly")]
    [SerializeField]
    bool mouseUsed = false;
    [SerializeField]
    Vector2 moveInput, lookInput;
    [SerializeField]
    bool sprintInputQueued, attackInput;
    [SerializeField]
    Vector3 moveDirection, lookDirection;

    [Header("State")]
    public bool sprintActive = true, canSprint = true, canAttack = true, canMove = true;
    public bool isSprinting = false, isAttacking = false, inputQueued = false;
    public float angle;

    [SerializeField]
    float inputQueueTimer;

    private void Awake() {         
        if (playerInputSpace == null) playerInputSpace = transform;
        inputActions = new InputSystem_Actions();
        up = playerInputSpace.up; up.z = 0f; up.Normalize();
        right = playerInputSpace.right; right.z = 0f; right.Normalize();

        moveAction = InputSystem.actions.FindAction("Move");
        lookAction = InputSystem.actions.FindAction("Look");
        sprintAction = InputSystem.actions.FindAction("Sprint");
        attackAction = InputSystem.actions.FindAction("Attack");

        moveAction.performed += ctx => {
            moveInput = ctx.ReadValue<Vector2>();
            moveDirection = right * moveInput.x + up * moveInput.y;
        };
        moveAction.canceled += ctx => {
            moveInput = Vector2.zero;
            moveDirection = Vector3.zero;
        };

        lookAction.performed += ctx => {
            lookInput = ctx.ReadValue<Vector2>();
            mouseUsed = ctx.control.device is Mouse;
            if (!mouseUsed) {
                lookDirection = up * lookInput.x + up * lookInput.y;
            } else {
                lookDirection = (Camera.main.ScreenToWorldPoint(lookInput) - transform.position).normalized;
            }
            angle = Vector2.SignedAngle(Vector2.right, lookDirection);
            angle = angle < 0 ? 360 + angle : angle;
        };

        attackAction.performed += ctx => attackInput = true;
        attackAction.canceled += ctx => attackInput = false;

        sprintAction.performed += ctx => {
            sprintInputQueued = true;
            inputQueued = true;
            inputQueueTimer = 0f;
        };
    }

    private void Update() {
        if (inputQueued) inputQueueTimer += Time.deltaTime;
    }

    public bool SprintThisFrame() {
        if (inputQueued) {
            if (inputQueueTimer > inputQueueTime) {
                sprintInputQueued = false;
                inputQueued = false;
            }
        }

        if (sprintInputQueued && sprintActive && canSprint) {
            inputQueued = false;
            sprintInputQueued = false;
            return true;
        }
        return false;
    }

    public override bool FireThisFrame(ABullet2D bullet) {
        return attackInput;
    }

    public override Vector3 AimDirection() {
        return lookDirection;
    }

    public override Vector3 MoveDirection() {
        return moveDirection;
    }

    public override float FireAngle() {
        return angle;
    }
}
