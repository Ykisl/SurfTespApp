using RedMoonGames.Basics;
using UnityEngine;

namespace Game.Drag
{
    public interface IDragTarget
    {
        public bool IsValdDraggable(IDraggable draggable);
        public void DropDraggable(IDraggable draggable);
    }
}
