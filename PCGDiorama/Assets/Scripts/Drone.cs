using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone : MonoBehaviour {

	Generator generator;
	Settlement settlement;
	Building building;

	public List<Transform> bodyPieces = new List<Transform>();
	public List<Transform> wingPieces = new List<Transform>();
	public List<Transform> topPieces = new List<Transform>();

	DroneJob currentJob;
	DroneJob previousJob;

	Settlement targetS;
	Building targetB;
	float offsetX = 0.0f, offsetZ = 0.0f;
	float targetOffsetX = 0.0f, targetOffsetZ = 0.0f;
	float timer = 0.0f;
	float timeToWait = 2.0f;
	float speed = 5.0f;

	public void Init(Generator generator, Settlement settlement, Building building) {
		this.generator = generator;
		this.settlement = settlement;
		this.building = building;

		if (generator.PercentChance(50)) currentJob = DroneJob.Move;
		else currentJob = DroneJob.Waiting;
		StartJob();

		TogglePieces(bodyPieces, generator.GetInt(0, bodyPieces.Count));
		TogglePieces(wingPieces, generator.GetInt(0, wingPieces.Count));
		TogglePieces(topPieces, generator.GetInt(0, topPieces.Count));
		// TODO: proc gen speed
	}

	void TogglePieces(List<Transform> list, int active) {
		foreach (Transform t in list) t.gameObject.SetActive(false);
		list[active].gameObject.SetActive(true);
	}

	void StartJob() {
		timer = 0.0f;

		if (currentJob == DroneJob.Waiting) {
			timeToWait = generator.GetFloat(2.0f, 8.0f);
		} else if (currentJob == DroneJob.Commute || currentJob == DroneJob.Move) {
			targetS = (currentJob == DroneJob.Commute) ? generator.settlements[(settlement.number == 1) ? 0 : 1] : settlement;
			targetB = targetS.buildings[generator.GetInt(0, targetS.buildings.Count)];

			if (targetB == building) {
				ChangeJob();
			} else {
				targetOffsetX = generator.GetFloat(-((targetB.width / 2) - 1), (targetB.width / 2) - 1);
				targetOffsetZ = generator.GetFloat(-((targetB.length / 2) - 1), (targetB.length / 2) - 1);
			}
		} else if (currentJob == DroneJob.Explore) {
			ChangeJob(); // TODO: solve
		}
	}
	void Update() {
		timer += Time.deltaTime;

		if (currentJob == DroneJob.Waiting) {
			if (timer >= timeToWait) ChangeJob();
		} else if (currentJob == DroneJob.Commute || currentJob == DroneJob.Move) {
			Vector3 start = (building.GetPosition() + settlement.transform.position) + new Vector3(offsetX, 4.0f, offsetZ);
			Vector3 end = (targetB.GetPosition() + targetS.transform.position) + new Vector3(targetOffsetX, 4.0f, targetOffsetZ);

			float distance = 2 * (Mathf.Sqrt(Mathf.Pow(((end - start).magnitude / 2), 2) + 100.0f));

			if (timer >= (distance / speed)) {
				settlement = targetS;
				building = targetB;
				offsetX = targetOffsetX;
				offsetZ = targetOffsetZ;
				ChangeJob();
			} else {
				transform.position = Vector3.Lerp(start, end, timer / (distance / speed)) + new Vector3(0.0f, 10.0f * Mathf.Sin(Mathf.PI * (timer / (distance / speed))), 0.0f);
				transform.LookAt(new Vector3(end.x, transform.position.y, end.z));
			}
		} else if (currentJob == DroneJob.Explore) {
			// TODO: solve
		}
	}
	void ChangeJob() {
		previousJob = currentJob;

		if (previousJob != DroneJob.Waiting) {
			currentJob = DroneJob.Waiting;
		} else {
			if (generator.PercentChance(50)) {
				currentJob = DroneJob.Move;
			} else {
				if (generator.PercentChance(75)) {
					currentJob = DroneJob.Commute;
				} else {
					currentJob = DroneJob.Explore;
				}
			}
		}

		StartJob();
	}
}

[System.Serializable]
public enum DroneJob {
	Waiting,
	Commute,
	Move,
	Explore
}
