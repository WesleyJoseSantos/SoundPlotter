using System.Collections.Generic;
using System.ComponentModel;
using System.Timers;

namespace SoundPlotter
{
    internal class PlotterData : Component
    {
        public Limits Scale { get; set; }

        public Limits Filter { get; set; }

        public Limits Limits { get; set; }

        public Trigger StartTrigger { get; set; }

        public Trigger StopTrigger { get; set; }

        public List<int> Buffer { get; set; }

        public double SampleRate { get => 1000 / timer.Interval; set => timer.Interval = 1000 / value; }

        private Timer timer;

        public PlotterData()
        {
            timer = new Timer();
            Scale = new Limits();
            Filter = new Limits();
            Limits = new Limits();
            StartTrigger = new Trigger();
            StopTrigger = new Trigger();
            Buffer = new List<int>();
        }

        [Browsable(false)]
        public Timer GetTimer => timer;
    }
}
