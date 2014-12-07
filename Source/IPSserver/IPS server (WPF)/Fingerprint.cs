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
using System.Threading.Tasks;

namespace IPS_server__WPF_
{
    public class Fingerprint
    {    

        public string FName {get;private set;}
        /// <summary>
        /// A Stack to determine how many mac does the fingerprint has.        
        /// <para>If stack is 0, there are no more empty spot for new Mac Address.</para>
        /// </summary>
        private Stack<int> MacStack { get; set; }
        public double x { get; private set; }
        public double y { get; private set; }

        private Dictionary<string, int> Macs;
        private short[,] SigRange;

        private int MacN = 20;

        //################      Constructor     ##################
       /// <summary>
       /// Creates a new Fingerprint object
       /// </summary>
       /// <param name="Fingname">Name of the Fingerprint</param>
       /// <param name="XY">Coordinates of the fingerprint (Will be converted to Double)</param>
       /// <param name="MAddress">Array of associated mac addresses</param>
        public Fingerprint(string Fingname,string[] XY,string[] MAddress)
        {
            int i;
            int macCount = 0;

            MacStack = new Stack<int>(MacN);
            Macs = new Dictionary<string, int>(MacN);
            SigRange = new short[MacN, 2];

            //for (i = 0; i < SigRange.Length; i = i + 2)
            //    SigRange[i, 0] = SigRange[i, 1] = 0;

            this.FName = Fingname;
            x = Convert.ToDouble(XY[0]);
            y = Convert.ToDouble(XY[1]);            

            for (i = SigRange.GetLength(0)-1; i >= 0 ; i--)
                MacStack.Push(i);

            for (i = 0; i < MAddress.Length; i = i + 2)
            {
                macCount = MacStack.Pop();
                Macs.Add(MAddress[i], macCount);
                SigRange[macCount, 0] = SigRange[macCount, 1] = Convert.ToInt16(MAddress[i + 1]);                
            }
        }

        //################      Public Methods     ##################
        /// <summary>
        /// Check and change input's mac addresses and signals with the Fingerprint's mac addresses and signal ranges
        /// <para>For each successive changes to the range, the value returned will increase</para>
        /// <para>Returns 0 if no mac address ranges has been changed</para>
        /// <para>Uses the following functions: MacTable.AddMac</para>
        /// </summary>
        /// <param name="MAddress">array of Mac Address, EG:"M1,20,M2,20...M10,20"</param>
        /// <returns></returns>
        public int FingCalibrate(string[] MAddress)
        {
            lock (MacStack)
            {
                lock (SigRange)
                {
                    int Calibrated = 0;
                    int macIndex, macLoc;

                    bool valueFound = false;

                    for (int i = 0; i < MAddress.Length; i = i + 2)
                    {
                        valueFound = Macs.TryGetValue(MAddress[i], out macIndex);

                        if (valueFound)
                            Calibrated += SigCalibrate(macIndex, Convert.ToInt16(MAddress[i+1]));
                        else if (MacStack.Count != 0)
                        {
                            if (MacTable.AddMac(MAddress[i], MacTable.GetFingObject(FName)))
                            {
                                macLoc = MacStack.Pop();
                                Macs.Add(MAddress[i], macLoc);
                                SigRange[macLoc, 0] = SigRange[macLoc, 1] = Convert.ToInt16(MAddress[i + 1]);
                                Calibrated++;
                            }                            
                        }
                    }
                    return Calibrated;
                }
            }
        }

        /// <summary>
        /// Resets the Mactable,SigRange and MacStack then insert the newly received mac address
        /// </summary>
        /// <param name="MAddress">EG: Mac1,Sig1,Mac2,Sig2,...MacN,SigN</param>
        /// <returns></returns>
        public int FingRecalibrate(string[] MAddress)
        {
            lock (MacStack)
            {
                lock (SigRange)
                {
                    int Calibrated = 0;
                    int index = 0;

                    Macs.Clear();
                    MacStack.Clear();
                    for (int i = MacN-1; i >= 0; i--)
                    {
                        SigRange[i, 0] = SigRange[i, 1] = 0;
                        MacStack.Push(i);
                    }

                    
                    for (int i = 0; i < MAddress.Length; i = i + 2)
                    {
                        index = MacStack.Pop();
                        Macs.Add(MAddress[i], index);
                        SigRange[index, 0] = SigRange[index, 1] = Convert.ToInt16(MAddress[i + 1]);
                        Calibrated++;
                    }
                    return Calibrated;
                }                
            }
        }
        

