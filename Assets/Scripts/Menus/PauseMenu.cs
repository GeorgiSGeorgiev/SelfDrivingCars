﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour {

    public static bool GameIsPaused = false;

    public GameObject PauseMenuUI;

    public GameObject SettingsMenuUI;
    public GameObject InfoPanel;

    public AudioMixer MainAudioMixer;
    public Slider SoundVolumeSlider;


    public TrackController MainTrackController;
    public InputField AgentNameField;

    public CameraSettings MainCamera;
    public Dropdown CameraModeDropdown;

    public Text InfoText;
    private string LastPathAndName;

    public static string clipBoard {
        get {
            return GUIUtility.systemCopyBuffer;
        }
        set {
            GUIUtility.systemCopyBuffer = value;
        }
    }

    public string MainMenuName;

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

	// Update is called once per frame
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

    public void OpenSettings() {
        // this has to be a new panel
        PauseMenuUI.SetActive(false);
        SettingsMenuUI.SetActive(true);
    }

    public void OpenMainMenu() {
        SceneManager.LoadScene(this.MainMenuName);
        GameIsPaused = false;
        Time.timeScale = 1f;
    }

    public void QuitGame() {
        Application.Quit();
	}

    public void BackToPauseMenu() {
        PauseMenuUI.SetActive(true);
        SettingsMenuUI.SetActive(false);
        //PauseMenuAnimator.gameObject.SetActive(true);
    }

    public void OnOkButtonClick() {
        SettingsMenuUI.SetActive(true);
        InfoPanel.SetActive(false);
	}

    public void CopyToClipboard() {
        PauseMenu.clipBoard = this.LastPathAndName;
	}

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

    public void ChangeVolume(float volume) {
        this.MainAudioMixer.SetFloat("Volume", volume);
    }
}
