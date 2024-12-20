using System.IO;
using System.Reflection;
using System.Runtime;
using System.Security.AccessControl;
using System.Security.Principal;

namespace Otus.Files
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var path1 = "C:\\Otus\\TestDir1";
            var path2 = "C:\\Otus\\TestDir2";
            LaunchProgram(path1);
            LaunchProgram(path2);
        }

        public static void LaunchProgram(string path)
        {
            var dir = CreateDirectory(path);
            CreateFilesInDir(dir);
            WriteNameToFile(dir);
            AddDateToFile(dir);
            ReadFiles(dir);
        }

        private static void ReadFiles(DirectoryInfo directory)
        {
            var files = directory.GetFiles();
            foreach (var file in files)
            {
                using var reader = new StreamReader(file.FullName);
                var text = reader.ReadToEnd();
                Console.WriteLine($"File name: {file.Name}. File content: {text}");
            }
        }

        private static void AddDateToFile(DirectoryInfo directory)
        {
            var files = directory.GetFiles();
            foreach (var file in files)
            {
                using var writer = new StreamWriter(file.FullName, true, System.Text.Encoding.UTF8);
                writer.WriteLine(DateTime.Now.ToString());
            }
        }

        private static void WriteNameToFile(DirectoryInfo directory)
        {
            var files = directory.GetFiles();
            foreach (var file in files)
            {
                try
                {
                    SetRules(directory);
                    using var writer = new StreamWriter(file.FullName, false, System.Text.Encoding.UTF8);
                    writer.WriteLine(file.Name);
                }
                catch (IOException e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        private static void SetRules(DirectoryInfo directory)
        {
            var sid = new SecurityIdentifier(WellKnownSidType.AuthenticatedUserSid, null);
            var myDirectorySecurity = directory.GetAccessControl();
            myDirectorySecurity.AddAccessRule(new FileSystemAccessRule(sid, FileSystemRights.FullControl, AccessControlType.Deny));
            directory.SetAccessControl(myDirectorySecurity);
        }

        private static DirectoryInfo CreateDirectory(string path)
        {
            var dirInfo = new DirectoryInfo(path);
            if (!dirInfo.Exists)
            {
                dirInfo.Create();
            }
            return dirInfo;
        }

        private static void CreateFilesInDir(DirectoryInfo directory)
        {
            for (var i = 0; i < 10; i++)
            {
                var path = directory.FullName + "\\" + $"File{i + 1}.txt";
                if (!File.Exists(path))
                {
                    var file = File.Create(path);
                    file.Close();
                }
            }
        }
    }
}
