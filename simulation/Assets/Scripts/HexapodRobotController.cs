using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;

public class HexapodRobotController : MonoBehaviour
{
    public IMUController IMU;
    public Transform Sensor;
    public float SensorRange = 1;

    public int NumberOfSensors = 1; 

    public ArticulationBody Torso;

    public ArticulationBody UpperLeg1;
    public ArticulationBody UpperLeg2;
    public ArticulationBody UpperLeg3; 
    public ArticulationBody UpperLeg4;
    public ArticulationBody UpperLeg5;
    public ArticulationBody UpperLeg6;

    public ArticulationBody UnderLeg1;
    public ArticulationBody UnderLeg2;
    public ArticulationBody UnderLeg3;
    public ArticulationBody UnderLeg4;
    public ArticulationBody UnderLeg5;
    public ArticulationBody UnderLeg6;

    public ArticulationBody Foot1;
    public ArticulationBody Foot2;
    public ArticulationBody Foot3;
    public ArticulationBody Foot4;
    public ArticulationBody Foot5;
    public ArticulationBody Foot6;

    public CollisionDetector TorsoCollision;

    public CollisionDetector UpperLeg1Collision;
    public CollisionDetector UpperLeg2Collision;
    public CollisionDetector UpperLeg3Collision;
    public CollisionDetector UpperLeg4Collision;
    public CollisionDetector UpperLeg5Collision;
    public CollisionDetector UpperLeg6Collision;

    public CollisionDetector UnderLeg1Collision;
    public CollisionDetector UnderLeg2Collision;
    public CollisionDetector UnderLeg3Collision;
    public CollisionDetector UnderLeg4Collision;
    public CollisionDetector UnderLeg5Collision;
    public CollisionDetector UnderLeg6Collision;

    public CollisionDetector Foot1Collision;
    public CollisionDetector Foot2Collision;
    public CollisionDetector Foot3Collision;
    public CollisionDetector Foot4Collision;
    public CollisionDetector Foot5Collision;
    public CollisionDetector Foot6Collision;

    public float NormalizedSensorDistance1 = 1;
    public float NormalizedSensorDistance2 = 1;

    public void SetLegConfiguration(int legIndex, float theta1, float theta2, float theta3)
    {
        if(legIndex == 0)
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

        if(legIndex == 1)
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

        if(legIndex == 2)
        {
            ArticulationDrive drive = UpperLeg3.yDrive;
            drive.target = theta1;
            UpperLeg3.yDrive = drive;

            ArticulationDrive drive2 = UpperLeg3.zDrive;
            drive2.target = -theta2;
            UpperLeg3.zDrive = drive2;

            ArticulationDrive drive3 = UnderLeg3.xDrive;
            drive3.target = theta3;
            UnderLeg3.xDrive = drive3;
            return;
        }

        if(legIndex == 3)
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

        if (legIndex == 4)
        {
            ArticulationDrive drive = UpperLeg5.yDrive;
            drive.target = theta1;
            UpperLeg5.yDrive = drive;

            ArticulationDrive drive2 = UpperLeg5.zDrive;
            drive2.target = theta2;
            UpperLeg5.zDrive = drive2;

            ArticulationDrive drive3 = UnderLeg5.xDrive;
            drive3.target = theta3;
            UnderLeg5.xDrive = drive3;
            return;
        }

        if (legIndex == 5)
        {
            ArticulationDrive drive = UpperLeg6.yDrive;
            drive.target = theta1;
            UpperLeg6.yDrive = drive;

            ArticulationDrive drive2 = UpperLeg6.zDrive;
            drive2.target = theta2;
            UpperLeg6.zDrive = drive2;

            ArticulationDrive drive3 = UnderLeg6.xDrive;
            drive3.target = theta3;
            UnderLeg6.xDrive = drive3;
            return;
        }

        Debug.Log("Robot tried to access leg that does not exist.");
    }

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

