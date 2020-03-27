using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PixlSpriter.Undo
{
    public class ChangePartUndo : IUndoable
    {
        byte[] pre;
        byte[] post;

        EditorImageLayer changedLayer;

        Int32Rect part;

        public ChangePartUndo(EditorImageLayer changingLayer, Int32Rect changingdPart, Action continueWith)
        {
            MemoryStream preStream = new MemoryStream();
            MemoryStream postStream = new MemoryStream();

            part = changingdPart;
            changedLayer = changingLayer;
            using (DeflateStream compress = new DeflateStream(preStream, CompressionMode.Compress))
            {
                byte[] arr = changingLayer.pixlmap.GetRectangle(part);
                compress.Write(arr, 0, arr.Length);
            }
            continueWith?.Invoke();
            using (DeflateStream compress = new DeflateStream(postStream, CompressionMode.Compress))
            {
                byte[] arr = changingLayer.pixlmap.GetRectangle(part);
                compress.Write(arr, 0, arr.Length);
            }

            pre = preStream.ToArray();
            post = postStream.ToArray();
        }

        public void redo(EditorContext context)
        {
            MemoryStream postStream = new MemoryStream(post);
            MemoryStream output = new MemoryStream();
            using (DeflateStream restore = new DeflateStream(postStream, CompressionMode.Decompress))
            {
                restore.CopyTo(output);
            }
            byte[] map = output.ToArray();
            changedLayer.pixlmap.SetRectangle(map, part);
            changedLayer.image = changedLayer.pixlmap.GetImage(context);
            output.Close();
        }

        public void undo(EditorContext context)
        {
            MemoryStream preStream = new MemoryStream(pre);
            MemoryStream output = new MemoryStream();
            using (DeflateStream restore = new DeflateStream(preStream, CompressionMode.Decompress))
            {
                restore.CopyTo(output);
            }
            byte[] map = output.ToArray();
            changedLayer.pixlmap.SetRectangle(map, part);
            changedLayer.image = changedLayer.pixlmap.GetImage(context);
            output.Close();
        }
    }
}
