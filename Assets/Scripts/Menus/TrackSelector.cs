using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// Manages the track selection and the preloaded genotypes control.
/// </summary>
public class TrackSelector : MonoBehaviour {
    /// <summary>
    /// Array that contains the names of all tracks.
    /// </summary>
    public string[] TrackSceneNames;
    /// <summary>
    /// Best agents for each track.
    /// </summary>
    public string[] BestAgentNames;

    public SceneFader sceneFader;

    /// <summary>
    /// Starts the requested track. If the selected game mode is Player vs AI then the best agent for the given track is selected and loaded to the game.
    /// </summary>
    /// <param name="trackName">The SCENE name of the selected track.</param>
    public void SelectTrack(string trackName) {
        // different validation checks
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
        // main part
        if (SettingsMenu.PlayerInput) {
            if (GameController.PreloadedGenotypes == null) {
                GameController.PreloadedGenotypes = new Queue<Genotype>();
            }
            else {
                GameController.PreloadedGenotypes.Clear();
            }
            // get the right track index
            int wantedIndex = 0;
            for (int i = 0; i < TrackSceneNames.Length; i++) {
                if (TrackSceneNames[i] == trackName) {
                    wantedIndex = i;
                    break;
                }
            }
            Genotype newGenotype = Genotype.LoadFromFile(Application.dataPath + "/PretrainedAgents", BestAgentNames[wantedIndex]);
            // set preloaded genotypes
            GameController.PreloadedGenotypes.Enqueue(newGenotype);
        }
        // change the scene to the requested track
        sceneFader.FadeToScene(trackName);
    }
}
