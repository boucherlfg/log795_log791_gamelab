namespace GameLab.Core
{
    public interface IState
    {
        public void EnterState();
        public void ExitState();
        public void UpdateState();
    }
}