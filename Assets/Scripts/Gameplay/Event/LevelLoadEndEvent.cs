using Shuile.Framework;

namespace Shuile.Gameplay.Event
{
    /// <summary>
    /// it will trigger and clear when the level is loaded
    /// </summary>
    public class LevelLoadEndEvent : GlobalEvent<LevelLoadEndEvent, string>
    {
    }

    public class LevelStartEvent : GlobalEvent<LevelStartEvent, string>
    {
    }
}
