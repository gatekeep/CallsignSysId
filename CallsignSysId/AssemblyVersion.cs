using System;
using System.Reflection;

namespace CallsignSysId
{
    /// <summary>
    /// Static class to build the version string.
    /// </summary>
    public class AssemblyVersion
    {
        /**
         * Fields
         */
        public static string _NAME;
        public static string _VERSION;
        public static string _COMPANY;
        public static string _BUILD;
        public static string _BUILD_DATE;
        public static string _COPYRIGHT;
        public static string _VERSION_STRING;

        /**
         * Methods
         */
        /// <summary>
        /// Initializes static members of the AssemblyVersion class.
        /// </summary>
        static AssemblyVersion()
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            AssemblyDescriptionAttribute asmDesc = asm.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false)[0] as AssemblyDescriptionAttribute;
            AssemblyCopyrightAttribute asmCopyright = asm.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false)[0] as AssemblyCopyrightAttribute;
            AssemblyCompanyAttribute asmCompany = asm.GetCustomAttributes(typeof(AssemblyCompanyAttribute), false)[0] as AssemblyCompanyAttribute;
            DateTime buildDate = new DateTime(2000, 1, 1).AddDays(asm.GetName().Version.Build).AddSeconds(asm.GetName().Version.Revision * 2);
            _NAME = asmDesc.Description;
            _VERSION = asm.GetName().Version.Major + "." + asm.GetName().Version.Minor;
            _COMPANY = asmCompany.Company;
            _BUILD = string.Empty + "B" + asm.GetName().Version.Build + "R" + asm.GetName().Version.Revision;
            _BUILD_DATE = buildDate.ToShortDateString() + " at " + buildDate.ToShortTimeString();
            _COPYRIGHT = asmCopyright.Copyright;

#if DEBUG
            _VERSION_STRING = AssemblyVersion._NAME + " " + AssemblyVersion._VERSION + " DEBUG_DNR build " + AssemblyVersion._BUILD;
#else
            _VERSION_STRING = AssemblyVersion._NAME + " " + AssemblyVersion._VERSION + " build " + AssemblyVersion._BUILD;
#endif
        }
    } // public class AssemblyVersion
} // namespace CallsignSysId
