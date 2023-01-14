using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(DecisionRequester))]


public class AgentCheckPoint : Agent
{
    [System.Serializable]
    public class RewardInfo
    {
        public float multForward = 0.005f;
        public float barrier = -0.01f;
        public float car = -0.005f;
        public float win = 5f;
        public float lose = -1;
        public float checkPointGood = 0.05f;
        public float checkPointBad = -0.5f;
        public float turnPunishment = -0.001f;
        public float backPunishment = -0.001f;
    }


    public float moveSpeed = 50f;
    public float turnSpeed = 4f;
    public RewardInfo rwd = new RewardInfo();
    public TrackCheckPoints trackCheckPoints;

    public int currentLap = 1;

    private Rigidbody rb;
    private Vector3 recallPosition;
    private Quaternion recallRotation;
    private Bounds bnd;
    private BoxCollider boxCollider;

    public override void Initialize()
    {
        boxCollider = this.GetComponent<BoxCollider>();
        rb = this.GetComponent<Rigidbody>();
        rb.drag = 1;
        rb.angularDrag = 5;
        rb.interpolation = RigidbodyInterpolation.Extrapolate;

        this.GetComponent<DecisionRequester>().DecisionPeriod = 1;

        bnd = this.transform.Find("Body").GetComponent<MeshRenderer>().bounds;

        recallPosition = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z);
        recallRotation = new Quaternion(this.transform.rotation.x, this.transform.rotation.y, this.transform.rotation.z, this.transform.rotation.w);
    }

    public override void OnEpisodeBegin()
    {
        if (trackCheckPoints.setUp)
        {
            trackCheckPoints.ResetCheckPointForCar(this.transform);
        }
        rb.velocity = Vector3.zero;
        this.transform.position = recallPosition;
        this.transform.rotation = recallRotation;
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        //space type: discrete
        //  branches size: 2 move, turn
        //      branch 0 size 3: fwd, none, back
        //      branch 1 size 3: left, none, right

        if (isWheelDown() == false)
        {
            return;
        }

        float mag = rb.velocity.sqrMagnitude;

        switch (actions.DiscreteActions.Array[0])
        {

            case 0:
                break;
            case 1:
                rb.AddRelativeForce(Vector3.back * moveSpeed * Time.deltaTime, ForceMode.VelocityChange);
                AddReward(rwd.backPunishment);
                break;
            case 2:
                rb.AddRelativeForce(Vector3.forward * moveSpeed * Time.deltaTime, ForceMode.VelocityChange);
                AddReward(mag * rwd.multForward);
                //AddReward(rwd.multForward);
                break;
        }
        switch (actions.DiscreteActions.Array[1])
        {
            case 0:
                break;
            case 1:
                this.rb.MoveRotation(Quaternion.Euler(new Vector3(0, -turnSpeed, 0)) * this.rb.rotation);
                rb.angularVelocity = Vector3.zero;
                AddReward(rwd.turnPunishment);
                break;
            case 2:
                this.rb.MoveRotation(Quaternion.Euler(new Vector3(0, turnSpeed, 0)) * this.rb.rotation);
                rb.angularVelocity = Vector3.zero;
                AddReward(rwd.turnPunishment);
                break;
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        actionsOut.DiscreteActions.Array[0] = 0;
        actionsOut.DiscreteActions.Array[1] = 0;

        float move = Input.GetAxis("Vertical");
        float turn = Input.GetAxis("Horizontal");

        if (move < 0)
        {
            actionsOut.DiscreteActions.Array[0] = 1;
        }
        else if (move > 0)
        {
            actionsOut.DiscreteActions.Array[0] = 2;
        }

        if (turn < 0)
        {
            actionsOut.DiscreteActions.Array[1] = 1;
        }
        else if (turn > 0)
        {
            actionsOut.DiscreteActions.Array[1] = 2;
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Barrier") == true)
        {
            AddReward(rwd.barrier);
        }
        else if (collision.gameObject.CompareTag("Car") == true)
        {
            AddReward(rwd.car);
        }
        else if (collision.gameObject.CompareTag("Win") == true)
        {
            AddReward(rwd.win);
        }
        else if (collision.gameObject.CompareTag("Lose") == true)
        {
            AddReward(rwd.lose);
        }
    }
    private void OnCollisionStay(Collision collision)
    {
         if (collision.gameObject.CompareTag("Barrier") == true)
          {
              AddReward(rwd.barrier);
          }
          else if (collision.gameObject.CompareTag("Car") == true)
          {
              AddReward(rwd.car);
          }
        
    }

    private bool isWheelDown()
    {
        //is car on the ground
        //return Physics.Raycast(this.transform.position, -this.transform.up, 1, LayerMask.GetMask("CarPerception"));
        return Physics.CheckCapsule(boxCollider.bounds.center, new Vector3(boxCollider.bounds.center.x, boxCollider.bounds.min.y - 0.1f, boxCollider.bounds.center.z), 0.18f);
    }
}