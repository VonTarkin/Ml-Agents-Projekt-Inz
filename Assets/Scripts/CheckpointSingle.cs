using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointSingle : MonoBehaviour
{

    private TrackCheckPoints trackCheckpoints;
    private MeshRenderer meshRenderer;

    public void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<AgentCheckPoint>(out AgentCheckPoint agentCheckPoint))
        {
            trackCheckpoints.CarThroughCheckPoint(this, other.gameObject.transform);
        }
    }
    public void SetTrackCheckpoints(TrackCheckPoints trackCheckPoints)
    {
        this.trackCheckpoints = trackCheckPoints;
    }

    public void Show()
    {
        meshRenderer.enabled = true;
    }

    public void Hide()
    {
        meshRenderer.enabled = false;
    }
}
