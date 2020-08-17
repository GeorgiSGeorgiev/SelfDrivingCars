using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour {
	public SceneFader SceneFader;
	public Dropdown GameModeDropdown;
	public InputField GenerationCountField;

	public Dropdown GameResolutionDropdown;
	private Resolution[] resolutions;

	public Dropdown CameraDropdown;

	public Slider SoundVolumeSlider;
	public AudioMixer MainAudioMixer;
	
	public InputField AgentName;
	public RawImage ImportIdentificator;

	public static int AgentCount { get; private set; } = 42;
	private int lastAgentCount = 0;

	public static bool PlayerInput { get; private set; }
	public static CameraMode CurrentCameraMode = CameraMode.Follow;
	public static Queue<Genotype> ImportedGenotypes { get; private set; }

	private void Awake() {
		// Default values
		AgentCount = 42;
		PlayerInput = false;
		ImportedGenotypes = new Queue<Genotype>();

		switch (CurrentCameraMode) {
			case (CameraMode.Follow):
				this.CameraDropdown.value = 0;
				break;
			case (CameraMode.FollowAndRotate):
				this.CameraDropdown.value = 1;
				break;
		}
		this.CameraDropdown.RefreshShownValue();

		float currentVolume = 0;
		MainAudioMixer.GetFloat("Volume", out currentVolume);
		this.SoundVolumeSlider.value = currentVolume;
	}

	private void Start() {
		this.resolutions = Screen.resolutions;

		int currentResolutionInx = 0;

		GameResolutionDropdown.ClearOptions();

		List<string> resolutionNames = new List<string>();

		for (int i = 0; i < this.resolutions.Length; i++) {
			string resName = this.resolutions[i].width + " x " + this.resolutions[i].height;
			resolutionNames.Add(resName);

			if (this.resolutions[i].Equals(Screen.currentResolution)) {
				currentResolutionInx = i;
			}
		}

		GameResolutionDropdown.AddOptions(resolutionNames);
		GameResolutionDropdown.value = currentResolutionInx;
		GameResolutionDropdown.RefreshShownValue();
	}

	public void GoBackToMainMenu() {
		SceneFader.FadeToScene("MainMenu");
	}

	public void OnValueChanged() {
		if (GenerationCountField.text == "") {
			GenerationCountField.text = "0";
		}
		if (GenerationCountField.text.StartsWith("0") && GenerationCountField.text != "0") {
			GenerationCountField.text = GenerationCountField.text.Remove(0,1);
		}
		int result = Convert.ToInt32(GenerationCountField.text);
		if (SettingsMenu.PlayerInput) {
			this.lastAgentCount = SettingsMenu.AgentCount;
		}
		SettingsMenu.AgentCount = result;
	}

	public void OnValueStringEndEdit() {
		if (SettingsMenu.AgentCount < 4 && !SettingsMenu.PlayerInput) {
			SettingsMenu.AgentCount = 4;
			this.lastAgentCount = SettingsMenu.AgentCount;
			GenerationCountField.text = "4";
		}
	}

	public void OnModeChanged() {
		if (GameModeDropdown.value == 0) {
			PlayerInput = false;
			this.GenerationCountField.enabled = true;
			// Debug.Log(this.lastAgentCount);
			this.GenerationCountField.text = this.lastAgentCount.ToString();
			AgentCount = this.lastAgentCount;
			return;
		}
		if (GameModeDropdown.value == 1) {
			PlayerInput = true;
			this.GenerationCountField.text = "1";
			this.GenerationCountField.enabled = false;
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

	public void ChangeResolution(int resolutionIndex) {
		Resolution resolution = this.resolutions[resolutionIndex];
		Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
	}

	public void SetFullScreenMode(bool isFullScreen) {
		Screen.fullScreen = isFullScreen;
	}

	public void ChangeQuality(int qualityIndex) {
		QualitySettings.SetQualityLevel(qualityIndex);
	}

	public void ChangeCamera(int cameraIndex) {
		switch (cameraIndex) {
			case 0:
				SettingsMenu.CurrentCameraMode = CameraMode.Follow;
				break;
			case 1:
				SettingsMenu.CurrentCameraMode = CameraMode.FollowAndRotate;
				break;
		}
	}

	public void ChangeVolume(float volume) {
		this.MainAudioMixer.SetFloat("Volume", volume);
	}
}
