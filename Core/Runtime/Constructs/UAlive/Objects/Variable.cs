﻿using Lasm.OdinSerializer;
using Ludiq;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Lasm.UAlive
{
    [Serializable]
    public sealed class Variable
    {
        [Serialize]
        public string name;

        [Serialize]
        public int id = new object().GetHashCode();
         
        [Serialize]
        private Type _type = typeof(int);
        [Inspectable]
        public Type type
        { 
            get => _type;
            set
            {
                var changed = _type != value;
                _type = value;
                if (changed)
                {
                    this.value = value.Default();
                    onChanged?.Invoke();
                }
            }
        }

        [Serialize]
        public object value = 0;
         
        [Serialize] 
        public Method getter = new Method();
        [Serialize]
        public Method setter = new Method();

        public event Action onChanged = new Action(() => { });

        public void Changed()
        {
            onChanged?.Invoke();
        }
    }
} 