using GameLab.Bonus;

using GameLab.Constructions;
using GameLab.Constructions.Canons;
using GameLab.Core;

using GameLab.Enums;

using GameLab.Player;

using UnityEngine;



namespace GameLab.ScriptableObjects.Bonus

{

    public class MetalBonusEffector : AbstractBonusEffector

    {

        private MetalBonusConfig _metalBonusConfig;

        private float _initialMaxSpeed;



        public override AbstractBonusConfig BonusConfig

        {

            get => _metalBonusConfig;

            set

            {

                if (value is MetalBonusConfig)

                {

                    _metalBonusConfig = (MetalBonusConfig)value;

                }

            }

        }





        public override void StartAffecting(MovingObject movingObject)

        {

            base.StartAffecting(movingObject);

            _initialMaxSpeed = movingObject.MaxSpeed;

            movingObject.MaxSpeed = _metalBonusConfig.MetalVitesseMax;

        }



        public override void StartAffecting(CanonConstruct canonConstruct)

        {

            base.StartAffecting(canonConstruct);

            canonConstruct.TimeInCanon = _metalBonusConfig.MetalTimeInCanon;

            canonConstruct.transform.localScale *= _metalBonusConfig.MetalScaleFactor;

        }



        public override void StartAffecting(BumperConstruction bumperConstruction)

        {

            base.StartAffecting(bumperConstruction);

            bumperConstruction.transform.localScale *= _metalBonusConfig.MetalScaleFactor;

        }



        public override void StartAffecting(RampConstruction rampConstruction)

        {

            base.StartAffecting(rampConstruction);

            rampConstruction.transform.localScale *= _metalBonusConfig.MetalScaleFactor;

        }



        public override void StopAffecting(MovingObject movingObject)

        {

            base.StartAffecting(movingObject);

            movingObject.MaxSpeed = _initialMaxSpeed;

        }

    }

}