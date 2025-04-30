/**
 * author: Jimmy Tremblay-Bernier
 */

using GameLab.Bonus;
using GameLab.Enums;
using UnityEngine;
using UnityEngine.Events;

namespace GameLab.Events
{
    public static class BonusManagerEvents
    {
        /// <summary>
        /// Request to start affecting a gameobject with an aura
        /// </summary>
        /// <param name="bonusType">The type of bonus</param>
        /// <param name="gameobject">A ref to the gameobject that is now affected by the aura</param>
        /// <param name="aura">A ref to the aura requesting the effect</param>
        public static readonly UnityEvent<EBonusType, GameObject, Aura> SubscribeElement = new();

        /// <summary>
        /// Request to stop affecting a gameobject with an aura
        /// </summary>
        /// <param name="bonusType">The type of bonus</param>
        /// <param name="gameobject">A ref to the gameobject that is now affected by the aura</param>
        /// <param name="aura">A ref to the aura requesting the effect</param>
        public static readonly UnityEvent<EBonusType, GameObject, Aura>  UnsubscribeElement = new();
        public static readonly UnityEvent<int, BonusSpawner, EBonusType> OnSpawnBonus = new();

        public static readonly UnityEvent<Aura> RegisterAura = new();
    }
}