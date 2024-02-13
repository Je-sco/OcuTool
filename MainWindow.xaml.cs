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
        RegistryItem codec = new RegistryItem("HEVC", "DWORD");
        RegistryItem bitrate = new RegistryItem("BitrateMbps", "String");
        RegistryItem localDimming = new RegistryItem("LocalDimming", "DWORD");
        RegistryItem DistortionCurve = new RegistryItem("DistortionCurve", "DWORD");

        public MainWindow()
        {
            InitializeComponent();

            visualSetup();
        }

        private void visualSetup()  // Set all of the checkboxes to their state in the registry
        {
            limitCodecCheckbox();
            limitDistortionCheckbox();

            bitrateSlider.Value = bitrate.getRegistryValue();

            if (localDimming.getRegistryValue() == 1)
            {
                localDimmingCheckbox.IsChecked = true;
            }
        }
        
        private void limitCodecCheckbox() // Limit the codec checkboxes to only one field, so only one of them can be checked at a time.
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
        
        private void limitDistortionCheckbox()
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
            limitCodecCheckbox();
        }

        private void H264checkbox_Checked(object sender, RoutedEventArgs e)
        {
            codec.setValue(0);
            limitCodecCheckbox();
        }

        private void H265checkbox_Checked(object sender, RoutedEventArgs e)
        {
            codec.setValue(1);
            limitCodecCheckbox();
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
            limitDistortionCheckbox();
        }

        private void highDistortion_Checked(object sender, RoutedEventArgs e)
        {
            DistortionCurve.setValue(1);
            limitDistortionCheckbox();
        }
    }
}
