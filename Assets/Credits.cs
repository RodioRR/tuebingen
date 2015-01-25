using UnityEngine;

/// <summary>
/// Class Pause. 
/// </summary>
public class Credits : VRGUI
{
	public GUISkin skin;


		public override void OnVRGUI ()
	
		{
		// GUI.skin = skin;
		
		// show pause 2d GUI if paused or experiment is over

			// draw 2D GUI
			GUI.enabled = true;
			GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
			GUILayout.BeginVertical("box");
			GUILayout.FlexibleSpace();

			GUILayout.Label("<color=lime> CREDITS </color>");
			GUILayout.Label("We hope you had fun!\n");

			GUILayout.Label("Programmers: \n\n Petr Legkov \n Krzysztof Izdebski \n\n\n Belt team \n\n\n Special thx to Tub team");
			
			GUILayout.EndVertical();
			GUILayout.EndArea();
	}
	private void Start ()
	{
		}
}