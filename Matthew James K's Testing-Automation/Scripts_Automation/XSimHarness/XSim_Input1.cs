/*----------------------------------------------------------------
 * 
 * ---------------------------------------------------------------
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;
using XDevkit;

namespace ConsoleApplication_Test_XSIM
{
    //enum XSIM_SYNCHMODE : uint
    //{
    //    XSIM_SYNCHMODE_FRAME = 0x00000001,
    //    XSIM_SYNCHMODE_TIME = 0x00000002
    //}

    public enum ControllerPort
    {
        Number1 = 0,
        Number2,
        Number3,
        Number4
    }

    class Program
    {
        private XboxManager manager;
        private XboxConsole console;
        private IXboxAutomation automation;
        private XBOX_AUTOMATION_GAMEPAD gamepad;
        private bool result;
        public bool enableMU;

        //[DllImport("XSimWrapperDll.dll", CharSet = CharSet.Ansi)]
        //public static extern uint XSimInitialize(UInt32 dwComponentFrameRate);

        public Program()
        {
            manager = new XboxManager();
            console = manager.OpenConsole( manager.DefaultConsole );
            automation = console.XboxAutomation;
            gamepad = new XBOX_AUTOMATION_GAMEPAD();
            result = false;
            enableMU = false;
        }

        public static void Main( string[] args )
        {
            Program program = new Program();
            program.enableMU = false;
            try
            {
                while( true )
                {
                    program.console.Reboot( null, null, null, XboxRebootFlags.Cold ); // Reboot the targeted xbox machine
                    Thread.CurrentThread.Join( 30000 );

                    program.automation.BindController( (uint)ControllerPort.Number1, 1 );
                    program.automation.ClearGamepadQueue( (uint)ControllerPort.Number1 );
                    program.automation.ConnectController( (uint)ControllerPort.Number1 );

                    program.gamepad.Buttons = XboxAutomationButtonFlags.X_Button; // Activate Tools menu
                    program.result = program.automation.QueueGamepadState( (uint)ControllerPort.Number1, program.gamepad, 2, 1 );
                    if( false == program.result ) break;
                    Thread.CurrentThread.Join( 2000 );

                    program.gamepad.Buttons = XboxAutomationButtonFlags.DPadDown; // Down arrow to System Settings
                    program.automation.QueueGamepadState( (uint)ControllerPort.Number1, program.gamepad, 2, 1 );
                    if( false == program.result ) break;
                    Thread.CurrentThread.Join( 2000 );

                    program.gamepad.Buttons = XboxAutomationButtonFlags.A_Button; // Select System Settings menu item
                    program.automation.QueueGamepadState( (uint)ControllerPort.Number1, program.gamepad, 2, 1 );
                    if( false == program.result ) break;
                    Thread.CurrentThread.Join( 2000 );

                    for( int x = 0; x < 3; ++x ) // Down arrow to MU menu item
                    {
                        program.gamepad.Buttons = XboxAutomationButtonFlags.DPadDown;
                        program.automation.QueueGamepadState( (uint)ControllerPort.Number1, program.gamepad, 2, 1 );
                        if( false == program.result ) break;
                        Thread.CurrentThread.Join( 2000 );
                    }

                    program.gamepad.Buttons = XboxAutomationButtonFlags.A_Button; // Select MU menu item
                    program.automation.QueueGamepadState( (uint)ControllerPort.Number1, program.gamepad, 2, 1 );
                    if( false == program.result ) break;
                    Thread.CurrentThread.Join( 2000 );

                    if( true == program.enableMU )
                    { // Enable the Xbox Memory Unit
                        program.gamepad.Buttons = XboxAutomationButtonFlags.DPadUp;
                        program.automation.QueueGamepadState( (uint)ControllerPort.Number1, program.gamepad, 2, 1 );
                        if( false == program.result ) break;
                        Thread.CurrentThread.Join( 2000 );

                        program.gamepad.Buttons = XboxAutomationButtonFlags.A_Button;
                        program.automation.QueueGamepadState( (uint)ControllerPort.Number1, program.gamepad, 2, 1 );
                        if( false == program.result ) break;
                        Thread.CurrentThread.Join( 2000 );

                        try
                        {
                            program.automation.DisconnectController( (uint)ControllerPort.Number1 );
                            program.automation.UnbindController( (uint)ControllerPort.Number1 );
                        }
                        catch (Exception)
                        { // Ignore any errors attempting to close the controller binding.
                        }
                    }
                    else
                    { // Disable the Xbox Memory Unit
                        program.gamepad.Buttons = XboxAutomationButtonFlags.DPadDown;
                        program.automation.QueueGamepadState( (uint)ControllerPort.Number1, program.gamepad, 2, 1 );
                        if( false == program.result ) break;
                        Thread.CurrentThread.Join( 2000 );

                        program.gamepad.Buttons = XboxAutomationButtonFlags.A_Button;
                        program.automation.QueueGamepadState( (uint)ControllerPort.Number1, program.gamepad, 2, 1 );
                        if( false == program.result ) break;

                        try
                        {
                            program.automation.DisconnectController( (uint)ControllerPort.Number1 );
                            program.automation.UnbindController( (uint)ControllerPort.Number1 );
                        }
                        catch( Exception )
                        { // Ignore any errors attempting to close the controller binding.
                        }
                        Thread.CurrentThread.Join( 25000 ); // Final A_Button reboots the machine, wait 25 seconds.
                    }
                    break;
                }
                if( false == program.result )
                    throw new ApplicationException("Last called controller action failed, or the controller is disconnected.");
            }
            catch( Exception error )
            {
                Console.WriteLine( string.Concat( "An error occured while attempting to manipulate Controller 1.\nError details:\n",
                    error.ToString() ) );
            }
            //uint statusValue = 0;
            //IntPtr handle = (IntPtr)0;

            //Directory.SetCurrentDirectory( GetXdkToolPath() );
            // Initialize XSim, polling 60 times a second
            //statusValue = XSimInitialize( 60 );

            //statusValue = XSimCreateTextSequencePlayer( "DU", XSIM_SYNCHMODE.XSIM_SYNCHMODE_FRAME, 60, out handle );

            //statusValue = XSimUninitialize();
        }

