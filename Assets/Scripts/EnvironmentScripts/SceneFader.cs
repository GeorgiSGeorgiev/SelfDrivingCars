using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;

public class SceneFader : MonoBehaviour
{
	public Image image;
	public AnimationCurve fadeCurve;

	private void Start() {
		StartCoroutine( FadeIn() ); // via Coroutine the whole FadeIn() method isn't executed in one frame.
	}

	public void FadeToScene(string scene) {
		StartCoroutine(FadeOut(scene));
	}

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
