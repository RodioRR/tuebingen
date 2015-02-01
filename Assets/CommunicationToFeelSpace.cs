using UnityEngine;
using System;
using System.Collections;
// this contains the serial port communication
using System.IO.Ports;


public class CommunicationToFeelSpace : MonoBehaviour {
	
	
	
	private SerialPort sp;
	public Transform character ;
	byte lsb;
	byte msb;
	public static float angleBetween = 0;

	/// <summary>
	/// variables needed for specific waypoint
	/// </summary>
	private int iii;
	private float currentX;
	private float currentZ;

	//private float angleBetween;
	//private Transform target;

	// Use this for initialization
	void Start () {
		
		// here we get the onject, that moves with the player, or the player, and than its transform optioons, that are his direction
		//character = GameObject.Find ("OVRPlayerController").transform;
		character = GameObject.Find ("CenterEyeAnchor").transform;
		
		// we have to open a COM port connection. this code needs to be adjusted manually for every come port. In unity one can have a drop down menu
		sp = new SerialPort ("COM3"
		                     , 9600	
		                     , Parity.None
		                     , 8
		                     , StopBits.One);
		sp.Open ();
		
		// here we start the function changebeltsignal to be executed every 0.1 second ?
		InvokeRepeating("ChangeBeltSignal", 0.1F, 0.1F);
		
	}
	// Update is called once per frame
	void ChangeBeltSignal () {
		iii = PathFinding.pointTo;
		currentX = PathFinding.pathElementsX[iii];
		currentZ = PathFinding.pathElementsZ[iii];
		Vector3 referenceForward = new Vector3(character.transform.forward.x, character.transform.forward.z);
		Vector3 referenceRight = Vector3.Cross (Vector3.forward, referenceForward);
		Vector3 newDirection = new Vector3(currentX, currentZ);
		float angle = Vector3.Angle (newDirection, referenceForward);
		float sign = Mathf.Sign(Vector3.Dot (newDirection,referenceRight));
		//Debug.Log (currentX);
		//Debug.Log (currentZ);
		float finalAngle = sign * angle;
		Debug.Log (finalAngle);
		if (finalAngle < 0) {
						finalAngle = 360 + finalAngle;
				}
		
		//ERROR: from Waypoint 13 to 17 there is an error saying that variable is out of range -> check PathFidning script
		lsb = (byte)(angleBetween % 256); //
		msb = (byte)Math.Floor (angleBetween/ 256); //  

		lsb = (byte)(finalAngle % 256); //
		msb = (byte)Math.Floor (finalAngle / 256); //  

		//Debug.Log (sign);
		//Debug.Log (finalAngle);

		// here the byte variable is filled for the belt, with the direction and the signal to vibrate there	
		sp.Write (new byte[] {0xAA, 0x04, lsb , msb , 0xFF}, 0, 5);
		
	}
	// we say the belt good night, and close the connection.
	void OnApplicationQuit() {
		
		sp.Write (new byte[] {0xAA, 0x01}, 0, 2);
		sp.Close();
	}
	
}