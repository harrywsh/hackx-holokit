using System.Collections;
using System.Collections.Generic;
using Finch;
using UnityEngine;
public class funtrail : MonoBehaviour {

	// Use this for initialization
	void Start () {

	}
	public TrailRenderer kk;
	public FinchChirality Chirality;

	void Update () {
		FinchController controller = FinchVR.GetFinchController (Chirality);

		if (Input.GetKeyDown (KeyCode.A) || controller.GetPressDown (FinchControllerElement.ButtonThumb)) {
			kk.enabled = false;
		}
	}

}