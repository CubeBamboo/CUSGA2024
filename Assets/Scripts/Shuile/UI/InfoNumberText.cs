using CbUtils.Extension;
using TMPro;
using UnityEngine;

namespace Shuile.UI
{
    public class InfoNumberText : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI number;

        public TextMeshProUGUI Number => number ? number : number = this.GetChildByName<TextMeshProUGUI>("number");
    }
}
