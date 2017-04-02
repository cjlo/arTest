using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


using Vuforia;

public class MainController : MonoBehaviour
{
    public CameraSnap cameraSnap;
    public OpenGallery openGallery;

    public Text CurrentStatusText = null;
    public Text PriceText = null;
    public Text PromotionText = null;

    public float ScaleMin = 0.4f;
    public float ScaleMax = 0.5f;
    public float ScalePeriodSecond = 1f;
    private GameObject _CurrentTargetGO = null;
    private GameObject _CurrentAugmentedGO = null;
    public bool Scaling = true;
    private float _TimeElapsed = 0f;
    private bool _focusModeSet = false;


    void Start()
    {
        CurrentStatusText.text = "";
        cameraSnap.SetMainController(this);
		openGallery.SetMainController(this);
    }

    public void SetScaling( bool value)
    {
        Scaling = value;
    }

    public void RefreshGalleryIcon()
    {
		openGallery.RefreshGalleryIcon();
    }
    
    // Update is called once per frame
    void Update()
    {
		if (true)//x!_focusModeSet)
        {
            _focusModeSet = CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO);
        }

        if (Scaling)
        {
            _TimeElapsed += Time.deltaTime;
            float phase = Mathf.Cos( Mathf.Deg2Rad * 360 * _TimeElapsed / ScalePeriodSecond);
            float scale = Mathf.Lerp(ScaleMin, ScaleMax, phase);
            if (_CurrentAugmentedGO != null)
            {
                Vector3 newScale;
                newScale.x = scale;
                newScale.y = scale;
                newScale.z = scale;
                _CurrentAugmentedGO.transform.localScale = newScale;
            }
        }
    }

    public void SelectionChange( GameObject targetGO, GameObject augmentedGO, float price, string promotionText )
    {
        if (targetGO == null)
        {
            if (_CurrentTargetGO != null) { 
                CurrentStatusText.text = _CurrentTargetGO.name + " lost";
            }
            PriceText.text = "";
            PromotionText.text = "";
            _CurrentAugmentedGO = augmentedGO;
        }
        else
        {
            CurrentStatusText.text = targetGO.name + " found";
            PriceText.text = ""+price;
            PromotionText.text = promotionText;
            _CurrentAugmentedGO = augmentedGO;
        }
        _CurrentTargetGO = targetGO;

    }
}