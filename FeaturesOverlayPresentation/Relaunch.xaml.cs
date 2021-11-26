using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace FeaturesOverlayPresentation
{
    public partial class Relaunch : Window
    {
        private bool pressed = false;
        MainWindow m;

        public Relaunch()
        {
            InitializeComponent();
            try
            {                               
                if (!FindFolder())
                    throw new Exception();
                RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\FOP");
                if (int.Parse(key.GetValue("DidItRunAlready").ToString()).Equals(0))
                {
                    m = new MainWindow();
                    m.Show();                    
                }      
                else
                {
                    this.Show();
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
            string current = Directory.GetCurrentDirectory();
            string imgDir = current + "\\img\\";
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
                if (PingHost(serverDropDown.Text) == true && portDropDown.Text != "")
                {
                    if (pressed == false)
                        webBrowser1.Navigate("http://" + serverDropDown.Text + ":" + portDropDown.Text
                    + "/recebeDadosEntrega.php?patrimonio=" + patrimTextBox.Text + "&dataEntrega=" + dateAndTime.ToShortDateString());
                    else
                        webBrowser1.Navigate("http://" + serverDropDown.Text + ":" + portDropDown.Text
                    + "/recebeDadosEntrega.php?patrimonio=" + patrimTextBox.Text + "&dataEntrega=" + null);
                    check = true;
                }
                else
                    MessageBox.Show("Servidor não encontrado. Selecione um servidor válido!", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                MessageBox.Show("Preencha os campos necessários!", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            

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
                }
                else
                {
                    RegistryKey key = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\RunOnce");
                    key.DeleteValue("FOP");
                    RegistryKey key2 = Registry.CurrentUser.CreateSubKey(@"Software\FOP");
                    key2.SetValue("DidItRunAlready", 1, RegistryValueKind.DWord);
                    YesLaterButton.Content = "Executar no próximo boot";
                    pressed = false;
                }
                
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private static bool PingHost(string servidor_web)
        {
            bool pingable = false;
            Ping pinger = new Ping();
            if (servidor_web == "")
                return false;
            try
            {
                PingReply reply = pinger.Send(servidor_web);
                pingable = reply.Status == IPStatus.Success;
            }
            catch (PingException)
            {
            }
            return pingable;
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
                YesLaterButton.IsEnabled = true;
                warningLabel.Visibility = Visibility.Hidden;
                userLabel.IsEnabled = false;
                userTextBox.IsEnabled = false;
                passwordLabel.IsEnabled = false;
                passwordBox.IsEnabled = false;
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
    }
}
