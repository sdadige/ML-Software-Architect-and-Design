using UnityEngine;
using System.Collections;

public class PongBallController : MonoBehaviour
{

    private Transform trBall;
    private Rigidbody rbBall;

    public GameObject paddle1;
    private Transform trPaddle1;


    public GameObject paddle2;
    private Transform trPaddle2;


    public float paddleOffSet = 0.5f;

    
    private int playerToFire = 1;


    private bool ballFired = false;
    private bool ballReset = false;
    public float maxBallPosition;
    private Vector3 velocityDirection = Vector3.zero;
    public float ballSpeed = 3;
    

   
    void Start()
    {
        
        trBall = this.transform;
        rbBall = this.GetComponent<Rigidbody>();

       
        trPaddle1 = paddle1.transform;
        trPaddle2 = paddle2.transform;

        ResetBall(playerToFire);
    }

    public void ResetBall(int player)
    {
       
        ballFired = false;

        
        playerToFire = player;

     
        rbBall.velocity = new Vector3(0, 0, 0).normalized;

        
        StartCoroutine(BallSet());
    }

  
    private IEnumerator BallSet()
    {
        yield return new WaitForSeconds(1.0f);
        ballReset = true;
    }

    
    private void FixedUpdate()
    {
       
        if (ballFired == false)
        {
           
            if (trBall.localPosition.x < 0)
            {
             
                trBall.localPosition = new Vector3(trPaddle1.localPosition.x + paddleOffSet, (trBall.localScale.y / 2), 
                    Mathf.Clamp(trPaddle1.localPosition.z, -maxBallPosition, maxBallPosition));

        
                paddle1.GetComponent<PongPaddleAgent>().AddReward(-0.01f);
            }
            else
            {
  
                trBall.localPosition = new Vector3(trPaddle2.localPosition.x - paddleOffSet, (trBall.localScale.y / 2),
                    Mathf.Clamp(trPaddle2.localPosition.z, -maxBallPosition, maxBallPosition));

            
                paddle2.GetComponent<PongPaddleAgent>().AddReward(-0.01f);
            }
        }
    }

   
    public void FireBall(int player)
    {
        if (ballFired == false && ballReset == true)
        {
           
            if (player == playerToFire)
            {
              
                if (trBall.localPosition.x < 0)
                {
                    velocityDirection = new Vector3(1, 0, 0).normalized;
                }
                else
                {
                    velocityDirection = new Vector3(-1, 0, 0).normalized;
                }

        
                rbBall.velocity = velocityDirection * ballSpeed;

         
                ballFired = true;
                ballReset = false;
            }
        }
    }

   
    void OnCollisionEnter(Collision col)
    {
 
        if (col.gameObject.name == "Paddle1")
        {
    
            float z = hitFactor(trBall.localPosition,
                col.transform.localPosition,
                col.collider.bounds.size.z);

            velocityDirection = new Vector3(1, 0, z).normalized;

            rbBall.velocity = velocityDirection * ballSpeed;
        }

 
        if (col.gameObject.name == "Paddle2")
        {
       
            float z = hitFactor(transform.localPosition,
                col.transform.localPosition,
                col.collider.bounds.size.z);

         
            velocityDirection = new Vector3(-1, 0, z).normalized;

            GetComponent<Rigidbody>().velocity = velocityDirection * ballSpeed;
        }


        if (col.gameObject.name == "TopBound" || col.gameObject.name == "BottomBound")
        {
     
            Vector3 direction = Vector3.Reflect(velocityDirection, col.contacts[0].normal);

            velocityDirection = new Vector3(velocityDirection.x, 0, direction.z).normalized;

            GetComponent<Rigidbody>().velocity = velocityDirection * ballSpeed;
        }
    }

 
    float hitFactor(Vector3 ballPos, Vector3 paddlePos, float paddleSize)
    {
      
        return Mathf.Clamp(((ballPos.z - paddlePos.z) / paddleSize), -1.0f, 1.0f);
    }

}
