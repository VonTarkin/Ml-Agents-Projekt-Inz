using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.MLAgents;
using UnityEngine;

public class TrackCheckPoints : MonoBehaviour
{
    public PauseMenu pauseMenu;

    public event EventHandler OnPlayerCorrectCheckpoint;
    public event EventHandler OnPlayerWrongCheckpoint;
    public bool setUp = false;
    public bool resetAfterLap = false;

    int lap = 1;
    public int maxLap = 2;

    public TMP_Text leaderboardText;

    [SerializeField] private List<Transform> carTransformList;

    private List<CheckpointSingle> checkpointSingleList;
    private List<int> nextCheckpointSingleIndexList;
    private List<CarPositionData> carPositionDataList = new List<CarPositionData>();

    void Awake()
    {
        Transform checkpointsTransform = transform.Find("CheckPoint");
        checkpointSingleList = new List<CheckpointSingle>();
        foreach (Transform checkpointSingleTransform in transform)
        {
            CheckpointSingle checkpointSingle = checkpointSingleTransform.GetComponent<CheckpointSingle>();
            checkpointSingle.SetTrackCheckpoints(this);
            checkpointSingleList.Add(checkpointSingle);
        }

        nextCheckpointSingleIndexList = new List<int>();
        foreach (Transform carTransform in carTransformList)
        {
            nextCheckpointSingleIndexList.Add(0);
            CarPositionData carPositionData = new CarPositionData(carTransform.gameObject.name, 0);
            carPositionDataList.Add(carPositionData);
        }

        leaderboardText.text = prepareLeaderboardText();
        setUp = true;
    }

    private string prepareLeaderboardText()
    {
        return "Lap: " + this.lap + "/" + this.maxLap + "\n" + carPositionListToString();
    }

    private string carPositionListToString()
    {
        string result = "";
        foreach(CarPositionData carPositionData in carPositionDataList)
        {
            result += carPositionData.name + ": " + carPositionData.checkPointIndex + "\n"; 
        }
        return result;
    }

    private void updateCarPositonList()
    {
        carPositionDataList.Clear();
        foreach(Transform carTransform in carTransformList)
        {
            int nextCheckpointSingleIndex = nextCheckpointSingleIndexList[carTransformList.IndexOf(carTransform)];
            CarPositionData carPositionData = new CarPositionData(carTransform.gameObject.name, nextCheckpointSingleIndex);
            carPositionDataList.Add(carPositionData);
        }

        carPositionDataList.Sort((p1, p2) =>
        {
            return p2.checkPointIndex.CompareTo(p1.checkPointIndex);
        });

        leaderboardText.text = prepareLeaderboardText();
    }

    public void ResetCheckPointForCar(Transform carTransform)
    {
        int index = carTransformList.IndexOf(carTransform);
        nextCheckpointSingleIndexList[index] = 0;
    }
    public void CarThroughCheckPoint(CheckpointSingle checkpointSingle, Transform carTransform)
    {
        int nextCheckpointSingleIndex = nextCheckpointSingleIndexList[carTransformList.IndexOf(carTransform)];
        if (checkpointSingleList.IndexOf(checkpointSingle) == nextCheckpointSingleIndex)
        {
            AgentCheckPoint agentCheckPoint = carTransform.GetComponent<AgentCheckPoint>();
            if (agentCheckPoint)
            {
                agentCheckPoint.AddReward(agentCheckPoint.rwd.checkPointGood);
            }
            if(nextCheckpointSingleIndex == checkpointSingleList.Count - 1)
            {
                if(agentCheckPoint.currentLap == maxLap)
                {
                    pauseMenu.GoToMainMenu();
                }

                agentCheckPoint.AddReward(agentCheckPoint.rwd.win);
                Debug.Log("Win");
                if (agentCheckPoint.currentLap == lap)
                {
                    lap += 1;
                }
                agentCheckPoint.currentLap += 1;

                if (resetAfterLap)
                {
                    agentCheckPoint.EndEpisode();
                }
            }
            // Correct checkpoint
            Debug.Log("Correct");
            CheckpointSingle correctCheckpointSingle = checkpointSingleList[nextCheckpointSingleIndex];
           // correctCheckpointSingle.Hide();

            nextCheckpointSingleIndexList[carTransformList.IndexOf(carTransform)]
                = (nextCheckpointSingleIndex + 1) % checkpointSingleList.Count;
            OnPlayerCorrectCheckpoint?.Invoke(this, EventArgs.Empty);
        }
        else
        {

            AgentCheckPoint agentCheckPoint = carTransform.GetComponent<AgentCheckPoint>();
            if (agentCheckPoint && nextCheckpointSingleIndex != 0)
            {
                agentCheckPoint.AddReward(agentCheckPoint.rwd.checkPointBad);
            }
            if (checkpointSingleList.IndexOf(checkpointSingle) == (checkpointSingleList.Count - 1))
            {
                //agentCheckPoint.EndEpisode();
            }
            // Wrong checkpoint
            Debug.Log("Wrong");
            OnPlayerWrongCheckpoint?.Invoke(this, EventArgs.Empty);

            CheckpointSingle correctCheckpointSingle = checkpointSingleList[nextCheckpointSingleIndex];
            //correctCheckpointSingle.Show();
        }

        updateCarPositonList();
    }
}
