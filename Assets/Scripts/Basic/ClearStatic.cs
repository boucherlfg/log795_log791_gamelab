using System;
using GameLab.Behaviours;
using GameLab.TestComponents;
using GameLab.UI;
using UnityEngine;

namespace GameLab.Basic
{
    [DefaultExecutionOrder(-200)]
    public class ClearStatic: MonoBehaviour
    {
        private void Awake()
        {
            SoundCategoryUI.ClearStaticVariable();
            PlayerScript.ClearStaticVariables();
            ScoreTester.ClearStaticVariables();
        }
    }
}