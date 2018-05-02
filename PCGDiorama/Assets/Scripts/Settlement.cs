using System;
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settlement : MonoBehaviour {

	Generator generator;
	public int number;
	public List<Building> buildings = new List<Building>();
	public List<KeyValuePair<int, int>> connections = new List<KeyValuePair<int, int>>();

	public Vector3 left, right, top, bottom, center;
	public float width, height;

	public void Init(Generator generator, int number, GameObject dronePrefab, GameObject wallPrefab, GameObject cornerPrefab, Material connectionMaterial) {
		this.generator = generator;
		this.number = number;

		foreach (Transform t in transform) {
			Destroy(t.gameObject);
		}
		buildings.Clear();
		connections.Clear();

		int targetCount = generator.GetInt(8, 12);

		while (buildings.Count < targetCount) {
			Building building = new Building(
				generator.buildings[generator.GetInt(0, generator.buildings.Count)],
				new Vector3(generator.GetFloat(-35.0f, 35.0f), 4.0f, generator.GetFloat(-35.0f, 35.0f)));

			if (!building.DoesOverlap(buildings)) {
				buildings.Add(building);
			}
		}

		foreach (Building b in buildings) {
			if (b.GetConnectionCount() == 0) {
				float closestConnection = float.MaxValue;
				Building closest = new Building();

				foreach (Building c in buildings) {
					if (b != c) {
						if ((c.GetPosition() - b.GetPosition()).magnitude < closestConnection) {
							closestConnection = (c.GetPosition() - b.GetPosition()).magnitude;
							closest = c;
						}
					}
				}

				if (closest.name != "") {
					connections.Add(new KeyValuePair<int, int>(buildings.IndexOf(b), buildings.IndexOf(closest)));
					b.AddConnection();
					closest.AddConnection();
				}
			}
		}

		// Connections
		foreach (KeyValuePair<int, int> k in connections) {
			Building a = buildings[k.Key];
			Building b = buildings[k.Value];

			float x = Mathf.Abs(b.GetPosition().x - a.GetPosition().x);
			float z = Mathf.Abs(b.GetPosition().z - a.GetPosition().z);

			List<Vector3> points = new List<Vector3>();
			Vector3 start, end, point1, point2;

			if (x > z) {
				start = ((a.GetRight().x < b.GetLeft().x) ? a.GetRight() : a.GetLeft()) + transform.position;
				end = ((a.GetRight().x < b.GetLeft().x) ? b.GetLeft() : b.GetRight()) + transform.position;
				point1 = start + new Vector3((end.x - start.x) / 2, 0.0f, 0.0f);
				point2 = end - new Vector3((end.x - start.x) / 2, 0.0f, 0.0f);
			} else {
				start = ((a.GetTop().z < b.GetBottom().z) ? a.GetTop() : a.GetBottom()) + transform.position;
				end = ((a.GetTop().z < b.GetBottom().z) ? b.GetBottom() : b.GetTop()) + transform.position;
				point1 = start + new Vector3(0.0f, 0.0f, (end.z - start.z) / 2);
				point2 = end - new Vector3(0.0f, 0.0f, (end.z - start.z) / 2);
			}

			points.Add(a.GetPosition());
			points.Add(start);

			float resolution = 0.02f;
			for (int i = 0; i < Mathf.FloorToInt(1f / resolution); i++) {
				float t = i * resolution;
				float oneMinusT = 1f - t;

				Vector3 Q = oneMinusT * start + t * point1;
				Vector3 R = oneMinusT * point1 + t * point2;
				Vector3 S = oneMinusT * point2 + t * end;
				Vector3 P = oneMinusT * Q + t * R;
				Vector3 T = oneMinusT * R + t * S;
				Vector3 newPos = oneMinusT * P + t * T;

				points.Add(newPos);
			}

			points.Add(end);
			points.Add(b.GetPosition());

			List<Vector3> vertices = new List<Vector3>();
			List<int> triangles = new List<int>();

			for (int i = 0; i < points.Count - 1; i++) {
				int startCount = vertices.Count;

				Vector3 A = points[i];
				Vector3 B = points[i + 1];
				Vector3 c = (A + B) / 2;

				Vector3 n = Quaternion.Euler(0.0f, 90.0f, 0.0f) * (B-A).normalized;
				Vector3 d = c + n;
				Vector3 e = c - n;

				vertices.Add(new Vector3(d.x, 4.0f, d.z));
				vertices.Add(new Vector3(e.x, 4.0f, e.z));
				vertices.Add(new Vector3(d.x, 6.592f, d.z));
				vertices.Add(new Vector3(e.x, 6.592f, e.z));

				if (i < points.Count - 2) {
					triangles.Add(startCount + 7);
					triangles.Add(startCount + 3);
					triangles.Add(startCount + 5);

					triangles.Add(startCount + 3);
					triangles.Add(startCount + 1);
					triangles.Add(startCount + 5);

					triangles.Add(startCount + 2);
					triangles.Add(startCount + 6);
					triangles.Add(startCount + 0);

					triangles.Add(startCount + 6);
					triangles.Add(startCount + 4);
					triangles.Add(startCount + 0);

					triangles.Add(startCount + 1);
					triangles.Add(startCount + 0);
					triangles.Add(startCount + 5);

					triangles.Add(startCount + 0);
					triangles.Add(startCount + 4);
					triangles.Add(startCount + 5);

					triangles.Add(startCount + 2);
					triangles.Add(startCount + 3);
					triangles.Add(startCount + 6);

					triangles.Add(startCount + 3);
					triangles.Add(startCount + 7);
					triangles.Add(startCount + 6);
				}

				// GameObject o = GameObject.CreatePrimitive(PrimitiveType.Sphere);
				// o.transform.parent = transform;
				// o.transform.position = c;
				// o.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
				//
				// GameObject o1 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
				// o1.transform.parent = transform;
				// o1.transform.position = d;
				// o1.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
				//
				// GameObject o2 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
				// o2.transform.parent = transform;
				// o2.transform.position = e;
				// o2.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
			}

			GameObject g = new GameObject("wall");
			g.transform.parent = transform;
			Mesh m = new Mesh();
			m.vertices = vertices.ToArray();
			m.triangles = triangles.ToArray();
			m.RecalculateNormals();

			g.AddComponent<MeshFilter>().mesh = m;
	        g.AddComponent<MeshRenderer>();
	        g.AddComponent<MeshCollider>();
			g.GetComponent<MeshRenderer>().material = connectionMaterial;
		}

		foreach (Building b in buildings) {
			GameObject o = GameObject.Instantiate(Resources.Load(b.name)) as GameObject;
			o.transform.parent = transform;
			o.transform.localPosition = b.GetPosition();
		}

		for (int i = 0; i < generator.GetInt(3, buildings.Count); i++) {
			GameObject drone = GameObject.Instantiate(dronePrefab, buildings[i].GetPosition() + transform.position + new Vector3(0.0f, 4.0f, 0.0f), Quaternion.identity);
			drone.transform.parent = transform;
			Drone d = drone.GetComponent<Drone>();
			d.Init(generator, this, buildings[i]);
		}

		float lowestX = float.MaxValue, highestX = -float.MaxValue, lowestZ = float.MaxValue, highestZ = -float.MaxValue;
		float lowestXIndex = -1, highestXIndex = -1, lowestZIndex = -1, highestZIndex = -1;
		for (int i = 0; i < buildings.Count; i++) {
			Building b = buildings[i];

			if (b.GetLeft().x < lowestX) {
				lowestX = b.GetLeft().x;
				lowestXIndex = i;
			}
			if (b.GetRight().x > highestX) {
				highestX = b.GetRight().x;
				highestXIndex = i;
			}

			if (b.GetBottom().z < lowestZ) {
				lowestZ = b.GetBottom().z;
				lowestZIndex = i;
			}
			if (b.GetTop().z > highestZ) {
				highestZ = b.GetTop().z;
				highestZIndex = i;
			}
		}
		lowestX -= 5.0f;
		highestX += 5.0f;
		lowestZ -= 5.0f;
		highestZ += 5.0f;

		GameObject corner1 = GameObject.Instantiate(cornerPrefab);
		corner1.transform.parent = transform;
		corner1.transform.localPosition = new Vector3(lowestX, 2.0f, lowestZ);
		GameObject corner2 = GameObject.Instantiate(cornerPrefab);
		corner2.transform.parent = transform;
		corner2.transform.localPosition = new Vector3(highestX, 2.0f, lowestZ);
		GameObject corner3 = GameObject.Instantiate(cornerPrefab);
		corner3.transform.parent = transform;
		corner3.transform.localPosition = new Vector3(lowestX, 2.0f, highestZ);
		GameObject corner4 = GameObject.Instantiate(cornerPrefab);
		corner4.transform.parent = transform;
		corner4.transform.localPosition = new Vector3(highestX, 2.0f, highestZ);

		GameObject wall1 = GameObject.Instantiate(wallPrefab);
		wall1.transform.parent = transform;
		wall1.transform.localPosition = new Vector3(lowestX, 5.0f, lowestZ + ((highestZ-lowestZ)/2));
		wall1.transform.localScale = new Vector3(1.0f, 1.0f, (highestZ-lowestZ));
		GameObject wall2 = GameObject.Instantiate(wallPrefab);
		wall2.transform.parent = transform;
		wall2.transform.localPosition = new Vector3(highestX, 5.0f, lowestZ + ((highestZ-lowestZ)/2));
		wall2.transform.localScale = new Vector3(1.0f, 1.0f, (highestZ-lowestZ));
		GameObject wall3 = GameObject.Instantiate(wallPrefab);
		wall3.transform.parent = transform;
		wall3.transform.localPosition = new Vector3(lowestX + ((highestX-lowestX)/2), 5.0f, lowestZ);
		wall3.transform.localEulerAngles = new Vector3(0.0f, 90.0f, 0.0f);
		wall3.transform.localScale = new Vector3(1.0f, 1.0f, (highestX-lowestX));
		GameObject wall4 = GameObject.Instantiate(wallPrefab);
		wall4.transform.parent = transform;
		wall4.transform.localPosition = new Vector3(lowestX + ((highestX-lowestX)/2), 5.0f, highestZ);
		wall4.transform.localEulerAngles = new Vector3(0.0f, 90.0f, 0.0f);
		wall4.transform.localScale = new Vector3(1.0f, 1.0f, (highestX-lowestX));

		center = new Vector3(lowestX + ((highestX-lowestX)/2), 0.0f, lowestZ + ((highestZ-lowestZ)/2));
		left = new Vector3(lowestX, 0.0f, lowestZ + ((highestZ-lowestZ)/2));
		right = new Vector3(highestX, 0.0f, lowestZ + ((highestZ-lowestZ)/2));
		bottom = new Vector3(lowestX + ((highestX-lowestZ)/2), 0.0f, lowestZ);
		top = new Vector3(lowestX + ((highestX-lowestZ)/2), 0.0f, highestZ);

		width = highestX - lowestX;
		height = highestZ - lowestZ;
	}
}

