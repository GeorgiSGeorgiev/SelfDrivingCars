using UnityEngine;

/// <summary>
/// Car sensor. Detects differents obstacles and sends its readings to the car logic (to the Neural Network).
/// </summary>
public class Sensor : MonoBehaviour {
    public LayerMask DetectionLayer;

    public SpriteRenderer UsedSprite;
    [SerializeField]
    private float MinReadDistance = 0.03f;
    [SerializeField]
    private float MaxReadDistance = 15f;

    /// <summary>
    /// The sensor readings in the interval [MinReadDistance, MaxReadDistance].
    /// </summary>
    public float Readings { get; private set; }

    // Start is called before the first frame update
    void Start() {
        this.SetSpriteColor(Color.white);
        this.UsedSprite.gameObject.SetActive(true);
    }

    // FixedUpdate is called every fixed framerate frame.
    void FixedUpdate() {
        Vector3 direction = UsedSprite.transform.position - this.transform.position;
        direction.Normalize();

        RaycastHit2D detector = Physics2D.Raycast(this.transform.position, direction, MaxReadDistance, this.DetectionLayer);

        if (detector.collider == null) {
            detector.distance = this.MaxReadDistance;
		}
        else if (detector.distance < MinReadDistance) {
            detector.distance = MinReadDistance;
		}

        // set new distance
        this.Readings = detector.distance;

        if (this.Readings != this.MaxReadDistance) {
            this.SetSpriteColor(Color.red);
        }
        else {
            this.SetSpriteColor(Color.white);
        }
        UsedSprite.transform.position = this.transform.position + direction * this.Readings;
    }

    /// <summary>
    /// Change the color of the sensor sprite (of the end of the detector).
    /// </summary>
    /// <param name="color"></param>
    public void SetSpriteColor(Color color) {
        this.UsedSprite.color = color;
    }
}
