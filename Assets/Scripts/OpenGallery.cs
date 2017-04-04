using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class OpenGallery : MonoBehaviour {

    public RawImage rawImage;
	public Text galleryRefreshedText;

	public List<Texture> texList = new List<Texture>();
	private int texId=0;

	private MainController _mainController;
	private Material _rawImageMaterial;
	private string _latestFilePath;

    // Use this for initialization
    void Start () {
		_rawImageMaterial = rawImage.material;
		RefreshGalleryIcon();
	}
	

	public void SetMainController(MainController mainController)
	{
		_mainController = mainController;
	}
		

	public void RefreshGalleryIcon()
    {

		rawImage.gameObject.SetActive(false);
		_latestFilePath = "";
		DirectoryInfo directoryInfo = new DirectoryInfo(Application.persistentDataPath);
		FileInfo latestFile = null;
		FileInfo[] fileList = directoryInfo.GetFiles();
		for (int i = 0; i < fileList.Length; i++)
		{
			Debug.Log("File " + i + " " + fileList[i].FullName);
			if (latestFile == null || fileList[i].LastWriteTime > latestFile.LastWriteTime)
			{
				latestFile = fileList[i];
			}
		}

		if (latestFile == null)
		{
			Debug.Log("no latest file found");
			return;
		}

		Debug.Log("Latest file " + latestFile.FullName);
		rawImage.gameObject.SetActive(true);
		_latestFilePath = latestFile.FullName;

		string fileURL = @"file://" + _latestFilePath;
		WWW www = new WWW (fileURL);
//		www.text
//				www.LoadImageIntoTexture();
		if (!_rawImageMaterial) {
			return;
		}
		_rawImageMaterial.mainTexture = www.texture;


		int w = _rawImageMaterial.mainTexture.width;
		int h = _rawImageMaterial.mainTexture.height;
		galleryRefreshedText.text = ""+w+"x"+h;
		Vector2 mainTexOffset;
		Vector2 mainTexScale;



//		float ratio = 1f;
		if (w > h) {
//			ratio = (w * 1f) / (h * 1f);
//			float extraPerSide = 0.5f * (ratio - 1f);
//			Rect newRect = new Rect();
//			newRect.x = extraPerSide;
//			newRect.width = (ratio - 1f) / ratio;
//			newRect.y = 0f;
//			newRect.height = 1f;
//			rawImage.uvRect = newRect;
			mainTexScale.x = (h * 1f) / (w*1f);
			mainTexScale.y = 1f;
			mainTexOffset.x = 0.5f * (1f - mainTexScale.x);
			mainTexOffset.y = 0f;
		} else {
			mainTexScale.x = 1f;
			mainTexScale.y = (w * 1f) / (h * 1f);
			mainTexOffset.x = 0f;
			mainTexOffset.y = 0.5f * (1f - mainTexScale.y);
//			ratio = (h * 1f) / (w * 1f);
//			float extraPerSide = 0.5f * (ratio - 1f);
//			Rect newRect = new Rect();
//			newRect.x = 0;
//			newRect.width = 1f;
//			newRect.y = extraPerSide;
//			newRect.height = (ratio - 1f) / ratio;
//
//			rawImage.uvRect = newRect;
//
		}
		_rawImageMaterial.mainTextureOffset = mainTexOffset;
		_rawImageMaterial.mainTextureScale = mainTexScale;
	}

	private IEnumerator updateTexture(string fileURL){

		galleryRefreshedText.gameObject.SetActive (true);
		yield return new WaitForSeconds (2f);
		galleryRefreshedText.gameObject.SetActive (false);
	}


    public void OpenAndroidGallery()
    {
		if (_latestFilePath.Length == 0)
		{
			return;
		}
		Application.OpenURL(_latestFilePath);
    }
}




