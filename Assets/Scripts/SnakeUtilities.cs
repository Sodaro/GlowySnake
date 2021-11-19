using UnityEngine;

namespace SnakeUtilities
{
    public enum Direction { UP = 0, RIGHT = 90, DOWN = 180, LEFT = 270 };
    public enum ContentType { NONE, SNAKE, APPLE, WALL };
    public struct GridTile
    {
        public GridTile(Vector3 position, ContentType contentType, GameObject content)
        {
            this.position = position;
            this.contentType = contentType;
            this.content = content;
        }
        public Vector3 position;
        public ContentType contentType;
        public GameObject content;
    }
}
