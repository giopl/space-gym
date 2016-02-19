using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Gym_Membership.Helpers
{
    public class DirectoryHelper
    {
        /// <summary>
        /// Create a directory if not exists
        /// </summary>
        /// <param name="path"></param>
        public static void CreatePath(string path)
        {
            if (!Directory.Exists(path))
            {
                DirectoryInfo di = Directory.CreateDirectory(path);
            }
        }

        /// <summary>
        /// Get all files in directory
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string[] GetAllFiles(string path)
        {
            string[] files = null;
            if (Directory.Exists(path))
            {
                // This path is a directory
                return ProcessDirectory(path);
            }

            return files;
        }

        public static void DeleteFilesByMask(string path, string mask)
        {

            var files = Directory.EnumerateFiles(path, mask);
            if (files != null && files.Count() > 0)
            {
                foreach (string f in files)
                {
                    File.Delete(f);
                }
            }
        }


        /// <summary>
        /// Check file exists
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static bool IsFileExist(string file)
        {
            return File.Exists(file);
        }


        // Process all files in the directory passed in, recurse on any directories  
        // that are found, and process the files they contain. 
        public static string[] ProcessDirectory(string targetDirectory)
        {
            // Process the list of files found in the directory. 
            string[] fileEntries = Directory.GetFiles(targetDirectory);


            // Recurse into subdirectories of this directory. 
            string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);


            if (fileEntries.Length == 0)
            {
                return subdirectoryEntries;
            }
            else
            {
                return fileEntries;
            }
        }

        /// <summary>
        /// Get all files name in directory
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <returns></returns>
        public static List<String> GetFilesNameInDirectory(string sourcePath)
        {
            List<String> result = new List<string>();
            if ((Directory.Exists(sourcePath)))
            {
                DirectoryInfo dir = new DirectoryInfo(sourcePath);
                foreach (FileInfo files in dir.GetFiles("*"))
                {
                    result.Add(files.Name);
                }
            }
            return result;
        }

        /// <summary>
        /// Delete file
        /// </summary>
        /// <param name="FileName"></param>
        /// <returns></returns>
        public static string DeleteFile(string FileName)
        {
            return DeleteFile(String.Empty, FileName);
        }

        public static string DeleteFile(string uploadFolder, string FileName)
        {
            string strMessage = "";
            try
            {
                string strPath = Path.Combine(GetPath(uploadFolder), FileName);
                if (File.Exists(strPath))
                {
                    File.Delete(strPath);
                    strMessage = "File Deleted";
                }
                else
                {
                    strMessage = "File Not Found";
                }
            }
            catch (Exception ex)
            {
                strMessage = ex.Message;
            }
            return strMessage;
        }


        /// <summary>
        /// Get the path. if path is virtual, will return full server path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetPath(string path)
        {
            string result = string.Empty;
            if (Path.IsPathRooted(path))
            {
                result = path;
            }
            else
            {
                result = System.Web.Hosting.HostingEnvironment.MapPath(path);
            }
            if (!result.EndsWith("\\"))
            {
                result = String.Concat(result, "\\");
            }
            return result;
        }


        /// <summary>
        /// Deletes all files inside the folder
        /// </summary>
        /// <param name="target_dir"></param>
        public static void DeleteDirectory(string target_dir)
        {
            string[] files = Directory.GetFiles(target_dir);
            string[] dirs = Directory.GetDirectories(target_dir);

            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string dir in dirs)
            {
                DeleteDirectory(dir);
            }

            Directory.Delete(target_dir, false);
        }
    }
}