using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform target;              // AICar veya PlayerCar
    public Vector3 offset = new Vector3(0, 3.5f, -6f);
    public float posLerp = 6f;
    public float rotLerp = 8f;

    void LateUpdate()
    {
        if (!target) return;

        Vector3 desiredPos = target.TransformPoint(offset);
        transform.position = Vector3.Lerp(
            transform.position,
            desiredPos,
            posLerp * Time.deltaTime
        );

        Quaternion desiredRot = Quaternion.LookRotation(
            target.position - transform.position,
            Vector3.up
        );
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            desiredRot,
            rotLerp * Time.deltaTime
        );
    }
}
