using System.Runtime.Serialization;

namespace DX12Editor.Models
{
    [DataContract()]
    public class Project
    {
        public static string Extenion { get; } = ".dx12project";
        [DataMember]
        public string Name { get; set; }
        public string Path { get; private set; }
        public string FullPath => $"{Path}{Name}{Extenion}";
        [DataMember(Name = "Scenes")]
        private List<Scene> _scenes = new List<Scene>();

        public Project(string path, string name = "NewProject")
        {
            Path = path;
            Name = name;
        }
    }
}
