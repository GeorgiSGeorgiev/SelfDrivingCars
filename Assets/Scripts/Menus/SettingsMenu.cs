using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour {
	public SceneFader SceneFader;
	public void GoBackTomainMenu() {
		SceneFader.FadeToScene("MainMenu");
		//Debug.Log("QUIT");
	}
}
