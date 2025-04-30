using System.Collections.Generic;
using GameLab.ScriptableObjects.Bonus;

namespace GameLab.Core
{
    public interface IAuraReceiver
    {
        public List<AbstractBonusEffector> BonusEffectors { get; set; }
        public void Receive();

        public void StartAffecting(AbstractBonusEffector effector);
        public void StopAffecting(AbstractBonusEffector effector);
    }
}