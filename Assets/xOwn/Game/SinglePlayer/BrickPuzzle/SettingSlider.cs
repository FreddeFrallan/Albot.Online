using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingSlider : MonoBehaviour {

    [SerializeField]
	private TextMeshProUGUI sliderValueText;
    [SerializeField]
    public Slider slider;

    // Use this for initialization
    void Start() {
        this.updateValue();
    }

    public void updateValue() {
        this.sliderValueText.text = this.slider.value.ToString();
        PuzzleGameMaster.slideSpeed = this.slider.value;
    }
}
