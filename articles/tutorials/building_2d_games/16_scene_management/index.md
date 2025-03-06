---
title: "Chapter 16: Scene Management"
description: "Learn how to implement scene management to handle different game screens like menus, gameplay, and transitions between scenes."
---

In [Chapter 15](../15_service_container/index.md), we explored MonoGame's service container to create a modular game architecture where components can easily communicate with each other. While our current game has a single screen where the slime chases the bat, most games consist of multiple screens such as title screens, menus, gameplay screens, and more.  Breaking our game into these distinct scenes offers benefits.  Instead of one monolithic file handling everything, we can organize our code into focused, management modules.

In this chapter, you will:

- Learn what scenes are and why they're important in game development.
- Create a reusable scene management system.
- Implement different game screens including a title screen and gameplay screen.
- Handle transitions between different scenes.

Let's start by understanding what scenes are and why they're useful in game development.

## Understanding Scenes

In game development, a scene (sometimes called a screen or state) represents a distinct section of your game. Each scene typically has its own update and draw logic, as well as its own set of game objects. Common examples of scenes include:

- **Title Screen**: The first screen players see when launching your game.
- **Main Menu**: Where players can select game modes or access options.
- **Gameplay**: The main interactive portion of your game.
- **Pause Menu**: Displayed when the game is paused.
- **Game Over Screen**: Shown when the player loses.
- **Victory Screen**: Displayed when the player wins.

Scenes help organize your game's code by separating different parts of your game into self-contained modules. This makes your code more manageable as your game grows in complexity.

### Benefits of Using Scenes

Implementing a scene management system offers several advantages:

1. **Improved organization**: Each scene contains only the code and assets relevant to that part of the game.
2. **Memory management**: Load assets only when needed and unload them when leaving a scene.
3. **Simplified state handling**: Each scene maintains its own state without affecting others.
4. **Code reusability**: Create reusable scene templates for common game screens.

### Scene Management Workflow

A scene management system typically follows this workflow:

1. The game initializes a scene manager.
2. The manager loads the initial scene (usually a title or splash screen).
3. The game delegates update and draw calls to the active scene.
4. When transitioning to a new scene:
   - The current scene is unloaded or hidden.
   - The new scene is loaded and initialized.
   - The new scene becomes the active scene.

This workflow lets you navigate between different parts of your game while maintaining clean separation between them.

## Creating a Scene Management System

For the scene management system, we'll need two modules

- The `Scene` base class
- The `SceneManager` to manage updating/drawing the current scene and transitions from one scene to another.

### The Scene Base Class

First, let's create an abstract base class for scenes.  This class will provide common functionality for all scenes and allow us to create specific scene types by inheriting from it. For the most part, an individual scene is similar to the [**Game**](xref:Microsoft.Xna.Framework.Game) class in that it needs to initialize, load content, update, and draw, so we can use that as a base structure.

In the *MonoGameLibrary* project, create a new directory called *Scenes*. Inside this directory, create a new file named *Scene.cs* with the following code initial structure:

