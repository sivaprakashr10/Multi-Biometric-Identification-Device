using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using WpfApp1.Model;
using zkemkeeper;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for home.xaml
    /// </summary>
    public partial class home : Window
    {
        public home()
        {
            InitializeComponent();
        }
        string ipAddress = "192.168.1.45";
        int port = 4370;
        public zkemkeeper.CZKEMClass axCZKEM1 = new zkemkeeper.CZKEMClass();
        private bool bIsConnected = false;
        private int iMachineNumber = 1;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MakeConnection();
        }
        private async void MakeConnection()
        {
            bIsConnected = axCZKEM1.Connect_Net(ipAddress, port);
            if (bIsConnected == true)
            {
                while (!GetGeneralLogData())
                {
                    await Task.Delay(1000);
                }

            }
            else
            {
                MessageBox.Show("Unable to connect the device");
            }
        }

        private bool GetGeneralLogData()
        {
            int idwErrorCode = 0;

            string sdwEnrollNumber = "";
            int idwVerifyMode = 0;
            int idwInOutMode = 0;
            int idwYear = 0;
            int idwMonth = 0;
            int idwDay = 0;
            int idwHour = 0;
            int idwMinute = 0;
            int idwSecond = 0;
            int idwWorkcode = 0;
            string Name = "";
            string Password = "";
            int iGLCount = 0;
            int iIndex = 0;
            int Privilege = 0;
            bool Enabled;

            //axCZKEM1.EnableDevice(iMachineNumber, false);
            if (axCZKEM1.ReadGeneralLogData(iMachineNumber))
            {
                while (axCZKEM1.SSR_GetGeneralLogData(iMachineNumber, out sdwEnrollNumber, out idwVerifyMode,
                            out idwInOutMode, out idwYear, out idwMonth, out idwDay, out idwHour, out idwMinute, out idwSecond, ref idwWorkcode))//get records from the memory
                {
                    //MessageBox.Show(sdwEnrollNumber);
                   while (axCZKEM1.SSR_GetUserInfo(iMachineNumber, sdwEnrollNumber, out Name, out Password, out Privilege, out Enabled))
                    {
                        //MessageBox.Show(Name);
                        GetUserFace(Name, sdwEnrollNumber);
                    }
                    //clearLog();
                    Biometrics biometrics = new Biometrics()
                    {
                        id = sdwEnrollNumber,
                        idVerifyMode = idwVerifyMode.ToString(),
                        idInOutMode = idwInOutMode.ToString(),
                        date = idwYear.ToString() + "-" + idwMonth.ToString() + "-" + idwDay.ToString() + " " + idwHour.ToString() + ":" + idwMinute.ToString() + ":" + idwSecond.ToString(),
                        workMode = idwWorkcode.ToString()
                    };
                }
                return true;
            }
            else
            {

                return false;
            }
            //axCZKEM1.EnableDevice(iMachineNumber, true);
        }

        private void GetUserFace(string userName, string dwEnrollNumber)
        {
            byte PhotoData = 0;
            int PhotoLength = 0;
            string TmpData = "";
            int TmpLength = 0;
            string AllPhotoName = "";
            string PhotoName = $"{dwEnrollNumber}.jpg";
            string dir = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "UserPhotos");

            string pathToSave = System.IO.Path.Combine(dir, userName);
            while (axCZKEM1.GetUserFacePhotoByName(iMachineNumber, PhotoName, out PhotoData, out PhotoLength))
            {
                MessageBox.Show(AllPhotoName);
            }
            //axCZKEM1.ReadUserAllTemplate(ima);
        }

        private void btnCLearLog(object sender, RoutedEventArgs e)
        {
            clearLog();
        }

        private void clearLog()
        {
            if (axCZKEM1.ClearGLog(iMachineNumber))
            {
                axCZKEM1.RefreshData(iMachineNumber);
                MessageBox.Show("All att Logs have been cleared from teiminal!", "Success");
            }
        }
    }

}
