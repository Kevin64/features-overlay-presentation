using ConstantsDLL;
using ConstantsDLL.Properties;
using LogGeneratorDLL;
using Microsoft.Win32;
using Newtonsoft.Json;
using RestApiDLL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace FeaturesOverlayPresentation.XAML
{
    /// <summary> 
    /// Class for Relaunch.xaml
    /// </summary>
    public partial class Relaunch : Window
    {
        private bool pressed = false;
        private bool present, isFormat;
        private readonly bool resPass = true;
        private static string jsonFile;
        private MainWindow m;

        private static Agent agent;
        private static Asset existingAsset, newAsset;
        private static location newLocation;
        private static Definitions definitions;
        private static HttpClient client;
        private static LogGenerator log;
        private static StreamReader fileC;
        private static ServerParam serverParam;

        public class ConfigurationOptions
        {
            public Definitions Definitions { get; set; }
        }

        public class Definitions
        {
            public string LogLocation { get; set; }
            public List<string> ServerIP { get; set; }
            public List<string> ServerPort { get; set; }
            public string Logo1URL { get; set; }
            public string Logo2URL { get; set; }
            public string Logo3URL { get; set; }
        }

        /// <summary> 
        /// Relaunch constructor
        /// </summary>
        public Relaunch()
        {
            //Code for testing string localization for other languages
            //System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en");
            //System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en");
            //LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(System.Windows.Markup.XmlLanguage.GetLanguage(System.Globalization.CultureInfo.CurrentCulture.IetfLanguageTag)));

            try
            {
                InitializeComponent();

                newLocation = new location();
                newAsset = new Asset()
                {
                    location = newLocation
                };

                fileC = new StreamReader(GenericResources.CONFIG_FILE);
                jsonFile = fileC.ReadToEnd();
                ConfigurationOptions jsonParse = JsonConvert.DeserializeObject<ConfigurationOptions>(@jsonFile);
                fileC.Close();

                //Creates 'Definitions' JSON section object
                definitions = new Definitions()
                {
                    LogLocation = jsonParse.Definitions.LogLocation,
                    ServerIP = jsonParse.Definitions.ServerIP,
                    ServerPort = jsonParse.Definitions.ServerPort,
                    Logo1URL = jsonParse.Definitions.Logo1URL,
                    Logo2URL = jsonParse.Definitions.Logo2URL,
                    Logo3URL = jsonParse.Definitions.Logo3URL
                };

                comboBoxServerIP.ItemsSource = definitions.ServerIP;
                comboBoxServerPort.ItemsSource = definitions.ServerPort;

                bool logFileExists = bool.Parse(Misc.MiscMethods.CheckIfLogExists(definitions.LogLocation));
#if DEBUG
                //Create a new log file (or append to an existing one)
                log = new LogGenerator(System.Windows.Application.Current.MainWindow.GetType().Assembly.GetName().Name + " - v" + System.Windows.Application.Current.MainWindow.GetType().Assembly.GetName().Version + "-" + GenericResources.DEV_STATUS_BETA, definitions.LogLocation, ConstantsDLL.Properties.GenericResources.LOG_FILENAME_FOP + "-v" + System.Windows.Application.Current.MainWindow.GetType().Assembly.GetName().Version + "-" + GenericResources.DEV_STATUS_BETA + ConstantsDLL.Properties.GenericResources.LOG_FILE_EXT, Convert.ToBoolean(ConstantsDLL.Properties.GenericResources.CONSOLE_OUT_CLI));
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), ConstantsDLL.Properties.LogStrings.LOG_DEBUG_MODE, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.GenericResources.CONSOLE_OUT_CLI));

                comboBoxServerIP.SelectedIndex = 1;
                comboBoxServerPort.SelectedIndex = 0;
#else
                //Create a new log file (or append to a existing one)
                log = new LogGenerator(Application.Current.MainWindow.GetType().Assembly.GetName().Name + " - v" + Application.Current.MainWindow.GetType().Assembly.GetName().Version, definitions.LogLocation, ConstantsDLL.Properties.GenericResources.LOG_FILENAME_FOP + "-v" + Application.Current.MainWindow.GetType().Assembly.GetName().Version + ConstantsDLL.Properties.GenericResources.LOG_FILE_EXT, Convert.ToBoolean(ConstantsDLL.Properties.GenericResources.CONSOLE_OUT_CLI));
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), ConstantsDLL.Properties.LogStrings.LOG_RELEASE_MODE, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.GenericResources.CONSOLE_OUT_CLI));

                comboBoxServerIP.SelectedIndex = 0;
                comboBoxServerPort.SelectedIndex = 0;
