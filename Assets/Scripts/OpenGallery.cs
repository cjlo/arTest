using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class OpenGallery : MonoBehaviour {

    public RawImage rawImage;
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
		Texture2D t = new Texture2D(128, 128);
		(new WWW(fileURL)).LoadImageIntoTexture(t);
		_rawImageMaterial.mainTexture = t;
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
