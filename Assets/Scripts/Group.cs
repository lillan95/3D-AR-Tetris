using UnityEngine;
using System.Collections;
using Vuforia;

public class Group : MonoBehaviour {
	float lastFall = 0;
	public GameObject origin;
	public GameObject imageTarget;

	// Use this for initializations
	void Start () {

		 origin =  GameObject.Find("/ImageTarget/Plane/Origin");
		 imageTarget = GameObject.Find("/ImageTarget");

		// Default localPosition not valid? Then it's game over
	if (!isValidGridPos()) {
			//Debug.Log("GAME OVER");
			Destroy(gameObject);
		}
	}

	string getOrientation() {
		Vector3 targetRotation = imageTarget.transform.eulerAngles;
			if(targetRotation.y > 45 && targetRotation.y < 135) return "Right";
			if(targetRotation.y > 315 || targetRotation.y < 45) return "Front";
			if(targetRotation.y > 225 && targetRotation.y < 315) return "Left";
			if(targetRotation.y > 135 && targetRotation.y < 225) return "Back";
			else return "Error";
	}

	void Update() {

		if(isTrackingMarker()){
			// Move Left
			if (Input.GetKeyDown(KeyCode.LeftArrow)) {
				transform.localPosition += new Vector3(-1, 0, 0);

				if (isValidGridPos())
					updateGrid();

				else
					transform.localPosition += new Vector3(1, 0, 0);
			}

			else if (Input.GetKeyDown(KeyCode.RightArrow)) {

				transform.localPosition += new Vector3(1, 0, 0);

				if (isValidGridPos())
					updateGrid();

				else
					transform.localPosition += new Vector3(-1, 0, 0);
			}

			// Move Front
			else if (Input.GetKeyDown(KeyCode.UpArrow)) {

				transform.localPosition += new Vector3(0, 0, -1);

				if (isValidGridPos ()) {
					updateGrid ();
				} else {
					transform.localPosition += new Vector3 (0, 0, 1);
				}
			}



			// Move Back
			else if (Input.GetKeyDown(KeyCode.DownArrow)) {

				transform.localPosition += new Vector3(0, 0, 1);

				if (isValidGridPos()){
					updateGrid();}

				else{
					transform.localPosition += new Vector3(0, 0, -1);
				}
			}

			// Rotate
			else if (Input.GetKeyDown(KeyCode.Space)) {
				transform.Rotate(0, 0, -90);

				if (isValidGridPos())
					updateGrid();

				else
					transform.Rotate(0, 0, 90);
			}

			// Move Downwards and Fall
			else if (Input.GetKeyDown(KeyCode.R) ||
				Time.time - lastFall >= 1) {
				transform.localPosition += new Vector3(0, -1, 0);

				if (isValidGridPos()) {
					updateGrid();
				} else {
					transform.localPosition += new Vector3(0, 1, 0);
					//Debug.Log ("Delete row initiate! ");
					// Clear filled horizontal lines
					Grid.deleteFullRows();

					// Spawn next Group
					FindObjectOfType<Spawner>().spawnNewBlock();

					// Disable script
					enabled = false;
				}

				lastFall = Time.time;
			}
		}
	}

	bool isValidGridPos() {        // 2D So far
		foreach (Transform child in transform) {
			//Vector3 v_transform = Grid.roundVec3 (transform.localPosition);
			Vector3 v_origin = origin.transform.position;
			Vector3 v = child.position;
			Vector3 v_adjusted = Grid.roundVec3(origin.transform.InverseTransformPoint(v));

			if (!Grid.insideBorder (v_adjusted)) {
				return false;
			}

			// Block in grid cell (and not part of same group)?
			if (Grid.grid[(int)v_adjusted.x, (int)v_adjusted.z, (int)v_adjusted.y] != null &&
				Grid.grid[(int)v_adjusted.x, (int)v_adjusted.z, (int)v_adjusted.y].parent != transform)
				return false;
		}
		return true;
	}

	private bool isTrackingMarker()
		{
				var imageTarget = GameObject.Find("ImageTarget");
				var trackable = imageTarget.GetComponent<TrackableBehaviour>();
				var status = trackable.CurrentStatus;
				return status == TrackableBehaviour.Status.TRACKED;
		}

	void updateGrid() {
		// Remove old children from grid
		for (int y = 0; y < Grid.h; ++y)
			for (int x = 0; x < Grid.w; ++x)
				for (int z = 0; z < Grid.d; ++z) {
					if (Grid.grid [x, z, y] != null)
					if (Grid.grid [x, z, y].parent == transform)
						Grid.grid [x, z, y] = null;
				}

		// Add new children to grid
		foreach (Transform child in transform) {
			//Vector3 v_transform = Grid.roundVec3 (transform.localPosition);
			Vector3 v_origin = origin.transform.position;
			Vector3 v = child.position;
			Vector3 v_adjusted = Grid.roundVec3(origin.transform.InverseTransformPoint(v));

			//Vector3 v = Grid.roundVec3(child.localPosition);
			Grid.grid[(int)v_adjusted.x, (int)v_adjusted.z, (int)v_adjusted.y] = child;
		}
	}

}
