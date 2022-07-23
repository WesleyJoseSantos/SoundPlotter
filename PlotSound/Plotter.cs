using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Timers;
using NAudio.Wave;
using Newtonsoft.Json;

namespace SoundPlotter
{
    internal class Plotter : Component
    {
        public PlotterData Data { get; set; }

        public WaveIn Input { get; set; }

        public event EventHandler StartTriggerChanged;

        public event EventHandler StopTriggerChanged;

        public event EventHandler SampleAvailable;

        public event EventHandler ValueAvailable;

        public int Sample { get; set; }

        public int Value { get; set; }

        bool started;

        public Plotter()
        {
            Data = new PlotterData();
            Input = new WaveIn();

            Data.GetTimer.Elapsed += Timer_Elapsed;

            Input.DataAvailable += Input_DataAvailable;
            Sample = 0;

            StartTriggerChanged += Plotter_StartTriggerChanged;
            StopTriggerChanged += Plotter_StopTriggerChanged;

            LoadData();
        }

        private void Plotter_StartTriggerChanged(object sender, EventArgs e)
        {
            Data.GetTimer.Start();
        }

        private void Plotter_StopTriggerChanged(object sender, EventArgs e)
        {
            Data.GetTimer.Stop();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var val = Sample;

            if (Data.Scale.Enabled)
            {
                val = Map(val, 0, 32768, Data.Scale.Min, Data.Scale.Max);
            }
            if (Data.Limits.Enabled)
            {
                val = Math.Min(Data.Limits.Max, val);
                val = Math.Max(Data.Limits.Min, val);
            }
            if (Data.Filter.Enabled)
            {
                var lMin = Data.Limits.Enabled ? Data.Limits.Min : 0;
                var lMax = Data.Limits.Enabled ? Data.Limits.Max : 32768;
                val = val <= Data.Filter.Min ? lMin : val;
                val = val >= Data.Filter.Max ? lMax : val;
            }

            Value = val;
            Console.WriteLine(val);
            Data.Buffer.Add(val);
            ValueAvailable?.Invoke(this, null);
        }

        public void Start()
        {
            if (!started)
            {
                started = true;
                Input?.StartRecording();
            }
            if (!Data.StartTrigger.Enabled) Data.GetTimer.Start();
        }

        public void Stop()
        {
            if (started)
            {
                started = false;
                Input?.StopRecording();
            }
            if(!Data.StopTrigger.Enabled) Data.GetTimer.Stop();
        }

        public void Import(string file)
        {
            var fileData = System.IO.File.ReadAllText(file);
            var data = JsonConvert.DeserializeObject<List<int>>(fileData);
            if (data != null) Data.Buffer = data;
        }

        public void Export(string file)
        {
            var data = JsonConvert.SerializeObject(Data.Buffer);
            System.IO.File.WriteAllText(file, data);
        }

        public void SaveData(string file = "")
        {
            if(file == "")
            {
                Properties.Settings.Default.Cfg = JsonConvert.SerializeObject(Data);
                Properties.Settings.Default.Save();
            }
            else
            {
                System.IO.File.WriteAllText(file, JsonConvert.SerializeObject(Data));
            }
        }

        public void LoadData(string file = "")
        {
            if(file == "")
            {
                var data = JsonConvert.DeserializeObject<PlotterData>(Properties.Settings.Default.Cfg);
                Data = data ?? new PlotterData();
                Data.GetTimer.Elapsed += Timer_Elapsed;
            }
            else
            {
                var fileData = System.IO.File.ReadAllText(file);
                var data = JsonConvert.DeserializeObject<PlotterData>(fileData);
                if (data != null)
                {
                    Data = data;
                    Data.GetTimer.Elapsed += Timer_Elapsed;
                }
            }
        }

        private void Input_DataAvailable(object sender, WaveInEventArgs e)
        {
            for (int index = 0; index < e.BytesRecorded; index += 2)
            {
                Sample = (short)((e.Buffer[index + 1] << 8) |
                                        e.Buffer[index + 0]);

                if (Sample < 0) Sample = -Sample;
            }

            SampleAvailable?.Invoke(this, null);
            CheckTrigger(Data.StartTrigger, StartTriggerChanged);
            CheckTrigger(Data.StopTrigger, StopTriggerChanged);
        }

        private void CheckTrigger(Trigger trigger, EventHandler triggerEvent)
        {
            var val = trigger.UseFilter ? Value : Sample;
            if (trigger.Enabled)
            {
                switch (trigger.Condition)
                {
                    case TriggerCondition.Less:
                        if(trigger.Value > val) triggerEvent.Invoke(trigger, null);
                        break;
                    case TriggerCondition.Equal:
                        if(trigger.Value == val) triggerEvent.Invoke(trigger, null);
                        break;
                    case TriggerCondition.Greater:
                        if(trigger.Value < val) triggerEvent.Invoke(trigger, null);
                        break;
                    default:
                        break;
                }
            }
        }

        private int Map(int s, int a1, int a2, int b1, int b2)
        {
            return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
        }
    }
}
