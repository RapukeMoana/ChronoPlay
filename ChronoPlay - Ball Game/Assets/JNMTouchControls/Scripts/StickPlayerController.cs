using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.JNMTouchControls.Scripts
{
    public class StickPlayerController : StickController
    {
        private IPlayer _player;

        protected override void Start()
        {
            base.Start();

            _player = GameObject.FindWithTag("Player").GetComponent<IPlayer>();
        }

        protected override void HandleInput(UnityEngine.Vector3 input)
        {
            base.HandleInput(input);
            _player = GameObject.FindWithTag("Player").GetComponent<IPlayer>();
            if (_player != null)
            {
                Vector3 diff = _button.position - _buttonFrame.position;

                float distance = Vector3.Distance(_button.position, _buttonFrame.position);

                distance /= (_buttonFrame.sizeDelta.x / 2.0f);

                Vector2 normDiff = new Vector3(diff.x / _dragRadius, diff.y / _dragRadius);

                _player.OnStickChanged(distance, normDiff);
            }

        }
        public void PlayerInitialise()
        {

            _player = GameObject.FindWithTag("Player").GetComponent<IPlayer>();
        }
    }
}
