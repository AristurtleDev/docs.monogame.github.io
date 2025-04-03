#declaration
using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameLibrary.UI;

public class UIElement : IEnumerable<UIElement>
{

}
#endregion
{
    #region properties-state
    private List<UIElement> _children;
    private bool _isEnabled;
    private bool _isVisible;
    private bool _isSelected;
    private bool _wasSelectedThisFrame;
    private Color _enabledColor;
    private Color _disabledColor;

    /// <summary>
    /// Gets the ui element that is the parent of this ui element, or null of there is no parent.
    /// </summary>
    public UIElement Parent { get; private set; }

    /// <summary>
    /// Gets or Sets the position of this element.
    /// </summary>
    /// <remarks>
    /// If this element is a child element, this position is relative to the position of the parent.
    /// </remarks>
    public Vector2 Position { get; set; }

    /// <summary>
    /// Gets the position of this element relative to the game screen.
    /// </summary>
    public Vector2 AbsolutePosition
    {
        get
        {
            // If there is a parent element, return the sum of the parent
            // element's absolute position with this element's relative position.
            if (Parent is UIElement parent)
            {
                return parent.AbsolutePosition + Position;
            }

            // Otherwise, just return this element's position since it has no
            // parent to be relative to.
            return Position;
        }
    }

    /// <summary>
    /// Gets or Sets a value that indicates whether this UI ELement is enabled.
    /// </summary>
    /// <remarks>
    /// If this UI element is a child of another UI element, and that parent is
    /// not enabled, this this will return false.
    /// </remarks>
    public bool IsEnabled
    {
        get
        {
            // If there is a parent element, and if that parent element is not
            // enabled, then return false automatically since all children of
            // a disabled parent are also disabled.
            if (Parent is UIElement parent && !parent.IsEnabled)
            {
                return false;
            }

            // Otherwise, there is either no parent element or the parent element
            // is enabled, in which case we just return back the enabled state
            // of this element
            return _isEnabled;

        }
        set => _isEnabled = value;
    }

    /// <summary>
    /// Gets or Sets a value that indicates whether this UI element is visible.
    /// </summary>
    /// <remarks>
    /// If this UI element is a child of another UI element, and that parent is
    /// not visible, then this will return false.
    /// </remarks>
    public bool IsVisible
    {
        get
        {
            // If there is a parent element, and if that parent element is not
            // visible, then return false automatically since all children of
            // a non-visible parent are also not visible.
            if (Parent is UIElement parent && !parent.IsVisible)
            {
                return false;
            }

            // Otherwise, there is either no parent element or the parent element
            // is visible, in which case, we just return back the visual state
            // of this element
            return _isVisible;
        }
        set => _isVisible = value;
    }

    /// <summary>
    /// Gets or Sets the color mask to apply to this element and all its children
    //  when it is enabled.
    /// </summary>
    /// <remarks>
    /// Default value is Color.White.
    /// </remarks>
    public Color EnabledColor
    {
        get
        {
            // If there is a parent element, return back the enabled color of
            // the parent to maintain visual consistency with grouped element.
            if (Parent is UIElement parent)
            {
                return parent.EnabledColor;
            }

            // Otherwise, return the enabled color of this element
            return _enabledColor;
        }
        set => _enabledColor = value;
    }

    /// <summary>
    /// Gets or Sets the color mask to apply to this element and all its children
    //  when it is disabled.
    /// </summary>
    /// <remarks>
    /// Default value is Color.White.
    /// </remarks>
    public Color DisabledColor
    {
        get
        {
            // If there is a parent element, return back the disabled color of
            // the parent to maintain visual consistency with grouped element.
            if (Parent is UIElement parent)
            {
                return parent.DisabledColor;
            }

            // Otherwise, return the enabled color of this element
            return _disabledColor;
        }
        set => _disabledColor = value;
    }

    /// <summary>
    /// Gets or Sets a value that indicates if this ui element is currently selected.
    /// </summary>
    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            _isSelected = value;
            _wasSelectedThisFrame = value;
        }
    }
    #endregion

    #region properties-navigation
    /// <summary>
    /// Gets or Sets the UI Element controller used for navigation input for this UI element.
    /// </summary>
    public IUIElementController Controller { get; set; }

    /// <summary>
    /// Gets or Sets the action to perform when this ui element is selected and
    /// the up menu input is pressed.
    /// </summary>
    public Action UpAction { get; set; }

    /// <summary>
    /// Gets or Sets the action to perform when this ui element is selected and
    /// the down menu input is pressed.
    /// </summary>
    public Action DownAction { get; set; }

    /// <summary>
    /// Gets or Sets the action to perform when this ui element is selected and
    /// the left menu input is pressed.
    /// </summary>
    public Action LeftAction { get; set; }

    /// <summary>
    /// Gets or Sets the action to perform when this ui element is selected and
    /// the right menu input is pressed.
    /// </summary>
    public Action RightAction { get; set; }

    /// <summary>
    /// Gets or Sets the action to perform when this ui element is selected and
    /// the confirm menu input is pressed.
    /// </summary>
    public Action ConfirmAction { get; set; }

    /// <summary>
    /// Gets or Sets the action to perform when this ui element is selected and
    /// the cancel menu input in pressed.
    /// </summary>
    public Action CancelAction { get; set; }
    #endregion

    #region ctors
    /// <summary>
    /// Creates a new ui element with an optional parent.
    /// </summary>
    public UIElement()
    {
        _children = new List<UIElement>();
        IsEnabled = true;
        IsVisible = true;
        EnabledColor = Color.White;
        DisabledColor = Color.White;
    }
    #endregion

    #region methods
    /// <summary>
    /// Adds the given ui element as a child of this ui element.
    /// </summary>
    /// <param name="child">The ui element to add as a child of this ui element.</param>
    public T CreateChild<T>() where T : UIElement, new()
    {
        T child = new T();
        _children.Add(child);
        child.Parent = this;
        return child;
    }

    /// <summary>
    /// Updates this ui element.
    /// </summary>
    /// <param name="gameTime">A snapshot of the timing values for the current update cycle.</param>
    public virtual void Update(GameTime gameTime)
    {
        if(!IsEnabled)
        {
            return;
        }
        if (IsSelected && Controller != null && !_wasSelectedThisFrame)
        {
            HandleNavigation();
        }

        _wasSelectedThisFrame = false;

        foreach (UIElement child in _children)
        {
            child.Update(gameTime);
        }
    }

    private void HandleNavigation()
    {
        if (Controller.NavigateUp() && UpAction != null)
        {
            UpAction();
        }
        else if (Controller.NavigateDown() && DownAction != null)
        {
            DownAction();
        }
        else if (Controller.NavigateLeft() && LeftAction != null)
        {
            LeftAction();
        }
        else if (Controller.NavigateRight() && RightAction != null)
        {
            RightAction();
        }
        else if (Controller.Confirm() && ConfirmAction != null)
        {
            ConfirmAction();
        }
        else if (Controller.Cancel() && CancelAction != null)
        {
            CancelAction();
        }
    }

    /// <summary>
    /// Draws this ui element.
    /// </summary>
    /// <param name="spriteBatch">The sprite batch used to draw.</param>
    public virtual void Draw(SpriteBatch spriteBatch)
    {
        // Draw each child element of this element that is also a visual element.
        foreach (UIElement child in _children)
        {
            child.Draw(spriteBatch);
        }
    }

    /// <summary>
    /// Returns an enumerator that iterates through each child element in this ui element.
    /// </summary>
    /// <returns>An enumerator that iterates through each child element in this ui element.</returns>
    public IEnumerator<UIElement> GetEnumerator() => _children.GetEnumerator();

    /// <summary>
    /// Returns an enumerator that iterates through each child element in this ui element.
    /// </summary>
    /// <returns>An enumerator that iterates through each child element in this ui element.</returns>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    #endregion
}
