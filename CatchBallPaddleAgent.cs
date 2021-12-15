using UnityEngine;
using MLAgents;

public class CatchBallPaddleAgent : Agent
{
   
    private Transform trPaddle;
    public float translateFactor = 0.1f;
    private float maxPaddlePosition;

  
    public GameObject Ball;
    private Transform trBall;
    private Rigidbody rbBall;
    public float ballSpeed = 1;
    private float maxBallPosition;


    public Vector3 normPosFactor = new Vector3(5.0f, 1.0f, 3.5f);
    public float normVelFactor = 1.0f;

    void Start()
    {
       
        trPaddle = this.transform;

       
        trBall = Ball.transform;
        rbBall = Ball.GetComponent<Rigidbody>();

        
        maxPaddlePosition = normPosFactor.x - (trPaddle.localScale.x / 2);

      
        maxBallPosition = normPosFactor.z - (trBall.localScale.z / 2) - .05f;
    }


   
    public override void CollectObservations()
    {
       
        AddVectorObs(trBall.localPosition.x / normPosFactor.x);
        AddVectorObs(trBall.localPosition.z / normPosFactor.z);
        AddVectorObs(rbBall.velocity.x / normVelFactor);
        AddVectorObs(rbBall.velocity.z / normVelFactor);

       
        AddVectorObs(trPaddle.localPosition.x / normPosFactor.x);
    }


   
    public override void AgentAction(float[] vectorAction, string textAction)
    {
       
        if (trBall.localPosition.z < trPaddle.localPosition.z)
        {
            Done();
            AddReward(-1.0f);
        }

        
        int action = Mathf.FloorToInt(vectorAction[0]);
        Vector3 dirToGo = Vector3.zero;
        switch (action)
        {
            case 0:

                break;
            case 1:
              
                dirToGo = Vector3.left;
                break;
            case 2:
                
                dirToGo = Vector3.right;
                break;
        }

       
        Vector3 newPosition = trPaddle.localPosition + (dirToGo * translateFactor);
        newPosition.x = Mathf.Clamp(newPosition.x, -maxPaddlePosition, maxPaddlePosition);
        trPaddle.localPosition = newPosition;
    }

    void OnCollisionEnter(Collision col)
    {

        if (col.gameObject.name == "Ball")
        {
            AddReward(1.0f);
            ResetBall();
        }
    }

    public override void AgentReset()
    {
  
        ResetBall();
    }

    void ResetBall()
    {

        trBall.localPosition = new Vector3(Random.Range(-maxPaddlePosition, maxPaddlePosition), (trBall.localScale.y / 2), maxBallPosition);
        this.rbBall.angularVelocity = Vector3.zero;
        this.rbBall.velocity = Vector3.zero;

        Vector3 forceSignal = new Vector3(0, 0, -ballSpeed);
        rbBall.AddForce(forceSignal);
    }

}
