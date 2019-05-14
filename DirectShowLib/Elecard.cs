using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace FutureConcepts.Media.DirectShowLib
{
    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("486F726E-4D43-49b9-8A0C-C22A2B0524E8"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IModuleConfig : IPersistStream
    {
        [PreserveSig]
        int SetValue(
            [In] Guid pParamID,
            [In] IntPtr pValue);
        
        [PreserveSig]
        int GetValue(
            [In] Guid pParamID,
            [Out] IntPtr pValue);
        
        [PreserveSig]
        int GetParamConfig(
            [In] Guid pParamID,
            [Out, MarshalAs(UnmanagedType.Interface)] out IParamConfig pValue);
        
        [PreserveSig]
        [return: MarshalAs(UnmanagedType.U1)]
        bool IsSupported(
            [In] ref Guid pParamID);
        
        [PreserveSig]
        int SetDefState();
        
        [PreserveSig]
        int EnumParams( 
            ref long pNumParams,
            ref Guid pParamIDs);
        
        [PreserveSig]
        int CommitChanges(
           [In]IntPtr pReason);

        [PreserveSig]
        int DeclineChanges();
     
        [PreserveSig]
        int SaveToRegistry(
            [In] Int32 hKeyRoot,
            [In, MarshalAs(UnmanagedType.BStr)] String pszKeyName,
            [In, MarshalAs(UnmanagedType.Bool)] bool bPreferReadable);
        
        [PreserveSig]
        int LoadFromRegistry( 
            [In] Int32 hKeyRoot,
            [In, MarshalAs(UnmanagedType.BStr)] String pszKeyName,
            [In, MarshalAs(UnmanagedType.Bool)] bool bPreferReadable);
        
        [PreserveSig]
        int RegisterForNotifies(
            [In, MarshalAs(UnmanagedType.Interface)] ref IModuleCallback pModuleCallback);
        
        [PreserveSig]
        int UnregisterFromNotifies( 
            [In, MarshalAs(UnmanagedType.Interface)] ref IModuleCallback pModuleCallback);       
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity, 
    Guid("486F726E-4D45-49b9-8A0C-C22A2B0524E8"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IModuleCallback 
    {   
        [PreserveSig]
        int OnModuleNotify( 
            [In] long cParams,
            [In] ref Guid pParamIDs);       
    };

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("486F726E-4D43-49b9-8A0C-C22A2B0524E8"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IParamConfig
    {
        int SetValue( 
            [In] ref Object pValue,
            [In, MarshalAs(UnmanagedType.Bool)] bool bSetAndCommit);
        
        int GetValue( 
            ref Object pValue,
            [In, MarshalAs(UnmanagedType.Bool)] bool bGetCommitted);
        
        int SetVisible( 
            [MarshalAs(UnmanagedType.Bool)] bool bVisible);
        
        int GetVisible( 
            [MarshalAs(UnmanagedType.Bool)] bool bVisible);
        
        int GetParamID(
            ref Guid pParamID);
        
        int GetName(
            [MarshalAs(UnmanagedType.BStr)] ref String pName);
        
        int GetReadOnly( 
            [MarshalAs(UnmanagedType.Bool)] bool bReadOnly);
        
        int GetFullInfo( 
            ref Object pValue,
            [MarshalAs(UnmanagedType.BStr)] ref String pMeaning,
            [MarshalAs(UnmanagedType.BStr)] ref String pName,
            [MarshalAs(UnmanagedType.Bool)] ref bool bReadOnly,
            [MarshalAs(UnmanagedType.Bool)] ref bool pVisible);
        
        int GetDefValue( 
            ref Object pValue);
        
        int GetValidRange( 
            ref Object pMinValue,
            ref Object pMaxValue,
            ref Object pDelta);
        
        int EnumValidValues( 
            ref long pNumValidValues,
            ref Object pValidValues,
            [MarshalAs(UnmanagedType.BStr)] ref String pValueNames);
        
        int ValueToMeaning(
            [In] ref Object pValue,
            [MarshalAs(UnmanagedType.BStr)] ref String pMeaning);
        
        int MeaningToValue( 
            [In, MarshalAs(UnmanagedType.BStr)] String pMeaning,
            ref Object pValue);        
    }
}
