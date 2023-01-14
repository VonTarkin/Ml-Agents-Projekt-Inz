using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class MoveToGoalAgent : Agent {

    [SerializeField] private Transform targetTransform;
    float moveSpeed = 1f;

    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(Random.Range(-1f,6f), -2.5f, Random.Range(-3f, 2f));
        targetTransform.localPosition = new Vector3(Random.Range(0f, 5f), -2.5f, Random.Range(-2f, 1f));
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        AddReward(-0.01f);
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(targetTransform.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = 1f * Mathf.Clamp(actions.ContinuousActions[0], -1f, 1f);
        float moveZ = 1f * Mathf.Clamp(actions.ContinuousActions[1], -1f, 1f);

        transform.position += new Vector3(moveX, 0, moveZ) * Time.deltaTime * moveSpeed;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continousActions = actionsOut.ContinuousActions;
        continousActions[0] = Input.GetAxisRaw("Horizontal");
        continousActions[1] = Input.GetAxisRaw("Vertical");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Goal>(out Goal goal))
        {
            Debug.Log("win");
            SetReward(1f);
            EndEpisode();
        }  
        if (other.TryGetComponent<Wall>(out Wall wall))
        {
            AddReward(-1f);
            EndEpisode();
        }
    }
}

