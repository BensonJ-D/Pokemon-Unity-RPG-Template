using UnityEngine;
using UnityEngine.UI;

namespace Menu
{
    public class MenuCursor : MonoBehaviour
    {
        [SerializeField] public Image pointer;
        [SerializeField] public Vector2 offset;

        public void SetPosition(float x, float y)
        {
            var newPosition = new Vector2(x, y);
            newPosition.x += offset.x;
            newPosition.y += offset.y;
        
            transform.localPosition = newPosition;
        }
    }
}
