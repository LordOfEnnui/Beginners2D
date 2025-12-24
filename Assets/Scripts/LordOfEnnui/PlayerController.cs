using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour {
    GameObject pCamera;
    InputAction moveAction, jumpAction, sprintAction;
    Rigidbody body, groundBody, prevGroundBody;
    float minGroundDotProduct;

    [SerializeField]
    Transform playerInputSpace = default;

    [SerializeField]
    float maxSpeed = 5, maxAcceleration = 20, maxAirAcceleration = 2, jumpHeight = 2, groundGraceTime = 0.2f, jumpCooldown = 0.2f, sprintSpeed = 2f, sprintCooldown = 0.1f, sprintResetCooldown = 2f;
    [SerializeField, Range(0f, 90f)]
    float groundContactAngle = 25f;
    [SerializeField]
    int maxAirJumps = 0, maxConsecutiveSprints = 2;
    [SerializeField]
    bool angleJumps = true;
    [SerializeField]
    public bool canMove = true;


    [Header("Out (ReadOnly)")]

    [SerializeField]
    Vector2 playerInput;
    [SerializeField]
    Vector3 targetVelocity, velocity, contactNormal = Vector3.up, groundVelocity;
    [SerializeField]
    bool jumpInput, sprintInput, onGround;
    [SerializeField]
    int jumpPhase, sprintPhase;
    [SerializeField]
    float groundGraceTimer = 0f, sprintCooldownTimer = 0f, sprintResetTimer = 0f, jumpCooldownTimer = 0f;

    private void Start() {
        // Find the references to the "Move" and "Jump" actions
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
        sprintAction = InputSystem.actions.FindAction("Sprint");
        pCamera = LDirectory.Instance.pCamera;
        playerInputSpace = pCamera.GetComponent<Transform>();
        body = GetComponent<Rigidbody>();
        OnValidate();
    }

    void Update() {
        playerInput = moveAction.ReadValue<Vector2>();
        jumpInput |= jumpAction.WasPressedThisFrame();
        sprintInput |= sprintAction.WasPressedThisFrame();
        Vector3 forward = playerInputSpace.forward;
        forward.y = 0f;
        forward.Normalize();
        Vector3 right = playerInputSpace.right;
        right.y = 0f;
        right.Normalize();
        targetVelocity =
            (forward * playerInput.y + right * playerInput.x) * maxSpeed;
        groundGraceTimer += Time.deltaTime;
        jumpCooldownTimer += Time.deltaTime;
        sprintCooldownTimer += Time.deltaTime;
        if (sprintPhase > 0) {
            sprintResetTimer += Time.deltaTime;
        }
    }

    private void OnValidate() {
        minGroundDotProduct = Mathf.Cos(groundContactAngle * Mathf.Deg2Rad);
    }

    private void FixedUpdate() {
        if (!canMove) {
            body.linearVelocity = Vector3.zero;
            return;
        }
        velocity = body.linearVelocity;
        if (onGround) {
            jumpPhase = 0;
        } else {
            contactNormal = Vector3.up;
            groundVelocity = Vector3.zero;
            groundBody = null;
        }

        if (sprintInput) {
            sprintInput = false;
            if (sprintResetTimer > sprintResetCooldown) {
                sprintPhase = 0;
                sprintResetTimer = 0;
            }
            if ((sprintPhase < maxConsecutiveSprints || maxConsecutiveSprints < 0) && sprintCooldownTimer > sprintCooldown) {
                sprintPhase++;
                sprintResetTimer = 0;
                sprintCooldownTimer = 0;
                Vector3 sprintDirection = Vector3.Normalize(targetVelocity);
                float alignedSpeed = Vector3.Dot(velocity, sprintDirection);
                float adjustedSprintSpeed = sprintSpeed;
                if (alignedSpeed > 0f)
                    adjustedSprintSpeed = Mathf.Max(adjustedSprintSpeed - alignedSpeed, 0f);
                velocity += sprintDirection * adjustedSprintSpeed;
            }
        }

        if (jumpInput) {
            jumpInput = false;
            if ((onGround || jumpPhase < maxAirJumps) && jumpCooldownTimer > jumpCooldown) {
                jumpPhase++;
                jumpCooldownTimer = 0;
                float jumpSpeed = Mathf.Sqrt(-2f * Physics.gravity.y * jumpHeight);
                float alignedSpeed = Vector3.Dot(velocity, contactNormal);
                if (alignedSpeed > 0f)
                    jumpSpeed = Mathf.Max(jumpSpeed - alignedSpeed, 0f);
                velocity += contactNormal * jumpSpeed;
            }
        }
        float acceleration = onGround ? maxAcceleration : maxAirAcceleration;
        float maxSpeedChange = acceleration * Time.fixedDeltaTime;
        velocity.x =
            Mathf.MoveTowards(velocity.x, targetVelocity.x, maxSpeedChange);
        velocity.z =
            Mathf.MoveTowards(velocity.z, targetVelocity.z, maxSpeedChange);

        body.linearVelocity = velocity;
        onGround = groundGraceTimer < groundGraceTime;
    }

    private void OnCollisionEnter(Collision collision) {

    }

    private void OnCollisionStay(Collision collision) {
        for (int i = 0; i < collision.contactCount; i++) {
            Vector3 normal = collision.GetContact(i).normal;
            if (normal.y >= minGroundDotProduct) {
                groundGraceTimer = 0;
                if (angleJumps)
                    contactNormal = normal;
                groundBody = collision.rigidbody;
            }
        }
    }
}
