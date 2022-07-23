using System.ComponentModel;

namespace SoundPlotter
{
    public class Limits : Component
    {
        public bool Enabled { get; set; }
        public int Min { get; set; }
        public int Max { get; set; }

        public Limits()
        {
            Min = 0;
            Max = 32786;
        }
    }
}
