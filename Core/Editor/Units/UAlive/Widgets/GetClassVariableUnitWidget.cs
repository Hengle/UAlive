﻿using Bolt;
using Ludiq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Lasm.UAlive
{
    [Widget(typeof(GetClassVariableUnit))]
    public class GetClassVariableUnitWidget : UnitWidget<GetClassVariableUnit>
    {
        private float buttonPadding => 8;
        private bool missingContent => unit.Class == null || unit.variable == null;

        public GetClassVariableUnitWidget(FlowCanvas canvas, GetClassVariableUnit unit) : base(canvas, unit)
        {
        }

        protected override bool showHeaderAddon => true;
        protected override float GetHeaderAddonHeight(float width)
        {
            return 20;
        }

        protected override float GetHeaderAddonWidth()
        {
            return Mathf.Clamp(GUI.skin.label.CalcSize(new GUIContent(unit.variable == null ? "   (None Selected)   " : unit.Class?.title + "." + unit.variable?.name)).x + buttonPadding, base.GetHeaderAddonWidth(), 400);
        }

        public override bool foregroundRequiresInput => true;

        protected override NodeColorMix color => NodeColorMix.TealReadable;

        protected override void DrawHeaderAddon()
        {
            var buttonText = "(None Selected)";
            if (!missingContent)
            {
                buttonText = unit.Class.title + "." + unit.variable.name;
            } 

            if (GUI.Button(position.Add().X(42).Add().Y(23).Set().Height(20).Set().Width(missingContent ? 120 : GUI.skin.label.CalcSize(new GUIContent(unit.Class?.title + "." + unit.variable?.name)).x + buttonPadding), buttonText))
            {
                var classes = HUMAssets.Find().Assets().OfType<CustomClass>();

                GenericMenu menu = new GenericMenu();

                for (int i = 0; i < classes.Count; i++)
                {
                    for (int j = 0; j < classes[i].variables.variables.Count; j++)
                    {
                        menu.AddItem(new GUIContent(classes[i].title + "/" + classes[i].variables.variables[j].name), false, (data) =>
                        {
                            var tuple = (ValueTuple<CustomClass, Variable>)data;
                            unit.Class = tuple.Item1;
                            unit.variable = tuple.Item2;
                            unit.Define();
                        }, (classes[i], classes[i].variables.variables[j]));
                    } 
                }

                menu.ShowAsContext();
            }
        } 
    }
}
