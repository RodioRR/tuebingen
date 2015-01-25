// *********************************************************************** Assembly :
// Assembly-CSharp Author : razial Created : 01-07-2015 Created : 01-07-2015
// 
// Last Modified By : razial Last Modified On : 01-08-2015 Last Modified On : 01-08-2015
// <copyright file="testofsql.cs" company="INLUSIO">
//     Copyright (c) INLUSIO. All rights reserved. 
// </copyright>
// <summary>
// this class handles everything related to saving and retrieving data to the data base 
// </summary>
// *********************************************************************** 
using Mono.Data.SqliteClient;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using System;
using System.IO;

/// <summary>
/// this class handles everything related to saving and retrieving data to the data base 
/// </summary>
public class testofsql : MonoBehaviour
{
    /// <summary>
    /// The instance 
    /// </summary>
    public static testofsql Instance = null;

    /// <summary>
    /// The mConnection is the IDbConnection Object we will use to connect to the Database 
    /// </summary>
    private static IDbConnection mConnection = null;

    /// <summary>
    /// The mCommand is the IDbCommand Object, we will assign querry`s we want to run in the DB,
    /// without returning values
    /// </summary>
    private static IDbCommand mCommand = null;

    /// <summary>
    /// The mReader is the IDataReader Object, we will assign querry`s we want to run in the DB,
    /// with returning values
    /// </summary>
    private static IDataReader mReader = null;

    /// <summary>
    /// The mSQL string is a string we use frequently to put remporaly the querries to be execuded 
    /// </summary>
    public static string mSQLString;

    /// <summary>
    /// The m create new table 
    /// </summary>
    public bool mCreateNewTable = false;

    // the id of the currentTriallistEnty is the Last_Triallist_id_Putted_In - current TrialNumber lol 
    /// <summary>
    /// The last_ triallist_id_ putted_ in 
    /// </summary>
    public static int Last_Triallist_id_Putted_In;

    /// <summary>
    /// The subjec t_ identifier 
    /// </summary>
    public static int SUBJECT_ID;

    /// <summary>
    /// The sessio n_ identifier 
    /// </summary>
    public static int SESSION_ID;

    /// <summary>
    /// The las t_ inserte d_ triallist_ identifier 
    /// </summary>
    public static int LAST_INSERTED_Triallist_ID;

    /// <summary>
    /// The firs t_ inserte d_ triallist_ identifier 
    /// </summary>
    public static int FIRST_INSERTED_Triallist_ID;

    /// <summary>
    /// The current_ triallist_ identifier 
    /// </summary>
    public static int Current_Triallist_ID; // this needs

    /// <summary>
    /// The curren t_ tria l_ identifier 
    /// </summary>
    public static int CURRENT_TRIAL_ID;

    /// <summary>
    /// The comand sum to be execuded in the end of each trial 
    /// </summary>
    public static string comandSumToBeExecudedInTheEndOfEachTrial;

    /// <summary>
    /// The easy difficulty level 
    /// </summary>
    public static float EasyDifficultyLevel;

    /// <summary>
    /// The hard difficulty level 
    /// </summary>
    public static float HardDifficultyLevel;

    public static string StringForAccomulatingDynamicDifficultyEvents ;
    /// <summary>
    /// The after block number 
    /// </summary>
    static int AfterBlockNumber = 0;

    public static int CurentWaypointId;

    public static string SaveDynamicDifficultyEventString = "";

    public static List<trialContainer> trialList2 = new List<trialContainer>();

    static string SQL_DB_LOCATION = @"URI=file:C:\temp\inlusio_data\InlusioDB.sqlite";
    static string PathWhereToCopy = @"C:\Dropbox\inlusio_data\InlusioDB" + System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".sqlite";
    static  string PathWhereToCopyBackup = @"C:\temp\inlusio_data\InlusioDB" + System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".sqlite";
    static string SQLiteDbLocationFile = @"C:\temp\inlusio_data\InlusioDB.sqlite";

    /// <summary>
    /// Basic initialization of SQLite This will be activated by the manager script 
    /// </summary>
    /// 
    void Awake ()
    {
        Instance = this;
    }

