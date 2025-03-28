#region declaration
using Microsoft.Xna.Framework;
using MonoGameLibrary.Graphics;

namespace MonoGameLibrary.UI;

public class UIButton : UISprite
{

}
#endregion
{
    #region properties
    private Sprite _normalSprite;
    private Sprite _selectedSprite;
    private bool _isSelected;

    /// <summary>
    /// Gets or Sets a value that indicates whether this ui button is selected.
    /// </summary>
    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (_isSelected == value)
            {
                return;
            }

            _isSelected = value;

            // Change the base sprite based on the new selected value.
            Sprite = _isSelected ? _selectedSprite : _normalSprite;
        }
    }
    #endregion

    #region ctors
    /// <summary>
    /// Creates a new ui button.
    /// </summary>
    /// <param name="normalSprite">The sprite to use when the button is not selected.</param>
    /// <param name="selectedSprite">The sprite to use when the button is selected.</param>
    public UIButton(Sprite normalSprite, Sprite selectedSprite)
        : base(normalSprite)
    {
        _normalSprite = normalSprite;
        _selectedSprite = selectedSprite;
    }

    /// <summary>
    /// Creates a new ui button as a child of the given ui element.
    /// </summary>
    /// <param name="parent">The ui element to set as the parent of this ui button.</param>
    /// <param name="normalSprite">The sprite to use when the button is not selected.</param>
    /// <param name="selectedSprite">The sprite to use when the button is selected.</param>
    public UIButton(UIElement parent, Sprite normalSprite, Sprite selectedSprite)
        : base(parent, normalSprite)
    {
        _normalSprite = normalSprite;
        _selectedSprite = selectedSprite;
    }
    #endregion

    #region methods
    /// <summary>
    /// Centers the origin of the normal and selected sprites for this ui button.
    /// </summary>
    public override void CenterOrigin()
    {
        _normalSprite.CenterOrigin();
        _selectedSprite.CenterOrigin();
    }
    #endregion
}