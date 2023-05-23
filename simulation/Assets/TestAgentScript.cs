using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class TestAgentScript : Agent
{
    public Transform Target;
    public float speed = 2;
    private Vector3 InitialPosition;

    private void Awake()
    {
        InitialPosition = this.transform.position;
    }

    public override void OnEpisodeBegin()
    {
        this.transform.position = InitialPosition;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(this.transform.position);
        sensor.AddObservation(Target.position);
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        this.transform.position += new Vector3(vectorAction[0], 0, vectorAction[1]) * speed;
    }

    public override void Heuristic(float[] actionsOut)
    {
        actionsOut[0] = Input.GetAxisRaw("Horizontal");
        actionsOut[1] = Input.GetAxisRaw("Vertical");
    }

    private void OnCollisionEnter(Collision other)
    {
        if(other.transform.name == "Goal")
        {
            AddReward(1);
            EndEpisode();
        }

        if(other.transform.name == "Wall")
        {
            AddReward(-1);
            EndEpisode();
        }
    }
}
