using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CreateEventObjects : MonoBehaviour {

	public TextAsset listOfEvents;

	public Material objectMaterial;

	public enum VisualAttribute {Kein, PosX, PosY, PosZ, Farbwert, Helligkeit, Form, Größe, Opazität};

	public VisualAttribute zeit;
	public VisualAttribute ortX;
	public VisualAttribute ortY;
	public VisualAttribute ortZ;
	public VisualAttribute art;
	public VisualAttribute häufung;

	List<Event> eventList = new List<Event>();

	public float hueMin;
	public float hueMax;

	public float brightnessMin;
	public float brightnessMax;

	public float scaleMin;
	public float scaleMax;

	public float opazitätHäufung;

	float deathHue = 0f;
	float mergeHue = 55f;
	float splitHue = 245f;
	float birthHue = 145f;

	float timeMin;
	float timeMax;
	float locXMin;
	float locXMax;
	float locYMin;
	float locYMax;
	float locZMin;
	float locZMax;
	float TAMin;
	float TAMax;
	float LAMin;
	float LAMax;
	
	void Start () {
		setEventList ();
	}

	void Update () {
		if (Input.GetMouseButtonUp(0)) { // Cheat to simulate detection of param change.
			foreach (GameObject o in GameObject.FindGameObjectsWithTag("Event"))
				Destroy (o);
			foreach (Event e in eventList) {
				createEventMesh (e);
			}
		}

		Controls.getKeys();
	}

	void createEventMesh(Event e) {
		GameObject eventObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		//eventObject.name = "Event " + e.id.ToString() + ": " + e.eventType.ToString() + " at " + e.location.ToString() + " on time " + e.time.ToString();
		eventObject.transform.position = getPosition (e);
		eventObject.transform.localScale = getScale(e);

		addShape (e, eventObject);

		Material material = eventObject.GetComponent<Renderer> ().material;

		// Might not work when project gets exported!
		// see http://forum.unity3d.com/threads/access-rendering-mode-var-on-standard-shader-via-scripting.287002/
		material.SetFloat("_Mode", 2);
		material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
		material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
		material.SetInt("_ZWrite", 0);
		material.DisableKeyword("_ALPHATEST_ON");
		material.EnableKeyword("_ALPHABLEND_ON");
		material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
		material.renderQueue = 3000;

		// Does not seem to work like that, but maybe the way to go if method above does not work anymore.
		//eventObject.GetComponent<Renderer> ().material.CopyPropertiesFromMaterial (objectMaterial);

		material.color = getColor (e);

		eventObject.name = e.eventType.ToString() + " at " + e.location.ToString() + " on time " + e.time.ToString() + ": " + eventObject.GetComponent<Renderer> ().material.color.ToString();
		eventObject.tag = "Event";
	}

	/**
	 * Defines a shape by adding Cylinders.
	 * Location x min, max are used for shape min, max.
	 */
	void addShape (Event e, GameObject parent) {
		/*
		Vector3[] newVertices;
		Vector2[] newUV;
		int[] newTriangles;

		Mesh mesh = new Mesh();
		parent.GetComponent<MeshFilter>().mesh = mesh;
		mesh.vertices = newVertices;
		mesh.uv = newUV;
		mesh.triangles = newTriangles;
*/
		float numberOfCorners = getAttributeValue(e, VisualAttribute.Form, locXMin, locXMax, 0f);

		for (int i = 0; i < (int) numberOfCorners; i++) {
			GameObject corner = GameObject.CreatePrimitive(PrimitiveType.Capsule);
			corner.transform.parent = parent.transform;
			transform.localPosition = new Vector3(i, 1, 0);
		}
	}

	/**
	 * Location min, max are used for position min, max.
	 */
	Vector3 getPosition (Event e) {
		float x = getAttributeValue(e, VisualAttribute.PosX, locXMin, locXMax, 0f);
		float y = getAttributeValue(e, VisualAttribute.PosY, locYMin, locYMax, 0f);
		float z = getAttributeValue(e, VisualAttribute.PosY, locZMin, locZMax, 0f);

		return new Vector3(x, y, z);
	}

	Vector3 getScale (Event e) {
		float scale = getAttributeValue (e, VisualAttribute.Größe, scaleMin, scaleMax, 1f);
		return new Vector3(scale, scale, scale);
	}

	Color getColor (Event e) {
		float hue = getAttributeValue(e, VisualAttribute.Farbwert, hueMin, hueMax, 350f);
		float saturation = 1f;
		float brightness = getAttributeValue(e, VisualAttribute.Helligkeit, brightnessMin, brightnessMax, .8f);
		float alpha = getAttributeValue(e, VisualAttribute.Opazität, brightnessMin, brightnessMax, 1f);

		//print ("H: " + hue + ", B: " + brightness + ", A: " + alpha);

		return HSBColor.ToColor(new HSBColor(hue, saturation, brightness, alpha));
	}

	/**
	 * Uses linear approximation to determine attribute values, except when:
	 * - Art = Farbwert
	 * - Häufung = Opacity
	 * @Return: defaultValue if nothing was set.
	 */
	float getAttributeValue (Event e, VisualAttribute visAttr, float aMin, float aMax, float defaultValue) {
		if (zeit == visAttr)
			return linearApproximation(e.time, timeMin, timeMax, aMin, aMax);
		if (ortX == visAttr)
			return linearApproximation(e.location.x, locXMin, locXMax, aMin, aMax);
		if (ortY == visAttr)
			return linearApproximation(e.location.y, locYMin, locYMax, aMin, aMax);
		if (ortZ == visAttr)
			return linearApproximation(e.location.z, locZMin, locZMax, aMin, aMax);
		if (art == visAttr) {
			if (visAttr == VisualAttribute.Farbwert) {
				switch(e.eventType) {
				case Event.EventType.Death:
					return deathHue;
				case Event.EventType.Merge:
					return mergeHue;
				case Event.EventType.Split:
					return splitHue;
				case Event.EventType.Birth:
					return birthHue;
				}
			} else {
				float numberedEventType = ((float) e.eventType) / 3f;
				//print (e.eventType.ToString() + ": " + (float) e.eventType + ", " + numberedEventType);
				return linearApproximation (numberedEventType, 0f, 1f, aMin, aMax);
			}
		}
		if (häufung == visAttr) {
			if (visAttr == VisualAttribute.Opazität)
				return opazitätHäufung;
			else {
				if (zeit == VisualAttribute.PosX || zeit == VisualAttribute.PosY || zeit == VisualAttribute.PosZ)
					return linearApproximation (e.temporalAgglomeration, TAMin, TAMax, aMin, aMax);
				else 
					return linearApproximation (e.locationAgglomeration, LAMin, LAMax, aMin, aMax);
			}
		}
		return defaultValue;
	}

	/**
	 * Transforms a parameter p into an attribute a with the help of linear approximation.
	 * 
	 * @Return: Corresponding attribute float.
	 */
	float linearApproximation(float p, float pMin, float pMax, float aMin, float aMax) {
		float n = (aMin * pMax - aMax * pMin) / (pMax - pMin);
		float m = (aMax - n) / pMax;
		//print ("aMin: " + aMin + ", aMax: " + aMax + ", pMin: " + pMin + ", pMax: " + pMax + ", n: " + n + ", m: " + m);
		return m * p + n;
	}
	
	void setEventList() {
		string[,] listOfEventsGrid = CSVReader.SplitCsvGrid (listOfEvents.text); //CSVReader.DebugOutputGrid(listOfEventsGrid);
		
		for (int row = 0; row < listOfEventsGrid.GetUpperBound(1); row++) {
			Event e = new Event ();
			e.id = int.Parse(listOfEventsGrid[0,row]);
			e.time = float.Parse(listOfEventsGrid[1,row]);
			e.location.Set(float.Parse(listOfEventsGrid[2,row]), float.Parse(listOfEventsGrid[3,row]), 0);
			e.setEventType(listOfEventsGrid[4,row]);
			eventList.Add(e);
		}

		eventList.setLocationAgglomeration ();
		eventList.setTemporalAgglomeration ();
		
		timeMin = eventList.getMinTime();
		timeMax = eventList.getMaxTime();
		
		TAMin = eventList.getMinTA();
		TAMax = eventList.getMaxTA();
		LAMin = eventList.getMinLA();
		LAMax = eventList.getMaxLA();
		
		print ("TA: (" + TAMin + ", " + TAMax + "), LA: (" + LAMin + ", " + LAMax + ")");
		
		locXMin = eventList.getMinLocX();
		locXMax = eventList.getMaxLocX();
		locYMin = eventList.getMinLocY();
		locYMax = eventList.getMaxLocY();
		locZMin = eventList.getMinLocZ();
		locZMax = eventList.getMaxLocZ();
		
		print ("x: (" + locXMin + ", " + locXMax + ")");
		print ("y: (" + locYMin + ", " + locYMax + ")");
		print ("z: (" + locZMin + ", " + locZMax + ")");
	}
}
