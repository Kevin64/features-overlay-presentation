using ConstantsDLL;
using FeaturesOverlayPresentation.Properties;
using IniParser;
using IniParser.Exceptions;
using IniParser.Model;
using JsonFileReaderDLL;
using LogGeneratorDLL;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace FeaturesOverlayPresentation
{
    public partial class Relaunch : Window
    {
        private bool pressed = false;
        private bool present, isFormat;
        private readonly bool resPass = true;
        private readonly LogGenerator log;
        private static List<string[]> parametersListSection;
        private static string[] logLocationSection, serverIPListSection, serverPortListSection, logo1URLSection, logo2URLSection, logo3URLSection, agentData = new string[2];
        private static string logLocationStr, serverIPStr, serverPortStr, logo1URLStr, logo2URLStr, logo3URLStr;
        private MainWindow m;

        public Relaunch()
        {
            //Code for testing string localization for other languages
            //Thread.CurrentThread.CurrentCulture = new CultureInfo("en");
            //Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");
            //LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(System.Windows.Markup.XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));

            try
            {
                InitializeComponent();
                IniData def = null;
                FileIniDataParser parser = new FileIniDataParser();
                //Parses the INI file
                def = parser.ReadFile(ConstantsDLL.Properties.Resources.defFile, Encoding.UTF8);

                logLocationStr = def[ConstantsDLL.Properties.Resources.INI_SECTION_1][ConstantsDLL.Properties.Resources.INI_SECTION_1_9];
                serverIPStr = def[ConstantsDLL.Properties.Resources.INI_SECTION_1][ConstantsDLL.Properties.Resources.INI_SECTION_1_11];
                serverPortStr = def[ConstantsDLL.Properties.Resources.INI_SECTION_1][ConstantsDLL.Properties.Resources.INI_SECTION_1_12];
                logo1URLStr = def[ConstantsDLL.Properties.Resources.INI_SECTION_1][ConstantsDLL.Properties.Resources.INI_SECTION_1_16];
                logo2URLStr = def[ConstantsDLL.Properties.Resources.INI_SECTION_1][ConstantsDLL.Properties.Resources.INI_SECTION_1_17];
                logo3URLStr = def[ConstantsDLL.Properties.Resources.INI_SECTION_1][ConstantsDLL.Properties.Resources.INI_SECTION_1_18];

                logLocationSection = logLocationStr.Split().ToArray();
                serverIPListSection = serverIPStr.Split(',').ToArray();
                serverPortListSection = serverPortStr.Split(',').ToArray();
                logo1URLSection = logo1URLStr.Split().ToArray();
                logo2URLSection = logo2URLStr.Split().ToArray();
                logo3URLSection = logo3URLStr.Split().ToArray();

                parametersListSection = new List<string[]>
                {
                    logLocationSection,
                    logo1URLSection,
                    logo2URLSection,
                    logo3URLSection,
                    serverIPListSection,
                    serverPortListSection
                };

                comboBoxServerIP.ItemsSource = serverIPListSection;
                comboBoxServerPort.ItemsSource = serverPortListSection;

                bool logFileExists = bool.Parse(MiscMethods.CheckIfLogExists(logLocationStr));
#if DEBUG
                //Create a new log file (or append to an existing one)
                log = new LogGenerator(Application.Current.MainWindow.GetType().Assembly.GetName().Name + " - v" + Application.Current.MainWindow.GetType().Assembly.GetName().Version + "-" + Properties.Resources.dev_status, logLocationStr, ConstantsDLL.Properties.Resources.LOG_FILENAME_FOP + "-v" + Application.Current.MainWindow.GetType().Assembly.GetName().Version + "-" + Properties.Resources.dev_status + ConstantsDLL.Properties.Resources.LOG_FILE_EXT, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
                log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), ConstantsDLL.Properties.Strings.LOG_DEBUG_MODE, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));

                comboBoxServerIP.SelectedIndex = 1;
                comboBoxServerPort.SelectedIndex = 0;
#else
                //Create a new log file (or append to a existing one)
                log = new LogGenerator(Application.Current.MainWindow.GetType().Assembly.GetName().Name + " - v" + Application.Current.MainWindow.GetType().Assembly.GetName().Version, logLocationStr, ConstantsDLL.Properties.Resources.LOG_FILENAME_FOP + "-v" + Application.Current.MainWindow.GetType().Assembly.GetName().Version + ConstantsDLL.Properties.Resources.LOG_FILE_EXT, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
                log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), ConstantsDLL.Properties.Strings.LOG_RELEASE_MODE, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));

                comboBoxServerIP.SelectedIndex = 0;
                comboBoxServerPort.SelectedIndex = 0;
