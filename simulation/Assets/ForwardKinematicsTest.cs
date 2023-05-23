using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForwardKinematicsTest : MonoBehaviour
{
    public LowLevelRobotController controller;
    public GenericController GenController;

    float samples;

    float counter = 0;
    private bool BeenTriggered = false;
    float Leg1Error = 0;
    float Leg2Error = 0;
    float Leg3Error = 0;
    float Leg4Error = 0;
    float Leg5Error = 0;
    float Leg6Error = 0;

    private void FixedUpdate()
    {
        GenController.StateBasedControl(10, 15, 1, 0);

        Leg1Error += Vector3.Distance(GenController.IndividualLegForwardKinematics(0, controller.Angles[0, 0], controller.Links[0, 0], controller.Links[0, 1], controller.Links[0, 2]) + this.transform.position, controller.foots[0].transform.position);
        Leg2Error += Vector3.Distance(GenController.IndividualLegForwardKinematics(1, controller.Angles[1, 0], controller.Links[1, 0], controller.Links[1, 1], controller.Links[1, 2]) + this.transform.position, controller.foots[1].transform.position);
        Leg3Error += Vector3.Distance(GenController.IndividualLegForwardKinematics(2, controller.Angles[2, 0], controller.Links[2, 0], controller.Links[2, 1], controller.Links[2, 2]) + this.transform.position, controller.foots[2].transform.position);
        Leg4Error += Vector3.Distance(GenController.IndividualLegForwardKinematics(3, controller.Angles[3, 0], controller.Links[3, 0], controller.Links[3, 1], controller.Links[3, 2]) + this.transform.position, controller.foots[3].transform.position);        
        Leg5Error += Vector3.Distance(GenController.IndividualLegForwardKinematics(4, controller.Angles[4, 0], controller.Links[4, 0], controller.Links[4, 1], controller.Links[4, 2]) + this.transform.position, controller.foots[4].transform.position);
        Leg6Error += Vector3.Distance(GenController.IndividualLegForwardKinematics(5, controller.Angles[5, 0], controller.Links[5, 0], controller.Links[5, 1], controller.Links[5, 2]) + this.transform.position, controller.foots[5].transform.position);

        samples += 1;

        if(counter > 60 && BeenTriggered == false)
        {
            BeenTriggered = true;

            float Leg1AverageError = Leg1Error / samples;
            float Leg2AverageError = Leg2Error / samples;
            float Leg3AverageError = Leg3Error / samples;
            float Leg4AverageError = Leg4Error / samples; 
            float Leg5AverageError = Leg5Error / samples;
            float Leg6AverageError = Leg6Error / samples;

            float AverageError = (Leg1AverageError + Leg2AverageError + Leg3AverageError + Leg4AverageError + Leg5AverageError + Leg6AverageError) / 6;
            Debug.Log("Hexapod - Average Error: " + AverageError);
        }
        counter += Time.fixedDeltaTime;
    }
}
