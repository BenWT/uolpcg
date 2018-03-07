using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settlement : MonoBehaviour {

	Generator generator;

	int randomInt;
	float randomFloat;

	public void Init(Generator generator) {
		this.generator = generator;

		this.randomInt = generator.GetInt(0, 12);
		this.randomFloat = generator.GetFloat(0.0f, 12.0f);

		Debug.Log(randomInt);
		Debug.Log(randomFloat);

		GenerateBuilding();
	}

	public void GenerateBuilding() {
		GameObject bGO = new GameObject("Building");
		bGO.transform.parent = transform;
		bGO.transform.localPosition = Vector3.zero;
		Building b = bGO.AddComponent<Building>();
		b.Init(this.generator);
	}
}
