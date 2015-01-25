// *********************************************************************** Assembly :
// Assembly-CSharp Author : razial Created : 12-29-2014
// 
// Last Modified By : razial Last Modified On : 01-07-2015 ***********************************************************************
// <copyright file="Stressor.cs" company="INLUSIO">
//     Copyright (c) INLUSIO. All rights reserved. 
// </copyright>
// <summary>
// </summary>
// *********************************************************************** 
using System.Collections;
using UnityEngine;
using XInputDotNetPure;
using System;


/// <summary>
/// Class Stressor. 
/// </summary>
public class Stressor : MonoBehaviour
{
    /// <summary>
    /// The displaytext 
    /// </summary>
    private static GameObject displaytext;

    /// <summary>
    /// The random 
    /// </summary>
    private float random;

    /// <summary>
    /// The v 
    /// </summary>
    private Vector3 v, pos;

    /// <summary>
    /// The ray direction 
    /// </summary>
    private Vector3 rayDirection;

    /// <summary>
    /// The key pressed to early 
    /// </summary>
    private bool keyPressedToEarly = false;

    /// <summary>
    /// The rotation speed 
    /// </summary>
    private float rotationSpeed = 100f;

    /// <summary>
    /// The rotation speed easy 
    /// </summary>
    private float rotationSpeedEasy = 50f;

    /// <summary>
    /// The rotation speed hard 
    /// </summary>
    private float rotationSpeedHard = 500f;

    /// <summary>
    /// The transformation speed 
    /// </summary>
    private float transformationSpeed = 15f;

    /// <summary>
    /// The distance to goal 
    /// </summary>
    private float distanceToGoal = 10;

    /// <summary>
    /// The spawn distance 
    /// </summary>
    private float spawnDistance = 40f;

    /// <summary>
    /// The spawnheight 
    /// </summary>
    private float spawnheight = 20f;

    /// <summary>
    /// The cool down 
    /// </summary>
    private float coolDown = 1.5f;       // How long to hide

    /// <summary>
    /// The spawn time 
    /// </summary>
    private string SpawnTime;

    /// <summary>
    /// The start defeat time 
    /// </summary>
    private string StartDefeatTime;
    System.DateTime StartDefeatTimeee ;

    /// <summary>
    /// The defeated at time 
    /// </summary>
    private string DefeatedAtTime;

    /// <summary>
    /// The missed hard balls 
    /// </summary>
    private int missedHardBalls = 0;

    /// <summary>
    /// The missed easy balls 
    /// </summary>
    private int missedEasyBalls = 0;

    /// <summary>
    /// The catched hard balls 
    /// </summary>
    private int catchedHardBalls = 0;

    /// <summary>
    /// The catched easy balls 
    /// </summary>
    private int catchedEasyBalls = 0;

    /// <summary>
    /// The easy delay 
    /// </summary>
    public static float EasyDelay = 0.500f;

    /// <summary>
    /// The hard dealy 
    /// </summary>
    public static float HardDealy = 0.300f;

    //stuff for vibrating
    /// <summary>
    /// The player index set
    /// </summary>
    private bool playerIndexSet = false;

    /// <summary>
    /// The player index 
    /// </summary>
    private PlayerIndex playerIndex;

    /// <summary>
    /// The state 
    /// </summary>
    private GamePadState state;

    /// <summary>
    /// The previous state 
    /// </summary>
    private GamePadState prevState;

    /// <summary>
    /// The fake press 
    /// </summary>
    public bool FakePress = false; // this is needed for the debug player

    public static float DistanceToPlayerBeforeGone;

    /// <summary>
    /// The urand 
    /// </summary>
    private UnityRandom urand;

    /// <summary>
    /// The time till exp 
    /// </summary>
    private int timeTillExp = 1; // how long till explosion


    /// <summary>
    /// The move scale 
    /// </summary>
    public static float moveScale;

    /// <summary>
    /// The onset of defeat at time 
    /// </summary>
    private float onsetOfDefeatAtTime;

    /// <summary>
    /// The duration of response period 
    /// </summary>
    public  float durationOfResponsePeriod;

    /// <summary>
    /// The p controller 
    /// </summary>
    private GameObject pController;

    /// <summary>
    /// The camera transform 
    /// </summary>
    private Transform cameraTransform = null;

