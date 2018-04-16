using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour {

	public GameObject groundPlane;

	int seed;
	Settlement settlement;

	int terrainSize = 15;

	void Start() {
		this.seed = Random.Range(0, 10000);
		Random.InitState(this.seed);
		DoGenerate();
	}

	public void DoGenerate() {

		// Cleanup previous generation
		foreach (Transform t in transform) {
			Destroy(t.gameObject);
		}

		// Create Terrain
		// GameObject terrainGO = GameObject.CreatePrimitive(PrimitiveType.Plane); // TODO replace with instantiate
		GameObject terrainGO = GameObject.Instantiate(groundPlane, Vector3.zero, Quaternion.identity);
		terrainGO.transform.parent = transform;
		terrainGO.transform.localPosition = Vector3.zero;

		MeshFilter mF = terrainGO.GetComponent<MeshFilter>();

		Vector3[] verts = mF.mesh.vertices;

		float scale = 0.5f;

		for (int k = 0; k < verts.Length; k++) {
			float pX = (verts[k].x * scale) + this.seed;
			float pY = (verts[k].y * scale) + this.seed;

			verts[k].z = Mathf.PerlinNoise(pX, pY) * 4.0f;
		}

		mF.mesh.vertices = verts;
		mF.mesh.RecalculateNormals();
		mF.mesh.RecalculateBounds();

		terrainGO.transform.localRotation = Quaternion.Euler(-90.0f, 0.0f, 0.0f);

		// Create Settlement
		GameObject settlementGO = new GameObject("Settlement");
		settlementGO.transform.parent = transform;
		settlementGO.transform.localPosition = Vector3.zero;
		Settlement settlement = settlementGO.AddComponent<Settlement>();
		settlement.Init(this);

		GetFloat(0.0f, 1.0f);
	}

	public float GetFloat(float min, float max) {
		return Random.Range(min, max);
	}

	public int GetInt(int min, int max) {
		return Random.Range(min, max);
	}

	// TODO: remove for production
	void OnGUI() {
		if (GUILayout.Button("Generate")) {
			this.seed = Random.Range(0, 10000);
			Random.InitState(this.seed);
			DoGenerate();
		}
		if (GUILayout.Button("Fixed Generate")) {
			this.seed = 100;
			Random.InitState(this.seed);
			DoGenerate();
		}
	}
}
