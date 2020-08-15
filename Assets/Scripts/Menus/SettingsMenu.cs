using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour {
	public SceneFader SceneFader;
	public Dropdown GameModeDropdown;
	public InputField GenerationCountField;
	public Dropdown GameResolutionDropdown;
	public Slider SoundVolumelider;
	public AudioSource Music;
	public InputField AgentName;
	public RawImage ImportIdentificator;

	public static int AgentCount { get; private set; } = 42;
	public static bool PlayerInput { get; private set; }
	public static float MusicVolume { get; private set; } = 1f;
	public static Queue<Genotype> ImportedGenotypes { get; private set; }

	private void Awake() {
		// Default values
		AgentCount = 42;
		PlayerInput = false;
		ImportedGenotypes = new Queue<Genotype>();
		this.Music.volume = 1f;
	}

	public void GoBackTomainMenu() {
		SceneFader.FadeToScene("MainMenu");
	}

	public void OnValueChanged() {
		SettingsMenu.AgentCount = Convert.ToInt32(GenerationCountField.text);
	}

	public void OnModeChanged() {
		if (GameModeDropdown.value == 0) {
			PlayerInput = false;
			return;
		}
		if (GameModeDropdown.value == 1) {
			PlayerInput = true;
			return;
		}
	}

	public void OnImportClick() {
		Genotype result = null;
		try {
			if (this.AgentName.text == "") {
				result = Genotype.LoadFromFile();
			}
			else {
				result = Genotype.LoadFromFile(this.AgentName.text);
			}
			this.ImportIdentificator.color = Color.green;
		}
		catch {
			this.ImportIdentificator.color = Color.red;
		}
		//Debug.Log(result.GenotypeValuesToString());
		ImportedGenotypes.Enqueue(result);
		//Debug.Log(ImportedGenotypes.Count);
	}

	public void ChangeResolution() {
		switch (GameResolutionDropdown.value) {
			case 0:
				Screen.SetResolution(640, 480, true);
				break;
			case 1:
				Screen.SetResolution(800, 600, true);
				break;
			case 2:
				Screen.SetResolution(1280, 720, true);
				break;
			case 3:
				Screen.SetResolution(1920, 1080, true);
				break;
			case 4:
				Screen.SetResolution(2560, 1440, true);
				break;
		}
	}

	public void ChangeVolume(float value) {
		this.Music.volume = value;
		MusicVolume = value;
	}
}
