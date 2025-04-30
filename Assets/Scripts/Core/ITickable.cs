namespace GameLab.Core
{
    public interface ITickable
    {
        public int Priority { get; }
        void Tick(float time);
        void Reset();
    }
}