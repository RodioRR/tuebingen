using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathFinding : MonoBehaviour {
	//the horizontal coordinates
	public static List<float> pathElementsX = new List<float>(); 
	public static List<float> pathElementsZ = new List<float>();

	public int[] pathSequence = new int[]{8, 9, 11, 12, 13, 17, 7};
	public static int ii;
	public static int pointTo;
	//public OVRPlayerController i;
	void Start () {
		//getting coordinates of each waypoint and creating a list out of them
		foreach (Transform child in transform) {
						//Debug.Log (child.transform.position);
						pathElementsX.Add (child.transform.position.x);
						pathElementsZ.Add (child.transform.position.z);
				}
	}
	
	// Update is called once per frame
	void Update () {
		ii = OVRPlayerController.i;
			pointTo = pathSequence[ii];
	}
				

}

