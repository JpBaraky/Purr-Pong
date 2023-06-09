using System.Collections;
using UnityEngine;

public class EnemyPaddleController: MonoBehaviour {

    public float speed; // how fast the enemy paddle moves
    public Transform ball; // reference to the ball object
    public bool gameStarted = false; // flag to track if the game has started
    public bool startBoost;
    public Rigidbody2D ballRigidBody;
    private GameController gameController;


    // parameters for the AI behavior
    public float minPaddleSpeed = 4.0f;
    public float maxPaddleSpeed = 4.0f;
    public float reactionTime = 0.5f;
    public float errorMargin = 1f;
    public AiDificults.AI aiDificults;

    // variables to track the paddle movement
    private Vector2 targetPosition;
    private float currentSpeed;
    private float lastBallPositionY;

    void Start() {
        currentSpeed = Random.Range(minPaddleSpeed,maxPaddleSpeed);
        gameController = FindObjectOfType(typeof(GameController)) as GameController;
        speed = AiDificults.GetSpeed(aiDificults);
        reactionTime = AiDificults.GetReactionTime(aiDificults);
        errorMargin = AiDificults.GetErrorMargin(aiDificults);
    }

    void FixedUpdate() {
    
        if(gameStarted) {
            if(!startBoost) {
                StartCoroutine(TempSpeed());
                startBoost= true;
            }
            // calculate the target position based on the ball's predicted position


            float predictedPositionY = PredictBallPosition();
            targetPosition = new Vector2(transform.position.x,predictedPositionY);



            // move the paddle towards the target position with smooth damp
            float smoothTime = reactionTime * 0.5f;



            float newPosY = Mathf.SmoothDamp(transform.position.y,targetPosition.y,ref currentSpeed,smoothTime,speed);

            if(!float.IsNaN(newPosY)) {
                transform.position = Vector2.Lerp(transform.position,new Vector2(transform.position.x,newPosY),0.7f);
            }



            //transform.position = new Vector2(transform.position.x,newPosY);






        }
    }



    private float PredictBallPosition() {
        // calculate the predicted position of the ball based on its current position and velocity
        float distanceToTarget = ball.position.x - transform.position.x;
        float ballVelocityY = (ball.position.y - lastBallPositionY) / Time.deltaTime;
        float timeToTarget = Mathf.Abs(distanceToTarget / ball.GetComponent<Rigidbody2D>().velocity.x);
        float predictedPositionY = ball.position.y + ballVelocityY * timeToTarget;

        // add some randomness and error margin to make the AI more human-like
        predictedPositionY += Random.Range(-errorMargin,errorMargin);
        predictedPositionY = Mathf.Clamp(predictedPositionY,-2.5f,2.5f);

        // update the last ball position for the next prediction
        lastBallPositionY = ball.position.y;

        return predictedPositionY;
    }
    private IEnumerator TempSpeed() {
        speed = speed * 2;

        yield return new WaitForSeconds(2);
        speed = speed / 2;
    }
}