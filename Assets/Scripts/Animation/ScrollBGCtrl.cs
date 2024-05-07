using CbUtils.Extension;

using UnityEngine;

namespace Shuile
{
    /// <summary>
    /// [WIP] inifinit scrolling background
    /// </summary>
    public class ScrollBGCtrl : MonoBehaviour
    {
        private SpriteRenderer spriteRenderer;
        private GameObject spriteObject => spriteRenderer.gameObject;

        // [copy itself]
        private readonly static (int x, int y)[] posIndex = new (int, int)[9]
            { (-1, -1), (-1, 0), (-1, 1), (0, -1), (0, 0), (0, 1), (1, -1), (1, 0), (1, 1) };
        private Vector2[] positionArray = new Vector2[9];

        // [scroll]
        [SerializeField] private Vector2 velocity;

        private void Start()
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            if(!spriteRenderer || spriteRenderer.gameObject == gameObject)
            {
                throw new System.Exception("[Error]: you need to set SpriteRenderer in the child object of ScrollBGCtrl in case the inifinte recursion.");
            }
            Init();
        }

        private void FixedUpdate()
        {
            MoveUpdate();
        }

        private void Init()
        {
            Vector2 originPos = spriteObject.transform.position;
            Vector2 originScale = spriteObject.transform.localScale;
            Vector3 spriteSize = spriteRenderer.sprite.bounds.size;
            Vector2 originSize = new Vector2(spriteSize.x * originScale.x, spriteSize.y * originScale.y);
            for (int i = 0; i < 9; i++)
            {
                // TODO: has bugs.
                var newPos = originPos + new Vector2(posIndex[i].x * originSize.x, posIndex[i].x * originSize.y);
                positionArray[i] = newPos;

                if (i == 4) continue; // it's itself.

                spriteObject.Instantiate().SetPosition(newPos).SetParent(transform);
            }
        }

        private void MoveUpdate()
        {
            transform.position += new Vector3(velocity.x, velocity.y);
        }
    }
}
