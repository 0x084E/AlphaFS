/*  Copyright (C) 2008-2018 Peter Palotas, Jeffrey Jangli, Alexandr Normuradov
 *  
 *  Permission is hereby granted, free of charge, to any person obtaining a copy 
 *  of this software and associated documentation files (the "Software"), to deal 
 *  in the Software without restriction, including without limitation the rights 
 *  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
 *  copies of the Software, and to permit persons to whom the Software is 
 *  furnished to do so, subject to the following conditions:
 *  
 *  The above copyright notice and this permission notice shall be included in 
 *  all copies or substantial portions of the Software.
 *  
 *  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 *  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 *  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
 *  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 *  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
 *  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN 
 *  THE SOFTWARE. 
 */

using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Security;
using System.Security.AccessControl;
using Alphaleonis.Win32.Filesystem;
using Alphaleonis.Win32.Security;

namespace Alphaleonis.Win32.Device
{
   /// <summary>[AlphaFS] Provides access to information of a physical disk on the Computer.</summary>
   [Serializable]
   [SecurityCritical]
   public sealed partial class PhysicalDiskInfo
   {
      #region Fields

      private Collection<int> _partitionIndexCollection;
      private Collection<string> _volumeGuidCollection;
      private Collection<string> _logicalDriveCollection;

      #endregion // Fields


      #region Constructors

      private PhysicalDiskInfo()
      {
      }


      /// <summary>[AlphaFS] Initializes an instance from a physical disk number.</summary>
      /// <param name="deviceNumber">A number that indicates a physical disk on the Computer.</param>
      public PhysicalDiskInfo(int deviceNumber)
      {
         if (deviceNumber < 0)
            throw new ArgumentOutOfRangeException("deviceNumber");

         CreatePhysicalDiskInfoInstance(deviceNumber, null, null, null);
      }


      /// <summary>[AlphaFS] Initializes an instance from a physical disk device path.</summary>
      /// <remark>
      ///   Creating an instance for every volume/logical drive on the Computer is expensive as each call queries all physical disks, associated volumes/logical drives.
      ///   Instead, use method <see cref="Local.EnumeratePhysicalDisks()"/> and property <see cref="VolumeGuids"/> or <see cref="LogicalDrives"/>.
      /// </remark>
      /// <param name="devicePath">
      ///    <para>A disk path such as: <c>\\.\PhysicalDrive0</c></para>
      ///    <para>A drive path such as: <c>C</c>, <c>C:</c> or <c>C:\</c></para>
      ///    <para>A volume <see cref="Guid"/> such as: <c>\\?\Volume{xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx}\</c></para>
      ///    <para>A <see cref="Filesystem.DeviceInfo.DevicePath"/> string such as: <c>\\?\scsi#disk...{xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx}</c></para>
      /// </param>
      public PhysicalDiskInfo(string devicePath)
      {
         CreatePhysicalDiskInfoInstance(-1, devicePath, null, null);
      }


      /// <summary>Used by <see cref="Local.EnumeratePhysicalDisks()"/></summary>
      internal PhysicalDiskInfo(int deviceNumber, StorageDeviceInfo storageDeviceInfo, DeviceInfo deviceInfo)
      {
         CreatePhysicalDiskInfoInstance(deviceNumber, null, storageDeviceInfo, deviceInfo);
      }

      #endregion // Constructors


      [SecurityCritical]
      private void CreatePhysicalDiskInfoInstance(int deviceNumber, string devicePath, StorageDeviceInfo storageDeviceInfo, DeviceInfo deviceInfo)
      {
         var isElevated = ProcessContext.IsElevatedProcess;
         var getByDeviceNumber = deviceNumber > -1;

         bool isDrive;
         bool isVolume;
         bool isDevice;

         if (!getByDeviceNumber && Utils.IsNullOrWhiteSpace(devicePath) && null != deviceInfo)
            devicePath = deviceInfo.DevicePath;

         var localDevicePath = FileSystemHelper.GetValidatedDevicePath(getByDeviceNumber ? Path.PhysicalDrivePrefix + deviceNumber.ToString(CultureInfo.InvariantCulture) : devicePath, out isDrive, out isVolume, out isDevice);

         if (isDrive)
            localDevicePath = FileSystemHelper.GetLocalDevicePath(localDevicePath);

         string physicalDriveNumberPath = null;
         
         // The StorageDeviceInfo is always needed as it contains the device- and partition number.

         StorageDeviceInfo = storageDeviceInfo ?? Local.GetStorageDeviceInfo(isElevated, isDevice, deviceNumber, localDevicePath, out physicalDriveNumberPath);
         
         if (null == StorageDeviceInfo)
            return;

         deviceNumber = getByDeviceNumber ? deviceNumber : StorageDeviceInfo.DeviceNumber;

         if (!SetDeviceInfoDataFromDeviceNumber(isElevated, deviceNumber, deviceInfo))
            return;


         // If physicalDriveNumberPath != null, the drive is opened using: "\\.\PhysicalDriveX" path format
         // which is the device, not the volume/logical drive.

         localDevicePath = FileSystemHelper.GetValidatedDevicePath(physicalDriveNumberPath ?? localDevicePath, out isDrive, out isVolume, out isDevice);

         DosDeviceName = Volume.QueryDosDevice(Path.GetRegularPathCore(localDevicePath, GetFullPathOptions.None, false));


         using (var safeFileHandle = Local.OpenDevice(localDevicePath, isElevated ? FileSystemRights.Read : NativeMethods.FILE_ANY_ACCESS))
         {
            StorageAdapterInfo = Local.GetStorageAdapterInfo(safeFileHandle, deviceNumber, localDevicePath, DeviceInfo.BusReportedDeviceDescription);

            StoragePartitionInfo = Local.GetStoragePartitionInfo(safeFileHandle, deviceNumber, localDevicePath);
         }
         

         PopulatePhysicalDisk(isElevated);


         // The Win32 API to retrieve the total size of the device requires an elevated process or the TotalSize is 0.
         // The Win32 API to retrieve the total size of the device partition does not require elevation, so use that value.

         if (!isElevated && StorageDeviceInfo.TotalSize == 0 && null != StoragePartitionInfo)

            StorageDeviceInfo.TotalSize = isDevice ? StoragePartitionInfo.TotalSize : new DiskSpaceInfo(localDevicePath, false, true, true).TotalNumberOfBytes;
      }
      

