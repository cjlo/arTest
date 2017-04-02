using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TouchScript.Gestures;

public class GestureHandler : MonoBehaviour {

	public MainCardboardController cardboardController;
	public ScreenTransformGesture swipeGesture;
	public ScreenTransformGesture scaleGesture;

	// Use this for initialization
	private void OnEnable () {
		Debug.Log ("register on enable gesture");
		swipeGesture.Transformed += SwipeGestureTransformed;
		scaleGesture.Transformed += ScaleGestureTransformed;
	}

	private void OnDisable () {
		Debug.Log ("unregister on enable gesture");
		swipeGesture.Transformed -= SwipeGestureTransformed;
		scaleGesture.Transformed -= ScaleGestureTransformed;
	}

	public void SwipeGestureTransformed(object sender, System.EventArgs e){
//		Debug.Log ("swipe gesture transformed "+swipeGesture.ScreenPosition.x+", "+swipeGesture.ScreenPosition.y);
		cardboardController.MoveCurrentCardboard (swipeGesture.ScreenPosition, swipeGesture.PreviousScreenPosition);
		// if there is currently detected object, get orientation
		// swipe action screen space -> object space
		// object's recognized area proportional to move magnitude
	}

	public void ScaleGestureTransformed(object sender, System.EventArgs e){
//		Debug.Log ("scale gesture transformed");
		cardboardController.ScaleCurrentCardboard (scaleGesture.DeltaScale);
	}
}
