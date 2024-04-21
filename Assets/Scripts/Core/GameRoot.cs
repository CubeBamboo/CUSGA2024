using CbUtils;

using System.Collections.Generic;
using UnityEngine;

namespace Shuile
{
    public class GameRoot : MonoSingletons<GameRoot>
    {
        [Header("DontDestroyOnLoad")]
        private GameObject globalObjectRoot;
        public List<GameObject> notDestroyOnLoadList; // it's just a transition scheme for game jam(((

        protected override void OnAwake()
        {
            SetDontDestroyOnLoad();

            globalObjectRoot = new GameObject("GlobalObject");
            foreach (var obj in notDestroyOnLoadList)
            {
                var spawn = obj.Instantiate();
                spawn.SetParent(globalObjectRoot.transform);
            }
            DontDestroyOnLoad(globalObjectRoot);
        }

        private void Start()
        {
            MonoGameRouter.Instance.LoadMenu();
        }
    }
}