    public static void SQLiteInit ()
    {
        // the data base is here 

        // we connect to the data base 
        mConnection = new SqliteConnection(SQL_DB_LOCATION);
        mCommand = mConnection.CreateCommand();
        mConnection.Open();
        ExecuteQuerry("PRAGMA page_size = " + "4096" + ";");
        ExecuteQuerry("PRAGMA synchronous = " + "0" + ";");
        mConnection.Close();

    }

    /// <summary>
    /// Initials the savings to database. All the nessery enteties, like Subject, Session and
    /// Triallist are instantiated here In case they exist, their values are retrieved. When the
    /// Experiment will crush at any arbitrary time and is restarted, this function will make sure,
    /// the experiment starts from exactly the moment it crashed
    /// </summary>
    /// <param name="chiffre">   The chiffre. </param>
    /// <param name="EasyDelay"> The easy delay. </param>
    /// <param name="HardDealy"> The hard dealy. </param>
    /// <param name="session">   The session. </param>
    /// <param name="trialList"> The trial list. </param>
    public static void InitialSavingsToDB (string chiffre, string EasyDelay, string HardDealy, string session, List<trialContainer> trialList)
    {
        mConnection.Open();

        
        /// Subject entety code 
        // Check if Subject_Number exists, if no, we create him 
        #region

        // if the Subject does not exist 
        if (QueryInt("SELECT EXISTS(SELECT * FROM Subject WHERE Subject_Number='" + chiffre + "' LIMIT 1);") == 0)
        {
            ExecuteQuerry("INSERT INTO 'Subject'('Subject_Number','EasyDifficultyLevel','HardDifficultyLevel') VALUES ('" + chiffre + "','" + EasyDelay + "','" + HardDealy + "');");
            // geting the fresh created Subject_id 
            SUBJECT_ID = QueryInt("SELECT Subject_id FROM Subject WHERE Subject_Number = '" + chiffre + "'");
        }
        // If he exists, lets grab his Number and update the dynamic difficulty to the level from
        // his previos session
        else
        {
            SUBJECT_ID = QueryInt("SELECT Subject_id FROM Subject WHERE Subject_Number = '" + chiffre + "'");
            // when he exists, we need to update the dynamic difficulty to the level from his
            // previos session
            EasyDifficultyLevel = QueryFloat("SELECT EasyDifficultyLevel FROM Subject WHERE SUbject_Number = '" + chiffre + "'");
            HardDifficultyLevel = QueryFloat("SELECT HardDifficultyLevel FROM Subject WHERE SUbject_Number = '" + chiffre + "'");
            Stressor.SetDinamicDifficultyFromLastSession(EasyDifficultyLevel, HardDifficultyLevel);
        }
        #endregion

        /// Session and Trial list enteties code 
        // Check if session exists allready, if no initialize the session and the trial list. Else
        // retrieve the old unfinished trials from Triallist
        #region

        // check if session exists, if not we will create it 
        if (QueryInt("SELECT EXISTS(SELECT * FROM Session WHERE Subject_id='" + SUBJECT_ID + "' AND SessionNumber='" + session + "'  LIMIT 1);") == 0)
        {
            // Session creation 

            ExecuteQuerry(" INSERT INTO Session (Subject_ID, Timestamp, SessionNumber) VALUES ("
                + "'" + SUBJECT_ID + "','"
                + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff") + "','"
                + session + "'" + ");");
            SESSION_ID = QueryInt("SELECT last_insert_rowid()");

            // trial list creation 
            string SqlComands = "";

            for (int i=0; i < trialList.Count; i++)
            {
                SqlComands = SqlComands + "INSERT INTO Trialist (Session_id,Type) VALUES ("
                    + "'" + SESSION_ID + "','"
                    + trialList [i].CondtionTypeVariableInContainer + "');";
            }
            ExecuteBigQuerry(SqlComands);
            LAST_INSERTED_Triallist_ID = QueryInt("SELECT last_insert_rowid()");
            FIRST_INSERTED_Triallist_ID = LAST_INSERTED_Triallist_ID - trialList.Count;
        } else
        {
            SESSION_ID = QueryInt("SELECT Session_id FROM Session WHERE Subject_id = '" + SUBJECT_ID + "' AND SessionNumber = '" + session + "'");

            mSQLString = "SELECT Type FROM Trialist WHERE Session_id = ( " +
                "SELECT Session_id FROM Session WHERE Subject_id = ( " +
                "SELECT Subject_id FROM Subject WHERE Subject_Number =" + chiffre +
                ") AND SessionNumber =" + session +
                ")  AND Done < 1; ";
            Debug.Log("bla");
            mCommand.CommandText = mSQLString;
            mReader = mCommand.ExecuteReader();
            while (mReader.Read())
            {
                trialContainer tempTrial = new trialContainer(mReader.GetString(0));
                trialList2.Add(tempTrial);

            }
            mReader.Close();
            ManagerScript.RestoreTrialListFromPreviosSessionOfSubject(trialList2);


            mSQLString = "SELECT Triallist_id FROM Trialist WHERE Session_id = ( " +
                "SELECT Session_id FROM Session WHERE Subject_id = ( " +
                "SELECT Subject_id FROM Subject WHERE Subject_Number =" + chiffre +
                ") AND SessionNumber =" + session +
                ")  AND Done < 1 order  by rowid desc limit 1 ; ";

            LAST_INSERTED_Triallist_ID = QueryInt(mSQLString);

            mSQLString = "SELECT Triallist_id FROM Trialist WHERE Session_id = ( " +
                "SELECT Session_id FROM Session WHERE Subject_id = ( " +
                "SELECT Subject_id FROM Subject WHERE Subject_Number =" + chiffre +
                ") AND SessionNumber =" + session +
                ")  AND Done < 1 order  by rowid  limit 1 ; ";
            
            
            
            FIRST_INSERTED_Triallist_ID = QueryInt(mSQLString);



            
            // if it exists, check if it is finished 

            //if exists and not finished, hmmm ... in theory, count the remaining trials and start the generation of a new
            //trial list
            // currentlly lets just start over , also this is not a niec sollution, we need to redo it ...

            // best case, the session is created, lets get the 
        }

        #endregion
    }

