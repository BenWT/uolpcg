using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settlement : MonoBehaviour {

	Generator generator;

	List<Building> buildings = new List<Building>();
	string[] buildingPrefabs = new string[] { "Building1", "Building2", "Building3" };

	public void Init(Generator generator) {
		this.generator = generator;

		// int buildingTarget = generator.GetInt(3, 12);
		int buildingTarget = 12;

		// while (buildings.Count < buildingTarget) {
		// 	for (int i = 0; i < buildingTarget; i++) {
		// 		GameObject b = GameObject.Instantiate(Resources.Load(buildingPrefabs[generator.GetInt(0, buildingPrefabs.Length)])) as GameObject;
		// 		b.transform.parent = transform;
		// 		b.transform.localPosition = new Vector3(generator.GetFloat(-35.0f, 35.0f), 4.0f, generator.GetFloat(-35.0f, 35.0f));
		// 		buildings.Add(b.GetComponent<Building>());
		// 	}
		//
		// 	for (int i = 0; i < buildings.Count; i++) {
		// 		bool foundOverlap = false;
		//
		// 		for (int j = 0; j < buildings.Count; j++) {
		// 			if (i != j) {
		//
		// 				Building b = buildings[i];
		// 				Building other = buildings[j];
		//
		// 				if (b.transform.position.x - (b.width / 2) < other.transform.position.x + (other.width / 2) &&
		// 					b.transform.position.x + (b.width / 2) > other.transform.position.x - (other.width / 2) &&
		// 					b.transform.position.z - (b.length / 2) < other.transform.position.z + (other.length / 2) &&
		// 					b.transform.position.z + (b.length / 2) > other.transform.position.z - (other.length / 2))
		// 				{
		// 					foundOverlap = true;
		// 				}
		// 			}
		// 		}
		//
		// 		if (foundOverlap) {
		// 			// buildings[i].transform.GetChild(0).GetComponent<Renderer>().material.color = Color.red;
		// 			Destory(buidlings[i].gameObject);
		// 		} else {
		// 			buildings[i].transform.GetChild(0).GetComponent<Renderer>().material.color = Color.green;
		// 		}
		// 	}
		// }

		while (buildings.Count < buildingTarget) {
			GameObject b = GameObject.Instantiate(Resources.Load(buildingPrefabs[generator.GetInt(0, buildingPrefabs.Length)])) as GameObject;
			b.transform.parent = transform;
			b.transform.localPosition = new Vector3(generator.GetFloat(-35.0f, 35.0f), 4.0f, generator.GetFloat(-35.0f, 35.0f));

			bool foundOverlap = false;
			for (int i = 0; i < buildings.Count; i++) {
				Building other = buildings[i];

				if (b.transform.position.x - (b.GetComponent<Building>().width / 2) < other.transform.position.x + (other.width / 2) &&
					b.transform.position.x + (b.GetComponent<Building>().width / 2) > other.transform.position.x - (other.width / 2) &&
					b.transform.position.z - (b.GetComponent<Building>().length / 2) < other.transform.position.z + (other.length / 2) &&
					b.transform.position.z + (b.GetComponent<Building>().length / 2) > other.transform.position.z - (other.length / 2))
				{
					foundOverlap = true;
				}
			}

			if (!foundOverlap) {
				buildings.Add(b.GetComponent<Building>());
			} else {
				Destroy(b);
			}
		}
	}
}
