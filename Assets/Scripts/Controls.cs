using UnityEngine;
using System.Collections;

public static class Controls {

	public static void getKeys () {
		if (Input.GetKeyUp (KeyCode.N)) {
			GameObject.FindGameObjectWithTag ("GameController").GetComponent<CreateEventObjects>().makeNewEvents();
		}
		if (Input.GetKeyUp (KeyCode.B)) {
			GameObject.FindGameObjectWithTag ("GameController").GetComponent<CreateEventObjects>().loadEvents();
		}
		if (Input.GetKeyUp (KeyCode.P)) {
			makeScreenshot();
		}
		if (Input.GetKeyUp (KeyCode.Space)) {
			switchTwoCameras();
		}
		if (Input.GetKeyUp (KeyCode.Tab)) {
			toggleUI();
		}
	}

	static void toggleUI() {
		Canvas canvas = GameObject.Find ("Canvas").GetComponent<Canvas> ();
		canvas.enabled = !canvas.enabled;
	}

	static void makeScreenshot () {
		// Hide UI.
		Canvas canvas = GameObject.Find ("Canvas").GetComponent<Canvas> ();
		canvas.enabled = false;
		
		CreateEventObjects o = GameObject.FindGameObjectWithTag ("GameController").GetComponent<CreateEventObjects>();
		string filename = (o.zeit != CreateEventObjects.VisualAttribute.Kein ? o.zeit.ToString() : "") + "-"
			+ (o.ortX != CreateEventObjects.VisualAttribute.Kein ? o.ortX.ToString() : "") + "-"
				+ (o.ortY != CreateEventObjects.VisualAttribute.Kein ? o.ortY.ToString() : "") + "-"
				+ (o.ortZ != CreateEventObjects.VisualAttribute.Kein ? o.ortZ.ToString() : "") + "-"
				+ (o.art != CreateEventObjects.VisualAttribute.Kein ? o.art.ToString() : "");
		Application.CaptureScreenshot (filename + ".png", 4);
		Debug.Log ("Screenshot captured: " + filename);
	}

	static void switchTwoCameras() {
		foreach (GameObject o in GameObject.FindGameObjectsWithTag("MainCamera")) {
			o.GetComponent<Camera> ().enabled = !o.GetComponent<Camera> ().enabled;
			if (o.GetComponent<ControlWASD>())
				o.GetComponent<ControlWASD>().resetRotation(new Vector3(15f,5f,5f));
		}
	}
}
