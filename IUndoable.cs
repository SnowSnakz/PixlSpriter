using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixlSpriter
{
    public interface IUndoable
    {
        void undo(EditorContext context);
        void redo(EditorContext context);
    }
}
