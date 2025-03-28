---
title: "Chapter 19: User Interface"
description: "Learn how to implement a user interface system in MonoGame, including creating reusable UI components and building interactive game menus"
---

A critical component of any game is the user interface (UI) that allows players to interact with the game beyond just controlling the the character.  UI elements include menus, buttons, panels, labels, and various other interactive components that provide information and control options to the player.

In this chapter you will

- Learn the basics of user interface design in games.
- Create reusable UI components for the MonoGameLibrary.
- Understand the parent-child relationship for UI elements.
- Implement a UI component hierarchy for flexible layouts.
- Build an options menu for our game using the UI system.

Let's start by understanding what a user interface is and how it functions in game development.

## Understanding Game User Interfaces

A user interface in games serves as the bridge between the player and the game's systems.  Well designed UIs help players navigate the games's mechanics, understand their current status, and make informed decisions. For new game developers, understanding UI principles is crucial because even the most mechanically sound game can fail if players can't effectively interact with it.

Game UIs consist of various visual elements that serve different purposes:

1. **Information Display**: Elements like health bars, score counters, or minimap displays provide players with game state information.  These elements help players understand their progress, resources, and current status without interrupting gameplay.
2. **Interactive Controls**: Buttons, sliders, checkboxes, and other interactive elements allow players to make choices, adjust settings, or navigate through different sections of the game.  These elements should provide clear visual feedback when interacted with to confirm the player's actions.
3. **Feedback Mechanisms**: Visual effects like highlighting, color changes, or animations that respond to player actions help confirm that input was received.  This feedback loop creates an intuitive and responsive feel for the UI in your game.

User interfaces for games can be categorized into two main types, each with its own design considerations

- **Diegetic UI**: These elements exist within the game world itself and are often part of the narrative.  Examples include a health meter integrated into a character's suit, ammunition displayed on a weapon's holographic sight, or the dashboard instruments the cockpit of a racing game.  Diegetic UI can enhance immersion by making interface elements feel like natural parts of the game world.
- **Non-diegetic UI**: These elements exist outside the game world, overlaid on top of the gameplay.  Traditional menus, health bars int he corner of the screen, and score displays are common examples.  While less immersive than diegetic UI, non-diegetic elements are often clearer and easier to read.

For our game project, we'll focus on creating non-diegetic UI elements, specifically menu screens that allow players to navigate between different parts of the game and adjust settings.  This approach provides a solid foundation for understanding UI concepts that you can later expand upon in more complex games.

### UI Layout Systems

WHen designing and implementing game UI systems, developers must decide how UI elements will be positioned on the screen. Two primary approaches exist, each with distinct advantages and trade-offs;

1. **Absolute Positioning**:  In this approach, each Ui element is placed at specific coordinates on the screen.  Elements are positioned using exact locations, which gives precise control over the layout.  This approach is straightforward to implement and works well for static layouts where elements don't need to adjust based on screen size or content changes.  The main disadvantage of absolute positioning is its lack of flexibility.  If the screen resolution changes or if an element's size changes, manual adjustments to positions are often necessary to maintain the desired layout.

2. **Layout engines**: These system position UI elements relative to one another using rules and constraints.  Elements might be positioned using concepts like "center", "align to parent", or "flow horizontally with spacing".  Layout engines add complexity but provide flexibility.  The advantage of layout engines is adaptability to different screen sizes and content changes.  However, they require more initial setup and can be more complex to implement from scratch.

For our implementation we'll take a middle ground approach.  We'll primarily use absolute positioning for simplicity but will build a parent-child relationship system that provides some of the flexibility found in layout engines.  This hybrid approach gives us reasonable control without adding a lot of complexity.

Child elements will be positioned relative to their parent's position, forming a hierarchial structure.  When a parent element moves, all its children move with it, maintaining their relative positions.  This approach simplifies the management of grouped elements without requiring a fully layout engine.

### Parent-Child Relationships

