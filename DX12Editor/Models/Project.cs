using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using DX12Editor.Utilities.Serializers;

namespace DX12Editor.Models
{
    [DataContract]
    public class Project
    {

        [DataMember]
        public string Name { get; private set; }
        public string Path { get; set; }
        public string FullPath => $"{Path}{Name}{Extenion}";


        public static string Extenion { get; } = ".dx12project";

        public Project(string path, string name)
        {
            Path = path;
            Name = name;
        }

        public static void CreateProject(string path, string name)
        {
            // Combine paths using Path.Combine for better cross-platform compatibility
            string projectDirectory = System.IO.Path.Combine(path, name);

            // Create the directory if it does not exist
            if (!Directory.Exists(projectDirectory))
            {
                Directory.CreateDirectory(projectDirectory);
            }

            // Serialize the project to a file
            string projectFilePath = System.IO.Path.Combine(projectDirectory, $"{name}{Extenion}");

            try
            {
                // Assuming Serializer.ToFile takes an instance of Project and a file path
                Serializer.ToFile(new Project(path, name), projectFilePath);
                CreateGitIgnoreFile(projectDirectory);

            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur during serialization
                Debug.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        private static void CreateGitIgnoreFile(string path)
        {

            var assembly = Assembly.GetExecutingAssembly();
            var resourceNames = assembly.GetManifestResourceNames();

            foreach (var resourceName in resourceNames)
            {
                if (resourceName.EndsWith(".gitignore"))
                {
                    using (Stream resourceStream = assembly.GetManifestResourceStream(resourceName))
                    {
                        if (resourceStream != null)
                        {
                            string gitignoreFilePath = System.IO.Path.Combine(path, ".gitignore");
                            using (FileStream fileStream = new FileStream(gitignoreFilePath, FileMode.Create, FileAccess.Write))
                            {
                                resourceStream.CopyTo(fileStream);
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
}
