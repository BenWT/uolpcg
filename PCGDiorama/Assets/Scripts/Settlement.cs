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

		for (int i = 0; i < buildingTarget; i++) {
			GameObject b = GameObject.Instantiate(Resources.Load(buildingPrefabs[generator.GetInt(0, buildingPrefabs.Length)])) as GameObject;
			b.transform.parent = transform;
			b.transform.localPosition = new Vector3(generator.GetFloat(-35.0f, 35.0f), 4.0f, generator.GetFloat(-35.0f, 35.0f));
			buildings.Add(b.GetComponent<Building>());
		}

		// foreach (Building b in buildings) {
		// 	foreach (Building other in buildings) {
		// 		if (b != other) {
		// 			// if (RectA.Left < RectB.Right &&
		// 			// 	RectA.Right > RectB.Left &&
		// 			// 	RectA.Top > RectB.Bottom &&
		// 			// 	RectA.Bottom < RectB.Top )
		//
		//
		// 			if (b.transform.position.x - b.width < other.transform.position.x + other.width &&
		// 				b.transform.position.x + b.width > other.transform.position.x - other.width &&
		// 				b.transform.position.z - b.length < other.transform.position.z + other.length &&
		// 				b.transform.position.z + b.length > other.transform.position.z - other.length)
		// 			{
		// 				// b.transform.GetChild(0).GetComponent<Renderer>().material.color = Color.red;
		// 				other.transform.GetChild(0).GetComponent<Renderer>().material.color = Color.red;
		// 			}
		// 		}
		// 	}
		// }

		for (int i = 0; i < buildings.Count; i++) {

			bool foundOverlap = false;

			for (int j = 0; j < buildings.Count; j++) {
				if (i != j) {

					Building b = buildings[i];
					Building other = buildings[j];

					if (b.transform.position.x - (b.width / 2) < other.transform.position.x + (other.width / 2) &&
						b.transform.position.x + (b.width / 2) > other.transform.position.x - (other.width / 2) &&
						b.transform.position.z - (b.length / 2) < other.transform.position.z + (other.length / 2) &&
						b.transform.position.z + (b.length / 2) > other.transform.position.z - (other.length / 2))
					{
						foundOverlap = true;
					}
				}
			}

			if (foundOverlap) {
				buildings[i].transform.GetChild(0).GetComponent<Renderer>().material.color = Color.red;
			} else {
				buildings[i].transform.GetChild(0).GetComponent<Renderer>().material.color = Color.green;
			}
		}
	}
}
