using UnityEngine;

/// <summary>
/// Represents a single checkpoint. It is a class that holds only different information about the checkpoint.
/// It has no methods except the Awake "constructor".
/// </summary>
public class Checkpoint : MonoBehaviour {
	/// <summary>
	/// The default radius from which the checkpoint can be captured. This value can be changed from the Unity editor.
	/// </summary>
    public double CaptureRadius = 4;
    private SpriteRenderer spriteRenderer;
	/// <summary>
	/// Distance between this checkpoint and the last one.
	/// </summary>
    public float DistanceFromLastOne { get; set; }
	/// <summary>
	/// Distance from the first checkpoint on the track.
	/// </summary>
	public float DistanceFromStart { get; set; }
	/// <summary>
	/// The score value of this checkpoint.
	/// </summary>
	public float ScoreValue { get; set; }
	/// <summary>
	/// The maximal score which can be obtained untill this moment.
	/// </summary>
	public float ScoreTotal { get; set; }
	/// <summary>
	/// Gets the model (sprite/picture) of the checkpoint.
	/// </summary>
	private void Awake() {
		this.spriteRenderer = GetComponent<SpriteRenderer>();
	}

	/// <summary>
	/// Contains information about the checkpoint's visibility.
	/// </summary>
	public bool IsVisible {
		get => spriteRenderer.enabled;
		set => spriteRenderer.enabled = value;
	}

	/// <summary>
	/// The color of the checkpoint's sprite.
	/// </summary>
	public Color Color {
		get => spriteRenderer.color;
		set => spriteRenderer.color = value;
	}

	/// <summary>
	/// The sprite of the checkpoint.
	/// </summary>
	public Sprite Sprite {
		get => spriteRenderer.sprite;
		set => spriteRenderer.sprite = value;
	}
}
