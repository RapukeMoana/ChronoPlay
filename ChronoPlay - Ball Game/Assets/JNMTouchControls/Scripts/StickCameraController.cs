using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.JNMTouchControls.Scripts
{
    public class StickCameraController : StickController
    {
        private bool keyReleased;

        protected override void Start()
        {
            base.Start();
            keyReleased = true;

        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

	
        }

        protected override void HandleInput(UnityEngine.Vector3 input)
        {
            base.HandleInput(input);
            
            Vector3 diff = _button.position - _buttonFrame.position;

            Vector2 normDiff = new Vector3(diff.x / _dragRadius, diff.y / _dragRadius);

            if(keyReleased && Mathf.Round(normDiff.y) == 1.0)
            {
                
                if (PlayerMovement.browseLevel > 0)
                {
                    PlayerMovement.browseLevel--;
                    GameObject.Find("Player").GetComponent<PlayerMovement>().changeBrowseView();
                }
                keyReleased = false;

            }

            if (keyReleased && Mathf.Round(normDiff.y) == -1.0)
            {
                if (PlayerMovement.browseLevel < PlayerMovement.level)
                {
                    PlayerMovement.browseLevel++;
                    GameObject.Find("Player").GetComponent<PlayerMovement>().changeBrowseView();
                }
                keyReleased = false;
            }

            if (Mathf.Round(normDiff.y) == 0)
            {
                keyReleased = true;
            }
            
        }
    }
}