[System.Serializable]
public class Building : IComparable<Building> {
	public string name = "";
	public float width;
	public float length;
	Vector3 position;
	int connectionCounter = 0;

	public Building() {}
	public Building(Building b, Vector3 position) {
		this.name = b.name;
		this.width = b.width;
		this.length = b.length;
		this.position = position;
	}

	public Vector3 GetPosition() {
		return position;
	}
	public Vector3 GetTop() {
		return position + new Vector3(0.0f, 0.0f, length / 2);
	}
	public Vector3 GetBottom() {
		return position - new Vector3(0.0f, 0.0f, length / 2);
	}
	public Vector3 GetLeft() {
		return position - new Vector3(width / 2, 0.0f, 0.0f);
	}
	public Vector3 GetRight() {
		return position + new Vector3(width / 2, 0.0f, 0.0f);
	}
	public int GetConnectionCount() {
		return connectionCounter;
	}
	public void AddConnection() {
		connectionCounter++;
	}

	public bool DoesOverlap(List<Building> others) {
		foreach (Building b in others) {
			if (position.x - ((width + 2) / 2) < b.GetPosition().x + ((b.width + 2) / 2) &&
				position.x + ((width + 2) / 2) > b.GetPosition().x - ((b.width + 2) / 2) &&
				position.z - ((length + 2) / 2) < b.GetPosition().z + ((b.length + 2) / 2) &&
				position.z + ((length + 2) / 2) > b.GetPosition().z - ((b.length + 2) / 2))
			{
				return true;
			}
		}
		return false;
	}

    public int CompareTo(Building other) {
		return position.sqrMagnitude.CompareTo(other.GetPosition().sqrMagnitude);
    }
}
