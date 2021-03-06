﻿using UnityEngine;
using Ludiq;
using Bolt;
using Lasm.OdinSerializer;

namespace Lasm.UAlive.IO
{
    /// <summary>
    /// Deletes a Binary Save file from a path.
    /// </summary>
    [UnitCategory("IO")]
    [UnitTitle("Delete Save")]
    [RenamedFrom("Lasm.BoltExtensions.IO.DeleteBinarySave")]
    [RenamedFrom("Lasm.BoltExtensions.DeleteBinarySave")]
    public class DeleteBinarySave : BinarySaveUnit
    {
        /// <summary>
        /// Uses the OS application path (Persistant Data Path) if true.
        /// </summary>
        [Serialize]
        [Inspectable]
        [InspectorToggleLeft]
        public bool usePersistantDataPath = true;

        [OdinSerialize]
        private bool isInit;

        /// <summary>
        /// The Value Input port for a custom path. Shown only when usePersistantDataPath is false.
        /// </summary>
        [DoNotSerialize]
        public ValueInput path;

        /// <summary>
        /// The filename and file extension of this save.
        /// </summary>
        [DoNotSerialize]
        public ValueInput fileName;

        /// <summary>
        /// The Control Input port to enter when you want to delete the save.
        /// </summary>
        [DoNotSerialize]
        public ControlInput delete;

        /// <summary>
        /// The Control Output port invoked when deleting is complete.
        /// </summary>
        [DoNotSerialize]
        public ControlOutput complete;

        public override void AfterAdd()
        {
            base.AfterAdd();

            if (!isInit)
            {
                usePersistantDataPath = true;
                Define();
                isInit = true;
            }
        }

        protected override void Definition()
        {
            if (!usePersistantDataPath) path = ValueInput<string>("path", string.Empty);
            fileName = ValueInput<string>(nameof(fileName), string.Empty);

            complete = ControlOutput("complete");
            delete = ControlInput("delete", (flow) => {
                BinarySave.Delete((usePersistantDataPath) ? Application.persistentDataPath + "/data/" + flow.GetValue<string>(fileName) : flow.GetValue<string>(path) + "/" + flow.GetValue<string>(fileName));
                return complete;
            });

            Requirement(fileName, delete);
            Succession(delete, complete);
        }

    }
}