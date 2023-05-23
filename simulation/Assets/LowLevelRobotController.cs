using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LowLevelRobotController : MonoBehaviour
{
    public IMUController IMU;
    public ArticulationBody Torso;

    [SerializeField]
    public int[] LegNumbering;

    [SerializeField]
    public ArticulationBody[] UpperLegs;

    [SerializeField]
    public ArticulationBody[] UnderLegs;

    [SerializeField]
    public ArticulationBody[] foots;

    public CollisionDetector TorsoDetector;

    [SerializeField]
    public CollisionDetector[] UpperLegDetectors;

    [SerializeField]
    public CollisionDetector[] UnderLegDetectors;

    [SerializeField]
    public CollisionDetector[] FootDetectors;

    public float[,] Angles;

    public float[,] Links;

    public void SetLegConfiguration(int legIndex, float theta1, float theta2, float theta3)
    {
        if(legIndex < 0 || legIndex > UpperLegs.Length - 1)
        {
            Debug.Log("Robot tried to access a leg that didn't exist.");
            return;
        }

        if((legIndex == 1 || legIndex == 4) && Broken == true)
        {
            theta1 = 0;
            theta2 = -20;
            theta3 = -45;
        }

        ArticulationDrive drive = UpperLegs[legIndex].yDrive; // Vertical
        drive.target = theta1;
        UpperLegs[legIndex].yDrive = drive;

        ArticulationDrive drive2 = UpperLegs[legIndex].zDrive; // Horizontal
        drive2.target = theta2;
        UpperLegs[legIndex].zDrive = drive2;

        ArticulationDrive drive3 = UnderLegs[legIndex].xDrive; // Underleg
        drive3.target = theta3;
        UnderLegs[legIndex].xDrive = drive3;
    }

    public void ForceLegConfiguration(int legIndex, float theta1, float theta2, float theta3)
    {
        SetLegConfiguration(legIndex, theta1, theta2, theta3);

        UpperLegs[legIndex].jointPosition = new ArticulationReducedSpace(theta1 * Mathf.Deg2Rad, theta2 * Mathf.Deg2Rad, 0.0f);
        UpperLegs[legIndex].jointVelocity = new ArticulationReducedSpace(0.0f, 0.0f, 0.0f);
        UpperLegs[legIndex].jointAcceleration = new ArticulationReducedSpace(0.0f, 0.0f, 0.0f);
        UpperLegs[legIndex].jointForce = new ArticulationReducedSpace(0.0f, 0.0f, 0.0f);

        UnderLegs[legIndex].jointPosition = new ArticulationReducedSpace(theta3 * Mathf.Deg2Rad);
        UnderLegs[legIndex].jointVelocity = new ArticulationReducedSpace(0.0f, 0.0f, 0.0f);
        UnderLegs[legIndex].jointAcceleration = new ArticulationReducedSpace(0.0f, 0.0f, 0.0f);
        UnderLegs[legIndex].jointForce = new ArticulationReducedSpace(0.0f, 0.0f, 0.0f);

        UpperLegs[legIndex].velocity = Vector3.zero;
        UpperLegs[legIndex].velocity = Vector3.zero;

        UpperLegs[legIndex].angularVelocity = Vector3.zero;
        UpperLegs[legIndex].angularVelocity = Vector3.zero;


        
    }

    public void ResetRobotPositionAndOrientation(Vector3 startPosition, Quaternion startOrientation, bool RandomOrientation)
    {
        if(RandomOrientation)
        {
            startOrientation = Quaternion.Euler(0, UnityEngine.Random.Range(0, 360), 0);
        }

        Torso.TeleportRoot(startPosition, startOrientation);
        Torso.jointVelocity = new ArticulationReducedSpace(0.0f, 0.0f, 0.0f);
        Torso.jointAcceleration = new ArticulationReducedSpace(0.0f, 0.0f, 0.0f);
        Torso.jointForce = new ArticulationReducedSpace(0.0f, 0.0f, 0.0f);
        Torso.velocity = Vector3.zero;
        Torso.angularVelocity = Vector3.zero;

        for(int i = 0; i < UpperLegs.Length; i++)
        {
            ForceLegConfiguration(i, 0, 0, 0);
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

        for(int i = 0; i < UpperLegs.Length; i++)
        {
            ForceLegConfiguration(i, 0, 0, 0);
        }
    }

    public Vector3 DebugRotation;
    private Vector3 lastOrientation;
    void Awake()
    {
        lastOrientation = this.transform.rotation.eulerAngles;

        // Debug message
        if(LegNumbering.Length != UpperLegs.Length)
        {
            Debug.Log("Leg numbering and number of legs doesn't match");
        }
        FoundPoints = new Vector3[6]{new Vector3(0,0,0), new Vector3(0,0,0), new Vector3(0,0,0), new Vector3(0,0,0), new Vector3(0,0,0), new Vector3(0,0,0)};

        if(UpperLegs.Length == 6)
        {
            Links = new float[6, 3] {{0.742f, 0.779f, 0.587f}, {0.742f, 0.779f, 0.587f}, {0.742f, 0.779f, 0.587f}, {0.742f, 0.779f, 0.587f}, {0.742f, 0.779f, 0.587f}, {0.742f, 0.779f, 0.587f}};
            Angles = new float[6, 1] {{305}, {270}, {235}, {125}, {90}, {55}};
        }
        else
        {
            Links = new float[6, 3] {{0.742f, 0.779f, 0.587f}, {0.742f, 0.779f, 0.587f}, {0.742f, 0.779f, 0.587f}, {0.742f, 0.779f, 0.587f}, {0, 0, 0}, {0, 0, 0}};
            Angles = new float[6, 1] {{305}, {235}, {125}, {55}, {0} , {0}};
        }
    }

    void FixedUpdate()
    {
        // Use for debugging
        
        /*
        FoundPoints[0] = IndividualLegForwardKinematics(0) + this.transform.position;
        FoundPoints[1] = IndividualLegForwardKinematics(1) + this.transform.position;
        FoundPoints[2] = IndividualLegForwardKinematics(2) + this.transform.position;
        FoundPoints[3] = IndividualLegForwardKinematics(3) + this.transform.position;
        FoundPoints[4] = IndividualLegForwardKinematics(4) + this.transform.position;
        FoundPoints[5] = IndividualLegForwardKinematics(5) + this.transform.position;
        */

        /*
        DebugDistancesToGround = new float[6] {0, 0, 0, 0, 0, 0};
        for(int i = 0; i < foots.Length; i++)
        {
            DebugDistancesToGround[i] = FootDistanceToGround(i);
        }
        */

        // Debug.Log(FoundPoints[0] - this.Torso.transform.position);
        
        /*
        if(Broken == true)
        {
            DamageHappend();
        }
        */
    }

    public Vector3[] FoundPoints;

    public float[] DebugDistancesToGround;

    public LayerMask FloorDetectableMask;
    public float FootDistanceToGround(int LegNumber)
    {
        if(FootDetectors[LegNumber].Broken == true || LegNumber >= foots.Length)
        {   
            return -100;
        }

        RaycastHit hit; // https://docs.unity3d.com/ScriptReference/Physics.Raycast.html
        if(Physics.Raycast(foots[LegNumber].transform.position, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity, FloorDetectableMask))
        {
            // Debug.DrawRay(foots[LegNumber].transform.position, transform.TransformDirection(Vector3.down) * hit.distance, Color.red);
            return hit.distance;
        }
        return -1;
    }

    public float IndividualFootStatus(int LegNumber)
    {
        if(LegNumber > 5)
        {
            Debug.Log("Tried to get food collisions from a leg that isn't possible.");
        }

        if(LegNumber > UpperLegs.Length - 1)
        {
            return -1;
        }

        if(FootDetectors[LegNumber].IsColliding == true)
        {
            return 1;
        }
        return 0;
    }

    public float[] FeetStatus()
    {
        float[] returnList = new float[FootDetectors.Length];

        for(int i = 0; i < FootDetectors.Length; i++)
        {
            returnList[i] = 0;
            if(FootDetectors[i].IsColliding == true)
            {
                returnList[i] = 1;
            }
        }

        return returnList;
    }

    private void OnDrawGizmos()
    {
        // Use for debugging
        
        Gizmos.color = Color.green;
        for(int i = 0; i < FoundPoints.Length; i++)
        {
            Gizmos.DrawSphere(FoundPoints[i], 0.2f);
        }
        
        float[] FeetCollisions = new float[6]{0, 0, 0, 0, 0, 0};
        Vector3[] FeetLocations = new Vector3[6]{new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0)};

        FeetCollisions[0] = IndividualFootStatus(0);
        FeetCollisions[1] = IndividualFootStatus(1);
        FeetCollisions[2] = IndividualFootStatus(2);
        FeetCollisions[3] = IndividualFootStatus(3);
        FeetCollisions[4] = IndividualFootStatus(4);
        FeetCollisions[5] = IndividualFootStatus(5);

        /*
        FeetLocations[0] = IndividualLegForwardKinematics(0);
        FeetLocations[1] = IndividualLegForwardKinematics(1);
        FeetLocations[2] = IndividualLegForwardKinematics(2);
        FeetLocations[3] = IndividualLegForwardKinematics(3);
        FeetLocations[4] = IndividualLegForwardKinematics(4);
        FeetLocations[5] = IndividualLegForwardKinematics(5);
        */

        // ProjectedCenterOfMass = new Vector3(0, -0.5f, 0);
        float VerticalProjectedCenterOfMass = 0;
        int NumberOfLegs = 0;

        int FirstLegTouchingTheGround = -1;
        int LastLegTouchingTheGround = -1;
        for(int i = 0; i < FeetCollisions.Length - 1; i++)
        {
            for(int j = i + 1; j < FeetCollisions.Length; j++)
            {
                if(FeetCollisions[i] == 1 && FeetCollisions[j] == 1)
                {
                    LastLegTouchingTheGround = j;
                    if(FirstLegTouchingTheGround == -1)
                    {
                        FirstLegTouchingTheGround = i;
                    }

                    Gizmos.DrawLine(FeetLocations[i] + this.transform.position, FeetLocations[j] + this.transform.position);
                    VerticalProjectedCenterOfMass += FeetLocations[i].y;
                    NumberOfLegs++;
                    //DrawTriangle(FeetLocations[i] + this.transform.position, FeetLocations[j] + this.transform.position, this.transform.position);
                    break;
                }
            }
        }

        if(FirstLegTouchingTheGround != -1 && LastLegTouchingTheGround != -1)
        {
            if(FeetCollisions[FirstLegTouchingTheGround] == 1 && FeetCollisions[LastLegTouchingTheGround] == 1) // Only there as a catch
            {
                Gizmos.DrawLine(FeetLocations[FirstLegTouchingTheGround] + this.transform.position, FeetLocations[LastLegTouchingTheGround] + this.transform.position);
                VerticalProjectedCenterOfMass += FeetLocations[LastLegTouchingTheGround].y;
                NumberOfLegs++;
            }
        }
    }


    private float L1 = 0.742f;
    private float L2 = 0.779f;
    private float L3 = 0.587f;
    public float[] IndividualLegInverseKinematics(int LegNumber, Vector3 TCP)
    {
        Vector3 ShoulderLocation;

        Vector3 TorsoToLeg = new Vector3(0, 0, 0.742f);
        float Angle = 305 - 35 * (LegNumbering[LegNumber] - 1);

        // Quaternion rotationTorso;
        //rotationTorso = Quaternion.Euler(IMU.CurrentOrientation.z, Angle + IMU.CurrentOrientation.y, 0);
        if(LegNumber > UpperLegs.Length / 2 - 1)
        {
            Angle -= 75;
            //rotationTorso = Quaternion.Euler(-IMU.CurrentOrientation.z, Angle + IMU.CurrentOrientation.y, 0);
        }

        Matrix4x4 mB1 = Matrix4x4.Rotate(Quaternion.Euler(IMU.CurrentOrientation.x, IMU.CurrentOrientation.y, IMU.CurrentOrientation.z));

        Quaternion rotationTorso = Quaternion.Euler(0, Angle, 0);
        Matrix4x4 m1 = Matrix4x4.Rotate(rotationTorso);

        // ShoulderLocation = this.transform.position + mB1.MultiplyPoint3x4(m1.MultiplyPoint3x4(TorsoToLeg));
        ShoulderLocation = new Vector3(0, this.transform.position.y, 0) + m1.MultiplyPoint3x4(TorsoToLeg);

        TCP = TCP - ShoulderLocation;

        float theta1;
        float theta2;
        float theta3;

        theta1 = Mathf.Atan(TCP.x/TCP.z) * Mathf.Rad2Deg;        

        float num = -Mathf.Pow(L3, 2) + Mathf.Pow(L2, 2) + Mathf.Pow(TCP.x, 2) + Mathf.Pow(TCP.y, 2) + Mathf.Pow(TCP.z, 2);

        float denum = 2 * L2 * Mathf.Sqrt(Mathf.Pow(TCP.x, 2) + Mathf.Pow(TCP.y, 2) + Mathf.Pow(TCP.z, 2));

        float term1 = Mathf.Acos(num/denum);
        float term2 = Mathf.Atan(TCP.y / Mathf.Sqrt( Mathf.Pow(TCP.x, 2) + Mathf.Pow(TCP.z, 2)));

        theta2 = (term1 + term2) * Mathf.Rad2Deg;

        float num2 = Mathf.Pow(TCP.x, 2) + Mathf.Pow(TCP.y, 2) + Mathf.Pow(TCP.z, 2) - Mathf.Pow(L2, 2) - Mathf.Pow(L3, 2);
        float denum2 = 2 * L2 * L3;
        theta3 = Mathf.Acos(num2/denum2) * Mathf.Rad2Deg;

        float offset = 0;
        if(LegNumbering[LegNumber] == 1)
        {
            offset = 55;
        }
        if(LegNumbering[LegNumber] == 2)
        {
            offset = 90;
        }
        if(LegNumbering[LegNumber] == 3)
        {
            offset = -55;
        }
        if(LegNumbering[LegNumber] == 4)
        {
            offset = 55;
        }
        if(LegNumbering[LegNumber] == 5)
        {
            offset = -90;
        }
        if(LegNumbering[LegNumber] == 6)
        {
            offset = -55;
        }
                        
        

        return new float[3] {-(theta1 + offset), theta2 + 20, theta3 - 45};
    }

    public bool Broken = false;

    public bool DamageHasHappend = false;

    public void DamageHappend()
    {
        if(DamageHasHappend == true)
        {
            return;
        }

        // Leg 2
        UpperLegs[1].transform.GetComponent<MeshRenderer>().enabled = false;
        UnderLegs[1].transform.GetComponent<MeshRenderer>().enabled = false;
        foots[1].transform.GetComponent<MeshRenderer>().enabled = false;

        UpperLegs[1].transform.GetComponent<MeshCollider>().enabled = false;
        UnderLegs[1].transform.GetComponent<MeshCollider>().enabled = false;
        foots[1].transform.GetComponent<SphereCollider>().enabled = false;

        UpperLegDetectors[1].Broken = true;
        UnderLegDetectors[1].Broken = true;
        FootDetectors[1].Broken = true;

        // Leg 5
        UpperLegs[4].transform.GetComponent<MeshRenderer>().enabled = false;
        UnderLegs[4].transform.GetComponent<MeshRenderer>().enabled = false;
        foots[4].transform.GetComponent<MeshRenderer>().enabled = false;

        UpperLegs[4].transform.GetComponent<MeshCollider>().enabled = false;
        UnderLegs[4].transform.GetComponent<MeshCollider>().enabled = false;
        foots[4].transform.GetComponent<SphereCollider>().enabled = false;

        UpperLegDetectors[4].Broken = true;
        UnderLegDetectors[4].Broken = true;
        FootDetectors[4].Broken = true;

        DamageHasHappend = true;

        /*
        UpperLegs[4].transform.GetComponent<MeshRenderer>().enabled = false;
        UnderLegs[4].transform.GetComponent<MeshRenderer>().enabled = false;
        foots[4].transform.GetComponent<MeshRenderer>().enabled = false;

        UpperLegs[4].transform.GetComponent<MeshCollider>().enabled = false;
        UnderLegs[4].transform.GetComponent<MeshCollider>().enabled = false;
        foots[4].transform.GetComponent<SphereCollider>().enabled = false;

        UpperLegDetectors[4].Broken = true;
        UnderLegDetectors[4].Broken = true;
        FootDetectors[4].Broken = true;
        */
    }

    public void DamageUnHappened()
    {
        // Leg 2
        UpperLegs[1].transform.GetComponent<MeshRenderer>().enabled = true;
        UnderLegs[1].transform.GetComponent<MeshRenderer>().enabled = true;
        foots[1].transform.GetComponent<MeshRenderer>().enabled = true;

        UpperLegs[1].transform.GetComponent<MeshCollider>().enabled = true;
        UnderLegs[1].transform.GetComponent<MeshCollider>().enabled = true;
        foots[1].transform.GetComponent<SphereCollider>().enabled = true;

        UpperLegDetectors[1].Broken = false;
        UnderLegDetectors[1].Broken = false;
        FootDetectors[1].Broken = false;

        // Leg 5
        UpperLegs[4].transform.GetComponent<MeshRenderer>().enabled = true;
        UnderLegs[4].transform.GetComponent<MeshRenderer>().enabled = true;
        foots[4].transform.GetComponent<MeshRenderer>().enabled = true;

        UpperLegs[4].transform.GetComponent<MeshCollider>().enabled = true;
        UnderLegs[4].transform.GetComponent<MeshCollider>().enabled = true;
        foots[4].transform.GetComponent<SphereCollider>().enabled = true;

        UpperLegDetectors[4].Broken = false;
        UnderLegDetectors[4].Broken = false;
        FootDetectors[4].Broken = false;

        DamageHasHappend = false;
    }

}
