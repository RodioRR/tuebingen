// *********************************************************************** Assembly :
// Assembly-CSharp Author : razial Created : 12-29-2014
// 
// Last Modified By : razial Last Modified On : 01-07-2015 ***********************************************************************
// <copyright file="Pause.cs" company="INLUSIO">
//     Copyright (c) INLUSIO. All rights reserved. 
// </copyright>
// <summary>
// </summary>
// *********************************************************************** 
using UnityEngine;

/// <summary>
/// Class Pause. 
/// </summary>
public class Pause : VRGUI
{
    /// <summary>
    /// The pausekey 
    /// </summary>
    private static KeyCode pausekey = KeyCode.P;

    /// <summary>
    /// The previous state 
    /// </summary>
    private static ManagerScript.states prevState;

    /// <summary>
    /// The number of yellow spaw 
    /// </summary>
    private static int NumberOfYellowSpaw = 0;

    /// <summary>
    /// The number of yellow defeted 
    /// </summary>
    private static int NumberOfYellowDefeted = 0;

    /// <summary>
    /// The number of yellow missed 
    /// </summary>
    private static int NumberOfYellowMissed = 0;


    /// <summary>
    /// The paused 
    /// </summary>
    private static bool paused = false;

    /// <summary>
    /// The display text 
    /// </summary>
    public static string displayText = "";

    /// <summary>
    /// The skin 
    /// </summary>
    public GUISkin skin;

    /// <summary>
    /// The fake pause button 
    /// </summary>
    public bool FakePauseButton = false;

    /// <summary>
    /// The file path2 
    /// </summary>
    public string filePath2;

    /// <summary>
    /// The end time paused 
    /// </summary>
    private  string EndTimePaused;

    /// <summary>
    /// The start time paused 
    /// </summary>
    public static  string StartTimePaused;

    /// <summary>
    /// Starts this instance. 
    /// </summary>
    private void Start ()
    {
        // dont show the pause menu at the beginnig of the game
        GUI.enabled = false;
    }

    /// <summary>
    /// Updates this instance. 
    /// </summary>
    private void Update ()
    {
        // evalute if pause key is pressed if yes switch to pause if the experiment was not paused before and to the previous state of
        // the main state machiene if the game was paused before.
        if (FakePauseButton || Input.GetKeyDown(pausekey) || Input.GetButtonDown("360controllerButtonStart"))
        {
            if (paused)
            {
                // set trial as missed in manager script. Paused trial could yield to different results thus they are not included into
                // analysis of the experiment
                ManagerScript.TrialMissed = true; 
                //therefore  a new trial is startet
                ManagerScript.switchState(ManagerScript.states.NewTrial);
                // save the timepoint when the pause is ended
                EndTimePaused = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff");
                //delete the state specific text displayed in pause menu
                displayText = "";
                paused = false;
                // if pause not at start of game insert pause parameters into sql database
                if (ManagerScript.NumberofTrialsStartet > 0)
                {
                    testofsql.CreatePause(StartTimePaused, EndTimePaused);
                }
            } else if (!paused && ManagerScript.getState() != ManagerScript.states.startScreen && ManagerScript.getState() != ManagerScript.states.pointing && ManagerScript.getState() != ManagerScript.states.end && ManagerScript.getState() != ManagerScript.states.start)
            {
                // switch to pause
                paused = true;
                ManagerScript.switchState(ManagerScript.states.pause);
            }
            //TODO why do we need this thing?
            FakePauseButton = false;
        }
    
    
        // block used only for debugging
        //TODO shall we delete this? it does nothing what is not done above
        if (paused && ManagerScript.debugg == 1)
        {
            ManagerScript.switchState(ManagerScript.states.NewTrial);
            EndTimePaused = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff");
            displayText = "";
            paused = false;
        }
    }

