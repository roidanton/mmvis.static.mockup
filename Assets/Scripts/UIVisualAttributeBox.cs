using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Kender.uGUI;

public class UIVisualAttributeBox : MonoBehaviour {

	ComboBox comboBox;
	List<ComboBox> otherVABoxes = new List<ComboBox>(); // Contains the other boxes responsible for setting visual attributes.

	CreateEventObjects controller;

	private void Start() {
		comboBox = this.gameObject.GetComponent<ComboBox>();
		controller = GameObject.FindGameObjectWithTag("GameController").GetComponent<CreateEventObjects>();

		getOtherVisualAttributeBoxes ();
		setBoxItems();
		setInitialValue();

		comboBox.OnSelectionChanged += (int index) => {
			checkOtherVisualAttributeBoxes(index);
			controller.updateItems(); // Doesn't seem to be always called.
		};
	}

	void getOtherVisualAttributeBoxes() {
		foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag ("VAComboBox")) {
			if (gameObject != this.gameObject)
				otherVABoxes.Add(gameObject.GetComponent<ComboBox>());
		}
	}

	/**
	 * Avoids having the same visual attributes assigned to the same parameter.
	 */
	void checkOtherVisualAttributeBoxes(int index) {
		foreach (ComboBox comboBoxCheckIndex in otherVABoxes) {
			if (index > 0 && comboBoxCheckIndex.SelectedIndex == index) {
				print("Changing " + comboBox.name + " and detecting same index at " + comboBoxCheckIndex.name);
				comboBoxCheckIndex.SelectedIndex = 0;
				comboBoxCheckIndex.Items[0].OnSelect(); // OnSelect Action call of "none".
			}
		}
	}

	void setBoxItems() {
		ComboBoxItem none = new ComboBoxItem("Kein Attribut");
		ComboBoxItem posX = new ComboBoxItem("Position X");
		ComboBoxItem posY = new ComboBoxItem("Position Y");
		ComboBoxItem posZ = new ComboBoxItem("Position Z");
		ComboBoxItem hue = new ComboBoxItem("Farbwert");
		ComboBoxItem brightness = new ComboBoxItem("Helligkeit");
		ComboBoxItem shape = new ComboBoxItem("Form");
		ComboBoxItem size = new ComboBoxItem("Größe");
		ComboBoxItem opacity = new ComboBoxItem("Opazität");
		
		none.OnSelect += () => {
			setParameter(CreateEventObjects.VisualAttribute.Kein);
		};
		posX.OnSelect += () => {
			setParameter(CreateEventObjects.VisualAttribute.PosX);
		};
		posY.OnSelect += () => {
			setParameter(CreateEventObjects.VisualAttribute.PosY);
		};
		posZ.OnSelect += () => {
			setParameter(CreateEventObjects.VisualAttribute.PosZ);
		};
		hue.OnSelect += () => {
			setParameter(CreateEventObjects.VisualAttribute.Farbwert);
		};
		brightness.OnSelect += () => {
			setParameter(CreateEventObjects.VisualAttribute.Helligkeit);
		};
		shape.OnSelect += () => {
			setParameter(CreateEventObjects.VisualAttribute.Form);
		};
		size.OnSelect += () => {
			setParameter(CreateEventObjects.VisualAttribute.Größe);
		};
		opacity.OnSelect += () => {
			setParameter(CreateEventObjects.VisualAttribute.Opazität);
		};
		comboBox.AddItems (none, posX, posY, posZ, hue, brightness, shape, size, opacity);
	}

	/**
	 * Sets the correct parameter of the controller.
	 */
	void setParameter(CreateEventObjects.VisualAttribute va) {
		switch (this.gameObject.name) {
		case "Time":
			controller.zeit = va;
			break;
		case "LocationX":
			controller.ortX = va;
			break;
		case "LocationY":
			controller.ortY = va;
			break;
		case "LocationZ":
			controller.ortZ = va;
			break;
		case "EventType":
			controller.art = va;
			break;
		case "Agglomeration":
			controller.häufung = va;
			break;
		}
	}

	void setInitialValue() {
		switch (this.gameObject.name) {
		case "Time":
			comboBox.SelectedIndex = (int) controller.zeit;
			break;
		case "LocationX":
			comboBox.SelectedIndex = (int) controller.ortX;
			break;
		case "LocationY":
			comboBox.SelectedIndex = (int) controller.ortY;
			break;
		case "LocationZ":
			comboBox.SelectedIndex = (int) controller.ortZ;
			break;
		case "EventType":
			comboBox.SelectedIndex = (int) controller.art;
			break;
		case "Agglomeration":
			comboBox.SelectedIndex = (int) controller.häufung;
			break;
		}
	}
}