    /// <summary>
    /// here we will have a function for creating trials. each time we create a trial we get its
    /// trial id and set the triallist id
    /// </summary>
    /// <param name="StartTimeTrial">  The start time trial. </param>
    /// <param name="TrialNumber">     The trial number. </param>
    /// <param name="RealTrialNumber"> The real trial number. </param>
    /// <param name="Type">            The type. </param>
    public static void CreateTrial (string StartTimeTrial, string TrialNumber, string RealTrialNumber, string Type)
    {
        Current_Triallist_ID = FIRST_INSERTED_Triallist_ID + ManagerScript.realTrialNumber;
        ExecuteQuerry(" INSERT INTO Trial (Session_id,StartTimeTrial,Triallist_id,TrialNumber,RealTrialNumber,Type) VALUES ( "
            + "'" + SESSION_ID + "','"
            + StartTimeTrial + "','"
            + Current_Triallist_ID + "','"
            + TrialNumber + "','"
            + RealTrialNumber + "','"
            + Type + "');");

        // after we create a trial, we need to knew about it 
        CURRENT_TRIAL_ID = QueryInt("SELECT last_insert_rowid()");
    }

    /// <summary>
    /// Updates the trial. 
    /// </summary>
    /// <param name="argument">          
    /// The argument can be either "abort" or "success", so we decide what to save to DB.
    /// </param>
    /// <param name="AbsoluteErrorAngle"> The absolute error angle. </param>
    /// <param name="ErrorAngle">         The error angle. </param>
    /// <param name="OverShoot">          The over shoot. </param>
    /// <param name="StartTimePointing">  The start time pointing. </param>
    /// <param name="EndTimePoining">     The end time poining. </param>
    /// <param name="DurationOfPointing"> The duration of pointing. </param>
    /// <param name="DurationOfWalking">  The duration of walking. </param>
    /// <param name="EndTimeTrial">       The end time trial. </param>
    public static void UpdateTrial (string argument, string AbsoluteErrorAngle, string ErrorAngle, string OverShoot, string StartTimePointing, string EndTimePoining, string EndTimeTrial)
    {
        if (argument == "abort")
        {
            mSQLString = " UPDATE 'Trial' SET 'Success'=0,  'EndTimeTrial'= '" + EndTimeTrial + " ' WHERE _rowid_=" + CURRENT_TRIAL_ID + ";";
            ExecuteQuerry(mSQLString);

        }

        if (argument == "success")
        {
            mSQLString = " UPDATE 'Trial' SET 'Success'=1 , 'AbsoluteErrorAngle'=" + AbsoluteErrorAngle + ", 'OverShoot'= " + OverShoot + ", 'ErrorAngle'= " + ErrorAngle + ", 'StartTimePointing'=' " + StartTimePointing + "',  'EndTimePoining'=' " + EndTimePoining + "', 'EndTimeTrial'=' " + EndTimeTrial + "'  WHERE _rowid_=" + CURRENT_TRIAL_ID + ";";
            Debug.Log(mSQLString);
            ExecuteQuerry(mSQLString);

            UpdateTriallist(CURRENT_TRIAL_ID.ToString());
        }
        
    }

