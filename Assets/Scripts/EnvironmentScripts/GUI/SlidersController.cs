using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Custom made class that combines two different sliders to create a fancy double-sided speedometer.
/// </summary>
public class SlidersController: MonoBehaviour {
    public Image LeftSlideBackground;
    public Image LeftHandle;
    public Slider LeftSlider;
    public Image RightSlideBackground;
    public Image RightHandle;
    public Slider RightSlider;

    /// <summary>
    /// Sets the maximal velocity of the two sliders.
    /// </summary>
    /// <param name="max">The maximal velocity to be set. Both sliders have the same <c>max</c> value.</param>
    public void SetParameters(float max) {

        LeftSlider.minValue = 0;
        LeftSlider.maxValue = max;
        RightSlider.minValue = 0;
        RightSlider.maxValue = max;
        LeftSlideBackground.color = Color.red;
        RightSlideBackground.color = Color.blue;
	}

    /// <summary>
    /// Sets the current car velocity on the sliders.
    /// </summary>
    /// <param name="value">The velocity to be shown on the sliders.</param>
    public void SetValue(float value) {
        if (value <= 0) {
            if (!LeftSlideBackground.enabled) {
                LeftSlideBackground.enabled = true;
                LeftHandle.enabled = true;
            }
            if (RightSlideBackground.enabled) {
                RightSlideBackground.enabled = false;
                RightHandle.enabled = false;
			}
            if (!LeftSlider.enabled) {
                LeftSlider.enabled = true;
			}
            if (RightSlider.enabled) {
                RightSlider.enabled = false;
			}
            LeftSlider.value = Math.Abs(value);
		}
        else {
            if (LeftSlideBackground.enabled) {
                LeftSlideBackground.enabled = false;
                LeftHandle.enabled = false;
            }
            if (!RightSlideBackground.enabled) {
                RightSlideBackground.enabled = true;
                RightHandle.enabled = true;
            }
            if (LeftSlider.enabled) {
                LeftSlider.enabled = false;
            }
            if (!RightSlider.enabled) {
                RightSlider.enabled = true;
            }
            RightSlider.value = value;
        }
	}
}