        if (legIndex == 4)
        {
            SetLegConfiguration(4, theta1, theta2, theta3);

            UpperLeg5.jointPosition = new ArticulationReducedSpace(theta1 * Mathf.Deg2Rad, theta2 * Mathf.Deg2Rad, 0.0f);
            UpperLeg5.jointVelocity = new ArticulationReducedSpace(0.0f, 0.0f, 0.0f);
            UpperLeg5.jointAcceleration = new ArticulationReducedSpace(0.0f, 0.0f, 0.0f);
            UpperLeg5.jointForce = new ArticulationReducedSpace(0.0f, 0.0f, 0.0f);

            UnderLeg5.jointPosition = new ArticulationReducedSpace(theta3 * Mathf.Deg2Rad);
            UnderLeg5.jointVelocity = new ArticulationReducedSpace(0.0f, 0.0f, 0.0f);
            UnderLeg5.jointAcceleration = new ArticulationReducedSpace(0.0f, 0.0f, 0.0f);
            UnderLeg5.jointForce = new ArticulationReducedSpace(0.0f, 0.0f, 0.0f);

            UpperLeg5.velocity = Vector3.zero;
            UnderLeg5.velocity = Vector3.zero;

            UpperLeg5.angularVelocity = Vector3.zero;
            UnderLeg5.angularVelocity = Vector3.zero;
        }

        if (legIndex == 5)
        {
            SetLegConfiguration(5, theta1, theta2, theta3);

            UpperLeg6.jointPosition = new ArticulationReducedSpace(theta1 * Mathf.Deg2Rad, theta2 * Mathf.Deg2Rad, 0.0f);
            UpperLeg6.jointVelocity = new ArticulationReducedSpace(0.0f, 0.0f, 0.0f);
            UpperLeg6.jointAcceleration = new ArticulationReducedSpace(0.0f, 0.0f, 0.0f);
            UpperLeg6.jointForce = new ArticulationReducedSpace(0.0f, 0.0f, 0.0f);

            UnderLeg6.jointPosition = new ArticulationReducedSpace(theta3 * Mathf.Deg2Rad);
            UnderLeg6.jointVelocity = new ArticulationReducedSpace(0.0f, 0.0f, 0.0f);
            UnderLeg6.jointAcceleration = new ArticulationReducedSpace(0.0f, 0.0f, 0.0f);
            UnderLeg6.jointForce = new ArticulationReducedSpace(0.0f, 0.0f, 0.0f);

            UpperLeg6.velocity = Vector3.zero;
            UnderLeg6.velocity = Vector3.zero;

            UpperLeg6.angularVelocity = Vector3.zero;
            UnderLeg6.angularVelocity = Vector3.zero;
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
        ForceLegConfiguration(4, 0, 0, 0);
        ForceLegConfiguration(5, 0, 0, 0);
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
        VelocitySum += UpperLeg5.velocity;
        VelocitySum += UpperLeg6.velocity;
        // 7

        VelocitySum += UnderLeg1.velocity;
        VelocitySum += UnderLeg2.velocity;
        VelocitySum += UnderLeg3.velocity;
        VelocitySum += UnderLeg4.velocity;
        VelocitySum += UnderLeg5.velocity;
        VelocitySum += UnderLeg6.velocity;
        // 13

        VelocitySum += Foot1.velocity;
        VelocitySum += Foot2.velocity;
        VelocitySum += Foot3.velocity;
        VelocitySum += Foot4.velocity;
        VelocitySum += Foot5.velocity;
        VelocitySum += Foot6.velocity;
        // 19

        return VelocitySum / 19;
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

        Gizmos.color = Color.green;
        Gizmos.DrawLine(Torso.transform.position, Torso.transform.position + LegForwardKinematics(0));
        Gizmos.DrawLine(Torso.transform.position, Torso.transform.position + LegForwardKinematics(1));
        Gizmos.DrawLine(Torso.transform.position, Torso.transform.position + LegForwardKinematics(2));
        Gizmos.DrawLine(Torso.transform.position, Torso.transform.position + LegForwardKinematics(3));
        Gizmos.DrawLine(Torso.transform.position, Torso.transform.position + LegForwardKinematics(4));
        Gizmos.DrawLine(Torso.transform.position, Torso.transform.position + LegForwardKinematics(5));
    }

