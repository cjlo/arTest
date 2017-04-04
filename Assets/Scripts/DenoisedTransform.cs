using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DenoisedTransform : MonoBehaviour {

	private GameObject _dampedGO;
	private List<Quaternion> _rotationBuffer = new List<Quaternion> ();
	private int _rotationBufferID = 0;
	public int rotationBufferSize = 20;
	private int _appliedRotationBufferSize;

	private Quaternion _resultQuaternion = new Quaternion();
	private Vector4 _cumQuaternion = new Vector4 ();


	// Use this for initialization
	void Start () {
		Transform offsetChild = transform.FindChild ("offset");
		_dampedGO = new GameObject (gameObject.name + "damped");
		_dampedGO.transform.position = transform.position;
		_dampedGO.transform.rotation = transform.rotation;
		_dampedGO.transform.localScale = transform.localScale;
		offsetChild.parent = _dampedGO.transform;

		for (int i = 0; i < rotationBufferSize; i++) {
			_rotationBuffer.Add (Quaternion.identity);
		}
		_appliedRotationBufferSize = rotationBufferSize;
		_dampedGO.SetActive (false);
	}

	void OnDisable(){
		Destroy (_dampedGO);
	}

	public void ScaleOffset(float deltaScaleValue, float minScale, float maxScale, float scaleMultiplier){
		Transform offsetTransform = _dampedGO.transform.FindChild ("offset");
		Debug.Assert (offsetTransform, "offset GO not found for " + _dampedGO.name);
		Debug.Log ("Delta scale " + deltaScaleValue);
		float newScale = offsetTransform.localScale.x + (deltaScaleValue-1f) * scaleMultiplier;
		newScale = Mathf.Clamp (newScale, minScale, maxScale);
		Vector3 newScaleVec3;
		newScaleVec3.x = newScale;
		newScaleVec3.y = newScale;
		newScaleVec3.z = newScale;
		offsetTransform.localScale = newScaleVec3;
	}

	public void MoveOffset(Vector2 screenSpacePos, Vector2 screenSpacePrevPos){
		Transform offsetTransform = _dampedGO.transform.FindChild ("offset");
		Debug.Assert (offsetTransform, "offset GO not found for " + _dampedGO.name);

		Vector3 screenSpacePos3d;
		screenSpacePos3d.x = screenSpacePos.x;
		screenSpacePos3d.y = screenSpacePos.y;
		screenSpacePos3d.z = _dampedGO.transform.localPosition.z;
		Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenSpacePos3d);

		Vector3 screenSpacePrevPos3d;
		screenSpacePrevPos3d.x = screenSpacePrevPos.x;
		screenSpacePrevPos3d.y = screenSpacePrevPos.y;
		screenSpacePrevPos3d.z = _dampedGO.transform.localPosition.z;
		Vector3 worldPrevPos = Camera.main.ScreenToWorldPoint (screenSpacePrevPos3d);

		Vector3 delta3dPos = worldPos - worldPrevPos;

		Vector3 offsetTransformPos = offsetTransform.position;
		offsetTransform.position = offsetTransformPos + delta3dPos;
	}

	public void ApplyDefaultOffset(){
		Transform offsetTransform = _dampedGO.transform.FindChild ("offset");
		Debug.Assert (offsetTransform, "offset GO not found for " + _dampedGO.name);
		offsetTransform.localPosition = Vector3.zero;
		offsetTransform.localScale = Vector3.one;
	}
	
	// Update is called once per frame
	void Update () {
		if (_appliedRotationBufferSize != rotationBufferSize) {
			_rotationBuffer.Clear ();
			for (int i = 0; i < rotationBufferSize; i++) {
				_rotationBuffer.Add (transform.rotation);
			}
			_appliedRotationBufferSize = rotationBufferSize;
			_rotationBufferID = 0;
		}

		_rotationBuffer [_rotationBufferID] = transform.rotation;
		_rotationBufferID += 1;
		if (_rotationBufferID >= _rotationBuffer.Count)
			_rotationBufferID = 0;

		_cumQuaternion = Vector4.zero;
		_resultQuaternion = transform.rotation;
		for (int i = 1; i < _rotationBuffer.Count; i++) {
			_resultQuaternion = AverageQuaternion (ref _cumQuaternion, _rotationBuffer [i], _rotationBuffer [0], i);
//			avgQuaternion += _rotationBuffer [i];
		}
//		avgQuaternion /= _rotationBuffer.Count * 1f;

		_dampedGO.transform.position = transform.position;
		_dampedGO.transform.rotation = _resultQuaternion;
//		_dampedGO.transform.rotation = transform.rotation;
		_dampedGO.transform.localScale = transform.localScale;
	}

	public void TrackingFound(){
		_dampedGO.SetActive (true);
		Transform offsetGOTransform = _dampedGO.transform.FindChild("offset");
		if (offsetGOTransform) {
			offsetGOTransform.localPosition = Vector3.zero;
			offsetGOTransform.localScale = Vector3.one;
		}

		for (int i = 0; i < rotationBufferSize; i++) {
			_rotationBuffer[i] = transform.rotation;
		}
		_rotationBufferID = 0;
	}

	public void TrackingLost(){
		if (_dampedGO) {
			_dampedGO.SetActive (false);
		}
	}

	//Get an average (mean) from more then two quaternions (with two, slerp would be used).
	//Note: this only works if all the quaternions are relatively close together.
	//Usage: 
	//-Cumulative is an external Vector4 which holds all the added x y z and w components.
	//-newRotation is the next rotation to be added to the average pool
	//-firstRotation is the first quaternion of the array to be averaged
	//-addAmount holds the total amount of quaternions which are currently added
	//This function returns the current average quaternion
	public static Quaternion AverageQuaternion(ref Vector4 cumulative, Quaternion newRotation, Quaternion firstRotation, int addAmount){

		float w = 0.0f;
		float x = 0.0f;
		float y = 0.0f;
		float z = 0.0f;

		//Before we add the new rotation to the average (mean), we have to check whether the quaternion has to be inverted. Because
		//q and -q are the same rotation, but cannot be averaged, we have to make sure they are all the same.
		if(!AreQuaternionsClose(newRotation, firstRotation)){

			newRotation = InverseSignQuaternion(newRotation);	
		}

		//Average the values
		float addDet = 1f/(float)addAmount;
		cumulative.w += newRotation.w;
		w = cumulative.w * addDet;
		cumulative.x += newRotation.x;
		x = cumulative.x * addDet;
		cumulative.y += newRotation.y;
		y = cumulative.y * addDet;
		cumulative.z += newRotation.z;
		z = cumulative.z * addDet;		

		//note: if speed is an issue, you can skip the normalization step
		return NormalizeQuaternion(x, y, z, w);
	}

	public static Quaternion NormalizeQuaternion(float x, float y, float z, float w){

		float lengthD = 1.0f / (w*w + x*x + y*y + z*z);
		w *= lengthD;
		x *= lengthD;
		y *= lengthD;
		z *= lengthD;

		return new Quaternion(x, y, z, w);
	}

	//Changes the sign of the quaternion components. This is not the same as the inverse.
	public static Quaternion InverseSignQuaternion(Quaternion q){

		return new Quaternion(-q.x, -q.y, -q.z, -q.w);
	}

	//Returns true if the two input quaternions are close to each other. This can
	//be used to check whether or not one of two quaternions which are supposed to
	//be very similar but has its component signs reversed (q has the same rotation as
	//-q)
	public static bool AreQuaternionsClose(Quaternion q1, Quaternion q2){

		float dot = Quaternion.Dot(q1, q2);

		if(dot < 0.0f){

			return false;					
		}

		else{

			return true;
		}
	}
}
