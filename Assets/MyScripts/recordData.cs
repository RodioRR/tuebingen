// * This file will serve as a utility internal class to private help write private down the private values into CSVs
//*/
//using UnityEngine;
//using System.Collections;
//using System.IO;
//using System.Text;

//public static class recordData
//{
//    private static string delimiter = ",";

// //creates file for saving parameters public static void recordDataParametersInit () { string
// filePath = ManagerScript.trialFolder + "/parameters_" + ManagerScript.chiffre + "_" +
// (ManagerScript.session).ToString() + ".csv"; //Check if the file exists if
// (!File.Exists(filePath)) { File.Create(filePath).Close(); ManagerScript.parameterFile = filePath;
// Debug.Log(filePath); } }

// //records the paramters public static void recordDataParameters (int success, string error) {
// string successString = "-1";

// if (success == 1) { successString = "1"; } 

// if ((success == 2)) { successString = "0"; } 

// string conditionVal = ""; 

// if (ManagerScript.CondtionTypeVariableInContainer == "Easy") { conditionVal = "1"; } else if
// (ManagerScript.CondtionTypeVariableInContainer == "Hard") { conditionVal = "2"; } else if
// (ManagerScript.CondtionTypeVariableInContainer == "Easy-False") { conditionVal = "3"; } else if
// (ManagerScript.CondtionTypeVariableInContainer == "Hard-False") { conditionVal = "4"; } else if
// (ManagerScript.CondtionTypeVariableInContainer == "Training") { conditionVal = "5"; } else if
// (ManagerScript.CondtionTypeVariableInContainer == "ENDTRIAL") { conditionVal = "6"; } else {
// conditionVal = "0"; }

// //putting values for column in csv string[][] output = new string[][]{ new string[] {
// System.DateTime.UtcNow.ToString("o"), ManagerScript.realTrialNumber.ToString(), "ERROR", // this
// cant be saved liek this, it is generated per ball, not trial !!!
// ManagerScript.generatedAngle.ToString(), ManagerScript.timetoPointingStage.ToString(),
// ManagerScript.pointingTime.ToString(), ManagerScript.abortedTrials.ToString(),
// PointingScript.avarageError.ToString(), conditionVal,
// ManagerScript.CurrentOrientation.ToString(), error, successString.ToString() } };

// int length = output.GetLength(0); 

// StringBuilder sb = new StringBuilder(); 

// for (int index = 0; index < length; index++) sb.AppendLine(string.Join(delimiter, output
// [index])); File.AppendAllText(ManagerScript.parameterFile, sb.ToString()); }

// // Updates the csv for Smallspread with marker of spawned , destroyed or missed public static
// void recordDataSmallspread (string status, string durationResponse) { string dResponse = "-1";
// //string filePath = ManagerScript.trialFolder + "/Trial" + ManagerScript.trialNumber +
// "-Smallspread.csv"; string filePath = ManagerScript.trialFolder + "/Smallspread_" +
// ManagerScript.chiffre + "_" + (ManagerScript.session).ToString() + ".csv"; //Check if the file
// exists if (!File.Exists(filePath)) { File.Create(filePath).Close(); }

// string statusVal = ""; 

// if (status == "S") { statusVal = "0"; } else if (status == "M") { statusVal = "1"; } else if
// (status == "P") { statusVal = "3"; } else if (status == "PF") { statusVal = "4"; } else if
// (status == "Onset") { statusVal = "5"; dResponse = durationResponse; } else { statusVal = "2"; }

// //putting values for column in csv string[][] output = new string[][]{ new string[]{
// System.DateTime.UtcNow.ToString("o"), ManagerScript.realTrialNumber.ToString(), dResponse,
// (Time.realtimeSinceStartup).ToString(), statusVal} };

// int length = output.GetLength(0); 

// StringBuilder sb = new StringBuilder(); 

//        for (int index = 0; index < length; index++)
//            sb.AppendLine(string.Join(delimiter, output [index]));
//        File.AppendAllText(filePath, sb.ToString());
//    }
//}

///*
//        public static void recordDataFlow ()
//        {
//                string filePath = ManagerScript.trialFolder + "/Trial-" + ManagerScript.trialNumber + "-Flow.csv";
//                if (!File.Exists (filePath)) {
//                        File.Create (filePath).Close ();
//                }

// //putting values for column in csv string[][] output = new string[][]{ new string[]{
// //ManagerScript.trialNumber.ToString (), (transform.position.x).ToString(),
// (transform.position.y).ToString(), (transform.position.z).ToString()
// //(transform.forward).ToString(), //(Time.realtimeSinceStartup).ToString (),
// //(Time.deltaTime).ToString(), //(angleBetween).ToString (), //conditionVal
// //ManagerScript.CondtionTypeVariableInContainer } };

// int length = output.GetLength (0); 

// StringBuilder sb = new StringBuilder (); 

// for (int index = 0; index < length; index++) sb.AppendLine (string.Join (delimiter, output
// [index])); File.AppendAllText (filePath, sb.ToString ()); }
// * /