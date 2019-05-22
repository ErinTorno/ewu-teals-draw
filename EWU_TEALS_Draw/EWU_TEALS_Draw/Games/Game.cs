﻿using Emgu.CV;
using Emgu.CV.UI;
using EwuTeals.Detectables;
using EwuTeals.Games.States;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EwuTeals.Games {
   abstract class Game : IDisposable {
        private const double SecondsToAllowToggle = 0.5; // a half second

        /// <summary>
        /// If true, all key inputs will be ignored so that the container for this can address them
        /// </summary>
        public Boolean ShouldYieldKeys { get; protected set; }
        public Keys ToggleYieldKey = Keys.Space;

        public ObservableCollection<Detectable> Detectables { get; protected set; }

        public readonly State Intro;
        public readonly State Detect;
        public readonly State Ready;
        public readonly State Running;
        public readonly State Result;
        public readonly State End;
        private State CurState;

        protected double logicTime = 0;
        protected int logicTicks = 0;
        protected ImageBox canvas;
        protected Form form;
        // this key, when pressed, will toggle ShouldYieldKeys
        private double lastToggleTime = 0.0;

        protected Game(Form form, ImageBox canvas) {
            this.canvas = canvas;
            ShouldYieldKeys = false;
            this.form = form;
            form.KeyDown += OnKeyPress;
            Detectables = new ObservableCollection<Detectable>();
        }

        abstract public void DisplayIntro();
        abstract public void DetectColor();
        abstract public void PromptAddPlayer();
        abstract public void DisplayResults();
        abstract public void Exit();

        public void SetState (State state)
        {
            this.CurState = state;
        }

        public virtual void Dispose() {
            form.KeyDown -= OnKeyPress;
        }

        protected virtual void OnKeyPress(object sender, KeyEventArgs e) {
            if (e.KeyCode == ToggleYieldKey && logicTime >= lastToggleTime + SecondsToAllowToggle) {
                lastToggleTime = logicTime;
                ShouldYieldKeys = !ShouldYieldKeys;
            }
        }

        public virtual void Update(double dT, Mat input) {
            ++logicTicks;
            logicTime += dT;
        }
    }
}