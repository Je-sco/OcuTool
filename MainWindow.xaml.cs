using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Windows;
using Windows.Graphics.Display;

namespace OcuTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        RegistryItem codec = new RegistryItem("HEVC", "DWORD");
        RegistryItem bitrate = new RegistryItem("BitrateMbps", "String");   
        RegistryItem localDimming = new RegistryItem("LocalDimming", "DWORD");
        RegistryItem DistortionCurve = new RegistryItem("DistortionCurve", "DWORD");
        RegistryItem linkSharp = new RegistryItem("LinkSharpeningEnabled", "DWORD");

        public MainWindow()
        {
            InitializeComponent();

            VisualSetup();
        }

        private void VisualSetup()  // Set all of the checkboxes to their state in the registry
        {
            LimitCodecCheckbox();
            LimitSharpeningCheckbox();
            LimitDistortionCheckbox();

            bitrateSlider.Value = bitrate.getRegistryValue();

            if (localDimming.getRegistryValue() == 1)
            {
                localDimmingCheckbox.IsChecked = true;
            }
        }
        
        private void LimitCodecCheckbox() // Limit the codec checkboxes to only one field, so only one of them can be checked at a time.
        {
            int codecValue = codec.getRegistryValue();

            switch (codecValue)
                {
                case -1:
                    autoCodecCheckbox.IsChecked = true;
                    H264checkbox.IsChecked = false;
                    H265checkbox.IsChecked = false;

                    bitrateSlider.Maximum = 1000;
                    break;
                case 0:
                    autoCodecCheckbox.IsChecked = false;
                    H264checkbox.IsChecked = true;
                    H265checkbox.IsChecked = false;
                   
                    bitrateSlider.Maximum = 1000;
                    break;
                case 1:
                    autoCodecCheckbox.IsChecked = false;
                    H264checkbox.IsChecked = false;
                    H265checkbox.IsChecked = true;
                    
                    bitrateSlider.Maximum = 300;
                    if (bitrate.getRegistryValue() > 300)
                    {
                        bitrate.setValue(300);
                    }
                    break;
                } 
        }

        private void LimitSharpeningCheckbox()
        {
            int sharpValue = linkSharp.getRegistryValue();

            switch (sharpValue)
            {
                case -1:
                case 2:
                    offSharp.IsChecked = false;
                    normalSharp.IsChecked = true;
                    qualitySharp.IsChecked = false;
                    break;
                case 1:
                    offSharp.IsChecked = true;
                    normalSharp.IsChecked = false;
                    qualitySharp.IsChecked = false;
                    break;
                case 3:
                    offSharp.IsChecked = false;
                    normalSharp.IsChecked = false;
                    qualitySharp.IsChecked = true;
                    break;
            }
        }
        
        private void LimitDistortionCheckbox()
        {
            int distortionValue = DistortionCurve.getRegistryValue();
            
            switch (distortionValue)
            {
                case 0:
                    lowDistortion.IsChecked = true;
                    highDistortion.IsChecked = false;
                    break;
                case -1:
                case 1:
                    lowDistortion.IsChecked = false;
                    highDistortion.IsChecked = true;
                    break;
            }
        }

        private void autoCodecCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            codec.setValue(-1);
            LimitCodecCheckbox();
        }

        private void H264checkbox_Checked(object sender, RoutedEventArgs e)
        {
            codec.setValue(0);
            LimitCodecCheckbox();
        }

        private void H265checkbox_Checked(object sender, RoutedEventArgs e)
        {
            codec.setValue(1);
            LimitCodecCheckbox();
        }

        private void bitrateSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)   
        {
            bitrate.setValue(Convert.ToInt32(bitrateSlider.Value));
        }

        private void localDimmingCheckbox_Clicked(object sender, RoutedEventArgs e)
        {
            if (localDimmingCheckbox.IsChecked == false)
            {
                localDimming.setValue(0);
            }
            else
            {
                localDimming.setValue(1);
            }
        }

        private void lowDistortion_Checked(object sender, RoutedEventArgs e)
        {
            DistortionCurve.setValue(0);
            LimitDistortionCheckbox();
        }

        private void highDistortion_Checked(object sender, RoutedEventArgs e)
        {
            DistortionCurve.setValue(1);
            LimitDistortionCheckbox();
        }

        private void offSharp_Checked(object sender, RoutedEventArgs e)
        {
            linkSharp.setValue(1);
            LimitSharpeningCheckbox();

        }

        private void normalSharp_Checked(object sender, RoutedEventArgs e)
        {
            linkSharp.setValue(2);
            LimitSharpeningCheckbox();

        }

        private void qualitySharp_Checked(object sender, RoutedEventArgs e)
        {
            linkSharp.setValue(3);
            LimitSharpeningCheckbox();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            String helpText = @"Codec: H264 allows higher bitrates, but H265 is more efficient. Generally use H264 if you can achieve bitrates above 300 Mbps, otherwise use H265. Auto uses H264 over Link, and H265 over AirLink." + "\n" +
            @"
Distortion Curvature: High focuses the pixel density more in the middle" + "\n" +
            @"
Bitrate: How many Megabits are sent per second. A higher bitrate may increase latency but also reduces compression artifacts. Over Link the Quests decoder will be the limiting factor, over AirLink it's likely your WiFi setup." + "\n" +
            @"
Link Sharpening: Quality setting uses Meta Quest Super Resolution, a spatial upscaler similar to FSR 1.0 and Virtual Desktops SDSR." + "\n" +
            @"
Local Dimming: Only matters on Quest Pro. Local Dimming greatly improves contrast but can introduce blooming and increases display persistence";

            MessageBox.Show(helpText);
        }
    }
}
