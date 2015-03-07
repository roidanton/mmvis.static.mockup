using UnityEngine;
using System.Collections;

public static class Controls {

	static CreateEventObjects obj;

	public static void getKeys () {
		obj = GameObject.FindGameObjectWithTag ("GameController").GetComponent<CreateEventObjects>();
		if (Input.GetKeyUp (KeyCode.P)) {
			string filename = (obj.zeit != CreateEventObjects.VisualAttribute.Kein ? obj.zeit.ToString() : "") + "-"
				+ (obj.ortX != CreateEventObjects.VisualAttribute.Kein ? obj.ortX.ToString() : "") + "-"
					+ (obj.ortY != CreateEventObjects.VisualAttribute.Kein ? obj.ortY.ToString() : "") + "-"
					+ (obj.ortZ != CreateEventObjects.VisualAttribute.Kein ? obj.ortZ.ToString() : "") + "-"
					+ (obj.art != CreateEventObjects.VisualAttribute.Kein ? obj.art.ToString() : "");
			Application.CaptureScreenshot (filename + ".png", 4);
			Debug.Log ("Screenshot captured: " + filename);
		}
	}
}
