/* 
Infinite Runner Ultimate Presented by Black Gear Studio ©
         Programmed by Subhojeet Pramanik

This script manages the Player Movements, sound, and Player Animation


*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(CapsuleCollider))]
[System.Serializable]
public class PlayerControls : MonoBehaviour
{  //Please refer to documentation for public variables
    [HideInInspector]
    public float speed = 10; //The Current Player Speed
    public Transform Target; //The rotation target
    public bool useTarget = true;//Set this to true always to avoid position problems.
    public bool EnableJump = true;
    public bool EnableSlide = true;
    public float RotSpeed = 10;
    public float TurnSpeed = 10;
    public float JumpSpeed = 10;
    public float JumpHeight = 2;
    public AudioClip JumpSound;
    public float SlideTime = 1f;
    public float SlideDivideFactor = 2.5f;
    public AudioClip SlideSound;
    public float ResetLocalPositionSpeed = 3f;
    public TrackTypeEnum TrackType;  //The Enum Variable of type of track.
    public float rightvalue; //The maximum movable distance to the right
    public float leftvalue;  //The maximum movable distance to the left
    public LayerMask GroundLayer; //The layer used by all the grounds
    public GameObject DeathParticle;
    public float minSwipeDistX;    //Touch swipe only
    public float minSwipeDistY;
    public bool StaticCamera;      //Does the camera move while jumping
    public GameObject CameraObject;
    public float CameraHeight;     //Height of camera is it is static
    public float StumbleDuration = 3f;
    public float ShakeDuration = 2f;
    public float ShakeMagnitude = 1f;
    public LayerMask EnemyAndWallLayer; //The layer used by all collidable enemies or obstacles.
    public float Deathdist = 0.5f;          //The distance below which to detect beig collided by an obstacle/enemy
    public bool DisableAnimations = false;
    public Animator animator;       //Assign the anamator of your character here
    public bool AutoTurn = false;    //Auto turn on turn triggers. Useful in powerups
    [HideInInspector]
    public GameObject Enemy;
    [HideInInspector]
    public Vector3 enemyPosition;
    [HideInInspector]
    public Vector3 enemybackPosition;      //The enemy position when its not visible
    private bool jump = false; //Whether the player is moving up
    private WorldManager worldmanager;
    [HideInInspector]
    public bool UseEnemy = false;
    public float BackUpOnRevivalDistance = 0.8f;
    private float CacheJumpSpeed;
    public bool CanJump = true;        //Can we jump now
    private Vector3 animatorLocalPos;
    private bool isRotating = false;    //whether Player is taking a turn
    [HideInInspector]
    public bool dead = false;
    [HideInInspector]
    public float LimitedSpeed;
    [HideInInspector]
    public bool isSpeedLimited = false;
    private bool isSliding = false;
    private Vector3 cameraLocalPosition;
    private float Slidetimer = 0f;
    private CapsuleCollider collider;
    private CapsuleCollider capsuleCollider;
    private Quaternion rotateQuaternion;
    public enum TrackTypeEnum
    {
        ThreeSlotTrack, FreeHorizontalMovement
    };

    private GameObject tempref; //The temporary refrence gameobject used to calculate the center of the Track

    private int CurrentSlot = 0;
    private Vector2 startPos;

    [HideInInspector]
    public bool canChangeSlot = true;


    private bool isStumbling = true; //Whether Player is stumbling/stripping now
    private float StumbleTimer = 0f;
    private bool isinit = false;
    public enum GameState
    {
        Stopped, Playing, Pause, Dead  //The four game states 
    };

    public GameState CurrentGameState; //The current game state. Setting this to PlayerControls.GameState.Playing makes the player start running.
    private bool canDetectSwipe = true;

    private bool ishaking = false;
    [HideInInspector]
    public List<SpeedandDistance> SpeedDist = new List<SpeedandDistance>();
    [HideInInspector]
    public float Distance = 0f;  //Not the actual Distance. Actual distance is managed by PlayerScore.cs
    private int CurrentCount = 0;
    [HideInInspector]
    public float TargetSpeed; //The next target speed with which player will transition to
    float HorizontalMoveValue;
    public bool FastRun = false;
    private float nextChangeDistance;
    float PartialDist;
    
    void init()
    {
        if(!DisableAnimations)
            animator.SetBool("GameStart", true);
    }
    void Awake()
    {

        isSpeedLimited = false;
        canChangeSlot = true;
        Distance = 0f;
        nextChangeDistance = SpeedDist[0].Distance; //Setting the first speed as target
        TargetSpeed = SpeedDist[0].Speed;
    }
    void Start()
    {
        worldmanager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<WorldManager>();
        cameraLocalPosition = CameraObject.transform.localPosition;
        //Target = worldmanager.StartTarget;
        Physics.gravity = new Vector3(0, -20, 0); //Configure gravity to bring realism. g=9.8unit/s^2 does not look good in infinite runner game
        ishaking = false;
        isinit = false;
        CurrentGameState = GameState.Stopped; //Start Game with stopped state
        CurrentSlot = 0; //Middle is the current horizontal position
        tempref = new GameObject();
        Instantiate(tempref, transform.position, transform.rotation);
        tempref.transform.parent = this.transform;
        if(!DisableAnimations)
            animatorLocalPos = animator.transform.localPosition;
        CacheJumpSpeed = JumpSpeed;
        capsuleCollider = GetComponent<CapsuleCollider>();

    }


    void Update()
    {

        if (CurrentGameState == GameState.Pause)
        { //What to do in Pause
            canDetectSwipe = false;
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f; //Time scale to normal
        }
        if (CurrentGameState == GameState.Playing)
        { //What to do in Playing state
            if (FastRun == false)
            { //Assigning the next speed based on distance
                Distance += speed * Time.deltaTime;
                if (Distance >= nextChangeDistance && nextChangeDistance != 0f)
                {
                    ++CurrentCount;
                    nextChangeDistance = SpeedDist[CurrentCount].Distance;
                    TargetSpeed = SpeedDist[CurrentCount].Speed;
                }
            }
            if (isSpeedLimited == false)
                speed = Mathf.Lerp(speed, TargetSpeed, 5 * Time.deltaTime);//Changing current speed smoothly to targetspeed
            else
                speed = Mathf.Lerp(speed, LimitedSpeed, 5 * Time.deltaTime);
            if (!isinit)
            {//Init scripts on Player Start
                init();
                isinit = true;
            }
            CheckDeath(); //Checking whether player is about to collide or is colliding with enemy or wall
            if (isStumbling == true)
            {  //Things to do in stumbling
                StumbleTimer += Time.deltaTime;
               
                if (UseEnemy == true)
                {
                    Enemy.transform.localPosition = Vector3.Slerp(Enemy.transform.localPosition, enemyPosition, 5f * Time.deltaTime);
                    Enemy.SetActive(true); //Bring the enemy forward
                    if (dead == false)
                    {
                        if (StumbleTimer > StumbleDuration - 1)
                            Enemy.transform.localPosition = Vector3.Slerp(Enemy.transform.localPosition, enemybackPosition, 5f * Time.deltaTime);
                    }

                }
                if (StumbleTimer > StumbleDuration)
                {
                    StumbleTimer = 0f;
                    isStumbling = false;
                }
            }
            else
            {
                if (UseEnemy == true)
                {
                    if (Enemy.activeInHierarchy == true)
                        Enemy.SetActive(false);
                }
            }


            RaycastHit hitg;
            if (StaticCamera == true && ishaking == false)
            {
                if ((Physics.Raycast(transform.position, Vector3.down, out hitg, 5f, GroundLayer)))
                {  //Assigning camera position if it is not static


                    CameraObject.transform.position = new Vector3(CameraObject.transform.position.x, hitg.point.y + CameraHeight, CameraObject.transform.position.z);
                    CameraObject.transform.localPosition = new Vector3(cameraLocalPosition.x, CameraObject.transform.localPosition.y, cameraLocalPosition.z);

                }
            }




            //Code for Jump Starts.
            if (EnableJump&&(Input.GetKeyDown(KeyCode.UpArrow) || AndroidControls() == 1) && CanJump == true && dead == false)
            {
                jump = true;
                CanJump = false;
                GetComponent<AudioSource>().PlayOneShot(JumpSound);

            }

            if (!DisableAnimations&&jump == true)
            { //The process of jumping
               
                    animator.SetBool("Jump", true);
                    animator.transform.localPosition = Vector3.Slerp(animator.transform.localPosition, animatorLocalPos, Time.deltaTime * ResetLocalPositionSpeed); //Resting position while jumping
                
            }
            else if (jump == false &&  !DisableAnimations&&animator.GetBool("Jump") == true)
            {
                RaycastHit hit;

                if (Physics.Raycast(transform.position, Vector3.down, out hit,100, GroundLayer))
                {
                    if (hit.distance < 1.2f)
                    {
                        CanJump = true;
                        if (!DisableAnimations)
                        {
                            animator.SetBool("Jump", false);

                            animator.gameObject.transform.rotation = Quaternion.Slerp(animator.gameObject.transform.rotation, transform.rotation, Time.deltaTime * 50);
                            animator.transform.localPosition = Vector3.Slerp(animator.transform.localPosition, animatorLocalPos, ResetLocalPositionSpeed * Time.deltaTime);
                        }
                        }
                }
            }else if(jump == false)
            {
                RaycastHit hit;

                if (Physics.Raycast(transform.position, Vector3.down, out hit, 100, GroundLayer))
                {
                    if (hit.distance < 1.2f)
                    {
                        CanJump = true;
                    }
                }
            }
            if ( !DisableAnimations&&animator.GetBool("Jump") == false)
            { //Reseting animator position after jump is complete
                
                    animator.transform.localPosition = Vector3.Slerp(animator.transform.localPosition, animatorLocalPos, ResetLocalPositionSpeed * Time.deltaTime);
                    
                }

            //Code for Jump Ends
            //Code for Slide Starts
            if (EnableSlide&&(Input.GetKeyDown(KeyCode.DownArrow) || AndroidControls() == 2) && dead == false && isSliding == false)
            {
                if(!DisableAnimations)
                    animator.SetBool("Slide", true);
                isSliding = true;
                float f = capsuleCollider.height;
                capsuleCollider.height /= SlideDivideFactor; //Reducing player height and the center accordingly
                capsuleCollider.center = new Vector3(0, -capsuleCollider.height / SlideDivideFactor, 0);
                GetComponent<AudioSource>().PlayOneShot(SlideSound);

            }
            if (isSliding == true)
            {
                Slidetimer += Time.deltaTime;


                if (Slidetimer >= SlideTime)
                {
                    isSliding = false;
                    if(!DisableAnimations)
                        animator.SetBool("Slide", false);
                    Slidetimer = 0f;
                    capsuleCollider.height *= SlideDivideFactor;
                    capsuleCollider.center = new Vector3(0, 0, 0);

                }
            }

            //Code for Slide Ends
            if (isRotating == true)
            {
                GetComponent<Rigidbody>().rotation = Quaternion.Slerp(GetComponent<Rigidbody>().rotation, rotateQuaternion, TurnSpeed * Time.deltaTime); //Roate the player if input detected on turns
                if (Quaternion.Angle(GetComponent<Rigidbody>().rotation, rotateQuaternion) < 1)
                    isRotating = false;
            }
            if (useTarget == true)
            {
                Target = worldmanager.CurrentTarget;

                if (Vector3.Angle(transform.forward, Target.forward) > 0.1)
                {

                    transform.forward = Vector3.Slerp(transform.forward, Target.forward, Time.deltaTime * RotSpeed); //Alinging player's forward with the forward of the current player face direction target
                }

            }


            SlotSystemMove();
            //AlignToCenter();
            Quaternion q = new Quaternion();
            q.eulerAngles = new Vector3(0, 0, 0);
            if (!DisableAnimations)
                animator.transform.localRotation = q;//Reseting animator local rotation
                                                 //Anthing to do after death
            if (dead == true)
            {
                isStumbling = true;

                Instantiate(DeathParticle, transform.position, transform.rotation);

            }
            canDetectSwipe = true;
        }
        else if (CurrentGameState == GameState.Dead && UseEnemy == true)
        {///What to do in dead state
            Enemy.SetActive(true);
            Enemy.transform.localPosition = Vector3.Slerp(Enemy.transform.localPosition, enemyPosition, 5f * Time.deltaTime);
        }

    }

    void SlotSystemMove()
    { //Slot movement are the movement of the Player in horizontal direction
      //All sideways movement calculations are here.
        float SideWayEnemyCheckDist = 1.5f; //Maximum Distance to check whether there's an enemy on our side
        if (dead == false && isRotating == false && canChangeSlot == true)
        {
            if (TrackType == TrackTypeEnum.ThreeSlotTrack)
            {
                if (AndroidControls(1) == 3 || Input.GetKeyDown(KeyCode.RightArrow))
                {
                    RaycastHit hit;
                    if (Physics.Raycast(transform.position, transform.right, out hit, SideWayEnemyCheckDist, EnemyAndWallLayer))
                    {
                        if (hit.distance < SideWayEnemyCheckDist) //If there is a wall on the side then stumble. If already stumbling then die
                        {
                            ishaking = true;
                            StartCoroutine("Shake");
                            ReduceSpeed();
                            if (isStumbling)
                            {
                                Die();
                            }
                            isStumbling = true;
                        }
                    }
                    else if (CurrentSlot == 0)
                    {
                        CurrentSlot = 1;

                    }
                    else if (CurrentSlot == -1)
                    {
                        CurrentSlot = 0;
                    }
                    else if (CurrentSlot == 1)
                    {//If there is a wall on the side then stumble. If already stumbling then die
                        ishaking = true;
                        StartCoroutine("Shake");
                        ReduceSpeed();
                        if (isStumbling)
                        {
                            Die();
                        }
                        isStumbling = true;
                    }
                }
                if (AndroidControls(1) == 4 || Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    RaycastHit hit;
                    if (Physics.Raycast(transform.position, -transform.right, out hit, SideWayEnemyCheckDist, EnemyAndWallLayer))
                    {
                        if (hit.distance < SideWayEnemyCheckDist)
                        {
                            ishaking = true;
                            StartCoroutine("Shake");
                            ReduceSpeed();
                            if (isStumbling)
                            {//If there is a wall on the side then stumble. If already stumbling then die
                                Die();
                            }
                            isStumbling = true;
                        }
                    }
                    else if (CurrentSlot == 0)
                    {
                        CurrentSlot = -1;
                    }
                    else if (CurrentSlot == 1)
                    {
                        CurrentSlot = 0;
                    }
                    else if (CurrentSlot == -1)
                    {//If there is a wall on the side then stumble. If already stumbling then die
                        ishaking = true;
                        StartCoroutine("Shake");
                        ReduceSpeed();
                        if (isStumbling)
                        {
                            Die();
                        }
                        isStumbling = true;
                    }
                }

            }
            else if (TrackType == TrackTypeEnum.FreeHorizontalMovement)
            {
                RaycastHit hit;
                bool change = true;
                if (Physics.Raycast(transform.position, transform.right * Mathf.Sign(Input.acceleration.x), out hit, SideWayEnemyCheckDist, EnemyAndWallLayer))
                {
                    if (hit.distance < SideWayEnemyCheckDist)
                    {
                        change = false;
                    }

                }//Change Sensitivity from here
                if (change == true)
                    PartialDist = (leftvalue * Input.acceleration.x * 2f);
                if (PartialDist > leftvalue)
                    PartialDist = leftvalue;
                if (PartialDist < -leftvalue)
                    PartialDist = -leftvalue;
                HorizontalMoveValue = PartialDist;
                if (AndroidControls(1) == 3 || Input.GetKeyDown(KeyCode.RightArrow))
                {//If there is a wall on the side then stumble. If already stumbling then die
                    HorizontalMoveValue = 2f;
                    ishaking = true;
                    StartCoroutine("Shake");
                    ReduceSpeed();
                    if (isStumbling)
                    {
                        Die();
                    }
                    isStumbling = true;
                }
                else if (AndroidControls(1) == 4 || Input.GetKeyDown(KeyCode.LeftArrow))
                {//If there is a wall on the side then stumble. If already stumbling then die
                    HorizontalMoveValue = -2f;
                    ishaking = true;
                    StartCoroutine("Shake");
                    ReduceSpeed();
                    if (isStumbling)
                    {
                        Die();
                    }
                    isStumbling = true;
                }
            }
        }
    }
    public void Respawn()  //Respawn Player at Respawn point
    {
        isSpeedLimited = false;
        canChangeSlot = true;
        Distance = 0f;
        nextChangeDistance = SpeedDist[0].Distance; //Setting the first speed as target
        TargetSpeed = SpeedDist[0].Speed;
        PlayerPoweUpsUGUI pu;
        transform.position -= BackUpOnRevivalDistance * transform.forward;
        CurrentSlot = 0;
        animator.SetBool("Dead", false);
        animator.SetBool("GameStart", true); //Play dead animation
        dead = false;
        CurrentGameState = GameState.Playing; //Change gamestate to dead
        GetComponent<Rigidbody>().isKinematic = false;
        if (GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerPoweUpsUGUI>())
        {
            pu = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerPoweUpsUGUI>();
            pu.ActivateState(PlayerPoweUpsUGUI.State.Invincible);
        }
    }
    void ReduceSpeed()
    { //Reduce player speed on stumble.
        if (CurrentCount == 0)
            Distance = 0f;
        else
        {
            --CurrentCount;
            if (CurrentCount >= 1)
            {
                Distance = SpeedDist[CurrentCount - 1].Distance;
                TargetSpeed = SpeedDist[CurrentCount].Speed;
                speed = SpeedDist[CurrentCount].Speed;
            }
            else
            {
                speed = SpeedDist[CurrentCount].Speed;
                TargetSpeed = SpeedDist[CurrentCount].Speed;
                Distance = 0f;
            }
        }
    }
    void AlignToCenter()
    {//Aligning to the perfect position. Here center IS NOT THE CENTER OF THE TRACK. its a refrence position to calculate the proper horizontal position 

        tempref.transform.position = Target.position;
        tempref.transform.localPosition = new Vector3(tempref.transform.localPosition.x, 0, 0);
        Vector3 value = tempref.transform.position;
        if (TrackType == TrackTypeEnum.ThreeSlotTrack)
        {
            //Calculating proper position based on slot
            if (CurrentSlot == 1)
            {
                value += transform.right * rightvalue;
            }
            else if (CurrentSlot == -1)
            {
                value -= transform.right * leftvalue;
            }
        }
        else if (TrackType == TrackTypeEnum.FreeHorizontalMovement)
        {
            value += transform.right * HorizontalMoveValue;


        }
        GetComponent<Rigidbody>().position = Vector3.Slerp(GetComponent<Rigidbody>().position, value, 10 * Time.deltaTime);

    }
    //You can change Player Turn Controls from Here
    public void Rotate(Vector3 pos, ref bool canrot, int rotType = 0, Transform target = null, Transform target1 = null)
    {
        if (dead == false && CurrentGameState == GameState.Playing)
        {
            if (rotType == 0)
            {//both side turn
                if ((Input.GetKeyDown(KeyCode.LeftArrow) || AndroidControls(1) == 4) || AutoTurn == true)
                {
                    if (target != null)
                    {
                        worldmanager.CurrentTarget = target;
                    }
                    transform.position = new Vector3(pos.x, transform.position.y, pos.z);
                    isRotating = true;
                    rotateQuaternion = GetComponent<Rigidbody>().rotation * Quaternion.Euler(0, -90, 0);
                    canrot = false;
                }
                else if ((Input.GetKeyDown(KeyCode.RightArrow) || AndroidControls(1) == 3) || AutoTurn == true)
                {
                    if (target != null)
                    {
                        worldmanager.GetComponent<WorldManager>().CurrentTarget = target1;
                    }
                    transform.position = new Vector3(pos.x, transform.position.y, pos.z);
                    isRotating = true;
                    rotateQuaternion = GetComponent<Rigidbody>().rotation * Quaternion.Euler(0, 90, 0);
                    canrot = false;
                }
            }
            if (rotType == 1)
            { //Right turn
                if ((Input.GetKeyDown(KeyCode.RightArrow) || AndroidControls(1) == 3) || AutoTurn == true)
                {
                    if (target != null)
                    {
                        worldmanager.GetComponent<WorldManager>().CurrentTarget = target;
                    }
                    transform.position = new Vector3(pos.x, transform.position.y, pos.z);
                    isRotating = true;
                    rotateQuaternion = GetComponent<Rigidbody>().rotation * Quaternion.Euler(0, 90, 0);
                    canrot = false;
                }
            }
            if (rotType == -1)
            { //Left Turn
                if ((Input.GetKeyDown(KeyCode.LeftArrow) || AndroidControls(1) == 4) || AutoTurn == true)
                {
                    if (target != null)
                    {
                        worldmanager.GetComponent<WorldManager>().CurrentTarget = target;
                    }
                    transform.position = new Vector3(pos.x, transform.position.y, pos.z);
                    isRotating = true;
                    rotateQuaternion = GetComponent<Rigidbody>().rotation * Quaternion.Euler(0, -90, 0);
                    canrot = false;
                }
            }
        }
    }
    void FixedUpdate()
    {
        if (CurrentGameState == GameState.Playing)
        {
            if (isRotating == false && dead == false)
            {

                GetComponent<Rigidbody>().MovePosition(GetComponent<Rigidbody>().position + transform.forward * speed * Time.deltaTime); //Moving the player forward 

            }
            AlignToCenter(); //Aligning to the perfect position. Here center IS NOT THE CENTER OF THE TRACK. its a refrence position to calculate the proper horizontal position 

            if (dead == false)
            {
                if (jump == true)
                {
                    Jump();
                    GetComponent<Rigidbody>().useGravity = false;//Turn off gravity while jumping
                }
                else
                {
                    GetComponent<Rigidbody>().useGravity = true; //Turn on gravity after jump
                }
            }
        }

    }
    void Jump()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, 100, GroundLayer))
        {
            if (hit.distance > JumpHeight)
            {//Jumping until we get to the maximum jump height

                jump = false;
                JumpSpeed = CacheJumpSpeed;

            }

            GetComponent<Rigidbody>().MovePosition(GetComponent<Rigidbody>().position + transform.up * JumpSpeed * Time.deltaTime);
        }
    }

    void CheckDeath()
    {

        RaycastHit hit;
        float factor = 1.2f; //Increase this value if you are getting collision misses
        Vector3 v = new Vector3(0, 0f, 0);
        if (dead == false)
        {
            if (Physics.Raycast(transform.localPosition + v - transform.forward * factor, transform.forward, out hit, 10f, EnemyAndWallLayer))
            {

                if (hit.distance < 1 * factor + Deathdist)
                {

                    Die();
                }
            }
        }

    }


    int AndroidControls(int type = 0)
    {  //The function that detects touch swipe and returns an integer 
       //#if UNITY_ANDROID || UNITY_IPHONE 
        if (canDetectSwipe == true)
        {
            if (Input.touchCount > 0)

            {

                Touch touch = Input.touches[0];



                switch (touch.phase)

                {

                    case TouchPhase.Began:

                        startPos = touch.position;

                        break;



                    case TouchPhase.Ended:

                        float swipeDistVertical = (new Vector3(0, touch.position.y, 0) - new Vector3(0, startPos.y, 0)).magnitude;
                        float swipeDistHorizontal = (new Vector3(touch.position.x, 0, 0) - new Vector3(startPos.x, 0, 0)).magnitude;

                        if (swipeDistVertical > minSwipeDistY && (swipeDistVertical > swipeDistHorizontal))

                        {
                            float swipeValue = Mathf.Sign(touch.position.y - startPos.y);

                            if (swipeValue > 0)//up swipe
                            {
                                return 1;
                            }
                            else if (swipeValue < 0)//down swipe
                            {
                                return 2;
                            }

                        }



                        if (swipeDistHorizontal > minSwipeDistX)

                        {

                            float swipeValue = Mathf.Sign(touch.position.x - startPos.x);

                            if (swipeValue > 0)//right swipe
                            {
                                return 3;
                            }
                            else if (swipeValue < 0)//left swipe
                            {
                                return 4;

                            }
                        }

                        break;

                }

            }
        }
        //#endif
        return 0;
    }

    public void Die()
    {//What to do on death
        ishaking = true;
        StartCoroutine("Shake"); //Shake the screen
        if (!DisableAnimations)
        {
            animator.SetBool("Dead", true); //Play dead animation
            animator.SetBool("GameStart", false);
        }
        dead = true;
        CurrentGameState = GameState.Dead; //Change gamestate to dead
        GetComponent<Rigidbody>().isKinematic = true;

    }


    IEnumerator Shake()
    { //Shake the camera
        ishaking = true;
        float elapsed = 0.0f;

        Vector3 originalCamPos = CameraObject.transform.localPosition;

        while (elapsed < ShakeDuration)
        {

            elapsed += Time.deltaTime;

            float percentComplete = elapsed / ShakeDuration;
            float damper = 1.0f - Mathf.Clamp(4.0f * percentComplete - 3.0f, 0.0f, 1.0f);

            // map value to [-1, 1]
            float x = Random.value * 2.0f - 1.0f;
            float y = Random.value * 2.0f - 1.0f;
            x *= ShakeMagnitude * damper;
            y *= ShakeMagnitude * damper;

            CameraObject.transform.localPosition = new Vector3(x + originalCamPos.x, y + originalCamPos.y, originalCamPos.z);

            yield return null;
        }
        ishaking = false;
        CameraObject.transform.localPosition = originalCamPos;
    }


}
[System.Serializable]
public class SpeedandDistance
{
    public float Distance;//Set Distance to zero if the Speed Continues till Infinity
    public float Speed;
}



