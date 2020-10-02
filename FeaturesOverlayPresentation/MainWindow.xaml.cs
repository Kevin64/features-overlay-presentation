using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        private int counter = 0, maxCounter = 5;
        
        public MainWindow()
        {            
            InitializeComponent();
            this.KeyDown += OnPreviewKeyDown;
            TimerTickCreation();
            ButtonPrevious.IsEnabled = false;
            LabelPrint();
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
            LabelPage.Content = counter + 1 + " de " + maxCounter;
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
                if (counter == 4)
                    FinishPrint();
                else
                    NextPrint();
                ButtonNext.IsEnabled = true;
            }
        }

        private void ButtonNext_Click(object sender, RoutedEventArgs e)
        {
            if (counter == 0)
            {
                if (alreadyCount1 == false)
                {
                    TimerTickCreation();
                    alreadyCount1 = true;
                }
                Frame1.Content = new Page1();
                counter++;
                ButtonPrevious.IsEnabled = true;
                LabelPrint();
            }
            else if (counter == 1)
            {
                if (alreadyCount2 == false)
                {
                    TimerTickCreation();
                    alreadyCount2 = true;
                }
                Frame1.Content = new Page2();
                counter++;
                LabelPrint();
            }
            else if (counter == 2)
            {
                if (alreadyCount3 == false)
                {
                    TimerTickCreation();
                    alreadyCount3 = true;
                }
                Frame1.Content = new Page3();
                counter++;
                LabelPrint();
            }
            else if (counter == 3)
            {
                if (alreadyCount4 == false)
                {
                    TimerTickCreation();
                    alreadyCount4 = true;
                }
                Frame1.Content = new Page4();
                counter++;
                FinishPrint();
                LabelPrint();
            }
            else if(counter == 4)
            {
                Environment.Exit(0);
            }
        }

        private void ButtonPrevious_Click(object sender, RoutedEventArgs e)
        {
            if (counter == 1)
            {
                Frame1.Content = new Page0();
                counter--;
                ButtonPrevious.IsEnabled = false;
                LabelPrint();
            }
            else if (counter == 2)
            {
                Frame1.Content = new Page1();
                counter--;
                LabelPrint();
            }
            else if (counter == 3)
            {
                Frame1.Content = new Page2();
                counter--;
                LabelPrint();
            }
            else if (counter == 4)
            {
                Frame1.Content = new Page3();
                counter--;
                LabelPrint();
                if (!nextBlock.Text.Equals("Próximo"))
                    NextPrint();
            }
        }
    }
}
