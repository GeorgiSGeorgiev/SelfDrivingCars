using UnityEngine;

/// <summary>
/// The Start Menu controller.
/// </summary>
public class StartMenu : MonoBehaviour {

	/// <summary>
	/// The next game scene.
	/// </summary>
	public string sceneToLoad = "TrackSelect";
	/// <summary>
	/// Gadget used for a fancy scene transition.
	/// </summary>
	public SceneFader sceneFader;

	/// <summary>
	/// On Play click method. Go to the "TrackSelect" scene.
	/// </summary>
	public void StartPlaying() {
		//SceneManager.LoadScene(sceneToLoad);
		sceneFader.FadeToScene(sceneToLoad);
	}

	/// <summary>
	/// Open settings menu.
	/// </summary>
	public void StartSettingsMenu() {
		//SceneManager.LoadScene(sceneToLoad);
		sceneFader.FadeToScene("MainMenuSettings");
	}

	/// <summary>
	/// Quit game.
	/// </summary>
	public void QuitGame() {
		//Debug.Log("QUIT");
		Application.Quit();
	}
}