    /// <summary>
    /// Creates the stressor. 
    /// </summary>
    /// <param name="Stressors_id">         The stressors_id. </param>
    /// <param name="SpawnTime">            The spawn time. </param>
    /// <param name="StartDefeatTime">      The start defeat time. </param>
    /// <param name="HowLongDefeatable">    The how long defeatable. </param>
    /// <param name="DefeatedAtTime">       The defeated at time. </param>
    /// <param name="Defeated">             The defeated. </param>
    /// <param name="RotationSpeed">        The rotation speed. </param>
    /// <param name="ButtonToEarlyPushed">  The button to early pushed. </param>
    /// <param name="Type">                 The type. </param>
    /// <param name="DefeatableTimeWindow"> The defeatable time window. </param>
    /// <param name="ReactionTime">         The reaction time. </param>
    /// <param name="Trial_id">             The trial_id. </param>
    /// <param name="ExplosionTime">        The explosion time. </param>
    public static void CreateStressor (string command, string DistanceToPlayerBeforeGone, string SpawnTime, string StartDefeatTime, string HowLongDefeatable, string DefeatedAtTime, string RotationSpeed, string ButtonToEarlyPushed, string Type, string Trial_id, string ExplosionTime)
    {

        if (command == "Missed")
        {


            string CreateStressor =
                " INSERT INTO 'Stressors' ('SpawnTime','StartDefeatTime','DefeatableTimeWindow','DefeatedAtTime','Defeated','RotationSpeed','ButtonToEarlyPushed','Type','ReactionTime','Trial_id','ExplosionTime','DistanceToPlayerBeforeGone') VALUES"
                + "(" 
                + "'" + SpawnTime + "',"
                + "'" + StartDefeatTime + "',"
                + "'" + HowLongDefeatable + "',"
                + "'" + DefeatedAtTime + "',"
                + "'" + 0 + "',"
                + "'" + RotationSpeed + "',"
                + "'" + ButtonToEarlyPushed + "',"
                + "'" + Type + "',"
                + "'" + "NULL" + "',"
                + "'" + Trial_id + "',"
                + "'" + ExplosionTime + "',"
                + "'" + DistanceToPlayerBeforeGone + "');";
            ExecuteQuerry(CreateStressor);


        } else if (command == "Defeated")
        {

            string CreateStressor =
                " INSERT INTO 'Stressors' ('SpawnTime','StartDefeatTime','DefeatedAtTime','DefeatableTimeWindow','Defeated','RotationSpeed','ButtonToEarlyPushed','Type','Trial_id' , 'DistanceToPlayerBeforeGone', 'ReactionTime') VALUES"
                + "(" 
                + "'" + SpawnTime + "',"
                + "'" + StartDefeatTime + "',"
                + "'" + DefeatedAtTime + "',"

                + "'" + HowLongDefeatable + "',"
                + "'" + 1 + "',"
                + "'" + RotationSpeed + "',"
                + "'" + ButtonToEarlyPushed + "',"
                + "'" + Type + "',"
                + "'" + Trial_id + "',"
                + "'" + DistanceToPlayerBeforeGone + "',"
                + "JULIANDAY(datetime(" + "'" + DefeatedAtTime + "'" + ")) - JULIANDAY(" + "'" + StartDefeatTime + "'" + ") *24*3600" +
                ");";

            ExecuteQuerry(CreateStressor);


        }


    }

