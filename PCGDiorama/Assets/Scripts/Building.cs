using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour {

	Generator generator;

	int randomInt;
	float randomFloat;

	public void Init(Generator generator) {
		this.generator = generator;

		this.randomInt = generator.GetInt(0, 12);
		this.randomFloat = generator.GetFloat(0.0f, 12.0f);

		Debug.Log(randomInt);
		Debug.Log(randomFloat);
	}
}
