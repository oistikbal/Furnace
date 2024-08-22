using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DX12Editor.Models
{
    [DataContract]
    public class Project
    {
        public static string Extenion { get; } = ".dx12";
        public string Name { get; set; }
        public string Path { get; private set; }
        public string FullPath => $"{Path}{Name}{Extenion}";
    }
}
