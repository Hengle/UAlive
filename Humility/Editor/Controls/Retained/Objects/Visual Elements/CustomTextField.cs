﻿using System;
using UnityEngine.UIElements;

namespace Lasm.UAlive
{
    public class CustomTextField : AdvancedField<TextField, string>
    {
        public CustomTextField(string label, string @default, LabelPosition position = LabelPosition.Left, int minFieldWidth = 120, VisualElement prependLabel = null, VisualElement appendLabel = null, Action<TextField, string> onValueChanged = null) : base(label, @default, position, minFieldWidth, prependLabel, appendLabel, onValueChanged)
        {
        }
    }
}
