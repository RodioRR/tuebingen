using UnityEngine;
using System.Collections;

public class ChangeWaypoint : MonoBehaviour {
	private int i = 0;
	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Cube") {
			i += 1;
			Destroy (other.gameObject);
		}
	}

}
