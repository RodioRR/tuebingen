using UnityEngine;
using System;
using System.Collections;

public class ComPortAsk : MonoBehaviour {

	string textFieldStringScale="";

	// Use this for initialization
	void OnGUI() {

		textFieldStringScale=GUI.TextField(new Rect(Screen.width / 5 * 1, 250, 80, 20),textFieldStringScale,10);
		textFieldStringScale=GUI.TextField(new Rect(Screen.width / 5 * 3, 250, 80, 20),textFieldStringScale,10);
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
