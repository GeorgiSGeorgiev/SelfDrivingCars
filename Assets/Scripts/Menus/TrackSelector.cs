using UnityEngine;
using System.Collections.Generic;
using System;

public class TrackSelector : MonoBehaviour {
    public string[] TrackSceneNames;
    public string[] BestAgentNames;

    public SceneFader sceneFader;

    /// <summary>
    /// Starts the requested track. If the selected game mode is Player vs AI then the best agent for the given track is selected and loaded to the game.
    /// </summary>
    /// <param name="trackName">The SCENE name of the selected track.</param>
    public void SelectTrack(string trackName) {
        if (TrackSceneNames == null || TrackSceneNames.Length == 0 || BestAgentNames == null || BestAgentNames.Length == 0) {
            throw new ArgumentException("No tracks or best agents were added to the TrackSelector.");
		}
        if (TrackSceneNames.Length != BestAgentNames.Length) {
            throw new ArgumentException("TrackSceneNames and BestAgentNames must have the same length.");
        }
		for (int i = 0; i < TrackSceneNames.Length; i++) {
            if (TrackSceneNames[i] == null || BestAgentNames[i] == null || TrackSceneNames[i] == "" || BestAgentNames[i] == "") {
                throw new ArgumentException("Null or empty Name string.");
			}
		}
        if (SettingsMenu.PlayerInput) {
            if (GameController.PreloadedGenotypes == null) {
                GameController.PreloadedGenotypes = new Queue<Genotype>();
            }
            else {
                GameController.PreloadedGenotypes.Clear();
            }
            int wantedIndex = 0;
            for (int i = 0; i < TrackSceneNames.Length; i++) {
                if (TrackSceneNames[i] == trackName) {
                    wantedIndex = i;
                    break;
                }
            }
            Genotype newGenotype = Genotype.LoadFromFile(Application.dataPath + "/PretrainedAgents", BestAgentNames[wantedIndex]);
            GameController.PreloadedGenotypes.Enqueue(newGenotype);
        }
        sceneFader.FadeToScene(trackName);
    }
}
