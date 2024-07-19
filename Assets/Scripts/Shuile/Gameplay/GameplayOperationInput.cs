using Shuile.UI.Gameplay;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Shuile.Gameplay
{
    public class GameplayOperationInput : MonoBehaviour
    {
        [SerializeField] private PlayerInput input;
        [SerializeField] private GameplayExitPanel exitPanel;

        private void Awake()
        {
            input.onActionTriggered += OnActionTrigger;
        }

        private void OnDestroy()
        {
            input.onActionTriggered -= OnActionTrigger;
        }

        private void OnActionTrigger(InputAction.CallbackContext context)
        {
            if (context.action.name == "Exit")
            {
                //Debug.Log(context.action.phase);
                if (context.action.phase == InputActionPhase.Started)
                {
                    exitPanel.Show();
                }
                else if (context.action.phase == InputActionPhase.Canceled)
                {
                    exitPanel.Hide();
                }
            }
        }
    }
}
