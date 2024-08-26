using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;

namespace DX12Editor.Models
{
    [DataContract()]
    public class Project
    {

        [DataMember]
        public string Name { get; private set; }
        public string Path { get; private set; }
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
                Serializers.Serializer.ToFile(new Project(path, name), projectFilePath);
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur during serialization
                Debug.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }
}
