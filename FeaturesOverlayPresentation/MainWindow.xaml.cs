using ConstantsDLL;
using HardwareInformation;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace FeaturesOverlayPresentation
{
    /// <summary>
    /// Interação lógica para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int furthestCount = 0, counter = 0;
        private int timerTickCount, finalCount;
        private readonly int tickSeconds = 3;
        private bool empty = false;
        private string newFilePath;
        private List<string> imgList, labelList;
        private readonly List<string[]> parametersList;
        private readonly BlurEffect blurEffect1;
        private ReinstallError e;
        private DispatcherTimer timer;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        private const int GWL_EX_STYLE = -20;
        private const int WS_EX_APPWINDOW = 0x00040000, WS_EX_TOOLWINDOW = 0x00000080;

        //Main Window constructor
        public MainWindow(List<string[]> parametersList)
        {
            InitializeComponent();
            _ = new Microsoft.Xaml.Behaviors.DefaultTriggerAttribute(typeof(Trigger), typeof(Microsoft.Xaml.Behaviors.TriggerBase), null);
            blurEffect1 = FindName("BlurImage") as BlurEffect;
            blurEffect1.Radius = 5;
            ButtonPrevious.Visibility = Visibility.Hidden;
            
            TextAppVersion.Text = "v" + MiscMethods.Version;
            FindImages();
            FindLabels();
            LabelPrint();
            ChangeSource(mainImage, new BitmapImage(new Uri(imgList[counter])), TimeSpan.FromSeconds(StringsAndConstants.FADE_TIME), TimeSpan.FromSeconds(StringsAndConstants.FADE_TIME));
            mainImage.Visibility = Visibility.Visible;
            MiscMethods.RegRecreate(empty);

            this.parametersList = parametersList;

            try
            {
                foreach (string item in labelList)
                {
                    _ = ComboBoxNavigate.Items.Add(item.Remove(0, 5));
                }

                ComboBoxNavigate.SelectedIndex = 0;
            }
            catch
            {
                e = new ReinstallError();
                e.Show();
                Close();
                empty = true;
            }
        }

        //Form loaded event handler
        private void FormLoaded(object sender, RoutedEventArgs args)
        {
            if (!MiscMethods.RegCheck())
            {
                //Variable to hold the handle for the form
                IntPtr helper = new WindowInteropHelper(this).Handle;
                //Performing some magic to hide the form from Alt+Tab
                _ = SetWindowLong(helper, GWL_EX_STYLE, (GetWindowLong(helper, GWL_EX_STYLE) | WS_EX_TOOLWINDOW) & ~WS_EX_APPWINDOW);
                Topmost = true;
                ShowInTaskbar = false;
                KeyDown += OnPreviewKeyDown;
                TimerTickCreation();
                ExitPresentationButton.Visibility = Visibility.Hidden;
                AboutButton.Visibility = Visibility.Hidden;
                ComboBoxNavigate.Visibility = Visibility.Hidden;
            }
            else
            {
                Topmost = false;
                ShowInTaskbar = true;
                ExitPresentationButton.Visibility = Visibility.Visible;
                AboutButton.Visibility = Visibility.Visible;
                TextStandBy.Visibility = Visibility.Hidden;
                ComboBoxNavigate.Visibility = Visibility.Visible;
            }

        }

        //Deny Alt+F4 exiting
        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.System && e.SystemKey == Key.F4)
            {
                e.Handled = true;
            }
        }

        //Prints the slide counter on the screen
        private void LabelPrint()
        {
            LabelPage.Content = counter + 1 + " de " + finalCount;
        }

        //Finds and creates a list of the image file names found inside the specified folder
        public void FindLabels()
        {
            finalCount = 0;
            string imgDir = Directory.GetCurrentDirectory() + ConstantsDLL.Properties.Resources.resourcesDir + ConstantsDLL.Properties.Resources.imgDir;
            try
            {
                List<string> filePathList = Directory.GetFiles(imgDir).ToList();
                labelList = new List<string>();
                foreach (string filePath in filePathList)
                {
                    if (Path.GetFileName(filePath).ToLower().Contains(ConstantsDLL.Properties.Resources.imgExt))
                    {
                        newFilePath = filePath.Replace(ConstantsDLL.Properties.Resources.imgExt, string.Empty);
                        labelList.Add(Path.GetFileName(newFilePath));
                        finalCount++;
                    }
                }
                if (finalCount == 0)
                {
                    e = new ReinstallError();
                    e.Show();
                    Close();
                    empty = true;
                }
            }
            catch
            {
                e = new ReinstallError();
                e.Show();
                Close();
                empty = true;
            }
        }

        //Finds and creates a list of the slide images found inside the specified folder
        public void FindImages()
        {
            finalCount = 0;
            string imgDir = Directory.GetCurrentDirectory() + ConstantsDLL.Properties.Resources.resourcesDir + ConstantsDLL.Properties.Resources.imgDir;
            try
            {
                List<string> filePathList = Directory.GetFiles(imgDir).ToList();
                imgList = new List<string>();
                foreach (string filePath in filePathList)
                {
                    if (Path.GetFileName(filePath).ToLower().Contains(ConstantsDLL.Properties.Resources.imgExt))
                    {
                        imgList.Add(filePath);
                        finalCount++;
                    }
                }
                if (finalCount == 0)
                {
                    e = new ReinstallError();
                    e.Show();
                    Close();
                    empty = true;
                }
            }
            catch
            {
                e = new ReinstallError();
                e.Show();
                Close();
                empty = true;
            }
        }

        //Prints filename label next to each slide
        private void SlideSubTitlePrint(int counter, bool flag)
        {
            string str;
            if (flag)
            {
                str = ComboBoxNavigate.Items.GetItemAt(counter).ToString();
                LabelSlideSubtitle.Content = str;
            }
            else
            {
                LabelSlideSubtitle.Content = string.Empty;
            }
        }

        //Define 'next' button name
        private void NextTextPrint()
        {
            nextBlock.Text = ConstantsDLL.Properties.Strings.nextText;
        }

        //Define 'finish' button name
        private void FinishTextPrint()
        {
            nextBlock.Text = ConstantsDLL.Properties.Strings.finishText;
        }

        //Creates a timer for each slide when running for the first time
        private void TimerTickCreation()
        {
            timerTickCount = tickSeconds;
            timer = new DispatcherTimer
            {
                Interval = new TimeSpan(0, 0, 1)
            };
            timer.Tick += new EventHandler(TimerTickRun);
            timer.Start();
            ButtonNext.Visibility = Visibility.Hidden;
            TextStandBy.Visibility = Visibility.Visible;
        }

        //Runs the previous created timer
        private void TimerTickRun(object sender, EventArgs e)
        {
            TextStandBy.Text = ConstantsDLL.Properties.Strings.waitText + "(" + timerTickCount.ToString() + ")";
            ButtonNext.Visibility = Visibility.Hidden;
            if (--timerTickCount == 0)
            {
                timer.Stop();
                TextStandBy.Text = ConstantsDLL.Properties.Strings.waitText + "(" + (tickSeconds + 1) + ")";
                if (counter == finalCount - 1)
                {
                    FinishTextPrint();
                }
                else
                {
                    NextTextPrint();
                }

                TextStandBy.Visibility = Visibility.Hidden;
                ButtonNext.Visibility = Visibility.Visible;
            }
        }

        //Changes the image source, adding fade-in/fade-out animation
        private void ChangeSource(Image image, ImageSource source, TimeSpan fadeOutTime, TimeSpan fadeInTime)
        {
            if (Environment.OSVersion.Version.Major.ToString().Contains(ConstantsDLL.Properties.Resources.win10ntMajor))
            {
                DoubleAnimation fadeInAnimation = new DoubleAnimation(1d, fadeInTime);

                if (image.Source != null)
                {
                    DoubleAnimation fadeOutAnimation = new DoubleAnimation(0d, fadeOutTime);

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

        //When the 'next' button is pressed
        private void ButtonNext_Click(object sender, RoutedEventArgs e)
        {
            if (counter == furthestCount && !MiscMethods.RegCheck())
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
                FinishTextPrint();
                ComboBoxNavigate.SelectedIndex = counter;
                SlideSubTitlePrint(counter, false);
            }
            else
            {
                MiscMethods.RegDelete();
                RegistryKey key = Registry.CurrentUser.CreateSubKey(ConstantsDLL.Properties.Resources.FopRegKey);
                key.SetValue(ConstantsDLL.Properties.Resources.DidItRunAlready, 1);
                File.Delete(StringsAndConstants.credentialsFilePath);
                Application.Current.Shutdown();
            }
        }

        //When clicking on the arms pictures, opens its sites
        private void Logo2_MouseLeftButtonUp(object sender, RoutedEventArgs e)
        {
            _ = Process.Start(parametersList[2][0]);
        }

        //When clicking on the arms pictures, opens its sites
        private void Logo1_MouseLeftButtonUp(object sender, RoutedEventArgs e)
        {
            _ = Process.Start(parametersList[1][0]);
        }

        //When clicking on the arms pictures, opens its sites
        private void Logo3_MouseLeftButtonUp(object sender, RoutedEventArgs e)
        {
            _ = Process.Start(parametersList[3][0]);
        }

        //When the 'previous' button is pressed
        private void ButtonPrevious_Click(object sender, RoutedEventArgs e)
        {
            if (counter > 1)
            {
                counter--;
                LabelPrint();
                NextTextPrint();
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
            else if (counter == 1)
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
            AboutBox aboutForm = new AboutBox();
            _ = aboutForm.ShowDialog();
        }

        //When the 'exit' button is pressed
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            File.Delete(StringsAndConstants.credentialsFilePath);
            Application.Current.Shutdown();
        }

        //When a combobox item is selected
        private void ComboBoxNavigate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            counter = ComboBoxNavigate.SelectedIndex;
            LabelPrint();
            if (counter > 0 && counter < finalCount - 1)
            {
                ButtonPrevious.Visibility = Visibility.Visible;
                ChangeSource(mainImage, new BitmapImage(new Uri(imgList[counter])), TimeSpan.FromSeconds(StringsAndConstants.FADE_TIME), TimeSpan.FromSeconds(StringsAndConstants.FADE_TIME));
                SlideSubTitlePrint(counter, true);
                NextTextPrint();
            }
            else if (counter == 0)
            {
                ButtonPrevious.Visibility = Visibility.Hidden;
                ChangeSource(mainImage, new BitmapImage(new Uri(imgList[counter])), TimeSpan.FromSeconds(StringsAndConstants.FADE_TIME), TimeSpan.FromSeconds(StringsAndConstants.FADE_TIME));
                SlideSubTitlePrint(counter, false);
                NextTextPrint();
            }
            else if (counter == finalCount - 1)
            {
                ButtonPrevious.Visibility = Visibility.Visible;
                ChangeSource(mainImage, new BitmapImage(new Uri(imgList[counter])), TimeSpan.FromSeconds(StringsAndConstants.FADE_TIME), TimeSpan.FromSeconds(StringsAndConstants.FADE_TIME));
                SlideSubTitlePrint(counter, false);
                FinishTextPrint();
            }
        }
    }
}
