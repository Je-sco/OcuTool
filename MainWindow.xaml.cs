using Microsoft.Win32;
using System;
using System.Windows;

namespace OcuTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        const string DEBUG_KEY_PATH = "Software\\Oculus\\RemoteHeadset";
        RegistryKey DebugKey = Registry.CurrentUser.OpenSubKey(DEBUG_KEY_PATH, true);

        bool bitrateChangedByMe = true;
        bool distortionChangedByMe = true;

        public MainWindow()
        {
            InitializeComponent();
            getCurrentValues();
        }

        private void getCurrentValues()
        {
            getBitrate();
            getCodec();
            getLocalDimming();
            getDistortionCurvature();
        }

        private void getBitrate()
        {
            if (DebugKey.GetValue("BitrateMbps") == null)
            {
                updateBitrateSlider(0);
            }
            else
            {
                updateBitrateSlider(Convert.ToInt32(DebugKey.GetValue("BitrateMbps")));
            }
        }

        private void getLocalDimming()
        {
            if (DebugKey.GetValue("LocalDimming") == null || Convert.ToInt32(DebugKey.GetValue("LocalDimming")) == 1)
            {
                localDimmingCheckbox.IsChecked = true;
            }
            else
            {
                localDimmingCheckbox.IsChecked = false;
            }

        }

        private void getDistortionCurvature()
        {
            if (DebugKey.GetValue("DistortionCurve") == null || Convert.ToInt32(DebugKey.GetValue("DistortionCurve")) == 1) // If Distortion Curve is set to High or default
            {
                highDistortion.IsChecked = true;
            }
            else   // If low distortion curve
            {
                lowDistortion.IsChecked = true;
            }
        }

        private void getCodec()
        {
            if (DebugKey.GetValue("HEVC") == null)
            {
                autoCodecCheckbox.IsChecked = true;
            }
            else
            {
                int tempCodec = Convert.ToInt32(DebugKey.GetValue("HEVC"));
                if (tempCodec == 0) // If Set to H264
                {
                    H264checkbox.IsChecked = true;
                }
                else                // If set to H265
                {
                    H265checkbox.IsChecked = true;
                }
            }
        }

        private void autoCodecCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            H264checkbox.IsChecked = false;
            H265checkbox.IsChecked = false;
            bitrateSlider.Maximum = 960;
            if (!(DebugKey.GetValue("HEVC") == null))
            {
                DebugKey.DeleteValue("HEVC");
            }
        }

        private void H264checkbox_Checked(object sender, RoutedEventArgs e)
        {
            H265checkbox.IsChecked = false;
            autoCodecCheckbox.IsChecked = false;
            bitrateSlider.Maximum = 960;
            DebugKey.SetValue("HEVC", 0);
        }

        private void H265checkbox_Checked(object sender, RoutedEventArgs e)
        {
            H264checkbox.IsChecked = false;
            autoCodecCheckbox.IsChecked = false;
            bitrateSlider.Maximum = 200;
            if (bitrateSlider.Value > 200)
            {
                updateBitrateSlider(200);
                getBitrate();
            }
            DebugKey.SetValue("HEVC", 1);
        }

        private void updateBitrateSlider(int value)
        {
            bitrateChangedByMe = true;
            bitrateSlider.Value = value;
            bitrateChangedByMe = false;
        }

        private void bitrateSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)   
        {
            if (!(bitrateChangedByMe))
            {
                DebugKey.SetValue("BitrateMbps", bitrateSlider.Value);
                if (bitrateSlider.Value == 0)
                {
                    DebugKey.DeleteValue("BitrateMbps");
                }
            }
        }

        private void localDimmingCheckbox_Clicked(object sender, RoutedEventArgs e)
        {
            if (localDimmingCheckbox.IsChecked == false)
            {
                DebugKey.SetValue("LocalDimming", 0);
            }
            else
            {
                DebugKey.SetValue("LocalDimming", 1);
            }
        }

        private void lowDistortion_Checked(object sender, RoutedEventArgs e)
        {
            highDistortion.IsChecked = false;
            DebugKey.SetValue("DistortionCurve", 0);
        }

        private void highDistortion_Checked(object sender, RoutedEventArgs e)
        {
            lowDistortion.IsChecked = false;
            DebugKey.SetValue("DistortionCurve", 1);
        }

    }
}
