using RedMoonGames.Basics;
using UnityEngine;

namespace Game.Drag
{
    public interface IDraggable
    {
        TryResult TryStartDrag(Vector3 position);

        void StopDrag(Vector3 position, Vector3 startPosition);
    }
}
