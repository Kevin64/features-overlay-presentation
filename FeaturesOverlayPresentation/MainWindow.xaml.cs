using Microsoft.Win32;
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
using ConstantsDLL;
using System.Windows.Controls;
using System.Xml.Linq;
using System.Windows.Media.Effects;
using System.Windows.Navigation;
using System.Windows.Media;
using System.Diagnostics;

namespace FeaturesOverlayPresentation
{
    /// <summary>
    /// Interação lógica para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int furthestCount = 0;
        private int timerTickCount;
        private int tickSeconds = 3;
        private int counter = 0;
        private int finalCount;
        private bool empty = false;
        private string newFilePath;
        private DispatcherTimer timer;
        private BlurEffect blurEffect1;
        List<string> imgList, labelList;
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
            var _ = new Microsoft.Xaml.Behaviors.DefaultTriggerAttribute(typeof(Trigger), typeof(Microsoft.Xaml.Behaviors.TriggerBase), null);
            blurEffect1 = this.FindName("BlurImage") as BlurEffect;
            blurEffect1.Radius = 5;
            ButtonPrevious.Visibility = Visibility.Hidden;
            LabelPrint();
            TextAppVersion.Text = "v" + MiscMethods.Version;
            FindImages();
            FindLabels();
            ChangeSource(mainImage, new BitmapImage(new Uri(imgList[counter])), TimeSpan.FromSeconds(StringsAndConstants.FADE_TIME), TimeSpan.FromSeconds(StringsAndConstants.FADE_TIME));
            mainImage.Visibility = Visibility.Visible;
            MiscMethods.regRecreate(empty);

