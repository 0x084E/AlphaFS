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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace AlphaFS.UnitTest
{
   partial class DirectoryTest
   {
      // Pattern: <class>_<function>_<scenario>_<expected result>

      [TestMethod]
      public void Directory_CreateDirectory_LocalAndUNC_Success()
      {
         Directory_CreateDirectory_Delete(false);
         Directory_CreateDirectory_Delete(true);
      }


      [TestMethod]
      public void Directory_CreateDirectory_WithFileSecurity_LocalAndUNC_Success()
      {
         if (!UnitTestConstants.IsAdmin())
            Assert.Inconclusive();

         Directory_CreateDirectory_WithFileSecurity(false);
         Directory_CreateDirectory_WithFileSecurity(true);
      }


      [TestMethod]
      public void Directory_CreateDirectory_CatchAlreadyExistsException_FileExistsWithSameNameAsDirectory_LocalAndUNC_Success()
      {
         Directory_CreateDirectory_CatchAlreadyExistsException_FileExistsWithSameNameAsDirectory(false);
         Directory_CreateDirectory_CatchAlreadyExistsException_FileExistsWithSameNameAsDirectory(true);
      }


      [TestMethod]
      public void Directory_CreateDirectory_CatchArgumentException_PathContainsInvalidCharacters_LocalAndUNC_Success()
      {
         Directory_CreateDirectory_CatchArgumentException_PathContainsInvalidCharacters(false);
         Directory_CreateDirectory_CatchArgumentException_PathContainsInvalidCharacters(true);
      }


      [TestMethod]
      public void Directory_CreateDirectory_CatchArgumentException_PathStartsWithColon_Local_Success()
      {
         Directory_CreateDirectory_CatchArgumentException_PathStartsWithColon(false);
      }
      

      [TestMethod]
      public void Directory_CreateDirectory_CatchDirectoryNotFoundException_NonExistingDriveLetter_LocalAndUNC_Success()
      {
         Directory_CreateDirectory_CatchDirectoryNotFoundException_NonExistingDriveLetter(false);
         Directory_CreateDirectory_CatchDirectoryNotFoundException_NonExistingDriveLetter(true);
      }


      [TestMethod]
      public void Directory_CreateDirectory_CatchNotSupportedException_PathContainsColon_LocalAndUNC_Success()
      {
         Directory_CreateDirectory_CatchNotSupportedException_PathContainsColon(false);
         Directory_CreateDirectory_CatchNotSupportedException_PathContainsColon(true);
      }
      



      private void Directory_CreateDirectory_Delete(bool isNetwork)
      {
         UnitTestConstants.PrintUnitTestHeader(isNetwork);

         var tempPath = System.IO.Path.GetTempPath();
         if (isNetwork)
            tempPath = PathUtils.AsUncPath(tempPath);


         using (var rootDir = new TemporaryDirectory(tempPath, "Directory.CreateDirectory"))
         {
            var folder = rootDir.Directory.FullName;

            // Directory depth level.
            var level = new Random().Next(10, 1000);

#if NET35
            // MSDN: .NET 4+ Trailing spaces are removed from the end of the path parameter before deleting the directory.
            folder += UnitTestConstants.EMspace;
#endif

            Console.WriteLine("\nInput Directory Path: [{0}]\n", folder);


            var root = folder;

            for (var i = 0; i < level; i++)
               root = System.IO.Path.Combine(root, "Level-" + (i + 1) + "-subFolder");

            Alphaleonis.Win32.Filesystem.Directory.CreateDirectory(root);

            Console.WriteLine("Created directory structure: Depth: [{0}], path length: [{1}] characters.", level, root.Length);
            Console.WriteLine("\n{0}", root);

            Assert.IsTrue(Alphaleonis.Win32.Filesystem.Directory.Exists(root), "The directory does not exists, but is expected to.");
         }

         Console.WriteLine();
      }


      private void Directory_CreateDirectory_WithFileSecurity(bool isNetwork)
      {
         UnitTestConstants.PrintUnitTestHeader(isNetwork);

         var tempPath = System.IO.Path.GetTempPath();
         if (isNetwork)
            tempPath = PathUtils.AsUncPath(tempPath);


         using (var rootDir = new TemporaryDirectory(tempPath, "Directory.CreateDirectory_WithFileSecurity"))
         {
            var folder = rootDir.RandomFileFullPath;
            Console.WriteLine("\nInput Directory Path: [{0}]\n", folder);


            var pathExpected = rootDir.RandomFileFullPath + ".txt";
            var pathActual = rootDir.RandomFileFullPath + ".txt";

            var expectedFileSecurity = new System.Security.AccessControl.DirectorySecurity();
            expectedFileSecurity.AddAccessRule(new System.Security.AccessControl.FileSystemAccessRule(new System.Security.Principal.SecurityIdentifier(System.Security.Principal.WellKnownSidType.WorldSid, null), System.Security.AccessControl.FileSystemRights.FullControl, System.Security.AccessControl.AccessControlType.Allow));
            expectedFileSecurity.AddAuditRule(new System.Security.AccessControl.FileSystemAuditRule(new System.Security.Principal.SecurityIdentifier(System.Security.Principal.WellKnownSidType.WorldSid, null), System.Security.AccessControl.FileSystemRights.Read, System.Security.AccessControl.AuditFlags.Success));


            using (new Alphaleonis.Win32.Security.PrivilegeEnabler(Alphaleonis.Win32.Security.Privilege.Security))
            {
               var s1 = System.IO.Directory.CreateDirectory(pathExpected, expectedFileSecurity);
               var s2 = Alphaleonis.Win32.Filesystem.Directory.CreateDirectory(pathActual, expectedFileSecurity);
            }

            var expected = System.IO.File.GetAccessControl(pathExpected).GetSecurityDescriptorSddlForm(System.Security.AccessControl.AccessControlSections.All);
            var actual = Alphaleonis.Win32.Filesystem.File.GetAccessControl(pathActual).GetSecurityDescriptorSddlForm(System.Security.AccessControl.AccessControlSections.All);

            Assert.AreEqual(expected, actual);
         }

         Console.WriteLine();
      }


      private void Directory_CreateDirectory_CatchAlreadyExistsException_FileExistsWithSameNameAsDirectory(bool isNetwork)
      {
         UnitTestConstants.PrintUnitTestHeader(isNetwork);

         var tempPath = System.IO.Path.GetTempPath();
         if (isNetwork)
            tempPath = PathUtils.AsUncPath(tempPath);


         using (var rootDir = new TemporaryDirectory(tempPath, "Directory.CreateDirectory"))
         {
            var file = rootDir.RandomFileFullPath + ".txt";
            Console.WriteLine("\nInput File Path: [{0}]\n", file);

            using (System.IO.File.Create(file)) { }


            var gotException = false;
            try
            {
               Alphaleonis.Win32.Filesystem.Directory.CreateDirectory(file);

            }
            catch (Exception ex)
            {
               gotException = ex.GetType().Name.Equals("AlreadyExistsException", StringComparison.OrdinalIgnoreCase);
            }
            Assert.IsTrue(gotException, "The exception was not caught, but is expected to.");
         }

         Console.WriteLine();
      }


      private void Directory_CreateDirectory_CatchArgumentException_PathContainsInvalidCharacters(bool isNetwork)
      {
         UnitTestConstants.PrintUnitTestHeader(isNetwork);

         var tempPath = System.IO.Path.GetTempPath();
         if (isNetwork)
            tempPath = PathUtils.AsUncPath(tempPath);

         var folder = tempPath + @"\Folder<>";
         Console.WriteLine("\nInput Directory Path: [{0}]\n", folder);


         var gotException = false;
         try
         {
            Alphaleonis.Win32.Filesystem.Directory.CreateDirectory(folder);
         }
         catch (Exception ex)
         {
            gotException = ex.GetType().Name.Equals("ArgumentException", StringComparison.OrdinalIgnoreCase);
         }
         Assert.IsTrue(gotException, "The exception was not caught, but is expected to.");

         Console.WriteLine();
      }


      private void Directory_CreateDirectory_CatchArgumentException_PathStartsWithColon(bool isNetwork)
      {
         UnitTestConstants.PrintUnitTestHeader(isNetwork);

         var folder = @":AAAAAAAAAA";
         Console.WriteLine("\nInput Directory Path: [{0}]\n", folder);


         var gotException = false;
         try
         {
            Alphaleonis.Win32.Filesystem.Directory.CreateDirectory(folder);
         }
         catch (Exception ex)
         {
            gotException = ex.GetType().Name.Equals("ArgumentException", StringComparison.OrdinalIgnoreCase);
         }
         Assert.IsTrue(gotException, "The exception was not caught, but is expected to.");

         Console.WriteLine();
      }


      private void Directory_CreateDirectory_CatchDirectoryNotFoundException_NonExistingDriveLetter(bool isNetwork)
      {
         UnitTestConstants.PrintUnitTestHeader(isNetwork);

         var folder = Alphaleonis.Win32.Filesystem.DriveInfo.GetFreeDriveLetter() + @":\NonExistingDriveLetter";
         if (isNetwork)
            folder = Alphaleonis.Win32.Filesystem.Path.LocalToUnc(folder);

         Console.WriteLine("\nInput Directory Path: [{0}]\n", folder);


         var gotException = false;
         try
         {
            Alphaleonis.Win32.Filesystem.Directory.CreateDirectory(folder);
         }
         catch (Exception ex)
         {
            gotException = ex.GetType().Name.Equals(isNetwork ? "IOException" : "DirectoryNotFoundException", StringComparison.OrdinalIgnoreCase);
         }
         Assert.IsTrue(gotException, "The exception was not caught, but is expected to.");

         Console.WriteLine();
      }


      private void Directory_CreateDirectory_CatchNotSupportedException_PathContainsColon(bool isNetwork)
      {
         UnitTestConstants.PrintUnitTestHeader(isNetwork);

         var colonText = ":aaa.txt";
         var folder = (isNetwork ? PathUtils.AsUncPath(UnitTestConstants.LocalHostShare) : UnitTestConstants.SysDrive + @"\dev\test\") + colonText;

         Console.WriteLine("\nInput Directory Path: [{0}]\n", folder);


         var gotException = false;
         try
         {
            Alphaleonis.Win32.Filesystem.Directory.CreateDirectory(folder);
         }
         catch (Exception ex)
         {
            gotException = ex.GetType().Name.Equals("NotSupportedException", StringComparison.OrdinalIgnoreCase);
         }
         Assert.IsTrue(gotException, "The exception was not caught, but is expected to.");

         Console.WriteLine();
      }
   }
}