[!code-csharp[](./snippets/scene.cs#declaration)]

This declares the Scene class as an `abstract` class that implements [`IDisposable`](https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-dispose) to ensure proper resource cleanup. The abstract modifier indicates that this class cannot be instantiated directly but must be inherited by concrete scene implementations.

#### Scene Properties

Add the following properties to the `Scene` class:

[!code-csharp[](./snippets/scene.cs#properties)]

The `Scene` class provides several protected properties:

- `Game`: Provides access to the main game instance to access services and other game components.
- `Content`: A separate ContentManager for each scene, allowing scene-specific assets to be loaded and unloaded independently.
- `GraphicsDeviceManager`: Provides access to the graphics device manager for adjusting display settings.
- `GraphicsDevice`: Direct access to the graphics device for rendering operations.
- `IsDisposed`: Tracks whether the scene has been disposed, preventing multiple dispose calls.

> [!TIP]
> Having a dedicated [**ContentManager**](xref:Microsoft.Xna.Framework.Content.ContentManager) for each scene is particularly important as it allows us to easily unload all scene-specific content when transitioning between scenes, helping to manage memory efficiently.

#### Scene Constructor

Add the following constructor and finalizer to the `Scene` class:

[!code-csharp[](./snippets/scene.cs#ctors)]

The constructor performs several important setup steps:

- Stores the reference to the game instance.
- Retrieves the [**GraphicsDeviceManager**](xref:Microsoft.Xna.Framework.GraphicsDeviceManager) and [**GraphicsDevice**](xref:Microsoft.Xna.Framework.Graphics.GraphicsDevice) from the game's service container.
- Creates a new [**ContentManager**](xref:Microsoft.Xna.Framework.Content.ContentManager) specific to this scene.
- Sets the content root directory to match the game's content directory.

> [!NOTE]
> Finalizers are special methods that are called by the Garbage Collector when an object is cleaned up.  

#### Scene Methods

Add the following methods to the `Scene` class:

[!code-csharp[](./snippets/scene.cs#methods)]

The `Scene` class provides several methods that handle the scene's lifecycle:

- `Initialize`: Sets up the scene and calls LoadContent.
- `LoadContent`: Virtual method where derived scenes load their specific assets.
- `UnloadContent`: Unloads all content loaded through the scene's content manager.
- `Update`: Updates the scene's logic based on the current game time.
- `Draw`: Renders the scene using the provided sprite batch.
- `Dispose` and `Dispose(bool)`: Properly clean up scene resources, following the standard [.NET dispose pattern](https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-dispose).

### The ISceneManager Interface

Now that we have our base `Scene` class, we need a way to manage scenes and handle transitions between them. First, let's define an interface for our scene manager. This interface will specify the minimum functionality that any scene manager implementation must provide.

In the *Scenes* directory of the *MonoGameLibrary* project, create a new file named *ISceneManager.cs* with the following code:

[!code-csharp[](./snippets/iscenemanager.cs)]

This simple interface defines the essential functionality needed for scene management: the ability to change from one scene to another. By defining this as an interface, we can register it in the service container and access it throughout our game.

### The SceneManager Class

Now that we have our interface, let's implement the `SceneManager` class that will manage our scenes. Create a new file named *SceneManager.cs* in the *Scenes* directory with the following initial structure:

[!code-csharp[](./snippets/scenemanager.cs#declaration)]

The SceneManager class inherits from [**DrawableGameComponent**](xref:Microsoft.Xna.Framework.DrawableGameComponent), which means it will automatically have its `Update` and `Draw` methods called by the game loop. It also implements our `ISceneManager` interface, allowing it to be registered in the game's service container.

#### SceneManager Fields

Add the following private fields to the `SceneManager` class:

[!code-csharp[](./snippets/scenemanager.cs#fields)]

These fields track:

- `_graphics`: Graphics-related resources for rendering
- `_currentScene`: The currently active scene
- `_nextScene`: The next scene to transition to (if any)

The two-stage approach to scene changes ensures that transitions happen at a controlled time during the update cycle, preventing potential issues that could arise from changing scenes mid-update or mid-draw.

#### SceneManager Constructor

Add the following constructor to the `SceneManager` class:

[!code-csharp[](./snippets/scenemanager.cs#ctors)]

The constructor takes a reference to the game and passes it to the base [**DrawableGameComponent**](xref:Microsoft.Xna.Framework.DrawableGameComponent) constructor. It then registers itself as an `ISceneManager` service in the game's service container, making it accessible to other components that need to change scenes.

#### SceneManager Methods

Add the following public and protected methods to the SceneManager class:

[!code-csharp[](./snippets/scenemanager.cs#methods)]

These methods handle the core scene management functionality:

- `Initialize`: Sets up the graphics resources needed for scene rendering.
- `Update`: Checks for pending scene transitions and updates the current scene.
- `Draw`: Renders the current scene using the shared sprite batch.
- `ChangeScene`: Queues a scene change to occur on the next update cycle.
- `TransitionScene`: Handles the actual scene transition, disposing the old scene and initializing the new one.
- `Dispose`: Properly cleans up resources when the scene manager is disposed.

> [!NOTE]
> Forcing garbage collection with GC.Collect() in the `TransitionScene` method is generally not recommended in most applications, but can be useful during scene transitions in games to ensure memory is freed immediately rather than waiting for the garbage collector's normal schedule.

## Adding Scenes To Our Game

Now that we have a scene management system in place, let's start breaking our monolithic `Game1` into two different scenes

1. **Title Scene**: A simple scene that displays the title of the game and informs the player to press a key or button to start playing
1. **Game Scene**: The scene where the actual game will be played.

Since these scenes are specific for the the game we are creating, we'll be making them in the game project and not the library project.

### The Title Scene

### The Game Scene