    /// <summary>
    /// Creates the waypoint. 
    /// </summary>
    /// <param name="Waypoints_id">      The waypoints_id. </param>
    /// <param name="DegreeOfRespawn">   The degree of respawn. </param>
    /// <param name="TimeWhenRespawned"> The time when respawned. </param>
    /// <param name="GlobalCoordinats">  The global coordinats. </param>
    /// <param name="TransformRotation"> The transform rotation. </param>
    /// <param name="NumberInTrial">     The number in trial. </param>
    /// <param name="Trial_id">          The trial_id. </param>
    /// <param name="reached">           The reached. </param>
    /// <param name="TimeWhenReached">   The time when reached. </param>
    public static void CreateWaypoint (string DegreeOfRespawn, string TimeWhenRespawned, string GlobalCoordinats, string TransformRotation, string NumberInTrial)
    {
        string CreateWaypoint = "INSERT INTO 'Waypoints'('DegreeOfRespawn','TimeWhenRespawned','GlobalCoordinats','TransformRotation','NumberInTrial','Trial_id') VALUES"
            + "('" + DegreeOfRespawn + "','"
            + TimeWhenRespawned + "','"
            + GlobalCoordinats + "','"
            + TransformRotation + "','"
            + NumberInTrial + "','"
            + CURRENT_TRIAL_ID.ToString() + "');";
        ExecuteQuerry(CreateWaypoint);

        CurentWaypointId = QueryInt(("SELECT last_insert_rowid()"));
    }

    public static void UpdateWaypoint (string reached, string TimeWhenReached)
    {

        mSQLString = " UPDATE 'Waypoints' SET 'reached'= '" + reached + "','TimeWhenReached'= '" + TimeWhenReached + "'  WHERE _rowid_=" + CurentWaypointId.ToString() + ";";
        ExecuteQuerry(mSQLString);
    }

    /// <summary>
    /// Updates the session to successfull state after all trials are done in the trial list and the
    /// manager script changes to the end state.
    /// </summary>
    public static void UpdateSession ()
    {
        ExecuteQuerry(" UPDATE 'Session' SET 'finished'= 1 WHERE Session_id = " + SESSION_ID + ";");
    }

    /// <summary>
    /// Saves the statisics to data base. 
    /// </summary>
    /// <param name="NumberOfYellowSpawn">   The number of yellow spawn. </param>
    /// <param name="NumberOfYellowDefeted"> The number of yellow defeted. </param>
    /// <param name="NumberOfYellowMissed">  The number of yellow missed. </param>
    /// <param name="abortedTrials">         The aborted trials. </param>
    /// <param name="avarageError">          The avarage error. </param>
    public static void SaveStatisicsToDataBase (string NumberOfYellowSpawn, string NumberOfYellowDefeted, string NumberOfYellowMissed, string abortedTrials, string avarageError)
    {
        AfterBlockNumber++;

        string CreateStatisticsInGameEntryAfterBlock = "INSERT INTO 'StaticsInGame'('Session','NumberOfYellowSpawn','NumberOfYellowDefeted','NumberOfYellowMissed','abortedTrials','avarageError','AfterBlockNumber') VALUES ('" + SESSION_ID + "','"
            + NumberOfYellowSpawn + "','"
            + NumberOfYellowDefeted + "','"
            + NumberOfYellowMissed + "','"
            + abortedTrials + "','"
            + avarageError + "','"
            + AfterBlockNumber + "');";

        ExecuteQuerry(CreateStatisticsInGameEntryAfterBlock);
    }

