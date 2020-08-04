using UnityEngine;

public class TrackSelector : MonoBehaviour {
	public SceneFader sceneFader;
	public void SelectTrack(string trackName) {
		sceneFader.FadeToScene(trackName);
	}
}
