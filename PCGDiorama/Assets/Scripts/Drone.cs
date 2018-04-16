using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone : MonoBehaviour {

	Generator generator;

	List<Transform> bodyPieces = new List<Transform>();
	List<Transform> wingPieces = new List<Transform>();

	public void Init(Generator generator) {
		this.generator = generator;

		TogglePieces(bodyPieces, generator.GetInt(0, bodyPieces.Count));
		TogglePieces(wingPieces, generator.GetInt(0, wingPieces.Count));
	}

	void TogglePieces(List<Transform> list, int active) {
		foreach (Transform t in list) {
			t.gameObject.SetActive(false);
		}

		list[active].gameObject.SetActive(true);
	}
}
