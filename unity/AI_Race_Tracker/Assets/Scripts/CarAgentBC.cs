using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

[RequireComponent(typeof(Rigidbody))]
public class CarAgentBC : Agent
{
    public float motorForce = 0.2
    public float steerStrength = 45f;
    public float maxSpeed = 1f

    private Rigidbody rb;
    private float steer;
    private float throttle;

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // İstersen reset pozisyonu ekle:
        // transform.position = startPos; transform.rotation = startRot;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Ray sensor zaten çevreyi veriyor.
        // Buraya sadece "kendi durumunu" eklemek mantıklı.
        float speed = rb.velocity.magnitude;
        sensor.AddObservation(speed / maxSpeed);

        Vector3 localVel = transform.InverseTransformDirection(rb.velocity);
        sensor.AddObservation(localVel.z / maxSpeed);

        sensor.AddObservation(rb.angularVelocity.y);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        steer = Mathf.Clamp(actions.ContinuousActions[0], -1f, 1f);
        throttle = Mathf.Clamp(actions.ContinuousActions[1], -1f, 1f);

        ApplyControl(steer, throttle);

        // Basit güvenlik: çok hızlanmasın
        if (rb.velocity.magnitude > maxSpeed)
            rb.velocity = rb.velocity.normalized * maxSpeed;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // Demo kaydı için: SEN sürüyorsun, model izliyor
        var ca = actionsOut.ContinuousActions;
        ca[0] = Input.GetAxis("Horizontal"); // A/D
        ca[1] = Input.GetAxis("Vertical");   // W/S
    }

    private void ApplyControl(float steerInput, float throttleInput)
    {
        // Basit fizik: ileri kuvvet + yaw dönüş
        Vector3 forwardForce = transform.forward * throttleInput * motorForce;
        rb.AddForce(forwardForce, ForceMode.Acceleration);

        float steerAmount = steerInput * steerStrength * Time.fixedDeltaTime;
        Quaternion turnOffset = Quaternion.Euler(0f, steerAmount, 0f);
        rb.MoveRotation(rb.rotation * turnOffset);
    }
}
