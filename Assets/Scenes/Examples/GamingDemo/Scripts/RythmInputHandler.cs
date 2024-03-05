using CbUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CUSGA2024
{
    //处理玩家输入，联系playerInput和playercontroller
    public class RythmInputHandler : MonoBehaviour
    {
        [SerializeField] private GameObject cursorTipsPrefab;
        private GameObject cursorTipsObject;

        [SerializeField] private PlayerController player1;
        [SerializeField] private PlayerController player2;
        private PlayerInput playerInput;
        private Camera mCamera;
        private int currentMode; // 0: place block, 1: remove block

        private int playerIndex; // 0: player1, 1: player2

        private void Start()
        {
            //player1 = GetComponent<PlayerController>();
            playerInput = GetComponent<PlayerInput>();
            playerInput.onActionTriggered += OnActionTriggered;
            mCamera = Camera.main;
            cursorTipsObject = cursorTipsPrefab.Instantiate();

            InitBlock();
        }

        private void OnDestroy()
        {
            playerInput.onActionTriggered -= OnActionTriggered;
        }

#if UNITY_EDITOR
        private void OnGUI()
        {
            GUI.skin.label.fontSize = 30;

            string playerName = playerIndex == 0 ? "blue" : "purple";
            string modeName = currentMode == 0 ? "place" : "remove";
            GUILayout.Label($"CurrentPlayer:{playerName}");
            GUILayout.Label($"CurrentMode:{modeName}");

            Vector2 mousePos = Mouse.current.position.ReadValue();
            Vector3 mouseWorldPos = mCamera.ScreenToWorldPoint(mousePos);
            Vector3Int gridPos = player1.grid.WorldToCell(mouseWorldPos);
            GUILayout.Label($"CursorPosition:{gridPos}");
        }
#endif

        private void OnActionTriggered(InputAction.CallbackContext context)
        {
            switch (context.action.name)
            {
                case "PointerPosition":
                    PointerPosition(context);
                    break;
                case "Fire":
                    OnFire(context);
                    break;
            }
        }

        private void OnFire(InputAction.CallbackContext context)
        {
            if (context.phase != InputActionPhase.Canceled) return;

            //TODO: 记得重构

            Vector2 mousePos = Mouse.current.position.ReadValue(); 
            Vector3 mouseWorldPos = mCamera.ScreenToWorldPoint(mousePos);
            Vector3Int gridPos = player1.grid.WorldToCell(mouseWorldPos);
            PlayerController currentPlayer = playerIndex == 0 ? player1 : player2;

            switch (currentMode)
            {
                case 0:
                    currentPlayer.PlaceBlock(gridPos);
                    break;
                case 1:
                    currentPlayer.RemoveBlock(gridPos);
                    break;
            }

            playerIndex = (playerIndex + 1) % 2;
        }

        private void PointerPosition(InputAction.CallbackContext context)
        {
            //update cursor tips
            Vector2 mousePos = context.ReadValue<Vector2>();
            Vector3 mouseWorldPos = mCamera.ScreenToWorldPoint(mousePos);
            Vector3Int gridPos = player1.grid.WorldToCell(mouseWorldPos);
            if (!player1.grid.IsOutOfBound(gridPos))
            {
                cursorTipsObject.SetActive(true);
                Vector3 gridWorldPos = player1.grid.CellToWorld(gridPos);
                cursorTipsObject.transform.position = gridWorldPos;
            }
            else
            {
                cursorTipsObject.SetActive(false);
            }
        }

        public void SwitchMode()
        {
            currentMode = (currentMode + 1) % 2;
        }

        private void InitBlock()
        {
            player1.PlaceBlockWithoutCheckClear(new Vector3Int(1, 0, 0));
            player2.PlaceBlockWithoutCheckClear(new Vector3Int(player2.grid.width - 2, 0, 0));
        }
    }
}
