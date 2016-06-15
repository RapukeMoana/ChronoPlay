using System;
using UnityEngine;
using System.Collections;

/// <summary>
/// Player controller sample implementation of IPlayer-baseclass
/// </summary>
public class PlayerController : IPlayer
{
	/// <summary>
	/// The rotation speed of the Camera
	/// </summary>
	public float RotationSpeed = 12.0f;

	/// <summary>
	/// The move speed of the character
	/// </summary>
	public float MoveSpeed = 7.0f;

	/// <summary>
	/// The transform (model) of the character
	/// </summary>
	public Transform PlayerTransform;

	/// <summary>
	/// The animator of the character
	/// </summary>
	private Animator _animator;

	/// <summary>
	/// The rigidbody to control the character
	/// </summary>
	private Rigidbody _rigidbody;

    /// <summary>
    /// The CharacterController to control the character
    /// </summary>
    private CharacterController _characterController;

    private float gravity = 20.0f; // Gravity for the character.


	/// <summary>
	/// Get the rigidbody of the character and the Animator
	/// </summary>
    void Start()
    {
		_rigidbody = GetComponent<Rigidbody>();

        _characterController = GetComponent<CharacterController>();

//		_animator = PlayerTransform.gameObject.GetComponent<Animator>();

    }

	void Update()
	{
		#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_WEBPLAYER

		// In Standalone and editor we can test the Jump-action with space key
		if(Input.GetKeyDown(KeyCode.Space))
		{
			DoJump();
		}
		#endif
	}

    // Check if the player is grounded.
    bool IsGrounded()
    {
        if (_characterController == null)
            return false;

        return _characterController.isGrounded;
    }

	/// <summary>
	/// Raises the stick changed event when 
	/// the position of the virtual joystick has changed
	/// </summary>
	/// <param name="distance">of the change</param>
	/// <param name="stickPos">Stick position (coordinates)</param>
	public override void OnStickChanged(float distance, Vector2 stickPos)
	{
		//var movement = new Vector3(
		//	stickPos.x,
		//	0,
		//	);
        float moveHorizontal = stickPos.x;
        float moveVertical = stickPos.y;

        Vector3 movement = new Vector3(moveHorizontal, 0, moveVertical);
        //If there is movement, reset idle time (for auto play)
        if(moveHorizontal!=0 && moveVertical != 0)
        {
            PlayerMovement.idleTime = 0;
        }
        _rigidbody.AddForce(movement * MoveSpeed);
//        //float speed = distance;

//		// We have an animator so let's apply the states Run and Jump
//		// by defining the parameters Jump and MoveSpeed
////		if (_animator)
//		{
////			AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);

//            Quaternion tempRotation = Camera.main.transform.rotation;
//            tempRotation.x = 0;
//            tempRotation.z = 0;
//            transform.rotation = tempRotation;
//            if (speed >= 0)
//            //if(speed >= 0.1f)
//            {
////				_animator.SetFloat("MoveSpeed", speed);

//				movement *=speed;

//                float xMv = stickPos.x;
//                float zMv = stickPos.y;

//                float tempAngle = Mathf.Atan2(zMv, xMv);
//                xMv *= Mathf.Abs(Mathf.Cos(tempAngle));
//                zMv *= Mathf.Abs(Mathf.Sin(tempAngle));

//                movement = new Vector3(xMv, 0, zMv);
//                movement = transform.TransformDirection(movement);
//                movement *= MoveSpeed;

//                movement.y -= gravity * Time.deltaTime;

//                Vector3 tempPos = transform.position;
//                tempPos.x += xMv;
//                tempPos.z += zMv;

// //               PlayerTransform.localRotation = Quaternion.Slerp(PlayerTransform.localRotation, Quaternion.LookRotation(tempPos - transform.position), RotationSpeed * Time.deltaTime);

//                if (_rigidbody != null)
//                {
//                    _rigidbody.MovePosition(PlayerTransform.position + (movement * Time.deltaTime));
//                }
//                else if (_characterController != null)
//                {
//                    _characterController.Move((movement * Time.deltaTime));
//                }

//			}
//			//else
//   //         {
//   //             _animator.SetFloat("MoveSpeed", 0.0f);
//   //         }
//        }
	}

	/// <summary>
	/// Does the jump-animation
	/// </summary>
	public void DoJump()
	{
		//if (_animator)
		//{
		//	_animator.SetBool("Jump", true);
		//}
	}
}
