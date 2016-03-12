using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.JNMTouchControls.Scripts
{
    public class StickCameraController : StickController
    {
        private Transform _camera;

        private Transform _player;

        private float _eulerY = 0.0f;

        public float RotationXSpeed = 100.0f;

        public float RotationYSpeed = 20.0f;

        public float AngleX = 40.0f;

        public float MinAngleX = 20.0f;

        public float MaxAngleX = 60.0f;

        public float Distance = 10.0f;

        protected override void Start()
        {
            base.Start();


        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
	
        }

        protected override void HandleInput(UnityEngine.Vector3 input)
        {
            base.HandleInput(input);
            
            Vector3 diff = _button.position - _buttonFrame.position;


            float distance = Vector3.Distance(_button.position, _buttonFrame.position);

            distance /= (_buttonFrame.sizeDelta.x / 2.0f);

            Vector2 normDiff = new Vector3(diff.x / _dragRadius, diff.y / _dragRadius);
            if (normDiff.x < 0 ||normDiff.x > 0)
            {
                _camera.RotateAround(_player.position, new Vector3(0, 1, 0), normDiff.x * RotationXSpeed * Time.deltaTime);
                _eulerY = _camera.eulerAngles.y;
            }

            if (normDiff.y < 0 || normDiff.y  > 0)
            {
                float diffAngleX = normDiff.y * RotationYSpeed * Time.deltaTime;

                if (diffAngleX > 0)
                {
                    if (AngleX < MaxAngleX)
                    {
                        AngleX += diffAngleX;
                    }
                }
                else
                {
                    if (AngleX > MinAngleX)
                    {
                        AngleX += diffAngleX;
                    }
                }
                AngleX = normDiff.y;
            }
        }
    }
}
