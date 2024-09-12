using System.Numerics;
using System.Runtime.Serialization;
using ReactiveUI;

namespace FurnaceEditor.ViewModels.Components
{
    [DataContract]
    public class Transform : Component
    {
        private Vector3 _position;
        private Vector3 _rotation;
        private Vector3 _scale;

        public Vector3 Position
        {
            get { return _position; }
            set { this.RaiseAndSetIfChanged(ref _position, value); }
        }

        public Vector3 Rotation
        {
            get { return _rotation; }
            set { this.RaiseAndSetIfChanged(ref _rotation, value); }
        }

        public Vector3 Scale
        {
            get { return _scale; }
            set { this.RaiseAndSetIfChanged(ref _scale, value); }
        }
    }
}