    /// <summary>
    /// The px controller 
    /// </summary>
    private GameObject pxController;

    /// <summary>
    /// The xcontroller 
    /// </summary>
    private OVRPlayerController xcontroller;

    /// <summary>
    /// The explosion time 
    /// </summary>
    private static string ExplosionTime;

    /// <summary>
    /// Enum yellowSphereStates 
    /// </summary>
    public enum yellowSphereStates
    {
        /// <summary>
        /// The hidden 
        /// </summary>
        hidden,

        /// <summary>
        /// The moving 
        /// </summary>
        moving,

        /// <summary>
        /// The defeatable 
        /// </summary>
        defeatable,

        /// <summary>
        /// The not defeated in time 
        /// </summary>
        notDefeatedInTime,

        /// <summary>
        /// The start 
        /// </summary>
        start,

        /// <summary>
        /// The end 
        /// </summary>
        end,

        /// <summary>
        /// The defeated in time 
        /// </summary>
        defeatedInTime,
    }

    /// <summary>
    /// The s 
    /// </summary>
    private static yellowSphereStates s;

    public System.DateTime ReactionTime ;



    /// <summary>
    /// The time of defeat 
    /// </summary>
    private  float TimeOfDefeat;



    public string StunStartTime;
    public string StunStopTime;
    public string OldSpeed;
    public string NewSpeed;

    /// <summary>
    /// Awakes this instance. 
    /// </summary>
    private void Awake ()
    {
        cameraTransform = GameObject.FindWithTag("OVRcam").transform;
        displaytext = GameObject.Find("Displaytext2");
    }

    /// <summary>
    /// Starts this instance. 
    /// </summary>
    private void Start ()
    {
        pxController = GameObject.Find("OVRPlayerController");
        xcontroller = pxController.GetComponent<OVRPlayerController>();
        xcontroller.GetMoveScaleMultiplier(ref moveScale);
        urand = new UnityRandom((int)System.DateTime.Now.Ticks);
        renderer.enabled = false;
        switchState(yellowSphereStates.start);
    }

    /// <summary>
    /// Updates this instance. 
    /// </summary>
    private void Update ()
    {
        if (ManagerScript.getState() == ManagerScript.states.walking)
        {
            switch (s)
            {
                case yellowSphereStates.moving:

                    move();
                    if (Input.GetKeyDown(KeyCode.G) || Input.GetButtonDown("360controllerButtonB"))
                    {
                        keyPressedToEarly = true;
                    }
                    break;

                case yellowSphereStates.defeatable:

                    move();
                    if (FakePress || (Input.GetKeyDown(KeyCode.G) || Input.GetButtonDown("360controllerButtonB")) && !keyPressedToEarly)
                    {
                        switchState(yellowSphereStates.defeatedInTime);
                        DistanceToPlayerBeforeGone = Vector3.Distance(cameraTransform.position, transform.position);

                    }
                    break;

                case yellowSphereStates.notDefeatedInTime:
                    if (FakePress || (Input.GetKeyDown(KeyCode.G) || Input.GetButtonDown("360controllerButtonB")) && !keyPressedToEarly)
                    {
                        ReactionTime = System.DateTime.Now;
                    }

                    move();
                    break;
            }
        } else
        {
            renderer.enabled = false;
            displaytext.GetComponent<TextMesh>().text = "";
            CancelInvoke("startExp");
        }
    }

    /// <summary>
    /// Stressors the defeatable. 
    /// </summary>
    private void StressorDefeatable ()
    {
        switchState(yellowSphereStates.defeatable);
        StartDefeatTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff");
        StartDefeatTimeee = System.DateTime.Now;
    }

    /// <summary>
    /// Nots the defeated in time. 
    /// </summary>
    private void NotDefeatedInTime ()
    {
        switchState(yellowSphereStates.notDefeatedInTime);

        DistanceToPlayerBeforeGone = Vector3.Distance(cameraTransform.position, transform.position);
    }

    /// <summary>
    /// Resets this instance. 
    /// </summary>
    private void reset ()
    {
        renderer.enabled = false;
        CancelInvoke("startExp");
        keyPressedToEarly = false;
        //Invoke this shit after the coolDown time, basicaly after the coolDown
        Invoke("StartMoving", coolDown);
    }

