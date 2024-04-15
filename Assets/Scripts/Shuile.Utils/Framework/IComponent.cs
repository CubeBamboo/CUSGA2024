namespace Shuile.Framework
{
    public interface IComponent<TTarget>
    {
        TTarget Target { set; }
    }
}
