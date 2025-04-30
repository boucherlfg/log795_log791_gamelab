using System.Linq;
using GameLab.Utility;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace GameLab.Editor
{
    [CustomEditor(typeof(DeviceDropdownComponent))]
    public class DeviceDropdownEditor : UnityEditor.Editor
    {
        private int selectedIndex = 0;
        private string[] deviceNames;
        private InputDevice[] filteredDevices;
        private SerializedProperty selectedDeviceProperty;

        private void OnEnable()
        {
            // Cache the serialized property reference
            selectedDeviceProperty = serializedObject.FindProperty("selectedDevice");

            // Load devices and restore last selection
            LoadDevices();
            selectedIndex = System.Array.IndexOf(deviceNames, selectedDeviceProperty.stringValue);
            if (selectedIndex == -1) selectedIndex = 0;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update(); // Ensure we’re working with the latest data

            // Refresh devices list
            LoadDevices();

            // Dropdown UI
            selectedIndex = EditorGUILayout.Popup("Select Device", selectedIndex, deviceNames);

            // Set the value through the serialized property
            selectedDeviceProperty.stringValue = deviceNames[selectedIndex];

            // Apply changes properly
            serializedObject.ApplyModifiedProperties();
        }

        private void LoadDevices()
        {
            // Get all non-mouse devices
            filteredDevices = InputSystem.devices.Where(d => !(d is Mouse)).ToArray();

            // Populate device names for dropdown
            deviceNames = filteredDevices.Select(d => d.name).ToArray();

            // Fallback if no devices are found
            if (deviceNames.Length == 0)
            {
                deviceNames = new string[] { "No Devices Found" };
            }
        }
    }
}