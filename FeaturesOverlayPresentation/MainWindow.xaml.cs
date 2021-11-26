﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace FeaturesOverlayPresentation
{
    /// <summary>
    /// Interação lógica para MainWindow.xam
    /// </summary>
    public partial class MainWindow : Window
    {
        private int furthestCount = 0;
        private int timerTickCount;
        private int tickSeconds = 3;
        private int counter = 0;
        private int finalCount;
        private bool empty = false;
        private DispatcherTimer timer;
        List<string> imgList;
        ReinstallError e;

        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        private const int GWL_EX_STYLE = -20;
        private const int WS_EX_APPWINDOW = 0x00040000, WS_EX_TOOLWINDOW = 0x00000080;
        
        public MainWindow()
        {
            InitializeComponent();
            this.KeyDown += OnPreviewKeyDown;
            TimerTickCreation();
            ButtonPrevious.Visibility = Visibility.Hidden;
            frameIntro.Content = new Intro();
            frameIntro.Visibility = Visibility.Visible;
            frameEnd.Content = new Ending();
            frameEnd.Visibility = Visibility.Hidden;
            FindImages();
            LabelPrint();
            TextAppVersion.Text = "v" + Version;
            AnimateFrame();
            regRecreate();
        }

        //Form loaded event handler
        void FormLoaded(object sender, RoutedEventArgs args)
        {
            //Variable to hold the handle for the form
            var helper = new WindowInteropHelper(this).Handle;
            //Performing some magic to hide the form from Alt+Tab
            SetWindowLong(helper, GWL_EX_STYLE, (GetWindowLong(helper, GWL_EX_STYLE) | WS_EX_TOOLWINDOW) & ~WS_EX_APPWINDOW);
        }

        public string Version
        {
            get
            {
                return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.System && e.SystemKey == Key.F4)
                e.Handled = true;
        }

        private void LabelPrint()
        {
            LabelPage.Content = (counter + 1) + " de " + (finalCount + 1);
        }

        private void NextPrint()
        {
            nextBlock.Text = "Próximo";
        }

        private void FinishPrint()
        {
            nextBlock.Text = "Finalizar";
        }

        private void TimerTickCreation()
        {
            timerTickCount = tickSeconds;
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Tick += new EventHandler(TimerTickRun);
            timer.Start();
            ButtonNext.Visibility = Visibility.Hidden;
            TextStandBy.Visibility = Visibility.Visible;
        }

        private void TimerTickRun(object sender, EventArgs e)
        {
            //DispatcherTimer timer = (DispatcherTimer)sender;
            TextStandBy.Text = "Aguarde \n" + "(" + timerTickCount.ToString() + ")";
            ButtonNext.Visibility = Visibility.Hidden;
            if (--timerTickCount == 0)
            {
                timer.Stop();
                TextStandBy.Text = "Aguarde \n" + "(" + (tickSeconds + 1) + ")";
                if (counter == finalCount)
                    FinishPrint();
                else
                    NextPrint();
                TextStandBy.Visibility = Visibility.Hidden;
                ButtonNext.Visibility = Visibility.Visible;
            }
        }

        public void FindImages()
        {
            string current = Directory.GetCurrentDirectory();
            string imgDir = current + "\\img\\";
            try
            {
                List<string> filePathList = Directory.GetFiles(imgDir).ToList();
                imgList = new List<string>();
                foreach (string filePath in filePathList)
                {
                    if (System.IO.Path.GetFileName(filePath).ToLower().Contains(".png"))
                    {
                        imgList.Add(filePath);
                        finalCount++;
                    }
                }
                if(finalCount == 0)
                {
                    e = new ReinstallError();
                    e.Show();
                    this.Close();
                    empty = true;
                }
            }
            catch
            {
                e = new ReinstallError();
                e.Show();
                this.Close();
                empty = true;
            }
            finalCount++;
        }

        void AnimateFrame()
        {
            DoubleAnimation da = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = new Duration(TimeSpan.FromSeconds(1)),
                AutoReverse = false
            };
            frameIntro.BeginAnimation(OpacityProperty, da);
            frameEnd.BeginAnimation(OpacityProperty, da);
            mainImage.BeginAnimation(OpacityProperty, da);
        }

        private void ButtonNext_Click(object sender, RoutedEventArgs e)
        {
            AnimateFrame();
            if (counter == furthestCount)
            {
                TimerTickCreation();
                furthestCount++;
            }
            frameIntro.Visibility = Visibility.Hidden;
            if (counter < finalCount - 1)
            {
                mainImage.Source = new BitmapImage(new Uri(imgList[counter]));
                counter++;
                LabelPrint();
                ButtonPrevious.Visibility = Visibility.Visible;
            }
            else if (counter + 1 == finalCount)
            {
                counter++;
                frameEnd.Visibility = Visibility.Visible;
                mainImage.Source = null;
                LabelPrint();
                FinishPrint();
            }
            else
            {
                regDelete();
                RegistryKey key = Registry.CurrentUser.CreateSubKey(@"Software\FOP");
                key.SetValue("DidItRunAlready", 1);
                Environment.Exit(0);
            }
        }

        private void ButtonPrevious_Click(object sender, RoutedEventArgs e)
        {
            AnimateFrame();
            frameEnd.Visibility = Visibility.Hidden;
            if (counter > 1)
            {
                counter--;
                LabelPrint();
                NextPrint();
                mainImage.Source = new BitmapImage(new Uri(imgList[counter-1]));
            }
            else if (counter == finalCount)
            {
                counter--;
                LabelPrint();
                mainImage.Source = new BitmapImage(new Uri(imgList[counter-1]));
                if (!nextBlock.Text.Equals("Próximo"))
                    NextPrint();
            }
            else if(counter == 1)
            {
                counter--;
                ButtonPrevious.Visibility = Visibility.Hidden;
                mainImage.Source = null;
                frameIntro.Visibility = Visibility.Visible;
                LabelPrint();
                NextPrint();
            }
        }

        private void regRecreate()
        {
            if(!empty)
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\RunOnce", true);
                if (!key.GetValueNames().Contains("FOP"))
                {
                    if (Environment.Is64BitOperatingSystem == true)
                        key.SetValue("FOP", Environment.ExpandEnvironmentVariables("%ProgramFiles(x86)%") + "\\FOP" + "\\Rever tutorial de uso do computador.lnk", RegistryValueKind.String);
                    else
                        key.SetValue("FOP", Environment.ExpandEnvironmentVariables("%ProgramFiles%") + "\\FOP" + "\\Rever tutorial de uso do computador.lnk", RegistryValueKind.String);
                }
            }            
        }

        private void regDelete()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\RunOnce", true);
            if (key.GetValueNames().Contains("FOP"))
                key.DeleteValue("FOP");
        }
    }
}
