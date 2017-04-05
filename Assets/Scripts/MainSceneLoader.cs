using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainSceneLoader : MonoBehaviour {
	public Image progressImage;
	// Use this for initialization
	void Start () {
		StartCoroutine (loadScene ());
	}

	public IEnumerator loadScene(){
		AsyncOperation ao = SceneManager.LoadSceneAsync(1);
		//ao.allowSceneActivation = false;
		while (!ao.isDone) {
			progressImage.fillAmount = ao.progress;
			yield return null;
		}
	}

	void Update () {
		if (Input.GetKeyDown (KeyCode.Escape)) {
			Application.Quit ();
		}
	}

}
