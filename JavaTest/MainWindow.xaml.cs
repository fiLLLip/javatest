using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Windows.Threading;
using Microsoft.Win32;
using System.IO;
using System.Management;

namespace JavaTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer _dispatcherTimer = new DispatcherTimer();
        private Thread _thread;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnClearCache_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                RegistryKey rk = Registry.LocalMachine;
                RegistryKey subKey = rk.OpenSubKey("SOFTWARE\\JavaSoft\\Java Runtime Environment");

                string[] versionArray = subKey.GetValue("CurrentVersion").ToString().Split('.');

                int currentVersion = Int16.Parse(versionArray[1]);

                //MessageBox.Show(currentVersion.ToString());

                if (currentVersion < 7)
                {
                    MessageBox.Show("Vennligs vurder å oppgrader til en ny Java-versjon", "Advarsel", MessageBoxButton.OK, MessageBoxImage.Warning);
                }

                var procStartInfo = new System.Diagnostics.ProcessStartInfo("javaws", "-Xclearcache -uninstall");
                // The following commands are needed to redirect the standard output.
                // This means that it will be redirected to the Process.StandardOutput StreamReader.
                procStartInfo.RedirectStandardOutput = true;
                procStartInfo.UseShellExecute = false;
                procStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                // Do not create the black window.
                procStartInfo.CreateNoWindow = true;
                // Now we create a process, assign its ProcessStartInfo and start it
                var proc = new System.Diagnostics.Process();
                proc.StartInfo = procStartInfo;
                proc.Start();
                proc.WaitForExit();
                if (proc.ExitCode == -1)
                {
                    MessageBox.Show("Kunne ikke slette cache!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    MessageBox.Show("Cache er nå slettet!", "Suksess", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kunne ikke slette cache!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnDeleteCerts_Click(object sender, RoutedEventArgs e)
        {
            if (System.Environment.OSVersion.Version.Major == 6)
            {
                string basePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "Low\\Sun\\Java\\Deployment\\security\\";
                backupCerts(basePath);
            }
            else if (System.Environment.OSVersion.Version.Major == 5)
            {
                string basePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Sun\\Java\\Deployment\\security\\";
                backupCerts(basePath);
            }
            else
            {
                MessageBox.Show("Kan ikke slette sertifikater på dette operativsystemet.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnClearIETemp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.ProcessStartInfo tempFiles = new System.Diagnostics.ProcessStartInfo("rundll32.exe", "InetCpl.cpl,ClearMyTracksByProcess 8");
                // The following commands are needed to redirect the standard output.
                // This means that it will be redirected to the Process.StandardOutput StreamReader.
                tempFiles.RedirectStandardOutput = true;
                tempFiles.UseShellExecute = false;
                tempFiles.WindowStyle = ProcessWindowStyle.Hidden;
                // Do not create the black window.
                tempFiles.CreateNoWindow = true;
                // Now we create a process, assign its ProcessStartInfo and start it
                System.Diagnostics.Process proctemp = new System.Diagnostics.Process();
                proctemp.StartInfo = tempFiles;
                proctemp.Start();
                proctemp.WaitForExit();

                System.Diagnostics.ProcessStartInfo cache = new System.Diagnostics.ProcessStartInfo("rundll32.exe", "InetCpl.cpl,ClearMyTracksByProcess 2");
                // The following commands are needed to redirect the standard output.
                // This means that it will be redirected to the Process.StandardOutput StreamReader.
                cache.RedirectStandardOutput = true;
                cache.UseShellExecute = false;
                cache.WindowStyle = ProcessWindowStyle.Hidden;
                // Do not create the black window.
                cache.CreateNoWindow = true;
                // Now we create a process, assign its ProcessStartInfo and start it
                System.Diagnostics.Process proccache = new System.Diagnostics.Process();
                proccache.StartInfo = cache;
                proccache.Start();
                proccache.WaitForExit();

                MessageBox.Show("Midlertidige filer og cache er nå slettet!", "Suksess", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kunne ikke slette midlertidige filer eller cache!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void backupCerts(String basePath)
        {
            try
            {
                string oldPath = basePath + "trusted.certs";
                string newPath = basePath + "trusted.certs.bak";

                if (File.Exists(newPath) && File.Exists(oldPath)) {
                    File.Delete(newPath);
                }

                File.Move(oldPath, newPath);

                MessageBox.Show("Sertifikater er nå slettet og sikkerhetskopiert!", "Suksess", MessageBoxButton.OK, MessageBoxImage.Information);


            }
            catch (Exception ex)
            {
                MessageBox.Show("Kunne ikke slette sertifikater", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void restoreCerts(String basePath)
        {
            try
            {
                string newPath = basePath + "trusted.certs";
                string oldPath = basePath + "trusted.certs.bak";

                if (File.Exists(newPath) && File.Exists(oldPath))
                {
                    File.Delete(newPath);
                }

                File.Move(oldPath, newPath);

                MessageBox.Show("Sertifikater er nå gjenopprettet!", "Suksess", MessageBoxButton.OK, MessageBoxImage.Information);


            }
            catch (Exception ex)
            {
                MessageBox.Show("Kunne ikke gjenopprette sertifikater!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void btnRestoreCerts_Click(object sender, RoutedEventArgs e)
        {

            if (System.Environment.OSVersion.Version.Major == 6)
            {
                string basePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "Low\\Sun\\Java\\Deployment\\security\\";
                restoreCerts(basePath);
            }
            else if (System.Environment.OSVersion.Version.Major == 5)
            {
                string basePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Sun\\Java\\Deployment\\security\\";
                restoreCerts(basePath);
            }
            else
            {
                MessageBox.Show("Kunne ikke slette sertifikater på dette operativsystemer.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void findJavaSoftware()
        {
            try
            {
                var mos =
                  new ManagementObjectSearcher("SELECT * FROM Win32_Product WHERE Name Like '%Java%Update%'");
                foreach (ManagementObject mo in mos.Get())
                {
                    try
                    {
                        var ProgramName = mo["Name"].ToString();
                        if (MessageBox.Show("Vil du avinstallere " + ProgramName + "?", "Bekreft avinstallasjon", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        {
                            if (UninstallProgram(ProgramName))
                            {
                                MessageBox.Show("Avinstallerte " + ProgramName);
                            }
                            else
                            {
                                MessageBox.Show("Kunne ikke avinstallere " + ProgramName);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        //this program may not have a name property
                    }
                }

            }
            catch (Exception ex)
            {
            }
        }

        private void btnDeleteJava_Click(object sender, RoutedEventArgs e)
        {
            this.IsEnabled = false;
            lblPleaseWait.Visibility = Visibility.Visible;
            this.Title = "JavaTest.no - Vennligst vent, programmet jobber";
            
            _thread = new Thread(findJavaSoftware);
            _thread.Start();
            _dispatcherTimer.Tick += new EventHandler(DeleteJavaDispatcherTimerTick);
            _dispatcherTimer.Interval = new TimeSpan(100);
            _dispatcherTimer.Start();
        }

        private void DeleteJavaDispatcherTimerTick(object sender, EventArgs e)
        {
            if (!_thread.IsAlive)
            {
                this.IsEnabled = true;
                lblPleaseWait.Visibility = Visibility.Hidden;
                this.Title = "JavaTest.no";
                _dispatcherTimer.Tick -= new EventHandler(DeleteJavaDispatcherTimerTick);
                _dispatcherTimer.Stop();
            }
        }

        private bool UninstallProgram(string ProgramName)
        {
            try
            {
                var mos = new ManagementObjectSearcher(
                  "SELECT * FROM Win32_Product WHERE Name = '" + ProgramName + "'");
                foreach (ManagementObject mo in mos.Get())
                {
                    try
                    {
                        if (mo["Name"].ToString() == ProgramName)
                        {
                            var hr = mo.InvokeMethod("Uninstall", null);
                            
                            return (bool)true;
                        }
                    }
                    catch (Exception)
                    {
                        //this program may not have a name property, so an exception will be thrown
                    }
                }

                //was not found...
                return false;

            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