    public void BeginRecordingData()
    {
        DataAnalyser.OpenFile("leg1_on_ground");
        DataAnalyser.OpenFile("leg2_on_ground");
        DataAnalyser.OpenFile("leg3_on_ground");
        DataAnalyser.OpenFile("leg4_on_ground");
        DataAnalyser.OpenFile("leg5_on_ground");
        DataAnalyser.OpenFile("leg6_on_ground");

        DataAnalyser.OpenFile("leg1_position_x");
        DataAnalyser.OpenFile("leg1_position_y");
        DataAnalyser.OpenFile("leg1_position_z");

        DataAnalyser.OpenFile("leg2_position_x");
        DataAnalyser.OpenFile("leg2_position_y");
        DataAnalyser.OpenFile("leg2_position_z");

        DataAnalyser.OpenFile("leg3_position_x");
        DataAnalyser.OpenFile("leg3_position_y");
        DataAnalyser.OpenFile("leg3_position_z");

        DataAnalyser.OpenFile("leg4_position_x");
        DataAnalyser.OpenFile("leg4_position_y");
        DataAnalyser.OpenFile("leg4_position_z");

        DataAnalyser.OpenFile("leg5_position_x");
        DataAnalyser.OpenFile("leg5_position_y");
        DataAnalyser.OpenFile("leg5_position_z");

        DataAnalyser.OpenFile("leg6_position_x");
        DataAnalyser.OpenFile("leg6_position_y");
        DataAnalyser.OpenFile("leg6_position_z");

        DataAnalyser.OpenFile("position_x");
        DataAnalyser.OpenFile("position_y");
        DataAnalyser.OpenFile("position_z");

        DataAnalyser.OpenFile("velocity_x");
        DataAnalyser.OpenFile("velocity_y");
        DataAnalyser.OpenFile("velocity_z");

        DataAnalyser.OpenFile("Hex_center_of_mass_x");
        DataAnalyser.OpenFile("Hex_center_of_mass_y");
        DataAnalyser.OpenFile("Hex_center_of_mass_z");
    }

