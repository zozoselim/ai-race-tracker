using UnityEngine;

public class AIFollowController : MonoBehaviour
{
    public Transform target;
    public float followDistance = 5f;
    public float acceleration = 30;
    public float steering = 50f;
    public float maxSpeed = 40;

    [HideInInspector] public bool canDrive = false;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (!canDrive) return;
        if (target == null) return;

        Vector3 toTarget = target.position - transform.position;
        toTarget.y = 0f;

        float distance = toTarget.magnitude;
        Vector3 dirToTarget = toTarget.normalized;

        Vector3 forward = transform.forward;
        Vector3 projectedForward = new Vector3(forward.x, 0, forward.z).normalized;

        float steerAngle = Vector3.SignedAngle(projectedForward, dirToTarget, Vector3.up);
        float steerInput = Mathf.Clamp(steerAngle / 45f, -1f, 1f);

        float moveInput;
        if (distance > followDistance + 1f)
            moveInput = 1f;
        else if (distance < followDistance - 1f)
            moveInput = -0.5f;
        else
            moveInput = 0.2f;

        Vector3 forwardForce = transform.forward * moveInput * acceleration;
        rb.AddForce(forwardForce, ForceMode.Acceleration);

        Vector3 horizontalVel = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        if (horizontalVel.magnitude > maxSpeed)
        {
            horizontalVel = horizontalVel.normalized * maxSpeed;
            rb.velocity = new Vector3(horizontalVel.x, rb.velocity.y, horizontalVel.z);
        }

        if (horizontalVel.magnitude > 0.1f)
        {
            float turn = steerInput * steering * Time.fixedDeltaTime;
            Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
            rb.MoveRotation(rb.rotation * turnRotation);
        }
    }
}
