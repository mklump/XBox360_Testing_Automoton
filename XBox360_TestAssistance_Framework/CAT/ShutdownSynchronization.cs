// -----------------------------------------------------------------------
// <copyright file="ShutdownSynchronization.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace CAT
{
    using System;
    using System.Windows;
    using System.Windows.Threading;

    /// <summary>
    /// The ShutdownSynchronization class is used to defer shutdown of the application.
    /// This is useful in cases in which resources need to be properly cleaned up before
    /// shutdown should be allowed.
    /// </summary>
    public class ShutdownSynchronization
    {
        /// <summary>
        /// Instance of a object used as a mutex/monitor
        /// </summary>
        private static object syncLock = new object();

        /// <summary>
        /// The count of callers that have deferred shutdown
        /// </summary>
        private static int deferCount;

        /// <summary>
        /// A boolean value indicating whether the program is being shut down
        /// </summary>
        private static bool shuttingDown;

        /// <summary>
        /// Dispatcher used to execute shutdown in the UI thread
        /// </summary>
        private static Dispatcher quitDispatcher;

        /// <summary>
        /// Issues a shutdown request.  Not may shutdown immediately if shutdown has been deferred
        /// </summary>
        /// <param name="mainThreadDispatcher">Dispatcher used to execute shutdown in the UI thread</param>
        public static void Shutdown(Dispatcher mainThreadDispatcher)
        {
            quitDispatcher = mainThreadDispatcher;
            bool quit = false;

            // Wait for all users to finish before shutting down
            lock (syncLock)
            {
                shuttingDown = true;
                if (deferCount == 0)
                {
                    quit = true;
                }
            }

            if (quit)
            {
                Application.Current.Shutdown();
            }
        }

        /// <summary>
        /// Defers shutdown.  Shutdown requests will not issue a shutdown until all deferrals have been cleared.
        /// </summary>
        public static void DeferShutdown()
        {
            lock (syncLock)
            {
                ++deferCount;
            }
        }

        /// <summary>
        /// Releases a shutdown deferral.
        /// </summary>
        public static void AllowShutdown()
        {
            bool quit = false;
            lock (syncLock)
            {
                --deferCount;
                if (shuttingDown)
                {
                    if (deferCount == 0)
                    {
                        quit = true;
                    }
                }
            }

            if (quit)
            {
                quitDispatcher.BeginInvoke(new Action(() => { Application.Current.Shutdown(); }));
            }
        }
    }
}
