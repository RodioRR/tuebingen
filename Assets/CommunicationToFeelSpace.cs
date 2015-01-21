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
	
	// Use this for initialization
	void Start () {
		
		// here we get the onject, that moves with the player, or the player, and than its transform optioons, that are his direction
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
		
		// here the calculation for the belt is happening. we need y and x coordinates
		// they are in lsb and msb, a byte like way to whrite numbers
		lsb = (byte)(character.transform.eulerAngles.y % 256);
		msb = (byte)Math.Floor (character.transform.eulerAngles.y / 256);
		// here the byte variable is filled for the belt, with the direction and the signal to vibrate there	
		sp.Write (new byte[] {0xAA, 0x04, lsb , msb , 0xFF}, 0, 5);
		
	}
	// we say the belt good night, and close the connection.
	void OnApplicationQuit() {
		
		sp.Write (new byte[] {0xAA, 0x01}, 0, 2);
		sp.Close();
	}
	
}