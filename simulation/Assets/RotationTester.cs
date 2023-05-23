using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RotationTester : MonoBehaviour
{
    public Vector3 DisplayRotation;

    private void Update()
    {
        Vector3 NewRangeDifference = Vector3.zero;

        Vector3 RobotDown = -this.transform.up;
        Vector3 RobotForward = this.transform.forward;
        Vector3 RobotRight = this.transform.right;

        Vector3 RobotWorldDown = Vector3.down;
        Vector3 RobotWorldForward = Vector3.forward;
        Vector3 RobotWorldRight = Vector3.right;

        NewRangeDifference.x = Vector3.SignedAngle(RobotDown, RobotWorldDown, transform.right);

        NewRangeDifference.y = Vector3.SignedAngle(RobotForward, RobotWorldForward, transform.up);

        NewRangeDifference.z = Vector3.SignedAngle(RobotRight, RobotWorldRight, transform.forward);

        float roll  = Mathf.Atan2(2*transform.rotation.y*transform.rotation.w + 2*transform.rotation.x*transform.rotation.z, 1 - 2*transform.rotation.y*transform.rotation.y - 2*transform.rotation.z*transform.rotation.z);
        float pitch = Mathf.Atan2(2*transform.rotation.x*transform.rotation.w + 2*transform.rotation.y*transform.rotation.z, 1 - 2*transform.rotation.x*transform.rotation.x - 2*transform.rotation.z*transform.rotation.z);
        float yaw   =  Mathf.Asin(2*transform.rotation.x*transform.rotation.y + 2*transform.rotation.z*transform.rotation.w);

        DisplayRotation = new Vector3(roll, pitch, yaw);
    }
}
