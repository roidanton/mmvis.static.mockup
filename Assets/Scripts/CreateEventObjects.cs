using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CreateEventObjects : MonoBehaviour {

	public TextAsset listOfEvents;
	public TextAsset listOfPolyhedrons;

	public Material objectMaterial;

	public enum VisualAttribute {Kein, PosX, PosY, PosZ, Farbwert, Helligkeit, Form, Größe, Opazität};

	public VisualAttribute zeit;
	public VisualAttribute ortX;
	public VisualAttribute ortY;
	public VisualAttribute ortZ;
	public VisualAttribute art;
	public VisualAttribute häufung;

	List<Event> eventList = new List<Event>();
	List<string> polyList = new List<string>(); // List of Polyhedrons for generating shape from prefabs.

	public float hueMin;
	public float hueMax;

	public float brightnessMin;
	public float brightnessMax;

	public float scaleMin;
	public float scaleMax;

	public float opazitätHäufung;

	public int maxTime;

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
		loadEvents();
		polyList = getPolyList();
		updateItems();
	}

	void Update () {
		if (Input.GetMouseButtonUp(0)) { // Cheat to simulate detection of param change.
			updateItems();
		}
		Controls.getKeys();
	}

	public void updateItems () {
		foreach (GameObject o in GameObject.FindGameObjectsWithTag("Event"))
			Destroy (o);
		foreach (Event e in eventList) {
			createEventMesh (e);
		}
	}

	public void loadEvents() {
		eventList = getEventList();
		calculateEventListAttributes();
		updateItems();
	}

	public void makeNewEvents () {
		eventList = createRandomEventList((int) GameObject.Find ("NumberOfEventsSlider").GetComponent<UnityEngine.UI.Slider>().value, maxTime);
		calculateEventListAttributes();
		updateItems();
	}

	void createEventMesh(Event e) {
		GameObject eventObject = getObject(e);
		//eventObject.name = "Event " + e.id.ToString() + ": " + e.eventType.ToString() + " at " + e.location.ToString() + " on time " + e.time.ToString();
		eventObject.transform.position = getPosition(e);
		eventObject.transform.localScale = getScale(e);

		eventObject.GetComponent<Renderer> ().sharedMaterial = new Material(Shader.Find("Standard"));

		Material material = eventObject.GetComponent<Renderer> ().sharedMaterial;

		// Might not always work when project gets exported.
		// see http://forum.unity3d.com/threads/access-rendering-mode-var-on-standard-shader-via-scripting.287002/
		// Verdeckungsbug, Ursache unbekannt.
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

		material.color = getColor(e);

		eventObject.name = e.eventType.ToString() + " at " + e.location.ToString() + " on time " + e.time.ToString() + ": " + eventObject.GetComponent<Renderer> ().sharedMaterial.color.ToString();
		eventObject.tag = "Event";
	}

	/**
	 * Creates a polyhedron or sphere shape.
	 * 
	 * Currently there is only a list of polyhedrons.
	 * On-the-fly polyhedron creation would be awesome.
	 */
	GameObject getObject (Event e) {
		int shapeIndex = (int) getAttributeValue(e, VisualAttribute.Form, 0f, polyList.Count()-1f, 1000f);
		if (shapeIndex != 1000f)
			return Instantiate(Resources.Load (polyList [shapeIndex], typeof(GameObject))) as GameObject;
		else
			return GameObject.CreatePrimitive(PrimitiveType.Sphere);
	}

	/**
	 * Location min, max are used for position min, max.
	 */
	Vector3 getPosition (Event e) {
		float x = getAttributeValue(e, VisualAttribute.PosX, locXMin, locXMax, 0f);
		float y = getAttributeValue(e, VisualAttribute.PosY, locYMin, locYMax, 0f);
		float z = getAttributeValue(e, VisualAttribute.PosZ, locZMin, locZMax, 0f);

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
	 * @Return: defaultValue if nothing was set. Sometimes used as detection
	 * in calling funtion (bad style).
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

	List<string> getPolyList() {
		List<string> polyList = new List<string> ();
		string[,] listOfPolysGrid = CSVReader.SplitCsvGrid (listOfPolyhedrons.text); //CSVReader.DebugOutputGrid(listOfEventsGrid);
		
		for (int row = 0; row < listOfPolysGrid.GetUpperBound(1); row++) {
			string polyhedron = listOfPolysGrid[0,row];
			polyList.Add(polyhedron);
		}
		return polyList;
	}

	List<Event> createRandomEventList(int numberOfEvents, float maxTime) {
		List<Event> eventList = new List<Event> ();
		float maxRadius = 10f;
		
		for (int i = 0; i < numberOfEvents; i++) {
			float progress = (float) i/(float) numberOfEvents;
			Event e = new Event ();
			e.id = i+1;
			e.time = Random.Range(0f + progress * maxTime/2, 1f + progress * maxTime);
			e.location.Set(Mathf.Floor(e.time*4f/3f + .5f), Random.Range(0f, maxRadius), Random.Range(0f, maxRadius));
			e.eventType = (Event.EventType) Random.Range(0f, 3f);
			eventList.Add(e);
		}
		return eventList;
	}

	List<Event> getEventList() {
		List<Event> eventList = new List<Event> ();
		string[,] listOfEventsGrid = CSVReader.SplitCsvGrid (listOfEvents.text); //CSVReader.DebugOutputGrid(listOfEventsGrid);
		
		for (int row = 0; row < listOfEventsGrid.GetUpperBound(1); row++) {
			Event e = new Event ();
			e.id = int.Parse (listOfEventsGrid [0, row]);
			e.time = float.Parse (listOfEventsGrid [1, row]);
			e.location.Set (float.Parse (listOfEventsGrid [2, row]), float.Parse (listOfEventsGrid [3, row]), float.Parse (listOfEventsGrid [4, row]));
			e.setEventType (listOfEventsGrid [5, row]);
			eventList.Add (e);
		}
		return eventList;
	}

	void calculateEventListAttributes() {
		eventList.setLocationAgglomeration ();
		eventList.setTemporalAgglomeration ();
		
		timeMin = eventList.getMinTime();
		timeMax = eventList.getMaxTime();

		print ("Time: (" + timeMin + ", " + timeMax + ")");

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