        /// <summary>
        /// Compare a string of mac addresses + signals with this fingerprint's Mac addresses signal range
        /// <para>Returns the number of signals that passes the checks</para> 
        /// </summary>
        /// <param name="input">String array containing mac and signal subsequently</param>
        /// <param name="precision">Number of passed signals required to consider a locate success</param>
        /// <returns>Returns the number of signals that passes the checks</returns>
        public int MacCompare(string[] input,int precision)
        {
            int macIndex;
            int inRange = 0;
            bool valueFound = false;

            for (int i = 0; i < input.Length && inRange < precision; i = i + 2)
            {
                valueFound = Macs.TryGetValue(input[i], out macIndex);
                if (valueFound)
                    inRange = inRange + SigCompare(macIndex, Convert.ToInt16(input[i+1]));
            }
            
            return inRange;
        }

        /// <summary>
        /// Attempt to Deassociate mac address from this fingerprint
        /// <para>Returns 1 if success, -1 if failed</para>
        /// </summary>
        /// <param name="input">A string containing the Mac Address</param>
        /// <returns>Returns 1 if success, -1 if mac not found</returns>
        public int MacRemove(string input)
        {
            lock (MacStack)
            {
                lock (SigRange)
                {
                    int macIndex;
                    bool macFound = Macs.TryGetValue(input, out macIndex);

                    if (macFound)
                    {
                        SigRange[macIndex, 0] = SigRange[macIndex, 1] = 0;
                        MacStack.Push(macIndex);
                        Macs.Remove(input);
                        return 1;
                    }
                    else
                        return -1;
                }
            }
        }

        /// <summary>
        /// Dereference this fingerprint from associated Mac Address
        /// <para>Returns a string array containing the associated mac to this fingerprint</para>
        /// </summary>
        /// <returns>Returns a string array containing the associated mac to this fingerprint</returns>
        public string[] FingRemove()
        {
            string[] MacList = new string[Macs.Count()];
            int i=0;
            foreach (KeyValuePair<string, int> Keys in Macs)
                MacList[i++] = Keys.Key.ToString();
            return MacList;
        }

        /// <summary>
        /// Returns a string array that can be parsed by the parser for Loading purposes
        /// </summary>
        /// <returns></returns>
        public string[] saveFing()
        {
            string[] result = new string[2];
            StringBuilder buildresult = new StringBuilder();
            StringBuilder buildresult2 = new StringBuilder();

            int i = 0;
            foreach (KeyValuePair<string, int> Keys in Macs)
            {
                buildresult.AppendFormat(",{0},{1}", Keys.Key.ToString(), SigRange[Keys.Value, 0]);
                buildresult2.AppendFormat(",{0},{1}", Keys.Key.ToString(), SigRange[Keys.Value, 1]);
                i++;
            }
                
                result[0] = string.Format("addfing,{0},{1},{2}{3}", FName, x, y, buildresult);
                result[1] = string.Format("calibrate,{0}{1}",FName ,buildresult2);

                return result;
        }

        //################      Private Methods     ##################
        /// <summary> 
        /// Returns 1 if the input replaces the MIN or MAX value in the signal array
        /// otherwise, returns 0 for no changes
        /// </summary>
        /// <param name="index">index of the signal range in SigArray</param>
        /// <param name="input">the value to be compared with the signal range for calibration</param>
        /// <returns>Returns 1 if the input replaces the MIN or MAX value in the signal array
        /// <para>returns 0 for no changes</para></returns>
        private int SigCalibrate(int index,short input)
        {
            lock (SigRange)
            {
                if (input < SigRange[index, 0])
                {
                    SigRange[index, 0] = input;
                    return 1;
                }
                else if (input > SigRange[index, 1])
                {
                    SigRange[index, 1] = input;
                    return 1;
                }
                else
                    return 0;
            }
        }

        /// <summary>
        /// Checks if the input is within the range of the fingerprint's Mac address.
        /// <para>Returns 0 if in range, 1 if not in range.</para>
        /// </summary>
        /// <param name="index">The index of MacAddress in SigRange</param>
        /// <param name="input">The value to be compared with the SigRange</param>
        /// <returns></returns>
        private int SigCompare(int index, short input)
        {
            if (input < SigRange[index, 0])
                return 0;
            else if (input > SigRange[index, 1])
                return 0;            
            else
                return 1;
        }

    }
}