        //[DllImport("XSimWrapperDll.dll", CharSet = CharSet.Ansi)]
        //public static extern uint XSimCreateTextSequencePlayer( string lpPlaybackString, XSIM_SYNCHMODE eSynchMode,
        //    UInt32 dwDefaultPlaybackRate, out IntPtr phXSimHandle );

        //[DllImport("XSimWrapperDll.dll", CharSet = CharSet.Ansi)]
        //public static extern uint XSimUninitialize();

        /// <summary>
        /// Helper function that retrieves the "\\bin\\win32" tool path part of the XDK version installed on the local host.
        /// </summary>
        /// <returns>The win32 tool absolute path as a string.</returns>
        //public static string GetXdkToolPath()
        //{
        //    string toolPath = GetXdkPath() + "\\bin\\win32";
        //    DirectoryInfo info = new DirectoryInfo( toolPath );
        //    if( !info.Exists )
        //        Console.WriteLine( "Xbox 360 XDK command line (cmd) tools are missing from disk." );
        //    return toolPath;
        //}
        /// <summary>
        /// Return the base directory the Xbox 360 Development Kit is installed on this computer
        /// </summary>
        /// <returns>Path to the Xbox 360 development kit software</returns>
        //public static string GetXdkPath()
        //{
        //    string xdkPath = Environment.GetEnvironmentVariable( "XEDK" );
        //    if( true == string.IsNullOrEmpty( xdkPath ) )
        //    {
        //        Console.WriteLine("Variable XEDK is missing from the environment variables." );
        //    }
        //    DirectoryInfo info = new DirectoryInfo( xdkPath );
        //    if( !info.Exists )
        //    {
        //        Console.WriteLine( "The program installed path XDK is missing from the disk." );
        //    }
        //    return xdkPath;
        //}

    }
}
