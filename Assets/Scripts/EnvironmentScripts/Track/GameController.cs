using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Singleton class managing the overall simulation.
/// </summary>
public class GameController : MonoBehaviour {
    #region Members
    // The camera object, to be referenced in Unity Editor.
    [SerializeField]
    private CameraSettings Camera;

    public static GameController Instance {
        get;
        private set;
    }
    #endregion

    #region Constructors
    private void Awake() {
        if (Instance != null) {
            Debug.LogError("Multiple GameStateManagers in the Scene.");
            return;
        }
        Instance = this;
    }

    void Start() {
        TrackController.TCInstance.WinningCarHasChanged += OnBestCarChanged;
        GeneticsController.Instance.StartGeneticAlg();
    }
    #endregion

    #region Methods
    // Callback method for when the best car has changed.
    private void OnBestCarChanged(CarController bestCar) {
        if (bestCar == null)
            Camera.Target = null;
        else
            Camera.Target = bestCar.gameObject.transform;
    }
    #endregion
}