using UnityEngine;

public class CarController : MonoBehaviour
{
    public float moveSpeed = 10f;

    Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        float v = Input.GetAxis("Vertical"); // W=1, S=-1
        float h = Input.GetAxis("Horizontal");

        // sadece ileri geri test
        Vector3 vel = transform.forward * (v * moveSpeed);
        rb.velocity = new Vector3(vel.x, rb.velocity.y, vel.z);

        // dönme (opsiyonel)
        rb.angularVelocity = new Vector3(0, h * 2f, 0);
    }
}
