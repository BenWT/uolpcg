using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour {

	int seed;

	public GameObject groundPlane;
	public GameObject dronePrefab;
	public GameObject wallPrefab;
	public GameObject rockPrefab;
	public GameObject cornerPrefab;
	public Material connectionMaterial;
	int terrainSize = 15;

	public List<Building> buildings = new List<Building>();
	public List<Settlement> settlements = new List<Settlement>();

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
		settlements.Clear();

		// Create Terrain
		GameObject terrainGO = GameObject.Instantiate(groundPlane, Vector3.zero, Quaternion.identity);
		terrainGO.transform.parent = transform;
		terrainGO.transform.localPosition = Vector3.zero;

		MeshFilter mF = terrainGO.GetComponent<MeshFilter>();

		Vector3[] verts = mF.mesh.vertices;

		float scale = 0.05f;

		for (int k = 0; k < verts.Length; k++) {
			float pX = (verts[k].x * scale) + this.seed;
			float pY = (verts[k].y * scale) + this.seed;

			verts[k].z = Mathf.PerlinNoise(pX, pY) * 4.0f;
		}

		mF.mesh.vertices = verts;
		mF.mesh.RecalculateNormals();
		mF.mesh.RecalculateBounds();

		terrainGO.transform.localRotation = Quaternion.Euler(-90.0f, 0.0f, 0.0f);

		// Create Rocky outcrops
		for (int i = 0; i < GetInt(10, 20); i++) {
			GameObject outcrop = new GameObject("Outcrop");
			outcrop.transform.parent = transform;
			outcrop.transform.localPosition = new Vector3(GetFloat(15.0f, 200.0f), 1.0f, GetFloat(15.0f, 200.0f));

			for (int j = 0; j < GetInt(2, 5); j++) {
				GameObject rock = GameObject.Instantiate(rockPrefab);
				rock.transform.parent = outcrop.transform;
				rock.transform.localPosition = new Vector3(GetFloat(-5.0f, 5.0f), 0.0f, GetFloat(-5.0f, 5.0f));
				rock.transform.localEulerAngles = new Vector3(GetFloat(0.0f, 360.0f), GetFloat(0.0f, 360.0f), GetFloat(0.0f, 360.0f));
			}
		}

		// Create Settlements
		GameObject settlementGO1 = new GameObject("Settlement");
		settlementGO1.transform.parent = transform;
		settlementGO1.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
		Settlement settlement1 = settlementGO1.AddComponent<Settlement>();
		settlements.Add(settlement1);
		settlement1.Init(this, 0, dronePrefab, wallPrefab, cornerPrefab, connectionMaterial);

		float angle = GetFloat(0.0f, 90.0f) * Mathf.Deg2Rad;

		GameObject settlementGO2 = new GameObject("Settlement");
		settlementGO2.transform.parent = transform;
		Settlement settlement2 = settlementGO2.AddComponent<Settlement>();
		settlements.Add(settlement2);
		settlement2.Init(this, 1, dronePrefab, wallPrefab, cornerPrefab, connectionMaterial);

		settlementGO2.transform.localPosition = new Vector3(Mathf.Sin(angle) * ((settlement1.width / 2) + (settlement2.width / 2) + GetFloat(20.0f, 75.0f)), 0.0f, Mathf.Cos(angle) * ((settlement1.height / 2) + (settlement2.height / 2) + GetFloat(20.0f, 75.0f)));
	}

	public float GetFloat(float min, float max) {
		return Random.Range(min, max);
	}
	public int GetInt(int min, int max) {
		return Random.Range(min, max);
	}
	public bool PercentChance(int chance) {
		return (GetInt(0, 100) <= chance);
	}

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
