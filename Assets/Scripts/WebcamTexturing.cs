using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WebcamTexturing : MonoBehaviour {

	public RawImage rawImage;

	// Use this for initialization
	void Start () {
		WebCamTexture webcamTex = new WebCamTexture ();
		rawImage.texture = webcamTex;
		rawImage.material.mainTexture = webcamTex;
		webcamTex.Play ();
	}
	
	// Update is called once per frame
//	void Update () {
//		
//	}
//
//	void OnRenderImage (RenderTexture source, RenderTexture destination){
//
//	}
}
