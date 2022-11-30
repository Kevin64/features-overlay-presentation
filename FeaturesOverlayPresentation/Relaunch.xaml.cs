using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using ConstantsDLL;
using JsonFileReaderDLL;
using LogGeneratorDLL;
using IniParser;
using IniParser.Model;
using IniParser.Exceptions;

namespace FeaturesOverlayPresentation
{
    public partial class Relaunch : Window
    {
        private bool pressed = false;
        private bool present;
        private bool resPass = true;
        MainWindow m;
        LogGenerator log;

        public Relaunch()
        {
            IniData def = null;
            var parser = new FileIniDataParser();
            try
            {
                //Parses the INI file
                def = parser.ReadFile(StringsAndConstants.defFile);

                var logLocationStr = def[StringsAndConstants.INI_SECTION_1][StringsAndConstants.INI_SECTION_1_9];

                bool fileExists = bool.Parse(MiscMethods.checkIfLogExists(logLocationStr));
#if DEBUG
                //Create a new log file (or append to an existing one)
                log = new LogGenerator(Application.Current.MainWindow.GetType().Assembly.GetName().Name + " - v" + Application.Current.MainWindow.GetType().Assembly.GetName().Version + "-" + Properties.Resources.dev_status, logLocationStr, StringsAndConstants.LOG_FILENAME_FOP + "-v" + Application.Current.MainWindow.GetType().Assembly.GetName().Version + "-" + Properties.Resources.dev_status + StringsAndConstants.LOG_FILE_EXT, StringsAndConstants.consoleOutCLI);
                log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_DEBUG_MODE, string.Empty, StringsAndConstants.consoleOutCLI);
#else
                //Create a new log file (or append to a existing one)
                log = new LogGenerator(Application.Current.MainWindow.GetType().Assembly.GetName().Name + " - v" + Application.Current.MainWindow.GetType().Assembly.GetName().Version, logLocationStr, StringsAndConstants.LOG_FILENAME_FOP + "-v" + Application.Current.MainWindow.GetType().Assembly.GetName().Version + StringsAndConstants.LOG_FILE_EXT, StringsAndConstants.consoleOutCLI);
                log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_RELEASE_MODE, string.Empty, StringsAndConstants.consoleOutCLI);
#endif
                //Checks if log file exists
                if (!fileExists)
                    log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOGFILE_NOTEXISTS, string.Empty, StringsAndConstants.consoleOutCLI);
                else
                    log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOGFILE_EXISTS, string.Empty, StringsAndConstants.consoleOutCLI);

                log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_DEFFILE_FOUND, Directory.GetCurrentDirectory() + "\\" + StringsAndConstants.defFile, StringsAndConstants.consoleOutCLI);

                InitializeComponent();
                try
                {
                    if (!FindFolder())
                    {
                        
                        throw new Exception();
                    }
                    if (!MiscMethods.regCheck())
                    {
                        resPass = MiscMethods.resolutionError(true);
                        m = new MainWindow();
                        m.Show();
                        this.Hide();
                        this.ShowInTaskbar = false;
                    }
                    else
                    {
                        resPass = MiscMethods.resolutionError(false);
                        if (!resPass)
                            YesButton.IsEnabled = false;
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
            catch (ParsingException e) //If definition file was not found
            {
                Console.WriteLine(StringsAndConstants.LOG_DEFFILE_NOT_FOUND + ": " + e.Message);
                Console.WriteLine(StringsAndConstants.KEY_FINISH);
                Console.ReadLine();
                Environment.Exit(StringsAndConstants.RETURN_ERROR);
            }
            catch (FormatException e) //If definition file was malformed, but the logfile is not created (log path is undefined)
            {
                Console.WriteLine(StringsAndConstants.PARAMETER_ERROR + ": " + e.Message);
                Console.WriteLine(StringsAndConstants.KEY_FINISH);
                Console.ReadLine();
                Environment.Exit(StringsAndConstants.RETURN_ERROR);
            }
        }

        public bool FindFolder()
        {
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_DETECTING_OS, string.Empty, StringsAndConstants.consoleOutGUI);
            string imgDir = MiscMethods.OSCheck();
            try
            {
                log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_ENUM_FILES, string.Empty, StringsAndConstants.consoleOutGUI);
                List<string> filePathList = Directory.GetFiles(imgDir).ToList();
                log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_IMG_FOUND, string.Empty, StringsAndConstants.consoleOutGUI);
                return true;
            }
            catch
            {
                log.LogWrite(StringsAndConstants.LOG_ERROR, StringsAndConstants.LOG_IMG_NOTFOUND, string.Empty, StringsAndConstants.consoleOutGUI);
                return false;
            }
        }

        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_RUNNING, string.Empty, StringsAndConstants.consoleOutGUI);
                m = new MainWindow();
                m.Show();
                this.Hide();
            }
            catch
            {
                log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_CLOSING, string.Empty, StringsAndConstants.consoleOutGUI);
                this.Close();
                m.Close();
            }
        }

        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_CLOSING, string.Empty, StringsAndConstants.consoleOutGUI);
            File.Delete(StringsAndConstants.loginPath);
            Application.Current.Shutdown();
        }

        private void YesLaterButton_Click(object sender, RoutedEventArgs e)
        {
            DateTime dateAndTime = DateTime.Today;
            bool check = false;
            if (patrimTextBox.Text != "")
            {
                if (LoginFileReader.checkHostST(serverDropDown.Text, portDropDown.Text))
                {
                    if (pressed == false)
                    {
                        log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_SCHEDULING, string.Empty, StringsAndConstants.consoleOutGUI);
                        if (present == false)
                        {
                            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_EMPLOYEEAWAY, string.Empty, StringsAndConstants.consoleOutGUI);
                            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_REGISTERING_DELIVERY, string.Empty, StringsAndConstants.consoleOutGUI);
                            webBrowser1.Navigate("http://" + serverDropDown.Text + ":" + portDropDown.Text
                        + "/recebeDadosEntrega.php?patrimonio=" + patrimTextBox.Text + "&dataEntrega=" + dateAndTime.ToShortDateString() + "&siapeRecebedor=" + "Ausente" + "&entregador=" + userTextBox.Text);
                            check = true;
                        }
                        else if (SIAPETextBox.Text != "")
                        {
                            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_EMPLOYEEPRESENT, string.Empty, StringsAndConstants.consoleOutGUI);
                            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_REGISTERING_DELIVERY, string.Empty, StringsAndConstants.consoleOutGUI);
                            webBrowser1.Navigate("http://" + serverDropDown.Text + ":" + portDropDown.Text
                        + "/recebeDadosEntrega.php?patrimonio=" + patrimTextBox.Text + "&dataEntrega=" + dateAndTime.ToShortDateString() + "&siapeRecebedor=" + SIAPETextBox.Text + "&entregador=" + userTextBox.Text);
                            check = true;
                        }
                        else
                        {
                            log.LogWrite(StringsAndConstants.LOG_ERROR, StringsAndConstants.LOG_FILLFORM, string.Empty, StringsAndConstants.consoleOutGUI);
                            MessageBox.Show(StringsAndConstants.fillForm, StringsAndConstants.ERROR_WINDOWTITLE, MessageBoxButton.OK, MessageBoxImage.Error);
                            check = false;
                        }
                        YesButton.IsEnabled = false;
                    }
                    else
                    {
                        log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_NOTSCHEDULING, string.Empty, StringsAndConstants.consoleOutGUI);
                        webBrowser1.Navigate("http://" + serverDropDown.Text + ":" + portDropDown.Text
                    + "/recebeDadosEntrega.php?patrimonio=" + patrimTextBox.Text + "&dataEntrega=" + null + "&siapeRecebedor=" + null + "&entregador=" + null);
                        check = true;
                        if (resPass == true)
                            YesButton.IsEnabled = true;
                    }
                }
                else
                {
                    log.LogWrite(StringsAndConstants.LOG_ERROR, StringsAndConstants.LOG_SERVER_NOT_FOUND, string.Empty, StringsAndConstants.consoleOutGUI);
                    MessageBox.Show(StringsAndConstants.SERVER_NOT_FOUND_ERROR, StringsAndConstants.ERROR_WINDOWTITLE, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                log.LogWrite(StringsAndConstants.LOG_ERROR, StringsAndConstants.LOG_FILLFORM, string.Empty, StringsAndConstants.consoleOutGUI);
                MessageBox.Show(StringsAndConstants.fillForm, StringsAndConstants.ERROR_WINDOWTITLE, MessageBoxButton.OK, MessageBoxImage.Error);
            }

            if(check == true)
            {
                if(pressed == false)
                {
                    if (resPass == true)
                    {
                        log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_RESOLUTION_PASSED, string.Empty, StringsAndConstants.consoleOutGUI);
                        log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_ADDING_REG, string.Empty, StringsAndConstants.consoleOutGUI);
                        RegistryKey key = Registry.CurrentUser.CreateSubKey(StringsAndConstants.FopRunOnceKey);
                        if (Environment.Is64BitOperatingSystem)
                            key.SetValue(StringsAndConstants.FOP, StringsAndConstants.FOPx86);
                        else
                            key.SetValue(StringsAndConstants.FOP, StringsAndConstants.FOPx64);
                        RegistryKey key2 = Registry.CurrentUser.CreateSubKey(StringsAndConstants.FopRegKey);
                        key2.SetValue(StringsAndConstants.DidItRunAlready, 0, RegistryValueKind.DWord);
                        YesLaterButton.Content = StringsAndConstants.cancelExecution;
                    }
                    else
                    {
                        log.LogWrite(StringsAndConstants.LOG_ERROR, StringsAndConstants.LOG_RESOLUTION_FAILED, string.Empty, StringsAndConstants.consoleOutGUI);
                        YesLaterButton.Content = StringsAndConstants.cancelExecutionResError;
                    }
                    pressed = true;
                    patrimTextBox.IsEnabled = false;
                    EmployeePresentRadioNo.IsEnabled = false;
                    EmployeePresentRadioYes.IsEnabled = false;
                    SIAPETextBox.IsEnabled = false;
                }
                else
                {
                    if (resPass == true)
                    {
                        log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_RESOLUTION_PASSED, string.Empty, StringsAndConstants.consoleOutGUI);
                        log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_REMOVING_REG, string.Empty, StringsAndConstants.consoleOutGUI);
                        RegistryKey key = Registry.CurrentUser.CreateSubKey(StringsAndConstants.FopRunOnceKey);
                        key.DeleteValue(StringsAndConstants.FOP);
                        RegistryKey key2 = Registry.CurrentUser.CreateSubKey(StringsAndConstants.FopRegKey);
                        key2.SetValue(StringsAndConstants.DidItRunAlready, 1, RegistryValueKind.DWord);
                        YesLaterButton.Content = StringsAndConstants.doExecution;
                    }
                    else
                    {
                        log.LogWrite(StringsAndConstants.LOG_ERROR, StringsAndConstants.LOG_RESOLUTION_FAILED, string.Empty, StringsAndConstants.consoleOutGUI);
                        YesLaterButton.Content = StringsAndConstants.doExecutionResError;
                    }
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
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_CLOSING, string.Empty, StringsAndConstants.consoleOutGUI);
            File.Delete(StringsAndConstants.loginPath);
            Application.Current.Shutdown();
        }

        private void AuthButton_Click(object sender, RoutedEventArgs e)
        {
            string[] str = { };
            if (userTextBox.Text == "" || passwordBox.Password == "")
                MessageBox.Show(StringsAndConstants.NO_AUTH, StringsAndConstants.ERROR_WINDOWTITLE, MessageBoxButton.OK, MessageBoxImage.Error);
            else
            {
                log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_INIT_LOGIN, string.Empty, StringsAndConstants.consoleOutGUI);
                str = LoginFileReader.fetchInfoST(userTextBox.Text, passwordBox.Password, serverDropDown.Text, portDropDown.Text);

                if (str == null)
                {
                    log.LogWrite(StringsAndConstants.LOG_ERROR, StringsAndConstants.LOG_SERVER_NOT_FOUND, string.Empty, StringsAndConstants.consoleOutGUI);
                    MessageBox.Show(StringsAndConstants.SERVER_NOT_FOUND_ERROR, StringsAndConstants.ERROR_WINDOWTITLE, MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else if (str[0] == "false")
                {
                    log.LogWrite(StringsAndConstants.LOG_ERROR, StringsAndConstants.LOG_LOGIN_FAILED, string.Empty, StringsAndConstants.consoleOutGUI);
                    warningLabel.Visibility = Visibility.Visible;
                    passwordBox.SelectAll();
                    passwordBox.Focus();
                }
                else
                {
                    log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_LOGIN_SUCCESS, string.Empty, StringsAndConstants.consoleOutGUI);
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