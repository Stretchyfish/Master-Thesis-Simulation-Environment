using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadrupedRobotController : MonoBehaviour
{
    public IMUController IMU;
    public ArticulationBody Torso;
    public Transform Sensor;
    public float SensorRange = 1;

    public int NumberOfSensors = 1;

    public ArticulationBody UpperLeg1;
    public ArticulationBody UpperLeg2;
    public ArticulationBody UpperLeg3;
    public ArticulationBody UpperLeg4;

    public ArticulationBody UnderLeg1;
    public ArticulationBody UnderLeg2;
    public ArticulationBody UnderLeg3;
    public ArticulationBody UnderLeg4;

    public ArticulationBody Foot1;
    public ArticulationBody Foot2;
    public ArticulationBody Foot3;
    public ArticulationBody Foot4;

    public CollisionDetector TorsoCollision;

    public CollisionDetector UpperLeg1Collision;
    public CollisionDetector UpperLeg2Collision;
    public CollisionDetector UpperLeg3Collision;
    public CollisionDetector UpperLeg4Collision;

    public CollisionDetector UnderLeg1Collision;
    public CollisionDetector UnderLeg2Collision;
    public CollisionDetector UnderLeg3Collision;
    public CollisionDetector UnderLeg4Collision;

    public CollisionDetector Foot1Collision;
    public CollisionDetector Foot2Collision;
    public CollisionDetector Foot3Collision;
    public CollisionDetector Foot4Collision;

    public float NormalizedSensorDistance1 = 1;
    public float NormalizedSensorDistance2 = 1;


    public void ForceLegConfiguration(int legIndex, float theta1, float theta2, float theta3)
    {
        if(legIndex == 0)
        {
            SetLegConfiguration(0, theta1, theta2, theta3);

            UpperLeg1.jointPosition = new ArticulationReducedSpace(theta1 * Mathf.Deg2Rad, theta2 * Mathf.Deg2Rad, 0.0f);
            UpperLeg1.jointVelocity = new ArticulationReducedSpace(0.0f, 0.0f, 0.0f);
            UpperLeg1.jointAcceleration = new ArticulationReducedSpace(0.0f, 0.0f, 0.0f);
            UpperLeg1.jointForce = new ArticulationReducedSpace(0.0f, 0.0f, 0.0f);

            UnderLeg1.jointPosition = new ArticulationReducedSpace(theta3 * Mathf.Deg2Rad);
            UnderLeg1.jointVelocity = new ArticulationReducedSpace(0.0f, 0.0f, 0.0f);
            UnderLeg1.jointAcceleration = new ArticulationReducedSpace(0.0f, 0.0f, 0.0f);
            UnderLeg1.jointForce = new ArticulationReducedSpace(0.0f, 0.0f, 0.0f);

            UpperLeg1.velocity = Vector3.zero;
            UnderLeg1.velocity = Vector3.zero;

            UpperLeg1.angularVelocity = Vector3.zero;
            UnderLeg1.angularVelocity = Vector3.zero;
        }

        if (legIndex == 1)
        {
            SetLegConfiguration(1, theta1, theta2, theta3);

            UpperLeg2.jointPosition = new ArticulationReducedSpace(theta1 * Mathf.Deg2Rad, theta2 * Mathf.Deg2Rad, 0.0f);
            UpperLeg2.jointVelocity = new ArticulationReducedSpace(0.0f, 0.0f, 0.0f);
            UpperLeg2.jointAcceleration = new ArticulationReducedSpace(0.0f, 0.0f, 0.0f);
            UpperLeg2.jointForce = new ArticulationReducedSpace(0.0f, 0.0f, 0.0f);

            UnderLeg2.jointPosition = new ArticulationReducedSpace(theta3 * Mathf.Deg2Rad);
            UnderLeg2.jointVelocity = new ArticulationReducedSpace(0.0f, 0.0f, 0.0f);
            UnderLeg2.jointAcceleration = new ArticulationReducedSpace(0.0f, 0.0f, 0.0f);
            UnderLeg2.jointForce = new ArticulationReducedSpace(0.0f, 0.0f, 0.0f);

            UpperLeg2.velocity = Vector3.zero;
            UnderLeg2.velocity = Vector3.zero;

            UpperLeg2.angularVelocity = Vector3.zero;
            UnderLeg2.angularVelocity = Vector3.zero;
        }

        if (legIndex == 2)
        {
            SetLegConfiguration(2, theta1, theta2, theta3);

            UpperLeg3.jointPosition = new ArticulationReducedSpace(theta1 * Mathf.Deg2Rad, theta2 * Mathf.Deg2Rad, 0.0f);
            UpperLeg3.jointVelocity = new ArticulationReducedSpace(0.0f, 0.0f, 0.0f);
            UpperLeg3.jointAcceleration = new ArticulationReducedSpace(0.0f, 0.0f, 0.0f);
            UpperLeg3.jointForce = new ArticulationReducedSpace(0.0f, 0.0f, 0.0f);

            UnderLeg3.jointPosition = new ArticulationReducedSpace(theta3 * Mathf.Deg2Rad);
            UnderLeg3.jointVelocity = new ArticulationReducedSpace(0.0f, 0.0f, 0.0f);
            UnderLeg3.jointAcceleration = new ArticulationReducedSpace(0.0f, 0.0f, 0.0f);
            UnderLeg3.jointForce = new ArticulationReducedSpace(0.0f, 0.0f, 0.0f);
        
            UpperLeg3.velocity = Vector3.zero;
            UnderLeg3.velocity = Vector3.zero;

            UpperLeg3.angularVelocity = Vector3.zero;
            UnderLeg3.angularVelocity = Vector3.zero;
        }

        if (legIndex == 3)
        {
            SetLegConfiguration(3, theta1, theta2, theta3);

            UpperLeg4.jointPosition = new ArticulationReducedSpace(theta1 * Mathf.Deg2Rad, theta2 * Mathf.Deg2Rad, 0.0f);
            UpperLeg4.jointVelocity = new ArticulationReducedSpace(0.0f, 0.0f, 0.0f);
            UpperLeg4.jointAcceleration = new ArticulationReducedSpace(0.0f, 0.0f, 0.0f);
            UpperLeg4.jointForce = new ArticulationReducedSpace(0.0f, 0.0f, 0.0f);

            UnderLeg4.jointPosition = new ArticulationReducedSpace(theta3 * Mathf.Deg2Rad);
            UnderLeg4.jointVelocity = new ArticulationReducedSpace(0.0f, 0.0f, 0.0f);
            UnderLeg4.jointAcceleration = new ArticulationReducedSpace(0.0f, 0.0f, 0.0f);
            UnderLeg4.jointForce = new ArticulationReducedSpace(0.0f, 0.0f, 0.0f);

            UpperLeg4.velocity = Vector3.zero;
            UnderLeg4.velocity = Vector3.zero;

            UpperLeg4.angularVelocity = Vector3.zero;
            UnderLeg4.angularVelocity = Vector3.zero;
        }
    }

    public void ResetRobot(Vector3 initialPosition, bool RandomOrientation)
    {
        Quaternion startOrientation = Quaternion.identity;
        if(RandomOrientation)
        {
            startOrientation = Quaternion.Euler(0, UnityEngine.Random.Range(0, 360), 0);
        }

        Torso.TeleportRoot(initialPosition, startOrientation);
        Torso.jointVelocity = new ArticulationReducedSpace(0.0f, 0.0f, 0.0f);
        Torso.jointAcceleration = new ArticulationReducedSpace(0.0f, 0.0f, 0.0f);
        Torso.jointForce = new ArticulationReducedSpace(0.0f, 0.0f, 0.0f);
        Torso.velocity = Vector3.zero;
        Torso.angularVelocity = Vector3.zero;

        ForceLegConfiguration(0, 0, 0, 0);
        ForceLegConfiguration(1, 0, 0, 0);
        ForceLegConfiguration(2, 0, 0, 0);
        ForceLegConfiguration(3, 0, 0, 0);
    }

    public void SetLegConfiguration(int legIndex, float theta1, float theta2, float theta3)
    {
        if (legIndex == 0)
        {
            ArticulationDrive drive = UpperLeg1.yDrive;
            drive.target = theta1;
            UpperLeg1.yDrive = drive;

            ArticulationDrive drive2 = UpperLeg1.zDrive;
            drive2.target = -theta2;
            UpperLeg1.zDrive = drive2;

            ArticulationDrive drive3 = UnderLeg1.xDrive;
            drive3.target = theta3;
            UnderLeg1.xDrive = drive3;

            return;
        }

        if (legIndex == 1)
        {
            ArticulationDrive drive = UpperLeg2.yDrive;
            drive.target = theta1;
            UpperLeg2.yDrive = drive;

            ArticulationDrive drive2 = UpperLeg2.zDrive;
            drive2.target = -theta2;
            UpperLeg2.zDrive = drive2;

            ArticulationDrive drive3 = UnderLeg2.xDrive;
            drive3.target = theta3;
            UnderLeg2.xDrive = drive3;
            return;
        }

        if (legIndex == 2)
        {
            ArticulationDrive drive = UpperLeg3.yDrive;
            drive.target = theta1;
            UpperLeg3.yDrive = drive;

            ArticulationDrive drive2 = UpperLeg3.zDrive;
            drive2.target = theta2;
            UpperLeg3.zDrive = drive2;

            ArticulationDrive drive3 = UnderLeg3.xDrive;
            drive3.target = theta3;
            UnderLeg3.xDrive = drive3;
            return;
        }

        if (legIndex == 3)
        {
            ArticulationDrive drive = UpperLeg4.yDrive;
            drive.target = theta1;
            UpperLeg4.yDrive = drive;

            ArticulationDrive drive2 = UpperLeg4.zDrive;
            drive2.target = theta2;
            UpperLeg4.zDrive = drive2;

            ArticulationDrive drive3 = UnderLeg4.xDrive;
            drive3.target = theta3;
            UnderLeg4.xDrive = drive3;
            return;
        }
    
        Debug.Log("Robot tried to access leg that does not exist.");
    }

    // Start is called before the first frame update
    void Awake()
    {
        lastOrientation = this.transform.rotation.eulerAngles;
    }

    private Vector3 lastOrientation;
    public Vector3 GetBodyAngularVelocity()
    {
        Vector3 deltaRot = this.transform.rotation.eulerAngles - lastOrientation;
        lastOrientation = this.transform.rotation.eulerAngles;

        return deltaRot;
    }

    public Vector3 GetAverageVelocity()
    {
        Vector3 VelocitySum = Vector3.zero;

        VelocitySum += Torso.velocity;
        // 1

        VelocitySum += UpperLeg1.velocity;
        VelocitySum += UpperLeg2.velocity;
        VelocitySum += UpperLeg3.velocity;
        VelocitySum += UpperLeg4.velocity;
        // 7

        VelocitySum += UnderLeg1.velocity;
        VelocitySum += UnderLeg2.velocity;
        VelocitySum += UnderLeg3.velocity;
        VelocitySum += UnderLeg4.velocity;
        // 13

        VelocitySum += Foot1.velocity;
        VelocitySum += Foot2.velocity;
        VelocitySum += Foot3.velocity;
        VelocitySum += Foot4.velocity;
        // 19

        return VelocitySum / 13;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(this.transform.position, this.transform.position + GetAverageVelocity());

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(this.transform.position + this.Torso.centerOfMass, 0.1f);

        //Gizmos.DrawLine(Sensor.position, Sensor.position + new Vector3(1.5f, 0, 1.5f));
        //Debug.DrawRay(Sensor.position, Vector3.forward, Color.yellow);

        for(float angle = 50; angle > 0; angle -= 50 / NumberOfSensors)
        {
            Debug.DrawRay(Sensor.position, Sensor.TransformDirection(Quaternion.Euler(0, 0, angle) * Vector3.up) * SensorRange, Color.yellow);
            Debug.DrawRay(Sensor.position, Sensor.TransformDirection(Quaternion.Euler(0, 0, -angle) * Vector3.up) * SensorRange, Color.yellow);
        }
    }

    public float RightSensorStatus()
    {
        for (float angle = 50; angle > 0; angle -= 50 / NumberOfSensors)
        {
            RaycastHit hit;
            if (Physics.Raycast(Sensor.position, Sensor.TransformDirection(Quaternion.Euler(0, 0, angle) * Vector3.up), out hit, SensorRange))
            {
                if (hit.transform.tag == "RobotCollidable")
                {
                    NormalizedSensorDistance2 = 1- hit.distance / SensorRange;
                    Debug.DrawRay(Sensor.position, Sensor.TransformDirection(Quaternion.Euler(0, 0, angle) * Vector3.up) * SensorRange, Color.red);
                    return NormalizedSensorDistance2;
                }
            }
        }

        return 1;
    }

    public float changeAngle = 0;

    public float LeftSensorStatus()
    {
        for (float angle = 50; angle > 0; angle -= 50 / NumberOfSensors)
        {
            RaycastHit hit;
            if (Physics.Raycast(Sensor.position, Sensor.TransformDirection(Quaternion.Euler(0, 0, -angle) * Vector3.up), out hit, SensorRange))
            {
                if (hit.transform.tag == "RobotCollidable")
                {
                    NormalizedSensorDistance1 = 1 - hit.distance / SensorRange;
                    Debug.DrawRay(Sensor.position, Sensor.TransformDirection(Quaternion.Euler(0, 0, -angle) * Vector3.up) * SensorRange, Color.red);
                    return NormalizedSensorDistance2;
                }
            }
        }

        return 1;
    }

    public Vector3 GetCenterOfMass()
    {
        return this.Torso.centerOfMass;
    }

    public float[] FeetStatus()
    {
        float[] returnList = new float[4] { 0, 0, 0, 0 };

        if(Foot1Collision.IsColliding == true)
        {
            returnList[0] = 1;
        }

        if(Foot2Collision.IsColliding == true)
        {
            returnList[1] = 1;
        }

        if(Foot3Collision.IsColliding == true)
        {
            returnList[2] = 1;
        }

        if(Foot4Collision.IsColliding == true)
        {
            returnList[3] = 1;
        }

        return returnList;
    }

    public Vector3 LegForwardKinematics(int LegIndex)
    {
        // Missing the actual kinematics

        /*
        Leg1 = UpperLeg1.transform.position;

        Quaternion RotationLeg1 = Quaternion.Euler(-90 + UpperLeg1.zDrive.target, 0 , -140 + UpperLeg1.yDrive.target);
        Matrix4x4 RotationMatrix = Matrix4x4.Rotate(RotationLeg1);

        Leg1Estimate = Torso.transform.position + RotationMatrix.MultiplyPoint3x4(new Vector3(-0.60853f, -0.001180887f, 0.425265f));

        leg2 = UnderLeg1.transform.position;

        Quaternion RotationLeg2 = Quaternion.Euler(UnderLeg1.xDrive.target, 0, 0);
        Matrix4x4 RotationMatrix2 = Matrix4x4.Rotate(RotationLeg2);
        Leg2Estimate = Leg1Estimate + RotationMatrix.MultiplyPoint3x4(new Vector3(-0.008130312f, 0, 0));
        */

        // This needs to be changed
 
        Vector3 LocalDistanceToLeg = Vector3.zero;
        switch(LegIndex)
        {
            case 0:
                LocalDistanceToLeg = Foot1.transform.position - Torso.transform.position;
                break;

            case 1:
                LocalDistanceToLeg = Foot2.transform.position - Torso.transform.position;
                break;

            case 2:
                LocalDistanceToLeg = Foot3.transform.position - Torso.transform.position;
                break;

            case 3:
                LocalDistanceToLeg = Foot4.transform.position - Torso.transform.position;
                break;

            default:
                Debug.Log("Index in forward kinematics wrong");
                break;
        }

        return LocalDistanceToLeg;
    }


}