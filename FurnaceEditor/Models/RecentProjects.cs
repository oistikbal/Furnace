using System.Runtime.Serialization;

namespace FurnaceEditor.Models
{
    [DataContract]
    public class RecentProject
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Path { get; set; }
        [DataMember]
        public DateTime LastOpened { get; set; }
    }

    [DataContract]
    public class RecentProjects
    {
        [DataMember]
        List<RecentProject> _recentProjects;
    }
}
