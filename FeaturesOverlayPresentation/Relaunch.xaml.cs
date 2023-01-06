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
        private bool isFormat;
        MainWindow m;
        LogGenerator log;

        public Relaunch()
        {
            IniData def = null;
            var parser = new FileIniDataParser();
            try
            {
                InitializeComponent();
                //Parses the INI file
                def = parser.ReadFile(StringsAndConstants.defFile);

                var logLocationStr = def[StringsAndConstants.INI_SECTION_1][StringsAndConstants.INI_SECTION_1_9];

                bool logFileExists = bool.Parse(MiscMethods.checkIfLogExists(logLocationStr));
#if DEBUG
                //Create a new log file (or append to a existing one)
                log = new LogGenerator(Application.Current.MainWindow.GetType().Assembly.GetName().Name + " - v" + Application.Current.MainWindow.GetType().Assembly.GetName().Version + "-" + Properties.Resources.dev_status, logLocationStr, StringsAndConstants.LOG_FILENAME_FOP + "-v" + Application.Current.MainWindow.GetType().Assembly.GetName().Version + "-" + Properties.Resources.dev_status + StringsAndConstants.LOG_FILE_EXT, StringsAndConstants.consoleOutCLI);
                log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_DEBUG_MODE, string.Empty, StringsAndConstants.consoleOutCLI);

                serverDropDown.SelectedIndex = 1;
                portDropDown.SelectedIndex = 0;
#else
                //Create a new log file (or append to a existing one)
                log = new LogGenerator(Application.Current.MainWindow.GetType().Assembly.GetName().Name + " - v" + Application.Current.MainWindow.GetType().Assembly.GetName().Version, logLocationStr, StringsAndConstants.LOG_FILENAME_FOP + "-v" + Application.Current.MainWindow.GetType().Assembly.GetName().Version + StringsAndConstants.LOG_FILE_EXT, StringsAndConstants.consoleOutCLI);
                log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_RELEASE_MODE, string.Empty, StringsAndConstants.consoleOutCLI);

                serverDropDown.SelectedIndex = 0;
                portDropDown.SelectedIndex = 0;
