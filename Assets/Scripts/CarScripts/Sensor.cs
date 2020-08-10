using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sensor : MonoBehaviour {
    public LayerMask DetectionLayer;

    public SpriteRenderer UsedSprite;
    [SerializeField]
    private float MinReadDistance = 0.03f;
    [SerializeField]
    private float MaxReadDistance = 15f;

    /// <summary>
    /// The sensor readings as part of the MaxReadDistance.
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
            //Debug.Log("Hit");
        }
        else {
            this.SetSpriteColor(Color.white);
        }
        UsedSprite.transform.position = this.transform.position + direction * this.Readings;
    }

    public void SetSpriteColor(Color color) {
        this.UsedSprite.color = color;
    }
    public void HideSprite() {
        this.UsedSprite.gameObject.SetActive(false);
	}
    public void ShowSprite() {
        this.UsedSprite.gameObject.SetActive(true);
	}
}
