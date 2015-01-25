using UnityEngine;

public class StartMenu3dGui : VRGUI
{
    /// <summary>
    /// The skin.
    /// </summary>
    public GUISkin skin;

    /// <summary>
    /// The session identifier.
    /// </summary>
    public string SubjcetID;

    /// <summary>
    /// The session number.
    /// </summary>
    public string SessionNumber;

    /// <summary>
    /// The debugg field.
    /// </summary>
    public string debuggField;

    /// <summary>
    /// set to true if data is entered successful
    /// </summary>
    private bool success = true;

    /// <summary>
    /// The debugg.
    /// </summary>
    public static  int debugg = 0;

    /// <summary>
    /// Raises the VRGU event.
    /// </summary>
    public override void OnVRGUI ()
    {
        // at the beginning of the experiment show this GUI with fields to enter subject id, session number and debug flags
        if (ManagerScript.getState() == ManagerScript.states.startScreen)
        {
            // create the GUI 2D Window
            GUI.skin = skin;
            GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
            GUILayout.BeginVertical("box");

            // subject id
            GUILayout.Label("<color=lime> Subject ID: </color>");
            SubjcetID = GUILayout.TextField(SubjcetID, 25);


            // session number
            GUILayout.Label("<color=lime> Session Number:  </color>");
            SessionNumber = GUILayout.TextField(SessionNumber, 25);


            // debug flag
            GUILayout.Label("<color=lime> Debug Flag (1 = Debug): </color>");
            debuggField = GUILayout.TextField(debuggField, 25);
            ManagerScript.debugg = debugg;

            // if ok button is pressed, deactivate startmenu and switch to start state in main state machine
            if (GUILayout.Button("ok", GUILayout.ExpandHeight(true)))
            {
                success = true;
                // check if subject id is entered
                if (SubjcetID.Equals(""))
                {
                    success = false;
                }                            

                // check if valid session number is entered
                int temp;
                if (!int.TryParse(SessionNumber, out temp))
                {
                    success = false;
                } else if ((temp != 1) && (temp != 2) && (temp != 666))
                {
                    success = false;
                }

                // if data is entered successful transfer data to manager script
                if (success)
                {
                    int.TryParse(SessionNumber, out ManagerScript.session);
                    ManagerScript.chiffre = SubjcetID;
                    int.TryParse(debuggField, out debugg);
                    
                    // /generate trials and start with first trial
                    ManagerScript.switchState(ManagerScript.states.start);
                    
                    enabled = !enabled;
                }

            }

            GUILayout.EndVertical();
            GUILayout.EndArea();
        }
    }
}