    /// <summary>
    /// Called when [vrgui]. 
    /// </summary>
    public override void OnVRGUI ()
    {
        GUI.skin = skin;

        // show pause 2d GUI if paused or experiment is over
        if (paused || ManagerScript.getState() == ManagerScript.states.end)
        {
            // draw 2D GUI
            GUI.enabled = true;
            GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
            GUILayout.BeginVertical("box");
            GUILayout.FlexibleSpace();

            // show pause and how to exit
            GUILayout.Label("<color=lime> PAUSE </color>");
            GUILayout.Label("Press Start to resume.\n");

            // show state specific pause text
            GUILayout.Label(displayText);

            // show statistics of the experiment
            GUILayout.Label(NumberOfYellowSpaw + " balls spawned.");
            GUILayout.Label("You defeated " + NumberOfYellowDefeted + " balls.");
            GUILayout.Label("You missed " + NumberOfYellowMissed + " balls.");
            GUILayout.Label("You did " + (ManagerScript.realTrialNumber - 1) + " trials.");
            GUILayout.Label("You failed " + ManagerScript.abortedTrials + " trials.");
            GUILayout.Label("Your avarage error angle is " + PointingScript.avarageError + " degree.");

            GUILayout.EndVertical();
            GUILayout.EndArea();
        } else
        {
            GUI.enabled = false;
        }
    }

    /// <summary>
    /// Increment number of spwaned stressors if the trial in which the stressor was spawned should be used for ingame statistics
    /// </summary>
    public static void ChangeNumberOfYellowSpaw ()
    {
        if (ManagerScript.useTrialForIngameStatistics)
        {
            NumberOfYellowSpaw++;
        }
    }

    /// <summary>
    /// Increment number of defeted stressors if the trial in which the stressor was spawned should be used for ingame statistics
    /// </summary>
    public static void ChangeNumberOfYellowDefeted ()
    {
        if (ManagerScript.useTrialForIngameStatistics)
        {
            NumberOfYellowDefeted++;
        }
    }

    /// <summary>
    /// Increment number of missed i.e. exploded stressors if the trial in which the stressor was spawned should be used for ingame statistics
    /// </summary>
    public static void ChangeNumberOfYellowMissed ()
    {
        if (ManagerScript.useTrialForIngameStatistics)
        {
            NumberOfYellowMissed++;
        }
    }

    /// <summary>
    /// Pauses the experiment between trial blocks and at the end of the experiment and sets an appropriate pause message
    /// </summary>
    /// <param name="NextBlockType"> Type of the next block. </param>
    public static void PauseBetweenBlocks (string NextBlockType)
    {
        paused = true;
        if (NextBlockType.Contains("Easy") || NextBlockType.Contains("Easy-False"))
        {
            displayText = "Block Complted.\nNext block of Trials is Easy.\n";
        }

        if (NextBlockType.Contains("Hard") || NextBlockType.Contains("Hard-False"))
        {
            displayText = "Block Complted.\nNext block of Trials is Hard.\n";
        }

        if (NextBlockType.Contains("Explain"))
        {
            displayText = "Next block of Trials is Explain.\n";
        }

        if (NextBlockType.Contains("Training"))
        {
            displayText = "Block Completed. Next block of Trials is Training.\n";
        }
        if (NextBlockType.Contains("ENDTRIAL"))
        {
            displayText = "Experiment is over, please take of the oculus rift and report to the experimenter.\n";
        }

        if (NextBlockType.Contains("EXPOVER"))
        {
            displayText = "Experiment is over, please take of the oculus rift and report to the experimenter.\n";
        }
    }

    /// <summary>
    /// Saves the experiment statistics to the sql database
    /// </summary>
    public static void SaveValues ()
    {
        testofsql.SaveStatisicsToDataBase(NumberOfYellowSpaw.ToString(), NumberOfYellowDefeted.ToString(), NumberOfYellowMissed.ToString(), ManagerScript.abortedTrials.ToString(), PointingScript.avarageError.ToString());
    }
}