using CbUtils;
using CbUtils.Extension;
using System.Linq;

namespace Shuile.Utils.CbUtils.Kits.Core
{
    public static class Extension
    {
        public static string GetCurrentStateLog<TStateId>(this FSM<TStateId> fsm)
        {
            return fsm.States.ToArray()
                .Select(val => val.Key)
                .ToArray()
                .IEnumerableToString();
        }
    }
}
