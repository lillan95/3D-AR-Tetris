using UnityEngine;
using System.Collections;
using Vuforia;
using IBM.Watson.DeveloperCloud.Logging;
using IBM.Watson.DeveloperCloud.Services.SpeechToText.v1;
using IBM.Watson.DeveloperCloud.Utilities;
using IBM.Watson.DeveloperCloud.DataTypes;
using System.Collections.Generic;
using UnityEngine.UI;

public class Group : MonoBehaviour {
	float lastFall = 0;
	public GameObject origin;
	public GameObject imageTarget;
    private Quaternion localRotation; //
    public float speed = 1.0f; // ajustable speed from Inspector in Unity editor


	//Credentials for voice recognition service
	private string apiKey = "h87ZRSCl_NqRNLDJ2aVfNA2UYnvPL7EGwhCgXr-l9Q_w";
	private string serviceURL = "https://stream.watsonplatform.net/speech-to-text/api";
	private string iamURL = "";

	//Variables for voice recognition
	private int _recordingRoutine = 0;
	private string _microphoneID = null;
	private AudioClip _recording = null;
	private int _recordingBufferSize = 1;
	private int _recordingHZ = 22050;
	private SpeechToText _service;
	private string transcript;

    // Use this for initializations
    void Start () {

			Spawner.blockCount++;

		 origin =  GameObject.Find("/ImageTarget/Plane/Origin");
		 imageTarget = GameObject.Find("/ImageTarget");
         // copy the rotation of the object itself into a buffer
         localRotation = transform.rotation;
        // Default localPosition not valid? Then it's game over
        if (!isValidGridPos()) {
			//Debug.Log("GAME OVER");
			Destroy(gameObject);
		}

		LogSystem.InstallDefaultReactors();
		Runnable.Run(CreateService());
	}

	string getOrientation() {
		Vector3 targetRotation = imageTarget.transform.eulerAngles;
			if(targetRotation.y > 45 && targetRotation.y < 135) return "Right";
			if(targetRotation.y > 315 || targetRotation.y < 45) return "Front";
			if(targetRotation.y > 225 && targetRotation.y < 315) return "Left";
			if(targetRotation.y > 135 && targetRotation.y < 225) return "Back";
			else return "Error";
	}

    string getMoveDirection()
    {
        //Renderer rend = origin.GetComponent<Renderer>();
        //Create a new Material
        //Material material = new Material(Shader.Find("Standard"));
        string direction = "";
        if (Input.touchCount > 0)
        {
            switch (Input.GetTouch(0).phase)
            {
                case TouchPhase.Began:
                case TouchPhase.Moved:
                    //material.color = Color.green;

                    if (Input.acceleration.x < -0.9)
                    {
                        direction = "Forward";
                        //localRotation.x = -1;
                    }
                    else if (Input.acceleration.x > 0.9)
                    {
                        direction = "Backward";
                        //localRotation.x = 1;
                    }

                    if (Input.acceleration.z < -0.9)
                    {
                        direction = "Left";
                        //localRotation.z = -1;
                    }
                    else if(Input.acceleration.z > 0.9)
                    {
                        direction = "Right";
                        //localRotation.z = 1;
                    }

                    break;
                case TouchPhase.Ended:
                    //transform.rotation = localRotation;
                    //material.color = Color.white;
                    break;
            }
            //Switch to new material
            //rend.material = material;
        }
        return direction;
    }

		void controlArrows() {
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

			transform.localPosition += new Vector3(0, 0, 1);

			if (isValidGridPos ()) {
				updateGrid ();
			} else {
				transform.localPosition += new Vector3 (0, 0, -1);
			}
		}



