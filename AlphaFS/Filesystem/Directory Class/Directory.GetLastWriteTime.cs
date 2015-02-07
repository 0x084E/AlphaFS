/*  Copyright (C) 2008-2015 Peter Palotas, Jeffrey Jangli, Alexandr Normuradov
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
using System.Security;

namespace Alphaleonis.Win32.Filesystem
{
   partial class Directory
   {
      #region .NET

      /// <summary>Gets the date and time that the specified directory was last written to.</summary>
      /// <returns>A <see cref="System.DateTime"/> structure set to the date and time that the specified directory was last written to. This value is expressed in local time.</returns>
      /// <param name="path">The directory for which to obtain write date and time information.</param>
      [SecurityCritical]
      public static DateTime GetLastWriteTime(string path)
      {
         return File.GetLastWriteTimeInternal(null, path, false, PathFormat.RelativePath).ToLocalTime();
      }



      /// <summary>Gets the date and time, in coordinated universal time (UTC) time, that the specified directory was last written to.</summary>
      /// <returns>A <see cref="System.DateTime"/> structure set to the date and time that the specified directory was last written to. This value is expressed in UTC time.</returns>
      /// <param name="path">The directory for which to obtain write date and time information.</param>
      [SecurityCritical]
      public static DateTime GetLastWriteTimeUtc(string path)
      {
         return File.GetLastWriteTimeInternal(null, path, true, PathFormat.RelativePath);
      }

      #endregion // .NET

      /// <summary>[AlphaFS] Gets the date and time that the specified directory was last written to.</summary>
      /// <returns>A <see cref="System.DateTime"/> structure set to the date and time that the specified directory was last written to. This value is expressed in local time.</returns>
      /// <param name="path">The directory for which to obtain write date and time information.</param>
      /// <param name="pathFormat">Indicates the format of the path parameter(s).</param>
      [SecurityCritical]
      public static DateTime GetLastWriteTime(string path, PathFormat pathFormat)
      {
         return File.GetLastWriteTimeInternal(null, path, false, pathFormat).ToLocalTime();
      }



      /// <summary>[AlphaFS] Gets the date and time, in coordinated universal time (UTC) time, that the specified directory was last written to.</summary>
      /// <returns>A <see cref="System.DateTime"/> structure set to the date and time that the specified directory was last written to. This value is expressed in UTC time.</returns>
      /// <param name="path">The directory for which to obtain write date and time information.</param>
      /// <param name="pathFormat">Indicates the format of the path parameter(s).</param>
      [SecurityCritical]
      public static DateTime GetLastWriteTimeUtc(string path, PathFormat pathFormat)
      {
         return File.GetLastWriteTimeInternal(null, path, true, pathFormat);
      }

      #region Transactional

      /// <summary>[AlphaFS] Gets the date and time that the specified directory was last written to.</summary>
      /// <returns>A <see cref="System.DateTime"/> structure set to the date and time that the specified directory was last written to. This value is expressed in local time.</returns>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The directory for which to obtain write date and time information.</param>
      [SecurityCritical]
      public static DateTime GetLastWriteTimeTransacted(KernelTransaction transaction, string path)
      {
         return File.GetLastWriteTimeInternal(transaction, path, false, PathFormat.RelativePath).ToLocalTime();
      }

      /// <summary>[AlphaFS] Gets the date and time that the specified directory was last written to.</summary>
      /// <returns>A <see cref="System.DateTime"/> structure set to the date and time that the specified directory was last written to. This value is expressed in local time.</returns>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The directory for which to obtain write date and time information.</param>
      /// <param name="pathFormat">Indicates the format of the path parameter(s).</param>
      [SecurityCritical]
      public static DateTime GetLastWriteTimeTransacted(KernelTransaction transaction, string path, PathFormat pathFormat)
      {
         return File.GetLastWriteTimeInternal(transaction, path, false, pathFormat).ToLocalTime();
      }



      /// <summary>[AlphaFS] Gets the date and time, in coordinated universal time (UTC) time, that the specified directory was last written to.</summary>
      /// <returns>A <see cref="System.DateTime"/> structure set to the date and time that the specified directory was last written to. This value is expressed in UTC time.</returns>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The directory for which to obtain write date and time information.</param>
      [SecurityCritical]
      public static DateTime GetLastWriteTimeUtcTransacted(KernelTransaction transaction, string path)
      {
         return File.GetLastWriteTimeInternal(transaction, path, true, PathFormat.RelativePath);
      }

      /// <summary>[AlphaFS] Gets the date and time, in coordinated universal time (UTC) time, that the specified directory was last written to.</summary>
      /// <returns>A <see cref="System.DateTime"/> structure set to the date and time that the specified directory was last written to. This value is expressed in UTC time.</returns>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The directory for which to obtain write date and time information.</param>
      /// <param name="pathFormat">Indicates the format of the path parameter(s).</param>
      [SecurityCritical]
      public static DateTime GetLastWriteTimeUtcTransacted(KernelTransaction transaction, string path, PathFormat pathFormat)
      {
         return File.GetLastWriteTimeInternal(transaction, path, true, pathFormat);
      }
      
      #endregion // Transactional
   }
}