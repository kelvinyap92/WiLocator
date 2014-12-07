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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace IPS_server__WPF_
{
    class MacTable
    {      

        private static MultiValueDictionary<string, Fingerprint> MacDict;
        private static Dictionary<string, Fingerprint> FingDict;
        private static HashSet<string> MacBlacklist;
        private static Dictionary<string, short[]> EmergencyDict;

        private static string filepath = Environment.CurrentDirectory + "\\Backup\\test.txt";
        private static string dirpath = Environment.CurrentDirectory + "\\Backup";
        private static FileStream FS;
        private static StreamWriter SW;
        private static StreamReader SR;
        private static object ReadWriteLock = new object();

        //################      Initialization     ##################
        static MacTable()
        {            
            MacDict = new MultiValueDictionary<string,Fingerprint>(1000);
            FingDict = new Dictionary<string, Fingerprint>(1000);
            MacBlacklist = new HashSet<string>();
            EmergencyDict = new Dictionary<string,short[]>(1000);
            Parser.Precision = 6;
            
            if (!Directory.Exists(dirpath))
                Directory.CreateDirectory(dirpath);
            
        }

        //#################     PUBLIC METHODS     ###################
        //#################     METHODS CALLED BY SERVER     ###################
        /// <summary>
        /// Saves every Blacklisted Mac and Fingerprint into Test.txt
        /// <para>returns the number of commands generated</para>
        /// </summary>
        /// <returns>returns the number of commands generated</returns>
        public static int Save()
        {
            lock (ReadWriteLock)
            {
                    FS = new FileStream(filepath, FileMode.Create, FileAccess.Write);
                    SW = new StreamWriter(FS);

                    string[] saving;
                    int commandCounter = 0;

                    foreach (string MBlack in MacBlacklist)
                    {
                        SW.WriteLine("addblack,{0}", MBlack);
                        commandCounter++;
                    }
                    
                    foreach (KeyValuePair<string, Fingerprint> Keys in FingDict)
                    {
                        saving = Keys.Value.saveFing();
                        for (int i = 0; i < saving.Length; i++, commandCounter++)
                            SW.WriteLine(saving[i]);
                    }
                    SW.Flush();
                    SW.Close();
                    FS.Close();

                    return commandCounter;                    
            }
                    
        }

        /// <summary>
        /// Read and parse commands line by line from file
        /// <para>returns number of commands loaded</para>
        /// </summary>
        /// <returns>returns number of commands loaded</returns>
        public static int Load()
        {
            lock (ReadWriteLock)
            {
                try
                {
                    FS = new FileStream(filepath, FileMode.Open, FileAccess.Read);
                    SR = new StreamReader(FS);

                    int commandCounter = 0;
                    string loading = SR.ReadLine();

                    while (loading != null)
                    {
                        Parser.Parse(loading);
                        commandCounter++;
                        loading = SR.ReadLine();
                    }
                    SR.Close();
                    FS.Close();
                    return commandCounter;
                }
                catch (DirectoryNotFoundException e)
                { Console.WriteLine(e); return -1; }
                catch (FileNotFoundException e)
                { Console.WriteLine(e); return -2; }
                
            }
        }

        //#################     METHODS CALLED BY PARSER     ###################
        /// <summary>
        /// Adds a new Fingerprint into the database
        /// <para>0 if success, -1 if Fingerprint exists</para>
        /// <para>Calls by the switch</para>
        /// </summary>
        /// <param name="Fingname">Name of the Fingerprint</param>
        /// <param name="XY">Coordinates of the XY point</param>
        /// <param name="MAddress">Array of Mac Address</param>
        /// <returns>0 if success, -1 if Fingerprint exists</returns>
        public static int AddFing(string Fingname, string[] XY, string[] MAddress)
        {
            lock (MacBlacklist)
            {
                lock (MacDict)
                {
                    lock (FingDict)
                    {
                        int returncode;
                        Fingerprint Finger;

                        bool FingFound = FingDict.TryGetValue(Fingname, out Finger);

                        if (!FingFound)
                        {
                            try
                            {
                                for (int i = 0; i < MAddress.Length; i = i + 2)
                                    if (MacBlacklist.Contains<string>(MAddress[i]))
                                        MAddress[i] = MAddress[i + 1] = "";

                                Finger = new Fingerprint(Fingname, XY, MAddress);

                                FingDict.Add(Fingname, Finger);

                                for (int i = 0; i < MAddress.Length; i = i + 2)
                                    MacDict.Add(MAddress[i], Finger);

                                //GUI function here
                               // MainWindow.addFinger(Finger);
                                
                                returncode = 0;
                            }
                            catch(Exception e)
                            {
                                Console.WriteLine(">>{0}\nStack Trace:{1}", e.Message,e.StackTrace);

                                returncode = -2;
                            }
                        }
                        else                        
                            returncode = -1;
                        
                        return returncode;
                    }
                    
                }
            }
        }


        /// <summary>
        /// Dereference a fingerprint from all table for GC to collect
        /// <para>returns 0 if removal success, -1 otherwise</para>
        /// </summary>
        /// <param name="Fingname">Name of fingerprint to be removed</param>
        /// <returns>0 if removed, -1 if failed</returns>
        public static int DelFing(string Fingname)
        {
            lock(MacDict)
            {
                lock(FingDict)
                {
                    int returncode=-1;
                    Fingerprint finger = GetFingObject(Fingname);
                    string[] MacList = new string[10];
                    if (finger != null)
                    {
                        MacList = finger.FingRemove();
                        foreach (string delref in MacList)
                        {
                            if (delref == null)
                                break;
                            MacDict.Remove(delref, finger);
                        }
                        FingDict.Remove(Fingname);
                        returncode = 0;
                    }
                    return returncode;
                }
            }
        }


        /// <summary>
        /// Calibrates the fingerprint's mac address signal range
        /// <para>Returns -1 if Fingerprint is not found, otherwise return number of changes</para>
        /// </summary>
        /// <param name="Fingername">Name of the fingerprint to be calibrated</param>
        /// <param name="macAddress">Array containing Mac addresses and signals for checking</param>
        /// <returns>Returns -1 if Fingerprint is not found, otherwise return number of changes</returns>
        public static int CalibrateFing(string Fingername,string[] macAddress)
        {            
            int Calibrate;
            Fingerprint Finger;
            
            bool FoundFing = FingDict.TryGetValue(Fingername, out Finger);

            if (FoundFing)            
                Calibrate = Finger.FingCalibrate(macAddress);            
            else
                Calibrate = -1;

            return Calibrate;
        }

        /// <summary>
        /// Clears a Fingerprint's associated mac address table and then reassociate with 
        /// <para>the new mac address and signals</para>
        /// </summary>
        /// <param name="Fingername">Name of the fingerprint to be recalibrated</param>
        /// <param name="macAddress">Array containing Mac addresses and signals for checking</param>
        /// <returns></returns>
        public static int RecalibrateFing(string Fingername,string[] macAddress)
        {
            int Calibrate;
            Fingerprint Finger;

            bool FoundFing = FingDict.TryGetValue(Fingername, out Finger);

            if (FoundFing)
                Calibrate = Finger.FingRecalibrate(macAddress);
            else
                Calibrate = -1;

            return Calibrate;
        }


        /// <summary>
        /// Not implemented yet due to Core does not meet sufficient precision
        /// </summary>
        /// <param name="Emername">Emergency's name</param>
        /// <param name="XY">Coordinate of the point on the map</param>
        /// <returns>True if success, false otherwise</returns>
        public static bool AddEmergency(string Emername, string[] XY)
        {
            lock (EmergencyDict)
            {
                bool EmerExist = EmergencyDict.ContainsKey(Emername);
                string[] test = new string[50];

                if (EmerExist)
                    return false;
                return false;
            }
        }
        

        /// <summary>
        /// Check for the location of the client by comparing client's mac addresses and signals
        /// <para>with the mac addresses and signals in the Fingerprint database</para>
        /// <para>Returns the Fingerprint name if found, otherwise return empty string</para>
        /// </summary>
        /// <param name="macAddress">Array containing the mac addresses and signals</param>
        /// <param name="precision">The number of macs that are in range to be considered a successful comparison</param>
        /// <returns>Returns the Fingerprint name if found, returns null otherwise</returns>
        public static string ScanLocation(string[] macAddress, int precision)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            int Scan = -1;
            IReadOnlyCollection<Fingerprint> Finglist;
            List<Fingerprint> previous = new List<Fingerprint>();

            bool FoundMac;
            
            for (int i = 0; i < macAddress.Length/2; i = i + 2)
            {
                FoundMac = MacDict.TryGetValue(macAddress[i], out Finglist);
                if (FoundMac)
                {
                    foreach (Fingerprint Finger in Finglist)
                    {                        
                            if (!previous.Contains(Finger))
                            {
                                previous.Add(Finger);
                                Scan = Finger.MacCompare(macAddress, precision);
                                if (Scan >= precision)
                                {
                                    stopwatch.Stop();
                                    Console.WriteLine("Time taken for locate: {0} milliseconds", stopwatch.ElapsedMilliseconds);
                                    return Finger.FName;
                                }
                            }
                    }
                }
            }

            stopwatch.Stop();
            Console.WriteLine("Time taken for locate: {0} milliseconds", stopwatch.ElapsedMilliseconds);
            return "NULL";
        }

        /// <summary>
        /// Adds a Mac address to a Blacklist HashSet "MacBlacklist"         
        /// <para>Upon successful add to blacklist, Removes every fingerprint that contains this mac address</para>
        /// <para>after deassociating given mac from every fingerprint, removes mac address from MacDict </para>
        /// <para>Calls by the switch</para>
        /// </summary>
        /// <param name="Mac">the Mac address to be blacklisted</param>
        /// <returns>True if success, False otherwise</returns>
        public static bool BlacklistMac(string Mac)
        {            
            lock (MacBlacklist)
            {
                lock (MacDict)
                {
                    bool returnbool = false;

                    bool BlackMacExist = MacBlacklist.Add(Mac);
                    if (BlackMacExist)
                    {
                        try
                        {
                            IReadOnlyCollection<Fingerprint> Finglist;
                            bool MacExist = MacDict.TryGetValue(Mac, out Finglist);

                            if (MacExist)
                            {
                                foreach (Fingerprint Fing in Finglist)
                                    Fing.MacRemove(Mac);

                                MacDict.Remove(Mac);                                
                            }
                            returnbool = true;
                        }
                        catch(Exception e)
                        {
                            Console.WriteLine(">>{0}\nStack Trace:{1}", e.Message, e.StackTrace);
                            returnbool = false;
                        }
                    }
                    else
                    {
                        Console.WriteLine(">>MacTable - Attempted to blacklist an existing MAC: {0}", Mac);
                        returnbool = false;
                    }
                    return returnbool;
                }
            }            
        }
     
        /// <summary>
        /// Removes Mac Address from the blacklist
        /// <para>return TRUE if success, false if not exist</para>
        /// </summary>
        /// <param name="Mac"></param>
        /// <returns></returns>
        public static bool WhitelistMac(string Mac)
        {
            lock (MacBlacklist)
            {
                lock (MacDict)
                {
                    bool returnbool = false;

                    bool BlackMacExist = MacBlacklist.Contains(Mac);
                    if(BlackMacExist)
                    {
                        MacBlacklist.Remove(Mac);
                        returnbool = true;
                    }
                    else
                    {
                        returnbool = false;
                    }
                    return returnbool;
                }
            }
        }

        //#################     METHODS CALLED BY FINGERPRINT CLASS    ###################

        /// <summary>
        /// Adds the given Mac Address and Fingerprint into the dictionary
        /// <para>If given Mac Address exist in blacklist, function will not attempt to add the mac address</para>
        /// <para>Returns true if success, Fail otherwise (probably blacklisted or exist)</para>
        /// </summary>
        /// <param name="Mac">The mac adress where the Fingerprint will be added to</param>
        /// <param name="Finger">The Fingerprint to be added</param>
        /// <returns>True if success, Fail otherwise</returns>
        public static bool AddMac(string Mac, Fingerprint Finger)
        {
            lock (MacBlacklist)
            {
                lock (MacDict)
                {
                    bool returnbool = false;
                    bool blacklistedmac = MacBlacklist.Contains(Mac);
                    if (!blacklistedmac)
                    {
                        try
                        {
                            IReadOnlyCollection<Fingerprint> Finglist;
                            bool FoundMac = MacDict.TryGetValue(Mac, out Finglist);

                            if (FoundMac)
                            {
                                if (Finglist.Contains<Fingerprint>(Finger))
                                    returnbool = true;
                            }
                            else
                            {
                                MacDict.Add(Mac, Finger);
                                returnbool = true;
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(">>{0}\nStack Trace \n{1}", e.Message, e.StackTrace);
                            returnbool = false;
                        }
                    }
                    else
                        returnbool = false;
                    return returnbool;
                }
            }
        }

        /// <summary>
        /// Returns the pointer to the Fingerprints if found.
        /// </summary>
        /// <param name="Finger">the name of the Fingerprint object</param>        
        /// <returns>Output if found, null otherwise</returns>
        public static Fingerprint GetFingObject(string Finger)
        {
            Fingerprint output;
            bool FoundFinger = FingDict.TryGetValue(Finger, out output);
            if (FoundFinger)
                return output;
            else
                return null;
        }
     
    }
}
