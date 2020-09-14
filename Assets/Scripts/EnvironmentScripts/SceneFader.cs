using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;

/// <summary>
/// Class that allows smooth transition between scenes.
/// </summary>
public class SceneFader : MonoBehaviour {
	/// <summary>
	/// The background of the fader. To be referenced from the Unity editor.
	/// </summary>
	public Image image;
	
	/// <summary>
	/// Curve that controlls the style of the fading. Can be editted in the Unity editor.
	/// </summary>
	public AnimationCurve fadeCurve;

	/// <summary>
	/// Activates the <c>FadeIn</c> process at the beginning of the current scene.
	/// </summary>
	private void Start() {
		StartCoroutine( FadeIn() ); // via Coroutine the whole FadeIn() method isn't executed in one frame.
	}

	/// <summary>
	/// Changes the scenes and applies the Fade-Out effect to the scene transition.
	/// </summary>
	/// <param name="scene">The new scene we are changing to.</param>
	public void FadeToScene(string scene) {
		StartCoroutine(FadeOut(scene));
	}

	/// <summary>
	/// Activates the Fade-In process.
	/// </summary>
	/// <returns>Returns code 0 at the end of the frame.</returns>
	IEnumerator FadeIn () {
		float time = 1f;
		float speed = 1f;
		while (time > 0f) {
			time -= Time.deltaTime * speed; // decrease time, deltaTime is the time between frames
			float alpfa = fadeCurve.Evaluate(time); // apply the curve
			image.color = new Color(0f, 0f, 0f, alpfa); // modify the alfa channel
			yield return 0; // wait a frame and after that continue fading
		}
	}

	/// <summary>
	/// Activates the Fade-Out process.
	/// </summary>
	/// <param name="scene">The new scene we are changing to.</param>
	/// <returns>Returns code 0 at the end of the frame.</returns>
	IEnumerator FadeOut(string scene) {
		float time = 0f;
		float speed = 1f;
		while (time < 1f) {
			time += Time.deltaTime * speed; // decrease time, deltaTime is the time between frames
			float alpfa = fadeCurve.Evaluate(time); // apply the curve
			image.color = new Color(0f, 0f, 0f, alpfa); // modify the alfa channel
			yield return 0; // wait a frame and after that continue fading
		}

		SceneManager.LoadScene(scene);
	}
}
