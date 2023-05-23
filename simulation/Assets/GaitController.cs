using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GaitController : MonoBehaviour
{
    public LowLevelRobotController controller;
    public float theta1 = 0;
    public float theta2 = 0;
    public float theta3 = 0;

    public float L1 = 0.742f;
    public float L2 = 0.779f;
    public float L3 = 0.587f;

    private float ReachbilityRadius;
    private float ReachabilityMargin = 0.1f;

    private enum ControlLoop
    {
        thinking,
        Actuating,
    }
    private ControlLoop CurrentControlLoop = ControlLoop.thinking;

    private enum ASingleLegCycle
    {
        start,
        mid,
        end,
        finished
    }
    private ASingleLegCycle CurrentLegCycle = ASingleLegCycle.start;

    private float[] NextLeg1Configuration;
    private float[] NextLeg2Configuration;
    private float[] NextLeg3Configuration;
    private float[] NextLeg4Configuration;
    private float[] NextLeg5Configuration;
    private float[] NextLeg6Configuration;

    float counter = 0;
    private void FixedUpdate()
    {
        if(CurrentControlLoop == ControlLoop.thinking)
        {
            NextLeg1Configuration = controller.IndividualLegInverseKinematics(0, TargetTCPs[0]);
            NextLeg2Configuration = controller.IndividualLegInverseKinematics(1, TargetTCPs[1]);
            NextLeg3Configuration = controller.IndividualLegInverseKinematics(2, TargetTCPs[2]);
            NextLeg4Configuration = controller.IndividualLegInverseKinematics(3, TargetTCPs[3]);
            NextLeg5Configuration = controller.IndividualLegInverseKinematics(4, TargetTCPs[4]);
            NextLeg6Configuration = controller.IndividualLegInverseKinematics(5, TargetTCPs[5]);

            Debug.Log(NextLeg1Configuration[0] + ", " + NextLeg1Configuration[1] + ", " + NextLeg1Configuration[2]);
            Debug.Log(NextLeg3Configuration[0] + ", " + NextLeg3Configuration[1] + ", " + NextLeg3Configuration[2]);

            CurrentControlLoop = ControlLoop.Actuating;

            /*
            for(int i = 0; i < controller.UpperLegs.Length; i++)
            {
                if(Vector3.Distance(TargetTCPs[i], controller.IndividualLegForwardKinematics(i)) > (ReachbilityRadius - ReachabilityMargin))
                {
                    Debug.Log("Yes");
                    

                }
                else
                {
                    NextLeg1Configuration = controller.IndividualLegInverseKinematics(i, TargetTCPs[0]);
                    NextLeg2Configuration = controller.IndividualLegInverseKinematics(i, TargetTCPs[0]);
                    NextLeg3Configuration = controller.IndividualLegInverseKinematics(i, TargetTCPs[0]);
                    NextLeg4Configuration = controller.IndividualLegInverseKinematics(i, TargetTCPs[0]);
                    NextLeg5Configuration = controller.IndividualLegInverseKinematics(i, TargetTCPs[0]);
                    NextLeg6Configuration = controller.IndividualLegInverseKinematics(i, TargetTCPs[0]);



                    Debug.Log(NextLeg1Configuration[0] + ", " + NextLeg1Configuration[1] + ", " + NextLeg1Configuration[2]);

                }


            }
            */




        }

        if(CurrentControlLoop == ControlLoop.Actuating)
        {

            if(CurrentLegCycle == ASingleLegCycle.start)
            {
                // NextLeg1Configuration = controller.IndividualLegInverseKinematics(0, TargetTCPs[0]);
                // NextLeg3Configuration = controller.IndividualLegInverseKinematics(2, TargetTCPs[2]);
                // NextLeg5Configuration = controller.IndividualLegInverseKinematics(4, TargetTCPs[4]);

                NextLeg2Configuration = controller.IndividualLegInverseKinematics(1, TargetTCPs[1]);
                NextLeg4Configuration = controller.IndividualLegInverseKinematics(3, TargetTCPs[3]);
                NextLeg6Configuration = controller.IndividualLegInverseKinematics(5, TargetTCPs[5]);

                CurrentLegCycle = ASingleLegCycle.mid;
            }

            if(CurrentLegCycle == ASingleLegCycle.mid)
            {
                controller.SetLegConfiguration(1, 20, 0, -45);
                controller.SetLegConfiguration(3, 20, 0, -45);
                controller.SetLegConfiguration(5, 20, 0, -45);

                counter += Time.fixedDeltaTime;

                if(counter > 10)
                {
                    CurrentLegCycle = ASingleLegCycle.end;
                    counter = 0;
                }
            }

            if(CurrentLegCycle == ASingleLegCycle.end)
            {
                // controller.SetLegConfiguration(0, NextLeg1Configuration[1], NextLeg1Configuration[0], NextLeg1Configuration[2]);
                // controller.SetLegConfiguration(2, NextLeg3Configuration[1], NextLeg3Configuration[0], NextLeg3Configuration[2]);
                // controller.SetLegConfiguration(4, NextLeg5Configuration[1], NextLeg5Configuration[0], NextLeg5Configuration[2]);

                controller.SetLegConfiguration(1, NextLeg2Configuration[1], NextLeg2Configuration[0], NextLeg2Configuration[2]);
                controller.SetLegConfiguration(3, NextLeg4Configuration[1], NextLeg4Configuration[0], NextLeg4Configuration[2]);
                controller.SetLegConfiguration(5, NextLeg6Configuration[1], NextLeg6Configuration[0], NextLeg6Configuration[2]);

                DebugInverseKinematics = new Vector3[2] {new Vector3(NextLeg1Configuration[0],NextLeg1Configuration[1],NextLeg1Configuration[2]), new Vector3(NextLeg3Configuration[0],NextLeg3Configuration[1],NextLeg3Configuration[2])};

                counter += Time.fixedDeltaTime;

                if(counter > 10)
                {
                    CurrentLegCycle = ASingleLegCycle.finished;
                    counter = 0;
                }
            }

            if(CurrentLegCycle == ASingleLegCycle.finished)
            {
                return;
            }

            // NextLeg1Configuration = controller.IndividualLegInverseKinematics(0, TargetTCPs[0]);
            // NextLeg2Configuration = controller.IndividualLegInverseKinematics(1, TargetTCPs[1]);
            // NextLeg3Configuration = controller.IndividualLegInverseKinematics(2, TargetTCPs[2]);
            // NextLeg4Configuration = controller.IndividualLegInverseKinematics(3, TargetTCPs[3]);
            // NextLeg5Configuration = controller.IndividualLegInverseKinematics(4, TargetTCPs[4]);
            // NextLeg6Configuration = controller.IndividualLegInverseKinematics(5, TargetTCPs[5]);




            // controller.SetLegConfiguration(0, NextLeg1Configuration[1] - 20, -(NextLeg1Configuration[0] + 55), NextLeg1Configuration[2] - 45);
            // controller.SetLegConfiguration(1, NextLeg2Configuration[1] - 20, (NextLeg2Configuration[0] + 55), NextLeg2Configuration[2] - 45);
            // controller.SetLegConfiguration(2, NextLeg3Configuration[1] - 20, -(NextLeg3Configuration[0] + 55), NextLeg3Configuration[2] - 45);
        }


    }


    private Vector3 EndCoM;

    public Vector3[] TargetTCPs;
    private Vector3 Foot1TCP;
    private Vector3 Foot2TCP;
    private Vector3 Foot3TCP;
    private Vector3 Foot4TCP;
    private Vector3 Foot5TCP;
    private Vector3 Foot6TCP;
    private void Awake()
    {
        ReachbilityRadius = L2 + L3;

        EndCoM = this.transform.position + new Vector3(0, 0, 0.2f);

        /*
        Foot1TCP = controller.IndividualLegForwardKinematics(0) + EndCoM;
        Foot2TCP = controller.IndividualLegForwardKinematics(1) + EndCoM;
        Foot3TCP = controller.IndividualLegForwardKinematics(2) + EndCoM;
        Foot4TCP = controller.IndividualLegForwardKinematics(3) + EndCoM;        
        Foot5TCP = controller.IndividualLegForwardKinematics(4) + EndCoM;
        Foot6TCP = controller.IndividualLegForwardKinematics(5) + EndCoM;
        */

        TargetTCPs = new Vector3[6] {Foot1TCP, Foot2TCP, Foot3TCP, Foot4TCP, Foot5TCP, Foot6TCP};
    }

    public Vector3[] DebugInverseKinematics;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(EndCoM, 0.1f);

        Gizmos.color = Color.blue;
        for(int i = 0; i < TargetTCPs.Length; i++)
        {
            Gizmos.DrawSphere(TargetTCPs[i], 0.1f);
        }
    }




}
