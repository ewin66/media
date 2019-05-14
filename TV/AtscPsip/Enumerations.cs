using System;
using System.Collections.Generic;
using System.Text;

namespace FutureConcepts.Media.TV.AtscPsip
{
    /// <summary>
    /// Short values that are Packet IDs
    /// </summary>
    public enum PacketID
    {
        /// <summary>
        /// PID for Program Assocation Table data
        /// </summary>
        ProgramAssocation           = 0x0000,
        /// <summary>
        /// PID for Conditional Access Table data
        /// </summary>
        ConditionalAccess           = 0x0001,
        /// <summary>
        /// PID for Transport Stream Description data
        /// </summary>
        TransportStreamDescription  = 0x0002,
        /// <summary>
        /// PID for PAT_E (Program Association Table Extended?) data
        /// </summary>
        PAT_E                       = 0x1FF7,
        /// <summary>
        /// PID for STT_PID_E (?) data
        /// </summary>
        STT_PID_E                   = 0x1FF8,
        /// <summary>
        /// PID for BASE_PID_E (?) data
        /// </summary>
        BASE_PID_E                  = 0x1FF9,
        /// <summary>
        /// PID for Ops and Management data
        /// </summary>
        OpsAndManagement            = 0x1FFA,
        /// <summary>
        /// PID for all Program System Information Protocol data
        /// </summary>
        PSIP                        = 0x1FFB,
    }

    /// <summary>
    /// Byte values that are Table IDs
    /// </summary>
    /// <remarks>
    /// See ATSC A/65C (2 January 2006) pg. 18, Table 4.2
    /// </remarks>
    public enum TableID
    {
        /// <summary>
        /// TID for Master Guide Table (MGT)
        /// </summary>
        MasterGuideTable            = 0xC7,
        /// <summary>
        /// TID for Virtual Channel Table for over-the-air transmission (TVCT)
        /// </summary>
        VCTTerrestrial              = 0xC8,
        /// <summary>
        /// TID for Virtual Channel Table for cable transmission (CVCT)
        /// </summary>
        VCTCable                    = 0xC9,
        /// <summary>
        /// TID for the Rating/Region Table (RRT)
        /// </summary>
        RatingRegion                = 0xCA,
        /// <summary>
        /// TID for Event Information Table (EIT), may be varible per the MGT
        /// </summary>
        EventInformation            = 0xCB,
        /// <summary>
        /// TID for the Channel Extended Text Table (ETT), may be variable per the MGT
        /// </summary>
        ChannelExtendedText         = 0xCC,
        /// <summary>
        /// TID for Event Extended Text Table (ETT), may be variable per the MGT
        /// </summary>
        EventExtendedText           = 0xCD,
        /// <summary>
        /// TID for Data Event Table
        /// </summary>
        DataEvent                   = 0xCE,
        /// <summary>
        /// TID for Data Service Table
        /// </summary>
        DataService                 = 0xCF,
        /// <summary>
        /// TID for Network Resources Table
        /// </summary>
        NetworkResources            = 0xD1,
        /// <summary>
        /// TID for Long Term Service Table
        /// </summary>
        LongTermService             = 0xD2,
        /// <summary>
        /// TID for Directed Channel Change Table (DCCT)
        /// </summary>
        DirectedChannelChange       = 0xD3,
        /// <summary>
        /// TID for Directed Channel Change Selection Code Table (DCCSCT)
        /// </summary>
        DCCSelectionCode            = 0xD4,
        /// <summary>
        /// TID for Aggregate Event Information Table
        /// </summary>
        AggregateEventInformation   = 0xD6,
        /// <summary>
        /// TID for Aggregate Extended Text Table
        /// </summary>
        AggregateExtendedText       = 0xD7,
        /// <summary>
        /// TID for Satellite Virtual Channel Table
        /// </summary>
        VCTSatellite                = 0xDA
    };

    /// <summary>
    /// List of Modulation Modes listed in the Virtual Channel Table
    /// </summary>
    /// <remarks>
    /// As defined in ATSC A/65C (2 January 2006) pg. 34, Table 6.5
    /// </remarks>
    public enum VCTModulationModes
    {
        /// <summary>
        /// The virtual channel is modulated using standard analog methods for analog televions.
        /// </summary>
        Analog      = 0x01,
        /// <summary>
        /// Transmitted in accordance with Digital Video Tranmission Standard for Cable Television Mode 1. Typically 64-QAM
        /// </summary>
        SCTEMode1   = 0x02,
        /// <summary>
        /// Transmitted in accordance with Digital Video Tranmission Standard for Cable Television Mode 2. Typically 256-QAM
        /// </summary>
        SCTEMode2   = 0x03,
        /// <summary>
        /// 8-VSB modulation method, defined in ATSC A/53 Annex D
        /// </summary>
        ATSC8VSB    = 0x04,
        /// <summary>
        /// 16-VSB modulation method, defined in ATSC A/53 Annex D
        /// </summary>
        ATSC16VSB   = 0x05
    };

    /// <summary>
    /// Specifies the existance/location of any Extended Text Message for a given Virtual Channel
    /// </summary>
    /// <remarks>
    /// As defined in ATSC A/65C (2 January 2006) pg. 35, Table 6.6
    /// </remarks>
    public enum VCTETMLocations
    {
        /// <summary>
        /// No Extended Text Message
        /// </summary>
        NoETM           = 0x00,
        /// <summary>
        /// Extended Text Message is in the same PTC that is carrying this PSIP information
        /// </summary>
        ETMInThisPSIP   = 0x01,
        /// <summary>
        /// Extended Text Message is in the PTC defined by the channels' TSID. See <see cref="P:VirtualChannelTable.Entry.ChannelTSID"/>.
        /// </summary>
        ETMInChannelTS  = 0x02
    };

    /// <summary>
    /// Service Types used to indcate the type of content carried on a Virtual Channel
    /// </summary>
    /// <remarks>
    /// As defined in ATSC A/65C (2 January 2006) pg. 35, Table 6.7
    /// </remarks>
    public enum VCTServiceTypes
    {
        /// <summary>
        /// The virtual channel carries analog television programming
        /// </summary>
        AnalogTelevision = 0x01,
        /// <summary>
        /// ATSC digital TV, carrying video, audio, and optionally extra data
        /// </summary>
        ATSCDigitalTelevision = 0x02,
        /// <summary>
        /// ASTC digital TV carrying only audio, and optionally extra data
        /// </summary>
        ATSCAudio = 0x03,
        /// <summary>
        /// ATSC digital TV stream, but carrying no video or audio
        /// </summary>
        ATSCDataOnlyService = 0x04
    };
}