Parent-child relationships are part of many UI system, including the one we'll build in this chapter.  In this model, UI elements can contain other UI elements, creating a tree-like structure. This hierarchial approach mirrors how interface elements naturally group together in designs.

For example, a settings panel might contain multiple buttons, labels, and sliders.  By making these elements children of the panel, they can be managed as a cohesive unit.  This organizational structure provides several significant advantages:

- **Inheritance of Properties**: Child elements can automatically inherit certain properties from their parents.  For instance, if a parent element is hidden or disabled, all its children can be hidden or disabled as well. This cascading behavior simplifies state management across complex interfaces.
- **Relative Positioning**: Child elements can be positioned relative to their parents rather than relative to the screen.  This means you can place elements within a contain and then move the entire container as a unit without having to update ach child's position individually.
- **Simplified State Management**:  Actions on parent elements can automatically propagate to their children.  For example, disabling a menu panel can automatically disable all buttons within it, preventing interaction with elements that should be active.
- **Batch Operations**: Operations like drawing and updating can be performed on a parent element and automatically cascade to all children, reducing hte need for repetitive code.
- **Logical Grouping**: The hierarchy naturally models the conceptual grouping of UI elements, making the code structure more intuitive and easier to maintain.

## Creating a UI System

With an understanding of hte core concepts behind game user interfaces, let's build our own UI system for MonoGame.  WE'll implement a set of reusable classes that can be extended and combined to create various UI elements for our game.

Our UI system will consist of three primary classes that build upon each other

1. `UIElement`: The base class that handles the parent-child relationship, positioning, and visibility and enabled states.
2. `UISprite`: Extends the `UIElement` to include visual representation using our existing `Sprite` class.
3. `UIButton` Extends `UISprite` to add interactivity and selection states.

This hierarchial design allows each class to focus on a specific aspect of UI functionality while building on the capabilities of its parent class.

### The UIElement Class

The `UIElement` class is the foundation of our UI system.  It provides core functionality that all UI components need, including:

- Managing parent-child relationships.
- Handling visibility and enabled states.
- Calculating absolute position from relative positions.
- Providing update and draw lifecycle methods.

To get started, in the *MonoGameLibrary* project:

