using UnityEngine;

public class AIFollowController : MonoBehaviour
{
    public Transform target;          // PlayerCar
    public float followDistance = 5f; // Takip mesafesi
    public float acceleration = 12f;
    public float steering = 50f;
    public float maxSpeed = 18f;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (target == null) return;

        // Hedefe yönelme vektörü
        Vector3 toTarget = target.position - transform.position;
        toTarget.y = 0f;

        float distance = toTarget.magnitude;
        Vector3 dirToTarget = toTarget.normalized;

        // Şu an baktığı yön
        Vector3 forward = transform.forward;
        Vector3 projectedForward = new Vector3(forward.x, 0, forward.z).normalized;

        // Hedefe dönmek için gereken açı
        float steerAngle = Vector3.SignedAngle(projectedForward, dirToTarget, Vector3.up);

        // Direksiyon girdisi
        float steerInput = Mathf.Clamp(steerAngle / 45f, -1f, 1f);

        // Mesafeye göre gaz / fren
        float moveInput;
        if (distance > followDistance + 1f)
            moveInput = 1f;         // uzak → hızlan
        else if (distance < followDistance - 1f)
            moveInput = -0.5f;      // fazla yakın → geri gel
        else
            moveInput = 0.2f;       // mesafeyi koru

        // İleri kuvvet
        Vector3 forwardForce = transform.forward * moveInput * acceleration;
        rb.AddForce(forwardForce, ForceMode.Acceleration);

        // Hız limiti
        Vector3 horizontalVel = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        if (horizontalVel.magnitude > maxSpeed)
        {
            horizontalVel = horizontalVel.normalized * maxSpeed;
            rb.velocity = new Vector3(horizontalVel.x, rb.velocity.y, horizontalVel.z);
        }

        // Direksiyon
        if (horizontalVel.magnitude > 0.1f)
        {
            float turn = steerInput * steering * Time.fixedDeltaTime;
            Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
            rb.MoveRotation(rb.rotation * turnRotation);
        }
    }
}
