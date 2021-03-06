﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Vuforia;

public class PosterTrackableEventHandler : MonoBehaviour, ITrackableEventHandler {

	public MainCardboardController mainCardboardController;
	private TrackableBehaviour mTrackableBehaviour;
	private DenoisedTransform _denoisedTransform;

	// Use this for initialization
	void Start () {
		mTrackableBehaviour = GetComponent<TrackableBehaviour> ();
		if (mTrackableBehaviour) {
			mTrackableBehaviour.RegisterTrackableEventHandler (this);
		}
		_denoisedTransform = GetComponent<DenoisedTransform> ();
	}

	public void OnTrackableStateChanged(TrackableBehaviour.Status previousStatus,
		TrackableBehaviour.Status newStatus ){
		if (newStatus == TrackableBehaviour.Status.DETECTED ||
			newStatus == TrackableBehaviour.Status.TRACKED ||
			newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
		{
			Debug.Log ("Found GO " + gameObject.name);
			if (_denoisedTransform) {
				_denoisedTransform.TrackingFound ();
			}
			OnTrackingFound();
		}
		else
		{
			Debug.Log ("Lost GO " + gameObject.name);
			if (_denoisedTransform) {
				_denoisedTransform.TrackingLost ();
			}
			OnTrackingLost();
		}
	}

	private void OnTrackingFound(){
		// TODO reset offset position, scale
//		Transform offsetGOTransform = transform.FindChild("offset");
//		if (offsetGOTransform) {
//			offsetGOTransform.gameObject.SetActive (true);
//			offsetGOTransform.localPosition = Vector3.zero;
//			offsetGOTransform.localScale = Vector3.one;
//		}
		mainCardboardController.ObjectFound (gameObject);
	}

	private void OnTrackingLost(){
//		Transform offsetGOTransform = transform.FindChild("offset");
//		if (offsetGOTransform) {
//			offsetGOTransform.gameObject.SetActive (false);
//		}
		mainCardboardController.ObjectLost (gameObject);
	}
}
