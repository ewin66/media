using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
//using System.Drawing;
//using System.Text;
//using System.Threading;
using System.Windows.Forms;
using System.Runtime.InteropServices;

using FutureConcepts.Media.DirectShowLib;

namespace FutureConcepts.Media.TV.Scanner
{
    public class FilterPropertyPage
    {
        private Control _parentControl = null;
	    private IBaseFilter _baseFilter = null;

        public FilterPropertyPage(Control parentControl, IBaseFilter baseFilter)
        {
            _parentControl = parentControl;
            _baseFilter = baseFilter;
        }

        public void Show()
        {
            //Get the ISpecifyPropertyPages for the filter
            ISpecifyPropertyPages pProp = _baseFilter as ISpecifyPropertyPages;
            int hr = 0;

            if (pProp == null)
            {
                //If the filter doesn't implement ISpecifyPropertyPages, try displaying IAMVfwCompressDialogs instead!
                IAMVfwCompressDialogs compressDialog = _baseFilter as IAMVfwCompressDialogs;
                if (compressDialog != null)
                {

                    hr = compressDialog.ShowDialog(VfwCompressDialogs.Config, IntPtr.Zero);
                    DsError.ThrowExceptionForHR(hr);
                }
                else
                {
                    MessageBox.Show("Item has no property page", "No Property Page", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                return;
            }

            //Get the name of the filter from the FilterInfo struct
            FilterInfo filterInfo;
            hr = _baseFilter.QueryFilterInfo(out filterInfo);
            DsError.ThrowExceptionForHR(hr);

            // Get the propertypages from the property bag
            DsCAUUID caGUID;
            hr = pProp.GetPages(out caGUID);
            DsError.ThrowExceptionForHR(hr);

            //Create and display the OlePropertyFrame
            object oDevice = (object)_baseFilter;
            hr = NativeMethods.OleCreatePropertyFrame(_parentControl.Handle, 0, 0, filterInfo.achName, 1, ref oDevice, caGUID.cElems, caGUID.pElems, 0, 0, IntPtr.Zero);
            DsError.ThrowExceptionForHR(hr);

            Marshal.ReleaseComObject(oDevice);

            if (filterInfo.pGraph != null)
            {
                Marshal.ReleaseComObject(filterInfo.pGraph);
            }

            // Release COM objects
            Marshal.FreeCoTaskMem(caGUID.pElems);
        }
    }

    internal class NativeMethods
    {
        NativeMethods() {}

        [DllImport("olepro32.dll")]
        public static extern int OleCreatePropertyFrame(
            IntPtr hwndOwner,
            int x,
            int y,
            [MarshalAs(UnmanagedType.LPWStr)] string lpszCaption,
            int cObjects,
            [MarshalAs(UnmanagedType.Interface, ArraySubType = UnmanagedType.IUnknown)] 
            ref object ppUnk,
            int cPages,
            IntPtr lpPageClsID,
            int lcid,
            int dwReserved,
            IntPtr lpvReserved);
    }
}