    /// <summary>
    /// Starts the moving. 
    /// </summary>
    private void StartMoving ()
    {
        switchState(yellowSphereStates.moving);
    }

    /// <summary>
    /// Ends the stressor. 
    /// </summary>
    public void EndStressor ()
    {
        switchState(yellowSphereStates.end);
    }

    /// <summary>
    /// Starts the stressor. 
    /// </summary>
    public void StartStressor ()
    {
        switchState(yellowSphereStates.start);
    }

    // this is the function that respawns the yellow sphere 
    /// <summary>
    /// Moves the and show. 
    /// </summary>
    private void MoveAndShow ()
    {
        //position yellow sphere
        random = urand.Range(-10, 10, UnityRandom.Normalization.STDNORMAL, 0.1f);
        rayDirection = cameraTransform.TransformDirection(Vector3.forward);
        pos.x = (cameraTransform.position.x + rayDirection.x * spawnDistance) + random;
        pos.z = (cameraTransform.position.z + rayDirection.z * spawnDistance) - random;
        pos.y = spawnheight;
        transform.position = pos;




        renderer.enabled = true;
        // recordData.recordDataSmallspread("S", ""); 
        Pause.ChangeNumberOfYellowSpaw();
        SpawnTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff");
    }

    // this jidders the onset between 0.8 and 2.5 seconds 
    /// <summary>
    /// Generates the time onset of defeat time. 
    /// </summary>
    private void GenerateTimeOnsetOfDefeatTime ()
    {
        onsetOfDefeatAtTime = urand.Range(8, 25, UnityRandom.Normalization.STDNORMAL, 1.0f);
        onsetOfDefeatAtTime = onsetOfDefeatAtTime / 10;
    }

