using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameLab.Core
{
    public class MementoVehicleInfo
    {
        private readonly Dictionary<int, Vector3> _positionRecords;
        private readonly Dictionary<int, Quaternion> _rotationRecords;
        private readonly Dictionary<int, bool> _dashRecords;
        private readonly Dictionary<int, Color> _colorsRecords;

        public MementoVehicleInfo()
        {
            _positionRecords = new Dictionary<int, Vector3>();
            _rotationRecords = new Dictionary<int, Quaternion>();
            _dashRecords = new Dictionary<int, bool>();
            _colorsRecords = new Dictionary<int, Color>();
        }

        public void AddPosition(int frame, Vector3 position)
        {
            _positionRecords.Add(frame, position);
        }
        
        public void AddDash(int frame, bool dash) => _dashRecords.Add(frame, dash);

        public void AddColor(int frame, Color newColor)
        {
            if (!_colorsRecords.LastOrDefault().Equals(newColor))
                _colorsRecords.Add(frame, newColor);
        }
        
        public void AddRotation(int frame, Quaternion rotation)
        {
            _rotationRecords.Add(frame, rotation);
        }

        public Vector3 GetPosition(int frame)
        {
            return _positionRecords[frame];
        }
        
        public bool GetDash(int frame) => _dashRecords[frame];
        
        public Quaternion GetRotation(int frame)
        {
            return _rotationRecords[frame];
        }

        public bool PositionExists(int frame)
        {
            return _positionRecords.ContainsKey(frame);
        }

        public bool RotationExists(int frame)
        {
            return _rotationRecords.ContainsKey(frame);
        }
        
        public bool DashExists(int frame) => _dashRecords.ContainsKey(frame);
    }
}