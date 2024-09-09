namespace FurnaceEditor.Utilities
{
    public interface IUndo
    {
        string Name { get; }
        void Undo();
    }
}