    /// <summary>
    /// Updates the triallist. 
    /// </summary>
    /// <param name="CURRENT_TRIAL_ID"> The curren t_ tria l_ identifier. </param>
    public static void UpdateTriallist (string CURRENT_TRIAL_ID)
    {
//        Debug.Log(CURRENT_TRIAL_ID);
        mSQLString = " UPDATE 'Trialist' SET 'Done'=1  WHERE _rowid_=" + Current_Triallist_ID + ";";
        ExecuteQuerry(mSQLString);
    }

    /// <summary>
    /// Creates the pause. 
    /// </summary>
    /// <param name="StartTimePause"> The start time pause. </param>
    /// <param name="EndTimePause">   The end time pause. </param>
    public static void CreatePause (string StartTimePause, string EndTimePause)
    {
        string mSQLString1 = "INSERT INTO 'Pause'('trial','StartTimePause','EndTimePause') VALUES" +
            "('" + CURRENT_TRIAL_ID + "','"
            + StartTimePause + "','"
            + EndTimePause + "')";
        ExecuteQuerry(mSQLString1);
    }

    /// <summary>
    /// Sets the dynamic difficulty. 
    /// </summary>
    /// <param name="EasyDelay"> The easy delay. </param>
    /// <param name="HardDealy"> The hard dealy. </param>
    public static void SetDynamicDifficulty (string EasyDelay, string HardDealy) // each block we should update the difficulty of the subject in the data base lol
    {
        string mSQLString1 =
            " UPDATE 'Subject' SET 'EasyDifficultyLevel'=" + EasyDelay + " WHERE Subject_Number = '" + ManagerScript.chiffre + "';";
        string mSQLString2 =
            " UPDATE 'Subject' SET 'HardDifficultyLevel'=" + HardDealy + " WHERE Subject_Number = '" + ManagerScript.chiffre + "';";
        ExecuteQuerry(mSQLString1);
        ExecuteQuerry(mSQLString2);
        Debug.Log(mSQLString1 + "and" + mSQLString2);
    }

    public static void SaveDynamicDifficultyEvent2 ()
    {

        ExecuteBigQuerry(StringForAccomulatingDynamicDifficultyEvents);
        StringForAccomulatingDynamicDifficultyEvents = "";



    }

    public static void CreateDynamicDifficultyEvent (string TypeOfChange, string TimeofChange)
    {

        string EventString = "INSERT INTO `DynamicDifficulty`(`TypeOfChange`,`Session_id`,`TimeOfChange`) VALUES ('" + TypeOfChange + "','" + SESSION_ID + "','" + TimeofChange + "');";




        StringForAccomulatingDynamicDifficultyEvents = StringForAccomulatingDynamicDifficultyEvents + EventString;

    }


    /// <summary>
    /// Saves Dynamic difficulty events , by getting them together, and in the end of the trial they
    /// should be saved
    /// </summary>

    public static void SaveDynamicDifficultyEvent ()
    {
        if (!string.IsNullOrEmpty(SaveDynamicDifficultyEventString))
        {
            ExecuteBigQuerry(SaveDynamicDifficultyEventString);
            SaveDynamicDifficultyEventString = "";

        }

    }

    public static void SaveDynamicDifficultyEvent (string DynamicDifficultyEventString)
    {
        SaveDynamicDifficultyEventString = SaveDynamicDifficultyEventString + DynamicDifficultyEventString;
    }

    public static void SaveDynamicDifficultyEvent (string StunTime, string UnstunTime, string OldSpeed, string NewSpeed)
    {
        string mSQLString1 = "INSERT INTO 'Stun'('trial','StunTime','UnstunTime','OldSpeed','NewSpeed') VALUES" +
            "('" + CURRENT_TRIAL_ID + "','"
            + StunTime + "','"
            + UnstunTime + "','"
            + OldSpeed + "','"
            + NewSpeed + "');";

        SaveDynamicDifficultyEvent(mSQLString1);
    }

    public static void UpdateAndIncrease_Current_Triallist_ID ()
    {
        string sqlquerry = " UPDATE 'Trialist' SET 'Done'= 1" + " WHERE Triallist_id = '" + Current_Triallist_ID + "';";
        ExecuteQuerry(sqlquerry);
        Current_Triallist_ID++;
    }

