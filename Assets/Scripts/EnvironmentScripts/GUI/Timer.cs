using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The main game timer.
/// </summary>
public class Timer : MonoBehaviour {
    /// <summary>
    /// The timer's text box in the game's UI. To be referenced from the Unity editor.
    /// </summary>
    public Text TimeBox;
    private float TimeTracker;

    // Start is called before the first frame update
    void Start() {
        TimeTracker = 0;
    }

    // Update is called once per frame
    void Update() {
        TimeTracker += Time.deltaTime;
        TimeBox.text = TimeTracker.ToString();
        //Debug.Log($"{ TimeTracker }");
    }

    private void Reset() {
        TimeBox.text = "0";
    }
}
