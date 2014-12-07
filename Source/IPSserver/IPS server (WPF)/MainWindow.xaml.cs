////This program can receive TCP connection from client and then attempt to
//// compute the client's location based on the information given
////    Copyright (C) 2014, Davion Teh
////This file is part of "IPS server".

////    "IPS server" is free software: you can redistribute it and/or modify
////    it under the terms of the GNU General Public License as published by
////    the Free Software Foundation, either version 3 of the License, or
////    (at your option) any later version.

////    "IPS server" is distributed in the hope that it will be useful,
////    but WITHOUT ANY WARRANTY; without even the implied warranty of
////    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
////    GNU General Public License for more details.

////    You should have received a copy of the GNU General Public License
////    along with "IPS server".  If not, see <http://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
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
using System.Diagnostics;

namespace IPS_server__WPF_
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        ClientHandler CH;
        TCPserver TS;
        MacTable MT;
        Regex regex = new Regex("[^0-9]", RegexOptions.IgnoreCase);
        private bool ServerMode = false; //False = not listening
        int exitCode = 0;

        public MainWindow()
        {
            MT = new MacTable();
            CH = new ClientHandler();
            TS = new TCPserver();
            InitializeComponent();
        }
        //#################     PUBLIC METHODS    ###################


        //#################     PRIVATE METHODS    ###################
        private void BListen_Click(object sender, RoutedEventArgs e)
        {

            if (!ServerMode)
            {
                Match match = regex.Match(TBPort.Text);
                if (match.Success)
                {
                    Console.WriteLine(">>Main - Received invalid port \"{0}\"", TBPort.Text);
                    return;
                }

                int port = Int32.Parse(TBPort.Text);

                //CH.startServer(port,out exitCode);
                exitCode = TS.StartServer(port);

                if (exitCode == 0)
                {
                    ServerMode = true;

                    //FALSE -> TRUE GUI Toggle stuff here
                    STStatus.Content = "Waiting for Client";
                    BListen.Content = "Stop Listening";
                    TBPort.IsEnabled = false;
                }
            }
            else
            {

                //if (CH.stopServer() == 0)
                if (TS.StopServer() == 0)
                {
                    ServerMode = false;

                    //TRUE -> FALSE Gui Toggle stuff here
                    STStatus.Content = "Not Listening";
                    BListen.Content = "Start Listening";
                    TBPort.IsEnabled = true;
                }
            }

        }

        private void BSave_Click(object sender, RoutedEventArgs e)
        {

            MessageBoxResult result = MessageBox.Show("Are you sure you want to save?", "Save", MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.No);
            if (result == MessageBoxResult.Yes)
            {
                Stopwatch timing = new Stopwatch();

                timing.Start();
                int count = MacTable.Save();
                timing.Stop();

                Console.WriteLine("Time taken to save {0} commands is {1} seconds", count, (float)timing.ElapsedMilliseconds / 1000);
            }
            else
                return;
        }

        private void BLoad_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to load?", "Load", MessageBoxButton.YesNoCancel);
            if (result == MessageBoxResult.Yes)
            {
                Stopwatch timing = new Stopwatch();

                timing.Start();
                int count = MacTable.Load();
                timing.Stop();

                if (!(count < 0))
                    Console.WriteLine("Time taken to load {0} commands is {1} seconds", count, (float)timing.ElapsedMilliseconds / 1000);                    
                else
                    Console.WriteLine("\nLoad Failed");
            }
            else
                return;
        }

        private void BSetprecision_Click(object sender, RoutedEventArgs e)
        {
            Parser.Precision = Convert.ToInt32(TBPrecision.Text);
        }
    }
}