#endif
                //Checks if log file exists
                if (!logFileExists)
                    log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOGFILE_NOTEXISTS, string.Empty, StringsAndConstants.consoleOutCLI);
                else
                    log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOGFILE_EXISTS, string.Empty, StringsAndConstants.consoleOutCLI);

                log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_DEFFILE_FOUND, Directory.GetCurrentDirectory() + "\\" + StringsAndConstants.defFile, StringsAndConstants.consoleOutCLI);
                
                try
                {
                    if (!FindFolder() || !checkAppFiles())
                        throw new Exception();
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
                    ReinstallError r = new ReinstallError(); //Prompts the user to reinstall the program
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

        //Checks if critical app assembly and DLLs are present
        public bool checkAppFiles()
        {
            List<string> fileList1 = new List<string>();
            List<string> fileList2 = new List<string>();
            string getCurrentDir = Directory.GetCurrentDirectory();
            fileList1.AddRange(Directory.GetFiles(getCurrentDir));
            foreach (string file in fileList1)
                fileList2.Add(Path.GetFileName(file));
            if (!StringsAndConstants.fopFileList.All(x => fileList2.Any(y => y == x)))
            {
                log.LogWrite(StringsAndConstants.LOG_ERROR, StringsAndConstants.LOG_APPFILE_NOT_FOUND, string.Empty, StringsAndConstants.consoleOutGUI);
                return false;
            }
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_APPFILE_FOUND, string.Empty, StringsAndConstants.consoleOutGUI);
            return true;
        }

        //Returns true if slide folder that contains PNG pictures exist
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

        //If 'yes' button is pressed
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
        
        //If 'no' button is pressed
        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_CLOSING, string.Empty, StringsAndConstants.consoleOutGUI);
            File.Delete(StringsAndConstants.loginPath);
            Application.Current.Shutdown();
        }

        //If 'send' button is pressed
        private void YesLaterButton_Click(object sender, RoutedEventArgs e)
        {
            DateTime dateAndTime = DateTime.Today;
            bool check = false;
            if (patrimTextBox.Text != "") //If patrimony textbox is not empty
            {
                if (LoginFileReader.checkHostST(serverDropDown.Text, portDropDown.Text)) //If login succeeded
                {
                    if (pressed == false) //If 'send' button is not pressed already
                    {
                        log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_PATR_NUM, patrimTextBox.Text, StringsAndConstants.consoleOutGUI);
                        if (present == false) //If employee is not present
                        {
                            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_EMPLOYEEAWAY, string.Empty, StringsAndConstants.consoleOutGUI);
                            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_REGISTERING_DELIVERY, string.Empty, StringsAndConstants.consoleOutGUI);
                            webBrowser1.Navigate("http://" + serverDropDown.Text + ":" + portDropDown.Text
                        + "/recebeDadosEntrega.php?patrimonio=" + patrimTextBox.Text + "&dataEntrega=" + dateAndTime.ToShortDateString() + "&siapeRecebedor=" + "Ausente" + "&entregador=" + userTextBox.Text);
                            check = true;
                        }
                        else if (SIAPETextBox.Text != "") //If employee is present and SIAPE textbox is not empty
                        {
                            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_EMPLOYEEPRESENT, string.Empty, StringsAndConstants.consoleOutGUI);
                            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_REGISTERING_DELIVERY, string.Empty, StringsAndConstants.consoleOutGUI);
                            webBrowser1.Navigate("http://" + serverDropDown.Text + ":" + portDropDown.Text
                        + "/recebeDadosEntrega.php?patrimonio=" + patrimTextBox.Text + "&dataEntrega=" + dateAndTime.ToShortDateString() + "&siapeRecebedor=" + SIAPETextBox.Text + "&entregador=" + userTextBox.Text);
                            check = true;
                        }
                        else //If employee is present and SIAPE textbox is empty
                        {
                            log.LogWrite(StringsAndConstants.LOG_ERROR, StringsAndConstants.LOG_FILLFORM, string.Empty, StringsAndConstants.consoleOutGUI);
                            MessageBox.Show(StringsAndConstants.fillForm, StringsAndConstants.ERROR_WINDOWTITLE, MessageBoxButton.OK, MessageBoxImage.Error);
                            check = false;
                        }
                        YesButton.IsEnabled = false;
                    }
                    else //If 'send' button is already pressed
                    {
                        log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_NOTSCHEDULING, string.Empty, StringsAndConstants.consoleOutGUI);
                        webBrowser1.Navigate("http://" + serverDropDown.Text + ":" + portDropDown.Text
                    + "/recebeDadosEntrega.php?patrimonio=" + patrimTextBox.Text + "&dataEntrega=" + null + "&siapeRecebedor=" + null + "&entregador=" + null);
                        check = true;
                        YesButton.IsEnabled = true;
                    }
                }
                else //If login fails
                {
                    log.LogWrite(StringsAndConstants.LOG_ERROR, StringsAndConstants.LOG_SERVER_NOT_FOUND, string.Empty, StringsAndConstants.consoleOutGUI);
                    MessageBox.Show(StringsAndConstants.SERVER_NOT_FOUND_ERROR, StringsAndConstants.ERROR_WINDOWTITLE, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else //If patrimony textbox is empty
            {
                log.LogWrite(StringsAndConstants.LOG_ERROR, StringsAndConstants.LOG_FILLFORM, string.Empty, StringsAndConstants.consoleOutGUI);
                MessageBox.Show(StringsAndConstants.fillForm, StringsAndConstants.ERROR_WINDOWTITLE, MessageBoxButton.OK, MessageBoxImage.Error);
            }

            if(check == true) //If data is already sent to the server
            {
                if(pressed == false) //If 'send' button is not pressed
                {
                    if (resPass == true) //If screen resolution passes the requirement
                    {
                        log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_RESOLUTION_PASSED, string.Empty, StringsAndConstants.consoleOutGUI);
                        if (isFormat == true) //If service type is 'format'
                        {
                            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_SERVICE_TYPE, StringsAndConstants.LOG_FORMAT_SERVICE, StringsAndConstants.consoleOutGUI);
                            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_SCHEDULING, string.Empty, StringsAndConstants.consoleOutGUI);
                            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_ADDING_REG, string.Empty, StringsAndConstants.consoleOutGUI);
                            RegistryKey key = Registry.CurrentUser.CreateSubKey(StringsAndConstants.FopRunOnceKey);
                            if (Environment.Is64BitOperatingSystem)
                                key.SetValue(StringsAndConstants.FOP, StringsAndConstants.FOPx86);
                            else
                                key.SetValue(StringsAndConstants.FOP, StringsAndConstants.FOPx64);
                            RegistryKey key2 = Registry.CurrentUser.CreateSubKey(StringsAndConstants.FopRegKey);
                            key2.SetValue(StringsAndConstants.DidItRunAlready, 0, RegistryValueKind.DWord);
                        }
                        else //If service type is 'maintenance'
                        {
                            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_SERVICE_TYPE, StringsAndConstants.LOG_MAINTENANCE_SERVICE, StringsAndConstants.consoleOutGUI);
                            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_NOT_ADDING_REG, string.Empty, StringsAndConstants.consoleOutGUI);
                        }
                        YesLaterButton.Content = StringsAndConstants.cancelExecution;
                    }
                    else  //If screen resolution fails the requirement
                    {
                        log.LogWrite(StringsAndConstants.LOG_ERROR, StringsAndConstants.LOG_RESOLUTION_FAILED, string.Empty, StringsAndConstants.consoleOutGUI);
                        YesLaterButton.Content = StringsAndConstants.cancelExecutionResError;
                    }
                    
                    pressed = true;
                    patrimTextBox.IsEnabled = false;
                    EmployeePresentRadioNo.IsEnabled = false;
                    EmployeePresentRadioYes.IsEnabled = false;
                    FormatRadioButton.IsEnabled = false;
                    MaintenanceRadioButton.IsEnabled = false;
                    SIAPETextBox.IsEnabled = false;
                }
                else //If 'send' button is pressed
                {
                    if (resPass == true) //If screen resolution passes the requirement
                    {
                        log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_RESOLUTION_PASSED, string.Empty, StringsAndConstants.consoleOutGUI);
                        log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_REMOVING_REG, string.Empty, StringsAndConstants.consoleOutGUI);
                        RegistryKey key = Registry.CurrentUser.CreateSubKey(StringsAndConstants.FopRunOnceKey);
                        if(isFormat)
                            key.DeleteValue(StringsAndConstants.FOP);
                        RegistryKey key2 = Registry.CurrentUser.CreateSubKey(StringsAndConstants.FopRegKey);
                        key2.SetValue(StringsAndConstants.DidItRunAlready, 1, RegistryValueKind.DWord);
                        YesLaterButton.Content = StringsAndConstants.doExecution;
                    }
                    else //If screen resolution fails the requirement
                    {
                        log.LogWrite(StringsAndConstants.LOG_ERROR, StringsAndConstants.LOG_RESOLUTION_FAILED, string.Empty, StringsAndConstants.consoleOutGUI);
                        YesLaterButton.Content = StringsAndConstants.doExecutionResError;
                    }
                    pressed = false;
                    patrimTextBox.IsEnabled = true;
                    EmployeePresentRadioNo.IsEnabled = true;
                    EmployeePresentRadioYes.IsEnabled = true;
                    FormatRadioButton.IsEnabled = true;
                    MaintenanceRadioButton.IsEnabled = true;
                    SIAPETextBox.IsEnabled = true;
                }
            }
        }

        //When closing the window
        private void Window_Closing(object sender, EventArgs e)
        {
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_CLOSING, string.Empty, StringsAndConstants.consoleOutGUI);
            File.Delete(StringsAndConstants.loginPath);
            Application.Current.Shutdown();
        }

        //When clicking the authenticate button
        private void AuthButton_Click(object sender, RoutedEventArgs e)
        {
            string[] str = { };
            if (userTextBox.Text == "" || passwordBox.Password == "") //If user and password textboxes are empty
                MessageBox.Show(StringsAndConstants.NO_AUTH, StringsAndConstants.ERROR_WINDOWTITLE, MessageBoxButton.OK, MessageBoxImage.Error);
            else //... if are not empty
            {
                log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_INIT_LOGIN, string.Empty, StringsAndConstants.consoleOutGUI);
                str = LoginFileReader.fetchInfoST(userTextBox.Text, passwordBox.Password, serverDropDown.Text, portDropDown.Text);

                if (str == null) //If server is not found
                {
                    log.LogWrite(StringsAndConstants.LOG_ERROR, StringsAndConstants.LOG_SERVER_NOT_FOUND, string.Empty, StringsAndConstants.consoleOutGUI);
                    MessageBox.Show(StringsAndConstants.SERVER_NOT_FOUND_ERROR, StringsAndConstants.ERROR_WINDOWTITLE, MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else if (str[0] == "false") //If server is found but login fails
                {
                    log.LogWrite(StringsAndConstants.LOG_ERROR, StringsAndConstants.LOG_LOGIN_FAILED, string.Empty, StringsAndConstants.consoleOutGUI);
                    warningLabel.Visibility = Visibility.Visible;
                    passwordBox.SelectAll();
                    passwordBox.Focus();
                }
                else //If server is found and login succeeds
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
                    FormatRadioButton.IsEnabled = true;
                    MaintenanceRadioButton.IsEnabled = true;
                    AuthButton.IsEnabled = false;
                }
            }
        }

        //Limits textboxes to disallow cut, copy and paste operations
        private void textBox_PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == ApplicationCommands.Copy || e.Command == ApplicationCommands.Cut || e.Command == ApplicationCommands.Paste)
                e.Handled = true;
        }

        //Limits some textboxes to allow only numerical inputs
        private void textBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        //If 'employee radio button' changes...
        private void EmployeePresentRadioYes_Checked(object sender, RoutedEventArgs e)
        {
            SIAPETextBox.IsEnabled = true;
            present = true;
            if (FormatRadioButton.IsChecked == true || MaintenanceRadioButton.IsChecked == true) //... and 'format/maintenance radio button changes
                YesLaterButton.IsEnabled = true;
        }

        //If 'employee radio button' changes...
        private void EmployeePresentRadioNo_Checked(object sender, RoutedEventArgs e)
        {
            SIAPETextBox.IsEnabled = false;
            present = false;
            if (FormatRadioButton.IsChecked == true || MaintenanceRadioButton.IsChecked == true) //... and 'format/maintenance radio button changes
                YesLaterButton.IsEnabled = true;
        }

        //If 'format/maintenance radio button changes...
        private void FormatRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            isFormat = true;
            if (EmployeePresentRadioNo.IsChecked == true || EmployeePresentRadioYes.IsChecked == true) //... and 'employee radio button' changes
                YesLaterButton.IsEnabled = true;
        }

        //If 'format/maintenance radio button changes...
        private void MaintenanceRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            isFormat = false;
            if (EmployeePresentRadioNo.IsChecked == true || EmployeePresentRadioYes.IsChecked == true) //... and 'employee radio button' changes
                YesLaterButton.IsEnabled = true;
        }
    }
}