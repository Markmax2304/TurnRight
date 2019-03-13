using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BolidController : MonoBehaviour 
{
	public float maxSpeedBolid = 12.0f;
	public float speedUpBolid = 5.0f;
	public float angularSpeedBolid = 150.0f;
	public float maxDriftOffsetBolid = 20.0f;
	public float speedUpOffset = 5.0f;

	float driftOffsetBolid = 0f;
	float speedBolid = 0f;
	Vector3 move;

	public GameObject tapCircle;
	public GameObject DeathCircle;
	Transform _transform;

	void Awake ()
	{
		_transform = transform;
	}

	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Mouse0)) {
			Instantiate (tapCircle, _transform.localPosition, Quaternion.identity);
		}
	}

	void FixedUpdate ()
	{
		if (GameLogic.isGameOver) {
			return;
		}

		move = _transform.up * speedBolid;

		if (Input.GetKey (KeyCode.Mouse0)) {

			//Rotate on turn
			Vector3 rotation = _transform.localEulerAngles;
			rotation.z -= angularSpeedBolid * Time.deltaTime;
			_transform.localRotation = Quaternion.Euler (rotation);

			//Offset on turn
			if (driftOffsetBolid < maxDriftOffsetBolid) {
				driftOffsetBolid += speedUpOffset * Time.deltaTime;
			}
			if (speedBolid > maxSpeedBolid * 0.8f) {
				speedBolid -= speedUpBolid * Time.deltaTime;
			}
		} else {
			if (driftOffsetBolid > 0f) {
				driftOffsetBolid -= Mathf.Pow (speedUpOffset, 2) * Time.deltaTime;
			}
			if (speedBolid < maxSpeedBolid) {
				speedBolid += Mathf.Pow (speedUpBolid, 2) * Time.deltaTime;
			}
		}

		Vector3 offset = -_transform.right * driftOffsetBolid;
		move += offset;

		_transform.position += move * Time.deltaTime;
	}

	void OnTriggerEnter2D (Collider2D coll)
	{
		if (coll.CompareTag ("Obstacle")) {
			
			Instantiate (DeathCircle, _transform.localPosition, _transform.localRotation);

			GameLogic.isGameOver = true;
		}

		if (coll.CompareTag ("Line")) {
			Destroy (coll.gameObject);
			GameLogic.GetPointDelegate();
		}
	}
}
