namespace Code.MainSystem.StatSystem.UI
{
    public interface IUIElement<T>
    {
        void EnableFor(T stats);
        void Update();
    }
    
    public interface IUIElement<T1, T2>
    {
        void EnableFor(T1 stats, T2 statType);
        void Update();
    }
    
    public interface IUIElement<T1, T2, T3>
    {
        void EnableFor(T1 item1, T2 item2, T3 item3);
        void Update();
    }
}