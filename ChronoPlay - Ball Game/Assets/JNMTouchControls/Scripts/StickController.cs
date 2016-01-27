using UnityEngine;
using System.Collections;

public enum EnStickType
{
    Static = 0,
    Dynamic = 1
}

public class StickController : MonoBehaviour
{
    #region Properties

    /// <summary>
    /// Static or dynamic joystick
    /// </summary>
    public EnStickType StickType;

    /// <summary>
    /// Allow joystick to move in x-direction
    /// </summary>
    public bool AllowX = true;

    /// <summary>
    /// Allow joystick to move in y-direction
    /// </summary>
    public bool AllowY = true;

    #endregion

    #region Protected members

    protected RectTransform _button;

    protected RectTransform _buttonFrame;

    protected float _dragRadius = 0.0f;

    #endregion

    #region Private members

    private int _buttonId = -1;

    private Vector3 _startPos = Vector3.zero;

    #endregion


    // Use this for initialization
	protected virtual void Start () {

        _button = this.transform.FindChild("_stick").GetComponent<RectTransform>();
        _buttonFrame = this.transform.FindChild("_stickFrame").GetComponent<RectTransform>();
        _dragRadius = _buttonFrame.rect.width / 2.0f;

        HideDynamic();
	
	}

    private void HideDynamic()
    {
        if (StickType == EnStickType.Dynamic)
        {
            _buttonFrame.gameObject.SetActive(false);
            _button.gameObject.SetActive(false);
        }
    }

    protected virtual void Update()
    {
        HandleTouchInput();

    #if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_WEBPLAYER

        // Simulate the Stick in Editor or Standalone
        if (Input.GetMouseButtonDown(0))
        {
            if (CheckButtonDown(Input.mousePosition))
            {
                _buttonId = 1;
                ShowDynamic();
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            ButtonUp();
        }

    #endif

    }
	
	// Update is called once per frame
	protected virtual void FixedUpdate () {

#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_WEBPLAYER
        HandleInput(Input.mousePosition);
#endif

    }

    private void ShowDynamic()
    {
        if (StickType == EnStickType.Dynamic)
        {
            this.transform.position = Input.mousePosition;
            _buttonFrame.gameObject.SetActive(true);
            _button.gameObject.SetActive(true);
        }
    }

    private bool CheckButtonDown(Vector2 input)
    {
        if (StickType == EnStickType.Static)
        {
            float xMid = _buttonFrame.sizeDelta.x / 2.0f;
            float yMid = _buttonFrame.sizeDelta.y / 2.0f;

            Rect rect = new Rect(this.transform.position.x - xMid, this.transform.position.y - yMid, _buttonFrame.sizeDelta.x, _buttonFrame.sizeDelta.y);

            if (rect.Contains(input))
            {
                return true;
            }
        }
        else
        {
            return true;
        }

        return false;
    }

    private void ButtonUp()
    {
        _buttonId = -1;

        HideDynamic();

    }


    #region User Input

    void HandleTouchInput()
    {
        // We have touch-input (mobile)
        if (Input.touchCount > 0)
        {
            foreach (Touch touch in Input.touches)
            {
                Vector3 touchPos = new Vector3(touch.position.x, touch.position.y);

                if (touch.phase == TouchPhase.Began)
                {
                    if (CheckButtonDown(touch.position))
                    {
                        _buttonId = touch.fingerId;
                    }
                }

                if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                {
                    if (_buttonId == touch.fingerId)
                    {
                        HandleInput(touchPos);
                    }                 
                }

                if (touch.phase == TouchPhase.Ended)
                {
                    if (_buttonId == touch.fingerId)
                    {
                        _buttonId = -1;
                        HandleInput(touchPos);
                    }            
                }
            }
        }
    }

    protected virtual void HandleInput(Vector3 input)
    {
        if (_buttonId != -1 && !(!AllowX && !AllowY))
        {
            Vector3 differenceVector = (input - _buttonFrame.position);

            if (differenceVector.sqrMagnitude >
                _dragRadius * _dragRadius)
            {
                differenceVector.Normalize();

                _button.position = _buttonFrame.position +
                    differenceVector * _dragRadius;

            }
            else
            {
                _button.position = input;
            }

            if (AllowY == false)
            {
                _button.position = new Vector3(_button.position.x, _buttonFrame.position.y, 0);
            }

            if (AllowX == false)
            {
                _button.position = new Vector3(_buttonFrame.position.x, _button.position.y, 0);
            }
        }
        else
        {
            _button.position = _buttonFrame.position;
        }
    }

    #endregion
}
