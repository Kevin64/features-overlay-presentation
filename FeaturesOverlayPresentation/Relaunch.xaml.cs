using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace FeaturesOverlayPresentation
{
    public partial class Relaunch : Window
    {
        private bool pressed = false;
        private bool present;
        MainWindow m;

        public Relaunch()
        {
            Utils.resolutionError();
            InitializeComponent();            
            try
            {                               
                if (!FindFolder())
                    throw new Exception();
                if (!Utils.regCheck())
                {
                    m = new MainWindow();
                    m.Show();
                    this.ShowInTaskbar = false;
                }
                else
                {
                    this.Show();
                    this.ShowInTaskbar = true;
                }
            }
            catch
            {
                this.Hide();
                ReinstallError r = new ReinstallError();
                r.Show();                
            }
            
        }

        public bool FindFolder()
        {
            string imgDir = Utils.OSCheck();            
            try
            {
                List<string> filePathList = Directory.GetFiles(imgDir).ToList();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                m = new MainWindow();
                m.Show();
                this.Close();
            }
            catch
            {
                this.Close();
                m.Close();
            }
            
        }

        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void YesLaterButton_Click(object sender, RoutedEventArgs e)
        {
            DateTime dateAndTime = DateTime.Today;
            bool check = false;
            if(patrimTextBox.Text != "")
            {
                if (Utils.PingHost(serverDropDown.Text) == true && portDropDown.Text != "")
                {
                    if (pressed == false)
                    {
                        if (present == false)
                        {
                            webBrowser1.Navigate("http://" + serverDropDown.Text + ":" + portDropDown.Text
                        + "/recebeDadosEntrega.php?patrimonio=" + patrimTextBox.Text + "&dataEntrega=" + dateAndTime.ToShortDateString() + "&siapeRecebedor=" + "Ausente");
                            check = true;
                        }
                        else if (SIAPETextBox.Text != "")
                        {
                            webBrowser1.Navigate("http://" + serverDropDown.Text + ":" + portDropDown.Text
                        + "/recebeDadosEntrega.php?patrimonio=" + patrimTextBox.Text + "&dataEntrega=" + dateAndTime.ToShortDateString() + "&siapeRecebedor=" + SIAPETextBox.Text);
                            check = true;
                        }
                        else
                        {
                            MessageBox.Show("Preencha os campos necessários!", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                            check = false;
                        }
                    }
                    else
                    {
                        webBrowser1.Navigate("http://" + serverDropDown.Text + ":" + portDropDown.Text
                    + "/recebeDadosEntrega.php?patrimonio=" + patrimTextBox.Text + "&dataEntrega=" + null + "&siapeRecebedor=" + null);
                        check = true;
                    }                    
                }
                else
                    MessageBox.Show("Servidor não encontrado. Selecione um servidor válido!", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
                MessageBox.Show("Preencha os campos necessários!", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);         

            if(check == true)
            {                
                if(pressed == false)
                {
                    RegistryKey key = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\RunOnce");
                    if (Environment.Is64BitOperatingSystem)
                        key.SetValue("FOP", "C:\\Program Files (x86)\\FOP\\Rever tutorial de uso do computador.lnk");
                    else
                        key.SetValue("FOP", "C:\\Program Files\\FOP\\Rever tutorial de uso do computador.lnk");
                    RegistryKey key2 = Registry.CurrentUser.CreateSubKey(@"Software\FOP");
                    key2.SetValue("DidItRunAlready", 0, RegistryValueKind.DWord);
                    YesLaterButton.Content = "Cancelar execução no próximo boot";
                    pressed = true;
                    patrimTextBox.IsEnabled = false;
                    serverDropDown.IsEnabled = false;
                    portDropDown.IsEnabled = false;
                    EmployeePresentRadioNo.IsEnabled = false;
                    EmployeePresentRadioYes.IsEnabled = false;
                    SIAPETextBox.IsEnabled = false;
                }
                else
                {
                    RegistryKey key = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\RunOnce");
                    key.DeleteValue("FOP");
                    RegistryKey key2 = Registry.CurrentUser.CreateSubKey(@"Software\FOP");
                    key2.SetValue("DidItRunAlready", 1, RegistryValueKind.DWord);
                    YesLaterButton.Content = "Executar no próximo boot";
                    pressed = false;
                    patrimTextBox.IsEnabled = true;
                    serverDropDown.IsEnabled = true;
                    portDropDown.IsEnabled = true;
                    EmployeePresentRadioNo.IsEnabled = true;
                    EmployeePresentRadioYes.IsEnabled = true;
                    SIAPETextBox.IsEnabled = true;
                }
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void AuthButton_Click(object sender, RoutedEventArgs e)
        {
            if (userTextBox.Text == "lab74c" && passwordBox.Password == "admccshlab74cadm")
            {
                patrimLabel.IsEnabled = true;
                patrimTextBox.IsEnabled = true;
                serverLabel.IsEnabled = true;
                serverDropDown.IsEnabled = true;
                portLabel.IsEnabled = true;
                portDropDown.IsEnabled = true;
                warningLabel.Visibility = Visibility.Hidden;
                userLabel.IsEnabled = false;
                userTextBox.IsEnabled = false;
                passwordLabel.IsEnabled = false;
                passwordBox.IsEnabled = false;
                EmployeePresentLabel.IsEnabled = true;
                EmployeePresentRadioYes.IsEnabled = true;
                EmployeePresentRadioNo.IsEnabled = true;
                AuthButton.IsEnabled = false;
            }
            else
            {
                warningLabel.Visibility = Visibility.Visible;
                passwordBox.SelectAll();
                passwordBox.Focus();
            }
        }

        private void textBox_PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == ApplicationCommands.Copy || e.Command == ApplicationCommands.Cut || e.Command == ApplicationCommands.Paste)
                e.Handled = true;
        }

        private void textBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void EmployeePresentRadioYes_Checked(object sender, RoutedEventArgs e)
        {
            SIAPELabel.IsEnabled = true;
            SIAPETextBox.IsEnabled = true;
            present = true;
            YesLaterButton.IsEnabled = true;
        }

        private void EmployeePresentRadioNo_Checked(object sender, RoutedEventArgs e)
        {
            SIAPELabel.IsEnabled = false;
            SIAPETextBox.IsEnabled = false;
            present = false;
            YesLaterButton.IsEnabled = true;
        }
    }
}
