using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;
using UnityEngine.UI;
using System.IO;

public class MainCardboardController : MonoBehaviour {

	public Button defaultOffsetButton;
	public float maxScale = 2f;
	public float minScale = 0.5f;
	public float scaleMultiplier = 1f;
	public OpenGallery openGallery;
	public Canvas canvas;
	public GameObject buttonPanelGO;
	public GameObject loadingGO;

	public Slider rotationBufferSizeSlider;
	public Text rotationBufferSizeText;

	private GameObject _foundGO = null;
//
//	void Start(){
//		defaultOffsetButton.enabled = false;
//	}


	// Update is called once per frame
	void Update () {
		
		 CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO);

//		foreach (GameObject  go in _foundGOList) {
//			Vector3 pos = go.transform.position;
//			Vector3 euler = go.transform.rotation.eulerAngles;
//			Vector3 screenPoint = Camera.main.WorldToScreenPoint (go.transform.position);
//
//			// screen to world point (i.e. camera space), given z 
//			Debug.Log ("GO " + go.name + " at " + pos.x + ", " +
//				pos.y + ", " + pos.z + " rot "+euler.x +", "+euler.y+", "+euler.z);
//		}
		if (Input.GetKeyDown (KeyCode.Escape)) {
			Application.Quit ();
		}
	}

	public void ScaleCurrentCardboard( float deltaScaleValue ){
		if (_foundGO == null) {
			return;
		}
		DenoisedTransform dt = _foundGO.GetComponent<DenoisedTransform> ();
		dt.ScaleOffset (deltaScaleValue, minScale, maxScale, scaleMultiplier);
	}

	public void MoveCurrentCardboard( Vector2 screenSpacePos, Vector2 screenSpacePrevPos ){
		if (_foundGO == null) {
			return;
		}
		DenoisedTransform dt = _foundGO.GetComponent<DenoisedTransform> ();
		dt.MoveOffset (screenSpacePos, screenSpacePrevPos);
	}

	public void ApplyDefaultOffset(){
		if (_foundGO == null) {
			return;
		}
		DenoisedTransform dt = _foundGO.GetComponent<DenoisedTransform> ();
		dt.ApplyDefaultOffset ();

	}

	public void ObjectFound(GameObject foundGO){
		_foundGO = foundGO;

		DenoisedTransform dt = _foundGO.GetComponent<DenoisedTransform> ();
		rotationBufferSizeSlider.gameObject.SetActive (true);
		rotationBufferSizeText.gameObject.SetActive (true);
		rotationBufferSizeSlider.value = dt.rotationBufferSize;
		rotationBufferSizeText.text = ""+dt.rotationBufferSize;
		// TODO enable DefaultOffset button
	}

	public void ObjectLost(GameObject lostGO){
		_foundGO = null;
		rotationBufferSizeSlider.gameObject.SetActive (false);
		rotationBufferSizeText.gameObject.SetActive (false);
		rotationBufferSizeText.text = "";
		// TODO disable DefaultOffset button
	}

	public void SnapScreenshot()
	{
		Debug.Log("screen capturing");
		StartCoroutine(CaptureScreen());
//		CameraDevice.CameraDeviceMode

	}

	private IEnumerator CaptureScreen()
	{
		Debug.Assert (loadingGO, "LoadingGO is not set in " + gameObject.name);
		yield return null;
		canvas.enabled = false;
		yield return new WaitForEndOfFrame();
		string nowStr = System.DateTime.Now.ToString("yyyy-MM-dd.HH.mm.ss.ffff");
		string screenshotName = "Screenshot" + nowStr + ".png";
		string screenshotPath = screenshotName;
#if UNITY_EDITOR
		screenshotPath = Application.persistentDataPath + "/" + screenshotPath;
#endif
		Application.CaptureScreenshot(screenshotPath);
		yield return null;
		canvas.enabled = true;

		buttonPanelGO.SetActive (false);

		loadingGO.SetActive (true);
		while (getLatestFile() != screenshotName){
			yield return null;
		}
		loadingGO.SetActive (false);
		
		Debug.Log("captured to "+screenshotPath);
		RefreshGalleryIcon();
		buttonPanelGO.SetActive (true);
	}

	public string getLatestFile(){

		DirectoryInfo directoryInfo = new DirectoryInfo(Application.persistentDataPath);
		FileInfo latestFile = null;
		FileInfo[] fileList = directoryInfo.GetFiles();
		for (int i = 0; i < fileList.Length; i++)
		{
			if (latestFile == null || fileList[i].LastWriteTime > latestFile.LastWriteTime)
			{
				latestFile = fileList[i];
			}
		}

		if (latestFile == null) {
			return "";
		}
		return latestFile.Name;
	}

	public void RefreshGalleryIcon()
	{
		Debug.Assert (openGallery, "OpenGallery comp not assigned in "+ gameObject.name);
		openGallery.RefreshGalleryIcon();
	}

	public void OnApplicationPause(bool isPaused){
		Debug.Assert (openGallery, "OpenGallery comp not assigned in " + gameObject.name);
		if (!isPaused) {
			openGallery.RefreshGalleryIcon ();
		}
	}

	public void OnRotationBufferSizeChange( float newValue ){
		if (!_foundGO) {
			return;
		}
		int newValueInt = (int)newValue;
		rotationBufferSizeText.text = ""+newValueInt;
		DenoisedTransform dt = _foundGO.GetComponent<DenoisedTransform> ();
		dt.rotationBufferSize = newValueInt;

	}
}
