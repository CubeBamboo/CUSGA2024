using UnityEngine;
using UnityEngine.InputSystem;
using Shuile.Gameplay;

namespace Shuile.Rhythm
{
    //处理玩家输入，联系playerInput和playercontroller
    public class RhythmInputHandler : MonoBehaviour
    {
        [SerializeField] private PlayerInput playerInput;
        [SerializeField] private PlayerController player;

        private float currentHitOffset = 0f;

        private void Start()
        {
            playerInput.onActionTriggered += OnActionTriggered;
        }

        private void OnDestroy()
        {
            playerInput.onActionTriggered -= OnActionTriggered;
        }

#if UNITY_EDITOR
        private void OnGUI()
        {
            //GUI.skin.label.fontSize = 40;
            //GUILayout.Label($"hitOffset:{currentHitOffset}");
            //GUILayout.Label($"RealGameTime:{Time.fixedTime}");

            //if (!MusicRhythmManager.Instance.IsPlaying)
            //    return;

            //GUILayout.Label($"RhythmCheckTime:{MusicRhythmManager.Instance.CurrentTime}");
        }
#endif

        private void OnActionTriggered(InputAction.CallbackContext context)
        {
            if(context.action.name != "Attack" && context.action.name != "HorizontalMove")
                return;
            if (context.phase != InputActionPhase.Started)
                return;
            if (!MusicRhythmManager.Instance.CheckBeatRhythm(out currentHitOffset))
                return;

            switch(context.action.name)
            {
                case "Attack":
                    player.Attack();
                    break;
                case "HorizontalMove":
                    player.Move(context.ReadValue<float>());
                    break;
            }
        }

        
    }
}