    public void RecordData()
    {
        if(Foot1Collision.IsColliding)
        {
            DataAnalyser.FileWriteLine("leg1_on_ground", 1);
        }
        else
        {
            DataAnalyser.FileWriteLine("leg1_on_ground", 0);
        }

        if (Foot2Collision.IsColliding)
        {
            DataAnalyser.FileWriteLine("leg2_on_ground", 1);
        }
        else
        {
            DataAnalyser.FileWriteLine("leg2_on_ground", 0);
        }

        if (Foot3Collision.IsColliding)
        {
            DataAnalyser.FileWriteLine("leg3_on_ground", 1);
        }
        else
        {
            DataAnalyser.FileWriteLine("leg3_on_ground", 0);
        }

        if (Foot4Collision.IsColliding)
        {
            DataAnalyser.FileWriteLine("leg4_on_ground", 1);
        }
        else
        {
            DataAnalyser.FileWriteLine("leg4_on_ground", 0);
        }

        if (Foot5Collision.IsColliding)
        {
            DataAnalyser.FileWriteLine("leg5_on_ground", 1);
        }
        else
        {
            DataAnalyser.FileWriteLine("leg5_on_ground", 0);
        }

        if (Foot6Collision.IsColliding)
        {
            DataAnalyser.FileWriteLine("leg6_on_ground", 1);
        }
        else
        {
            DataAnalyser.FileWriteLine("leg6_on_ground", 0);
        }

        DataAnalyser.FileWriteLine("leg1_position_x", Foot1.transform.position.x);
        DataAnalyser.FileWriteLine("leg1_position_y", Foot1.transform.position.y);
        DataAnalyser.FileWriteLine("leg1_position_z", Foot1.transform.position.z);

        DataAnalyser.FileWriteLine("leg2_position_x", Foot2.transform.position.x);
        DataAnalyser.FileWriteLine("leg2_position_y", Foot2.transform.position.y);
        DataAnalyser.FileWriteLine("leg2_position_z", Foot2.transform.position.z);

        DataAnalyser.FileWriteLine("leg3_position_x", Foot3.transform.position.x);
        DataAnalyser.FileWriteLine("leg3_position_y", Foot3.transform.position.y);
        DataAnalyser.FileWriteLine("leg3_position_z", Foot3.transform.position.z);

        DataAnalyser.FileWriteLine("leg4_position_x", Foot4.transform.position.x);
        DataAnalyser.FileWriteLine("leg4_position_y", Foot4.transform.position.y);
        DataAnalyser.FileWriteLine("leg4_position_z", Foot4.transform.position.z);

        DataAnalyser.FileWriteLine("leg5_position_x", Foot5.transform.position.x);
        DataAnalyser.FileWriteLine("leg5_position_y", Foot5.transform.position.y);
        DataAnalyser.FileWriteLine("leg5_position_z", Foot5.transform.position.z);

        DataAnalyser.FileWriteLine("leg6_position_x", Foot6.transform.position.x);
        DataAnalyser.FileWriteLine("leg6_position_y", Foot6.transform.position.y);
        DataAnalyser.FileWriteLine("leg6_position_z", Foot6.transform.position.z);

        Vector3 currentVelocity = this.GetAverageVelocity();
        DataAnalyser.FileWriteLine("velocity_x", currentVelocity.x);
        DataAnalyser.FileWriteLine("velocity_y", currentVelocity.y);
        DataAnalyser.FileWriteLine("velocity_z", currentVelocity.z);

        DataAnalyser.FileWriteLine("position_x", this.IMU.transform.position.x);
        DataAnalyser.FileWriteLine("position_y", this.IMU.transform.position.y);
        DataAnalyser.FileWriteLine("position_z", this.IMU.transform.position.z);

        Vector3 currentCenterOfMass = this.IMU.transform.position + this.Torso.centerOfMass;

        DataAnalyser.FileWriteLine("Hex_center_of_mass_x", currentCenterOfMass.x);
        DataAnalyser.FileWriteLine("Hex_center_of_mass_y", currentCenterOfMass.y);
        DataAnalyser.FileWriteLine("Hex_center_of_mass_z", currentCenterOfMass.z);
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
                    NormalizedSensorDistance2 = 1 - hit.distance / SensorRange;
                    Debug.DrawRay(Sensor.position, Sensor.TransformDirection(Quaternion.Euler(0, 0, angle) * Vector3.up) * SensorRange, Color.red);
                    return NormalizedSensorDistance2;
                }
            }
        }
        
        NormalizedSensorDistance2 = 1;
        return NormalizedSensorDistance2;
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
                    return NormalizedSensorDistance1;
                }
            }
        }

        NormalizedSensorDistance1 = 1;
        return NormalizedSensorDistance1;
    }

    public Vector3 GetCenterOfMass()
    {
        return this.Torso.centerOfMass;
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

            case 4:
                LocalDistanceToLeg = Foot5.transform.position - Torso.transform.position;
                break;

            case 5:
                LocalDistanceToLeg = Foot6.transform.position - Torso.transform.position;
                break;

            default:
                Debug.Log("Index in forward kinematics wrong");
                break;
        }

        return LocalDistanceToLeg;
    }

    public float[] FeetStatus()
    {
        float[] returnList = new float[6] { 0, 0, 0, 0, 0, 0 };

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

        if(Foot5Collision.IsColliding == true)
        {
            returnList[4] = 1;
        }

        if(Foot6Collision.IsColliding == true)
        {
            returnList[5] = 1;
        }

        return returnList;
    }
}
