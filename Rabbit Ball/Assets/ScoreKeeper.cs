using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreKeeper : MonoBehaviour {
    public static ScoreKeeper keeper;
    private int carrotCount = 0;
    public Text carrotCountUi;
	// Use this for initialization
	void Start () {
        keeper = this;
        carrotCountUi.text = carrotCount.ToString();
	}
	
	// Update is called once per frame
	void Update () {
        
	}

    public void UpdateCarrotCount(int count) {
        carrotCount += count;
        carrotCountUi.text = carrotCount.ToString();
    }
}
