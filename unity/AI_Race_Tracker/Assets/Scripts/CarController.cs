using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CarController : MonoBehaviour
{
    public float moveSpeed = 5f;     // İleri-geri hızı
    public float turnSpeed = 120f;   // Sağa-sola dönüş hızı (derece/saniye)

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        // Klavye girdileri
        float moveInput = Input.GetAxis("Vertical");      // W = +1, S = -1
        float turnInput = Input.GetAxis("Horizontal");    // A/D veya ←/→

        // İleri-geri hareket
        Vector3 move = transform.forward * moveInput * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + move);

        // Dönüş
        if (moveInput != 0)   // Araba hareket ediyorsa dönüş olsun
        {
            float turnAmount = turnInput * turnSpeed * Time.fixedDeltaTime;
            Quaternion turn = Quaternion.Euler(0f, turnAmount, 0f);
            rb.MoveRotation(rb.rotation * turn);
        }
    }
}
