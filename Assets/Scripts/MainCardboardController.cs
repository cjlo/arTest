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
		Transform offsetTransform = _foundGO.transform.FindChild ("offset");
		Debug.Assert (offsetTransform, "offset GO not found for " + _foundGO.name);
		Debug.Log ("Delta scale " + deltaScaleValue);
		float newScale = offsetTransform.localScale.x + (deltaScaleValue-1f) * scaleMultiplier;
		newScale = Mathf.Clamp (newScale, minScale, maxScale);
		Vector3 newScaleVec3;
		newScaleVec3.x = newScale;
		newScaleVec3.y = newScale;
		newScaleVec3.z = newScale;
		offsetTransform.localScale = newScaleVec3;

	}

	public void MoveCurrentCardboard( Vector2 screenSpacePos, Vector2 screenSpacePrevPos ){
		if (_foundGO == null) {
			return;
		}
		Transform offsetTransform = _foundGO.transform.FindChild ("offset");
		Debug.Assert (offsetTransform, "offset GO not found for " + _foundGO.name);
			
		Vector3 screenSpacePos3d;
		screenSpacePos3d.x = screenSpacePos.x;
		screenSpacePos3d.y = screenSpacePos.y;
		screenSpacePos3d.z = _foundGO.transform.localPosition.z;
		Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenSpacePos3d);

		Vector3 screenSpacePrevPos3d;
		screenSpacePrevPos3d.x = screenSpacePrevPos.x;
		screenSpacePrevPos3d.y = screenSpacePrevPos.y;
		screenSpacePrevPos3d.z = _foundGO.transform.localPosition.z;
		Vector3 worldPrevPos = Camera.main.ScreenToWorldPoint (screenSpacePrevPos3d);

		Vector3 delta3dPos = worldPos - worldPrevPos;

		Vector3 offsetTransformPos = offsetTransform.position;
		offsetTransform.position = offsetTransformPos + delta3dPos;
	}

	public void ApplyDefaultOffset(){
		if (_foundGO == null) {
			return;
		}
		Transform offsetTransform = _foundGO.transform.FindChild ("offset");
		Debug.Assert (offsetTransform, "offset GO not found for " + _foundGO.name);
		offsetTransform.localPosition = Vector3.zero;
		offsetTransform.localScale = Vector3.one;
	}

	public void ObjectFound(GameObject foundGO){
		_foundGO = foundGO;
		// TODO enable DefaultOffset button
	}

	public void ObjectLost(GameObject lostGO){
		_foundGO = null;
		// TODO disable DefaultOffset button
	}

	public void SnapScreenshot()
	{
		Debug.Log("screen capturing");
		StartCoroutine(CaptureScreen());

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

		if (latestFile == null)
		{
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
}
