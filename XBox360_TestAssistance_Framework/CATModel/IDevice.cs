// -----------------------------------------------------------------------
// <copyright file="IDevice.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace CAT
{
    /// <summary>
    /// Delegate type used as a callback for changes to selected state
    /// </summary>
    /// <param name="device">Device the selection state has changed on</param>
    /// <param name="isSelected">Whether or not the device is now selected</param>
    public delegate void OnSelectedChangedDelegate(IDevice device, bool isSelected);
        
    /// <summary>
    /// Interface for test devices
    /// </summary>
    public interface IDevice
    {
        /// <summary>
        /// Gets the name of this Device
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets or sets a value indicating whether or not this device is selected in the device pool
        /// </summary>
        bool IsSelected { get; set; }

        /// <summary>
        /// Starts monitoring for changes to this device's selected state
        /// </summary>
        /// <param name="d">Delegate to call when the selection state changes</param>
        void StartMonitoringSelectionChanges(OnSelectedChangedDelegate d);

        /// <summary>
        /// Stops monitoring for changes to this device's selected state
        /// </summary>
        void StopMonitoringSelectionChanges();
    }
}
