// *********************************************************************** Assembly :
// Assembly-CSharp Author : razial Created : 12-29-2014
// 
// Last Modified By : razial Last Modified On : 01-07-2015 ***********************************************************************
// <copyright file="PointingScript.cs" company="INLUSIO">
//     Copyright (c) INLUSIO. All rights reserved. 
// </copyright>
// <summary>
// </summary>
// *********************************************************************** 
using UnityEngine;

/// <summary>
/// Class PointingScript. 
/// </summary>
public class PointingScript : MonoBehaviour
{
    /// <summary>
    /// The time for pointing 
    /// </summary>
    private int timeForPointing = 8;

    /// <summary>
    /// The displaytext 
    /// </summary>
    private GameObject displaytext;

    /// <summary>
    /// The file path 
    /// </summary>
    public static string filePath;

    /// <summary>
    /// The point fake button 
    /// </summary>
    public bool PointFakeButton = false;

    /// <summary>
    /// The angle between 
    /// </summary>
    public static float angleBetween = 0;

    /// <summary>
    /// The number of pointings 
    /// </summary>
    public static int numberOfPointings = 0;

    /// <summary>
    /// The sum of errors 
    /// </summary>
    public static float sumOfErrors = 0.0f;

    /// <summary>
    /// The avarage error 
    /// </summary>
    public static float avarageError = 0.0f;

    /// <summary>
    /// The target 
    /// </summary>
    public Transform target;

    /// <summary>
    /// The start.
    /// </summary>
    public static GameObject  start;

    /// <summary>
    /// The absolute error angle.
    /// </summary>
    public static  float AbsoluteErrorAngle;

    /// <summary>
    /// The end time poining.
    /// </summary>
    public static  string EndTimePoining;

    /// <summary>
    /// Starts this instance. 
    /// </summary>
    private void Start ()
    {
        displaytext = GameObject.Find("Displaytext");
        target = GameObject.Find("StartPoint").transform;


        start = GameObject.Find("CenterEyeAnchor");

    }

    /// <summary>
    /// Changes the object for debugger.
    /// </summary>
    public static void ChangeTheObjectForDebugger ()
    {
        start = GameObject.Find("OVRPlayerController");

        
    }

    /// <summary>
    /// Updates this instance. 
    /// </summary>
    void Update ()
    {
        // active only if in pointing stae of main state machine
        if (ManagerScript.getState() == ManagerScript.states.pointing)
        {
            // if button used for pointing is pressed
            if ((PointFakeButton || Input.GetKeyDown(KeyCode.K) || Input.GetButtonDown("360controllerButtonA")))
            {
                //compute 2d vectors and the angle between the target vector and the current forward vector
                Vector2 targetVector = new Vector2(target.position.x, target.position.z);
                Vector2 transformVector = new Vector2(start.transform.position.x, start.transform.position.z);
                Vector2 forwardVector = new Vector2(start.transform.forward.x, start.transform.forward.z);
                Vector2 targetDir = targetVector - transformVector;
                angleBetween = Vector3.Angle(targetDir, forwardVector);

                // TODO why do we need this
                Vector3 cross = Vector3.Cross(targetDir, forwardVector);
                if (cross.z < 0)
                {
                    angleBetween = -angleBetween;
                }
                    
                AbsoluteErrorAngle = Mathf.Abs(angleBetween);

                // TODO delete?
                SaveAngleBetweenOldWay();

                // update the statitics
                UpdateErrorAngleStatistics();
                // save how long it took to point
                EndTimePoining = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff");

                //update statistics in sql database
                testofsql.UpdateTriallist(testofsql.CURRENT_TRIAL_ID.ToString());
                testofsql.UpdateAndIncrease_Current_Triallist_ID();

                CancelInvoke("toLongPoint");

                // start a new trial
                ManagerScript.switchState(ManagerScript.states.NewTrial);

                PointFakeButton = false;
            }
        }
    }

    /// <summary>
    /// Start a new pointing period
    /// </summary>
    public void NewPointing ()
    {
        // stop pointing if subject takes to long to point
        Invoke("toLongPoint", timeForPointing);
        displaytext.GetComponent<TextMesh>().text = "Point to Origin";
        Invoke("clearGUItext", 1f);
    }

    /// <summary>
    /// Abort the trial if the subject takes to much time to point
    /// </summary>
    private void toLongPoint ()
    {
        ManagerScript.abortTrial();
    }

    /// <summary>
    /// Clears the GUI text
    /// </summary>
    private void clearGUItext ()
    {
        displaytext.GetComponent<TextMesh>().text = "";
    }

    /// <summary>
    /// Updates the error angle statistics. 
    /// </summary>
    private void UpdateErrorAngleStatistics ()
    {
        if (ManagerScript.useTrialForIngameStatistics)
        {
            sumOfErrors = sumOfErrors + Mathf.Abs(angleBetween);
            numberOfPointings++;
            avarageError = sumOfErrors / numberOfPointings;
        }
    }

    /// <summary>
    /// Saves the angle between old way. 
    /// </summary>
    //TODO can we delete this
    private void SaveAngleBetweenOldWay ()
    {
        if (!ManagerScript.TrialMissed)
        {
            // recordData.recordDataParameters(1, (angleBetween).ToString()); 
        } else
        {
            // recordData.recordDataParameters(2, (angleBetween).ToString()); 
        }
    }
}