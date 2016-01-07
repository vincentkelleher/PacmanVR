﻿using UnityEngine;
using System.Collections;

public abstract class GhostController : PacmanCharacterController {
	
	public Vector3 target;
	public Vector3 scatterCorner;
	public Vector3 moveDirection;

	public int scoreBeforeStart = 0;
	public int speed = 1;
	
	private Vector3[] offsetVectors = new Vector3[] { new Vector3(1, 0, 0), new Vector3(0, 0, -1), new Vector3(-1, 0, 0), new Vector3(0, 0, 1) };
	public float intersectionOffset = 1.5f;

	private CharacterController characterController;

	protected GameObject pacman;
	protected GameObject previousIntersection;

	// Use this for initialization
	void Start () {
		pacman = GameObject.Find("Pacman");
		characterController = GetComponent<CharacterController> ();

		target = new Vector3();
		transform.localPosition = startPosition;
		moveDirection = Vector3.forward;

		doOnStart ();
	}
	
	// Update is called once per frame
	void Update () {
		if (GameController.score >= scoreBeforeStart) {
			characterController.Move(moveDirection * speed * Time.deltaTime);
		}

		doOnUpdate ();
	}

	abstract protected void doOnStart();
	abstract protected void doOnUpdate();

	void OnTriggerEnter(Collider collider) {
		if (collider.transform.parent != null && collider.transform.parent.name == "Intersections") {
			IntersectionController intersection = collider.GetComponent<IntersectionController> ();
			moveDirection = NextDirection (intersection);
			previousIntersection = collider.gameObject;
		}
	}

	protected Vector3 NextDirection(IntersectionController intersection) {
		float minDistance = float.MaxValue;
		GameObject nextGameObject = intersection.gameObject;

		GameObject[] intersections = intersection.IntersectionList ();
		System.Array.Reverse (intersections);
		for (int intersectionIndex = 0; intersectionIndex < intersections.Length; intersectionIndex++) {
			GameObject childIntersection = intersections [intersectionIndex];

			if (childIntersection != null && childIntersection != previousIntersection) {
				float childIntersectionDistance = Vector3.Distance (intersection.transform.position + offsetVectors [intersectionIndex] * intersectionOffset, target);
				if (childIntersectionDistance < minDistance) {
					minDistance = childIntersectionDistance;
					nextGameObject = childIntersection;
				}
			}
		}

		return Vector3.Normalize(nextGameObject.transform.position - transform.position);
	}

	public new void OnPacmanDeath() {
		enabled = false;

		base.OnPacmanDeath ();

		enabled = true;
	}
}
