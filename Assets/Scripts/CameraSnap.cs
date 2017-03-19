using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;


public class CameraSnap : MonoBehaviour {
    public Canvas canvas;

    private MainController _mainController;
	

    public void SetMainController(MainController mainController)
    {
        _mainController = mainController;
    }

    public void onClick()
    {
        Debug.Log("capturing");
        StartCoroutine(CaptureScreen());

    }

    private IEnumerator CaptureScreen()
    {
        yield return null;
        canvas.enabled = false;
        yield return new WaitForEndOfFrame();
		string nowStr = System.DateTime.Now.ToString("yyyy-MM-dd.HH.mm.ss.ffff");
		string screenshotPath = "Screenshot" + nowStr + ".png";
#if UNITY_EDITOR
		screenshotPath = Application.persistentDataPath + "/" + screenshotPath;
#endif
		Application.CaptureScreenshot(screenshotPath);
		yield return null;
        canvas.enabled = true;
		Debug.Log("captured");
        _mainController.RefreshGalleryIcon();
    }
}
