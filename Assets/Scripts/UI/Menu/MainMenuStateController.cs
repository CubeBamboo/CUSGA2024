using UnityEngine;
using UnityEngine.InputSystem;

namespace Shuile
{
    [RequireComponent(typeof(MainMenuUIStateMachine))]
    public class MainMenuStateController : MonoBehaviour
    {
        MainMenuUIStateMachine stateMachine;

        private Mouse mouse;
        private Keyboard keyboard;

        private void Awake()
        {
            stateMachine = GetComponent<MainMenuUIStateMachine>();
            mouse = Mouse.current;
            keyboard = Keyboard.current;
        }

        private void Update()
        {
            if (keyboard.escapeKey.wasPressedThisFrame)
            {
                stateMachine.SwitchState(MainMenuUIStateMachine.State.Title);
            }

            if (!keyboard.escapeKey.wasPressedThisFrame && (mouse.leftButton.wasPressedThisFrame || keyboard.anyKey.wasPressedThisFrame))
            {
                stateMachine.SwitchState(MainMenuUIStateMachine.State.Select);
            }
        }

        //private bool escInput => keyboard.escapeKey.wasPressedThisFrame;
    }
}
