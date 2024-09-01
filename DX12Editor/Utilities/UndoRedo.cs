using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DX12Editor.Utilities
{
    interface IUndoRedo
    {
        string Name { get; }
        void Undo();
        void Redo();
    }

    class UndoRedo
    {
        private readonly ObservableCollection<IUndoRedo> _redoList =  new();
        private readonly ObservableCollection<IUndoRedo> _undoList =  new();

        public ReadOnlyObservableCollection<IUndoRedo> RedoList { get; }
        public ReadOnlyObservableCollection<IUndoRedo> UndoList { get; }

        public UndoRedo() 
        {
            RedoList = new(_redoList);
            UndoList = new(_undoList);
        }

        public void Add(IUndoRedo cmd)
        {
            _undoList.Add(cmd);
            _redoList.Clear();
        }

        public void Undo()
        {
            if (_undoList.Any())
            {
                var cmd = _undoList.Last();
                _undoList.RemoveAt(_undoList.Count -1);
                cmd.Undo();
                _redoList.Insert(0, cmd);
            }
        }

        public void Redo()
        {
            if (_redoList.Any())
            {
                var cmd = _redoList.First();
                _redoList.RemoveAt(0);
                cmd.Redo();
                _undoList.Add(cmd);
            }
        }

        public void Reset()
        {
            _redoList.Clear();
            _undoList.Clear();
        }
    }
}
