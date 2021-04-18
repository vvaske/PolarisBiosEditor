using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using System.Linq;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace PolarisBiosEditor
{
    public enum KIND_CONNECTOR
    {
        CONNECTOR_OBJECT_ID_NONE = 0x00,
        CONNECTOR_OBJECT_ID_SINGLE_LINK_DVI_I = 0x01,
        CONNECTOR_OBJECT_ID_DUAL_LINK_DVI_I = 0x02,
        CONNECTOR_OBJECT_ID_SINGLE_LINK_DVI_D = 0x03,
        CONNECTOR_OBJECT_ID_DUAL_LINK_DVI_D = 0x04,
        CONNECTOR_OBJECT_ID_VGA = 0x05,
        CONNECTOR_OBJECT_ID_COMPOSITE = 0x06,
        CONNECTOR_OBJECT_ID_SVIDEO = 0x07,
        CONNECTOR_OBJECT_ID_YPbPr = 0x08,
        CONNECTOR_OBJECT_ID_D_CONNECTOR = 0x09,
        CONNECTOR_OBJECT_ID_9PIN_DIN = 0x0A, /* Supports both CV & TV */
        CONNECTOR_OBJECT_ID_SCART = 0x0B,
        CONNECTOR_OBJECT_ID_HDMI_TYPE_A = 0x0C,
        CONNECTOR_OBJECT_ID_HDMI_TYPE_B = 0x0D,
        CONNECTOR_OBJECT_ID_LVDS = 0x0E,
        CONNECTOR_OBJECT_ID_7PIN_DIN = 0x0F,
        CONNECTOR_OBJECT_ID_PCIE_CONNECTOR = 0x10,
        CONNECTOR_OBJECT_ID_CROSSFIRE = 0x11,
        CONNECTOR_OBJECT_ID_HARDCODE_DVI = 0x12,
        CONNECTOR_OBJECT_ID_DISPLAYPORT = 0x13,
        CONNECTOR_OBJECT_ID_eDP = 0x14,
        CONNECTOR_OBJECT_ID_MXM = 0x15,
        CONNECTOR_OBJECT_ID_LVDS_eDP = 0x16
    }

    public enum KIND_ENCODER
    {
        ENCODER_OBJECT_ID_NONE                    =0x00, 
        
        /* Radeon Class Display Hardware */
        ENCODER_OBJECT_ID_INTERNAL_LVDS           =0x01,
        ENCODER_OBJECT_ID_INTERNAL_TMDS1          =0x02,
        ENCODER_OBJECT_ID_INTERNAL_TMDS2          =0x03,
        ENCODER_OBJECT_ID_INTERNAL_DAC1           =0x04,
        ENCODER_OBJECT_ID_INTERNAL_DAC2           =0x05,     /* TV/CV DAC */
        ENCODER_OBJECT_ID_INTERNAL_SDVOA          =0x06,
        ENCODER_OBJECT_ID_INTERNAL_SDVOB          =0x07,
        
        /* External Third Party Encoders */
        ENCODER_OBJECT_ID_SI170B                  =0x08,
        ENCODER_OBJECT_ID_CH7303                  =0x09,
        ENCODER_OBJECT_ID_CH7301                  =0x0A,
        ENCODER_OBJECT_ID_INTERNAL_DVO1           =0x0B,    /* This belongs to Radeon Class Display Hardware */
        ENCODER_OBJECT_ID_EXTERNAL_SDVOA          =0x0C,
        ENCODER_OBJECT_ID_EXTERNAL_SDVOB          =0x0D,
        ENCODER_OBJECT_ID_TITFP513                =0x0E,
        ENCODER_OBJECT_ID_INTERNAL_LVTM1          =0x0F,   /* not used for Radeon */
        ENCODER_OBJECT_ID_VT1623                  =0x10,
        ENCODER_OBJECT_ID_HDMI_SI1930             =0x11,
        ENCODER_OBJECT_ID_HDMI_INTERNAL           =0x12,
        ENCODER_OBJECT_ID_ALMOND                  =0x22,
        ENCODER_OBJECT_ID_TRAVIS                  =0x23,
        ENCODER_OBJECT_ID_NUTMEG                  =0x22,
        ENCODER_OBJECT_ID_HDMI_ANX9805            =0x26,
        
        /* Kaleidoscope (KLDSCP) Class Display Hardware (internal) */
        ENCODER_OBJECT_ID_INTERNAL_KLDSCP_TMDS1   =0x13,
        ENCODER_OBJECT_ID_INTERNAL_KLDSCP_DVO1    =0x14,
        ENCODER_OBJECT_ID_INTERNAL_KLDSCP_DAC1    =0x15,
        ENCODER_OBJECT_ID_INTERNAL_KLDSCP_DAC2    =0x16,  /* Shared with CV/TV and CRT */
        ENCODER_OBJECT_ID_SI178                   =0X17,  /* External TMDS (dual link, no HDCP.) */
        ENCODER_OBJECT_ID_MVPU_FPGA               =0x18,  /* MVPU FPGA chip */
        ENCODER_OBJECT_ID_INTERNAL_DDI            =0x19,
        ENCODER_OBJECT_ID_VT1625                  =0x1A,
        ENCODER_OBJECT_ID_HDMI_SI1932             =0x1B,
        ENCODER_OBJECT_ID_DP_AN9801               =0x1C,
        ENCODER_OBJECT_ID_DP_DP501                =0x1D,
        ENCODER_OBJECT_ID_INTERNAL_UNIPHY         =0x1E,
        ENCODER_OBJECT_ID_INTERNAL_KLDSCP_LVTMA   =0x1F,
        ENCODER_OBJECT_ID_INTERNAL_UNIPHY1        =0x20,
        ENCODER_OBJECT_ID_INTERNAL_UNIPHY2        =0x21,
        ENCODER_OBJECT_ID_INTERNAL_VCE            =0x24,
        ENCODER_OBJECT_ID_INTERNAL_UNIPHY3        =0x25,
        ENCODER_OBJECT_ID_INTERNAL_AMCLK          =0x27,
        
        ENCODER_OBJECT_ID_GENERAL_EXTERNAL_DVO    =0xFF,
    }

    public partial class PolarisBiosEditor : Form
    {

        /* DATA */

        string version = "1.7xml";
        string programTitle = "PolarisBiosEditor";


        string[] manufacturers = new string[]
        {
            "SAMSUNG",
            "ELPIDA",
            "HYNIX",
            "MICRON"
        };

        string[] supportedID = new string[] { "67DF", "67EF", "1002", "67FF", "699F" };

        string[] timings = new string[]
        {
    	
        // UberMix 3.1
		 "777000000000000022CC1C00AD615C41C0590E152ECCA60B006007000B031420FA8900A00300000010122F3FBA354019", // new, please test
    	//"777000000000000022CC1C00AD615C41C0590E152ECC8608006007000B031420FA8900A00300000010122F3FBA354019", //old
		
	    // FIXME Try UberMix 3.2 Timing
    	 "777000000000000022CC1C00CEE55C46C0590E1532CD66090060070014051420FA8900A00300000012123442C3353C19",
		
		// Good HYNIX_3 BY VASKE
         "999000000000000022CC1C00ADDD5B44A0551315B74C450A00400600750414206A8900A00200312010112D34C5303F17",
        
        // Good HYNIX_2
         "777000000000000022AA1C00B56A6D46C0551017BE8E060C006AE6000C081420EA8900AB030000001B162C31C0313F17",
        
        // Good Micron
    	//"777000000000000022AA1C0073626C41B0551016BA0D260B006AE60004061420EA8940AA030000001914292EB22E3B16", old
         "777000000000000022AA1C0073626C41B0551016BA0D260B0060060004061420EA8940AA030000001914292EB22E3B16", // new tested timings (much better xmr performance @ rx560 sapphire pulse)
         "777000000000000022AA1C00B56A6D4690551014BE8E060C0060060074081420EA8900AB020000001B162C31C02E3F15",// new Micron timing it's actually from Samsung
        
    	// Good Hynix_1
    	 "999000000000000022559D0010DE5B4480551312B74C450A00400600750414206A8900A00200312010112D34A42A3816",
        
    	// Good Elpida (fixed by VASKE)
    	 "777000000000000022AA1C00EF595B36A0550F15B68C1506004082007C041420CA8980A9020004C01712262B612B3715",
        //"777000000000000022AA1C00AC615B3CA0550F142C8C1506006004007C041420CA8980A9020004C01712262B612B3715" // new, please test

        // Universal Hynix
         "777000000000000022AA1C00B56A6D46C0551017BE8E060C006006000C081420EA8900AB030000001B162C31C0313F17",
         
         //Hynix 4 by vaske
         "999000000000000022559D0031626C46905F1015BC0D060C004004007D0714204A8900A0020071241B12312CC02D3C17", //new, please test

         //Samsung K4G80325FC // let's call it samsung4 pro timing
         "777000000000000022CC1C00106A5D4DD0571016B90D060C0060070014051420FA8900A0030000001011333DC0303A17", //new, please test

         //Samsung K4G80325FC // let's call it samsung4 basic timing
         "777000000000000022CC1C00106A6D4DD0571016B90D060C0060070014051420FA8900A0030000001B11333DC0303A17" //new, please test
        };

        Dictionary<string, string> rc = new Dictionary<string, string>();

        [StructLayout(LayoutKind.Explicit, Size = 96, CharSet = CharSet.Ansi)]
        public class VRAM_TIMING_RX
        {

        }

        Byte[] buffer;
        Int32Converter int32 = new Int32Converter();
        UInt32Converter uint32 = new UInt32Converter();

        int atom_rom_checksum_offset = 0x21;
        int atom_rom_header_ptr = 0x48;
        int atom_rom_header_offset;
        ATOM_ROM_HEADER atom_rom_header;
        ATOM_DATA_TABLES atom_data_table;

        int atom_powerplay_offset;
        ATOM_POWERPLAY_TABLE atom_powerplay_table;

        int atom_powertune_offset;
        ATOM_Polaris_PowerTune_Table atom_powertune_table;

        int atom_fan_offset;
        ATOM_FAN_TABLE atom_fan_table;

        int atom_mclk_table_offset;
        ATOM_MCLK_TABLE atom_mclk_table;
        ATOM_MCLK_ENTRY[] atom_mclk_entries;

        int atom_sclk_table_offset;
        ATOM_SCLK_TABLE atom_sclk_table;
        ATOM_SCLK_ENTRY[] atom_sclk_entries;

        int atom_vddc_table_offset;
        ATOM_VOLTAGE_TABLE atom_vddc_table;
        ATOM_VOLTAGE_ENTRY[] atom_vddc_entries;

        int atom_vram_info_offset;
        ATOM_VRAM_INFO atom_vram_info;
        ATOM_VRAM_ENTRY[] atom_vram_entries;
        ATOM_VRAM_TIMING_ENTRY[] atom_vram_timing_entries;
        int atom_vram_index = 0;
        const int MAX_VRAM_ENTRIES = 48; // e.g. MSI-Armor-RX-580-4GB has 36 entries
        int atom_vram_timing_offset;

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ATOM_COMMON_TABLE_HEADER
        {
            public Int16 usStructureSize;
            public Byte ucTableFormatRevision;
            public Byte ucTableContentRevision;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ATOM_ROM_HEADER
        {
            public ATOM_COMMON_TABLE_HEADER sHeader;
            //public UInt32 uaFirmWareSignature;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x4)]
            public Char[] uaFirmWareSignature;
            public UInt16 usBiosRuntimeSegmentAddress;
            public UInt16 usProtectedModeInfoOffset;
            public UInt16 usConfigFilenameOffset;
            public UInt16 usCRC_BlockOffset;
            public UInt16 usBIOS_BootupMessageOffset;
            public UInt16 usInt10Offset;
            public UInt16 usPciBusDevInitCode;
            public UInt16 usIoBaseAddress;
            public UInt16 usSubsystemVendorID;
            public UInt16 usSubsystemID;
            public UInt16 usPCI_InfoOffset;
            public UInt16 usMasterCommandTableOffset;
            public UInt16 usMasterDataTableOffset;
            public Byte ucExtendedFunctionCode;
            public Byte ucReserved;
            public UInt32 ulPSPDirTableOffset;
            public UInt16 usDeviceID;
            public UInt16 usVendorID;
        }

        String BIOS_BootupMessage;

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ATOM_DATA_TABLES
        {
            public ATOM_COMMON_TABLE_HEADER sHeader;
            public UInt16 UtilityPipeLine;
            public UInt16 MultimediaCapabilityInfo;
            public UInt16 MultimediaConfigInfo;
            public UInt16 StandardVESA_Timing;
            public UInt16 FirmwareInfo;
            public UInt16 PaletteData;
            public UInt16 LCD_Info;
            public UInt16 DIGTransmitterInfo;
            public UInt16 SMU_Info;
            public UInt16 SupportedDevicesInfo;
            public UInt16 GPIO_I2C_Info;
            public UInt16 VRAM_UsageByFirmware;
            public UInt16 GPIO_Pin_LUT;
            public UInt16 VESA_ToInternalModeLUT;
            public UInt16 GFX_Info;
            public UInt16 PowerPlayInfo;
            public UInt16 GPUVirtualizationInfo;
            public UInt16 SaveRestoreInfo;
            public UInt16 PPLL_SS_Info;
            public UInt16 OemInfo;
            public UInt16 XTMDS_Info;
            public UInt16 MclkSS_Info;
            public UInt16 Object_Header;
            public UInt16 IndirectIOAccess;
            public UInt16 MC_InitParameter;
            public UInt16 ASIC_VDDC_Info;
            public UInt16 ASIC_InternalSS_Info;
            public UInt16 TV_VideoMode;
            public UInt16 VRAM_Info;
            public UInt16 MemoryTrainingInfo;
            public UInt16 IntegratedSystemInfo;
            public UInt16 ASIC_ProfilingInfo;
            public UInt16 VoltageObjectInfo;
            public UInt16 PowerSourceInfo;
            public UInt16 ServiceInfo;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        unsafe public struct ATOM_POWERPLAY_TABLE
        {
            public ATOM_COMMON_TABLE_HEADER sHeader;
            public Byte ucTableRevision;
            public UInt16 usTableSize;
            public UInt32 ulGoldenPPID;
            public UInt32 ulGoldenRevision;
            public UInt16 usFormatID;
            public UInt16 usVoltageTime;
            public UInt32 ulPlatformCaps;
            public UInt32 ulMaxODEngineClock;
            public UInt32 ulMaxODMemoryClock;
            public UInt16 usPowerControlLimit;
            public UInt16 usUlvVoltageOffset;
            public UInt16 usStateArrayOffset;
            public UInt16 usFanTableOffset;
            public UInt16 usThermalControllerOffset;
            public UInt16 usReserv;
            public UInt16 usMclkDependencyTableOffset;
            public UInt16 usSclkDependencyTableOffset;
            public UInt16 usVddcLookupTableOffset;
            public UInt16 usVddgfxLookupTableOffset;
            public UInt16 usMMDependencyTableOffset;
            public UInt16 usVCEStateTableOffset;
            public UInt16 usPPMTableOffset;
            public UInt16 usPowerTuneTableOffset;
            public UInt16 usHardLimitTableOffset;
            public UInt16 usPCIETableOffset;
            public UInt16 usGPIOTableOffset;
            [XmlIgnore] public fixed UInt16 usReserved[6];
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ATOM_MCLK_ENTRY
        {
            public Byte ucVddcInd;
            public UInt16 usVddci;
            public UInt16 usVddgfxOffset;
            public UInt16 usMvdd;
            public UInt32 ulMclk;
            public UInt16 usReserved;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ATOM_MCLK_TABLE
        {
            public Byte ucRevId;
            public Byte ucNumEntries;
            // public unsafe fixed byte ATOM_MCLK_ENTRY entries[ucNumEntries]; [3]
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ATOM_SCLK_ENTRY
        {
            public Byte ucVddInd;
            public UInt16 usVddcOffset;
            public UInt32 ulSclk;
            public UInt16 usEdcCurrent;
            public Byte ucReliabilityTemperature;
            public Byte ucCKSVOffsetandDisable;
            public UInt32 ulSclkOffset;
            // Polaris Only, remove for compatibility with Fiji
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ATOM_SCLK_TABLE
        {
            public Byte ucRevId;
            public Byte ucNumEntries;
            // public unsafe fixed byte ATOM_SCLK_ENTRY entries[ucNumEntries]; [8]
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ATOM_VOLTAGE_ENTRY
        {
            public UInt16 usVdd;
            public UInt16 usCACLow;
            public UInt16 usCACMid;
            public UInt16 usCACHigh;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ATOM_VOLTAGE_TABLE
        {
            public Byte ucRevId;
            public Byte ucNumEntries;
            // public unsafe fixed byte ATOM_VOLTAGE_ENTRY entries[ucNumEntries]; [8]
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ATOM_FAN_TABLE
        {
            public Byte ucRevId;
            public Byte ucTHyst;
            public UInt16 usTMin;
            public UInt16 usTMed;
            public UInt16 usTHigh;
            public UInt16 usPWMMin;
            public UInt16 usPWMMed;
            public UInt16 usPWMHigh;
            public UInt16 usTMax;
            public Byte ucFanControlMode;
            public UInt16 usFanPWMMax;
            public UInt16 usFanOutputSensitivity;
            public UInt16 usFanRPMMax;
            public UInt32 ulMinFanSCLKAcousticLimit;
            public Byte ucTargetTemperature;
            public Byte ucMinimumPWMLimit;
            public UInt16 usFanGainEdge;
            public UInt16 usFanGainHotspot;
            public UInt16 usFanGainLiquid;
            public UInt16 usFanGainVrVddc;
            public UInt16 usFanGainVrMvdd;
            public UInt16 usFanGainPlx;
            public UInt16 usFanGainHbm;
            public UInt16 usReserved;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ATOM_Polaris_PowerTune_Table
        {
            public Byte ucRevId;
            public UInt16 usTDP;
            public UInt16 usConfigurableTDP;
            public UInt16 usTDC;
            public UInt16 usBatteryPowerLimit;
            public UInt16 usSmallPowerLimit;
            public UInt16 usLowCACLeakage;
            public UInt16 usHighCACLeakage;
            public UInt16 usMaximumPowerDeliveryLimit;
            public UInt16 usTjMax;
            public UInt16 usPowerTuneDataSetID;
            public UInt16 usEDCLimit;
            public UInt16 usSoftwareShutdownTemp;
            public UInt16 usClockStretchAmount;
            public UInt16 usTemperatureLimitHotspot;
            public UInt16 usTemperatureLimitLiquid1;
            public UInt16 usTemperatureLimitLiquid2;
            public UInt16 usTemperatureLimitVrVddc;
            public UInt16 usTemperatureLimitVrMvdd;
            public UInt16 usTemperatureLimitPlx;
            public Byte ucLiquid1_I2C_address;
            public Byte ucLiquid2_I2C_address;
            public Byte ucLiquid_I2C_Line;
            public Byte ucVr_I2C_address;
            public Byte ucVr_I2C_Line;
            public Byte ucPlx_I2C_address;
            public Byte ucPlx_I2C_Line;
            public UInt16 usBoostPowerLimit;
            public Byte ucCKS_LDO_REFSEL;
            public Byte ucHotSpotOnly;
            public Byte ucReserve;
            public UInt16 usReserve;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ATOM_OBJECT_HEADER_V3
        {
            public ATOM_COMMON_TABLE_HEADER sHeader;
            public UInt16 usDeviceSupport;
            public UInt16 usConnectorObjectTableOffset;
            public UInt16 usRouterObjectTableOffset;
            public UInt16 usEncoderObjectTableOffset;
            public UInt16 usProtectionObjectTableOffset; //only available when Protection block is independent.
            public UInt16 usDisplayPathTableOffset;
            public UInt16 usMiscObjectTableOffset;
        }


        public enum GRAPH_OBJECT_TYPE
        {
            GRAPH_OBJECT_TYPE_NONE = 0x0,
            GRAPH_OBJECT_TYPE_GPU = 0x1,
            GRAPH_OBJECT_TYPE_ENCODER = 0x2,
            GRAPH_OBJECT_TYPE_CONNECTOR = 0x3,
            GRAPH_OBJECT_TYPE_ROUTER = 0x4,
            /* deleted */
            GRAPH_OBJECT_TYPE_DISPLAY_PATH = 0x6,
            GRAPH_OBJECT_TYPE_GENERIC = 0x7,
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ATOM_OBJECT_ID
        {
            [XmlIgnore] public Byte KindInNamespaceRaw;
            [XmlIgnore] public Byte NamespaceAndIndex;
            public string KindInNamespace
            {
                get
                {
                    var type_name = Namespace.ToString().Split(new []{'_'}).Last();
                    var type = Type.GetType("PolarisBiosEditor.KIND_" + type_name);
                    if (type != null)
                    {
                        return Enum.ToObject(type, KindInNamespaceRaw).ToString() + " = 0x" + KindInNamespaceRaw.ToString("X");
                    }
                    return KindInNamespaceRaw.ToString();
                }
                set { throw new NotImplementedException(); }
            }
            public GRAPH_OBJECT_TYPE Namespace
            {
                get { return (GRAPH_OBJECT_TYPE)(NamespaceAndIndex >> 4); }
                set { throw new NotImplementedException(); }
            }
            public int Index
            {
                get { return NamespaceAndIndex & 7; }
                set { throw new NotImplementedException(); }
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ATOM_DISPLAY_OBJECT_PATH
        {
            public UInt16 usDeviceTag;                                   //supported device
            public UInt16 usSize;                                        //the size of ATOM_DISPLAY_OBJECT_PATH
            public ATOM_OBJECT_ID usConnObjectId;                                //Connector Object ID
            public ATOM_OBJECT_ID usGPUObjectId;                                 //GPU ID
            public ATOM_OBJECT_ID usGraphicObjIdsFirst;                          //1st Encoder Obj source from GPU to last Graphic Obj destinate to connector.
            //usGraphicObjIdsOthers
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ATOM_DISPLAY_OBJECT_PATH_TABLE
        {
            public Byte ucNumOfDispPath;
            public Byte ucVersion;
            public UInt16 ucPadding2;
            //ATOM_DISPLAY_OBJECT_PATH asDispPath[1];
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ATOM_OBJECT                                //each object has this structure
        {
            public ATOM_OBJECT_ID usObjectID;
            public UInt16 usSrcDstTableOffset;
            public UInt16 usRecordOffset;                     //this pointing to a bunch of records defined below
            public UInt16 usReserved;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ATOM_OBJECT_TABLE
        {
            public Byte ucNumberOfObjects;
            public Byte ucPadding0;
            public Byte ucPadding1;
            public Byte ucPadding2;
            //ATOM_OBJECT asObjects[1];
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ATOM_COMMON_RECORD_HEADER
        {
            [XmlIgnore] public Byte ucRecordType;                      //An emun to indicate the record type
            public string RecordType
            {
                get { return ((AtomRecordType)ucRecordType).ToString() + " = 0x" + ucRecordType.ToString("X"); }
                set { throw new NotImplementedException(); }
            }
            public Byte ucRecordSize;                      //The size of the whole record in byte
        }


        public enum AtomRecordType {
            ATOM_I2C_RECORD_TYPE = 1,
            ATOM_HPD_INT_RECORD_TYPE = 2,
            ATOM_OUTPUT_PROTECTION_RECORD_TYPE = 3,
            ATOM_CONNECTOR_DEVICE_TAG_RECORD_TYPE = 4,
            ATOM_CONNECTOR_DVI_EXT_INPUT_RECORD_TYPE = 5, //Obsolete, switch to use GPIO_CNTL_RECORD_TYPE
            ATOM_ENCODER_FPGA_CONTROL_RECORD_TYPE = 6, //Obsolete, switch to use GPIO_CNTL_RECORD_TYPE
            ATOM_CONNECTOR_CVTV_SHARE_DIN_RECORD_TYPE = 7,
            ATOM_JTAG_RECORD_TYPE = 8, //Obsolete, switch to use GPIO_CNTL_RECORD_TYPE
            ATOM_OBJECT_GPIO_CNTL_RECORD_TYPE = 9,
            ATOM_ENCODER_DVO_CF_RECORD_TYPE = 10,
            ATOM_CONNECTOR_CF_RECORD_TYPE = 11,
            ATOM_CONNECTOR_HARDCODE_DTD_RECORD_TYPE = 12,
            ATOM_CONNECTOR_PCIE_SUBCONNECTOR_RECORD_TYPE = 13,
            ATOM_ROUTER_DDC_PATH_SELECT_RECORD_TYPE = 14,
            ATOM_ROUTER_DATA_CLOCK_PATH_SELECT_RECORD_TYPE = 15,
            ATOM_CONNECTOR_HPDPIN_LUT_RECORD_TYPE = 16, //This is for the case when connectors are not known to object table
            ATOM_CONNECTOR_AUXDDC_LUT_RECORD_TYPE = 17, //This is for the case when connectors are not known to object table
            ATOM_OBJECT_LINK_RECORD_TYPE = 18, //Once this record is present under one object, it indicats the oobject is linked to another obj described by the record
            ATOM_CONNECTOR_REMOTE_CAP_RECORD_TYPE = 19,
            ATOM_ENCODER_CAP_RECORD_TYPE = 20,
            ATOM_BRACKET_LAYOUT_RECORD_TYPE = 21,
            ATOM_CONNECTOR_FORCED_TMDS_CAP_RECORD_TYPE = 22,
        }

        public enum atom_voltage_type: Byte
        {
            VOLTAGE_TYPE_VDDC_0x1 = 1,
            VOLTAGE_TYPE_MVDDC_0x2 = 2,
            VOLTAGE_TYPE_MVDDQ_0x3 = 3,
            VOLTAGE_TYPE_VDDCI_0x4 = 4,
            VOLTAGE_TYPE_VDDGFX_0x5 = 5,
            VOLTAGE_TYPE_PCC_0x6 = 6,
            VOLTAGE_TYPE_MVPP_0x7 = 7,
            VOLTAGE_TYPE_LEDDPM_0x8 = 8,
            VOLTAGE_TYPE_PCC_MVDD_0x9 = 9,
            VOLTAGE_TYPE_PCIE_VDDC_0xA = 10,
            VOLTAGE_TYPE_PCIE_VDDR_0xB = 11,
            VOLTAGE_TYPE_GENERIC_I2C_1_0x11 = 0x11,
            VOLTAGE_TYPE_GENERIC_I2C_2_0x12 = 0x12,
            VOLTAGE_TYPE_GENERIC_I2C_3_0x13 = 0x13,
            VOLTAGE_TYPE_GENERIC_I2C_4_0x14 = 0x14,
            VOLTAGE_TYPE_GENERIC_I2C_5_0x15 = 0x15,
            VOLTAGE_TYPE_GENERIC_I2C_6_0x16 = 0x16,
            VOLTAGE_TYPE_GENERIC_I2C_7_0x17 = 0x17,
            VOLTAGE_TYPE_GENERIC_I2C_8_0x18 = 0x18,
            VOLTAGE_TYPE_GENERIC_I2C_9_0x19 = 0x19,
            VOLTAGE_TYPE_GENERIC_I2C_10_0x1A = 0x1A,
        };

        public enum atom_voltage_object_mode: Byte
        { 
            VOLTAGE_OBJ_GPIO_LUT_0x0                 = 0,        //VOLTAGE and GPIO Lookup table ->ATOM_GPIO_VOLTAGE_OBJECT_V3
            VOLTAGE_OBJ_VR_I2C_INIT_SEQ_0x3          = 3,        //VOLTAGE REGULATOR INIT sequece through I2C -> ATOM_I2C_VOLTAGE_OBJECT_V3
            VOLTAGE_OBJ_PHASE_LUT_0x4                = 4,        //Set Vregulator Phase lookup table ->ATOM_GPIO_VOLTAGE_OBJECT_V3
            VOLTAGE_OBJ_SVID2_0x7                    = 7,        //Indicate voltage control by SVID2 ->ATOM_SVID2_VOLTAGE_OBJECT_V3
            VOLTAGE_OBJ_EVV_0x8                      = 8,
            VOLTAGE_OBJ_MERGED_POWER_0x9             = 9,
            VOLTAGE_OBJ_PWRBOOST_LEAKAGE_LUT_0x10    = 0x10,     //Powerboost Voltage and LeakageId lookup table->ATOM_LEAKAGE_VOLTAGE_OBJECT_V3
            VOLTAGE_OBJ_HIGH_STATE_LEAKAGE_LUT_0x11  = 0x11,     //High voltage state Voltage and LeakageId lookup table->ATOM_LEAKAGE_VOLTAGE_OBJECT_V3
            VOLTAGE_OBJ_HIGH1_STATE_LEAKAGE_LUT_0x12 = 0x12,     //High1 voltage state Voltage and LeakageId lookup table->ATOM_LEAKAGE_VOLTAGE_OBJECT_V3
        }

        enum atom_gpio_pin_assignment_gpio_id
        {
            I2C_HW_LANE_MUX_0x0F = 0x0f, /* only valid when bit7=1 */
            I2C_HW_ENGINE_ID_MASK_0x70 = 0x70, /* only valid when bit7=1 */
            I2C_HW_CAP_0x80 = 0x80, /*only when the I2C_HW_CAP is set, the pin ID is assigned to an I2C pin pair, otherwise, it's an generic GPIO pin */

            /* gpio_id pre-define id for multiple usage */
            /* GPIO use to control PCIE_VDDC in certain SLT board */
            PCIE_VDDC_CONTROL_GPIO_PINID_0x38 = 56,
            /* if PP_AC_DC_SWITCH_GPIO_PINID in Gpio_Pin_LutTable, AC/DC swithing feature is enable */
            PP_AC_DC_SWITCH_GPIO_PINID_0x3C = 60,
            /* VDDC_REGULATOR_VRHOT_GPIO_PINID in Gpio_Pin_LutTable, VRHot feature is enable */
            VDDC_VRHOT_GPIO_PINID_0x3D = 61,
            /*if VDDC_PCC_GPIO_PINID in GPIO_LUTable, Peak Current Control feature is enabled */
            VDDC_PCC_GPIO_PINID_0x3E = 62,
            /* Only used on certain SLT/PA board to allow utility to cut Efuse. */
            EFUSE_CUT_ENABLE_GPIO_PINID_0x3F = 63,
            /* ucGPIO=DRAM_SELF_REFRESH_GPIO_PIND uses  for memory self refresh (ucGPIO=0, DRAM self-refresh; ucGPIO= */
            DRAM_SELF_REFRESH_GPIO_PINID_0x40 = 64,
            /* Thermal interrupt output->system thermal chip GPIO pin */
            THERMAL_INT_OUTPUT_GPIO_PINID_0x41 = 65,
            is_mm_i2c = 0xA0,
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ATOM_GPIO_ROLE_I2C_ID
        {
            public Byte gpio_id;

            public bool bfHW_Capable
            {
                get { return (gpio_id & (Byte)atom_gpio_pin_assignment_gpio_id.I2C_HW_CAP_0x80) != 0; }
                set { throw new NotImplementedException(); }
            }
            public int bfHW_EngineID
            {
                get { return (gpio_id & (Byte)atom_gpio_pin_assignment_gpio_id.I2C_HW_ENGINE_ID_MASK_0x70) >> 4; }
                set { throw new NotImplementedException(); }
            }
            public int bfI2C_LineMux
            {
                get { return gpio_id & (Byte)atom_gpio_pin_assignment_gpio_id.I2C_HW_LANE_MUX_0x0F; }
                set { throw new NotImplementedException(); }
            }
            public string Description
            {
                get { return ((atom_gpio_pin_assignment_gpio_id)gpio_id).ToString(); }
                set { throw new NotImplementedException(); }
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ATOM_I2C_SHIFTED_SLAVE_ADDRESS
        {
            public Byte shifted_i2c_slave_addr;
            /* //Not sure if it actually shifted...
            public int EffectiveSlaveAddr
            {
                get { return shifted_i2c_slave_addr >> 1; }
                set { throw new NotImplementedException(); }
            }
            */
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct atom_i2c_voltage_object_v4_fields
        {
            public Byte regulator_id;                        //Indicate Voltage Regulator Id
            public ATOM_GPIO_ROLE_I2C_ID i2c_id;
            public ATOM_I2C_SHIFTED_SLAVE_ADDRESS i2c_slave_addr;
            public Byte i2c_control_offset;
            public Byte i2c_flag;                            // Bit0: 0 - One byte data; 1 - Two byte data
            public Byte i2c_speed;                           // =0, use default i2c speed, otherwise use it in unit of kHz. 
            public Byte reserved_0xA;
            public Byte reserved_0xB;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct FINAL_atom_i2c_data_entry
        {
            public UInt16 final_entry_index;
            public static UInt16 ENDING_INDEX_VALUE => 0xff;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct atom_i2c_data_entry
        {
            public UInt16 i2c_reg_index;               // i2c register address, can be up to 16bit
            public UInt16 i2c_reg_data;                // i2c register data, can be up to 16bit
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct atom_voltage_object_header_v4
        {
            public atom_voltage_type ucVoltageType;                            //Indicate Voltage Source: VDDC, MVDDC, MVDDQ or MVDDCI
            public atom_voltage_object_mode ucVoltageMode;                            //Indicate voltage control mode: Init/Set/Leakage/Set phase
            public UInt16 usSize;                                   //Size of Object

            public atom_i2c_voltage_object_v4_fields AsI2c;
            [XmlIgnore]
            public bool AsI2cSpecified => ucVoltageMode == atom_voltage_object_mode.VOLTAGE_OBJ_VR_I2C_INIT_SEQ_0x3;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ATOM_VOLTAGE_OBJECT_INFO_V3_1
        {
            public ATOM_COMMON_TABLE_HEADER sHeader;
            //ATOM_VOLTAGE_OBJECT_V3 asVoltageObj[3];   //Info for Voltage control
        }
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ATOM_GPIO_I2C_ASSIGMENT
        {
          public UInt16                    usClkMaskRegisterIndex;
          public UInt16                    usClkEnRegisterIndex;
          public UInt16                    usClkY_RegisterIndex;
          public UInt16                    usClkA_RegisterIndex;
          public UInt16                    usDataMaskRegisterIndex;
          public UInt16                    usDataEnRegisterIndex;
          public UInt16                    usDataY_RegisterIndex;
          public UInt16                    usDataA_RegisterIndex;
          public ATOM_GPIO_ROLE_I2C_ID    sucI2cId;
          public Byte                     ucClkMaskShift;
          public Byte                     ucClkEnShift;
          public Byte                     ucClkY_Shift;
          public Byte                     ucClkA_Shift;
          public Byte                     ucDataMaskShift;
          public Byte                     ucDataEnShift;
          public Byte                     ucDataY_Shift;
          public Byte                     ucDataA_Shift;
          public Byte                     ucReserved1;
          public Byte                     ucReserved2;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ATOM_GPIO_I2C_INFO
        {
          public ATOM_COMMON_TABLE_HEADER   sHeader;
          //ATOM_GPIO_I2C_ASSIGMENT   asGPIO_Info[16];
        };


        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ATOM_VRAM_TIMING_ENTRY
        {
            public UInt32 ulClkRange;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x30)]
            [XmlIgnore]public Byte[] ucLatency;
            public string LatencyString
            {
                get
                {
                    return ByteArrayToString(ucLatency);
                }
                set
                {
                    throw new NotImplementedException();
                }
            }

        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ATOM_VRAM_ENTRY
        {
            public UInt32 ulChannelMapCfg;
            public UInt16 usModuleSize;
            public UInt16 usMcRamCfg;
            public UInt16 usEnableChannels;
            public Byte ucExtMemoryID;
            public Byte ucMemoryType;
            public Byte ucChannelNum;
            public Byte ucChannelWidth;
            public Byte ucDensity;
            public Byte ucBankCol;
            public Byte ucMisc;
            public Byte ucVREFI;
            public UInt16 usReserved;
            public UInt16 usMemorySize;
            public Byte ucMcTunningSetId;
            public Byte ucRowNum;
            public UInt16 usEMRS2Value;
            public UInt16 usEMRS3Value;
            public Byte ucMemoryVenderID;
            public Byte ucRefreshRateFactor;
            public Byte ucFIFODepth;
            public Byte ucCDR_Bandwidth;
            public UInt32 ulChannelMapCfg1;
            public UInt32 ulBankMapCfg;
            public UInt32 ulReserved;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
            [XmlIgnore]
            public Byte[] strMemPNString;
            public string FullName
            {
                get
                {
                    return Encoding.UTF8.GetString(strMemPNString.TakeWhile(c => c != 0).ToArray());
                }
                set
                {
                    throw new NotImplementedException();
                }
            }
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ATOM_VRAM_INFO
        {
            public ATOM_COMMON_TABLE_HEADER sHeader;
            public UInt16 usMemAdjustTblOffset;
            public UInt16 usMemClkPatchTblOffset;
            public UInt16 usMcAdjustPerTileTblOffset;
            public UInt16 usMcPhyInitTableOffset;
            public UInt16 usDramDataRemapTblOffset;
            public UInt16 usReserved1;
            public Byte ucNumOfVRAMModule;
            public Byte ucMemoryClkPatchTblVer;
            public Byte ucVramModuleVer;
            public Byte ucMcPhyTileNum;
            // public ATOM_VRAM_ENTRY aVramInfo[ucNumOfVRAMModule];
        }

        class ConsecutiveReader<T>
        {
            public static ConsecutiveReader<T> From<TOther>(ConsecutiveReader<TOther> other)
            {
                return new ConsecutiveReader<T>(other.buffer.Array, other.buffer.Offset, other.editor);
            }
            
            public ConsecutiveReader(Byte[] entire_buffer, int offset, PolarisBiosEditor a_editor)
            {
                buffer = new ArraySegment<byte>(entire_buffer);
                editor = a_editor;
                Jump(offset);
            }
            public T Read()
            {
                T obj = default(T);
                int size = Marshal.SizeOf(obj);
                IntPtr ptr = Marshal.AllocHGlobal(size);

                Marshal.Copy(buffer.Array, buffer.Offset, ptr, size);
                obj = (T)Marshal.PtrToStructure(ptr, obj.GetType());
                Marshal.FreeHGlobal(ptr);

                return obj;
            }
            public T ReadPrint()
            {
                T result = Read();
                int size = Marshal.SizeOf(typeof(T));
                editor.Print(result, "of", string.Format("0x{0:X}-0x{1:X}  len=0x{2:X}={2}", buffer.Offset, buffer.Offset + size, size));
                return result;
            }
            public void Jump1Structure()
            {
                Jump(Marshal.SizeOf<T>());
            }

            public void Jump(int relative_offset)
            {
                int offset = buffer.Offset + relative_offset;
                buffer = new ArraySegment<byte>(buffer.Array, offset, buffer.Array.Length - offset);
            }
            public void JumpPrintExtra(int relative_offset)
            {
                int done_size = Marshal.SizeOf<T>();
                if (done_size < relative_offset)
                {
                    editor.Print(string.Format("Extra at 0x{0:x}:", buffer.Offset + done_size) + ByteArrayToString(buffer.Skip(done_size).Take(relative_offset - done_size).ToArray()));
                }
                Jump(relative_offset);
            }
            public ArraySegment<Byte> buffer;
            public PolarisBiosEditor editor;
        }
        [STAThread]
        static void Main(string[] args)
        {
            PolarisBiosEditor pbe = new PolarisBiosEditor();
            Application.Run(pbe);
        }

        static byte[] getBytes(object obj)
        {
            int size = Marshal.SizeOf(obj);
            byte[] arr = new byte[size];
            IntPtr ptr = Marshal.AllocHGlobal(size);

            Marshal.StructureToPtr(obj, ptr, true);
            Marshal.Copy(ptr, arr, 0, size);
            Marshal.FreeHGlobal(ptr);

            return arr;
        }

        T fromBytes<T>(byte[] arr)
        {
            return new ConsecutiveReader<T>(arr, 0, this).Read();
        }

        public void setBytesAtPosition(byte[] dest, int ptr, byte[] src)
        {
            for (var i = 0; i < src.Length; i++)
            {
                dest[ptr + i] = src[i];
            }
        }

        public void Print(object output, string desc_name = "", string desc = "")
        {
            try
            {
                if (output.GetType().IsPrimitive || output.GetType() == typeof(string))
                {
                    var s = output.ToString();
                    if (!string.IsNullOrWhiteSpace(desc_name))
                    {
                        s = desc_name + "=" + desc +" "+ s;
                    }
                    Console.WriteLine(s);
                }
                else
                {
                    //Create our own namespaces for the output
                    var ns = new XmlSerializerNamespaces();
                    var xs = new XmlSerializer(output.GetType());

                    //Add an empty namespace and empty value
                    ns.Add("", "");
                    
                    var doc = new XDocument();
                    using (var writer = doc.CreateWriter())
                    {
                        xs.Serialize(writer, output, ns);
                    }
                    foreach(var e in doc.Descendants())
                    {
                        if (e.Elements().Any()) continue;
                        if (Int64.TryParse(e.Value, out Int64 parsed) && parsed != 0 && parsed != 1)
                        {
                            e.Value = string.Format("{0} = 0x{0:X} = 0b{1}", parsed, Convert.ToString(parsed, 2));
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(desc))
                    {
                        doc.Root.Add(new XAttribute(desc_name, desc));
                    }
                    Console.WriteLine(doc);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private ListViewItem handler;

        private void listView_ChangeSelection(object sender, EventArgs e)
        {
            ListView lb = sender as ListView;
            String sel_name = lb.SelectedItems[0].Text;

            for (var i = 0; i < lb.Items.Count; i++)
            {

                ListViewItem container = lb.Items[i];
                var name = container.Text;
                var value = container.SubItems[1].Text;

                if (name == sel_name)
                {
                    editSubItem1.Text = name;
                    editSubItem2.Text = value;
                    handler = container;
                }

            }
        }

        private void apply_Click(object sender, EventArgs e)
        {
            if (handler != null)
            {
                handler.Text = editSubItem1.Text;
                handler.SubItems[1].Text = editSubItem2.Text;
            }
        }

        public PolarisBiosEditor()
        {
            InitializeComponent();
            this.Text = this.programTitle + " " + this.version;

#if !DEBUG
            try
            {

                WebClient myWebClient = new WebClient();
                Stream myStream = myWebClient.OpenRead("https://raw.githubusercontent.com/vvaske/PolarisBiosEditor/master/version");
                StreamReader sr = new StreamReader(myStream);
                string newVersion = sr.ReadToEnd().Trim();
                if (!newVersion.Equals(version)) {
                    MessageBox.Show("There is a new version available! " + version + " -> " + newVersion);
                }
                myStream.Close();

                myStream = myWebClient.OpenRead("https://raw.githubusercontent.com/vvaske/PolarisBiosEditor/master/notice");
                sr = new StreamReader(myStream);
                string notice = sr.ReadToEnd().Trim();

                MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                DialogResult result;

                result = MessageBox.Show(notice + "\n\nClick Yes button to copy to clipboard", "A message from the developer", buttons);

                if (result == System.Windows.Forms.DialogResult.OK)
                {

                    Clipboard.SetText(notice);

                }

                myStream.Close();


            } catch (System.Net.WebException) {
                this.Text += " (offline mode)";
            }
#else
            Console.WriteLine("This is a debug build, skipping update check.");
#endif
            Console.WriteLine("Open a AMD Polaris VBIOS file to see it's some detalied info as XML in this console");

            rc.Add("MT51J256M3", "MICRON");
            rc.Add("EDW4032BAB", "ELPIDA");
            rc.Add("H5GC4H24AJ", "HYNIX_1");
            rc.Add("H5GQ4H24AJ", "HYNIX_2");
            rc.Add("H5GQ8H24MJ", "HYNIX_2");
            rc.Add("H5GC8H24MJ", "HYNIX_3");
            rc.Add("H5GC8H24AJ", "HYNIX_4");
            rc.Add("K4G80325FB", "SAMSUNG");
            rc.Add("K4G41325FE", "SAMSUNG");
            rc.Add("K4G41325FC", "SAMSUNG");
            rc.Add("K4G41325FS", "SAMSUNG");
            rc.Add("K4G80325FC", "SAMSUNG4");

            save.Enabled = false;
            boxROM.Enabled = false;
            boxPOWERPLAY.Enabled = false;
            boxPOWERTUNE.Enabled = false;
            boxFAN.Enabled = false;
            boxGPU.Enabled = false;
            boxMEM.Enabled = false;
            boxVRAM.Enabled = false;

            tableVRAM.MouseClick += new MouseEventHandler(listView_ChangeSelection);
            tableVRAM_TIMING.MouseClick += new MouseEventHandler(listView_ChangeSelection);
            tableMEMORY.MouseClick += new MouseEventHandler(listView_ChangeSelection);
            tableGPU.MouseClick += new MouseEventHandler(listView_ChangeSelection);
            tableFAN.MouseClick += new MouseEventHandler(listView_ChangeSelection);
            tablePOWERTUNE.MouseClick += new MouseEventHandler(listView_ChangeSelection);
            tablePOWERPLAY.MouseClick += new MouseEventHandler(listView_ChangeSelection);
            tableROM.MouseClick += new MouseEventHandler(listView_ChangeSelection);

        }

        private void PolarisBiosEditor_Load(object sender, EventArgs e)
        {

        }

        private void editSubItem2_Click(object sender, EventArgs e)
        {
            MouseEventArgs me = (MouseEventArgs)e;
            if (me.Button == MouseButtons.Right)
            {
                if (editSubItem2.Text.Length == 96)
                {
                    byte[] decode = StringToByteArray(editSubItem2.Text);
                    MessageBox.Show("Decode Memory Timings " + decode + " / not implemented yet!");
                }
            }
        }

        private void OpenFileDialog_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "BIOS (.rom)|*.rom|All Files (*.*)|*.*";
            openFileDialog.FilterIndex = 1;
            openFileDialog.Multiselect = false;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                save.Enabled = false;

                tableROM.Items.Clear();
                tablePOWERPLAY.Items.Clear();
                tablePOWERTUNE.Items.Clear();
                tableFAN.Items.Clear();
                tableGPU.Items.Clear();
                tableMEMORY.Items.Clear();
                tableVRAM.Items.Clear();
                tableVRAM_TIMING.Items.Clear();

                this.Text = this.programTitle + " " + this.version + " - " + "[" + openFileDialog.SafeFileName + "]";

                System.IO.Stream fileStream = openFileDialog.OpenFile();
                if ((fileStream.Length != 524288) && (fileStream.Length != 524288 / 2))
                {
                    MessageBox.Show("This BIOS is non standard size.\nFlashing this BIOS may corrupt your graphics card.", "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                using (BinaryReader br = new BinaryReader(fileStream))
                {
                    buffer = br.ReadBytes((int)fileStream.Length);

                    atom_rom_header_offset = getValueAtPosition(16, atom_rom_header_ptr);
                    atom_rom_header = Reader<ATOM_ROM_HEADER>(atom_rom_header_offset).ReadPrint();
                    string vendorId = atom_rom_header.usVendorID.ToString("X");
                    fixChecksum(false);

                    String firmwareSignature = new string(atom_rom_header.uaFirmWareSignature);
                    if (!firmwareSignature.Equals("ATOM"))
                    {
                        MessageBox.Show("WARNING! BIOS Signature is not valid. Only continue if you are 100% sure what you are doing!");
                    }

                    DialogResult msgSuported = DialogResult.Yes;
                    if (!supportedID.Contains(vendorId))
                    {
                        msgSuported = MessageBox.Show("Unsupported DeviceID 0x" + vendorId + " - Continue?", "WARNING", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    }
                    if (msgSuported == DialogResult.Yes)
                    {
                        StringBuilder sb = new StringBuilder();

                        Int32 ptr = atom_rom_header.usBIOS_BootupMessageOffset + 2;
                        while (ptr != -1)
                        {
                            Char c = (Char)buffer[ptr];
                            if (c == '\0')
                            {
                                ptr = -1;
                            }
                            else if (c == '\n' || c == '\r')
                            {
                                ptr++;
                            }
                            else
                            {
                                sb.Append(c);
                                ptr++;
                            }
                        }

                        BIOS_BootupMessage = sb.ToString();

                        txtBIOSBootupMessage.Text = BIOS_BootupMessage;
                        txtBIOSBootupMessage.MaxLength = BIOS_BootupMessage.Length;

                        atom_data_table = Reader<ATOM_DATA_TABLES>(atom_rom_header.usMasterDataTableOffset).ReadPrint();
                        atom_powerplay_offset = atom_data_table.PowerPlayInfo;
                        atom_powerplay_table = Reader<ATOM_POWERPLAY_TABLE>(atom_powerplay_offset).ReadPrint();

                        atom_powertune_offset = atom_data_table.PowerPlayInfo + atom_powerplay_table.usPowerTuneTableOffset;
                        atom_powertune_table = Reader<ATOM_Polaris_PowerTune_Table>(atom_powertune_offset).ReadPrint();
                        Debug.Assert(atom_powertune_table.ucRevId == 4, "Unknown version of ATOM_POWERTUNE_TABLE");

                        atom_fan_offset = atom_data_table.PowerPlayInfo + atom_powerplay_table.usFanTableOffset;
                        atom_fan_table = Reader<ATOM_FAN_TABLE>(atom_fan_offset).ReadPrint();

                        atom_mclk_table_offset = atom_data_table.PowerPlayInfo + atom_powerplay_table.usMclkDependencyTableOffset;
                        atom_mclk_table = Reader<ATOM_MCLK_TABLE>(atom_mclk_table_offset).ReadPrint();
                        atom_mclk_entries = new ATOM_MCLK_ENTRY[atom_mclk_table.ucNumEntries];
                        for (var i = 0; i < atom_mclk_entries.Length; i++)
                        {
                            atom_mclk_entries[i] = Reader<ATOM_MCLK_ENTRY>(atom_mclk_table_offset + Marshal.SizeOf(typeof(ATOM_MCLK_TABLE)) + Marshal.SizeOf(typeof(ATOM_MCLK_ENTRY)) * i).ReadPrint();
                        }

                        atom_sclk_table_offset = atom_data_table.PowerPlayInfo + atom_powerplay_table.usSclkDependencyTableOffset;
                        atom_sclk_table = Reader<ATOM_SCLK_TABLE>(atom_sclk_table_offset).ReadPrint();
                        atom_sclk_entries = new ATOM_SCLK_ENTRY[atom_sclk_table.ucNumEntries];
                        for (var i = 0; i < atom_sclk_entries.Length; i++)
                        {
                            atom_sclk_entries[i] = Reader<ATOM_SCLK_ENTRY>(atom_sclk_table_offset + Marshal.SizeOf(typeof(ATOM_SCLK_TABLE)) + Marshal.SizeOf(typeof(ATOM_SCLK_ENTRY)) * i).ReadPrint();
                        }

                        atom_vddc_table_offset = atom_data_table.PowerPlayInfo + atom_powerplay_table.usVddcLookupTableOffset;
                        atom_vddc_table = Reader<ATOM_VOLTAGE_TABLE>(atom_vddc_table_offset).ReadPrint();
                        atom_vddc_entries = new ATOM_VOLTAGE_ENTRY[atom_vddc_table.ucNumEntries];
                        for (var i = 0; i < atom_vddc_table.ucNumEntries; i++)
                        {
                            atom_vddc_entries[i] = Reader<ATOM_VOLTAGE_ENTRY>(atom_vddc_table_offset + Marshal.SizeOf(typeof(ATOM_VOLTAGE_TABLE)) + Marshal.SizeOf(typeof(ATOM_VOLTAGE_ENTRY)) * i).ReadPrint();
                        }

                        atom_vram_info_offset = atom_data_table.VRAM_Info;
                        atom_vram_info = Reader<ATOM_VRAM_INFO>(atom_vram_info_offset).ReadPrint();
                        atom_vram_entries = new ATOM_VRAM_ENTRY[atom_vram_info.ucNumOfVRAMModule];
                        var atom_vram_entry_reader = Reader<ATOM_VRAM_ENTRY>(atom_vram_info_offset + Marshal.SizeOf(typeof(ATOM_VRAM_INFO)));
                        for (var i = 0; i < atom_vram_info.ucNumOfVRAMModule; i++)
                        {
                            atom_vram_entries[i] = atom_vram_entry_reader.ReadPrint();
                            atom_vram_entry_reader.Jump(atom_vram_entries[i].usModuleSize);
                        }
                        Print("End of mem\n");

                        atom_vram_timing_offset = atom_vram_info_offset + atom_vram_info.usMemClkPatchTblOffset + 0x2E;
                        atom_vram_timing_entries = new ATOM_VRAM_TIMING_ENTRY[MAX_VRAM_ENTRIES];
                        for (var i = 0; i < MAX_VRAM_ENTRIES; i++)
                        {
                            atom_vram_timing_entries[i] = Reader<ATOM_VRAM_TIMING_ENTRY>(atom_vram_timing_offset + Marshal.SizeOf(typeof(ATOM_VRAM_TIMING_ENTRY)) * i).ReadPrint();

                            // atom_vram_timing_entries have an undetermined length
                            // attempt to determine the last entry in the array
                            if (atom_vram_timing_entries[i].ulClkRange == 0)
                            {
                                Array.Resize(ref atom_vram_timing_entries, i);
                                break;
                            }
                        }

                        var atom_object_header = Reader<ATOM_OBJECT_HEADER_V3>(atom_data_table.Object_Header).ReadPrint();

                        ReadPrintObjectTable<ATOM_DISPLAY_OBJECT_PATH_TABLE, ATOM_DISPLAY_OBJECT_PATH>(atom_object_header.usDisplayPathTableOffset, tb => tb.ucNumOfDispPath, o => o.usSize);
                        Print("Encoders:");
                        ReadPrintObjectTable<ATOM_OBJECT_TABLE, ATOM_OBJECT>(atom_object_header.usEncoderObjectTableOffset, tb => tb.ucNumberOfObjects, PrintAndReturnLen);
                        Print("Connectors:");
                        ReadPrintObjectTable<ATOM_OBJECT_TABLE, ATOM_OBJECT>(atom_object_header.usConnectorObjectTableOffset, tb => tb.ucNumberOfObjects, PrintAndReturnLen);
                        Print("Routers:");
                        ReadPrintObjectTable<ATOM_OBJECT_TABLE, ATOM_OBJECT>(atom_object_header.usRouterObjectTableOffset, tb => tb.ucNumberOfObjects, PrintAndReturnLen);

                        var used_volt_obffset = 0;

                        ReadPrintTableDetailed<ATOM_VOLTAGE_OBJECT_INFO_V3_1, atom_voltage_object_header_v4>(atom_data_table.VoltageObjectInfo,
                            (volt_table, i) => used_volt_obffset < (volt_table.sHeader.usStructureSize - Marshal.SizeOf(volt_table.sHeader)),
                            (volt_object, reader) =>
                            {
                                used_volt_obffset += volt_object.usSize;
                                if (volt_object.AsI2cSpecified)
                                {
                                    var detailed_reader = ConsecutiveReader<atom_i2c_data_entry>.From(reader);
                                    detailed_reader.Jump(Marshal.SizeOf(volt_object));
                                    while(detailed_reader.Read().i2c_reg_index != FINAL_atom_i2c_data_entry.ENDING_INDEX_VALUE)
                                    {
                                        detailed_reader.ReadPrint();
                                        detailed_reader.Jump1Structure();
                                    }
                                    ConsecutiveReader<FINAL_atom_i2c_data_entry>.From(detailed_reader).ReadPrint();
                                }
                                reader.Jump(volt_object.usSize);
                            }
                            );

                        ReadPrintTable<ATOM_GPIO_I2C_INFO, ATOM_GPIO_I2C_ASSIGMENT>(atom_data_table.GPIO_I2C_Info,
                            (i2c_info_table, i) => i < (i2c_info_table.sHeader.usStructureSize - Marshal.SizeOf(i2c_info_table.sHeader)) / Marshal.SizeOf<ATOM_GPIO_I2C_ASSIGMENT>(),
                            i2c_assigment => Marshal.SizeOf(i2c_assigment)
                            );

                        tableROM.Items.Add(new ListViewItem(new string[] {
                            "BootupMessageOffset",
                            "0x" + atom_rom_header.usBIOS_BootupMessageOffset.ToString ("X")
                        }
                        ));
                        tableROM.Items.Add(new ListViewItem(new string[] {
                            "VendorID",
                            "0x" + atom_rom_header.usVendorID.ToString ("X")
                        }
                        ));
                        tableROM.Items.Add(new ListViewItem(new string[] {
                            "DeviceID",
                            "0x" + atom_rom_header.usDeviceID.ToString ("X")
                        }
                        ));
                        tableROM.Items.Add(new ListViewItem(new string[] {
                            "Sub ID",
                            "0x" + atom_rom_header.usSubsystemID.ToString ("X")
                        }
                        ));
                        tableROM.Items.Add(new ListViewItem(new string[] {
                            "Sub VendorID",
                            "0x" + atom_rom_header.usSubsystemVendorID.ToString ("X")
                        }
                        ));
                        tableROM.Items.Add(new ListViewItem(new string[] {
                            "Firmware Signature",
                            //"0x" + atom_rom_header.uaFirmWareSignature.ToString ("X")
                            new string(atom_rom_header.uaFirmWareSignature)
                        }
                        ));

                        tablePOWERPLAY.Items.Clear();
                        tablePOWERPLAY.Items.Add(new ListViewItem(new string[] {
                            "Max GPU Freq. (MHz)",
                            Convert.ToString (atom_powerplay_table.ulMaxODEngineClock / 100)
                        }
                        ));
                        tablePOWERPLAY.Items.Add(new ListViewItem(new string[] {
                            "Max Memory Freq. (MHz)",
                            Convert.ToString (atom_powerplay_table.ulMaxODMemoryClock / 100)
                        }
                        ));
                        tablePOWERPLAY.Items.Add(new ListViewItem(new string[] {
                            "Power Control Limit (%)",
                            Convert.ToString (atom_powerplay_table.usPowerControlLimit)
                        }
                        ));

                        tablePOWERTUNE.Items.Clear();
                        tablePOWERTUNE.Items.Add(new ListViewItem(new string[] {
                            "TDP (W)",
                            Convert.ToString (atom_powertune_table.usTDP)
                        }
                        ));
                        tablePOWERTUNE.Items.Add(new ListViewItem(new string[] {
                            "TDC (A)",
                            Convert.ToString (atom_powertune_table.usTDC)
                        }
                        ));
                        tablePOWERTUNE.Items.Add(new ListViewItem(new string[] {
                            "Max Power Limit (W)",
                            Convert.ToString (atom_powertune_table.usMaximumPowerDeliveryLimit)
                        }
                        ));
                        tablePOWERTUNE.Items.Add(new ListViewItem(new string[] {
                            "Max Temp. (C)",
                            Convert.ToString (atom_powertune_table.usTjMax)
                        }
                        ));
                        tablePOWERTUNE.Items.Add(new ListViewItem(new string[] {
                            "Shutdown Temp. (C)",
                            Convert.ToString (atom_powertune_table.usSoftwareShutdownTemp)
                        }
                        ));
                        tablePOWERTUNE.Items.Add(new ListViewItem(new string[] {
                            "Hotspot Temp. (C)",
                            Convert.ToString (atom_powertune_table.usTemperatureLimitHotspot)
                        }
                        ));
                        tablePOWERTUNE.Items.Add(new ListViewItem(new string[] {
                            "Clock Stretch Amount",
                            Convert.ToString (atom_powertune_table.usClockStretchAmount)
                        }
                        ));

                        tableFAN.Items.Clear();
                        tableFAN.Items.Add(new ListViewItem(new string[] {
                            "Temp. Hysteresis",
                            Convert.ToString (atom_fan_table.ucTHyst)
                        }
                        ));
                        tableFAN.Items.Add(new ListViewItem(new string[] {
                            "Min Temp. (C)",
                            Convert.ToString (atom_fan_table.usTMin / 100)
                        }
                        ));
                        tableFAN.Items.Add(new ListViewItem(new string[] {
                            "Med Temp. (C)",
                            Convert.ToString (atom_fan_table.usTMed / 100)
                        }
                        ));
                        tableFAN.Items.Add(new ListViewItem(new string[] {
                            "High Temp. (C)",
                            Convert.ToString (atom_fan_table.usTHigh / 100)
                        }
                        ));
                        tableFAN.Items.Add(new ListViewItem(new string[] {
                            "Max Temp. (C)",
                            Convert.ToString (atom_fan_table.usTMax / 100)
                        }
                        ));
                        tableFAN.Items.Add(new ListViewItem(new string[] {
                            "Target Temp. (C)",
                            Convert.ToString (atom_fan_table.ucTargetTemperature)
                        }
                        ));
                        tableFAN.Items.Add(new ListViewItem(new string[] {
                            "Legacy or Fuzzy Fan Mode",
                            Convert.ToString (atom_fan_table.ucFanControlMode)
                        }
                        ));
                        tableFAN.Items.Add(new ListViewItem(new string[] {
                            "Min PWM (%)",
                            Convert.ToString (atom_fan_table.usPWMMin / 100)
                        }
                        ));
                        tableFAN.Items.Add(new ListViewItem(new string[] {
                            "Med PWM (%)",
                            Convert.ToString (atom_fan_table.usPWMMed / 100)
                        }
                        ));
                        tableFAN.Items.Add(new ListViewItem(new string[] {
                            "High PWM (%)",
                            Convert.ToString (atom_fan_table.usPWMHigh / 100)
                        }
                        ));
                        tableFAN.Items.Add(new ListViewItem(new string[] {
                            "Max PWM (%)",
                            Convert.ToString (atom_fan_table.usFanPWMMax)
                        }
                        ));
                        tableFAN.Items.Add(new ListViewItem(new string[] {
                            "Max RPM",
                            Convert.ToString (atom_fan_table.usFanRPMMax)
                        }
                        ));
                        tableFAN.Items.Add(new ListViewItem(new string[] {
                            "Sensitivity",
                            Convert.ToString (atom_fan_table.usFanOutputSensitivity)
                        }
                        ));
                        tableFAN.Items.Add(new ListViewItem(new string[] {
                            "Acoustic Limit (MHz)",
                            Convert.ToString (atom_fan_table.ulMinFanSCLKAcousticLimit / 100)
                        }
                        ));

                        tableGPU.Items.Clear();
                        for (var i = 0; i < atom_sclk_table.ucNumEntries; i++)
                        {
                            tableGPU.Items.Add(new ListViewItem(new string[] {
                                Convert.ToString (atom_sclk_entries [i].ulSclk / 100),
                                Convert.ToString (atom_vddc_entries [atom_sclk_entries [i].ucVddInd].usVdd)
                            }
                            ));
                        }

                        tableMEMORY.Items.Clear();
                        for (var i = 0; i < atom_mclk_table.ucNumEntries; i++)
                        {
                            tableMEMORY.Items.Add(new ListViewItem(new string[] {
                                Convert.ToString (atom_mclk_entries [i].ulMclk / 100),
                                Convert.ToString (atom_mclk_entries [i].usMvdd)
                            }
                            ));
                        }

                        listVRAM.Items.Clear();
                        for (var i = 0; i < atom_vram_info.ucNumOfVRAMModule; i++)
                        {
                            if (atom_vram_entries[i].strMemPNString[0] != 0)
                            {
                                var mem_id_full = atom_vram_entries[i].FullName;
                                var mem_id = mem_id_full.Substring(0, 10);
                                string mem_vendor;
                                if (rc.ContainsKey(mem_id))
                                {
                                    mem_vendor = rc[mem_id];
                                }
                                else
                                {
                                    mem_vendor = "UNKNOWN";
                                }

                                listVRAM.Items.Add(mem_id_full + "-" + mem_vendor);
                            }
                        }
                        listVRAM.SelectedIndex = 0;
                        atom_vram_index = listVRAM.SelectedIndex;

                        tableVRAM_TIMING.Items.Clear();
                        for (var i = 0; i < atom_vram_timing_entries.Length; i++)
                        {
                            uint tbl = atom_vram_timing_entries[i].ulClkRange >> 24;
                            tableVRAM_TIMING.Items.Add(new ListViewItem(new string[] {
                                tbl.ToString () + ":" + (atom_vram_timing_entries [i].ulClkRange & 0x00FFFFFF) / 100,
                                atom_vram_timing_entries [i].LatencyString
                            }
                            ));
                        }

                        save.Enabled = true;
                        boxROM.Enabled = true;
                        boxPOWERPLAY.Enabled = true;
                        boxPOWERTUNE.Enabled = true;
                        boxFAN.Enabled = true;
                        boxGPU.Enabled = true;
                        boxMEM.Enabled = true;
                        boxVRAM.Enabled = true;
                    }
                    fileStream.Close();
                }
            }
            tableROM.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            tableROM.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);

            tableFAN.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            tableFAN.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);

            tablePOWERPLAY.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            tablePOWERPLAY.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);

            tableGPU.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            tableGPU.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);

            tablePOWERTUNE.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            tablePOWERTUNE.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);

            tableMEMORY.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            tableMEMORY.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);

            tableVRAM.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            tableVRAM.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);

            tableVRAM_TIMING.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            tableVRAM_TIMING.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        private int PrintAndReturnLen(ATOM_OBJECT o)
        {
            if (o.usRecordOffset != 0)
            {
                var reader = Reader<ATOM_COMMON_RECORD_HEADER>(atom_data_table.Object_Header + o.usRecordOffset);
                while(true)
                {
                    var first = reader.buffer.First();
                    if (first == 0 || first == 255)
                    {
                        break;
                    }
                    if (reader.buffer.Skip(1).First() < 2)
                    {
                        break;
                    }
                    var rec = reader.ReadPrint();
                    reader.JumpPrintExtra(rec.ucRecordSize);
                }
            }
            return Marshal.SizeOf(o);
        }
        private void ReadPrintTableDetailed<TTable, TObject>(int offset, Func<TTable, int, bool> continue_parse, Action<TObject, ConsecutiveReader<TObject>> next_jumper)
        {
            var table_reader = Reader<TTable>(offset);
            var table = table_reader.ReadPrint();
            table_reader.Jump1Structure();
            var object_reader = ConsecutiveReader<TObject>.From(table_reader);
            for (var i = 0; continue_parse(table, i); i++)
            {
                var atom_object = object_reader.ReadPrint();
                next_jumper(atom_object, object_reader);
            }
        }

        private void ReadPrintTable<TTable, TObject>(int offset, Func<TTable, int, bool> continue_parse, Func<TObject, int> entry_size)
        {
            ReadPrintTableDetailed<TTable, TObject>(offset, continue_parse, (atom_object, object_reader) => object_reader.Jump(entry_size(atom_object)));
        }

        private void ReadPrintObjectTable<TTable, TObject>(UInt16 offset, Func<TTable, int> entry_count, Func<TObject, int> entry_size)
        {
            if (offset == 0)
            {
                Print("Table not present:" + typeof(TTable).Name);
                return;
            }
            ReadPrintTable<TTable, TObject>(atom_data_table.Object_Header + offset, (table, i) => (i < entry_count(table)), entry_size);
        }

        ConsecutiveReader<T> Reader<T>(int offset)
        {
            return new ConsecutiveReader<T>(buffer, offset, this);
        }

        public Int32 getValueAtPosition(int bits, int position, bool isFrequency = false)
        {
            int value = 0;
            if (position <= buffer.Length - 4)
            {
                switch (bits)
                {
                    case 8:
                    default:
                        value = buffer[position];
                        break;
                    case 16:
                        value = (buffer[position + 1] << 8) | buffer[position];
                        break;
                    case 24:
                        value = (buffer[position + 2] << 16) | (buffer[position + 1] << 8) | buffer[position];
                        break;
                    case 32:
                        value = (buffer[position + 3] << 24) | (buffer[position + 2] << 16) | (buffer[position + 1] << 8) | buffer[position];
                        break;
                }
                if (isFrequency)
                    return value / 100;
                return value;
            }
            return -1;
        }

        public bool setValueAtPosition(int value, int bits, int position, bool isFrequency = false)
        {
            if (isFrequency)
                value *= 100;
            if (position <= buffer.Length - 4)
            {
                switch (bits)
                {
                    case 8:
                    default:
                        buffer[position] = (byte)value;
                        break;
                    case 16:
                        buffer[position] = (byte)value;
                        buffer[position + 1] = (byte)(value >> 8);
                        break;
                    case 24:
                        buffer[position] = (byte)value;
                        buffer[position + 1] = (byte)(value >> 8);
                        buffer[position + 2] = (byte)(value >> 16);
                        break;
                    case 32:
                        buffer[position] = (byte)value;
                        buffer[position + 1] = (byte)(value >> 8);
                        buffer[position + 2] = (byte)(value >> 16);
                        buffer[position + 3] = (byte)(value >> 32);
                        break;
                }
                return true;
            }
            return false;
        }

        private bool setValueAtPosition(String text, int bits, int position, bool isFrequency = false)
        {
            int value = 0;
            if (!int.TryParse(text, out value))
            {
                return false;
            }
            return setValueAtPosition(value, bits, position, isFrequency);
        }

        private void SaveFileDialog_Click(object sender, EventArgs e)
        {
            SaveFileDialog SaveFileDialog = new SaveFileDialog();
            SaveFileDialog.Title = "Save As";
            SaveFileDialog.Filter = "BIOS (*.rom)|*.rom";

            if (SaveFileDialog.ShowDialog() == DialogResult.OK)
            {
                FileStream fs = new FileStream(SaveFileDialog.FileName, FileMode.Create);
                BinaryWriter bw = new BinaryWriter(fs);

                for (var i = 0; i < tableROM.Items.Count; i++)
                {
                    ListViewItem container = tableROM.Items[i];
                    var name = container.Text;
                    var value = container.SubItems[1].Text;

                    if (name == "VendorID")
                    {
                        var num = (int)int32.ConvertFromString(value);
                        atom_rom_header.usVendorID = (UInt16)num;
                    }
                    else if (name == "DeviceID")
                    {
                        var num = (int)int32.ConvertFromString(value);
                        atom_rom_header.usDeviceID = (UInt16)num;
                    }
                    else if (name == "Sub ID")
                    {
                        var num = (int)int32.ConvertFromString(value);
                        atom_rom_header.usSubsystemID = (UInt16)num;
                    }
                    else if (name == "Sub VendorID")
                    {
                        var num = (int)int32.ConvertFromString(value);
                        atom_rom_header.usSubsystemVendorID = (UInt16)num;
                    }
                    else if (name == "Firmware Signature")
                    {
                        atom_rom_header.uaFirmWareSignature = value.ToCharArray();
                    }
                }

                for (var i = 0; i < tablePOWERPLAY.Items.Count; i++)
                {
                    ListViewItem container = tablePOWERPLAY.Items[i];
                    var name = container.Text;
                    var value = container.SubItems[1].Text;
                    var num = (int)int32.ConvertFromString(value);

                    if (name == "Max GPU Freq. (MHz)")
                    {
                        atom_powerplay_table.ulMaxODEngineClock = (UInt32)(num * 100);
                    }
                    else if (name == "Max Memory Freq. (MHz)")
                    {
                        atom_powerplay_table.ulMaxODMemoryClock = (UInt32)(num * 100);
                    }
                    else if (name == "Power Control Limit (%)")
                    {
                        atom_powerplay_table.usPowerControlLimit = (UInt16)num;
                    }
                }

                for (var i = 0; i < tablePOWERTUNE.Items.Count; i++)
                {
                    ListViewItem container = tablePOWERTUNE.Items[i];
                    var name = container.Text;
                    var value = container.SubItems[1].Text;
                    var num = (int)int32.ConvertFromString(value);

                    if (name == "TDP (W)")
                    {
                        atom_powertune_table.usTDP = (UInt16)num;
                    }
                    else if (name == "TDC (A)")
                    {
                        atom_powertune_table.usTDC = (UInt16)num;
                    }
                    else if (name == "Max Power Limit (W)")
                    {
                        atom_powertune_table.usMaximumPowerDeliveryLimit = (UInt16)num;
                    }
                    else if (name == "Max Temp. (C)")
                    {
                        atom_powertune_table.usTjMax = (UInt16)num;
                    }
                    else if (name == "Shutdown Temp. (C)")
                    {
                        atom_powertune_table.usSoftwareShutdownTemp = (UInt16)num;
                    }
                    else if (name == "Hotspot Temp. (C)")
                    {
                        atom_powertune_table.usTemperatureLimitHotspot = (UInt16)num;
                    }
                    else if (name == "Clock Stretch Amount")
                    {
                        atom_powertune_table.usClockStretchAmount = (UInt16)num;
                    }
                }

                for (var i = 0; i < tableFAN.Items.Count; i++)
                {
                    ListViewItem container = tableFAN.Items[i];
                    var name = container.Text;
                    var value = container.SubItems[1].Text;
                    var num = (int)int32.ConvertFromString(value);

                    if (name == "Temp. Hysteresis")
                    {
                        atom_fan_table.ucTHyst = (Byte)num;
                    }
                    else if (name == "Min Temp. (C)")
                    {
                        atom_fan_table.usTMin = (UInt16)(num * 100);
                    }
                    else if (name == "Med Temp. (C)")
                    {
                        atom_fan_table.usTMed = (UInt16)(num * 100);
                    }
                    else if (name == "High Temp. (C)")
                    {
                        atom_fan_table.usTHigh = (UInt16)(num * 100);
                    }
                    else if (name == "Max Temp. (C)")
                    {
                        atom_fan_table.usTMax = (UInt16)(num * 100);
                    }
                    else if (name == "Target Temp. (C)")
                    {
                        atom_fan_table.ucTargetTemperature = (Byte)num;
                    }
                    else if (name == "Legacy or Fuzzy Fan Mode")
                    {
                        atom_fan_table.ucFanControlMode = (Byte)(num);
                    }
                    else if (name == "Min PWM (%)")
                    {
                        atom_fan_table.usPWMMin = (UInt16)(num * 100);
                    }
                    else if (name == "Med PWM (%)")
                    {
                        atom_fan_table.usPWMMed = (UInt16)(num * 100);
                    }
                    else if (name == "High PWM (%)")
                    {
                        atom_fan_table.usPWMHigh = (UInt16)(num * 100);
                    }
                    else if (name == "Max PWM (%)")
                    {
                        atom_fan_table.usFanPWMMax = (UInt16)num;
                    }
                    else if (name == "Max RPM")
                    {
                        atom_fan_table.usFanRPMMax = (UInt16)num;
                    }
                    else if (name == "Sensitivity")
                    {
                        atom_fan_table.usFanOutputSensitivity = (UInt16)num;
                    }
                    else if (name == "Acoustic Limit (MHz)")
                    {
                        atom_fan_table.ulMinFanSCLKAcousticLimit = (UInt32)(num * 100);
                    }
                }

                for (var i = 0; i < tableGPU.Items.Count; i++)
                {
                    ListViewItem container = tableGPU.Items[i];
                    var name = container.Text;
                    var value = container.SubItems[1].Text;
                    var mhz = (int)int32.ConvertFromString(name) * 100;
                    var mv = (int)int32.ConvertFromString(value);

                    atom_sclk_entries[i].ulSclk = (UInt32)mhz;
                    atom_vddc_entries[atom_sclk_entries[i].ucVddInd].usVdd = (UInt16)mv;
                    if (mv < 0xFF00)
                    {
                        atom_sclk_entries[i].usVddcOffset = 0;
                    }
                }

                for (var i = 0; i < tableMEMORY.Items.Count; i++)
                {
                    ListViewItem container = tableMEMORY.Items[i];
                    var name = container.Text;
                    var value = container.SubItems[1].Text;
                    var mhz = (int)int32.ConvertFromString(name) * 100;
                    var mv = (int)int32.ConvertFromString(value);

                    atom_mclk_entries[i].ulMclk = (UInt32)mhz;
                    atom_mclk_entries[i].usMvdd = (UInt16)mv;
                }

                updateVRAM_entries();
                for (var i = 0; i < tableVRAM_TIMING.Items.Count; i++)
                {
                    ListViewItem container = tableVRAM_TIMING.Items[i];
                    var name = container.Text;
                    var value = container.SubItems[1].Text;
                    var arr = StringToByteArray(value);
                    UInt32 mhz;
                    if (name.IndexOf(':') > 0)
                    {
                        mhz = (UInt32)uint32.ConvertFromString(name.Substring(name.IndexOf(':') + 1)) * 100;
                        mhz += (UInt32)uint32.ConvertFromString(name.Substring(0, name.IndexOf(':'))) << 24; // table id
                    }
                    else
                    {
                        mhz = (UInt32)uint32.ConvertFromString(name) * 100;
                    }
                    atom_vram_timing_entries[i].ulClkRange = mhz;
                    atom_vram_timing_entries[i].ucLatency = arr;
                }

                setBytesAtPosition(buffer, atom_rom_header_offset, getBytes(atom_rom_header));
                setBytesAtPosition(buffer, atom_powerplay_offset, getBytes(atom_powerplay_table));
                setBytesAtPosition(buffer, atom_powertune_offset, getBytes(atom_powertune_table));
                setBytesAtPosition(buffer, atom_fan_offset, getBytes(atom_fan_table));

                for (var i = 0; i < atom_mclk_table.ucNumEntries; i++)
                {
                    setBytesAtPosition(buffer, atom_mclk_table_offset + Marshal.SizeOf(typeof(ATOM_MCLK_TABLE)) + Marshal.SizeOf(typeof(ATOM_MCLK_ENTRY)) * i, getBytes(atom_mclk_entries[i]));
                }

                for (var i = 0; i < atom_sclk_table.ucNumEntries; i++)
                {
                    setBytesAtPosition(buffer, atom_sclk_table_offset + Marshal.SizeOf(typeof(ATOM_SCLK_TABLE)) + Marshal.SizeOf(typeof(ATOM_SCLK_ENTRY)) * i, getBytes(atom_sclk_entries[i]));
                }

                for (var i = 0; i < atom_vddc_table.ucNumEntries; i++)
                {
                    setBytesAtPosition(buffer, atom_vddc_table_offset + Marshal.SizeOf(typeof(ATOM_VOLTAGE_TABLE)) + Marshal.SizeOf(typeof(ATOM_VOLTAGE_ENTRY)) * i, getBytes(atom_vddc_entries[i]));
                }

                var atom_vram_entry_offset = atom_vram_info_offset + Marshal.SizeOf(typeof(ATOM_VRAM_INFO));
                for (var i = 0; i < atom_vram_info.ucNumOfVRAMModule; i++)
                {
                    setBytesAtPosition(buffer, atom_vram_entry_offset, getBytes(atom_vram_entries[i]));
                    atom_vram_entry_offset += atom_vram_entries[i].usModuleSize;
                }

                atom_vram_timing_offset = atom_vram_info_offset + atom_vram_info.usMemClkPatchTblOffset + 0x2E;
                for (var i = 0; i < atom_vram_timing_entries.Length; i++)
                {
                    setBytesAtPosition(buffer, atom_vram_timing_offset + Marshal.SizeOf(typeof(ATOM_VRAM_TIMING_ENTRY)) * i, getBytes(atom_vram_timing_entries[i]));
                }

                BIOS_BootupMessage = txtBIOSBootupMessage.Text.Substring(0, BIOS_BootupMessage.Length);

                setBytesAtPosition(buffer, atom_rom_header.usBIOS_BootupMessageOffset+2, Encoding.ASCII.GetBytes(BIOS_BootupMessage));
                fixChecksum(true);
                bw.Write(buffer);

                fs.Close();
                bw.Close();
            }
        }

        private void fixChecksum(bool save)
        {
            Byte checksum = buffer[atom_rom_checksum_offset];
            int size = buffer[0x02] * 512;
            Byte offset = 0;

            for (int i = 0; i < size; i++)
            {
                offset += buffer[i];
            }
            if (checksum == (buffer[atom_rom_checksum_offset] - offset))
            {
                txtChecksum.ForeColor = Color.Green;
            }
            else if (!save)
            {
                txtChecksum.ForeColor = Color.Red;
                MessageBox.Show("Invalid checksum - Save to fix!", "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            if (save)
            {
                buffer[atom_rom_checksum_offset] -= offset;
                txtChecksum.ForeColor = Color.Green;
            }
            txtChecksum.Text = "0x" + buffer[atom_rom_checksum_offset].ToString("X");
        }

        public static string ByteArrayToString(byte[] ba)
        {
            string hex = BitConverter.ToString(ba);
            return hex.Replace("-", "");
        }

        public static byte[] StringToByteArray(String hex)
        {
            if (hex.Length % 2 != 0)
            {
                MessageBox.Show("Invalid hex string", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw new InvalidDataException();
            }
            byte[] bytes = new byte[hex.Length / 2];
            for (int i = 0; i < hex.Length; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }
            return bytes;
        }

        public void updateVRAM_entries()
        {
            for (var i = 0; i < tableVRAM.Items.Count; i++)
            {
                ListViewItem container = tableVRAM.Items[i];
                var name = container.Text;
                var value = container.SubItems[1].Text;
                var num = (int)int32.ConvertFromString(value);

                if (name == "VendorID")
                {
                    atom_vram_entries[atom_vram_index].ucMemoryVenderID = (Byte)num;
                }
                else if (name == "Size (MB)")
                {
                    atom_vram_entries[atom_vram_index].usMemorySize = (UInt16)num;
                }
                else if (name == "Density")
                {
                    atom_vram_entries[atom_vram_index].ucDensity = (Byte)num;
                }
                else if (name == "Type")
                {
                    atom_vram_entries[atom_vram_index].ucMemoryType = (Byte)num;
                }
            }
        }

        private void listVRAM_SelectionChanged(object sender, EventArgs e)
        {
            updateVRAM_entries();
            tableVRAM.Items.Clear();
            if (listVRAM.SelectedIndex >= 0 && listVRAM.SelectedIndex < listVRAM.Items.Count)
            {
                atom_vram_index = listVRAM.SelectedIndex;
                tableVRAM.Items.Add(new ListViewItem(new string[] {
                    "VendorID",
                    "0x" + atom_vram_entries [atom_vram_index].ucMemoryVenderID.ToString ("X")
                }
                ));
                tableVRAM.Items.Add(new ListViewItem(new string[] {
                    "Size (MB)",
                    Convert.ToString (atom_vram_entries [atom_vram_index].usMemorySize)
                }
                ));
                tableVRAM.Items.Add(new ListViewItem(new string[] {
                    "Density",
                    "0x" + atom_vram_entries [atom_vram_index].ucDensity.ToString ("X")
                }
                ));
                tableVRAM.Items.Add(new ListViewItem(new string[] {
                    "Type",
                    "0x" + atom_vram_entries [atom_vram_index].ucMemoryType.ToString ("X")
                }
                ));
                tableVRAM.Items.Add(new ListViewItem(new string[] {
                    "Num Of VRAM Module",
                     atom_vram_info.ucNumOfVRAMModule.ToString ("X")
                }
                ));
            }
        }

        private void listVRAM_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void apply_timings(int vendor_index, int timing_index)
        {
            for (var i = 0; i < tableVRAM_TIMING.Items.Count; i++)
            {
                ListViewItem container = tableVRAM_TIMING.Items[i];
                var name = container.Text;
                UInt32 real_mhz = 0;
                int mem_index = -1;

                if (name.IndexOf(':') > 0)
                {
                    // get mem index
                    mem_index = (Int32)int32.ConvertFromString(name.Substring(0, 1));
                }
                else
                {
                    mem_index = 32768;
                }

                real_mhz = (UInt32)uint32.ConvertFromString(name.Substring(name.IndexOf(':') + 1));

                if (real_mhz >= 1500 && (mem_index == vendor_index || mem_index == 32768))
                {
                    // set the timings
                    container.SubItems[1].Text = timings[timing_index];
                }
            }
        }

        private void apply_timings1(int vendor_index, int timing_index)
        {
            for (var i = 0; i < tableVRAM_TIMING.Items.Count; i++)
            {
                ListViewItem container = tableVRAM_TIMING.Items[i];
                var name = container.Text;
                UInt32 real_mhz = 0;
                int mem_index = -1;

                if (name.IndexOf(':') > 0)
                {
                    // get mem index
                    mem_index = (Int32)int32.ConvertFromString(name.Substring(0, 1));
                }
                else
                {
                    mem_index = 32768;
                }

                real_mhz = (UInt32)uint32.ConvertFromString(name.Substring(name.IndexOf(':') + 1));

                if (real_mhz >= 1750 && (mem_index == vendor_index || mem_index == 32768))
                {
                    // set the timings
                    container.SubItems[1].Text = timings[timing_index];
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            /*
            int samsung_index = -1;
            int micron_index = -1;
            int elpida_index = -1;
            int hynix_1_index = -1;
            int hynix_2_index = -1;
            int hynix_3_index = -1;
            for (var i = 0; i < atom_vram_info.ucNumOfVRAMModule; i++)
            {
                string mem_vendor;
                if (atom_vram_entries[i].strMemPNString[0] != 0)
                {
                    var mem_id = Encoding.UTF8.GetString(atom_vram_entries[i].strMemPNString).Substring(0, 10);

                    if (rc.ContainsKey(mem_id))
                    {
                        mem_vendor = rc[mem_id];
                    }
                    else
                    {
                        mem_vendor = "UNKNOWN";
                    }

                    switch (mem_vendor)
                    {
                        case "SAMSUNG":
                            samsung_index = i;
                            break;
                        case "MICRON":
                            micron_index = i;
                            break;
                        case "ELPIDA":
                            elpida_index = i;
                            break;
                        case "HYNIX_1":
                            hynix_1_index = i;
                            break;
                        case "HYNIX_2":
                            hynix_2_index = i;
                            break;
                        case "HYNIX_3":
                            hynix_3_index = i;
                            break;
                    }
                }
            }
            */
            int samsung_index = -1;
            int samsung4_index = -1;
            int micron_index = -1;
            int elpida_index = -1;
            int hynix_1_index = -1;
            int hynix_2_index = -1;
            int hynix_3_index = -1;
            int hynix_4_index = -1;
            for (int index = 0; index < (int)this.atom_vram_info.ucNumOfVRAMModule; ++index)
            {
                if ((int)this.atom_vram_entries[index].strMemPNString[0] != 0)
                {
                    string key = Encoding.UTF8.GetString(this.atom_vram_entries[index].strMemPNString).Substring(0, 10);
                    string str = !this.rc.ContainsKey(key) ? "[ UNKNOWN ]" : this.rc[key];
                    if (!(str == "SAMSUNG"))
                    {
                        if (!(str == "MICRON"))
                        {
                            if (!(str == "ELPIDA"))
                            {
                                if (!(str == "HYNIX_1"))
                                {
                                    if (!(str == "HYNIX_2"))
                                    {
                                        if (!(str == "HYNIX_3"))
                                        {
                                            if (!(str == "HYNIX_4"))
                                            {
                                                if (str == "SAMSUNG4")
                                                    samsung4_index = index;
                                            }
                                            else
                                                hynix_4_index = index;
                                        }
                                        else
                                            hynix_3_index = index;
                                    }
                                    else
                                        hynix_2_index = index;
                                }
                                else
                                    hynix_1_index = index;
                            }
                            else
                                elpida_index = index;
                        }
                        else
                            micron_index = index;
                    }
                    else
                        samsung_index = index;
                }
            }

            if (samsung_index != -1)
            {
                if (MessageBox.Show("Do you want faster Uber-mix 3.1?", "Important Question", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    int num = (int)MessageBox.Show("Samsung Memory found at index #" + (object)samsung_index + ", now applying UBERMIX 3.1 timings to 1750+ strap(s)");
                    this.apply_timings1(samsung_index, 0);
                }
                else
                {
                    int num = (int)MessageBox.Show("Samsung Memory found at index #" + (object)samsung_index + ", now applying UBERMIX 3.2 timings to 1750+ strap(s)");
                    this.apply_timings1(samsung_index, 1);
                }
            }

            if (samsung4_index != -1)
            {
                if (MessageBox.Show("Do you want pro Samsung timing?", "Important Question", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    MessageBox.Show("Samsung4 Memory found at index #" + (object)samsung4_index + ", now applying pro Samsung 30Mh/s timings to 1750+ strap(s)");
                    this.apply_timings1(samsung4_index, 10);
                    MessageBox.Show("Little help for testing timing, set Core clock to 1150mhz and Memory clock to 2100mhz", "Hint!");
                }
                else
                {
                    MessageBox.Show("Samsung4 Memory found at index #" + (object)samsung4_index + ", now applying Samsung4 basic 29Mh/s timings to 1750+ strap(s)");
                    this.apply_timings1(samsung4_index, 11);
                }
            }

            if (hynix_3_index != -1)
            {
                if (MessageBox.Show("Do you want Universal Hynix Timing?", "Important Question", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    MessageBox.Show("Hynix (3) Memory found at index #" + hynix_3_index + ", now applying Universal HYNIX MINING timings to 1500+ strap(s)");
                    apply_timings(hynix_3_index, 8);
                }
                else
                {
                    MessageBox.Show("Hynix (3) Memory found at index #" + hynix_3_index + ", now applying GOOD HYNIX MINING timings to 1500+ strap(s)");
                    apply_timings(hynix_3_index, 2);
                }
            }

            if (hynix_2_index != -1)
            {
                if (MessageBox.Show("Do you want Universal Hynix Timing?", "Important Question", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    MessageBox.Show("Hynix (2) Memory found at index #" + hynix_2_index + ", now applying Universal HYNIX MINING timings to 1500+ strap(s)");
                    apply_timings(hynix_2_index, 8);
                }
                else
                {
                   int num = (int)MessageBox.Show("Hynix (2) Memory found at index #" + (object)hynix_2_index + ", now applying GOOD Hynix timings to 1500+ strap(s)");
                   this.apply_timings(hynix_2_index, 3);
                }
            }

            if (hynix_4_index != -1)
            {
                if (MessageBox.Show("Do you want Universal Hynix Timing?", "Important Question", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    MessageBox.Show("Hynix (2) Memory found at index #" + hynix_4_index + ", now applying Universal HYNIX MINING timings to 1500+ strap(s)");
                    apply_timings(hynix_4_index, 8);
                }
                else
                {
                    int num = (int)MessageBox.Show("Hynix (4) Memory found at index #" + (object)hynix_4_index + ", now applying Hynix timings to 1500+ strap(s)");
                    this.apply_timings(hynix_4_index, 9);
                }
            }

            if (micron_index != -1)
            {
                if (MessageBox.Show("Do you want Good Micron Timing?", "Important Question", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    int num = (int)MessageBox.Show("Micron Memory found at index #" + (object)micron_index + ", now applying Good Micron timings to 1500+ strap(s)");
                    this.apply_timings(micron_index, 4);
                }
                else
                {
                    int num = (int)MessageBox.Show("Micron Memory found at index #" + (object)micron_index + ", now applying S Micron timings to 1500+ strap(s)");
                    this.apply_timings(micron_index, 5);
                }
            }

            if (hynix_1_index != -1)
            {
                MessageBox.Show("Hynix (1) Memory found at index #" + hynix_1_index + ", now applying GOOD HYNIX MINING timings to 1500+ strap(s)");
                apply_timings(hynix_1_index, 6);
            }

            if (elpida_index != -1)
            {
                MessageBox.Show("Elpida Memory found at index #" + elpida_index + ", now applying GOOD ELPIDA MINING timings to 1500+ strap(s)");
                apply_timings(elpida_index, 7);
            }
            if (samsung4_index == -1 && samsung_index == -1 && hynix_2_index == -1 && hynix_3_index == -1 && hynix_1_index == -1 && elpida_index == -1 && micron_index == -1)
            {
                MessageBox.Show("Sorry, no supported memory found");
            }

            this.tablePOWERPLAY.Items[1].SubItems[1].Text = "2300";

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.miningbios.com/product/polaris-bios-editor-3-4-1-srbpolaris-style/");
        }
    }
}
