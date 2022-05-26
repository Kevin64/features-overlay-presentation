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
            ButtonPrevious.Visibility = Visibility.Hidden;
            frameEnd.Content = new Ending();
            frameEnd.Visibility = Visibility.Hidden;
            LabelPrint();
            TextAppVersion.Text = "v" + MiscMethods.Version;
            AnimateFrame();
            FindImages();
            FindLabels();
            mainImage.Source = new BitmapImage(new Uri(imgList[counter]));
            mainImage.Visibility = Visibility.Visible;
            MiscMethods.regRecreate(empty);

            try
            {
                foreach (string item in labelList)
                    ComboBoxNavigate.Items.Add(item.Remove(0, 5));
                ComboBoxNavigate.Items.Add(StringsAndConstants.finaleScreen);
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
            LabelPage.Content = (counter + 1) + " de " + (finalCount + 1);
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
                if (counter == finalCount)
                    finishTextPrint();
                else
                    nextTextPrint();
                TextStandBy.Visibility = Visibility.Hidden;
                ButtonNext.Visibility = Visibility.Visible;
            }
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
            frameEnd.BeginAnimation(OpacityProperty, da);
            mainImage.BeginAnimation(OpacityProperty, da);
        }

        private void ButtonNext_Click(object sender, RoutedEventArgs e)
        {
            AnimateFrame();
            if (counter == furthestCount && !MiscMethods.regCheck())
            {
                TimerTickCreation();
                furthestCount++;
            }
            if (counter < finalCount - 1)
            {
                counter++;
                mainImage.Source = new BitmapImage(new Uri(imgList[counter]));                
                LabelPrint();
                ButtonPrevious.Visibility = Visibility.Visible;
                ComboBoxNavigate.SelectedIndex = counter;
                SlideSubTitlePrint(counter, true);
            }
            else if (counter + 1 == finalCount)
            {
                counter++;
                frameEnd.Visibility = Visibility.Visible;
                mainImage.Source = null;
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
                File.Delete(StringsAndConstants.fileLogin);
                Application.Current.Shutdown();
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
                nextTextPrint();
                mainImage.Source = new BitmapImage(new Uri(imgList[counter - 1]));
                ComboBoxNavigate.SelectedIndex = counter;
                SlideSubTitlePrint(counter, true);
            }
            else if (counter == finalCount)
            {
                counter--;
                LabelPrint();
                mainImage.Source = new BitmapImage(new Uri(imgList[counter - 1]));
                if (!nextBlock.Text.Equals(StringsAndConstants.nextText))
                    nextTextPrint();
            }
            else if(counter == 1)
            {
                counter--;
                ButtonPrevious.Visibility = Visibility.Hidden;
                mainImage.Source = null;
                LabelPrint();
                nextTextPrint();
                ComboBoxNavigate.SelectedIndex = counter;
                SlideSubTitlePrint(counter, false);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            File.Delete(StringsAndConstants.fileLogin);
            Application.Current.Shutdown();
        }

        private void ComboBoxNavigate_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            AnimateFrame();            
            counter = ComboBoxNavigate.SelectedIndex;
            LabelPrint();
            if (counter > 0 && counter < finalCount)
            {
                frameEnd.Visibility = Visibility.Hidden;
                ButtonPrevious.Visibility = Visibility.Visible;
                mainImage.Source = new BitmapImage(new Uri(imgList[counter]));
                SlideSubTitlePrint(counter, true);
                nextTextPrint();
            }
            else if (counter == 0)
            {
                frameEnd.Visibility = Visibility.Hidden;
                ButtonPrevious.Visibility = Visibility.Hidden;
                mainImage.Source = new BitmapImage(new Uri(imgList[counter]));
                SlideSubTitlePrint(counter, false);
                nextTextPrint();
            }
            else if (counter == finalCount)
            {
                frameEnd.Visibility = Visibility.Visible;
                ButtonPrevious.Visibility = Visibility.Visible;
                mainImage.Source = null;
                SlideSubTitlePrint(counter, false);
                finishTextPrint();
            }
        }
    }
}
