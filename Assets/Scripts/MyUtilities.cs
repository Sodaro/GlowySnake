using UnityEngine;

namespace MyUtilities
{
    public enum Direction { UP = 0, RIGHT = 90, DOWN = 180, LEFT = 270 };
    public struct GridTile
    {
        public Vector3 position;
        public GameObject content;
    }
}