#endif
                //Checks if log file exists
                if (!logFileExists)
                {
                    log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), ConstantsDLL.Properties.Strings.LOGFILE_NOTEXISTS, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
                }
                else
                {
                    log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), ConstantsDLL.Properties.Strings.LOGFILE_EXISTS, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
                }

                log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), ConstantsDLL.Properties.Strings.LOG_DEFFILE_FOUND, Directory.GetCurrentDirectory() + "\\" + ConstantsDLL.Properties.Resources.defFile, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));

                try
                {
                    if (!FindFolder())
                    {
                        throw new Exception();
                    }

                    if (!MiscMethods.RegCheck())
                    {
                        resPass = MiscMethods.ResolutionError(true);
                        m = new MainWindow(parametersListSection);
                        m.Show();
                        Hide();
                        ShowInTaskbar = false;
                    }
                    else
                    {
                        resPass = MiscMethods.ResolutionError(false);
                        if (!resPass)
                        {
                            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_ERROR), Strings.LOG_RESOLUTION_ERROR, SystemParameters.PrimaryScreenWidth.ToString() + 'x' + SystemParameters.PrimaryScreenHeight.ToString(), Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
                            YesButton.IsEnabled = false;
                        }
                        Show();
                        ShowInTaskbar = true;
                    }
                }
                catch
                {
                    Hide();
                    ReinstallError r = new ReinstallError(); //Prompts the user to reinstall the program
                    r.Show();
                }
            }
            catch (ParsingException e) //If definition file was not found
            {
                Console.WriteLine(ConstantsDLL.Properties.Strings.LOG_DEFFILE_NOT_FOUND + ": " + e.Message);
                Console.WriteLine(ConstantsDLL.Properties.Strings.KEY_FINISH);
                Console.ReadLine();
                Environment.Exit(Convert.ToInt32(ConstantsDLL.Properties.Resources.RETURN_ERROR));
            }
            catch (FormatException e) //If definition file was malformed, but the logfile is not created (log path is undefined)
            {
                Console.WriteLine(ConstantsDLL.Properties.Strings.PARAMETER_ERROR + ": " + e.Message);
                Console.WriteLine(ConstantsDLL.Properties.Strings.KEY_FINISH);
                Console.ReadLine();
                Environment.Exit(Convert.ToInt32(ConstantsDLL.Properties.Resources.RETURN_ERROR));
            }
        }

        //Returns true if slide folder that contains PNG pictures exist
        public bool FindFolder()
        {
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_DETECTING_OS, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
            string imgDir = Directory.GetCurrentDirectory() + ConstantsDLL.Properties.Resources.resourcesDir + ConstantsDLL.Properties.Resources.imgDir;
            try
            {
                log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_ENUM_FILES, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
                List<string> filePathList = Directory.GetFiles(imgDir).ToList();
                log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_IMG_FOUND, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
                return true;
            }
            catch
            {
                log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_ERROR), Strings.LOG_IMG_NOTFOUND, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
                return false;
            }
        }

        //If 'yes' button is pressed
        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_RUNNING, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
                m = new MainWindow(parametersListSection);
                m.Show();
                Hide();
            }
            catch
            {
                log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_CLOSING, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
                Close();
                m.Close();
            }
        }

        //If 'no' button is pressed
        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_CLOSING, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
            File.Delete(StringsAndConstants.credentialsFilePath);
            Application.Current.Shutdown();
        }

        //If 'send' button is pressed
        private void YesLaterButton_Click(object sender, RoutedEventArgs e)
        {
            string[] assetJsonStr;
            DateTime dateAndTime = DateTime.Today;
            bool check = false;
            if (textBoxAssetNumber.Text != string.Empty) //If patrimony textbox is not empty
            {
                if (CredentialsFileReader.CheckHostST(comboBoxServerIP.Text, comboBoxServerPort.Text)) //If login succeeded
                {
                    if (!pressed) //If 'send' button is not pressed already
                    {
                        assetJsonStr = AssetFileReader.FetchInfoST(textBoxAssetNumber.Text, comboBoxServerIP.Text, comboBoxServerPort.Text);
                        if (assetJsonStr[0] != "false")
                        {
                            if (assetJsonStr[9] == "1")
                            {
                                log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_ERROR), ConstantsDLL.Properties.Strings.PC_DROPPED, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
                                _ = MessageBox.Show(ConstantsDLL.Properties.Strings.PC_DROPPED, ConstantsDLL.Properties.Strings.ERROR_WINDOWTITLE, MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                            else
                            {
                                log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_PATR_NUM, textBoxAssetNumber.Text, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
                                if (present == false) //If employee is not present
                                {
                                    log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_EMPLOYEEAWAY, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
                                    log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_REGISTERING_DELIVERY, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
                                    webBrowser1.Navigate(ConstantsDLL.Properties.Resources.HTTP + comboBoxServerIP.Text + ":" + comboBoxServerPort.Text + "/" + ConstantsDLL.Properties.Resources.deliveryURL + ".php" + ConstantsDLL.Properties.Resources.phpAssetNumber + textBoxAssetNumber.Text + ConstantsDLL.Properties.Resources.phpLastDeliveryDate + dateAndTime.ToShortDateString() + ConstantsDLL.Properties.Resources.phpDeliveredToRegistrationNumber + ConstantsDLL.Properties.Strings.absent + ConstantsDLL.Properties.Resources.phpLastDeliveryMadeBy + agentData[0]);
                                    check = true;
                                }
                                else if (textBoxRegistrationNumber.Text != string.Empty) //If employee is present and SIAPE textbox is not empty
                                {
                                    log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_EMPLOYEEPRESENT, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
                                    log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_REGISTERING_DELIVERY, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
                                    webBrowser1.Navigate(ConstantsDLL.Properties.Resources.HTTP + comboBoxServerIP.Text + ":" + comboBoxServerPort.Text
                                + "/" + ConstantsDLL.Properties.Resources.deliveryURL + ".php" + ConstantsDLL.Properties.Resources.phpAssetNumber + textBoxAssetNumber.Text + ConstantsDLL.Properties.Resources.phpLastDeliveryDate + dateAndTime.ToShortDateString() + ConstantsDLL.Properties.Resources.phpDeliveredToRegistrationNumber + textBoxRegistrationNumber.Text + ConstantsDLL.Properties.Resources.phpLastDeliveryMadeBy + agentData[0]);
                                    check = true;
                                }
                                else //If employee is present and SIAPE textbox is empty
                                {
                                    log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_ERROR), ConstantsDLL.Properties.Strings.fillForm, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
                                    _ = MessageBox.Show(ConstantsDLL.Properties.Strings.fillForm, ConstantsDLL.Properties.Strings.ERROR_WINDOWTITLE, MessageBoxButton.OK, MessageBoxImage.Error);
                                    check = false;
                                }
                                YesButton.IsEnabled = false;
                            }
                        }
                        else
                        {
                            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_ERROR), ConstantsDLL.Properties.Strings.pcNotRegistered, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
                            _ = MessageBox.Show(ConstantsDLL.Properties.Strings.pcNotRegistered, ConstantsDLL.Properties.Strings.ERROR_WINDOWTITLE, MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else //If 'send' button is already pressed
                    {
                        log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_NOTSCHEDULING, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
                        webBrowser1.Navigate(ConstantsDLL.Properties.Resources.HTTP + comboBoxServerIP.Text + ":" + comboBoxServerPort.Text
                    + "/" + ConstantsDLL.Properties.Resources.deliveryURL + ".php" + ConstantsDLL.Properties.Resources.phpAssetNumber + textBoxAssetNumber.Text + ConstantsDLL.Properties.Resources.phpLastDeliveryDate + null + ConstantsDLL.Properties.Resources.phpDeliveredToRegistrationNumber + null + ConstantsDLL.Properties.Resources.phpLastDeliveryMadeBy + null);
                        check = true;
                        if (resPass)
                        {
                            YesButton.IsEnabled = true;
                        }
                    }
                }
                else //If login fails
                {
                    log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_ERROR), ConstantsDLL.Properties.Strings.SERVER_NOT_FOUND_ERROR, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
                    _ = MessageBox.Show(ConstantsDLL.Properties.Strings.SERVER_NOT_FOUND_ERROR, ConstantsDLL.Properties.Strings.ERROR_WINDOWTITLE, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else //If patrimony textbox is empty
            {
                log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_ERROR), ConstantsDLL.Properties.Strings.fillForm, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
                _ = MessageBox.Show(ConstantsDLL.Properties.Strings.fillForm, ConstantsDLL.Properties.Strings.ERROR_WINDOWTITLE, MessageBoxButton.OK, MessageBoxImage.Error);
            }

            //Do registry stuff, and handles control states
            if (check) //If data is already sent to the server
            {
                if (!pressed) //If 'send' button is not pressed
                {
                    if (resPass) //If screen resolution passes the requirement
                    {
                        log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_RESOLUTION_PASSED, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
                        if (isFormat == true) //If service type is 'format'
                        {
                            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_SERVICE_TYPE, Strings.LOG_FORMAT_SERVICE, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
                            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_SCHEDULING, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
                            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_ADDING_REG, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
                            MiscMethods.RegCreate();
                        }
                        else //If service type is 'maintenance'
                        {
                            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_SERVICE_TYPE, Strings.LOG_MAINTENANCE_SERVICE, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
                            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_NOT_ADDING_REG, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
                        }
                        YesLaterButtonTB.Text = ConstantsDLL.Properties.Strings.cancelExecution;
                    }
                    else  //If screen resolution fails the requirement
                    {
                        log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_WARNING), Strings.LOG_RESOLUTION_FAILED, Strings.LOG_DISABLE_BOOT, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
                        YesLaterButtonTB.Text = ConstantsDLL.Properties.Strings.cancelExecutionResError;
                    }

                    pressed = true;
                    textBoxAssetNumber.IsEnabled = false;
                    radioButtonEmployeePresentNo.IsEnabled = false;
                    radioButtonEmployeePresentYes.IsEnabled = false;
                    radioButtonFormatting.IsEnabled = false;
                    radioButtonMaintenance.IsEnabled = false;
                    textBoxRegistrationNumber.IsEnabled = false;
                }
                else //If 'send' button is pressed
                {
                    if (resPass) //If screen resolution passes the requirement
                    {
                        log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_RESOLUTION_PASSED, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
                        log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_REMOVING_REG, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
                        RegistryKey key = Registry.CurrentUser.CreateSubKey(ConstantsDLL.Properties.Resources.FopRunOnceKey);
                        if (isFormat)
                        {
                            key.DeleteValue(ConstantsDLL.Properties.Resources.FOP);
                        }

                        RegistryKey key2 = Registry.CurrentUser.CreateSubKey(ConstantsDLL.Properties.Resources.FopRegKey);
                        key2.SetValue(ConstantsDLL.Properties.Resources.DidItRunAlready, 1, RegistryValueKind.DWord);
                        YesLaterButtonTB.Text = ConstantsDLL.Properties.Strings.doExecution;
                    }
                    else //If screen resolution fails the requirement
                    {
                        YesLaterButtonTB.Text = ConstantsDLL.Properties.Strings.doExecutionResError;
                    }

                    pressed = false;
                    textBoxAssetNumber.IsEnabled = true;
                    radioButtonEmployeePresentNo.IsEnabled = true;
                    radioButtonEmployeePresentYes.IsEnabled = true;
                    radioButtonFormatting.IsEnabled = true;
                    radioButtonMaintenance.IsEnabled = true;
                    if (radioButtonEmployeePresentYes.IsChecked == true)
                    {
                        textBoxRegistrationNumber.IsEnabled = true;
                    }
                }
            }
        }

        //When closing the window
        private void Window_Closing(object sender, EventArgs e)
        {
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_CLOSING, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
            File.Delete(StringsAndConstants.credentialsFilePath);
            Application.Current.Shutdown();
        }

        //When clicking the authenticate button
        private void AuthButton_Click(object sender, RoutedEventArgs e)
        {
            if (textBoxUsername.Text == string.Empty || textBoxPassword.Password == string.Empty) //If user and password textboxes are empty
            {
                _ = MessageBox.Show(ConstantsDLL.Properties.Strings.NO_AUTH, ConstantsDLL.Properties.Strings.ERROR_WINDOWTITLE, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else //... if are not empty
            {
                log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), ConstantsDLL.Properties.Strings.LOG_INIT_LOGIN, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
                agentData = CredentialsFileReader.FetchInfoST(textBoxUsername.Text, textBoxPassword.Password, comboBoxServerIP.Text, comboBoxServerPort.Text);

                if (agentData == null) //If server is not found
                {
                    log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_ERROR), ConstantsDLL.Properties.Strings.SERVER_NOT_FOUND_ERROR, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
                    _ = MessageBox.Show(ConstantsDLL.Properties.Strings.SERVER_NOT_FOUND_ERROR, ConstantsDLL.Properties.Strings.ERROR_WINDOWTITLE, MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else if (agentData[0] == "false") //If server is found but login fails
                {
                    log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_ERROR), ConstantsDLL.Properties.Strings.LOG_LOGIN_FAILED, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
                    warningLabel.Visibility = Visibility.Visible;
                    textBoxPassword.SelectAll();
                    _ = textBoxPassword.Focus();
                }
                else //If server is found and login succeeds
                {
                    log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), ConstantsDLL.Properties.Strings.LOG_LOGIN_SUCCESS, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
                    lblFixedAssetNumber.IsEnabled = true;
                    textBoxAssetNumber.IsEnabled = true;
                    lblFixedServerIP.IsEnabled = false;
                    comboBoxServerIP.IsEnabled = false;
                    lblFixedServerPort.IsEnabled = false;
                    comboBoxServerPort.IsEnabled = false;
                    warningLabel.Visibility = Visibility.Hidden;
                    lblFixedUsername.IsEnabled = false;
                    textBoxUsername.IsEnabled = false;
                    lblFixedPassword.IsEnabled = false;
                    textBoxPassword.IsEnabled = false;
                    lblFixedEmployeePresent.IsEnabled = true;
                    radioButtonEmployeePresentYes.IsEnabled = true;
                    radioButtonEmployeePresentNo.IsEnabled = true;
                    radioButtonFormatting.IsEnabled = true;
                    radioButtonMaintenance.IsEnabled = true;
                    AuthButton.IsEnabled = false;
                }
            }
        }

        //Limits textboxes to disallow cut, copy and paste operations
        private void TextBox_PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == ApplicationCommands.Copy || e.Command == ApplicationCommands.Cut || e.Command == ApplicationCommands.Paste)
            {
                e.Handled = true;
            }
        }

        //Limits some textboxes to allow only numerical inputs
        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        //If 'employee radio button' changes...
        private void RadioButtonEmployeePresentYes_Checked(object sender, RoutedEventArgs e)
        {
            textBoxRegistrationNumber.IsEnabled = true;
            present = true;
            if (radioButtonFormatting.IsChecked == true || radioButtonMaintenance.IsChecked == true) //... and 'format/maintenance radio button changes
            {
                YesLaterButton.IsEnabled = true;
            }
        }

        //If 'employee radio button' changes...
        private void RadioButtonEmployeePresentNo_Checked(object sender, RoutedEventArgs e)
        {
            textBoxRegistrationNumber.IsEnabled = false;
            present = false;
            if (radioButtonFormatting.IsChecked == true || radioButtonMaintenance.IsChecked == true) //... and 'format/maintenance radio button changes
            {
                YesLaterButton.IsEnabled = true;
            }
        }

        //If 'format/maintenance radio button changes...
        private void RadioButtonFormatting_Checked(object sender, RoutedEventArgs e)
        {
            isFormat = true;
            if (radioButtonEmployeePresentNo.IsChecked == true || radioButtonEmployeePresentYes.IsChecked == true) //... and 'employee radio button' changes
            {
                YesLaterButton.IsEnabled = true;
            }
        }

        //If 'format/maintenance radio button changes...
        private void RadioButtonMaintenance_Checked(object sender, RoutedEventArgs e)
        {
            isFormat = false;
            if (radioButtonEmployeePresentNo.IsChecked == true || radioButtonEmployeePresentYes.IsChecked == true) //... and 'employee radio button' changes
            {
                YesLaterButton.IsEnabled = true;
            }
        }
    }
}