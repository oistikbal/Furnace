using System.Collections.ObjectModel;
using FurnaceEditor.Utilities;

namespace FurnaceEditor.Services
{
    public class UndoService
    {
        private readonly ObservableCollection<IUndo> _undoList = new();

        public ReadOnlyObservableCollection<IUndo> UndoList { get; }

        public UndoService()
        {
            UndoList = new(_undoList);
        }

        public void Add(IUndo cmd)
        {
            _undoList.Add(cmd);
        }

        public void Undo()
        {
            if (_undoList.Any())
            {
                var cmd = _undoList.Last();
                _undoList.RemoveAt(_undoList.Count - 1);
                cmd.Undo();
            }
        }

        public void Reset()
        {
            _undoList.Clear();
        }

    }
}
