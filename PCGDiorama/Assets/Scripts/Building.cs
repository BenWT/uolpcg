using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour {

	public float width;
	public float length;

	public bool left = false, right = false, up = false, down = false;

	public bool ClearConnection() {
		if (!left || !right || !up || !down) return true;
		else return false;
	}
}
