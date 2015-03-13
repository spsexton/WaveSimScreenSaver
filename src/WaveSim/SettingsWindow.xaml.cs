using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WaveSim
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private RaindropSettings settings;

        public SettingsWindow()
        {
            InitializeComponent();

            // Load settings from file (or just set to default values
            // if file not found)
            settings = RaindropSettings.Load(RaindropSettings.SettingsFile);

            SetSliders();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            settings.Save(RaindropSettings.SettingsFile);
            this.Close();
        }

        /// <summary>
        /// Set all sliders to their default values
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDefaults_Click(object sender, RoutedEventArgs e)
        {
            settings.SetDefaults();
            SetSliders();
        }

        private void SetSliders()
        {
            slidNumDrops.Value = 1.0 / (settings.RaindropPeriodInMS / 1000.0);
            slidDropStrength.Value = -1.0 * settings.SplashAmplitude;
            slidDropSize.Value = settings.DropSize;
            slidDamping.Value = settings.Damping * 100;
        }

        private void slidDropStrength_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (settings != null)
            {
                // Slider runs [0,30], so our amplitude runs [-30,0].  
                // Negative amplitude is desirable because we see little towers of  
                // water as each drop bloops in.  
                settings.SplashAmplitude = -1.0 * slidDropStrength.Value;
            }
        }

        private void slidNumDrops_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (settings != null)
            {
                // Slider runs from [1,1000], with 1000 representing more drops (1 every ms) and  
                // 1 representing fewer (1 ever 1000 ms).  This is to make slider seem natural  
                // to user.  But we need to invert it, to get actual period (ms)  
                settings.RaindropPeriodInMS = (1.0 / slidNumDrops.Value) * 1000.0;
            }
        }

        private void slidDropSize_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (settings != null)
            {
                settings.DropSize = (int)slidDropSize.Value;
            }
        }

        private void slidDamping_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (settings != null)
            {
                settings.Damping = slidDamping.Value / 100;
            }
        }
    }
}