      private bool SetDeviceInfoDataFromDeviceNumber(bool isElevated, int deviceNumber, DeviceInfo deviceInfo)
      {
         if (null == deviceInfo)
            foreach (var device in Local.EnumerateDevicesCore(null, new[] {DeviceGuid.Disk, DeviceGuid.CDRom}, false))
            {
               string unusedDevicePath;

               var storageDeviceInfo = Local.GetStorageDeviceInfo(isElevated, true, deviceNumber, device.DevicePath, out unusedDevicePath);

               if (null != storageDeviceInfo)
               {
                  deviceInfo = device;
                  break;
               }
            }


         DeviceInfo = deviceInfo;

         return null != DeviceInfo;
      }


      /// <summary>Retrieves volumes/logical drives that belong to the PhysicalDiskInfo instance.</summary>
      [SecurityCritical]
      private void PopulatePhysicalDisk(bool isElevated)
      {
         var deviceNumber = StorageDeviceInfo.DeviceNumber;

         _partitionIndexCollection = new Collection<int>();
         _volumeGuidCollection = new Collection<string>();
         _logicalDriveCollection = new Collection<string>();


         foreach (var volumeGuid in Volume.EnumerateVolumes())
         {
            string unusedLocalDevicePath;

            // The StorageDeviceInfo is always needed as it contains the device- and partition number.

            var storageDeviceInfo = Local.GetStorageDeviceInfo(isElevated, false, deviceNumber, volumeGuid, out unusedLocalDevicePath);

            if (null == storageDeviceInfo)
               continue;


            _partitionIndexCollection.Add(storageDeviceInfo.PartitionNumber);

            _volumeGuidCollection.Add(volumeGuid);


            // Resolve logical drive from volume matching DeviceNumber and PartitionNumber.

            var driveName = Volume.GetVolumeDisplayName(volumeGuid);
            
            if (!Utils.IsNullOrWhiteSpace(driveName))

               _logicalDriveCollection.Add(Path.RemoveTrailingDirectorySeparator(driveName));
         }


         PartitionIndexes = _partitionIndexCollection;

         VolumeGuids = _volumeGuidCollection;

         LogicalDrives = _logicalDriveCollection;
      }
      



      /// <summary>Returns the "FriendlyName" of the physical disk.</summary>
      /// <returns>Returns a string that represents this instance.</returns>
      public override string ToString()
      {
         return Name ?? DevicePath;
      }


      /// <summary>Determines whether the specified Object is equal to the current Object.</summary>
      /// <param name="obj">Another object to compare to.</param>
      /// <returns><c>true</c> if the specified Object is equal to the current Object; otherwise, <c>false</c>.</returns>
      public override bool Equals(object obj)
      {
         if (null == obj || GetType() != obj.GetType())
            return false;

         var other = obj as PhysicalDiskInfo;

         return null != other &&
                other.Name == Name &&
                other.DevicePath == DevicePath &&
                other.DosDeviceName == DosDeviceName &&
                other.PhysicalDeviceObjectName == PhysicalDeviceObjectName &&
                other.PartitionIndexes == PartitionIndexes &&
                other.VolumeGuids == VolumeGuids &&
                other.LogicalDrives == LogicalDrives &&
                other.StorageAdapterInfo == StorageAdapterInfo &&
                other.StorageDeviceInfo == StorageDeviceInfo &&
                other.StoragePartitionInfo == StoragePartitionInfo;
      }


      /// <summary>Serves as a hash function for a particular type.</summary>
      /// <returns>Returns a hash code for the current Object.</returns>
      public override int GetHashCode()
      {
         return null != DevicePath ? DevicePath.GetHashCode() : 0;
      }


      /// <summary>Implements the operator ==</summary>
      /// <param name="left">A.</param>
      /// <param name="right">B.</param>
      /// <returns>The result of the operator.</returns>
      public static bool operator ==(PhysicalDiskInfo left, PhysicalDiskInfo right)
      {
         return ReferenceEquals(left, null) && ReferenceEquals(right, null) || !ReferenceEquals(left, null) && !ReferenceEquals(right, null) && left.Equals(right);
      }


      /// <summary>Implements the operator !=</summary>
      /// <param name="left">A.</param>
      /// <param name="right">B.</param>
      /// <returns>The result of the operator.</returns>
      public static bool operator !=(PhysicalDiskInfo left, PhysicalDiskInfo right)
      {
         return !(left == right);
      }
   }
}