    /// <summary>
    /// Executes the big querry. 
    /// </summary>
    /// <param name="sqlcomands"> The sqlcomands. </param>
    public static void ExecuteBigQuerry (string sqlcomands)
    {
        string mSQLString2 = "BEGIN; " + sqlcomands + " COMMIT;";

        ExecuteQuerry(mSQLString2);
    }

    /// <summary>
    /// Executes the querry. 
    /// </summary>
    /// <param name="sqlcomand"> The sqlcomand. </param>
    public static void ExecuteQuerry (string sqlcomand)
    {
//        Debug.Log(sqlcomand);
        mCommand.CommandText = sqlcomand;
        Debug.Log(sqlcomand);
        mCommand.ExecuteNonQuery();
    }

    // yellow spheres and blue spheres do call this function to add the proper querry 
    /// <summary>
    /// Sums the incoming querries up. 
    /// </summary>
    /// <param name="incomingString"> The incoming string. </param>
    public void SumTheIncomingQuerriesUp (string incomingString)
    {
        comandSumToBeExecudedInTheEndOfEachTrial = comandSumToBeExecudedInTheEndOfEachTrial + incomingString;
    }

    // at the end of the trial we will write the SumQurry of blue and yellow spheres still not sure
    // how to update but maybe i can use last insert as an argument ??? CHECK IF YOU CAN DO THIS
    /// <summary>
    /// Executes the sum querry. 
    /// </summary>
    /// <param name="SumQuerry"> The sum querry. </param>
    public void ExecuteSumQuerry (string SumQuerry)
    {
        string mSQLString2 = "BEGIN; " + SumQuerry + " COMMIT;"; // build our new command

        mCommand.CommandText = mSQLString2;// assing the comand
        // mConnection.Open(); 
        mCommand.ExecuteNonQuery();
        //mConnection.Close();
        comandSumToBeExecudedInTheEndOfEachTrial = ""; // reset the sum of commands
    }

    /// <summary>
    /// Queries the int. 
    /// </summary>
    /// <param name="command"> The command. </param>
    /// <returns> System.Int32. </returns>
    public static int QueryInt (string command)
    {
        int number = 0;

        mCommand.CommandText = command;
        // mConnection.Open(); 
        mReader = mCommand.ExecuteReader();
        if (mReader.Read())
            number = mReader.GetInt32(0);
        else
            Debug.Log("QueryInt - nothing to read...");
        mReader.Close();
        return number;
    }

    /// <summary>
    /// Queries the float. 
    /// </summary>
    /// <param name="command"> The command. </param>
    /// <returns> System.Single. </returns>
    public static float QueryFloat (string command)
    {
        float number = 0;

        mCommand.CommandText = command;
        // mConnection.Open(); 
        mReader = mCommand.ExecuteReader();
        if (mReader.Read())
            number = mReader.GetFloat(0);
        else
            Debug.Log("QueryInt - nothing to read...");
        mReader.Close();
        // mConnection.Close(); 
        return number;
    }

    /// <summary>
    /// Called when [destroy]. 
    /// </summary>
  

    /// <summary>
    /// Called when [application quit]. 
    /// </summary>
    private void OnApplicationQuit ()
    {
        mConnection.Close();
        SQLiteClose();
    }

    /// <summary>
    /// Clean up everything for SQLite 
    /// </summary>
    private void SQLiteClose ()
    {
        if (mReader != null && !mReader.IsClosed)
            mReader.Close();
        mReader = null;

        if (mCommand != null)
            mCommand.Dispose();
        mCommand = null;

        if (mConnection != null && mConnection.State != ConnectionState.Closed)
            mConnection.Close();
        mConnection = null;


        try
        {
            
            System.IO.File.Copy(SQLiteDbLocationFile, PathWhereToCopy);

        } catch (Exception e)
        { 
            Debug.Log(e.ToString());


            try
            {
                System.IO.File.Copy(SQLiteDbLocationFile, PathWhereToCopyBackup);
            } catch (Exception e2)
            {
                Debug.Log(e2.ToString());
            }
        }



    }
}