		// Move Back
		else if (Input.GetKeyDown(KeyCode.DownArrow)) {

			transform.localPosition += new Vector3(0, 0, -1);

			if (isValidGridPos()){
				updateGrid();}

			else{
				transform.localPosition += new Vector3(0, 0, 1);
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
			Time.time - lastFall >= 2.0/Spawner.blockCount) {
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

void rightOrientationGesture(string direction){
	// Move Forward
	if (direction == "Left") {
		transform.localPosition += new Vector3(-1, 0, 0);

		if (isValidGridPos())
			updateGrid();

		else
			transform.localPosition += new Vector3(1, 0, 0);
	}

	else if (direction == "Right") {

		transform.localPosition += new Vector3(1, 0, 0);

		if (isValidGridPos())
			updateGrid();

		else
			transform.localPosition += new Vector3(-1, 0, 0);
	}

	// Move Left
	else if (direction == "Backward") {

		transform.localPosition += new Vector3(0, 0, 1);

		if (isValidGridPos ()) {
			updateGrid ();
		} else {
			transform.localPosition += new Vector3 (0, 0, -1);
		}
	}

	// Move Right
	else if (direction == "Forward") {

		transform.localPosition += new Vector3(0, 0, -1);

		if (isValidGridPos()){
			updateGrid();}

		else{
			transform.localPosition += new Vector3(0, 0, 1);
		}
	}
}

void frontOrientationGesture(string direction){
	// Move Forward
	if (direction == "Forward") {
		transform.localPosition += new Vector3(-1, 0, 0);

		if (isValidGridPos())
			updateGrid();

		else
			transform.localPosition += new Vector3(1, 0, 0);
	}

	else if (direction == "Backward") {

		transform.localPosition += new Vector3(1, 0, 0);

		if (isValidGridPos())
			updateGrid();

		else
			transform.localPosition += new Vector3(-1, 0, 0);
	}

	// Move Left
	else if (direction == "Left") {

		transform.localPosition += new Vector3(0, 0, 1);

		if (isValidGridPos ()) {
			updateGrid ();
		} else {
			transform.localPosition += new Vector3 (0, 0, -1);
		}
	}

	// Move Right
	else if (direction == "Right") {

		transform.localPosition += new Vector3(0, 0, -1);

		if (isValidGridPos()){
			updateGrid();}

		else{
			transform.localPosition += new Vector3(0, 0, 1);
		}
	}
}

void leftOrientationGesture(string direction){
	// Move Forward
	if (direction == "Right") {
		transform.localPosition += new Vector3(-1, 0, 0);

		if (isValidGridPos())
			updateGrid();

		else
			transform.localPosition += new Vector3(1, 0, 0);
	}

	else if (direction == "Left") {

		transform.localPosition += new Vector3(1, 0, 0);

		if (isValidGridPos())
			updateGrid();

		else
			transform.localPosition += new Vector3(-1, 0, 0);
	}

	// Move Left
	else if (direction == "Forward") {

		transform.localPosition += new Vector3(0, 0, 1);

		if (isValidGridPos ()) {
			updateGrid ();
		} else {
			transform.localPosition += new Vector3 (0, 0, -1);
		}
	}

	// Move Right
	else if (direction == "Backward") {

		transform.localPosition += new Vector3(0, 0, -1);

		if (isValidGridPos()){
			updateGrid();}

		else{
			transform.localPosition += new Vector3(0, 0, 1);
		}
	}
}

void backOrientationGesture(string direction){
	// Move Forward
	if (direction == "Backward") {
		transform.localPosition += new Vector3(-1, 0, 0);

		if (isValidGridPos())
			updateGrid();

		else
			transform.localPosition += new Vector3(1, 0, 0);
	}

	else if (direction == "Forward") {

		transform.localPosition += new Vector3(1, 0, 0);

		if (isValidGridPos())
			updateGrid();

		else
			transform.localPosition += new Vector3(-1, 0, 0);
	}

	// Move Left
	else if (direction == "Right") {

		transform.localPosition += new Vector3(0, 0, 1);

		if (isValidGridPos ()) {
			updateGrid ();
		} else {
			transform.localPosition += new Vector3 (0, 0, -1);
		}
	}

	// Move Right
	else if (direction == "Left") {

		transform.localPosition += new Vector3(0, 0, -1);

		if (isValidGridPos()){
			updateGrid();}

		else{
			transform.localPosition += new Vector3(0, 0, 1);
		}
	}
}

void controlGestures(){
	string direction = getMoveDirection();
	switch(getOrientation()){
		case "Right":
		rightOrientationGesture(direction);
		break;

		case "Front":
		frontOrientationGesture(direction);
		break;

		case "Left":
		leftOrientationGesture(direction);
		break;

		case "Back":
		backOrientationGesture(direction);
		break;
	}
}

    void Update() {
			if (isTrackingMarker()){
				controlArrows();
				controlGestures();
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


/*CODE FOR VOICE RECOGNITION
* This part of the code (voice recognition) is PARTIALLY based on Watson Unity SDK's sample code for speech recognition streaming (ExampleStreaming.cs)
* Copyright 2015 IBM Corp. All Rights Reserved.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*      http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*
*/

	private IEnumerator CreateService()
	{
		//  Create credential and instantiate service
		Credentials credentials = null;

		TokenOptions tokenOptions = new TokenOptions()
		{
				IamApiKey = apiKey,
				IamUrl = iamURL
		};

		credentials = new Credentials(tokenOptions, serviceURL);

		//  Wait for tokendata
		while (!credentials.HasIamTokenData())
		yield return null;

		_service = new SpeechToText(credentials);
		_service.StreamMultipart = true;

		Active = true;
		StartRecording();
	}

	public bool Active
	{
		get { return _service.IsListening; }
		set
		{
			if (value && !_service.IsListening)
			{
				//_service.RecognizeModel = (string.IsNullOrEmpty(_recognizeModel) ? "en-US_BroadbandModel" : _recognizeModel);
				_service.DetectSilence = true;
				_service.SilenceThreshold = 0.01f;
				_service.MaxAlternatives = 0;
				_service.EnableInterimResults = true;
				_service.OnError = OnError;
				_service.InactivityTimeout = 1;
				//_service.WordAlternativesThreshold = null;
				_service.StartListening(OnRecognize);
			}
			else if (!value && _service.IsListening)
			{
				_service.StopListening();
			}
		}
	}

	private void StartRecording()
	{
		if (_recordingRoutine == 0)
		{
			UnityObjectUtil.StartDestroyQueue();
			_recordingRoutine = Runnable.Run(RecordingHandler());
		}
	}

	private void StopRecording()
	{
		if (_recordingRoutine != 0)
		{
			Microphone.End(_microphoneID);
			Runnable.Stop(_recordingRoutine);
			_recordingRoutine = 0;
		}
	}

	private void OnError(string error)
	{
		Active = false;

		Log.Debug("ExampleStreaming.OnError()", "Error! {0}", error);
	}

	private IEnumerator RecordingHandler()
	{
		Log.Debug("ExampleStreaming.RecordingHandler()", "devices: {0}", Microphone.devices);
		_recording = Microphone.Start(_microphoneID, true, _recordingBufferSize, _recordingHZ);
		yield return null;      // let _recordingRoutine get set..

		if (_recording == null)
		{
			StopRecording();
			yield break;
		}

		bool bFirstBlock = true;
		int midPoint = _recording.samples / 2;
		float[] samples = null;

		while (_recordingRoutine != 0 && _recording != null)
		{
			int writePos = Microphone.GetPosition(_microphoneID);
			if (writePos > _recording.samples || !Microphone.IsRecording(_microphoneID))
			{
				Log.Error("ExampleStreaming.RecordingHandler()", "Microphone disconnected.");

				StopRecording();
				yield break;
			}

			if ((bFirstBlock && writePos >= midPoint)
				|| (!bFirstBlock && writePos < midPoint))
			{
				// front block is recorded, make a RecordClip and pass it onto our callback.
				samples = new float[midPoint];
				_recording.GetData(samples, bFirstBlock ? 0 : midPoint);

				AudioData record = new AudioData();
				record.MaxLevel = Mathf.Max(Mathf.Abs(Mathf.Min(samples)), Mathf.Max(samples));
				record.Clip = AudioClip.Create("Recording", midPoint, _recording.channels, _recordingHZ, false);
				record.Clip.SetData(samples, 0);

				_service.OnListen(record);

				bFirstBlock = !bFirstBlock;
			}
			else
			{
				// calculate the number of samples remaining until we ready for a block of audio,
				// and wait that amount of time it will take to record.
				int remaining = bFirstBlock ? (midPoint - writePos) : (_recording.samples - writePos);
				float timeRemaining = (float)remaining / (float)_recordingHZ;

				yield return new WaitForSeconds(timeRemaining);
			}

		}

		yield break;
	}

	private void OnRecognize(SpeechRecognitionEvent result, Dictionary<string, object> customData)
	{
		if (result != null && result.results.Length > 0)
		{
			int counter = 0;
			foreach (var res in result.results)
			{
				foreach (var alt in res.alternatives)
				{	counter++;
					Debug.Log ("counter: " + counter);
					voiceControl ("Left");
					//Debug.Log ();
					transcript = alt.transcript;
					Debug.Log (transcript);
					string text = string.Format("{0} ({1}, {2:0.00})\n", alt.transcript, res.final ? "Final" : "Interim", alt.confidence);
					Log.Debug("ExampleStreaming.OnRecognize()", text);
					//ResultsField.text = text;

					if (transcript.Contains ("move left")) {
						voiceControl ("Left");
					} else if (transcript.Contains ("move right")) {
						voiceControl ("Right");
					} else if (transcript.Contains ("move forward")) {
						voiceControl ("Forward");
					} else if (transcript.Contains ("move back")) {
						voiceControl ("Backward");
					} else if (transcript.Contains ("rotate")) {
						voiceControl ("Rotate");
					}
				}

				/*if (res.keywords_result != null && res.keywords_result.keyword != null)
				{
					foreach (var keyword in res.keywords_result.keyword)
					{
						Log.Debug("ExampleStreaming.OnRecognize()", "keyword: {0}, confidence: {1}, start time: {2}, end time: {3}", keyword.normalized_text, keyword.confidence, keyword.start_time, keyword.end_time);
					}
				}*/
			}
		}
	}

	void voiceControl(string command){
			Debug.Log ("command: " + command);
			switch (getOrientation ()) {
			case "Right":
				rightOrientationGesture (command);
				break;

			case "Front":
				frontOrientationGesture (command);
				break;

			case "Left":
				leftOrientationGesture (command);
				break;

			case "Back":
				backOrientationGesture (command);
				break;
			case "Rotate":
				transform.Rotate (0, 0, -90);

				if (isValidGridPos ())
					updateGrid ();
				else
					transform.Rotate (0, 0, 90);
				break;
			}
	}

}
