using UnityEngine;

public class CursorManager : MonoBehaviour
{
    [SerializeField] private Texture2D cursorTexture;

    void Start()
    {
        Cursor.SetCursor(cursorTexture, new Vector2(7.5f, 7.5f), CursorMode.Auto);
    }


}
