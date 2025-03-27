#region declaration
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
    #region properties
    private List<UIElement> _children;
    private bool _enabled;
    private bool _visible;

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
    public Point Position { get; set; }

    /// <summary>
    /// Gets the position of this element relative to the game screen.
    /// </summary>
    public Point AbsolutePosition
    {
        get
        {
            if (Parent == null)
            {
                return Position;
            }

            return Parent.AbsolutePosition + Position;
        }
    }

    /// <summary>
    /// Gets or Sets a value that indicates whether this ui element is enabled.
    /// </summary>
    /// <remarks>
    /// If this ui element isa child of another ui element, and the parent is not enabled, this will return false.
    /// </remarks>
    public bool Enabled
    {
        get
        {
            // If there is no parent element or if there is and the parent is
            // enabled, return this elements enabled value.
            if (Parent == null || Parent.Enabled)
            {
                return _enabled;
            }

            // otherwise, there is a parent, and the parent is disabled, so
            // all of the parent's children are also disabled.
            return false;

        }
        set => _enabled = value;
    }

    /// <summary>
    /// Gets or Sets a value that indicates whether this ui element is visible.
    /// </summary>
    /// <remarks>
    /// If this ui element is a child of another ui element, and the parent is not visible, this will return false.
    /// </remarks>
    public bool Visible
    {
        get
        {
            // If there is no parent element or if there is and the parent is
            // visible, return this element's visible value.
            if (Parent == null || Parent.Visible)
            {
                return _visible;
            }

            // otherwise, there is a parent, and the parent is not visible, so
            // all of the parent's children are also invisible.
            return false;
        }
        set => _visible = value;
    }
    #endregion

    #region ctors
    /// <summary>
    /// Creates a new ui element with an optional parent.
    /// </summary>
    /// <param name="parent">(Optional) The ui element that is the parent of this ui element.</param>
    public UIElement(UIElement parent = null)
    {
        _children = new List<UIElement>();
        Enabled = true;
        Visible = true;

        if(parent != null)
        {
            parent.AddChild(this);
        }
    }
    #endregion

    #region child-management
    /// <summary>
    /// Adds the given ui element as a child of this ui element.
    /// </summary>
    /// <param name="child">The ui element to add as a child of this ui element.</param>
    public void AddChild(UIElement child)
    {
        // If the child has a parent, remove it
        if (child.Parent != null)
        {
            child.Parent.RemoveChild(child);
        }

        // Add it to this element's child collection
        _children.Add(child);

        // Set the parent of the child to this element
        child.Parent = this;
    }

    /// <summary>
    /// Removes the given ui element from the children of this ui element.
    /// </summary>
    /// <param name="child">The child element to remove.</param>
    public void RemoveChild(UIElement child)
    {
        // Remove the child from this element's child collection, and if it
        // successful, orphan the child.
        if (_children.Remove(child))
        {
            child.Parent = null;
        }
    }
    #endregion

    #region lifecycle
    /// <summary>
    /// Updates this ui element.
    /// </summary>
    /// <param name="gameTime">A snapshot of the timing values for the current update cycle.</param>
    public virtual void Update(GameTime gameTime)
    {
        // Update each child of this element.
        foreach (UIElement child in _children)
        {
            child.Update(gameTime);
        }
    }

    /// <summary>
    /// Draws this ui element.
    /// </summary>
    /// <param name="spriteBatch">The sprite batch used for drawing.</param>
    public virtual void Draw(SpriteBatch spriteBatch)
    {
        // Draw each child of this element
        foreach (UIElement child in _children)
        {
            child.Draw(spriteBatch);
        }

    }
    #endregion

    #region ienumerable
    /// <summary>
    /// Returns an enumerator that iterates through each child element in this ui element.
    /// </summary>
    /// <returns>An enumerator that iterates through each child element in this ui element.</returns>
    public IEnumerator<UIElement> GetEnumerator()
    {
        foreach (UIElement child in _children)
        {
            yield return child;
        }
    }

    /// <summary>
    /// Returns an enumerator that iterates through each child element in this ui element.
    /// </summary>
    /// <returns>An enumerator that iterates through each child element in this ui element.</returns>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    #endregion
}