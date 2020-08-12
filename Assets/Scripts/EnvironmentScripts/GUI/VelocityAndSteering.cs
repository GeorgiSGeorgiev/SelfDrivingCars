using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VelocityAndSteering : MonoBehaviour
{
    //public GameObject Car;
    public CarPhysics CarPhysics;
    public Text VelocityText;
    public Text SteeringText;
    // Start is called before the first frame update
    void Start() {
        VelocityText.text = (CarPhysics.CurrentInput[0]*100).ToString();
        SteeringText.text = CarPhysics.CurrentInput[1].ToString();
    }

    // Update is called once per frame
    void Update() {
        VelocityText.text = (CarPhysics.CurrentInput[0] * 100).ToString();
        SteeringText.text = CarPhysics.CurrentInput[1].ToString();
    }
}
