namespace DX12Editor.Utilities
{
    public interface IUndo
    {
        string Name { get; }
        void Undo();
    }
}