#endif
                //Checks if log file exists
                if (!logFileExists)
                {
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), ConstantsDLL.Properties.UIStrings.LOGFILE_NOTEXISTS, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.GenericResources.CONSOLE_OUT_CLI));
                }
                else
                {
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), ConstantsDLL.Properties.UIStrings.LOGFILE_EXISTS, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.GenericResources.CONSOLE_OUT_CLI));
                }

                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), ConstantsDLL.Properties.LogStrings.LOG_PARAMETER_FILE_FOUND, Directory.GetCurrentDirectory() + "\\" + ConstantsDLL.Properties.GenericResources.CONFIG_FILE, Convert.ToBoolean(ConstantsDLL.Properties.GenericResources.CONSOLE_OUT_CLI));

                try
                {
                    if (!FindFolder())
                    {
                        throw new Exception();
                    }

                    if (!Misc.MiscMethods.RegCheck())
                    {
                        resPass = Misc.MiscMethods.ResolutionError(true);
                        m = new MainWindow(log, definitions);
                        m.Show();
                        Hide();
                        ShowInTaskbar = false;
                    }
                    else
                    {
                        resPass = Misc.MiscMethods.ResolutionError(false);
                        if (!resPass)
                        {
                            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_ERROR), LogStrings.LOG_RESOLUTION_ERROR, SystemParameters.PrimaryScreenWidth.ToString() + 'x' + SystemParameters.PrimaryScreenHeight.ToString(), Convert.ToBoolean(ConstantsDLL.Properties.GenericResources.CONSOLE_OUT_GUI));
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
            //If config file is malformed
            catch (Exception e) when (e is JsonReaderException || e is JsonSerializationException || e is FormatException)
            {
                Console.WriteLine(UIStrings.PARAMETER_ERROR + ": " + e.Message);
                Console.WriteLine(UIStrings.KEY_FINISH);
                Console.ReadLine();
                Environment.Exit(Convert.ToInt32(ExitCodes.ERROR));
            }
            //If config file is not found
            catch (FileNotFoundException e)
            {
                Console.WriteLine(LogStrings.LOG_PARAMETER_FILE_NOT_FOUND + ": " + e.Message);
                Console.WriteLine(UIStrings.KEY_FINISH);
                Console.ReadLine();
                Environment.Exit(Convert.ToInt32(ExitCodes.ERROR));
            }
        }

        /// <summary>
        /// Searches the image files
        /// </summary>
        /// <returns>Returns true if slide folder that contains PNG pictures exist</returns>
        public bool FindFolder()
        {
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_DETECTING_OS, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.GenericResources.CONSOLE_OUT_GUI));
            string imgDir = Directory.GetCurrentDirectory() + ConstantsDLL.Properties.GenericResources.FOP_RESOURCES_DIR + ConstantsDLL.Properties.GenericResources.FOP_IMG_DIR;
            try
            {
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_ENUM_FILES, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.GenericResources.CONSOLE_OUT_GUI));
                List<string> filePathList = Directory.GetFiles(imgDir).ToList();
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_IMG_FOUND, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.GenericResources.CONSOLE_OUT_GUI));
                return true;
            }
            catch
            {
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_ERROR), LogStrings.LOG_IMG_NOTFOUND, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.GenericResources.CONSOLE_OUT_GUI));
                return false;
            }
        }

        /// <summary>
        /// If 'yes' button is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_RUNNING, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.GenericResources.CONSOLE_OUT_GUI));
                m = new MainWindow(log, definitions);
                m.Show();
                Hide();
            }
            catch
            {
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_CLOSING, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.GenericResources.CONSOLE_OUT_GUI));
                Close();
                m.Close();
            }
        }

        /// <summary> 
        /// If 'no' button is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_CLOSING, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.GenericResources.CONSOLE_OUT_GUI));
            System.Windows.Application.Current.Shutdown();
        }

        /// <summary>
        /// If 'send' button is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void YesLaterButton_Click(object sender, RoutedEventArgs e)
        {
            DateTime dateAndTime = DateTime.Today;
            bool check = false;
            if (textBoxAssetNumber.Text != string.Empty) //If asset number textbox is not empty
            {
                if (!pressed) //If 'send' button is not pressed already
                {
                    try
                    {
                        existingAsset = await AssetHandler.GetAssetAsync(client, GenericResources.HTTP + comboBoxServerIP.Text + ":" + comboBoxServerPort.Text + GenericResources.APCS_V1_API_ASSET_NUMBER_URL + textBoxAssetNumber.Text);

                        if (existingAsset.discarded == "1")
                        {
                            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_ERROR), ConstantsDLL.Properties.UIStrings.ASSET_DROPPED, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.GenericResources.CONSOLE_OUT_GUI));
                            _ = System.Windows.MessageBox.Show(ConstantsDLL.Properties.UIStrings.ASSET_DROPPED, ConstantsDLL.Properties.UIStrings.ERROR_WINDOWTITLE, MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        else
                        {
                            newAsset.assetNumber = textBoxAssetNumber.Text;
                            newAsset.location.locLastDeliveryDate = dateAndTime.ToString(ConstantsDLL.Properties.GenericResources.DATE_FORMAT).Substring(0, 10);
                            newAsset.location.locLastDeliveryMadeBy = agent.id;

                            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_PATR_NUM, textBoxAssetNumber.Text, Convert.ToBoolean(ConstantsDLL.Properties.GenericResources.CONSOLE_OUT_GUI));
                            if (present == false) //If employee is not present
                            {
                                newAsset.location.locDeliveredToRegistrationNumber = UIStrings.ABSENT;
                                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_EMPLOYEEAWAY, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.GenericResources.CONSOLE_OUT_GUI));
                                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_REGISTERING_DELIVERY, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.GenericResources.CONSOLE_OUT_GUI));
                                System.Net.HttpStatusCode v = await AssetHandler.SetAssetAsync(client, GenericResources.HTTP + comboBoxServerIP.Text + ":" + comboBoxServerPort.Text + GenericResources.APCS_V1_API_DELIVERY_URL, newAsset);
                                check = true;
                            }
                            else if (textBoxRegistrationNumber.Text != string.Empty) //If employee is present and registration number textbox is not empty
                            {
                                newAsset.location.locDeliveredToRegistrationNumber = textBoxRegistrationNumber.Text;
                                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_EMPLOYEEPRESENT, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.GenericResources.CONSOLE_OUT_GUI));
                                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_REGISTERING_DELIVERY, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.GenericResources.CONSOLE_OUT_GUI));
                                System.Net.HttpStatusCode v = await AssetHandler.SetAssetAsync(client, GenericResources.HTTP + comboBoxServerIP.Text + ":" + comboBoxServerPort.Text + GenericResources.APCS_V1_API_DELIVERY_URL, newAsset);
                                check = true;
                            }
                            else //If employee is present and registration number textbox is empty
                            {
                                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_ERROR), ConstantsDLL.Properties.UIStrings.FILL_FORM, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.GenericResources.CONSOLE_OUT_GUI));
                                _ = System.Windows.MessageBox.Show(ConstantsDLL.Properties.UIStrings.FILL_FORM, ConstantsDLL.Properties.UIStrings.ERROR_WINDOWTITLE, MessageBoxButton.OK, MessageBoxImage.Error);
                                check = false;
                            }
                            YesButton.IsEnabled = false;
                        }
                    }
                    //If asset does not exist on the database
                    catch (UnregisteredAssetException ex)
                    {
                        log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_ERROR), ex.Message, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                        _ = System.Windows.MessageBox.Show(ConstantsDLL.Properties.UIStrings.ASSET_NOT_REGISTERED, ConstantsDLL.Properties.UIStrings.ERROR_WINDOWTITLE, MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    catch (InvalidRestApiCallException ex)
                    {
                        //Shows a message about an error in the APCS web service
                        log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_ERROR), ex.Message, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                        _ = System.Windows.MessageBox.Show(UIStrings.SERVER_ERROR, UIStrings.ERROR_WINDOWTITLE, MessageBoxButton.OK, MessageBoxImage.Error);
                        Environment.Exit(Convert.ToInt32(ExitCodes.ERROR));
                    }
                    catch (InvalidAgentException ex)
                    {
                        //Shows a message about an error of the agent credentials
                        log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_ERROR), ex.Message, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                        _ = System.Windows.MessageBox.Show(UIStrings.INVALID_CREDENTIALS, UIStrings.ERROR_WINDOWTITLE, MessageBoxButton.OK, MessageBoxImage.Error);
                        Environment.Exit(Convert.ToInt32(ExitCodes.ERROR));
                    }
                    //If server is unreachable
                    catch (HttpRequestException)
                    {
                        log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_ERROR), UIStrings.DATABASE_REACH_ERROR, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                        _ = System.Windows.MessageBox.Show(UIStrings.DATABASE_REACH_ERROR, UIStrings.ERROR_WINDOWTITLE, MessageBoxButton.OK, MessageBoxImage.Error);
                    }

                }
                else //If 'send' button is already pressed
                {
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_NOTSCHEDULING, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.GenericResources.CONSOLE_OUT_GUI));

                    newLocation.locDeliveredToRegistrationNumber = null;
                    newLocation.locLastDeliveryDate = null;
                    newLocation.locLastDeliveryMadeBy = null;
                    _ = await AssetHandler.SetAssetAsync(client, GenericResources.HTTP + comboBoxServerIP.Text + ":" + comboBoxServerPort.Text + GenericResources.APCS_V1_API_DELIVERY_URL, newAsset);
                    check = true;
                    if (resPass)
                        YesButton.IsEnabled = true;
                }
            }
            else //If asset number textbox is empty
            {
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_ERROR), ConstantsDLL.Properties.UIStrings.FILL_FORM, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.GenericResources.CONSOLE_OUT_GUI));
                _ = System.Windows.MessageBox.Show(ConstantsDLL.Properties.UIStrings.FILL_FORM, ConstantsDLL.Properties.UIStrings.ERROR_WINDOWTITLE, MessageBoxButton.OK, MessageBoxImage.Error);
            }

            //Do registry stuff, and handles control states
            if (check) //If data is already sent to the server
            {
                if (!pressed) //If 'send' button is not pressed
                {
                    if (resPass) //If screen resolution passes the requirement
                    {
                        log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_RESOLUTION_PASSED, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.GenericResources.CONSOLE_OUT_GUI));
                        if (isFormat == true) //If service type is 'format'
                        {
                            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_SERVICE_TYPE, LogStrings.LOG_FORMAT_SERVICE, Convert.ToBoolean(ConstantsDLL.Properties.GenericResources.CONSOLE_OUT_GUI));
                            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_SCHEDULING, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.GenericResources.CONSOLE_OUT_GUI));
                            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_ADDING_REG, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.GenericResources.CONSOLE_OUT_GUI));
                            Misc.MiscMethods.RegCreate();
                        }
                        else //If service type is 'maintenance'
                        {
                            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_SERVICE_TYPE, LogStrings.LOG_MAINTENANCE_SERVICE, Convert.ToBoolean(ConstantsDLL.Properties.GenericResources.CONSOLE_OUT_GUI));
                            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_NOT_ADDING_REG, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.GenericResources.CONSOLE_OUT_GUI));
                        }
                        YesLaterButtonTB.Text = ConstantsDLL.Properties.UIStrings.CANCEL_EXECUTION;
                    }
                    else  //If screen resolution fails the requirement
                    {
                        log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_WARNING), LogStrings.LOG_RESOLUTION_FAILED, LogStrings.LOG_DISABLE_BOOT, Convert.ToBoolean(ConstantsDLL.Properties.GenericResources.CONSOLE_OUT_GUI));
                        YesLaterButtonTB.Text = ConstantsDLL.Properties.UIStrings.CANCEL_EXECUTION_RES_ERROR;
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
                        log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_RESOLUTION_PASSED, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.GenericResources.CONSOLE_OUT_GUI));
                        log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_REMOVING_REG, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.GenericResources.CONSOLE_OUT_GUI));
                        RegistryKey key = Registry.CurrentUser.CreateSubKey(ConstantsDLL.Properties.GenericResources.REGISTRY_FOP_RUN_ONCE_KEY);
                        if (isFormat)
                        {
                            try
                            {
                                key.DeleteValue(ConstantsDLL.Properties.GenericResources.FOP_NAME);
                            }
                            catch
                            {
                            }
                        }

                        RegistryKey key2 = Registry.CurrentUser.CreateSubKey(ConstantsDLL.Properties.GenericResources.REGISTRY_FOP_REG_KEY);
                        key2.SetValue(ConstantsDLL.Properties.GenericResources.REGISTRY_DID_IT_RUN_ALREADY, 1, RegistryValueKind.DWord);
                        YesLaterButtonTB.Text = ConstantsDLL.Properties.UIStrings.DO_EXECUTION;
                    }
                    else //If screen resolution fails the requirement
                    {
                        YesLaterButtonTB.Text = ConstantsDLL.Properties.UIStrings.DO_EXECUTION_RES_ERROR;
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

        /// <summary> 
        /// When closing the window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, EventArgs e)
        {
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_CLOSING, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.GenericResources.CONSOLE_OUT_GUI));
            System.Windows.Application.Current.Shutdown();
        }

        /// <summary> 
        /// When clicking the authenticate button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void AuthButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (textBoxUsername.Text == string.Empty || textBoxPassword.Password == string.Empty) //If user and password textboxes are empty
                {
                    _ = System.Windows.MessageBox.Show(ConstantsDLL.Properties.UIStrings.FILL_IN_YOUR_CREDENTIALS, ConstantsDLL.Properties.UIStrings.ERROR_WINDOWTITLE, MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else //... if are not empty
                {
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), ConstantsDLL.Properties.LogStrings.LOG_AUTH_USER, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.GenericResources.CONSOLE_OUT_GUI));

                    client = RestApiDLL.MiscMethods.SetHttpClient(comboBoxServerIP.Text, comboBoxServerPort.Text, GenericResources.HTTP_CONTENT_TYPE_JSON, textBoxUsername.Text, textBoxPassword.Password);

                    serverParam = await ParameterHandler.GetParameterAsync(client, GenericResources.HTTP + comboBoxServerIP.Text + ":" + comboBoxServerPort.Text + GenericResources.APCS_V1_API_PARAMETERS_URL);

                    agent = await AuthenticationHandler.GetAgentAsync(client, GenericResources.HTTP + comboBoxServerIP.Text + ":" + comboBoxServerPort.Text + GenericResources.APCS_V1_API_AGENT_USERNAME_URL + textBoxUsername.Text);

                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), ConstantsDLL.Properties.LogStrings.LOG_LOGIN_SUCCESS, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.GenericResources.CONSOLE_OUT_GUI));
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
                    textBoxAssetNumber.MaxLength = serverParam.Parameters.AssetNumberDigitLimit;
                    lblFixedAssetNumber.IsEnabled = true;
                    lblFixedServiceType.IsEnabled = true;
                    lblFixedEmployeePresent.IsEnabled = true;
                    radioButtonEmployeePresentYes.IsEnabled = true;
                    radioButtonEmployeePresentNo.IsEnabled = true;
                    radioButtonFormatting.IsEnabled = true;
                    radioButtonMaintenance.IsEnabled = true;
                    lblFixedRegistrationNumber.IsEnabled = true;
                    textBoxRegistrationNumber.MaxLength = serverParam.Parameters.DeliveryRegistrationNumberDigitLimit;
                    AuthButton.IsEnabled = false;
                }
            }
            //If URI is invalid
            catch (UriFormatException ex)
            {
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_ERROR), ex.Message, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                _ = System.Windows.MessageBox.Show(UIStrings.FILL_IN_SERVER_DETAILS, UIStrings.ERROR_WINDOWTITLE, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            //If Agent does not exist because there is no internet connection
            catch (HttpRequestException ex)
            {
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_ERROR), ex.Message, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                _ = System.Windows.MessageBox.Show(UIStrings.INTRANET_REQUIRED, UIStrings.ERROR_WINDOWTITLE, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            //If Agent does not exist, but the connection succeeded
            catch (InvalidAgentException ex)
            {
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_ERROR), ex.Message, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                warningLabel.Visibility = Visibility.Visible;
                _ = System.Windows.MessageBox.Show(UIStrings.INVALID_CREDENTIALS, UIStrings.ERROR_WINDOWTITLE, MessageBoxButton.OK, MessageBoxImage.Error);
                textBoxPassword.SelectAll();
                _ = textBoxPassword.Focus();
            }
            //If Rest call is invalid
            catch (InvalidRestApiCallException ex)
            {
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_ERROR), ex.Message, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                _ = System.Windows.MessageBox.Show(UIStrings.SERVER_ERROR, UIStrings.ERROR_WINDOWTITLE, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary> 
        /// Limits textboxes to disallow cut, copy and paste operations
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == ApplicationCommands.Copy || e.Command == ApplicationCommands.Cut || e.Command == ApplicationCommands.Paste)
            {
                e.Handled = true;
            }
        }

        /// <summary> 
        /// Limits some textboxes to allow only numerical inputs
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        /// <summary> 
        /// If 'employee' radio button changes...
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadioButtonEmployeePresentYes_Checked(object sender, RoutedEventArgs e)
        {
            textBoxRegistrationNumber.IsEnabled = true;
            present = true;
            if (radioButtonFormatting.IsChecked == true || radioButtonMaintenance.IsChecked == true) //... and 'format/maintenance radio button changes
            {
                YesLaterButton.IsEnabled = true;
            }
        }

        /// <summary> 
        /// If 'employee' radio button changes...
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadioButtonEmployeePresentNo_Checked(object sender, RoutedEventArgs e)
        {
            textBoxRegistrationNumber.IsEnabled = false;
            present = false;
            if (radioButtonFormatting.IsChecked == true || radioButtonMaintenance.IsChecked == true) //... and 'format/maintenance radio button changes
            {
                YesLaterButton.IsEnabled = true;
            }
        }

        /// <summary> 
        /// If 'format/maintenance' radio button changes...
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadioButtonFormatting_Checked(object sender, RoutedEventArgs e)
        {
            isFormat = true;
            if (radioButtonEmployeePresentNo.IsChecked == true || radioButtonEmployeePresentYes.IsChecked == true) //... and 'employee radio button' changes
            {
                YesLaterButton.IsEnabled = true;
            }
        }

        /// <summary> 
        /// If 'format/maintenance' radio button changes...
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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