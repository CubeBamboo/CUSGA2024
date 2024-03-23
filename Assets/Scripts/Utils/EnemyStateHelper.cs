using Shuile.Gameplay;
using Shuile.Gameplay.EnemyState;

using System.Runtime.CompilerServices;

namespace Shuile
{
    public static class EnemyStateHelper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T CreateAndBind<T>(Enemy enemy) where T : State, new()
        {
            var state = new T();
            state.Rebind(enemy);
            return state;
        }
    }
}
