using Emgu.CV;
using Emgu.CV.UI;
using EwuTeals.Detectables;
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

        public abstract void Reset();

        public virtual void Dispose() {
            form.KeyDown -= OnKeyPress;
        }

        public virtual void OnKeyPress(object sender, KeyEventArgs e) {
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
