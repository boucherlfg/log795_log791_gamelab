using System;

using GameLab.Bonus;

using UnityEngine;

using GameLab.Constructions;
using GameLab.Constructions.Canons;
using GameLab.Enums;

using GameLab.Player;

using GameLab.ScriptableObjects.Bonus;



namespace GameLab.Core

{

    [Serializable]

    public abstract class AbstractBonusEffector

    {

        public virtual AbstractBonusConfig BonusConfig { get; set; }



        public virtual void StartAffecting(MovingObject movingObject){}

        public virtual void StartAffecting(CanonConstruct canonConstruct){}

        public virtual void StartAffecting(BumperConstruction bumperConstruction){}

        public virtual void StartAffecting(RampConstruction bumperConstruction){}



        public virtual void Affect(MovingObject movingObject){}

        public virtual void Affect(CanonConstruct canonConstruct){}

        public virtual void Affect(BumperConstruction bumperConstruction){}

        public virtual void Affect(RampConstruction bumperConstruction){}



        public virtual void StopAffecting(MovingObject movingObject){}

        public virtual void StopAffecting(CanonConstruct canonConstruct){}

        public virtual void StopAffecting(BumperConstruction bumperConstruction){}

        public virtual void StopAffecting(RampConstruction bumperConstruction){}

    }

}