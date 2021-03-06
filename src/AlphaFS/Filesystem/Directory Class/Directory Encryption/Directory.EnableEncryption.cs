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
using System.IO;
using System.Security;

namespace Alphaleonis.Win32.Filesystem
{
   public static partial class Directory
   {
      /// <summary>[AlphaFS] Enables encryption of the specified directory and the files in it.
      ///   <para>This method only creates/modifies the file "Desktop.ini" in the root of <paramref name="path"/> and enables encryption by writing: "Disable=0"</para>
      ///   <para>This method does not affect encryption of files and subdirectories below the indicated directory.</para>
      /// </summary>
      /// <exception cref="ArgumentException"/>
      /// <exception cref="ArgumentNullException"/>
      /// <exception cref="DirectoryReadOnlyException"/>
      /// <exception cref="FileReadOnlyException"/>
      /// <exception cref="IOException"/>
      /// <exception cref="NotSupportedException"/>
      /// <exception cref="UnauthorizedAccessException"/>
      /// <param name="path">The name of the directory for which to enable encryption.</param>
      [SecurityCritical]
      public static void EnableEncryption(string path)
      {
         EnableDisableEncryptionCore(path, true, PathFormat.RelativePath);
      }


      /// <summary>[AlphaFS] Enables encryption of the specified directory and the files in it.
      ///   <para>This method only creates/modifies the file "Desktop.ini" in the root of <paramref name="path"/> and enables encryption by writing: "Disable=0"</para>
      ///   <para>This method does not affect encryption of files and subdirectories below the indicated directory.</para>
      /// </summary>
      /// <exception cref="ArgumentException"/>
      /// <exception cref="ArgumentNullException"/>
      /// <exception cref="DirectoryReadOnlyException"/>
      /// <exception cref="FileReadOnlyException"/>
      /// <exception cref="IOException"/>
      /// <exception cref="NotSupportedException"/>
      /// <exception cref="UnauthorizedAccessException"/>
      /// <param name="path">The name of the directory for which to enable encryption.</param>
      /// <param name="pathFormat">Indicates the format of the path parameter(s).</param>
      [SecurityCritical]
      public static void EnableEncryption(string path, PathFormat pathFormat)
      {
         EnableDisableEncryptionCore(path, true, pathFormat);
      }
   }
}
