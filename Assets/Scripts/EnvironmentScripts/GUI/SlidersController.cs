using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlidersController: MonoBehaviour {
    public Image LeftSlideBackground;
    public Image LeftHandle;
    public Slider LeftSlider;
    public Image RightSlideBackground;
    public Image RightHandle;
    public Slider RightSlider;

    public void SetParameters(float max) {

        LeftSlider.minValue = 0;
        LeftSlider.maxValue = max;
        RightSlider.minValue = 0;
        RightSlider.maxValue = max;
        LeftSlideBackground.color = Color.red;
        RightSlideBackground.color = Color.blue;
	}

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
