using GameLab.Core;

namespace GameLab.Constructions.Canons
{
    public abstract class CanonState: IState
    {
        protected readonly CanonConstruct _canon;

        public CanonState(CanonConstruct canon)
        {
            _canon = canon;
        }
        
        public abstract void EnterState();
        public abstract void ExitState();
        public abstract void UpdateState();
    }
}