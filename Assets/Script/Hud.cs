//
// Copyright (c) Brian Hernandez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
//

using UnityEngine;
using UnityEngine.InputSystem;

namespace MFlight.Demo
{
    public class Hud : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private MouseFlightController flightController = null;

        [Header("HUD Elements")]
        [SerializeField] private RectTransform boresight = null;

        private Camera playerCam = null;

        private void Awake()
        {
            if (flightController == null)
                Debug.LogError(name + ": Hud - Flight Controller not assigned!");

            playerCam = flightController.GetComponentInChildren<Camera>();

            if (playerCam == null)
                Debug.LogError(name + ": Hud - No camera found on assigned Flight Controller!");
        }

        private void Update()
        {
            if (flightController == null || playerCam == null)
                return;

            UpdateGraphics(flightController);
        }

        private void UpdateGraphics(MouseFlightController controller)
        {
            if (boresight != null)
            {
                boresight.position = playerCam.WorldToScreenPoint(controller.BoresightPos);
                boresight.gameObject.SetActive(boresight.position.z > 1f);
            }
        }

        public void SetReferenceFlightController(MouseFlightController controller)
        {
            flightController = controller;
        }
    }
}