            try
            {
                foreach (string item in labelList)
                    ComboBoxNavigate.Items.Add(item.Remove(0, 5));
                ComboBoxNavigate.SelectedIndex = ComboBoxNavigate.Items.IndexOf(StringsAndConstants.introScreen);
            }
            catch
            {
                e = new ReinstallError();
                e.Show();
                this.Close();
                empty = true;
            }
        }

        //Form loaded event handler
        void FormLoaded(object sender, RoutedEventArgs args)
        {
            if(!MiscMethods.regCheck())
            {
                //Variable to hold the handle for the form
                var helper = new WindowInteropHelper(this).Handle;
                //Performing some magic to hide the form from Alt+Tab
                SetWindowLong(helper, GWL_EX_STYLE, (GetWindowLong(helper, GWL_EX_STYLE) | WS_EX_TOOLWINDOW) & ~WS_EX_APPWINDOW);
                this.Topmost = true;
                this.ShowInTaskbar = false;
                this.KeyDown += OnPreviewKeyDown;
                TimerTickCreation();
                ExitButtonPresentation.Visibility = Visibility.Hidden;
                ComboBoxNavigate.Visibility = Visibility.Hidden;
            }      
            else
            {
                this.Topmost = false;
                this.ShowInTaskbar = true;
                ExitButtonPresentation.Visibility = Visibility.Visible;
                TextStandBy.Visibility = Visibility.Hidden;
                ComboBoxNavigate.Visibility = Visibility.Visible;
            }
            
        }

        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.System && e.SystemKey == Key.F4)
                e.Handled = true;
        }

        private void LabelPrint()
        {
            LabelPage.Content = (counter + 1) + " de " + finalCount;
        }

        public void FindLabels()
        {
            finalCount = 0;
            string imgDir = MiscMethods.OSCheck();
            try
            {
                List<string> filePathList = Directory.GetFiles(imgDir).ToList();
                labelList = new List<string>();
                foreach (string filePath in filePathList)
                {
                    if (Path.GetFileName(filePath).ToLower().Contains(StringsAndConstants.imgExt))
                    {
                        newFilePath = filePath.Replace(StringsAndConstants.imgExt, "");
                        labelList.Add(Path.GetFileName(newFilePath));
                        finalCount++;
                    }
                }
                if (finalCount == 0)
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
        }

        public void FindImages()
        {
            finalCount = 0;
            string imgDir = MiscMethods.OSCheck();
            try
            {
                List<string> filePathList = Directory.GetFiles(imgDir).ToList();
                imgList = new List<string>();
                foreach (string filePath in filePathList)
                {
                    if (Path.GetFileName(filePath).ToLower().Contains(StringsAndConstants.imgExt))
                    {
                        imgList.Add(filePath);
                        finalCount++;
                    }
                }
                if (finalCount == 0)
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
        }

        private void SlideSubTitlePrint(int counter, bool flag)
        {
            string str;
            if(flag)
            {
                str = ComboBoxNavigate.Items.GetItemAt(counter).ToString();
                LabelSlideSubtitle.Content = str;
            }                
            else
                LabelSlideSubtitle.Content = "";
        }

        private void nextTextPrint()
        {
            nextBlock.Text = StringsAndConstants.nextText;
        }

        private void finishTextPrint()
        {
            nextBlock.Text = StringsAndConstants.finishText;
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
            TextStandBy.Text = StringsAndConstants.waitText + "(" + timerTickCount.ToString() + ")";
            ButtonNext.Visibility = Visibility.Hidden;
            if (--timerTickCount == 0)
            {
                timer.Stop();
                TextStandBy.Text = StringsAndConstants.waitText + "(" + (tickSeconds + 1) + ")";
                if (counter == finalCount - 1)
                    finishTextPrint();
                else
                    nextTextPrint();
                TextStandBy.Visibility = Visibility.Hidden;
                ButtonNext.Visibility = Visibility.Visible;
            }
        }

        private void ChangeSource(Image image, ImageSource source, TimeSpan fadeOutTime, TimeSpan fadeInTime)
        {
            if (Environment.OSVersion.Version.Major.ToString().Contains(StringsAndConstants.win10ntMajor))
            {
                var fadeInAnimation = new DoubleAnimation(1d, fadeInTime);

                if (image.Source != null)
                {
                    var fadeOutAnimation = new DoubleAnimation(0d, fadeOutTime);

                    fadeOutAnimation.Completed += (o, e) =>
                    {
                        image.Source = source;
                        image.BeginAnimation(Image.OpacityProperty, fadeInAnimation);
                    };

                    image.BeginAnimation(Image.OpacityProperty, fadeOutAnimation);
                }
                else
                {
                    image.Opacity = 0d;
                    image.Source = source;
                    image.BeginAnimation(Image.OpacityProperty, fadeInAnimation);
                }
            }
            else
            {
                image.Source = source;
            }
        }

        private void ButtonNext_Click(object sender, RoutedEventArgs e)
        {
            if (counter == furthestCount && !MiscMethods.regCheck())
            {
                TimerTickCreation();
                furthestCount++;
            }
            if (counter < finalCount - 2)
            {
                counter++;
                ChangeSource(mainImage, new BitmapImage(new Uri(imgList[counter])), TimeSpan.FromSeconds(StringsAndConstants.FADE_TIME), TimeSpan.FromSeconds(StringsAndConstants.FADE_TIME));
                LabelPrint();
                ButtonPrevious.Visibility = Visibility.Visible;
                ComboBoxNavigate.SelectedIndex = counter;
                SlideSubTitlePrint(counter, true);
            }
            else if (counter == finalCount - 2)
            {
                counter++;
                ChangeSource(mainImage, new BitmapImage(new Uri(imgList[counter])), TimeSpan.FromSeconds(StringsAndConstants.FADE_TIME), TimeSpan.FromSeconds(StringsAndConstants.FADE_TIME));
                LabelPrint();
                finishTextPrint();
                ComboBoxNavigate.SelectedIndex = counter;
                SlideSubTitlePrint(counter, false);
            }
            else
            {
                MiscMethods.regDelete();
                RegistryKey key = Registry.CurrentUser.CreateSubKey(StringsAndConstants.FopRegKey);
                key.SetValue(StringsAndConstants.DidItRunAlready, 1);
                File.Delete(StringsAndConstants.loginPath);
                Application.Current.Shutdown();
            }
        }

        private void brasaoSTI_MouseLeftButtonUp(object sender, RoutedEventArgs e)
        {
            Process.Start(StringsAndConstants.STI_URL);
        }

        private void brasaoUFSM_MouseLeftButtonUp(object sender, RoutedEventArgs e)
        {
            Process.Start(StringsAndConstants.UFSM_URL);
        }

        private void brasaoCCSH_MouseLeftButtonUp(object sender, RoutedEventArgs e)
        {
            Process.Start(StringsAndConstants.CCSH_URL);
        }

        private void ButtonPrevious_Click(object sender, RoutedEventArgs e)
        {
            if (counter > 1)
            {
                counter--;
                LabelPrint();
                nextTextPrint();
                ChangeSource(mainImage, new BitmapImage(new Uri(imgList[counter - 1])), TimeSpan.FromSeconds(StringsAndConstants.FADE_TIME), TimeSpan.FromSeconds(StringsAndConstants.FADE_TIME));
                ComboBoxNavigate.SelectedIndex = counter;
                SlideSubTitlePrint(counter, true);
            }
            else if (counter == finalCount - 1)
            {
                counter--;
                LabelPrint();
                ChangeSource(mainImage, new BitmapImage(new Uri(imgList[counter - 1])), TimeSpan.FromSeconds(StringsAndConstants.FADE_TIME), TimeSpan.FromSeconds(StringsAndConstants.FADE_TIME));                
            }
            else if(counter == 1)
            {
                counter--;
                LabelPrint();
                ButtonPrevious.Visibility = Visibility.Hidden;
                ChangeSource(mainImage, null, TimeSpan.FromSeconds(StringsAndConstants.FADE_TIME), TimeSpan.FromSeconds(StringsAndConstants.FADE_TIME));                
                ComboBoxNavigate.SelectedIndex = counter;
                SlideSubTitlePrint(counter, false);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            File.Delete(StringsAndConstants.loginPath);
            Application.Current.Shutdown();
        }

        private void ComboBoxNavigate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            counter = ComboBoxNavigate.SelectedIndex;
            LabelPrint();
            if (counter > 0 && counter < finalCount - 1)
            {
                ButtonPrevious.Visibility = Visibility.Visible;
                ChangeSource(mainImage, new BitmapImage(new Uri(imgList[counter])), TimeSpan.FromSeconds(StringsAndConstants.FADE_TIME), TimeSpan.FromSeconds(StringsAndConstants.FADE_TIME));
                SlideSubTitlePrint(counter, true);
                nextTextPrint();
            }
            else if (counter == 0)
            {
                ButtonPrevious.Visibility = Visibility.Hidden;
                ChangeSource(mainImage, new BitmapImage(new Uri(imgList[counter])), TimeSpan.FromSeconds(StringsAndConstants.FADE_TIME), TimeSpan.FromSeconds(StringsAndConstants.FADE_TIME));
                SlideSubTitlePrint(counter, false);
                nextTextPrint();
            }
            else if (counter == finalCount - 1)
            {
                ButtonPrevious.Visibility = Visibility.Visible;
                ChangeSource(mainImage, new BitmapImage(new Uri(imgList[counter])), TimeSpan.FromSeconds(StringsAndConstants.FADE_TIME), TimeSpan.FromSeconds(StringsAndConstants.FADE_TIME));
                SlideSubTitlePrint(counter, false);
                finishTextPrint();
            }
        }
    }
}
