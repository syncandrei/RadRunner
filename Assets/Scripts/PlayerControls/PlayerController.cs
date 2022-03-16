using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float speed;                     //The current forward speed of the rad
    public static PlayerController Instance;

    [Header("Drive Settings")]
    public float driveForce = 17f;          //The force that the engine of board generates
    public float slowingVelFactor = .99f;   //The percentage of velocity the rad maintains when not thrusting (e.g., a value of .99 means the rad loses 1% velocity when not thrusting)
    public float brakingVelFactor = .95f;   //The percentage of velocty the rad maintains when braking
    public float angleOfRoll = 30f;         //The angle that the rad "banks" into a turn
    public bool realisticTorque = false;
    public float speedTorque = 0f;
    
    [Header("Hover Settings")]
    public float hoverHeight = 1.5f;        //The height the rad maintains when hovering
    public float maxGroundDist = 5f;        //The distance the rad can be above the ground before it is "falling"
    public float hoverForce = 300f;         //The force of the rad's hovering
    public LayerMask whatIsGround;          //A layer mask to determine what layer the ground is on
    public PIDController hoverPID;          //A PID controller to smooth the rad's hovering

    [Header("Physics Settings")]
    public Transform radBody;              //A reference to the rad's body, this is for cosmetics
    public float terminalVelocity = 100f;   //The max speed the rad can go
    public float hoverGravity = 20f;        //The gravity applied to the rad while it is on the ground
    public float fallGravity = 80f;         //The gravity applied to the rad while it is falling

    [Header("Particle System")]
    public GameObject emissionTail;         //Emission tail
    public GameObject AmpParticles;         //Particle Effect for amps
    public GameObject SpeedParticles;       //Particle Effect for Speed

    [Header("Audio")]
    public AudioSource tickSource;
    public AudioSource speedSoundSource;

    Rigidbody rigidBody;                    //A reference to the rad's rigidbody
    PlayerInput input;                      //A reference to the player's input					
    float drag;                             //The air resistance the rad recieves in the forward direction
    bool isOnGround;                        //A flag determining if the rad is currently on the ground
    float initialDriveForce;
    float rotationTorque;
    float speedBoost;
    float speedBoostDuration;
    

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        //Get references to the Rigidbody and PlayerInput components
        rigidBody = GetComponent<Rigidbody>();
        input = GetComponent<PlayerInput>();

        //Calculate the rad's drag value
        drag = driveForce / terminalVelocity;

        initialDriveForce = driveForce;

        speedBoost = PickUpManager.Instance.SpeedBoost();
        speedBoostDuration = PickUpManager.Instance.speedBoostDuration;

        tickSource = GetComponent<AudioSource>();
       

        //stop all coroutines
        StopAllCoroutines();
    }

    void FixedUpdate()
    {
        //Calculate the current speed by using the dot product. This tells us
        //how much of the rad's velocity is in the "forward" direction

        speed = Vector3.Dot(rigidBody.velocity, transform.forward);

        //Calculate the forces to be applied to the rad
        CalculatHover();
        CalculatePropulsion();
    }

    void CalculatHover()
    {
        //This variable will hold the "normal" of the ground. Think of it as a line
        //the points "up" from the surface of the ground
        Vector3 groundNormal;

        //Calculate a ray that points straight down from the rad
        Ray ray = new Ray(transform.position, -transform.up);

        //Declare a variable that will hold the result of a raycast
        RaycastHit hitInfo;

        //Determine if the rad is on the ground by Raycasting down and seeing if it hits 
        //any collider on the whatIsGround layer
        isOnGround = Physics.Raycast(ray, out hitInfo, maxGroundDist, whatIsGround);

        //If rad is on the ground
        if (isOnGround)
        {
            //...determine how high off the ground it is
            float height = hitInfo.distance;
            //...save the normal of the ground
            groundNormal = hitInfo.normal.normalized;
            //...use the PID controller to determine the amount of hover force needed
            float forcePercent = hoverPID.Seek(hoverHeight, height);

            //calulcate the total amount of hover force based on normal (or "up") of the ground
            Vector3 force = groundNormal * hoverForce * forcePercent;
            //calculate the force and direction of gravity to adhere the rad to the 
            //track (which is not always straight down in the world)
            Vector3 gravity = -groundNormal * hoverGravity * height;

            //...and finally apply the hover and gravity forces
            rigidBody.AddForce(force, ForceMode.Acceleration);
            rigidBody.AddForce(gravity, ForceMode.Acceleration);

            emissionTail.SetActive(true);
        }

        else
        {
            //...use Up to represent the "ground normal". This will cause our rad to
            //self-right itself in a case where it flips over
            groundNormal = Vector3.up;

            //Calculate and apply the stronger falling gravity straight down on the rad
            Vector3 gravity = -groundNormal * fallGravity;
            rigidBody.AddForce(gravity, ForceMode.Impulse); //<------------------We use impulse insted of acceleration
            emissionTail.SetActive(false);
        }

        //Calculate the amount of pitch and roll the rad needs to match its orientation
        //with that of the ground. This is done by creating a projection and then calculating
        //the rotation needed to face that projection
        Vector3 projection = Vector3.ProjectOnPlane(transform.forward, groundNormal);
        Quaternion rotation = Quaternion.LookRotation(projection, groundNormal);

        //Move the rad over time to match the desired rotation to match the ground. This is 
        //done smoothly (using Lerp) to make it feel more realistic
        rigidBody.MoveRotation(Quaternion.Lerp(rigidBody.rotation, rotation, Time.fixedDeltaTime * 10f));

        //Calculate the angle we want the rad's body to bank into a turn based on the current rudder.
        //It is worth noting that these next few steps are completetly optional and are cosmetic.
        //It just feels so darn cool
        float angle = angleOfRoll * -input.rudder;

        //Calculate the rotation needed for this new angle
        Quaternion bodyRotation = transform.rotation * Quaternion.Euler(0f, 0f, angle);
        //Finally, apply this angle to the rad's body
        radBody.rotation = Quaternion.Lerp(radBody.rotation, bodyRotation, Time.fixedDeltaTime * 10f);
    }

    void CalculatePropulsion()
    {
        //Calculate the yaw torque based on the rudder and current angular velocity
        //Test purpos
        if (!realisticTorque)
        {
            rotationTorque = input.rudder * Time.fixedDeltaTime * speedTorque;
        }
        else
        {
            rotationTorque = input.rudder - rigidBody.angularVelocity.y;
        }
        
        //Apply the torque to the rad's Y axis
        rigidBody.AddRelativeTorque(0f, rotationTorque, 0f, ForceMode.VelocityChange);

        //Calculate the current sideways speed by using the dot product. This tells us
        //how much of the rad's velocity is in the "right" or "left" direction
        float sidewaysSpeed = Vector3.Dot(rigidBody.velocity, transform.right);

        //Calculate the desired amount of friction to apply to the side of the rad board. This
        //is what keeps the rad from drifting into the walls during turns. If we want to add
        //drifting to the game, divide Time.fixedDeltaTime by some amount <--I made this because it will be cool stuff if we want 
        //to add curve tracks
        Vector3 sideFriction = -transform.right * (sidewaysSpeed / Time.fixedDeltaTime);

        //Finally, apply the sideways friction
        rigidBody.AddForce(sideFriction, ForceMode.Acceleration);

        //If not propelling the rad, slow the rads velocity
        //Player is not controlling the rad moving vertical but this will keep him at certain distance
        if (input.thruster <= 0f)
            rigidBody.velocity *= slowingVelFactor;

        //Braking or driving requires being on the ground, so if the rad
        //isn't on the ground, exit this method
        if (!isOnGround)
            return;

        //If the rad is braking, apply the braking velocty reduction
        if (input.isBraking)
            rigidBody.velocity *= brakingVelFactor;

        //Calculate and apply the amount of propulsion force by multiplying the drive force
        //by the amount of applied thruster and subtracting the drag amount

        //float propulsion = driveForce * input.thruster - drag * Mathf.Clamp(speed, 0f, terminalVelocity);
        //float propulsion = driveForce - drag * Mathf.Clamp(speed, 0f, terminalVelocity);

        rigidBody.AddForce(transform.TransformDirection(Vector3.forward) * driveForce, ForceMode.Acceleration);
    }

    void OnCollisionStay(Collision collision)
    {
        //If the rad has collided with an object on the Wall layer...
        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            //calculate how much upward impulse is generated and then push the rad board down by that amount 
            //to keep it stuck on the track (instead up popping up over the wall)
            //if we want to make it casual
            Vector3 upwardForceFromCollision = Vector3.Dot(collision.impulse, transform.up) * transform.up;
            rigidBody.AddForce(-upwardForceFromCollision, ForceMode.Impulse);
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("DeathWall"))
        {
            //this restarts the game for the moment
            //need to be implement a "Game Over" screen
            Debug.ClearDeveloperConsole();
            string sceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        }
    }

    public float GetSpeedPercentage()
    {
        //Returns the total percentage of speed the rad is traveling
        return rigidBody.velocity.magnitude / terminalVelocity;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "PickUpStatic")
        {
            other.gameObject.SetActive(false);
            GameManager.Instance.AddScore(PickUpManager.Instance.ForAmps);
            tickSource.Play();
            Instantiate(AmpParticles, other.transform.position, other.transform.rotation);
            Debug.Log("I got a score and impulse");
        }
        else if (other.gameObject.tag == "PickUpSpeedBoost")
        {
            other.gameObject.SetActive(false);
            GameManager.Instance.AddScore(PickUpManager.Instance.ForSpeedBooster);
            GameManager.Instance.speedBoostPS.Play();
            StartCoroutine(CountdownSpeedBoost());
            speedSoundSource.Play();
            Debug.Log("I got a speed boost");
        }
        else if (other.gameObject.tag == "PickUpJumpBoost")
        {
            //Not yet implemented
            other.gameObject.SetActive(false);
            rigidBody.AddForce(Vector3.up * speed);
            Debug.Log("I got a jump boost but I am not implemented");
        }
        else if (other.gameObject.tag == "SlowTimeObject")
        {
            other.gameObject.SetActive(false);
            GameManager.Instance.AddScore(PickUpManager.Instance.ForSlowTimeBooster);
            StartCoroutine(CountdownSlowTime(PickUpManager.Instance.SlowLerpTimeTo(), PickUpManager.Instance.TimeToTake()));
            Debug.Log("I got a slow time object");
        }

        else if (other.gameObject.tag == "EndGame")
        {
            other.gameObject.SetActive(false);
            StartCoroutine(EndGame(0f, 2f));
            EndGameUI.Instance.PlayerEndGame(GameManager.Instance.AwardPlayer());
            Debug.Log("I ended the game");
        }

        else if (other.gameObject.tag == "FailGame")
        {
            MenuManager.Instance.failCamera.SetActive(true);
            MenuManager.Instance.gameCamera.SetActive(false);
            MenuManager.Instance.failGameUI.SetActive(true);
            MenuManager.Instance.standardUI.SetActive(false);
            Time.timeScale = 0;
        }
    }
    IEnumerator CountdownSpeedBoost()
    {
        driveForce += speedBoost;
        yield return new WaitForSeconds(speedBoostDuration);
        driveForce = initialDriveForce;
        GameManager.Instance.speedBoostPS.Stop();
        //StopAllCoroutines();
    }

    IEnumerator CountdownSlowTime(float _lerpTimeTo, float _timeToTake)
    {
        //Time.timeScale = PickUpManager.Instance.SlowTimeObject();
        //yield return new WaitForSeconds(PickUpManager.Instance.slowTimeDuration);
        //Time.timeScale = 1;

        //Stupid code but I have to write fast because of the little time we have. Please do something with this!
        float endTime = Time.time + _timeToTake;
        float startTimeScale = Time.timeScale;
        float i = 0f;
        while (Time.time < endTime)
        {
            i += (1 / _timeToTake) * Time.fixedDeltaTime;
            Time.timeScale = Mathf.Lerp(startTimeScale, _lerpTimeTo, i);
            //print(Time.timeScale);
            yield return null;
        }
        Time.timeScale = _lerpTimeTo;

        yield return new WaitForSeconds(PickUpManager.Instance.slowTimeDuration);

        _lerpTimeTo = 1f;
        _timeToTake = PickUpManager.Instance.TimeToTake();
        endTime = Time.time + _timeToTake;
        startTimeScale = Time.timeScale;
        i = 0f;
        while (Time.time < endTime)
        {
            i += (1 / _timeToTake) * Time.fixedDeltaTime;
            Time.timeScale = Mathf.Lerp(startTimeScale, _lerpTimeTo, i);
            //print(Time.timeScale);
            yield return null;
        }
        Time.timeScale = _lerpTimeTo;
        Time.timeScale = 1;
    }

    IEnumerator EndGame(float _lerpTimeTo, float _timeToTake)
    {
        float endTime = Time.time + _timeToTake;
        float startTimeScale = Time.timeScale;
        float i = 0f;
        while (Time.time < endTime)
        {
            i += (1 / _timeToTake) * Time.fixedDeltaTime;
            Time.timeScale = Mathf.Lerp(startTimeScale, _lerpTimeTo, i);
            //print(Time.timeScale);
            yield return null;
        }
        Time.timeScale = _lerpTimeTo;
    }
}
