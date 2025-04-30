using System.Collections.Generic;
using UnityEngine;

namespace GameLab.Core
{
    public struct MementoPlayerInfo
    {
        public Vector3 InitialPosition;
        public Quaternion InitialRotation;
        public readonly List<MementoEntry> MementoEntries;
        public readonly int PlayerId;

        public MementoPlayerInfo(Vector3 initialPosition, Quaternion initialRotation, List<MementoEntry> mementoEntries, int playerId)
        {
            InitialPosition = initialPosition;
            InitialRotation = initialRotation;
            MementoEntries = mementoEntries;
            PlayerId = playerId;
        }
    }
}