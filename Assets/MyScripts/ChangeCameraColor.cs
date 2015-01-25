using UnityEngine;

public class ChangeCameraColor : MonoBehaviour
{
    public static string CondtionTypeVariableInContainer;
    public static string CondtionTypeVariableInContainerOld;
    private GameObject cam1;
    private GameObject cam2;
    private ChangeRenderSettings cr;

    // Use this for initialization 
    private void Start ()
    {
        // find the two camers of oculus rift
        cam1 = GameObject.Find("RightEyeAnchor");
        cam2 = GameObject.Find("LeftEyeAnchor");

        // find script which changes the render settings
        cr = (ChangeRenderSettings)GameObject.Find("helperObject").GetComponent("ChangeRenderSettings");
    }

    // Update is called once per frame 
    // check if trial condition (har, easy, easy-false, hard-false) have changed. save old condition and call changeSettings function
    private void Update ()
    {
        CondtionTypeVariableInContainer = ManagerScript.CondtionTypeVariableInContainer;
        if (CondtionTypeVariableInContainer != CondtionTypeVariableInContainerOld)
        {
            ChangeSettings();
            CondtionTypeVariableInContainerOld = CondtionTypeVariableInContainer;
        }
    }


    // call changeRenderSettings script to change render settings according to the trial conditions
    private void ChangeSettings ()
    {
        if (CondtionTypeVariableInContainer == "Easy" || CondtionTypeVariableInContainer == "Easy-False")
        {
            // Debug.Log("easy camera"); 
            cam1.SetActive(true);
            cam2.SetActive(true);
            // switch to green ambient light conditions
            cr.switchEasy();
        } else if (CondtionTypeVariableInContainer == "Hard" || CondtionTypeVariableInContainer == "Hard-False")
        {
            //Debug.Log("hard camera");
            cam1.SetActive(true);
            cam2.SetActive(true);
            // switch to red ambient light conditions
            cr.switchHard();
        } else if (CondtionTypeVariableInContainer == "Training" || CondtionTypeVariableInContainer == "Explain" || CondtionTypeVariableInContainer == "PostBaseline" || CondtionTypeVariableInContainer == "PreBaseline")
        {
            // Debug.Log("no cond camera"); 
            cam1.SetActive(true);
            cam2.SetActive(true);
            // switch to normal ambient light conditions
            cr.switchNormal();

        } else if (CondtionTypeVariableInContainer == "ENDTRIAL")
        {
            // also possible to deactivate cameras at the end trial. in this experiment the end game 2d GUI is shown
            cam1.SetActive(true);
            cam2.SetActive(true);
            // switch to normal ambient light conditions
            cr.switchNormal();
        }
    }
}