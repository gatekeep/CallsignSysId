using System;
using System.Globalization;
using System.Collections.Generic;
using System.Text;

namespace CallsignSysId
{
    class Program
    {
        public static readonly string[] RADIX_50 =  new string[] 
        {
            " ", "A", "B", "C", "D", "E", "F", "G", 
		    "H", "I", "J", "K", "L", "M", "N", "O", 
		    "P", "Q", "R", "S", "T", "U", "V", "W", 
		    "X", "Y", "Z", "$", ".", "?", "0", "1", 
		    "2", "3", "4", "5", "6", "7", "8", "9"
        };

        /**
	     * Decodes the string callsign from which the WACN and System identifiers
	     * were derived.
	     * 
	     * Based on code by Eric Carlson at:
	     * http://www.ericcarlson.net/project-25-callsign.html
	     * 
	     * @param wacn
	     * @param systemID
	     * @return
	     */
        static string getCallsign(int wacn, int systemID)
        {
            int n1 = wacn / 16;
            int n2 = 4096 * (wacn % 16) + systemID;

            StringBuilder sb = new StringBuilder();

            sb.Append(getLetter((int)(n1 / 1600)));
            sb.Append(getLetter((int)((n1 / 40) % 40)));
            sb.Append(getLetter((int)(n1 % 40)));
            sb.Append(getLetter((int)(n2 / 1600)));
            sb.Append(getLetter((int)((n2 / 40) % 40)));
            sb.Append(getLetter((int)(n2 % 40)));

            return sb.ToString();
        }

        static string getLetter(int value)
        {
            if (0 <= value && value < 40)
                return RADIX_50[value];

            return " ";
        }

        static void Usage(OptionSet p)
        {
            Console.WriteLine("usage: CallsignSysId <extra arguments ...>");
            Console.WriteLine();
            Console.WriteLine("Options:");
            p.WriteOptionDescriptions(Console.Out);
            Console.WriteLine();
        }

        static int GetNumber(string s)
        {
            int n = 0x0;
            if (s.StartsWith("0x", StringComparison.CurrentCultureIgnoreCase) ||
                s.StartsWith("&H", StringComparison.CurrentCultureIgnoreCase))
            {
                s = s.Substring(2);
            }

            if (!int.TryParse(s, NumberStyles.HexNumber, CultureInfo.CurrentCulture, out n))
            {
                if (!int.TryParse(s, NumberStyles.Integer, CultureInfo.CurrentCulture, out n))
                    Console.WriteLine("could not processed passed number");
            }

            return n;
        }

        static void Main(string[] args)
        {
            List<string> extraArgs = new List<string>();
            string callsign = string.Empty;
            int wacn = 0xBEE00, sysId = 0x001;
            bool decode = false, encode = false;
            bool showHelp = false;

            // command line parameters
            OptionSet options = new OptionSet()
            {
                { "w=", "WACN", v => wacn = GetNumber(v) },
                { "s=", "System ID", v => sysId = GetNumber(v) },
                { "c=", "Callsign", v => callsign = v },

                { "decode", "decode WACN and System ID", v => decode = v != null },
                { "encode", "encode WACN and System ID", v => encode = v != null },
                { "h|help", "show this message and exit", v => showHelp = v != null },
            };

            // attempt to parse the commandline
            try
            {
                extraArgs = options.Parse(args);
            }
            catch (OptionException)
            {
                if (args.Length == 0)
                {
                    Console.WriteLine("error: invalid arguments");
                    Usage(options);
                    Environment.Exit(-1);
                }
            }

            Console.WriteLine(">>> " + AssemblyVersion._VERSION_STRING + " (Built " + AssemblyVersion._BUILD_DATE + ")");
            Console.WriteLine(">>> " + AssemblyVersion._COPYRIGHT + " All Rights Reserved.");
            Console.WriteLine("OSVersion: " + Environment.OSVersion.ToString());
            Console.WriteLine("OSPlatform: " + Environment.OSVersion.Platform.ToString());

            if (showHelp)
            {
                Usage(options);
                return;
            }

            if (decode)
            {
                Console.WriteLine(string.Format("Decoding WACN = {0}, System ID = {1}", wacn.ToString("X"), sysId.ToString("X")));
                Console.WriteLine(string.Format("Callsign = \"{0}\"", getCallsign(wacn, sysId)));
            }
            if (encode)
            {
                if (callsign.Length < 6)
                {
                    int len = callsign.Length;
                    for (int i = 0; i < (6 - len); i++)
                        callsign = " " + callsign;
                }
                else
                    callsign = callsign.Substring(0, 6);

                Console.WriteLine(string.Format("Encoding Callsign = \"{0}\"", callsign));
                string first = callsign.Substring(0, 3);

                int eWACN = 0;
                for (int i = 0; i < 0xFFFFF; i++)
                {
                    string call = getCallsign(i, 0x000);
                    if (call.ToUpper() == (first + "   "))
                    {
                        eWACN = i;
                        break;
                    }
                }

                int eSysId = 0;
                do
                {
                    for (int i = 0; i < 0xFFF; i++)
                    {
                        string call = getCallsign(eWACN, i);
                        if (call.ToUpper() == callsign)
                        {
                            eSysId = i;
                            break;
                        }
                    }

                    if (eSysId == 0)
                        eWACN++;
                } while (eSysId == 0);

                Console.WriteLine(string.Format("WACN = {0}, System ID = {1}", eWACN.ToString("X"), eSysId.ToString("X")));
            }
        }
    }
}
