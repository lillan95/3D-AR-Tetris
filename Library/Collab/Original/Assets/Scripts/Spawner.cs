using UnityEngine;
using System.Collections;
using Vuforia;

public class Spawner : MonoBehaviour {

	// Groups
	public GameObject[] groups;
	public GameObject Origin;


	public void spawnNext() {
		// Random Index
		int i = Random.Range(0, groups.Length);

		// Spawn Group at current Position
		var block = Instantiate(groups[i],
					transform.position,
				  transform.rotation);
		block.transform.parent = Origin.transform;
	}

	void Start() {
		// Spawn initial Group
		spawnNext();
	}

	private bool isTrackingMarker()
    {
        var imageTarget = GameObject.Find("ImageTarget");
        var trackable = imageTarget.GetComponent<TrackableBehaviour>();
        var status = trackable.CurrentStatus;
        return status == TrackableBehaviour.Status.TRACKED;
    }


}
