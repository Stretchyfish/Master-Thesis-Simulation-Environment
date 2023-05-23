using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IMUController : MonoBehaviour
{
    public Transform Torso;
    public Vector3 HeadingVector;
    public Vector3 DefaultOrientation;
    public Vector3 CurrentOrientation;
    public Vector3 LocalRotation;
    private void Awake()
    {
        DefaultOrientation = this.transform.rotation.eulerAngles;
    }

    private void FixedUpdate()
    {
        Vector3 NewRangeDifference = Torso.transform.rotation.eulerAngles;
        LocalRotation = Torso.transform.rotation.eulerAngles;



        Vector3 DebugRangeDifference = Vector3.zero;

        Vector3 RobotDown = -this.transform.up;
        Vector3 RobotForward = this.transform.forward;
        Vector3 RobotRight = this.transform.right;

        Vector3 RobotWorldDown = Vector3.down;
        Vector3 RobotWorldForward = Vector3.forward;
        Vector3 RobotWorldRight = Vector3.right;

        DebugRangeDifference.x = Vector3.SignedAngle(RobotDown, RobotWorldDown, Vector3.right);

        DebugRangeDifference.y = Vector3.SignedAngle(RobotForward, RobotWorldForward, Vector3.up);

        DebugRangeDifference.z = Vector3.SignedAngle(RobotRight, RobotWorldRight, Vector3.forward);


        /*** Code here coming from: https://answers.unity.com/questions/416169/finding-pitchrollyaw-from-quaternions.html, user ratneshpatel ***/
        Quaternion q = Torso.transform.rotation; 
        float Pitch = Mathf.Rad2Deg * Mathf.Atan2(2 * q.x * q.w - 2 * q.y * q.z, 1 - 2 * q.x * q.x - 2 * q.z * q.z);
        float Yaw = Mathf.Rad2Deg * Mathf.Atan2(2 * q.y * q.w - 2 * q.x * q.z, 1 - 2 * q.y * q.y - 2 * q.z * q.z);
        float Roll = Mathf.Rad2Deg * Mathf.Asin(2 * q.x * q.y + 2 * q.z * q.w);

        DebugRangeDifference = new Vector3(Pitch, Yaw, Roll);

        /*
        if(DebugRangeDifference.x > 90 || DebugRangeDifference.x < -90)
        {
            NewRangeDifference.x = -90 - (90 - NewRangeDifference.x);
        }

        if(NewRangeDifference.x > 180)
        {
            NewRangeDifference.x -= 360;
        }

        if(NewRangeDifference.y > 180)
        {
            NewRangeDifference.y -= 360;
        }

        if(NewRangeDifference.z > 180)
        {
            NewRangeDifference.z -= 360;
        }
        */

        CurrentOrientation = DebugRangeDifference;
    }
}
