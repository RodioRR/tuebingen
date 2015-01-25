// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : razial
// Created          : 01-09-2015
//
// Last Modified By : razial
// Last Modified On : 01-09-2015
// ***********************************************************************
// <copyright file="ManagerScript.cs" company="INLUSIO">
//     Copyright (c) INLUSIO. All rights reserved.
// </copyright>
// <summary>
/*
 * This script manages and keeps global values and other scripts
 * relie on this for different variables and functions.
 * Also has a state machine defining the state of the trial
 */
// </summary>
// ***********************************************************************

using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Class ManagerScript.
/// </summary>
public class ManagerScript : MonoBehaviour
{
    /// <summary>
    /// The condtion type variable in container
    /// </summary>
    public static string CondtionTypeVariableInContainer;

    /// <summary>
    /// Multiplikator for player moving speed
    /// </summary>
    private static float moveScale;

    /// <summary>
    /// The instance
    /// </summary>
    public static ManagerScript Instance = null;

    //public static List<float> generatedAngles = new List<float> ();
    /// <summary>
    /// The generated angle
    /// </summary>
    public static float generatedAngle;

    /// <summary>
    /// The use trial for ingame statistics.
    /// </summary>
    public static bool useTrialForIngameStatistics = false;

    // states for state machine to describe in which experiment state we are 
    /// <summary>
    /// Enum states
    /// </summary>
    public enum states
    {
        /// <summary>
        /// The start screen 2D GUI the game begins with. Subject enters id and session number
        /// </summary>
        startScreen,

        /// <summary>
        /// walking part for each trial. walking to first and second waypoint
        /// </summary>
        walking,

        /// <summary>
        /// State after reaching last waypoint. subject has to point back to origin
        /// </summary>
        pointing,

        /// <summary>
        /// State between two blocks of trials
        /// </summary>
        blockover,

        /// <summary>
        /// Pause state whith pause 2D Gui displayed
        /// </summary>
        pause,

        /// <summary>
        /// State after all trials are finished
        /// </summary>
        end,

        /// <summary>
        /// state after stateScreen state. All trials and trialblocks are created. initialisation of game
        /// </summary>
        start,

        /// <summary>
        /// initialises a new trial 
        /// </summary>
        NewTrial
    }

    /// <summary>
    /// chiffre for identification, can be changed in start screen 
    /// </summary>
    public static string chiffre;

    /// <summary>
    /// Actual state for the main state machiene
    /// </summary>
    private static states state;
	
    /// <summary>
    /// session identifier, two different sessions for tow ddifferent days with distinct trial order
    /// </summary>
    public static int session = 1 ;

    /// <summary>
    /// List of the trials. are created in the start state and is used in new trial state to get a new trial
    /// </summary>
    public static List<trialContainer> trialList = new List<trialContainer>();
	
    /// <summary>
    /// true if trial is in process i.e. walking or pointint state
    /// </summary>
    public static bool trialInProcess = false;

    /// <summary>
    /// true if currently pointing
    /// </summary>
    public static bool pointTaskINprocess = false;

    /// <summary>
    /// The timeto pointing stage
    /// </summary>
    public static float timetoPointingStage = 0.0f;

    /// <summary>
    /// The pointing time
    /// </summary>
    public static float pointingTime = 0.0f;

    /// <summary>
    /// true if duplicate trials are in list
    /// </summary>
    private static bool duplicatePresent = true;

    /// <summary>
    /// number of aborted trials
    /// </summary>
    public static int abortedTrials = 0;

    /// <summary>
    /// The current orientation
    /// </summary>
    public static int CurrentOrientation; // 0 is for left , 1 is for right

    /// <summary>
    /// can be "success" or "abort". saved in statistics to indicate if trial is successful or not
    /// </summary>
    static string argument;

    /// <summary>
    /// Truei if a trial is missed
    /// </summary>
    public static bool  TrialMissed = false;

    /// <summary>
    /// The real trial number
    /// can repeat !!! (increeases with every succesfull trial ... )
    /// </summary>
    public static int realTrialNumber = 1;

    /// <summary>
    /// The numberof trials startet
    /// this increase with every start of a trial. so this number will represent the current database number of the trial
    /// </summary>
    public static int NumberofTrialsStartet = 0;// 

    /// <summary>
    /// used to debuG the SQLite Database
    /// </summary>
    public static int debugg;

    /// <summary>
    /// The start time pointing
    /// </summary>
    private static string StartTimePointing;

