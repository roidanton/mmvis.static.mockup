using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Event {
	[HideInInspector]
	public int id;
	[HideInInspector]
	public float time;
	[HideInInspector]
	public Vector3 location = new Vector3();
	[HideInInspector]
	public int locationAgglomeration = 0;
	public int temporalAgglomeration = 0;
	

	public enum EventType {Birth, Split, Merge, Death, None};

	public EventType eventType;

	public void setEventType (string str) {
		eventType = transformEventType (str);
	}

	public EventType transformEventType (string str){
		switch (str) {
		case "Birth":
			return EventType.Birth;
		case "Split":
			return EventType.Split;
		case "Merge":
			return EventType.Merge;
		case "Death":
			return EventType.Death;
		default:
			return EventType.None;
		}
	}
}

/**
 * Extensions for EventList.
 * Bad style: It should be an own class implementing IList Interface instead!
 */
public static class ListExtensions {

	public static void setLocationAgglomeration(this List<Event> list) {
		foreach (Event e1 in list)
			foreach (Event e2 in list)
				if (e2.location.x == e1.location.x && e2.location.y == e1.location.y && e2.location.z == e1.location.z)
					e1.locationAgglomeration++;
	}
	
	public static void setTemporalAgglomeration(this List<Event> list) {
		foreach (Event e1 in list)
			foreach (Event e2 in list)
				if (e2.time == e1.time)
					e1.temporalAgglomeration++;
	}

	public static float getMinTime(this List<Event> list)	{
		float a = 0f;
		foreach (Event e in list)
			if (e.time < a)
				a = e.time;
		return a;
	}
	public static float getMaxTime(this List<Event> list)	{
		float a = 0f;
		foreach (Event e in list)
			if (e.time > a)
				a = e.time;
		return a;
	}
	public static float getMinLocX(this List<Event> list)	{
		float a = 0f;
		foreach (Event e in list)
			if (e.location.x < a)
				a = e.location.x;
		return a;
	}
	public static float getMaxLocX(this List<Event> list)	{
		float a = 0f;
		foreach (Event e in list)
			if (e.location.x > a)
				a = e.location.x;
		return a;
	}
	public static float getMinLocY(this List<Event> list)	{
		float a = 0f;
		foreach (Event e in list)
			if (e.location.y < a)
				a = e.location.y;
		return a;
	}
	public static float getMaxLocY(this List<Event> list)	{
		float a = 0f;
		foreach (Event e in list)
			if (e.location.y > a)
				a = e.location.y;
		return a;
	}
	public static float getMinLocZ(this List<Event> list)	{
		float a = 0f;
		foreach (Event e in list)
			if (e.location.z < a)
				a = e.location.z;
		return a;
	}
	public static float getMaxLocZ(this List<Event> list)	{
		float a = 0f;
		foreach (Event e in list)
			if (e.location.z > a)
				a = e.location.z;
		return a;
	}

	public static int getMinTA(this List<Event> list)	{
		int a = 0;
		foreach (Event e in list)
			if (e.temporalAgglomeration < a)
				a = e.temporalAgglomeration;
		return a;
	}

	public static int getMaxTA(this List<Event> list)	{
		int a = 0;
		foreach (Event e in list)
			if (e.temporalAgglomeration > a)
				a = e.temporalAgglomeration;
		return a;
	}

	public static int getMinLA(this List<Event> list)	{
		int a = 0;
		foreach (Event e in list)
			if (e.locationAgglomeration < a)
				a = e.locationAgglomeration;
		return a;
	}
	
	public static int getMaxLA(this List<Event> list)	{
		int a = 0;
		foreach (Event e in list)
			if (e.locationAgglomeration > a)
				a = e.locationAgglomeration;
		return a;
	}
}
