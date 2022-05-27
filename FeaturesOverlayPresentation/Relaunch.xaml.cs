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
            MiscMethods.resolutionError();
            InitializeComponent();
            try
            {
                if (!FindFolder())
                    throw new Exception();
                if (!MiscMethods.regCheck())
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
            string imgDir = MiscMethods.OSCheck();
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
                this.Hide();
            }
            catch
            {
                this.Close();
                m.Close();
            }
        }

        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            File.Delete(StringsAndConstants.fileLogin);
            Application.Current.Shutdown();
        }

        private void YesLaterButton_Click(object sender, RoutedEventArgs e)
        {
            DateTime dateAndTime = DateTime.Today;
            bool check = false;
            if(patrimTextBox.Text != "")
            {
                if (LoginFileReader.checkHost(serverDropDown.Text, portDropDown.Text))
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
                            MessageBox.Show(StringsAndConstants.fillForm, StringsAndConstants.ERROR_WINDOWTITLE, MessageBoxButton.OK, MessageBoxImage.Error);
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
                    MessageBox.Show(StringsAndConstants.serverNotFound, StringsAndConstants.ERROR_WINDOWTITLE, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
                MessageBox.Show(StringsAndConstants.fillForm, StringsAndConstants.ERROR_WINDOWTITLE, MessageBoxButton.OK, MessageBoxImage.Error);         

            if(check == true)
            {                
                if(pressed == false)
                {
                    RegistryKey key = Registry.CurrentUser.CreateSubKey(StringsAndConstants.FopRunOnceKey);
                    if (Environment.Is64BitOperatingSystem)
                        key.SetValue(StringsAndConstants.FOP, StringsAndConstants.FOPx86);
                    else
                        key.SetValue(StringsAndConstants.FOP, StringsAndConstants.FOPx64);
                    RegistryKey key2 = Registry.CurrentUser.CreateSubKey(StringsAndConstants.FopRegKey);
                    key2.SetValue(StringsAndConstants.DidItRunAlready, 0, RegistryValueKind.DWord);
                    YesLaterButton.Content = StringsAndConstants.cancelExecution;
                    pressed = true;
                    patrimTextBox.IsEnabled = false;
                    EmployeePresentRadioNo.IsEnabled = false;
                    EmployeePresentRadioYes.IsEnabled = false;
                    SIAPETextBox.IsEnabled = false;
                }
                else
                {
                    RegistryKey key = Registry.CurrentUser.CreateSubKey(StringsAndConstants.FopRunOnceKey);
                    key.DeleteValue(StringsAndConstants.FOP);
                    RegistryKey key2 = Registry.CurrentUser.CreateSubKey(StringsAndConstants.FopRegKey);
                    key2.SetValue(StringsAndConstants.DidItRunAlready, 1, RegistryValueKind.DWord);
                    YesLaterButton.Content = StringsAndConstants.doExecution;
                    pressed = false;
                    patrimTextBox.IsEnabled = true;
                    EmployeePresentRadioNo.IsEnabled = true;
                    EmployeePresentRadioYes.IsEnabled = true;
                    SIAPETextBox.IsEnabled = true;
                }
            }
        }

        private void Window_Closing(object sender, EventArgs e)
        {
            File.Delete(StringsAndConstants.fileLogin);
            Application.Current.Shutdown();
        }

        private void AuthButton_Click(object sender, RoutedEventArgs e)
        {
            warningLabel.Visibility = Visibility.Hidden;
            string[] str = { };
            if (userTextBox.Text == "" || passwordBox.Password == "")
                MessageBox.Show(StringsAndConstants.NO_AUTH, StringsAndConstants.ERROR_WINDOWTITLE, MessageBoxButton.OK, MessageBoxImage.Error);
            else
            {
                str = LoginFileReader.fetchInfo(userTextBox.Text, passwordBox.Password, serverDropDown.Text, portDropDown.Text);

                if (str == null)
                    MessageBox.Show(StringsAndConstants.SERVER_NOT_FOUND_ERROR, StringsAndConstants.ERROR_WINDOWTITLE, MessageBoxButton.OK, MessageBoxImage.Error);
                else if (str[0] == "false")
                {
                    warningLabel.Visibility = Visibility.Visible;
                    passwordBox.SelectAll();
                    passwordBox.Focus();
                }
                else
                {
                    patrimLabel.IsEnabled = true;
                    patrimTextBox.IsEnabled = true;
                    serverLabel.IsEnabled = false;
                    serverDropDown.IsEnabled = false;
                    portLabel.IsEnabled = false;
                    portDropDown.IsEnabled = false;
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
