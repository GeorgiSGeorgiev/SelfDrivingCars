using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
/// <summary>
/// Class that represents the pause menu which contains different settings and the genotype export option.
/// </summary>
public class PauseMenu : MonoBehaviour {

    public static bool GameIsPaused = false;

    /// <summary>
    /// The pause menu panel.
    /// </summary>
    public GameObject PauseMenuUI;

    /// <summary>
    /// The pause menu settings panel.
    /// </summary>
    public GameObject SettingsMenuUI;
    /// <summary>
    /// The pause menu information panel.
    /// </summary>
    public GameObject InfoPanel;

    public AudioMixer MainAudioMixer;
    public Slider SoundVolumeSlider;


    public TrackController MainTrackController;
    public InputField AgentNameField;

    public CameraSettings MainCamera;
    public Dropdown CameraModeDropdown;

    public Text InfoText;
    private string LastPathAndName;

    /// <summary>
    /// A static property which represents the Windows clipboard.
    /// </summary>
    public static string clipBoard {
        get {
            return GUIUtility.systemCopyBuffer;
        }
        set {
            GUIUtility.systemCopyBuffer = value;
        }
    }

    /// <summary>
    /// The name of the main menu scene.
    /// </summary>
    public string MainMenuName;

    // Get the current volume and camera settings.
	private void Start() {
        float currentVolume = 0;
        MainAudioMixer.GetFloat("Volume", out currentVolume);
        this.SoundVolumeSlider.value = currentVolume;

        switch (SettingsMenu.CurrentCameraMode) {
            case CameraMode.Follow:
                CameraModeDropdown.value = 0;
                break;
            case CameraMode.FollowAndRotate:
                CameraModeDropdown.value = 1;
                break;
		}
        CameraModeDropdown.RefreshShownValue();
    }

	// Update is called once per frame.
    // Track the Escape key press.
	void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (GameIsPaused) {
                ResumeGame();
			}
            else {
                PauseGame();
			}
		}
    }

    /// <summary>
    /// Turn off the pause menu and resume the game.
    /// </summary>
    public void ResumeGame() {
        GameIsPaused = false;
        PauseMenuUI.SetActive(false);
        SettingsMenuUI.SetActive(false);
        Time.timeScale = 1f; // set time to normal
	}

    void PauseGame() {
        GameIsPaused = true;
        PauseMenuUI.SetActive(true);
        Time.timeScale = 0f; // freeze game
	}

    /// <summary>
    /// Activate the settings panel in the pause menu.
    /// </summary>
    public void OpenSettings() {
        // this has to be a new panel
        PauseMenuUI.SetActive(false);
        SettingsMenuUI.SetActive(true);
    }

    /// <summary>
    /// Return to the main menu.
    /// </summary>
    public void OpenMainMenu() {
        SceneManager.LoadScene(this.MainMenuName);
        GameIsPaused = false;
        Time.timeScale = 1f;
    }

    /// <summary>
    /// Quit the game.
    /// </summary>
    public void QuitGame() {
        Application.Quit();
	}

    /// <summary>
    /// Deactivate the settings panel and activate the main pause menu panel.
    /// </summary>
    public void BackToPauseMenu() {
        PauseMenuUI.SetActive(true);
        SettingsMenuUI.SetActive(false);
        //PauseMenuAnimator.gameObject.SetActive(true);
    }

    /// <summary>
    /// Deactivate the info panel and activate the pause menu settings panel.
    /// </summary>
    public void OnOkButtonClick() {
        SettingsMenuUI.SetActive(true);
        InfoPanel.SetActive(false);
	}

    /// <summary>
    /// Copy text to the (OS) clipboard.
    /// </summary>
    public void CopyToClipboard() {
        PauseMenu.clipBoard = this.LastPathAndName;
	}

    /// <summary>
    /// Export the best genotype and activate the info panel.
    /// </summary>
    public void ExportTheBest() {
        if (this.AgentNameField.text == null || this.AgentNameField.text == "") {
            MainTrackController.ExportTheBestGenotype();
            //Debug.Log("No name");
            this.LastPathAndName = Genotype.DefaultPathAndName;
        }
        else {
            MainTrackController.ExportTheBestGenotype(this.AgentNameField.text);
            this.LastPathAndName = Genotype.LastSavedTo;
        }
        SettingsMenuUI.SetActive(false);
        InfoPanel.SetActive(true);
        this.InfoText.text = $"The agent's genotype serialized and successfully exported to: \n { this.LastPathAndName }";
    }

    /// <summary>
    /// Change the camera mode.
    /// </summary>
    /// <param name="cameraIndex">The camera dropdown index.</param>
    public void ChangeCamera(int cameraIndex) {
        switch (cameraIndex) {
            case 0:
                SettingsMenu.CurrentCameraMode = CameraMode.Follow;
                break;
            case 1:
                SettingsMenu.CurrentCameraMode = CameraMode.FollowAndRotate;
                break;
        }
        this.MainCamera.cameraMode = SettingsMenu.CurrentCameraMode;
    }

    /// <summary>
    /// Change the mixer volume.
    /// </summary>
    /// <param name="volume">The volum's value.</param>
    public void ChangeVolume(float volume) {
        this.MainAudioMixer.SetFloat("Volume", volume);
    }
}
