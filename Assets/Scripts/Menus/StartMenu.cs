using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour {

	public string sceneToLoad = "TrackSelect";
	public SceneFader sceneFader;
	public void StartPlaying() {
		//SceneManager.LoadScene(sceneToLoad);
		sceneFader.FadeToScene(sceneToLoad);
	}

	public void StartSettingsMenu() {
		//SceneManager.LoadScene(sceneToLoad);
		sceneFader.FadeToScene("MainMenuSettings");
	}

	public void QuitGame() {
		//Debug.Log("QUIT");
		Application.Quit();
	}

	public void Fade() {
	}
}
