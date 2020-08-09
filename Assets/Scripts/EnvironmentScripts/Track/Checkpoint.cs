using UnityEngine;

/// <summary>
/// Represents a single checkpoint. It is a class that holds only different information about the checkpoint.
/// It has no methods except the Awake "constructor".
/// </summary>
public class Checkpoint : MonoBehaviour {
    public double CaptureRadius = 4;
    private SpriteRenderer spriteRenderer;
    public float DistanceFromLastOne { get; set; }
	public float DistanceFromStart { get; set; }
	public float ScoreValue { get; set; }
	public float ScoreTotal { get; set; }
	private void Awake() {
		this.spriteRenderer = GetComponent<SpriteRenderer>();
	}

	public bool IsVisible {
		get => spriteRenderer.enabled;
		set => spriteRenderer.enabled = value;
	}

	public Color Color {
		get => spriteRenderer.color;
		set => spriteRenderer.color = value;
	}

	public Sprite Sprite {
		get => spriteRenderer.sprite;
		set => spriteRenderer.sprite = value;
	}
}
