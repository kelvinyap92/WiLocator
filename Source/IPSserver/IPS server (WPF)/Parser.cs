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
    public static class Parser
    {
        public static int Precision { get; set; }        
        

        public static string Parse(string input)
       {
           input.TrimEnd(' ');
           input = input.Replace("\r\n", "");
           string[] split = input.Split(',');
           string Fname,result;
           string[] XY,mac;
           

           switch (split[0].ToLower())
           {               

              case "addfing":
                   int addResult;
                   Fname = split[1];
                   XY = new string[]{ split[2], split[3] };
                   mac = new string[split.Length - 4];

                   for (int i = 0; (i+4) < split.Length; i++)
                       mac[i] = split[i+4];

                   result = MacTable.ScanLocation(mac, Precision);

                   if (result.CompareTo("NULL") == 0)
                   {
                       addResult = MacTable.AddFing(Fname, XY, mac);

                       if (addResult == 0)
                           result = string.Format("Successfully added the fingerprint: {0}", Fname);
                       else
                           result = string.Format("Failed to add fingerprint: {0}, the name is being used!", Fname);
                   }
                   else
                       result = string.Format("Fingerprint {0} collision with current add attempt",result);

                   return result;

               case "delfing":
                   Fname = split[1];
                   if (MacTable.DelFing(Fname) == 0)
                       result = string.Format("Deleted Fingerprint: {0}", Fname);
                   else
                       result = string.Format("Failed to delete Fingerprint: {0}, Fingerprint does not exist!", Fname);
                   return result;

               case "calibrate":
                   int Calibration;
                   Fname = split[1];
                   mac = new string[split.Length - 2];

                   for (int i = 0; (i+2) < split.Length; i++)
                       mac[i] = split[i+2];

                   Calibration = MacTable.CalibrateFing(Fname, mac);

                   if (Calibration != -1)
                       result = string.Format("Calibrated {0} signal range for Fingerprint: {1}", Calibration, Fname);
                   else
                       result = "Fingerprint not found";

                   return result;

               case "recalibrate":
                   int Calibration2;
                   Fname = split[1];
                   mac = new string[split.Length - 2];

                   for (int i = 0; (i + 2) < split.Length; i++)
                       mac[i] = split[i + 2];

                   Calibration2 = MacTable.RecalibrateFing(Fname, mac);

                   if (Calibration2 != -1)
                       result = string.Format("Recalibrated {0} signal range for Fingerprint: {1}", Calibration2, Fname);
                   else
                       result = "Fingerprint not found";

                   return result;

               case "locate":
                   mac = new string[split.Length - 1];

                   for (int i = 0; (i+1) < split.Length; i++)
                       mac[i] = split[i+1];

                   if (mac.Length >= 6)
                       result = MacTable.ScanLocation(mac, Precision);
                   else
                       result = "DeadZone";

                   return result;

               case "addblack":
                   if (split.Length > 1)
                   {
                       if (MacTable.BlacklistMac(split[1]))
                           result = string.Format("Successfully added Mac Address \"{0}\" into blacklist", split[1]);
                       else
                           result = string.Format("Mac Address \"{0}\" already exist in the blacklist", split[1]);
                   }
                   else
                       result = ("Empty name detected!");
                   return result;

               case "delblack":
                   if (split.Length > 1)
                   {
                       if (MacTable.WhitelistMac(split[1]))
                           result = string.Format("Successfully removed Mac Address \"{0}\" from blacklist", split[1]);
                       else
                           result = string.Format("Mac Address \"{0}\" does not exist in the blacklist", split[1]);
                   }
                   else
                       result = ("Empty name detected!");
                   return result;
                      
               case "emergency":
                   Fname = split[1];
                   XY = new string[]{ split[2], split[3] };
                   return string.Format("Broadcast,{0},{1},{2}",Fname,XY[0],XY[1]);

               case "remergency":
                   return "Not Implemented yet";
                   
               default:
                   Console.WriteLine(">>Parser received unknown command");
                   Console.WriteLine(input);
                   return ("ERROR");
           }
            
       }
    }


}
