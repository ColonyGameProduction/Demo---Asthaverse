
using UnityEngine;

public class PlayableMinimapSymbolHandler : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    [SerializeField] private Color _playableColor, _friendColor;
    private void Awake() 
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void ChangeSymbolColorToPlayable()=> ChangeSymbolColor(_playableColor);
    public void ChangeSymbolColorToFriend()=> ChangeSymbolColor(_friendColor);
    private void ChangeSymbolColor(Color chosen)
    {
        _spriteRenderer.color = chosen;
    }
}
