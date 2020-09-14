using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

/// <summary>
/// Main game settings. Many of the fields are static because settings have to be global.
/// Settings are visible from every game scene. This way the pause menu in every track can access adn change them.
/// </summary>
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

	// Awake is called  on class load before Start.
	private void Awake() {
		// Default values. The following three parameters are reset on every SettingsMenu-scene load.
		AgentCount = 42;
		PlayerInput = false;
		ImportedGenotypes = new Queue<Genotype>();

		// Set the camera. Important if player has already changed the settings in game. So they are kept.
		switch (CurrentCameraMode) {
			case (CameraMode.Follow):
				this.CameraDropdown.value = 0;
				break;
			case (CameraMode.FollowAndRotate):
				this.CameraDropdown.value = 1;
				break;
		}
		this.CameraDropdown.RefreshShownValue();

		// Set the volume, restore the previous state (same as the camera)
		float currentVolume = 0;
		MainAudioMixer.GetFloat("Volume", out currentVolume);
		this.SoundVolumeSlider.value = currentVolume;
	}

	private void Start() {
		// Find all available screen resolutions.
		// Screen resolutions depend on the player's screen.
		this.resolutions = Screen.resolutions;

		int currentResolutionInx = 0;

		GameResolutionDropdown.ClearOptions();

		List<string> resolutionNames = new List<string>();

		// Add the available resolutions to list of strings (method that adds options to the settings menu dropdown requires it).
		for (int i = 0; i < this.resolutions.Length; i++) {
			string resName = this.resolutions[i].width + " x " + this.resolutions[i].height;
			resolutionNames.Add(resName);

			if (this.resolutions[i].Equals(Screen.currentResolution)) {
				currentResolutionInx = i; // Save the current screen resolution. We don't want to change it.
			}
		}

		// Set the GameResolutionDropdown.
		GameResolutionDropdown.AddOptions(resolutionNames);
		GameResolutionDropdown.value = currentResolutionInx;
		GameResolutionDropdown.RefreshShownValue(); // this method has to be called on every dropdown update from the code
	}

	/// <summary>
	/// Loads the MainMenu scene.
	/// </summary>
	public void GoBackToMainMenu() {
		SceneFader.FadeToScene("MainMenu");
	}

	/// <summary>
	/// Called on every value change of the <paramref name="GenerationCountField"/>.
	/// </summary>
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

	/// <summary>
	/// Called on end of the string edit of the <paramref name="GenerationCountField"/>.
	/// <para>Warning! The genetic algorithm works with generations which size is bigger than 3.
	/// The <paramref name="GenerationCountField"/> is set according to that and won't allow the user to add values less than 4. </para>
	/// </summary>
	public void OnValueStringEndEdit() {
		if (SettingsMenu.AgentCount < 4 && !SettingsMenu.PlayerInput) {
			SettingsMenu.AgentCount = 4;
			this.lastAgentCount = SettingsMenu.AgentCount;
			GenerationCountField.text = "4";
		}
	}

	/// <summary>
	/// Called on mode change. This method is made to be assinged to a Unity event.
	/// </summary>
	public void OnModeChanged() {
		if (GameModeDropdown.value == 0) {
			PlayerInput = false;
			this.GenerationCountField.enabled = true;
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

	/// <summary>
	/// Called on the button "Import" click. This method is made to be assinged to a Unity event.
	/// </summary>
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
			ImportedGenotypes.Enqueue(result);
		}
		catch {
			this.ImportIdentificator.color = Color.red;
		}
	}

	/// <summary>
	/// Sets the game resolution.
	/// </summary>
	/// <param name="resolutionIndex">Index of the selected resolution from the Resolution dropdown menu.</param>
	public void ChangeResolution(int resolutionIndex) {
		Resolution resolution = this.resolutions[resolutionIndex];
		Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
	}

	/// <summary>
	/// Activates Full-screen mode.
	/// </summary>
	/// <param name="isFullScreen">Boolean that controlls the Full-screen mode.</param>
	public void SetFullScreenMode(bool isFullScreen) {
		Screen.fullScreen = isFullScreen;
	}

	/// <summary>
	/// Sets the game quality.
	/// </summary>
	/// <param name="qualityIndex">Index of the selected quality setting from the Quality dropdown menu.</param>
	public void ChangeQuality(int qualityIndex) {
		QualitySettings.SetQualityLevel(qualityIndex);
	}

	/// <summary>
	/// Changes the camera setting.
	/// </summary>
	/// <param name="cameraIndex">Index of the selected camera setting from the Camera dropdown menu.</param>
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

	/// <summary>
	/// Change the general sound volume of the sound mixer.
	/// </summary>
	/// <param name="volume">The new volume of the mixer.</param>
	public void ChangeVolume(float volume) {
		this.MainAudioMixer.SetFloat("Volume", volume);
	}
}
