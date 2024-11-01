using CbUtils.Extension;
using UnityEngine;

namespace Shuile.UI
{
    public static class UIHelper
    {
        private static Canvas root;

        public static Canvas Root
        {
            get
            {
                if (root)
                {
                    return root;
                }

                return root = Resources.Load<GameObject>("UIDesign/RootCanvas").Instantiate().GetComponent<Canvas>();
            }
        }
    }
}
