using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Finch;
public class FinchFunTrail : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// // Update is called once per frame
	// void Update () {
		
	// }




	public TrailRenderer kk;
	public FinchChirality Chirality;

	void Update()
	{
		FinchController controller = FinchVR.GetFinchController(Chirality);

		if (Input.GetKeyDown(KeyCode.A) || controller.GetPressDown(FinchControllerElement.ButtonThumb))
		{
			kk.enabled =false;
		}
	}


}