    /// <summary>
    /// The end time trial
    /// </summary>
    private static string EndTimeTrial;
	
    /// <summary>
    /// Starts this instance.
    /// </summary>
    private void Start ()
    {
        // first grab the controller and assign the moveScale
        GameObject pController = GameObject.Find("OVRPlayerController");
        OVRPlayerController controller = pController.GetComponent<OVRPlayerController>();
        controller.GetMoveScaleMultiplier(ref moveScale);
        //TODO why this here?
        controller.SetMoveScaleMultiplier(3.5f);
        
        // initialisation of the Data Base
        testofsql.SQLiteInit(); 

        // dissable the stressor in the beginning
        GameObject.Find("StressorYellow").GetComponent<Stressor>().EndStressor(); 

        // show start screen and wait for user input by going to startScreen state
        ManagerScript.switchState(states.startScreen);        
    }

    /// <summary>
    /// Updates this instance.
    /// </summary>
    private void Update ()
    {
        if (state == states.walking)
        {
            timetoPointingStage += Time.deltaTime * 1;
        }
        if (state == states.pointing)
        {
            pointingTime += Time.deltaTime * 1;
        }
    }

    /// <summary>
    /// Aborts the trial.
    /// </summary>
    public static void abortTrial ()
    {
        // Without stun and unstun, the aboutTrial was repeating itself in the case, the move button
        // was presssed. It is fixes like this
        Waypoint.numberOfSpheresReached = 0;

        stun();
        trialInProcess = false;
        Time.timeScale = 0;

        CameraFade.StartAlphaFade(Color.black, false, 2f, 0f); // why we need this again ?
        new WaitForSeconds(2);
        Time.timeScale = 1;
        TrialMissed = true;
        ManagerScript.switchState(states.NewTrial);
        if (useTrialForIngameStatistics)
        {
            abortedTrials++;
        }
    }


