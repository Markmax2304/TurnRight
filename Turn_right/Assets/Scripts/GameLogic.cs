using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public delegate void GetPoint ();

public class GameLogic : MonoBehaviour 
{
	public static bool isGameOver = true;
	public static GetPoint GetPointDelegate;

	public BolidController bolidPrefab;
	public Vector3 startPosition;
	BolidController bolid;

	public GameObject finish;
	public Transform[] finPoints;
	Queue<Vector3> points;

	public Text tScore;
	public Text tRang;
	int score = 0;
	int nextRang = 1;
	int toRang = 0;

	public Material ground;
	public Color[] colors;
	Color currentColor;
	public float speedChangeColor = 10.0f;

	Camera _camera;

	bool isReady;

	void Awake ()
	{
		_camera = Camera.main;
	}

	void Start ()
	{
		GetPointDelegate += GetNewFinishPoint;
		GetPointDelegate += IncrementScore;
		points = new Queue<Vector3> ();

		currentColor = ground.color;
		GenerateFinishLines ();

		StartCoroutine (StartSizeCamera ());
	}

	void Update ()
	{
		if (isGameOver) {

			if (Input.GetKeyDown (KeyCode.Mouse0)) {
				Init ();
				GenerateFinishLines ();
				GetNewFinishPoint ();
				isReady = true;
			}

			if (isReady && Input.GetKeyUp (KeyCode.Mouse0)) {
				isGameOver = false;
				isReady = false;
			}

			return;
		}
	}

	void Init ()
	{
		if (bolid != null) {
			Destroy (bolid.gameObject);
		}

		bolid = Instantiate (bolidPrefab, startPosition, Quaternion.identity) as BolidController;
	}

	public void GetNewFinishPoint ()
	{
		Vector3 nextPoint = points.Dequeue ();
		Instantiate (finish, nextPoint, Quaternion.identity);
		points.Enqueue (nextPoint);
	}

	public void IncrementScore ()
	{
		score++;
		tScore.text = score.ToString ();

		toRang++;
		if(toRang >= nextRang) {
			
			if (nextRang >= 3) {
				SpeedUpBolid ();
			}

			nextRang++;
			toRang = 0;

			ChangeColor ();
		}

		tRang.text = nextRang.ToString () + "\\" + toRang.ToString ();
	}

	void GenerateFinishLines ()
	{
		GameObject[] lines = GameObject.FindGameObjectsWithTag ("Line");
		for (int i = 0; i < lines.Length; i++) {
			Destroy (lines [i]);
		}

		points.Clear ();
		for (int i = 0; i < finPoints.Length; i++) {
			points.Enqueue (finPoints[i].position);
		}

		ChangeColor ();

		score = 0;
		nextRang = 1;
		toRang = 0;
		tScore.text = score.ToString ();
		tRang.text = nextRang.ToString () + "\\" + toRang.ToString ();
	}

	void ChangeColor ()
	{
		int next = Random.Range (0, colors.Length);
		Color nextCol = colors [next];

		if (nextCol == currentColor) {
			ChangeColor ();
			return;
		}

		StartCoroutine (InterpChangeColor (nextCol));
	}

	void SpeedUpBolid ()
	{
		bolid.maxSpeedBolid += 1f;
		bolid.maxDriftOffsetBolid += 2f;
		bolid.angularSpeedBolid += 10f;
	}

	IEnumerator InterpChangeColor (Color nextColor)
	{
		float percent = 0f;
		while (percent < 1f) {
			
			percent += speedChangeColor * Time.deltaTime;
			ground.color = Color.Lerp (currentColor, nextColor, percent);
			yield return null;
		}

		currentColor = nextColor;
	}

	IEnumerator StartSizeCamera ()
	{
		while (_camera.orthographicSize > 13.5f) {

			_camera.orthographicSize -= Time.deltaTime;
			yield return null;
		}

		_camera.orthographicSize = 13.5f;
	}
}
