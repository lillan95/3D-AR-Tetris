using UnityEngine;
using System.Collections;

public class Group : MonoBehaviour {
	float lastFall = 0;
	// Use this for initialization
	void Start () {
		
		// Default position not valid? Then it's game over
		if (!isValidGridPos()) {
			//Debug.Log("GAME OVER");
			Destroy(gameObject);
		}
	}

	void Update() {
		// Move Left
		if (Input.GetKeyDown(KeyCode.LeftArrow)) {
			transform.position += new Vector3(-1, 0, 0);

			if (isValidGridPos())
				updateGrid();
			
			else
				transform.position += new Vector3(1, 0, 0);
		}

		else if (Input.GetKeyDown(KeyCode.RightArrow)) {

			transform.position += new Vector3(1, 0, 0);

			if (isValidGridPos())
				updateGrid();
			
			else
				transform.position += new Vector3(-1, 0, 0);
		}

		// Move Front
		else if (Input.GetKeyDown(KeyCode.UpArrow)) {
			
			transform.position += new Vector3(0, 0, -1);

			if (isValidGridPos ()) {
				updateGrid ();
			} else {
				transform.position += new Vector3 (0, 0, 1);
			}
		}



		// Move Back
		else if (Input.GetKeyDown(KeyCode.DownArrow)) {
			
			transform.position += new Vector3(0, 0, 1);

			if (isValidGridPos()){
				updateGrid();}
			
			else{
				transform.position += new Vector3(0, 0, -1);
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

		// Move down and check if row is full
		else if (Input.GetKeyDown(KeyCode.R) ||
			Time.time - lastFall >= 1) {
			transform.position += new Vector3(0, -1, 0);


			if (isValidGridPos()) {
				updateGrid();
			} else {
				transform.position += new Vector3(0, 1, 0);
				Grid.deleteFullRows();

				FindObjectOfType<Spawner>().spawnNewBlock();

				enabled = false;
			}

			lastFall = Time.time;
		}
	}
		

	bool isValidGridPos() {     
		foreach (Transform child in transform) {
			Vector3 v = Grid.roundVec3 (child.position);

			if (!Grid.insideBorder (v)) {
				return false;
			}
				
			if (Grid.grid[(int)v.x, (int)v.z, (int)v.y] != null &&
				Grid.grid[(int)v.x, (int)v.z, (int)v.y].parent != transform)
				return false;
		}
		return true;
	}

	void updateGrid() {
		for (int y = 0; y < Grid.h; ++y)
			for (int x = 0; x < Grid.w; ++x)
				for (int z = 0; z < Grid.d; ++z) {
					if (Grid.grid [x, z, y] != null)
					if (Grid.grid [x, z, y].parent == transform)
						Grid.grid [x, z, y] = null;
				}
					
		foreach (Transform child in transform) {
			Vector3 v = Grid.roundVec3(child.position);
			Grid.grid[(int)v.x, (int)v.z, (int)v.y] = child;
		}        
	}

}