    /// <summary>
    /// Generates the time window for responce. 
    /// </summary>
    private void GenerateTimeWindowForResponce ()
    {
        if (catchedEasyBalls > 10 && EasyDelay > 0.400f)
        {
            EasyDelay = EasyDelay - 0.030f;
            ResetBallsCounterForDynamicDifficulty();
            testofsql.CreateDynamicDifficultyEvent("EasyDelay reduced", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff"));
            if (EasyDelay < 0.400f)
            {

                EasyDelay = 0.400f;

            }

        }

        if (catchedHardBalls > 5 && HardDealy > 0.179f)
        {
            HardDealy = HardDealy - 0.030f;
            ResetBallsCounterForDynamicDifficulty();
            testofsql.CreateDynamicDifficultyEvent("EasyDelay increased", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff"));


        }

        if (missedEasyBalls > 5)
        {
            EasyDelay = EasyDelay + 0.030f;
            ResetBallsCounterForDynamicDifficulty();
            testofsql.CreateDynamicDifficultyEvent("EasyDelay increased", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff"));

        }

        if (missedHardBalls > 5)
        {
            HardDealy = HardDealy + 0.030f;
            ResetBallsCounterForDynamicDifficulty();
            testofsql.CreateDynamicDifficultyEvent("HardDelay increased", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff"));

        }


        if (ManagerScript.CondtionTypeVariableInContainer == "Easy" || ManagerScript.CondtionTypeVariableInContainer == "Hard-False")
        {
            durationOfResponsePeriod = EasyDelay + (UnityEngine.Random.Range(1f, 200)) / 1000;
            rotationSpeed = rotationSpeedEasy;
        } else if (ManagerScript.CondtionTypeVariableInContainer == "Hard" || ManagerScript.CondtionTypeVariableInContainer == "Easy-False")
        {
            durationOfResponsePeriod = HardDealy + (UnityEngine.Random.Range(1f, 100)) / 1000;
            rotationSpeed = rotationSpeedHard;
        }
    }

    /// <summary>
    /// Datas the saving after explosion. 
    /// </summary>
    private void DataSavingAfterExplosion ()
    {
        // recordData.recordDataSmallspread("M", ""); 
        ExplosionTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff");
       

        testofsql.CreateStressor("Missed", DistanceToPlayerBeforeGone.ToString(), SpawnTime, StartDefeatTime, durationOfResponsePeriod.ToString(), "", rotationSpeed.ToString(), keyPressedToEarly.ToString(), ManagerScript.CondtionTypeVariableInContainer, testofsql.CURRENT_TRIAL_ID.ToString(), ExplosionTime);
    }

    /// <summary>
    /// Datas the saving after defeate. 
    /// </summary>
    private void DataSavingAfterDefeate ()
    {
        // recordData.recordDataSmallspread("D", ""); 
        DefeatedAtTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff");


        testofsql.CreateStressor("Defeated", DistanceToPlayerBeforeGone.ToString(), SpawnTime, StartDefeatTime, durationOfResponsePeriod.ToString(), DefeatedAtTime, rotationSpeed.ToString(), keyPressedToEarly.ToString(), ManagerScript.CondtionTypeVariableInContainer, testofsql.CURRENT_TRIAL_ID.ToString(), "");
    }

    /// <summary>
    /// Starts the exp. 
    /// </summary>
    private void startExp ()
    {
        DataSavingAfterExplosion();
        StunStartTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff");
        StartCoroutine(stunForSeconds(1));
        ((Detonator)(GetComponent("Detonator"))).Explode();
        // ((Detonator)(GameObject.Find("WaypointBlue").GetComponent("Detonator"))).Explode();
        switchState(yellowSphereStates.hidden);
        StartCoroutine(vibrateController());

    }

    /// <summary>
    /// Vibrates the controller. 
    /// </summary>
    /// <returns> IEnumerator. </returns>
    private IEnumerator vibrateController ()
    {
        
        if (!playerIndexSet || !prevState.IsConnected)
        {
            for (int i = 0; i < 4; ++i)
            {
                PlayerIndex testPlayerIndex = (PlayerIndex)i;
                GamePadState testState = GamePad.GetState(testPlayerIndex);
                if (testState.IsConnected)
                {
                    //Debug.Log(string.Format("GamePad found {0}", testPlayerIndex));
                    playerIndex = testPlayerIndex;
                    playerIndexSet = true;
                }
            }
        }
        prevState = state;
        state = GamePad.GetState(playerIndex);

        // Set vibration according to triggers
        //GamePad.SetVibration (playerIndex, state.Triggers.Left, state.Triggers.Right);
        GamePad.SetVibration(0, 1.0f, 1.0f);
        yield return new WaitForSeconds(0.2f);
        GamePad.SetVibration(0, 0.0f, 0.0f);
        yield return new WaitForSeconds(0.01f);
        GamePad.SetVibration(0, 1.0f, 1.0f);
        yield return new WaitForSeconds(0.2f);
        GamePad.SetVibration(0, 0.0f, 0.0f);
        yield return new WaitForSeconds(0.01f);
        GamePad.SetVibration(0, 1.0f, 1.0f);
        yield return new WaitForSeconds(0.8f);
        GamePad.SetVibration(0, 0.0f, 0.0f);
    }

    /// <summary>
    /// Stuns for seconds. 
    /// </summary>
    /// <param name="sec"> The sec. </param>
    /// <returns> IEnumerator. </returns>
    private IEnumerator stunForSeconds (int sec)
    {
        xcontroller.SetMoveScaleMultiplier(0.0f);
        yield return new WaitForSeconds(sec);
        StunStopTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff");

        OldSpeed = moveScale.ToString();
        moveScale = moveScale * 0.6f;
        NewSpeed = moveScale.ToString();
        xcontroller.SetMoveScaleMultiplier(moveScale);
        float temp = 0.0f;
        xcontroller.GetMoveScaleMultiplier(ref temp);
        testofsql.SaveDynamicDifficultyEvent(StunStartTime, StunStopTime, OldSpeed, NewSpeed);
    }

    /// <summary>
    /// News the trial. 
    /// </summary>
    public void NewTrial ()
    {
        switchState(yellowSphereStates.start);
    }

    /// <summary>
    /// Moves this instance. 
    /// </summary>
    private void move ()
    {
        v = cameraTransform.position;
        rayDirection = cameraTransform.TransformDirection(Vector3.forward);
        v.x = v.x + rayDirection.x * distanceToGoal + Mathf.Sin(Time.time) * 2;
        v.z = v.z + rayDirection.z * distanceToGoal + Mathf.Sin(Time.time) * 2;
        v.y = 7 + Mathf.Sin(Time.time) * 2;
        transform.position = Vector3.MoveTowards(transform.position, v, (transformationSpeed * Time.deltaTime));
        transform.Rotate(Vector3.right * Time.deltaTime * rotationSpeed);
    }

    /// <summary>
    /// Switches the state. 
    /// </summary>
    /// <param name="newState"> The new state. </param>
    public void switchState (yellowSphereStates newState)
    {
        displaytext.GetComponent<TextMesh>().text = "";
        switch (newState)
        {
            case yellowSphereStates.defeatable:
                displaytext.GetComponent<TextMesh>().text = "SHOOT";
                s = yellowSphereStates.defeatable;
                // recordData.recordDataSmallspread("Onset", durationOfResponsePeriod.ToString()); 
                Invoke("NotDefeatedInTime", durationOfResponsePeriod);
                break;

            case yellowSphereStates.hidden:

                s = yellowSphereStates.hidden;
                CancelInvoke("NotDefeatedInTime"); // if
                GenerateTimeWindowForResponce(); // we randomize the ball parapeters lol
                reset();
                break;

            case yellowSphereStates.moving:

                // here we get a rondom value for the jidder of the onset 
                GenerateTimeOnsetOfDefeatTime();
                MoveAndShow();
                s = yellowSphereStates.moving;
                Invoke("StressorDefeatable", onsetOfDefeatAtTime); // after some time we can defeat the stressor

                break;

            case yellowSphereStates.notDefeatedInTime:
                Pause.ChangeNumberOfYellowMissed();
                Invoke("startExp", timeTillExp); // this activates the data saving
                s = yellowSphereStates.notDefeatedInTime;
                if (ManagerScript.CondtionTypeVariableInContainer == "Easy")
                {
                    missedEasyBalls++;
                } else if (ManagerScript.CondtionTypeVariableInContainer == "Hard")
                {
                    missedHardBalls++;
                }

                break;

            case yellowSphereStates.defeatedInTime:
                s = yellowSphereStates.defeatedInTime;

                if (ManagerScript.CondtionTypeVariableInContainer == "Easy")
                {
                    catchedEasyBalls++;
                } else if (ManagerScript.CondtionTypeVariableInContainer == "Hard")
                {
                    catchedHardBalls++;
                }

                Pause.ChangeNumberOfYellowDefeted();
                FakePress = false;

                DataSavingAfterDefeate();
                switchState(yellowSphereStates.hidden);
                break;

            case yellowSphereStates.start: // if the stressor should spawn, we set it to the start state
                s = yellowSphereStates.start;
                moveScale = 3.5f;
                xcontroller.SetMoveScaleMultiplier(moveScale);
                switchState(yellowSphereStates.hidden);
                break;

            case yellowSphereStates.end: // if we want the stressor to stop, we set it to the end state
                s = yellowSphereStates.end;
                CancelInvoke("startExp"); 
                CancelInvoke("StartMoving");
                CancelInvoke("NotDefeatedInTime");
                CancelInvoke("StressorDefeatable");
                displaytext.GetComponent<TextMesh>().text = "";
                renderer.enabled = false;
                break;
        }
    }

    /// <summary>
    /// Resets the balls counter for dynamic difficulty. 
    ///  this schould happen every time we switch the blocks 
    /// </summary>
    public void ResetBallsCounterForDynamicDifficulty ()
    {
        missedHardBalls = 0;
        missedEasyBalls = 0;
        catchedHardBalls = 0;
        catchedEasyBalls = 0;
    }

    /// <summary>
    /// Gets the speed move scale. 
    /// </summary>
    /// <returns> System.Single. </returns>
    public static float GetSpeedMoveScale ()
    {
        return moveScale;
    }

    /// <summary>
    /// Gets the state of the yellow. 
    /// </summary>
    /// <returns> yellowSphereStates. </returns>
    public yellowSphereStates GetYellowState ()
    {
        return s;
    }



    // this function is executed by the testofsql stuff, in case we need to revive the old stats
    // from the previos session of the sama player
    /// <summary>
    /// Sets the dinamic difficulty from last session. 
    /// </summary>
    /// <param name="EasyDifficultyLevel"> The easy difficulty level. </param>
    /// <param name="HardDifficultyLevel"> The hard difficulty level. </param>
    internal static void SetDinamicDifficultyFromLastSession (float EasyDifficultyLevel, float HardDifficultyLevel)
    {
        HardDealy = HardDifficultyLevel;
        EasyDelay = EasyDifficultyLevel;
    }


}