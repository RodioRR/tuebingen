/*
 * This class holds the parameters and also randomizes parameters 
 * for different types of trials
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class trialContainer
{
    // FIXME this is in fact saved but never assigned to the yellow spehre ... this should be redone !!! at least the saving
    public int spawnDistance ;
    public double CoolDown ;       // How long to hide
    public float timer_red ; // timer, than needs to reach CoolDown
    public float TimerForLooking ; // timer, than needs to reach CoolDownValue
    public int moveDistance;   // How close can the character get
    public float speed ;
    public Color bColor;
    public string CondtionTypeVariableInContainer;
    public bool isTraining;

    //	Transform temp1 ;

    public trialContainer ()
    {
    }

    public trialContainer (string trialType)
    {
//				 Camera.main.clearFlags = CameraClearFlags.SolidColor;

        if (trialType == "Easy")
        {
						
            //Placing the value in container
            bColor = Color.green;
            spawnDistance = 25;
            CoolDown = 2.0;    
            timer_red = 0.0f; 
            TimerForLooking = 0.0f; 
            moveDistance = 5;  
            speed = 5.0f;
            CondtionTypeVariableInContainer = "Easy";
		
        } else if (trialType == "Hard")
        {
						
            bColor = Color.red;
            spawnDistance = 25;
            CoolDown = 2.5;    
            timer_red = 0.0f; 
            TimerForLooking = 0.0f; 
            moveDistance = 5;  
            speed = 6.0f;
            CondtionTypeVariableInContainer = "Hard";
						
						
        } else if (trialType == "Easy-False")
        {
						
            bColor = Color.red;
            spawnDistance = 25;
            CoolDown = 2.5;    
            timer_red = 0.0f; 
            TimerForLooking = 0.0f; 
            moveDistance = 5;  
            speed = 6.0f;
            CondtionTypeVariableInContainer = "Easy-False";

						

        } else if (trialType == "Hard-False")
        {

            bColor = Color.red;
            spawnDistance = 25;
            CoolDown = 2.5;    
            timer_red = 0.0f; 
            TimerForLooking = 0.0f; 
            moveDistance = 5;  
            speed = 6.0f;
            CondtionTypeVariableInContainer = "Hard-False";

						
        } else if (trialType == "Training")
        {
					
            //Placing the value in container
            bColor = Color.black;
            spawnDistance = 30;
            CoolDown = 2.0;    
            timer_red = 0.0f; 
            TimerForLooking = 0.0f; 
            moveDistance = 5;  
            speed = 4.0f;	
            CondtionTypeVariableInContainer = "Training";

        } else if (trialType == "PostBaseline")
        {
            
            //Placing the value in container
            bColor = Color.black;
            spawnDistance = 30;
            CoolDown = 2.0;    
            timer_red = 0.0f; 
            TimerForLooking = 0.0f; 
            moveDistance = 5;  
            speed = 4.0f;   
            CondtionTypeVariableInContainer = "PostBaseline";

        } else if (trialType == "PreBaseline")
        {
            
            //Placing the value in container
            bColor = Color.black;
            spawnDistance = 30;
            CoolDown = 2.0;    
            timer_red = 0.0f; 
            TimerForLooking = 0.0f; 
            moveDistance = 5;  
            speed = 4.0f;   
            CondtionTypeVariableInContainer = "PreBaseline";
		
        } else if (trialType == "ENDTRIAL")
        {
					
            //Placing the value in container
            bColor = Color.black;
            spawnDistance = 30;
            CoolDown = 2.0;    
            timer_red = 0.0f; 
            TimerForLooking = 0.0f; 
            moveDistance = 5;  
            speed = 4.0f;	
            CondtionTypeVariableInContainer = "ENDTRIAL";
					
        } else if (trialType == "BLOCKOVER")
        {
					
            bColor = Color.black;
            spawnDistance = 2220;
            CoolDown = 200000.5;    
            timer_red = 0.0f; 
            TimerForLooking = 0.0f; 
            moveDistance = 5000000;  
            speed = 0.0f;
            CondtionTypeVariableInContainer = "BLOCKOVER";
        } else
        {
						
            bColor = Color.black;
            spawnDistance = 2220;
            CoolDown = 200000.5;    
            timer_red = 0.0f; 
            TimerForLooking = 0.0f; 
            moveDistance = 5000000;  
            speed = 0.0f;
            CondtionTypeVariableInContainer = "Explain";
        }
    }
}
