using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformRotateLimiter : MonoBehaviour
{
    public Vector3 maxAxis;
    public Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        ClampRotation();
    }

    void ClampRotation()
    {
        Vector3 euler = rb.rotation.eulerAngles;

        float rotateX = NormalizeAngle(euler.x);
        float rotateZ = NormalizeAngle(euler.z);

        rotateX = Mathf.Clamp(rotateX, -maxAxis.x, maxAxis.x);
        rotateZ = Mathf.Clamp(rotateZ, -maxAxis.z, maxAxis.z);

        Vector3 clEuler = new Vector3(rotateX, 0, rotateZ);
        rb.MoveRotation(Quaternion.Euler(clEuler));
    }

    float NormalizeAngle(float angle)
    {
        if (angle > 180)
        {
            angle -= 360;
        }
        return angle;
    }
}
