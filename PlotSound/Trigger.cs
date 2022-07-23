using System;
using System.ComponentModel;

namespace SoundPlotter
{
    public enum TriggerCondition
    {
        Less,
        Equal,
        Greater,
    }

    public class Trigger : Component
    {
        public bool Enabled { get; set; }

        public int Value { get; set; }

        public bool UseFilter { get; set; }

        public TriggerCondition Condition { get; set; }
    }
}
