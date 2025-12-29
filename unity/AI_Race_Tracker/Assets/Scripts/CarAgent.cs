using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

[RequireComponent(typeof(Rigidbody))]
public class CarAgent : Agent
{
    public Transform target;      // PlayerCar
    public float acceleration = 30;
    public float steering = 50f;
    public float maxSpeed = 40;

    private Rigidbody rb;
    private Vector3 startPos;
    private Quaternion startRot;

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        startPos = transform.position;
        startRot = transform.rotation;
    }

    public override void OnEpisodeBegin()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        transform.position = startPos;
        transform.rotation = startRot;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        if (target == null)
        {
            sensor.AddObservation(Vector3.zero); // yön
            sensor.AddObservation(0f);           // mesafe
        }
        else
        {
            Vector3 toTarget = target.position - transform.position;
            toTarget.y = 0f;

            sensor.AddObservation(toTarget.normalized);         // 3 float
            sensor.AddObservation(toTarget.magnitude / 30f);    // 1 float (normalize)
        }

        // Hız
        Vector3 vel = rb.velocity;
        Vector2 horizVel = new Vector2(vel.x, vel.z) / maxSpeed;
        sensor.AddObservation(horizVel);   // 2 float
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveInput = Mathf.Clamp(actions.ContinuousActions[0], -1f, 1f);
        float steerInput = Mathf.Clamp(actions.ContinuousActions[1], -1f, 1f);

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

        // Hedefe yaklaştıkça ufak ödül
        if (target != null)
        {
            float dist = Vector3.Distance(transform.position, target.position);
            AddReward(-dist * 0.0005f);
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // Klavye ile test
        var cont = actionsOut.ContinuousActions;
        cont[0] = Input.GetAxis("Vertical");    // gaz / fren
        cont[1] = Input.GetAxis("Horizontal");  // direksiyon
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Wall"))
        {
            AddReward(-1f);   // Duvara çarpınca ceza
            EndEpisode();     // Bölümü bitir
        }
    }
}
