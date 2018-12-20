using UnityEngine;
using System.Collections;

public class Grid : MonoBehaviour {
	// The Grid itself
	public static int w = 10;
	public static int h = 20;
	public static int d = 10;
	public static Transform[,,] grid = new Transform[w, d, h];

	public static Vector3 roundVec3(Vector3 v) {
		return new Vector3(Mathf.Round(v.x), Mathf.Round(v.y), Mathf.Round(v.z));
	}

	public static bool insideBorder(Vector3 pos) {
		//Debug.Log ("\t\t\t\t\t\tX:" + pos.x + " Z: " + pos.z + " Y: " + pos.y);
		return ((int)pos.x >= 0 && (int)pos.x < w && (int)pos.z < d && (int)pos.z >= 0 && (int)pos.y >= 0);
	}

	public static void deleteRow(int y, int z) {
		//Debug.Log ("Deleting row ");
		for (int x = 0; x < w; ++x) {
			Destroy (grid [x, z, y].gameObject);
			grid [x, z, y] = null;
		}
	}

	public static void decreaseRow(int y, int z) {  // ONLY 2D SO FAR
		//Debug.Log ("Decrease row");
		for (int x = 0; x < w; ++x) {
			if (grid[x, z, y] != null) {
				// Move one towards bottom
				grid[x, z, y-1] = grid[x, z, y];
				grid[x, z, y] = null;

				// Update Block localPosition
				grid[x, z, y-1].localPosition += new Vector3(0, -1, 0);
			}
		}
	}

	public static void decreaseRowsAbove(int y , int z) { // ONLY 2D SO FAR
		for (int i = y; i < h; ++i)
			decreaseRow(i, z);
	}

	public static bool fullRow(int y, int z) {
		for (int x = 0; x < w; ++x){

			if (grid [x, z, y] == null)
				return false;
		}
		return true;
	}

	public static bool fullRowY(int y, int x) {
		for (int z = 0; z < d; ++z){

			if (grid [x, z, y] == null)
				return false;
		}
		return true;
	}

	public static void deleteRowY (int y, int x) {
		//Debug.Log ("Deleting row ");
		for (int z = 0; z < d; ++z) {
			Destroy (grid [x, z, y].gameObject);
			grid [x, z, y] = null;
		}
	}

	public static void decreaseRowsAboveY(int y , int x) {
		for (int i = y; i < h; ++i)
			decreaseRowY(i, x);
	}

	public static void decreaseRowY(int y, int x) {
		for (int z = 0; z < d; ++z) {
			if (grid[x, z, y] != null) {
				// Move one towards bottom
				grid[x, z, y-1] = grid[x, z, y];
				grid[x, z, y] = null;

				// Update Block localPosition
				grid[x, z, y-1].localPosition += new Vector3(0, -1, 0);
			}
		}
	}


	public static void deleteFullRows() {
		//Debug.Log("DELETE FULL ROWS" );
		for (int x = 0; x < d; ++x) {
			for (int z = 0; z < d; ++z) {
				for (int y = 0; y < h; ++y) {
					if (fullRow (y, z)) {
						deleteRow (y, z);
						decreaseRowsAbove (y + 1, z);
						--y;
					}

					if (fullRowY (y, x)){
						//Debug.Log ("fullrow");
						deleteRowY (y, x);
						decreaseRowsAboveY (y + 1, z);
						--y;
					}
				}
			}
		}
	}


}
