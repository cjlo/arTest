using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Vuforia;

/// <summary>
/// A custom handler that implements the ITrackableEventHandler interface.
/// </summary>
public class MyTrackableEventHandler : MonoBehaviour,
                                            ITrackableEventHandler
{
    public MainController mainController;
    public GameObject AugmentedGO;
    public float Price = 0f;
    public string PromotionText = "";

    #region PRIVATE_MEMBER_VARIABLES

    private TrackableBehaviour mTrackableBehaviour;

    #endregion // PRIVATE_MEMBER_VARIABLES



    #region UNTIY_MONOBEHAVIOUR_METHODS

    void Start()
    {
        mTrackableBehaviour = GetComponent<TrackableBehaviour>();
        if (mTrackableBehaviour)
        {
            mTrackableBehaviour.RegisterTrackableEventHandler(this);
        }
    }

    #endregion // UNTIY_MONOBEHAVIOUR_METHODS



    #region PUBLIC_METHODS

    /// <summary>
    /// Implementation of the ITrackableEventHandler function called when the
    /// tracking state changes.
    /// </summary>
    public void OnTrackableStateChanged(
                                    TrackableBehaviour.Status previousStatus,
                                    TrackableBehaviour.Status newStatus)
    {
        if (newStatus == TrackableBehaviour.Status.DETECTED ||
            newStatus == TrackableBehaviour.Status.TRACKED ||
            newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
        {
            OnTrackingFound();
        }
        else
        {
            OnTrackingLost();
        }
    }

    #endregion // PUBLIC_METHODS



    #region PRIVATE_METHODS


    private void OnTrackingFound()
    {
        Renderer[] rendererComponents = GetComponentsInChildren<Renderer>(true);
        Collider[] colliderComponents = GetComponentsInChildren<Collider>(true);

        // Enable rendering:
        foreach (Renderer component in rendererComponents)
        {
            component.enabled = true;
        }

        // Enable colliders:
        foreach (Collider component in colliderComponents)
        {
            component.enabled = true;
        }

        Debug.Log("Trackable "+ mTrackableBehaviour.TrackableName + " found");
        mainController.SelectionChange(gameObject, AugmentedGO, Price, PromotionText);
    }


    private void OnTrackingLost()
    {
        Renderer[] rendererComponents = GetComponentsInChildren<Renderer>(true);
        Collider[] colliderComponents = GetComponentsInChildren<Collider>(true);

        // Disable rendering:
        foreach (Renderer component in rendererComponents)
        {
            component.enabled = false;
        }

        // Disable colliders:
        foreach (Collider component in colliderComponents)
        {
            component.enabled = false;
        }

        Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " lost");
        mainController.SelectionChange(null, null, 0f, "");
    }

    #endregion // PRIVATE_METHODS
}
