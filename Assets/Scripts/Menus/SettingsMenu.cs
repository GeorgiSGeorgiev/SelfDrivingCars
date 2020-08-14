using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour {
	public SceneFader SceneFader;
	public InputField GenerationCountField;
	public Dropdown GameModeDropdown;
	public InputField AgentName;

	public static int AgentCount { get; private set; }
	public static bool PlayerInput { get; private set; }
	public static Queue<Genotype> ImportedGenotypes { get; private set; }

	private void Awake() {
		// Default values
		AgentCount = 42;
		PlayerInput = false;
		ImportedGenotypes = new Queue<Genotype>();
	}

	public void GoBackTomainMenu() {
		SceneFader.FadeToScene("MainMenu");
	}

	public void OnValueChanged() {
		SettingsMenu.AgentCount = Convert.ToInt32(GenerationCountField.text);
	}

	public void OnModeChanged() {
		if (GameModeDropdown.value == 0) {
			//Debug.Log("AI mode");
			PlayerInput = false;
			return;
		}
		if (GameModeDropdown.value == 1) {
			//Debug.Log("Player mode");
			PlayerInput = true;
			return;
		}
	}

	public void OnImportClick() {
		Genotype result;
		if (this.AgentName.text == "") {
			result = Genotype.LoadFromFile();
		}
		else {
			Debug.Log("Name");
			result = Genotype.LoadFromFile(this.AgentName.text);
		}
		ImportedGenotypes.Enqueue(result);
	}
}
