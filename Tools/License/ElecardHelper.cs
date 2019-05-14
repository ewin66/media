using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

using ElecardModuleConfig;

namespace FutureConcepts.Media.Tools.License
{
    public class ElecardHelper
    {
        public static void Unlock()
        {
            try
            {
                ActivateFilter(new Guid("6F470083-A06D-4C06-8664-85BBAAB453A0"), "Elecard RTSP RTSP NetSource");
                ActivateFilter(new Guid("5C122C6D-8FCC-46F9-AAB7-DCFB0841E04D"), "Elecard AVC Video Decoder");
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception in ElecardHelper Unlock");
                Debug.WriteLine(e.Message);
            }
        }

        private static void ActivateFilter(Guid clsid, String friendlyName)
        {
            Type type = Type.GetTypeFromCLSID(clsid);
            Object filter = Activator.CreateInstance(type);
            ElecardModuleConfig.IModuleConfig moduleConfig = (ElecardModuleConfig.IModuleConfig)filter;
     //       moduleConfig.SetValue(FC_GUID, null);
            Marshal.ReleaseComObject(moduleConfig);
        }
    }
}
