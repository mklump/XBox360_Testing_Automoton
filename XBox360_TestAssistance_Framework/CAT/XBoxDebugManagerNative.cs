// -----------------------------------------------------------------------
// <copyright file="XboxDebugManagerNative.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace CAT
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text;
    using ComTypes = System.Runtime.InteropServices.ComTypes;

    /// <summary>
    /// A helper class that exposes native Xbox Debug Manager APIs
    /// </summary>
    public class XboxDebugManagerNative
    {
        /// <summary>
        /// HResult enumeration
        /// </summary>
        internal enum HResult : uint
        {
            /// <summary>
            /// No error occurred. 
            /// </summary>
            XBDM_NOERR = 0x02DA0000,

            /// <summary>
            /// An undefined error has occurred.
            /// </summary>
            XBDM_UNDEFINED = 0x82DA0000,

            /// <summary>
            /// The maximum number of connections has been exceeded.
            /// </summary>
            XBDM_MAXCONNECT = 0x82DA0001,

            /// <summary>
            /// No such file exists.
            /// </summary>
            XBDM_NOSUCHFILE = 0x82DA0002,

            /// <summary>
            /// No such module exists.
            /// </summary>
            XBDM_NOMODULE = 0x82DA0003,

            /// <summary>
            /// The referenced memory has been unmapped.
            /// </summary>
            XBDM_MEMUNMAPPED = 0x82DA0004,

            /// <summary>
            /// No such thread ID exists.
            /// </summary>
            XBDM_NOTHREAD = 0x82DA0005,

            /// <summary>
            /// The console clock is not set. 
            /// </summary>
            XBDM_CLOCKNOTSET = 0x82DA0006,

            /// <summary>
            /// An invalid command was specified. 
            /// </summary>
            XBDM_INVALIDCMD = 0x82DA0007,

            /// <summary>
            /// Thread not stopped. 
            /// </summary>
            XBDM_NOTSTOPPED = 0x82DA0008,

            /// <summary>
            /// File must be copied, not moved. 
            /// </summary>
            XBDM_MUSTCOPY = 0x82DA0009,

            /// <summary>
            /// A file already exists with the same name. 
            /// </summary>
            XBDM_ALREADYEXISTS = 0x82DA000A,

            /// <summary>
            /// The directory is not empty. 
            /// </summary>
            XBDM_DIRNOTEMPTY = 0x82DA000B,

            /// <summary>
            /// An invalid file name was specified.
            /// </summary>
            XBDM_BADFILENAME = 0x82DA000C,

            /// <summary>
            /// Cannot create the specified file.
            /// </summary>
            XBDM_CANNOTCREATE = 0x82DA000D,

            /// <summary>
            /// Cannot access the specified file. 
            /// </summary>
            XBDM_CANNOTACCESS = 0x82DA000E,

            /// <summary>
            /// The device is full. 
            /// </summary>
            XBDM_DEVICEFULL = 0x82DA000F,

            /// <summary>
            /// This title cannot be debugged 
            /// </summary>
            XBDM_NOTDEBUGGABLE = 0x82DA0010,

            /// <summary>
            /// The counter type is invalid. 
            /// </summary>
            XBDM_BADCOUNTTYPE = 0x82DA0011,

            /// <summary>
            /// Counter data is not available.  
            /// </summary>
            XBDM_COUNTUNAVAILABLE = 0x82DA0012,

            /// <summary>
            /// The console is not locked. 
            /// </summary>
            XBDM_NOTLOCKED = 0x82DA0014,

            /// <summary>
            /// Key exchange is required. 
            /// </summary>
            XBDM_KEYXCHG = 0x82DA0015,

            /// <summary>
            /// A dedicated connection is required.
            /// </summary>
            XBDM_MUSTBEDEDICATED = 0x82DA0016,

            /// <summary>
            /// The argument was invalid. 
            /// </summary>
            XBDM_INVALIDARG = 0x82DA0017,

            /// <summary>
            /// The profile is not started. 
            /// </summary>
            XBDM_PROFILENOTSTARTED = 0x82DA0018,

            /// <summary>
            /// The profile is already started.
            /// </summary>
            XBDM_PROFILEALREADYSTARTED = 0x82DA0019,

            /// <summary>
            /// The console is already in DMN_EXEC_STOP. 
            /// </summary>
            XBDM_ALREADYSTOPPED = 0x82DA001A,

            /// <summary>
            /// FastCAP is not enabled. 
            /// </summary>
            XBDM_FASTCAPNOTENABLED = 0x82DA001B,

            /// <summary>
            /// The Debug Monitor could not allocate memory. 
            /// </summary>
            XBDM_NOMEMORY = 0x82DA001C,

            /// <summary>
            /// Initialization of profiling has taken longer than allowed.
            /// </summary>
            XBDM_TIMEOUT = 0x82DA001D,

            /// <summary>
            /// The path was not found. 
            /// </summary>
            XBDM_NOSUCHPATH = 0x82DA001E,

            /// <summary>
            /// The screen input format is invalid.
            /// </summary>
            XBDM_INVALID_SCREEN_INPUT_FORMAT = 0x82DA001F,

            /// <summary>
            /// The screen output format is invalid.
            /// </summary>
            XBDM_INVALID_SCREEN_OUTPUT_FORMAT = 0x82DA0020,

            /// <summary>
            /// CallCAP is not enabled.
            /// </summary>
            XBDM_CALLCAPNOTENABLED = 0x82DA0021,

            /// <summary>
            /// Both FastCAP and CallCAP are enabled in different modules.
            /// </summary>
            XBDM_INVALIDCAPCFG = 0x82DA0022,

            /// <summary>
            /// Neither FastCAP nor CallCAP are enabled. 
            /// </summary>
            XBDM_CAPNOTENABLED = 0x82DA0023,

            /// <summary>
            /// A branched to a section the instrumentation code failed.
            /// </summary>
            XBDM_TOOBIGJUMP = 0x82DA0024,

            /// <summary>
            /// A necessary field is not present in the header of Xbox 360 title. 
            /// </summary>
            XBDM_FIELDNOTPRESENT = 0x82DA0025,

            /// <summary>
            /// Provided data buffer for profiling is too small. 
            /// </summary>
            XBDM_OUTPUTBUFFERTOOSMALL = 0x82DA0026,

            /// <summary>
            /// The Xbox 360 console is currently rebooting. 
            /// </summary>
            XBDM_PROFILEREBOOT = 0x82DA0027,

            /// <summary>
            /// The maximum duration was exceeded.
            /// </summary>
            XBDM_MAXDURATIONEXCEEDED = 0x82DA0029,

            /// <summary>
            /// The current state of game controller automation is incompatible with the requested action. 
            /// </summary>
            XBDM_INVALIDSTATE = 0x82DA002A,

            /// <summary>
            /// The maximum number of extensions are already used. 
            /// </summary>
            XBDM_MAXEXTENSIONS = 0x82DA002B,

            /// <summary>
            /// The Performance Monitor Counters (PMC) session is already active.
            /// </summary>
            XBDM_PMCSESSIONALREADYACTIVE = 0x82DA002C,

            /// <summary>
            /// The Performance Monitor Counters (PMC) session is not active. 
            /// </summary>
            XBDM_PMCSESSIONNOTACTIVE = 0x82DA002D,

            /// <summary>
            /// The string passed to a debug monitor function was too long. The total length of a command string, which includes its null termination and trailing CR/LF must be less than or equal too 512 characters.
            /// </summary>
            XBDM_LINE_TOO_LONG = 0x82DA002E,

            /// <summary>
            /// The current application has an incompatible version of D3D. 
            /// </summary>
            XBDM_D3D_DEBUG_COMMAND_NOT_IMPLEMENTED = 0x82DA0050,

            /// <summary>
            /// The D3D surface is not currently valid.
            /// </summary>
            XBDM_D3D_INVALID_SURFACE = 0x82DA0051,

            /// <summary>
            /// Cannot connect to the target system. 
            /// </summary>
            XBDM_CANNOTCONNECT = 0x82DA0100,

            /// <summary>
            /// The connection to the target system has been lost. 
            /// </summary>
            XBDM_CONNECTIONLOST = 0x82DA0101,

            /// <summary>
            /// An unexpected file error has occurred.
            /// </summary>
            XBDM_FILEERROR = 0x82DA0103,

            /// <summary>
            /// Used by the walk functions to signal the end of a list.  
            /// </summary>
            XBDM_ENDOFLIST = 0x82DA0104,

            /// <summary>
            /// The buffer referenced was too small to receive the requested data. 
            /// </summary>
            XBDM_BUFFER_TOO_SMALL = 0x82DA0105,

            /// <summary>
            /// The file specified is not a valid XBE. 
            /// </summary>
            XBDM_NOTXBEFILE = 0x82DA0106,

            /// <summary>
            /// Not all requested memory could be written. 
            /// </summary>
            XBDM_MEMSETINCOMPLETE = 0x82DA0107,

            /// <summary>
            /// No target system name has been set. 
            /// </summary>
            XBDM_NOXBOXNAME = 0x82DA0108,

            /// <summary>
            /// There is no string representation of this error code. 
            /// </summary>
            XBDM_NOERRORSTRING = 0x82DA0109,

            /// <summary>
            /// The Xbox 360 console returns an formatted status string following a command. When using the custom command processor, it may indicate that console and PC code are not compatible.  
            /// </summary>
            XBDM_INVALIDSTATUS = 0x82DA010A,

            /// <summary>
            /// A previous command is still pending.
            /// </summary>
            XBDM_TASK_PENDING = 0x82DA0150,

            /// <summary>
            /// A connection has been successfully established.
            /// </summary>
            XBDM_CONNECTED = 0x02DA0001,

            /// <summary>
            /// One of the three types of continued transactions supported by the command processor.
            /// </summary>
            XBDM_MULTIRESPONSE = 0x02DA0002,

            /// <summary>
            /// One of the three types of continued transactions supported by the command processor. 
            /// </summary>
            XBDM_BINRESPONSE = 0x02DA0003,

            /// <summary>
            /// One of the three types of continued transactions supported by the command processor.
            /// </summary>
            XBDM_READYFORBIN = 0x02DA0004,

            /// <summary>
            /// A connection has been dedicated to a specific threaded command handler. 
            /// </summary>
            XBDM_DEDICATED = 0x02DA0005,

            /// <summary>
            /// The profiling session has been restarted successfully.
            /// </summary>
            XBDM_PROFILERESTARTED = 0x02DA0006,

            /// <summary>
            /// Fast call-attribute profiling is enabled. 
            /// </summary>
            XBDM_FASTCAPENABLED = 0x02DA0007,

            /// <summary>
            /// Calling call-attribute profiling is enabled. 
            /// </summary>
            XBDM_CALLCAPENABLED = 0x02DA0008,

            /// <summary>
            /// A result code. 
            /// </summary>
            XBDM_RESULTCODE = 0x02DA0009,

            /// <summary>
            /// Access is denied.
            /// </summary>
            ERROR_ACCESS_DENIED = 0x80070005,

            /// <summary>
            /// Not enough storage is available to process this command. 
            /// </summary>
            ERROR_NOT_ENOUGH_MEMORY = 0x80070008,

            /// <summary>
            /// The program issued a command but the command length is incorrect. 
            /// </summary>
            ERROR_BAD_LENGTH = 0x80070018,

            /// <summary>
            /// The parameter is incorrect.
            /// </summary>
            ERROR_INVALID_PARAMETER = 0x80070057,

            /// <summary>
            /// The file name or directory name syntax is incorrect. 
            /// </summary>
            ERROR_INVALID_NAME = 0x8007007B,

            /// <summary>
            /// The required resource is busy. 
            /// </summary>
            ERROR_BUSY = 0x800700AA,

            /// <summary>
            /// The system software cannot find the function. 
            /// </summary>
            ERROR_INVALID_ORDINAL = 0x800700B6,

            /// <summary>
            /// System could not allocate the required space in a log. 
            /// </summary>
            ERROR_NO_LOG_SPACE = 0x800703FB,

            /// <summary>
            /// The service has not been started. 
            /// </summary>
            ERROR_SERVICE_NOT_ACTIVE = 0x80070426,

            /// <summary>
            /// Element not found. 
            /// </summary>
            ERROR_NOT_FOUND = 0x80070490
        }

        /// <summary>
        /// An enumeration of values used when getting the debug memory status
        /// </summary>
        internal enum ConsoleMemConfig : uint
        {
            /// <summary>
            /// No additional memory
            /// </summary>
            DM_CONSOLEMEMCONFIG_NOADDITIONALMEM = 0,

            /// <summary>
            /// Additional memory is disabled
            /// </summary>
            DM_CONSOLEMEMCONFIG_ADDITIONALMEMDISABLED = 1,

            /// <summary>
            /// Additional memory is enabled
            /// </summary>
            DM_CONSOLEMEMCONFIG_ADDITIONALMEMENABLED = 2
        }

        /// <summary>
        /// Sets an Xbox as the target of native APIs
        /// Does not set as default
        /// </summary>
        /// <param name="name">Name of Xbox to set as target of native APIs</param>
        /// <returns>An HRESULT</returns>
        [DllImport("xbdm.dll", CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        internal static extern HResult DmSetXboxNameNoRegister([MarshalAs(UnmanagedType.LPStr)] string name);

        /// <summary>
        /// Gets the PDB Signature for the specified module
        /// </summary>
        /// <param name="baseAddress">Base address of module to get PDB signature for</param>
        /// <param name="pdbSignature">Receives the PDB signature</param>
        /// <returns>An HRESULT</returns>
        [DllImport("xbdm.dll", CharSet = CharSet.Ansi)]
        internal static extern HResult DmFindPdbSignature(uint baseAddress, out DM_PDB_SIGNATURE pdbSignature);

        /// <summary>
        /// Gets system information
        /// </summary>
        /// <param name="pdmGetSystemInfo">Receives system information</param>
        /// <returns>An HRESULT</returns>
        [DllImport("xbdm.dll", CharSet = CharSet.Ansi)]
        internal static extern HResult DmGetSystemInfo(ref DM_SYSTEM_INFO pdmGetSystemInfo);

        /// <summary>
        /// Gets the available disk space of a drive on the Xbox
        /// </summary>
        /// <param name="drive">Drive to check for free disk space on</param>
        /// <param name="freeBytesAvailableToCaller">Receives the number of bytes available</param>
        /// <param name="totalNumberOfBytes">Receives the total number of bytes</param>
        /// <param name="totalNumberOfFreeBytes">Receives the total number of free bytes</param>
        /// <returns>An HRESULT</returns>
        [DllImport("xbdm.dll", CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        internal static extern HResult DmGetDiskFreeSpace([MarshalAs(UnmanagedType.LPStr)] string drive, out ulong freeBytesAvailableToCaller, out ulong totalNumberOfBytes, out ulong totalNumberOfFreeBytes);

        /// <summary>
        /// Gets the debug memory status of the Xbox
        /// </summary>
        /// <param name="config">Receives the debug memory status</param>
        /// <returns>An HRESULT</returns>
        [DllImport("xbdm.dll", CharSet = CharSet.Ansi)]
        internal static extern HResult DmGetConsoleDebugMemoryStatus(out ConsoleMemConfig config);

        /// <summary>
        /// Structure for PDB Signature
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        internal struct DM_PDB_SIGNATURE
        {
            /// <summary>
            /// GUID of PDB Signature
            /// </summary>
            public Guid Guid;

            /// <summary>
            /// Age field of PDB Signature
            /// </summary>
            public int Age;

            /// <summary>
            /// Path field of PDB Signature
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string Path;
        }

        /// <summary>
        /// Structure for XDK version information
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        internal struct DM_VERSION_INFO
        {
            /// <summary>
            /// Major version number
            /// </summary>
            public ushort Major;

            /// <summary>
            /// Minor version number
            /// </summary>
            public ushort Minor;

            /// <summary>
            /// Build number
            /// </summary>
            public ushort Build;

            /// <summary>
            /// QFE number
            /// </summary>
            public ushort Qfe;
        }

        /// <summary>
        /// Structure for Xbox system information
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        internal struct DM_SYSTEM_INFO
        {
            /// <summary>
            /// Size of this struct
            /// </summary>
            public int SizeOfStruct;

            /// <summary>
            /// Base Kernel version
            /// </summary>
            public DM_VERSION_INFO BaseKernelVersion;

            /// <summary>
            /// Kernel version
            /// </summary>
            public DM_VERSION_INFO KernelVersion;

            /// <summary>
            /// XDK Version
            /// </summary>
            public DM_VERSION_INFO XDKVersion;

            /// <summary>
            /// System Info Flags
            /// </summary>
            public uint SystemInfoFlags;
        }
    }
}