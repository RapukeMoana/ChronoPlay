using UnityEngine;
using System.Collections;

/// <summary>
/// Sample class for a camera following the player
/// </summary>
public class FollowCamera : MonoBehaviour {

	/// <summary>
	/// The player
	/// </summary>
	private GameObject _player;

	/// <summary>
	/// The camera following the player
	/// </summary>
	public Transform FCamera;

	/// <summary>
	/// The camera distance
	/// </summary>
	public float CameraDistance = 10f;

	/// <summary>
	/// The camera rotation speed.
	/// </summary>
	public float CameraRotationSpeed = 120.0f;

	/// <summary>
	/// The camera X angle.
	/// </summary>
	public float CameraXAngle = 30.0f;

	/// <summary>
	/// The y angle
	/// </summary>
	private float y = 0.0f;

	/// <summary>
	/// The start position of the mouse when swiping the camera
	/// </summary>
	private Vector3 _startPosition = Vector3.zero;

	/// <summary>
	/// The current direction the cmaera is rotated to (-x, +x)
	/// </summary>
	private Vector3 _currentDirection = Vector3.zero;

	private Transform _playerTransform;


	void Awake()
	{
		_player = GameObject.FindWithTag("Player");

		_playerTransform = _player.transform.FindChild("rabbit");

		if(FCamera == null)
		{
			FCamera = Camera.main.transform;
		}

	}

	void Start()
	{
		Vector3 angles = FCamera.eulerAngles;
		y = angles.y;
	}
	
	private static bool _drag = false;
	
	public void OnButtonDown()
	{
		_startPosition = Input.mousePosition;
	}

	public void OnButtonUp()
	{
		_drag = false;
	}
	
	public void OnCameraMove()
	{
		_drag = true;
		
		_currentDirection = Input.mousePosition - _startPosition;
		_currentDirection.Normalize();
	}
	

	void FixedUpdate()
	{
		if (!_player)
			return;

		// In the sample implementation for rotating the camera
		// we only rotate it in x-direction.
		if(_drag)
		{
			if(_currentDirection.x < 0)
			{
				y += CameraRotationSpeed * Time.deltaTime;
			}
			else if(_currentDirection.x > 0)
			{
				y-= CameraRotationSpeed * Time.deltaTime;
			}
		}

		// Do rotation to target when stick is not moving.
		if(Stick.IsMoving && y != 0.0f)
		{

			float currentRotationAngle = FCamera.eulerAngles.y;
			y = Mathf.LerpAngle(currentRotationAngle, 0, 3.0f * Time.deltaTime);

		}

		// Apply rotation
		Quaternion rotation = Quaternion.Euler(CameraXAngle, y, 0);
		FCamera.rotation = rotation;

		// Apply position
		Vector3 position = _player.transform.position - (rotation * Vector3.forward * CameraDistance + new Vector3(0, -1, 0));
		FCamera.position = position;

		FCamera.LookAt(_player.transform.position);

	}

}