1. Create a new directory named *UI*.
2. Add a new class file named *UIElement.cs* to the *UI* directory you just created.
3. Add the following code as the initial structure for the class:

    [!code-csharp[](./snippets/uielement.cs#declaration)]

    > [!NOTE]
    > The `UIElement` class implements the [`IEnumerable<T>`](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1?view=net-9.0) interface.  Using this interface provides an enumerator for the `UIElement` class that we can use to iterate each of the child elements without directly exposing the internal collection.

#### UIElement Field and Properties

The `UIElement` class will need fields and properties to track the parent-child relationship between elements, if the element is enabled or visible, and the position of the element.  Add the following fields and properties:

[!code-csharp[](./snippets/uielement.cs#properties)]

Let's take a look at some of the key properties here and their significance:

- `Position`: This stores the elements position relative to its parent. For root elements (those without a parent), this is equivalent to the screen position.  This is the property that will be used to adjust the position of the element.
- `AbsolutePosition`: This calculates the elements absolute position on the screen by combining its relative position with the absolute position of its parent.  This property is what will be used when drawing the element.
- `Enabled`: Determines whether the element can be interacted with. The getter checks if the parent is enabled first because if the parent is disabled, then all children of it should effectively disabled as well.
- `Visible`: Determines whether the element should be drawn. Similar to `Enabled`, the getter checks if the parent is visible first because if the parent is not visible, then all children of it should not be visible either.

#### UIElement Constructor

The `UIElement` constructor will take an optional `UIElement` parameter that will automatically set that as the parent of the one being created.  Add the following constructor:

[!code-csharp[](./snippets/uielement.cs#ctors)]

#### UIElement Parent-Child Management

The `UIElement` class will need methods to help manage the parent-child relationship between elements by allowing the addition and removal of elements.  Add the following methods:

[!code-csharp[](./snippets/uielement.cs#child-management)]

When a child is added to a `UIElement`, a check is made first to see if that child already has a parent, and if so removes it first from that parent before adding it to this element. A similar check is made when removing a child from a `UIElement`.  If the removal is successful, meaning it was indeed a child, only then is the parent of the child set to null.

##### UIElement Lifecycle Management

The `UIElement` class will need methods to manage the lifecycle of updating and drawing.  Add the following methods:

[!code-csharp[](./snippets/uielement.cs#lifecycle)]

These methods are marked as `virtual` so derived classes can override them to add their own behavior will still being able to call the base implementation to ensure children are updated and drawn.

### The UISprite Class

The `UISprite` class extends the `UIElement` class to add a visual component using the previously created `Sprite` class in our library.  It will handle

- Drawing the sprite at the elements position.
- Color tinting the sprite based on enabled and disabled states
- Propagation of color settings to child elements

In the *UI* directory of the *MonoGameLibrary* project, create a new class file named *UISprite.cs* and add the following code as the initial structure:

[!code-csharp[](./snippets/uisprite.cs#declaration)]

#### UISprite Fields and Properties

The `UISprite` class needs fields and properties to track the color tint to use based on the enabled state, as well as the `Sprite` to draw.  Add the following fields and properties:

[!code-csharp[](./snippets/uisprite.cs#properties)]

When a `UISprite` is the parent element to other `UISprites` and the `EnabledColor` or `DisabledColor` property changes, the value is propagated to the child `UiSprites` to ensure visual consistency within the UI group.

#### UISprite Constructors

The `UISprite` class provides constructors that accept a `Sprite` and an optional parent element.  Add the following constructors:

[!code-csharp[](./snippets/uisprite.cs#ctors)]

These constructors initialize the sprite with the default [**Color.White**](xref:Microsoft.Xna.Framework.Color.White) value for both the enabled and disabled states, which can be customized after creation if needed.

#### UISprite Methods

The `UISprite` class overrides the `Update` and `Draw` methods to handle the sprite specific functionality, and adds a `CenterOrigin` method for convenient sprite positioning.  Add the following methods:

[!code-csharp[](./snippets/uielement.cs#methods)]

Since an `AnimatedSprite` is a derived class from `Sprite`, the `Update` method here performs special handling to determine if the sprite given is an animated one, and automatically updates it if so.  This allows us to use both animated and non-animated sprites for this element.  The `Draw` method applies the appropriate color tint to the sprite before drawing it based on the enabled stated.  Both `Update` and `Draw` call into the their respective base methods at the end to ensure that the base `UIElement` class handles updating and drawing any child elements.

### The UIButton Class

The `UIButton` class extends the `UISprite` class to create interactive buttons.  It handles two visual states; normal and selected.  It also automatically handles switching between sprites based on the selection state.

In the *UI* directory of the *MonoGameLibrary* project, create a new class file named *UIButton.cs* and add the following code as the initial structure:

[!code-csharp[](./snippets/uibutton.cs#declaration)]

#### UIButton Fields and Properties

The `UIButton` class needs fields to track the two different sprites that represent the different states, and a property to track whether the button is currently selected or not.  Add the following fields and properties:

[!code-csharp[](./snippets/uibutton.cs#properties)]

To allow the button to change its visual state based on its selected stated, the `IsSelected` property, when set, will adjust the base `Sprite` property to reference either the selected or normal sprite based on the value being set.

#### UIButton Constructors

The `UIButton` class provides constructors that accept both normal and selected state sprites, with an optional parent element.  Add the following constructors:

[!code-csharp[](./snippets/uibutton.cs#ctors)]

#### UIButton Methods

The `UIButton` class overrides the `CenterOrigin` method from the base `UISprite` class to handle its two sprite nature, ensuring that both sprites are centered.  Add the following method:

[!code-csharp[](./snippets/uibutton.cs#methods)]