    /// <summary>
    /// Switches the state.
    /// </summary>
    /// <param name="newState">The new state.</param>
    public static void switchState (states newState)
    {
        Debug.Log(newState);

        switch (newState)
        {
        //show start screen
            case states.startScreen:
                Time.timeScale = 1;
                ManagerScript.state = states.startScreen;
                break;

        // init game and generat trailList
            case states.start:
                ManagerScript.state = states.start;

                // TODO why do we need it ?
                ManagerScript.trialInProcess = true;

                //generate the trialList
                generateTrials();


                if (debugg == 1)
                {
                    GameObject.Find("OVRPlayerController").GetComponent<OVRPlayerController>().HmdRotatesY = false;

                    GameObject.Find("OVRPlayerController").GetComponent<DebugPlayer>().enabled = true;

                    PointingScript.ChangeTheObjectForDebugger();
                } else
                {
                    GameObject.Find("OVRPlayerController").GetComponent<OVRPlayerController>().HmdRotatesY = true;
                    GameObject.Find("OVRPlayerController").GetComponent<DebugPlayer>().enabled = false;
                }


                // init sql databank
                // lets create the initial savings 
                testofsql.InitialSavingsToDB(chiffre, Stressor.EasyDelay.ToString(), Stressor.HardDealy.ToString(), session.ToString(), trialList);
                Debug.Log(trialList [realTrialNumber + 1].CondtionTypeVariableInContainer);
                Pause.PauseBetweenBlocks(trialList [realTrialNumber + 1].CondtionTypeVariableInContainer);
                switchState(states.pause);

                break;

        // switch to walking from start to waypoint to waypoint state
            case states.walking:

                //fades Screen
                ((OVRScreenFade)(GameObject.Find("LeftEyeAnchor").GetComponent("OVRScreenFade"))).fadeTime = 0.25f;
                ((OVRScreenFade)(GameObject.Find("RightEyeAnchor").GetComponent("OVRScreenFade"))).fadeTime = 0.25f;
                ((OVRScreenFade)(GameObject.Find("LeftEyeAnchor").GetComponent("OVRScreenFade"))).OnEnable();
                ((OVRScreenFade)(GameObject.Find("RightEyeAnchor").GetComponent("OVRScreenFade"))).OnEnable();

                //reset player position and rotation
                ResetPositionRorationPlayerWaypoint();

                //set timescale to normal timescale
                Time.timeScale = 1;

                ManagerScript.state = states.walking;

                // set waypoints for new trial
                ((Waypoint)(GameObject.Find("WaypointBlue").GetComponent("Waypoint"))).switchState(Waypoint.WayPointStates.NewTrial);

                //Activate or deactivate the Stressor according to the current CondtionTypeVariableInContainer
                if (ManagerScript.CondtionTypeVariableInContainer != "Explain"
                    && ManagerScript.CondtionTypeVariableInContainer != "Dummy"
                    && ManagerScript.CondtionTypeVariableInContainer != "Training"
                    && ManagerScript.CondtionTypeVariableInContainer != "PostBaseline" 
                    && ManagerScript.CondtionTypeVariableInContainer != "PreBaseline")
                {
                    GameObject.Find("StressorYellow").GetComponent<Stressor>().StartStressor();
                   
                } else
                {
                    GameObject.Find("StressorYellow").GetComponent<Stressor>().EndStressor();

                }

                //unstun the player. i.e. player can move 
                unStun();

                break;
            
        //switch to pause
            case states.pause:

                ManagerScript.state = states.pause;

                //TODO @petr rename your functions properly! 
                ((Waypoint)(GameObject.Find("WaypointBlue").GetComponent("Waypoint"))).STOPFUCKINGINVOKE();

                // save the start time of pause
                Pause.StartTimePaused = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff");

                Debug.Log("state pause manager script");
                if (debugg == 1)
                {
                    UnPause();
                }
                Time.timeScale = 0;

                break;

        // switch to pointing
            case states.pointing:
                // normal timescale
                Time.timeScale = 1;
                // stop stressors
                ((Stressor)(GameObject.Find("StressorYellow").GetComponent("Stressor"))).switchState(Stressor.yellowSphereStates.end);
                // notify pointing script which handles pointing of the swith to pointing state
                ((PointingScript)(GameObject.Find("OVRPlayerController").GetComponent("PointingScript"))).NewPointing();
                // player shall not move
                stun();
                ManagerScript.state = states.pointing;
                // get time
                StartTimePointing = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff");
                break;

        // start a new trial
            case states.NewTrial:         
                // stop stressors
                ((Stressor)(GameObject.Find("StressorYellow").GetComponent("Stressor"))).switchState(Stressor.yellowSphereStates.end);
                ManagerScript.state = states.NewTrial;
                //get time
                EndTimeTrial = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff");
                // increment number of trials
                NumberofTrialsStartet++;
                //check if trial was successful
                if (!TrialMissed)
                {
                    realTrialNumber++;
                    Debug.Log("We increase the realTrialNumber");
                    argument = "success";

                } else
                {
                    TrialMissed = false;
                    argument = "abort";

                }

                // if it was a real trial save statistics to sql database
                if (trialList [realTrialNumber - 1].CondtionTypeVariableInContainer != "BLOCKOVER")
                {
                    testofsql.UpdateTrial(argument, PointingScript.AbsoluteErrorAngle.ToString(), PointingScript.angleBetween.ToString(), ManagerScript.CurrentOrientation.ToString(), StartTimePointing, PointingScript.EndTimePoining, EndTimeTrial);
                }

                // reset timers
                timetoPointingStage = 0.0f;
                pointingTime = 0.0f;
            
                //TODO for what do we need fakebuton
                ((PointingScript)(GameObject.Find("OVRPlayerController").GetComponent("PointingScript"))).PointFakeButton = false;
                Debug.Log(trialList [realTrialNumber + 1].CondtionTypeVariableInContainer);

                if (trialList [realTrialNumber + 1].CondtionTypeVariableInContainer != "ENDTRIAL")
                {
                    CondtionTypeVariableInContainer = trialList [realTrialNumber].CondtionTypeVariableInContainer;
                }

                //start new trial
                trialInProcess = true;
                Time.timeScale = 0;
                //create databank entry for new trial
                testofsql.CreateTrial(System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff"), NumberofTrialsStartet.ToString(), realTrialNumber.ToString(), CondtionTypeVariableInContainer);

                // check if trial should be used for statisitcs. this is the case if it is no training trial
                if (trialList [realTrialNumber].isTraining)
                {
                    useTrialForIngameStatistics = false;
                } else
                {
                    useTrialForIngameStatistics = true;
                }

                // if the new trial is blockover switch state to blockover
                if (trialList [realTrialNumber].CondtionTypeVariableInContainer == "BLOCKOVER")
                {
                    switchState(states.blockover);
                    Pause.SaveValues();
                    // if new trial is endtrial swith state to endtrial
                } else if (trialList [realTrialNumber].CondtionTypeVariableInContainer == "ENDTRIAL")
                {
                    testofsql.UpdateSession(); // this session is over
                    Pause.SaveValues();
                    switchState(states.end);
                    // if new state is a real trial switch to walking state
                } else
                    switchState(states.walking);
                break;

        // switch to blockover state
            case states.blockover:
                // increase trial number since blockover is also trial in triallist
                realTrialNumber++;
                ManagerScript.state = states.blockover;

                // save dynamic difficulty propertoes to database
                testofsql.SetDynamicDifficulty(Stressor.EasyDelay.ToString(), Stressor.HardDealy.ToString());
                testofsql.SaveDynamicDifficultyEvent();
                testofsql.SaveDynamicDifficultyEvent2();
   
                // if the next trial is the end trial swith state to new state
                if (trialList.Count == (realTrialNumber + 1))
                {
                    switchState(states.NewTrial);
                    // there it will go to the end state
                } else
                {
                    // else show the pause menu from where a new trial is started afte unpause
                    Debug.Log(trialList.Count);
                    Debug.Log(realTrialNumber);
                    Pause.PauseBetweenBlocks(trialList [realTrialNumber + 1].CondtionTypeVariableInContainer);
                }
                // save statistics of last trial to database
                testofsql.UpdateTrial("success", PointingScript.AbsoluteErrorAngle.ToString(), PointingScript.angleBetween.ToString(), ManagerScript.CurrentOrientation.ToString(), StartTimePointing, PointingScript.EndTimePoining, EndTimeTrial);

                // reset dynamic difficulty
                ((Stressor)(GameObject.Find("StressorYellow").GetComponent("Stressor"))).ResetBallsCounterForDynamicDifficulty();

                if (debugg == 1)
                {
                    UnPause();
                }

                break;

        //switch to end state
            case states.end:
                ManagerScript.state = states.end;
                Pause.PauseBetweenBlocks("EXPOVER");
                Pause.displayText = "--Fine--\nAll trials completed.\n";
                Time.timeScale = 0;
                break;
        }
    }

    /// <summary>
    /// Resets the position and rotation of player and the waypoint.
    /// </summary>
    private static void ResetPositionRorationPlayerWaypoint ()
    {
        GameObject.Find("OVRPlayerController").transform.position = GameObject.Find("StartPoint").transform.position;
        GameObject.Find("OVRPlayerController").transform.rotation = GameObject.Find("StartPoint").transform.rotation;
        GameObject.FindWithTag("OVRcam").transform.rotation = GameObject.Find("StartPoint").transform.rotation;
        GameObject.Find("OVRPlayerController").transform.position = GameObject.Find("StartPoint").transform.position;
        GameObject.Find("WaypointBlue").transform.rotation = GameObject.Find("StartPoint").transform.rotation;
    }

    /// <summary>
    /// stuns the player. i.e. he can not move
    /// </summary>
    private static void stun ()
    {
        GameObject pController = GameObject.Find("OVRPlayerController");
        OVRPlayerController controller = pController.GetComponent<OVRPlayerController>();
        controller.SetMoveScaleMultiplier(0.0f);
    }

    /// <summary>
    /// Unstuns the player. i.e. he can move
    /// </summary>
    private static void unStun ()
    {
        GameObject pController = GameObject.Find("OVRPlayerController");
        OVRPlayerController controller = pController.GetComponent<OVRPlayerController>();
        controller.SetMoveScaleMultiplier(3.5f);
    }

    /// <summary>
    /// Gets the state.
    /// </summary>
    /// <returns>states.</returns>
    public static states getState ()
    {
        return state;
    }

    /// <summary>
    /// This method creates a list of the trials for the experiment depending on the session number
    /// TrialContainers are created containing a certain type of trial and are concatenated into the list "trialList".
    /// </summary>
    public static void generateTrials ()
    {
        // create trail container with a blockover trial which is inserted after a block of trials is over
        trialContainer blockTrial = new trialContainer("BLOCKOVER");
        // create trial container for end trial which is added at the end of the trial list and is the endstate of the experiment
        trialContainer endTrial = new trialContainer("ENDTRIAL");

        // for the first session there are 10 Explain trial, 40 PreBaseLine Trials, 5 Easy training trails, 5 hard training trails
        // 45 hard trials, 45 easy trials, and a shuffeled block of 2 easy, 34 easy and 9 easy false trails. then the same for hard and\
        // false-hard trials. at the end 40 post baseline trials are added
        if (session == 1)
        {
            trialList.Add(blockTrial);

            // 10 explaining trials
            for (int i=0; i < 10; i++)
            {
                trialContainer tempTrial = new trialContainer("Explain");
                tempTrial.isTraining = true;
                trialList.Add(tempTrial);
            }
            trialList.Add(blockTrial);

            //40 prebaseline trials
            for (int i=0; i < 40; i++)
            {
                trialContainer tempTrial = new trialContainer("PreBaseline");
                tempTrial.isTraining = false;
                trialList.Add(tempTrial);
            }
            trialList.Add(blockTrial);

            // 5 easy training trials
            for (int i=0; i < 5; i++)
            {
                trialContainer tempTrial = new trialContainer("Easy");
                tempTrial.isTraining = true;
                trialList.Add(tempTrial);
            }
            trialList.Add(blockTrial);

            // 5 hard training trials
            for (int i=0; i < 5; i++)
            { //20
                trialContainer tempTrial = new trialContainer("Hard");
                tempTrial.isTraining = true;
                trialList.Add(tempTrial);
            }
            trialList.Add(blockTrial);

            // 45 easy trials
            for (int i=0; i < 45; i++)
            {
                trialContainer tempTrial = new trialContainer("Easy");
                tempTrial.isTraining = false;
                trialList.Add(tempTrial);
            }
            trialList.Add(blockTrial);

            // 45 hard trials
            for (int i=0; i < 45; i++)
            { //20
                trialContainer tempTrial = new trialContainer("Hard");
                tempTrial.isTraining = false;
                trialList.Add(tempTrial);
            }
            trialList.Add(blockTrial);

            // first block of easy/easyfalse trials
            List<trialContainer> easyBlock1 = new List<trialContainer>();

            //TODO why do we have to and 34 easy trials and not 36 easy trials
            for (int i=0; i < 2; i++)
            {
                trialContainer tempTrial = new trialContainer("Easy");
                tempTrial.isTraining = false;
                trialList.Add(tempTrial);
            }

            for (int i=0; i < 34; i++)
            {
                trialContainer tempTrial = new trialContainer("Easy");
                tempTrial.isTraining = false;
                easyBlock1.Add(tempTrial);
            }

            for (int i=0; i < 9; i++)
            {
                trialContainer tempTrial = new trialContainer("Easy-False");
                tempTrial.isTraining = false;
                easyBlock1.Add(tempTrial);
            }

            // unefficient way to make shure that no false trail is followed by another false trail
            while (duplicatePresent)
            {
                easyBlock1.Shuffle();
                for (int i=0; i < easyBlock1.Count - 1; i++)
                {
                    if (easyBlock1 [i].CondtionTypeVariableInContainer == "Easy-False" && easyBlock1 [i + 1].CondtionTypeVariableInContainer == "Easy-False")
                    {
                        duplicatePresent = true;
                        break;
                    }
                    duplicatePresent = false;
                }
            }

            trialList.AddRange(easyBlock1);
            trialList.Add(blockTrial);
            duplicatePresent = true;


            // first block of hard/falsehard trials
            List<trialContainer> hardBlock1 = new List<trialContainer>();

            for (int i=0; i < 2; i++)
            { //20
                trialContainer tempTrial = new trialContainer("Hard");
                tempTrial.isTraining = false;
                trialList.Add(tempTrial);
            }


            for (int i=0; i < 34; i++)
            { //20
                trialContainer tempTrial = new trialContainer("Hard");
                tempTrial.isTraining = false;
                hardBlock1.Add(tempTrial);
            }

            for (int i=0; i < 9; i++)
            {
                trialContainer tempTrial = new trialContainer("Hard-False");
                tempTrial.isTraining = false;
                hardBlock1.Add(tempTrial);
            }

            // unefficient way to make shure that no false trail is followed by another false trail
            while (duplicatePresent)
            {
                hardBlock1.Shuffle();
                for (int i=0; i < easyBlock1.Count - 1; i++)
                {
                    if (hardBlock1 [i].CondtionTypeVariableInContainer == "Hard-False" && hardBlock1 [i + 1].CondtionTypeVariableInContainer == "Hard-False")
                    {
                        duplicatePresent = true;
                        break;
                    }
                    duplicatePresent = false;
                }
            }

            trialList.AddRange(hardBlock1);
            trialList.Add(blockTrial);
            duplicatePresent = true;

            for (int i=0; i < 40; i++)
            {
                trialContainer tempTrial = new trialContainer("PostBaseline");
                tempTrial.isTraining = false;
                trialList.Add(tempTrial);
            }
           
            trialList.Add(blockTrial);
            // add end trial
            trialList.Add(endTrial);
            //TODO why is this here two times
            trialList.Add(endTrial);

            // session 2 starts with 40 prebaseline trials followed by an easy/easyfalse block with 45 trials and an  equal hard/hardfalse block
            // followed by 40 postBaseline trials
        } else if (session == 2)
        {
            trialList.Add(blockTrial);

            // 40 prebaseline trials
            for (int i=0; i < 40; i++)
            {
                trialContainer tempTrial = new trialContainer("PreBaseline");
                tempTrial.isTraining = false;
                trialList.Add(tempTrial);
            }
            trialList.Add(blockTrial);

            // randomly generate order of hard/hardfalse and easy/easyfalse trails (very efficient and short, NOT)
            List<int> orderNumbers = new List<int> { 1, 2 };
            orderNumbers.Shuffle();
            switch (orderNumbers [1])
            {
                case 1:

                    List<trialContainer> easyBlock1 = new List<trialContainer>();

                    for (int i=0; i < 2; i++)
                    {
                        trialContainer tempTrial = new trialContainer("Easy");
                        tempTrial.isTraining = false;
                        trialList.Add(tempTrial);
                    }

                    for (int i=0; i < 34; i++)
                    {
                        trialContainer tempTrial = new trialContainer("Easy");
                        tempTrial.isTraining = false;
                        easyBlock1.Add(tempTrial);
                    }

                    for (int i=0; i < 9; i++)
                    {
                        trialContainer tempTrial = new trialContainer("Easy-False");
                        tempTrial.isTraining = false;
                        easyBlock1.Add(tempTrial);
                    }

                    while (duplicatePresent)
                    {
                        easyBlock1.Shuffle();
                        for (int i=0; i < easyBlock1.Count - 1; i++)
                        {
                            if (easyBlock1 [i].CondtionTypeVariableInContainer == "Easy-False" && easyBlock1 [i + 1].CondtionTypeVariableInContainer == "Easy-False")
                            {
                                duplicatePresent = true;
                                break;
                            }
                            duplicatePresent = false;
                        }
                    }

                    trialList.AddRange(easyBlock1);
                    trialList.Add(blockTrial);
                    duplicatePresent = true;

                    List<trialContainer> hardBlock1 = new List<trialContainer>();


                    for (int i=0; i < 2; i++)
                    {
                        trialContainer tempTrial = new trialContainer("Hard");
                        tempTrial.isTraining = false;
                        trialList.Add(tempTrial);
                    }

                    for (int i=0; i < 34; i++)
                    {
                        trialContainer tempTrial = new trialContainer("Hard");
                        tempTrial.isTraining = false;
                        hardBlock1.Add(tempTrial);
                    }

                    for (int i=0; i < 9; i++)
                    {
                        trialContainer tempTrial = new trialContainer("Hard-False");
                        tempTrial.isTraining = false;
                        hardBlock1.Add(tempTrial);
                    }

                    while (duplicatePresent)
                    {
                        hardBlock1.Shuffle();
                        for (int i=0; i < hardBlock1.Count - 1; i++)
                        {
                            if (hardBlock1 [i].CondtionTypeVariableInContainer == "Hard-False" && hardBlock1 [i + 1].CondtionTypeVariableInContainer == "Hard-False")
                            {
                                duplicatePresent = true;
                                break;
                            }
                            duplicatePresent = false;
                        }
                    }

                    trialList.AddRange(hardBlock1);
                    trialList.Add(blockTrial);
                    duplicatePresent = true;

                    List<trialContainer> easyBlock2 = new List<trialContainer>();

                   
                    for (int i=0; i < 2; i++)
                    {
                        trialContainer tempTrial = new trialContainer("Easy");
                        tempTrial.isTraining = false;
                        trialList.Add(tempTrial);
                    }

                    for (int i=0; i < 34; i++)
                    {
                        trialContainer tempTrial = new trialContainer("Easy");
                        tempTrial.isTraining = false;
                        easyBlock2.Add(tempTrial);
                    }

                    for (int i=0; i < 9; i++)
                    {
                        trialContainer tempTrial = new trialContainer("Easy-False");
                        tempTrial.isTraining = false;
                        easyBlock2.Add(tempTrial);
                    }

                    while (duplicatePresent)
                    {
                        easyBlock2.Shuffle();
                        for (int i=0; i < easyBlock2.Count - 1; i++)
                        {
                            if (easyBlock2 [i].CondtionTypeVariableInContainer == "Easy-False" && easyBlock2 [i + 1].CondtionTypeVariableInContainer == "Easy-False")
                            {
                                duplicatePresent = true;
                                break;
                            }
                            duplicatePresent = false;
                        }
                    }

                    trialList.AddRange(easyBlock2);
                    trialList.Add(blockTrial);
                    duplicatePresent = true;

                    List<trialContainer> hardBlock2 = new List<trialContainer>();

                    for (int i=0; i < 2; i++)
                    {
                        trialContainer tempTrial = new trialContainer("Hard");
                        tempTrial.isTraining = false;
                        trialList.Add(tempTrial);
                    }

                    for (int i=0; i < 34; i++)
                    {
                        trialContainer tempTrial = new trialContainer("Hard");
                        tempTrial.isTraining = false;
                        hardBlock2.Add(tempTrial);
                    }

                    for (int i=0; i < 9; i++)
                    {
                        trialContainer tempTrial = new trialContainer("Hard-False");
                        tempTrial.isTraining = false;
                        hardBlock2.Add(tempTrial);
                    }

                    while (duplicatePresent)
                    {
                        hardBlock2.Shuffle();
                        for (int i=0; i < hardBlock2.Count - 1; i++)
                        {
                            if (hardBlock2 [i].CondtionTypeVariableInContainer == "Hard-False" && hardBlock2 [i + 1].CondtionTypeVariableInContainer == "Hard-False")
                            {
                                duplicatePresent = true;
                                break;
                            }
                            duplicatePresent = false;
                        }
                    }

                    trialList.AddRange(hardBlock2);
                    trialList.Add(blockTrial);

                    duplicatePresent = true;
                    break;

                case 2:

                    List<trialContainer> hardBlock3 = new List<trialContainer>();

                    for (int i=0; i < 2; i++)
                    {
                        trialContainer tempTrial = new trialContainer("Hard");
                        tempTrial.isTraining = false;
                        trialList.Add(tempTrial);
                    }

                    for (int i=0; i < 34; i++)
                    {
                        trialContainer tempTrial = new trialContainer("Hard");
                        tempTrial.isTraining = false;
                        hardBlock3.Add(tempTrial);
                    }

                    for (int i=0; i < 9; i++)
                    {
                        trialContainer tempTrial = new trialContainer("Hard-False");
                        tempTrial.isTraining = false;
                        hardBlock3.Add(tempTrial);
                    }

                    while (duplicatePresent)
                    {
                        hardBlock3.Shuffle();
                        for (int i=0; i < hardBlock3.Count - 1; i++)
                        {
                            if (hardBlock3 [i].CondtionTypeVariableInContainer == "Hard-False" && hardBlock3 [i + 1].CondtionTypeVariableInContainer == "Hard-False")
                            {
                                duplicatePresent = true;
                                break;
                            }
                            duplicatePresent = false;
                        }
                    }

                    trialList.AddRange(hardBlock3);
                    trialList.Add(blockTrial);
                    duplicatePresent = true;

                    List<trialContainer> easyBlock3 = new List<trialContainer>();

                    for (int i=0; i < 2; i++)
                    {
                        trialContainer tempTrial = new trialContainer("Easy");
                        tempTrial.isTraining = false;
                        trialList.Add(tempTrial);
                    }

                    for (int i=0; i < 34; i++)
                    {
                        trialContainer tempTrial = new trialContainer("Easy");
                        tempTrial.isTraining = false;
                        easyBlock3.Add(tempTrial);
                    }

                    for (int i=0; i < 9; i++)
                    {
                        trialContainer tempTrial = new trialContainer("Easy-False");
                        tempTrial.isTraining = false;
                        easyBlock3.Add(tempTrial);
                    }

                    while (duplicatePresent)
                    {
                        easyBlock3.Shuffle();
                        for (int i=0; i < easyBlock3.Count - 1; i++)
                        {
                            if (easyBlock3 [i].CondtionTypeVariableInContainer == "Easy-False" && easyBlock3 [i + 1].CondtionTypeVariableInContainer == "Easy-False")
                            {
                                duplicatePresent = true;
                                break;
                            }
                            duplicatePresent = false;
                        }
                    }

                    trialList.AddRange(easyBlock3);
                    trialList.Add(blockTrial);
                    duplicatePresent = true;

                    List<trialContainer> hardBlock4 = new List<trialContainer>();

                    for (int i=0; i < 2; i++)
                    {
                        trialContainer tempTrial = new trialContainer("Hard");
                        tempTrial.isTraining = false;
                        trialList.Add(tempTrial);
                    }

                    for (int i=0; i < 34; i++)
                    {
                        trialContainer tempTrial = new trialContainer("Hard");
                        tempTrial.isTraining = false;
                        hardBlock4.Add(tempTrial);
                    }

                    for (int i=0; i < 9; i++)
                    {
                        trialContainer tempTrial = new trialContainer("Hard-False");
                        tempTrial.isTraining = false;
                        hardBlock4.Add(tempTrial);
                    }

                    while (duplicatePresent)
                    {
                        hardBlock4.Shuffle();
                        for (int i=0; i < hardBlock4.Count - 1; i++)
                        {
                            if (hardBlock4 [i].CondtionTypeVariableInContainer == "Hard-False" && hardBlock4 [i + 1].CondtionTypeVariableInContainer == "Hard-False")
                            {
                                duplicatePresent = true;
                                break;
                            }
                            duplicatePresent = false;
                        }
                    }

                    trialList.AddRange(hardBlock4);
                    trialList.Add(blockTrial);
                    duplicatePresent = true;

                    List<trialContainer> easyBlock4 = new List<trialContainer>();

                    for (int i=0; i < 2; i++)
                    {
                        trialContainer tempTrial = new trialContainer("Easy");
                        tempTrial.isTraining = false;
                        trialList.Add(tempTrial);
                    }

                    for (int i=0; i < 34; i++)
                    {
                        trialContainer tempTrial = new trialContainer("Easy");
                        tempTrial.isTraining = false;
                        easyBlock4.Add(tempTrial);
                    }

                    for (int i=0; i < 9; i++)
                    {
                        trialContainer tempTrial = new trialContainer("Easy-False");
                        tempTrial.isTraining = false;
                        easyBlock4.Add(tempTrial);
                    }

                    while (duplicatePresent)
                    {
                        easyBlock4.Shuffle();
                        for (int i=0; i < easyBlock4.Count - 1; i++)
                        {
                            if (easyBlock4 [i].CondtionTypeVariableInContainer == "Easy-False" && easyBlock4 [i + 1].CondtionTypeVariableInContainer == "Easy-False")
                            {
                                duplicatePresent = true;
                                break;
                            }
                            duplicatePresent = false;
                        }
                    }

                    trialList.AddRange(easyBlock4);
                    trialList.Add(blockTrial);
                    duplicatePresent = true;

                    break;
            }

            for (int i=0; i < 40; i++)
            {
                trialContainer tempTrial = new trialContainer("PostBaseline");
                tempTrial.isTraining = false;
                trialList.Add(tempTrial);
            }

            trialList.Add(blockTrial);

            trialList.Add(endTrial);
            trialList.Add(endTrial);

            // session for debuging with onlye few trials 
        } else if (session == 666)
        {

            trialList.Add(blockTrial);

            {
                trialContainer tempTrial = new trialContainer("Easy");
                trialList.Add(tempTrial);
            }

            trialList.Add(blockTrial);

            for (int i=0; i < 5; i++)
            { //20
                trialContainer tempTrial = new trialContainer("Hard");
                trialList.Add(tempTrial);
            }
                    
            trialList.Add(blockTrial);
            for (int i=0; i < 5; i++)
            {
                trialContainer tempTrial = new trialContainer("Easy-False");
                trialList.Add(tempTrial);
            }
            trialList.Add(blockTrial);
            for (int i=0; i < 5; i++)
            {
                trialContainer tempTrial = new trialContainer("Hard-False");
                trialList.Add(tempTrial);
            }
            
            trialList.Add(blockTrial);       

            for (int i=0; i < 5; i++)
            {
                trialContainer tempTrial = new trialContainer("PostBaseline");
                trialList.Add(tempTrial);
            }


            trialList.Add(blockTrial);

            trialList.Add(endTrial);
            trialList.Add(endTrial);

            // quit if no session number is entered
        } else
            Application.Quit();
    }


    /// <summary>
    /// Restores the trial list from previos session of subject.
    /// </summary>
    /// <param name="trialList2">Trial list2.</param>
    public static void RestoreTrialListFromPreviosSessionOfSubject (List<trialContainer> trialList2)
    {
        trialList.Clear();
        ManagerScript.trialList = trialList2;

    }


    //TODO for what is this needed
    /// <summary>
    /// Uns the pause.
    /// </summary>
    private static void UnPause ()
    {
        ((Pause)(GameObject.Find("CenterEyeAnchor").GetComponent("Pause"))).FakePauseButton = true;
    }
}