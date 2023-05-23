using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexapodController : MonoBehaviour
{
    public LowLevelRobotController controller;

    public Vector3 TargetCoM;
    public Vector3[] TargetFootTCPs;

    private float ReachbilityRadius = 0.779f + 0.587f;
    private float ReachabilityMargin = 0.1f;

    private void Awake()
    {
        Vector3 StartPos = new Vector3(0, controller.IMU.transform.position.y, 0);
        Vector3 PositionOffset = new Vector3(0, 0, 2f);

        TargetCoM = StartPos + PositionOffset;

        Vector3 FootTCP1 = IndividualLegForwardKinematics(0, 0.742f, 0.779f, 0.587f) + TargetCoM;
        Vector3 FootTCP2 = IndividualLegForwardKinematics(1, 0.742f, 0.779f, 0.587f) + TargetCoM;
        Vector3 FootTCP3 = IndividualLegForwardKinematics(2, 0.742f, 0.779f, 0.587f) + TargetCoM;
        Vector3 FootTCP4 = IndividualLegForwardKinematics(3, 0.742f, 0.779f, 0.587f) + TargetCoM;
        Vector3 FootTCP5 = IndividualLegForwardKinematics(4, 0.742f, 0.779f, 0.587f) + TargetCoM;
        Vector3 FootTCP6 = IndividualLegForwardKinematics(5, 0.742f, 0.779f, 0.587f) + TargetCoM;
        TargetFootTCPs = new Vector3[6] {FootTCP1, FootTCP2, FootTCP3, FootTCP4, FootTCP5, FootTCP6};
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    Vector3 UnitVectorDemo;
    private void FixedUpdate()
    {
        LinkLengthsGuess();

        Vector3 Shoulder1 = ShoulderLocation(0, 0.742f);
        //Shoulder1 = IndividualLegForwardKinematics(0,  0.742f, 0.779f, 0.587f) + controller.IMU.transform.position;

        //float Shoulder1DistanceToTarget = Mathf.Sqrt(Mathf.Pow(TargetFootTCPs[0].x - Shoulder1.x,2) + Mathf.Pow(TargetFootTCPs[0].y - Shoulder1.y,2) + Mathf.Pow(TargetFootTCPs[0].z - Shoulder1.z,2));

        //UnitVectorDemo = new Vector3((TargetFootTCPs[0].x - Shoulder1.x)/Shoulder1DistanceToTarget, (TargetFootTCPs[0].y - Shoulder1.y)/Shoulder1DistanceToTarget, (TargetFootTCPs[0].z - Shoulder1.z)/Shoulder1DistanceToTarget);

        Vector3 ReachabilityLine = ReachbilityRadius * Vector3.Normalize(Shoulder1);

        Debug.DrawLine(this.transform.position + Shoulder1, this.transform.position + Shoulder1 + ReachabilityLine);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 Leg1ForwardKinVec = IndividualLegForwardKinematics(0,  0.742f, 0.779f, 0.587f);
        Vector3 Leg2ForwardKinVec = IndividualLegForwardKinematics(1,  0.742f, 0.779f, 0.587f);
        Vector3 Leg3ForwardKinVec = IndividualLegForwardKinematics(2,  0.742f, 0.779f, 0.587f);
        Vector3 Leg4ForwardKinVec = IndividualLegForwardKinematics(3,  0.742f, 0.779f, 0.587f);
        Vector3 Leg5ForwardKinVec = IndividualLegForwardKinematics(4,  0.742f, 0.779f, 0.587f);
        Vector3 Leg6ForwardKinVec = IndividualLegForwardKinematics(5,  0.742f, 0.779f, 0.587f);

        DebugForwardKinematics = new Vector3[6] {Leg1ForwardKinVec, Leg2ForwardKinVec, Leg3ForwardKinVec, Leg4ForwardKinVec, Leg5ForwardKinVec, Leg6ForwardKinVec};

        float[] Leg1InverseKin = IndividualLegInverseKinematics(0, controller.foots[0].transform.position, 0.742f, 0.779f, 0.587f);
        Vector3 Leg1InverseKinVec = new Vector3(Leg1InverseKin[0], Leg1InverseKin[1], Leg1InverseKin[2]);

        float[] Leg2InverseKin = IndividualLegInverseKinematics(1, controller.foots[1].transform.position, 0.742f, 0.779f, 0.587f);
        Vector3 Leg2InverseKinVec = new Vector3(Leg2InverseKin[0], Leg2InverseKin[1], Leg2InverseKin[2]);

        float[] Leg3InverseKin = IndividualLegInverseKinematics(2, controller.foots[2].transform.position, 0.742f, 0.779f, 0.587f);
        Vector3 Leg3InverseKinVec = new Vector3(Leg3InverseKin[0], Leg3InverseKin[1], Leg3InverseKin[2]);

        float[] Leg4InverseKin = IndividualLegInverseKinematics(3, controller.foots[3].transform.position, 0.742f, 0.779f, 0.587f);
        Vector3 Leg4InverseKinVec = new Vector3(Leg3InverseKin[0], Leg3InverseKin[1], Leg3InverseKin[2]);

        float[] Leg5InverseKin = IndividualLegInverseKinematics(4, controller.foots[4].transform.position, 0.742f, 0.779f, 0.587f);
        Vector3 Leg5InverseKinVec = new Vector3(Leg4InverseKin[0], Leg4InverseKin[1], Leg4InverseKin[2]);

        float[] Leg6InverseKin = IndividualLegInverseKinematics(5, controller.foots[5].transform.position, 0.742f, 0.779f, 0.587f);
        Vector3 Leg6InverseKinVec = new Vector3(Leg5InverseKin[0], Leg5InverseKin[1], Leg5InverseKin[2]);

        DebugInverseKinematics = new Vector3[6] {Leg1InverseKinVec, Leg2InverseKinVec, Leg3InverseKinVec, Leg4InverseKinVec, Leg5InverseKinVec, Leg6InverseKinVec};
    }

    private void OnDrawGizmos()
    {
        Vector3 GlobalStartPosition = controller.Torso.transform.position;

        Gizmos.color = Color.green;
        for(int i = 0; i < DebugForwardKinematics.Length; i++)
        {
            Gizmos.DrawSphere(DebugForwardKinematics[i] + GlobalStartPosition, 0.1f);
        }

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(TargetCoM, 0.1f);        
        for(int i = 0; i < TargetFootTCPs.Length; i++)
        {
            Gizmos.DrawSphere(TargetFootTCPs[i], 0.1f);
        }

        Vector3 Shoulder1 = DebugForwardKinematics[0];
        Gizmos.DrawLine(this.transform.position + Shoulder1, this.transform.position + Shoulder1 + UnitVectorDemo);
    }

    public Vector3[] DebugForwardKinematics;
    public Vector3[] DebugInverseKinematics;

    public float[] IndividualLegInverseKinematics(int LegNumber, Vector3 TCP, float L1, float L2, float L3)
    {
        Vector3 ShoulderLocation;

        Vector3 TorsoToLeg = new Vector3(0, 0, 0.742f);
        float Angle = 305 - 35 * (controller.LegNumbering[LegNumber] - 1);

        // Quaternion rotationTorso;
        //rotationTorso = Quaternion.Euler(IMU.CurrentOrientation.z, Angle + IMU.CurrentOrientation.y, 0);
        if(LegNumber > controller.UpperLegs.Length / 2 - 1)
        {
            Angle -= 75;
            //rotationTorso = Quaternion.Euler(-IMU.CurrentOrientation.z, Angle + IMU.CurrentOrientation.y, 0);
        }

        Matrix4x4 mB1 = Matrix4x4.Rotate(Quaternion.Euler(controller.IMU.CurrentOrientation.x, controller.IMU.CurrentOrientation.y, controller.IMU.CurrentOrientation.z));

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
        if(controller.LegNumbering[LegNumber] == 1)
        {
            offset = 55;
        }
        if(controller.LegNumbering[LegNumber] == 2)
        {
            offset = 90;
        }
        if(controller.LegNumbering[LegNumber] == 3)
        {
            offset = -55;
        }
        if(controller.LegNumbering[LegNumber] == 4)
        {
            offset = 55;
        }
        if(controller.LegNumbering[LegNumber] == 5)
        {
            offset = -90;
        }
        if(controller.LegNumbering[LegNumber] == 6)
        {
            offset = -55;
        }

        return new float[3] {-(theta1 + offset), theta2 + 20, theta3 - 45};
    }


    public float DebugL1Guess = 10;
    public float DebugL2Guess = 20;
    public float DebugL3Guess = 30;

    public float DebugError = 0;

    private bool FirstCalculation = true;
    private float PreviousCalculation1 = 10f;
    private float PreviousCalculation2 = 20f;
    private float PreviousCalculation3 = 30f;
    private float PreviousError = 100;
    public void LinkLengthsGuess()
    {
        float learningRate = 0.001f;
        float h = 0.00001f;

        float LegDistanceToGround = controller.FootDistanceToGround(0);
        // DebugDistancesToGround = LegDistanceToGround;
        Vector3 TCP = IndividualLegForwardKinematics(0, DebugL1Guess, DebugL2Guess, DebugL3Guess);
        float error = Mathf.Abs(TCP.y - LegDistanceToGround);
        Debug.Log("Error normal: " + error);
        //float error = LegDistanceToGround - TCP.y;

        DebugError = error;

        // Gradient Decent L1

        Vector3 TCPPlusHL1 = IndividualLegForwardKinematics(0, DebugL1Guess + h, DebugL2Guess, DebugL3Guess);
        float errorPlusHL1 = Mathf.Abs(TCPPlusHL1.y - LegDistanceToGround);
        Debug.Log("Error plus h: " + errorPlusHL1);

        // float errorPlusHL1 = LegDistanceToGround - TCPPlusHL1.y;
        float de_L1 = (errorPlusHL1 - error)/h;

        // Gradient Decent L2
        /*
        Vector3 TCPPlusHL2 = IndividualLegForwardKinematics(0, DebugL1Guess, DebugL2Guess + h, DebugL3Guess);
        float errorPlusHL2 = Mathf.Abs(TCPPlusHL2.y - LegDistanceToGround);
        // float errorPlusHL2 = LegDistanceToGround - TCPPlusHL2.y;
        float de_L2 = (errorPlusHL2 - error)/h;

        // Gradient Decent L3

        Vector3 TCPPlusHL3 = IndividualLegForwardKinematics(0, DebugL1Guess, DebugL2Guess, DebugL3Guess + h);
        float errorPlusHL3 = Mathf.Abs(TCPPlusHL3.y - LegDistanceToGround);
        //float errorPlusHL3 = LegDistanceToGround - TCPPlusHL3.y;
        float de_L3 = (errorPlusHL3 - error)/h;
        */
        // Update all parameters
        DebugL1Guess = DebugL1Guess - learningRate * de_L1;
        // DebugL2Guess = DebugL2Guess - learningRate * de_L2;
        // DebugL3Guess = DebugL3Guess - learningRate * de_L3;

        // DebugL1Guess = Mathf.Max(DebugL1Guess, 0);
        // DebugL2Guess = Mathf.Max(DebugL2Guess, 0);
        // DebugL3Guess = Mathf.Max(DebugL3Guess, 0);        

        Debug.Log(de_L1);
    }


    public Vector3 ShoulderLocation(int LegNumber, float L1)
    {
        if(LegNumber > 5)
        {
            Debug.Log("Trying to check a leg that is not possible for kinematics");
        }

        if(LegNumber > controller.UpperLegs.Length - 1)
        {
            return Vector3.zero;
        }

        Vector3 TorsoToLeg = new Vector3(0, 0, L1);

        float Angle = 305 - 35 * (controller.LegNumbering[LegNumber] - 1);

        if(LegNumber > controller.UpperLegs.Length / 2 - 1)
        {
            Angle -= 75;
            //rotationTorso = Quaternion.Euler(-IMU.CurrentOrientation.z, Angle + IMU.CurrentOrientation.y, 0);
        }

        Matrix4x4 mB1 = Matrix4x4.Rotate(Quaternion.Euler(controller.IMU.CurrentOrientation.x, controller.IMU.CurrentOrientation.y, controller.IMU.CurrentOrientation.z));

        Quaternion rotationTorso = Quaternion.Euler(0, Angle, 0);
        Matrix4x4 m1 = Matrix4x4.Rotate(rotationTorso);

        // Vector3 EstimatedPosition = m1.MultiplyPoint3x4(TorsoToLeg);
        Vector3 EstimatedPosition =  mB1.MultiplyPoint3x4(m1.MultiplyPoint3x4(TorsoToLeg));
        return EstimatedPosition;
    }

    public Vector3 IndividualLegForwardKinematics(int LegNumber, float L1, float L2, float L3)
    {
        if(LegNumber > 5)
        {
            Debug.Log("Trying to check a leg that is not possible for kinematics");
        }

        if(LegNumber > controller.UpperLegs.Length - 1)
        {
            return Vector3.zero;
        }

        Vector3 TorsoToLeg = new Vector3(0, 0, L1);
        Vector3 UpperLegToKnee = new Vector3(0, 0, L2);
        Vector3 KneeToFoot = new Vector3(0, 0, L3);

        float Angle = 305 - 35 * (controller.LegNumbering[LegNumber] - 1);

        // Quaternion rotationTorso;
        //rotationTorso = Quaternion.Euler(IMU.CurrentOrientation.z, Angle + IMU.CurrentOrientation.y, 0);
        if(LegNumber > controller.UpperLegs.Length / 2 - 1)
        {
            Angle -= 75;
            //rotationTorso = Quaternion.Euler(-IMU.CurrentOrientation.z, Angle + IMU.CurrentOrientation.y, 0);
        }
        //rotationTorso = Quaternion.Euler(IMU.CurrentOrientation.x, Angle + IMU.CurrentOrientation.y, IMU.CurrentOrientation.z);

        Matrix4x4 mB1 = Matrix4x4.Rotate(Quaternion.Euler(controller.IMU.CurrentOrientation.x, controller.IMU.CurrentOrientation.y, controller.IMU.CurrentOrientation.z));
        // Matrix4x4 mB2 = Matrix4x4.Rotate(Quaternion.Euler(IMU.CurrentOrientation.x, IMU.CurrentOrientation.y, IMU.CurrentOrientation.z));
        // Matrix4x4 mB3 = Matrix4x4.Rotate(Quaternion.Euler(IMU.CurrentOrientation.x, IMU.CurrentOrientation.y, IMU.CurrentOrientation.z));
        // Matrix4x4 mB4 = Matrix4x4.Rotate(Quaternion.Euler(IMU.CurrentOrientation.x, IMU.CurrentOrientation.y, IMU.CurrentOrientation.z));
        // Matrix4x4 mB5 = Matrix4x4.Rotate(Quaternion.Euler(IMU.CurrentOrientation.x, IMU.CurrentOrientation.y, IMU.CurrentOrientation.z));
        // Matrix4x4 mB6 = Matrix4x4.Rotate(Quaternion.Euler(IMU.CurrentOrientation.x, IMU.CurrentOrientation.y, IMU.CurrentOrientation.z));

        Quaternion rotationTorso = Quaternion.Euler(0, Angle, 0);
        Quaternion rotationUpperLeg = Quaternion.Euler(20 - controller.UpperLegs[LegNumber].jointPosition[0] * Mathf.Rad2Deg, Angle - controller.UpperLegs[LegNumber].jointPosition[1] * Mathf.Rad2Deg, 0);
        Quaternion rotationUnderLeg = Quaternion.Euler(20 - controller.UpperLegs[LegNumber].jointPosition[0] * Mathf.Rad2Deg + 45 + controller.UnderLegs[LegNumber].jointPosition[0] * Mathf.Rad2Deg, Angle - controller.UpperLegs[LegNumber].jointPosition[1] * Mathf.Rad2Deg, 0);

        Matrix4x4 m1 = Matrix4x4.Rotate(rotationTorso);
        Matrix4x4 m2 = Matrix4x4.Rotate(rotationUpperLeg);
        Matrix4x4 m3 = Matrix4x4.Rotate(rotationUnderLeg);

        // Vector3 EstimatedPosition = m1.MultiplyPoint3x4(TorsoToLeg);
        Vector3 EstimatedPosition =  mB1.MultiplyPoint3x4(m1.MultiplyPoint3x4(TorsoToLeg)) + mB1.MultiplyPoint3x4(m2.MultiplyPoint3x4(UpperLegToKnee)) + mB1.MultiplyPoint3x4(m3.MultiplyPoint3x4(KneeToFoot));

        return EstimatedPosition;
    }

}
