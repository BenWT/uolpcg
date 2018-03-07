using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour {

	int seed;
	Settlement settlement;

	public void Generate() {
		this.seed = Random.seed;
		DoGenerate();
	}
	public void Generate(int seed) {
		this.seed = seed;
		Random.InitState(this.seed);
		DoGenerate();
	}

	public void DoGenerate() {
		// TODO: Generate Terrain with perlin noise
		// Make sure perlin noise is seeded, use the same seed as random

		// Create Settlement
		GameObject sGO = new GameObject("Settlement");
		sGO.transform.parent = transform;
		sGO.transform.localPosition = Vector3.zero;
		Settlement s = sGO.AddComponent<Settlement>();
		s.Init(this);
	}

	public float GetFloat(float min, float max) {
		// return ((float)random.NextDouble() * (max - min)) + min;

		return Random.Range(min, max);
	}

	public int GetInt(int min, int max) {
		// return (random.Next() * (max - min)) + min;

		return Random.Range(min, max);
	}

	void OnGUI() {
		if (GUILayout.Button("Generate")) {
			Generate();
		}
		if (GUILayout.Button("Generate from Seed 100")) {
			Generate(100);
		}
	}
}
