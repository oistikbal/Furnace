using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DX12Editor.Models
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
