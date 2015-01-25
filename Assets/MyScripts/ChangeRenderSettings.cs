// *********************************************************************** Assembly :
// Assembly-CSharp Author : razial Created : 12-29-2014
// 
// Last Modified By : razial Last Modified On : 01-02-2015 ***********************************************************************
// <copyright file="ChangeRenderSettings.cs" company="INLUSIO">
//     Copyright (c) INLUSIO. All rights reserved. 
// </copyright>
// <summary>
// </summary>
// *********************************************************************** 
using UnityEngine;

/// <summary>
/// Class ChangeRenderSettings. 
/// </summary>
public class ChangeRenderSettings : MonoBehaviour
{
    /// <summary>
    /// The fog color for standard ambient light conditions
    /// </summary>
    private Color fogColorNormal = Color.black;

    /// <summary>
    /// The fog color easy ambient light conditions
    /// </summary>
    private Color fogColorEasy = new Color(0.0F / 255, 13F / 255, 2F / 255);

    /// <summary>
    /// The fog color hard ambient light conditions
    /// </summary>
    private Color fogColorHard = new Color(15F / 255, 0.0F / 255, 0.0F / 255);

    /// <summary>
    /// The standard ambient light color 
    /// </summary>
    private Color ambientLightColorNormal = new Color(82F / 255, 82F / 255, 82F / 255);

    /// <summary>
    /// The ambient light color for easy, false easy ... trials 
    /// </summary>
    private Color ambientLightColorEasy = new Color(61F / 255, 145F / 255, 81F / 255);

    /// <summary>
    /// The ambient light color for hard, false hard ... trials 
    /// </summary>
    private Color ambientLightColorHard = new Color(145F / 255, 61F / 255, 61F / 255);

    /// <summary>
    /// The cam1 
    /// </summary>
    private Camera cam1;

    /// <summary>
    /// The cam2 
    /// </summary>
    private Camera cam2;

    // Use this for initialization 
    /// <summary>
    /// Starts this instance. 
    /// </summary>
    private void Start ()
    {
        //find the two oculus cameras on startup
        cam1 = GameObject.Find("RightEyeAnchor").camera;
        cam2 = GameObject.Find("LeftEyeAnchor").camera;
    }

    /// <summary>
    /// Switches the easy ambient light conditions
    /// </summary>
    public void switchEasy ()
    {
        RenderSettings.ambientLight = ambientLightColorEasy;
        RenderSettings.fogColor = fogColorEasy;
        RenderSettings.fogDensity = 0.02f;
        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.ExponentialSquared;
        // the backgorund color is also set to the fog colour else the background would be visible in the "sky" where no fog is present
        cam1.backgroundColor = fogColorEasy;
        cam2.backgroundColor = fogColorEasy;
    }

    /// <summary>
    /// Switches the hard ambient light conditions
    /// </summary>
    public void switchHard ()
    {
        RenderSettings.ambientLight = ambientLightColorHard;
        RenderSettings.fogColor = fogColorHard;
        RenderSettings.fogDensity = 0.02f;
        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.ExponentialSquared;
        // the backgorund color is also set to the fog colour else the background would be visible in the "sky" where no fog is present
        cam1.backgroundColor = fogColorHard;
        cam2.backgroundColor = fogColorHard;
    }

    /// <summary>
    /// Switches the normal ambient light conditions
    /// </summary>
    public void switchNormal ()
    {
        RenderSettings.ambientLight = ambientLightColorNormal;
        RenderSettings.fogColor = fogColorNormal;
        RenderSettings.fogDensity = 0.02f;
        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.ExponentialSquared;
        // the backgorund color is also set to the fog colour else the background would be visible in the "sky" where no fog is present
        cam1.backgroundColor = fogColorNormal;
        cam2.backgroundColor = fogColorNormal;
    }
}