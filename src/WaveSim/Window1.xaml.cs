using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace WaveSim
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        private Vector3D zoomDelta;

        private WaveGrid _grid;
        private bool _rendering;
        private double _lastTimeRendered;
        private Random _rnd = new Random(1234);

        // Raindrop parameters, from .xml settings file
        private RaindropSettings _settings;
        
        private double _splashDelta = 1.0;      // Actual splash height is Ampl +/- Delta (random)
        private double _waveHeight = 15.0;

        // Values to try:
        //   GridSize=20, RenderPeriod=125
        //   GridSize=50, RenderPeriod=50
        private const int GridSize = 250; //50;    
        private const double RenderPeriodInMS = 60; //50; 

        public Window1()
        {
            InitializeComponent();

            // Read in settings from .xml file
            _settings = RaindropSettings.Load(RaindropSettings.SettingsFile);

            // Set up the grid
            _grid = new WaveGrid(GridSize);
            _grid.Damping = _settings.Damping;
            meshMain.Positions = _grid.Points;
            meshMain.TriangleIndices = _grid.TriangleIndices;

            // On each WheelMouse change, we zoom in/out a particular % of the original distance
            const double ZoomPctEachWheelChange = 0.02;
            zoomDelta = Vector3D.Multiply(ZoomPctEachWheelChange, camMain.LookDirection);

            StartStopRendering();
        }

        private void Window_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
                // Zoom in
                camMain.Position = Point3D.Add(camMain.Position, zoomDelta);
            else
                // Zoom out
                camMain.Position = Point3D.Subtract(camMain.Position, zoomDelta);
        }

        // Start/stop animation
        private void StartStopRendering()
        {
            if (!_rendering)
            {
                //_grid = new WaveGrid(GridSize);        // New grid allows buffer reset
                _grid.FlattenGrid();
                meshMain.Positions = _grid.Points;

                _lastTimeRendered = 0.0;
                CompositionTarget.Rendering += new EventHandler(CompositionTarget_Rendering);
                _rendering = true;
            }
            else
            {
                CompositionTarget.Rendering -= new EventHandler(CompositionTarget_Rendering);
                _rendering = false;
            }
        }

        void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            RenderingEventArgs rargs = (RenderingEventArgs)e;
            if ((rargs.RenderingTime.TotalMilliseconds - _lastTimeRendered) > RenderPeriodInMS)
            {
                // Unhook Positions collection from our mesh, for performance
                // (see http://blogs.msdn.com/timothyc/archive/2006/08/31/734308.aspx)
                meshMain.Positions = null;

                // Do the next iteration on the water grid, propagating waves
                double NumDropsThisTime = RenderPeriodInMS / _settings.RaindropPeriodInMS;
                
                // Result at this point for number of drops is something like
                // 2.25.  We'll induce integer portion (e.g. 2 drops), then
                // 25% chance for 3rd drop.
                int NumDrops = (int)NumDropsThisTime;   // trunc
                for (int i = 0; i < NumDrops; i++)
                    _grid.SetRandomPeak(_settings.SplashAmplitude, _splashDelta, _settings.DropSize);

                if ((NumDropsThisTime - NumDrops) > 0)
                {
                    double DropChance = NumDropsThisTime - NumDrops;
                    if (_rnd.NextDouble() <= DropChance)
                        _grid.SetRandomPeak(_settings.SplashAmplitude, _splashDelta, _settings.DropSize);
                }

                _grid.ProcessWater();

                // Then update our mesh to use new Z values
                meshMain.Positions = _grid.Points;

                _lastTimeRendered = rargs.RenderingTime.TotalMilliseconds;
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
