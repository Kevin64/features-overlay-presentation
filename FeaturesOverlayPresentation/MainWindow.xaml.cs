using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace FeaturesOverlayPresentation
{
    /// <summary>
    /// Interação lógica para MainWindow.xam
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool alreadyCount1 = false;
        private bool alreadyCount2 = false;
        private bool alreadyCount3 = false;
        private bool alreadyCount4 = false;
        private int timerTickCount;
        private DispatcherTimer timer;
        private int counter = 0;
        private int finalCount;
        List<string> imgList;


        public MainWindow()
        {
            InitializeComponent();
            this.KeyDown += OnPreviewKeyDown;
            TimerTickCreation();
            ButtonPrevious.IsEnabled = false;
            LabelPrint();
            FindImages();
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
            timerTickCount = 3;
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Tick += new EventHandler(TimerTickRun);
            timer.Start();
            ButtonNext.IsEnabled = false;
        }

        private void TimerTickRun(object sender, EventArgs e)
        {
            DispatcherTimer timer = (DispatcherTimer)sender;
            nextBlock.Text = "Próximo (" + timerTickCount.ToString() + ")";
            if (--timerTickCount == 0)
            {
                timer.Stop();
                if (counter == finalCount)
                    FinishPrint();
                else
                    NextPrint();
                ButtonNext.IsEnabled = true;
            }
        }

        void FindImages()
        {
            List<string> filePathList = Directory.GetFiles(Directory.GetCurrentDirectory()).ToList();
            imgList = new List<string>();
            foreach (string filePath in filePathList)
            {
                if (System.IO.Path.GetFileName(filePath).ToLower().Contains(".png"))
                {
                    imgList.Add(filePath);
                    finalCount++;
                }
            }
        }

        private void ButtonNext_Click(object sender, RoutedEventArgs e)
        {
            if (counter != finalCount)
            {                
                TimerTickCreation();
                mainImage.Source = new BitmapImage(new Uri(imgList[counter]));
                counter++;
                ButtonPrevious.IsEnabled = true;
                LabelPrint();
            }
            else
            {
                Environment.Exit(0);
            }
        }

        private void ButtonPrevious_Click(object sender, RoutedEventArgs e)
        {
            if (counter > 1)
            {
                counter--;
                LabelPrint();
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
                ButtonPrevious.IsEnabled = false;
                mainImage.Source = null;
            }
        }
    }
}
