using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InverseKinematics : MonoBehaviour
{
    public Transform TCP;
    public Vector3 pos;
    public float theta1 = 0;
    public float theta2 = 0;

    public float theta3 = 0;

    public float L1 = 0.742f;
    public float L2 = 0.779f;
    public float L3 = 0.587f;

    private Vector3 L1Vec = new Vector3(0, 0, 0.742f);

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private Vector3 testPoint;

    // Update is called once per frame
    void Update()
    {
        Vector3 TCP_position = TCP.position; 

        Quaternion rotationTorso = Quaternion.Euler(0, 305, 0);
        Matrix4x4 m1 = Matrix4x4.Rotate(rotationTorso);
        Vector3 Shoulder_position = this.transform.position + m1.MultiplyPoint3x4(L1Vec);

        testPoint = Shoulder_position;

        pos = TCP_position - Shoulder_position;

        TCP_position = TCP_position - Shoulder_position;

        theta1 = Mathf.Atan(TCP_position.x/TCP_position.z) * Mathf.Rad2Deg;        

        float num = -Mathf.Pow(L3, 2) + Mathf.Pow(L2, 2) + Mathf.Pow(TCP_position.x, 2) + Mathf.Pow(TCP_position.y, 2) + Mathf.Pow(TCP_position.z, 2);

        float denum = 2 * L2 * Mathf.Sqrt(Mathf.Pow(TCP_position.x, 2) + Mathf.Pow(TCP_position.y, 2) + Mathf.Pow(TCP_position.z, 2));

        // float denum = 2 * L2 * (Mathf.Abs(TCP_position.x) + Mathf.Abs(TCP_position.y) + Mathf.Abs(TCP_position.z)); 

        float term1 = Mathf.Acos(num/denum);
        float term2 = Mathf.Atan(TCP_position.y / Mathf.Sqrt( Mathf.Pow(TCP_position.x, 2) + Mathf.Pow(TCP_position.z, 2)));

        theta2 = (term1 + term2) * Mathf.Rad2Deg;

        float num2 = Mathf.Pow(TCP_position.x, 2) + Mathf.Pow(TCP_position.y, 2) + Mathf.Pow(TCP_position.z, 2) - Mathf.Pow(L2, 2) - Mathf.Pow(L3, 2);
        float denum2 = 2 * L2 * L3;
        theta3 = Mathf.Acos(num2/denum2) * Mathf.Rad2Deg;
    }

    private float ValueRangeMapping(float value, float oldMin, float oldMax, float newMin, float newMax)
    {
        return newMin + (value - oldMin) * (newMax - newMin) / (oldMax - oldMin); // Inspired from https://prime31.github.io/simple-value-mapping/
    }

    /// <summary>
    /// Callback to draw gizmos that are pickable and always drawn.
    /// </summary>
    void OnDrawGizmos()
    {
        Gizmos.DrawSphere(testPoint, 0.2f);
    }
}
