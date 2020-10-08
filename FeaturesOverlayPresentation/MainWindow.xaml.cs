using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
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
        private DispatcherTimer timer;
        private int counter = 0;
        private int finalCount;
        List<string> imgList;
        Error e;


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
            TextAppVersion.Text = Version + "-beta";
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
            {
                e.Handled = true;
            }
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

        void FindImages()
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
                    e = new Error();
                    e.Show();
                    this.Hide();
                }
            }
            catch
            {
                e = new Error();
                e.Show();
                this.Hide();
            }
            finalCount++;
        }

        private void ButtonNext_Click(object sender, RoutedEventArgs e)
        {
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
                Environment.Exit(0);
            }
        }

        private void ButtonPrevious_Click(object sender, RoutedEventArgs e)
        {
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
    }
}
