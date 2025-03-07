---
title: "Chapter 16: The MonoGame Services Container"
description: "Learn how to use MonoGame's Service Container to create a modular game architecture, enabling components to easily find and communicate with each other."
---

In [Chapter 14](../14_soundeffects_and_music/index.md), we added audio feedback to our game by implementing sound effects and background music. These audio features, along with our input handling from earlier chapters, are implemented as reusable game components.

As our game grows more complex, we'll need to manage numerous systems and share resources across different parts of code.  Currently, we are storing direct references to our components, which creates tight coupling between our game and the specific implementations of these systems.  In the following chapters, we are going to start breaking the game up into different modules and will need to share resources between them.  To do this, a more flexible approach would be use to use MonoGame's built-in service system to make these components available throughout the game.

In this chapter, you will:

- Learn about the concept of dependency injection and service locators.
- Understand how MonoGame's service container works.
- Create interfaces for our existing manager components.
- Register components as services in the game.
- Access services from other components.
- Update our existing managers to use the service container.

Let's start by understanding what services are and why they're useful in game development.

## Understanding Services

In software development, a service is a reusable module that provides specific functionality to other modules. Using services helps separate concerns in your code and creates a more modular architecture. Instead of tightly coupling modules together through direct references, services allow modules to communicate through well-defined interfaces.

The MonoGame service container implements the service locator pattern, which provides a centralized registry where modules can:

1. Register themselves as service providers.
2. Look up and consume services provided by other modules.

This pattern is particularly useful in games where you have many systems that need to interact with each other, such as:

- Input systems that need to be accessed by UI, player controls, and debug features.
- Audio systems that might be triggered by gameplay events, UI interactions, or scripted sequences.
- Physics systems that might be referenced by rendering, gameplay, or debugging code.

Using services also offers several benefits:

- **Reduced coupling**: Services don't need direct references to each other.
- **Global Access**: Access resources from anywhere without making them actual global variables.
- **Improved Organization**: Clear separation between service providers and consumers.
- **More flexible architecture**: You can change implementations without modifying consuming code.

### Game Components vs Services

In MonoGame, it's important to understand the distinction between game components and services, as they serve different but complementary purposes:

**Game Components** (like [**GameComponent**](xref:Microsoft.Xna.Framework.GameComponent) and [**DrawableGameComponent**](xref:xref:Microsoft.Xna.Framework.DrawableGameComponent)) are:

- Modular pieces of game functionality that can be added to a game.
- Automatically updated and/or drawn each frame through the game loop.
- Managed by the game's component collection.
- Typically responsible for a specific system's behavior (input handling, audio, physics, etc.).

**Services**, on the other hand, are:

- References to functionality that can be accessed throughout your game.
- Registered with and retrieved from the game's services container.
- Not automatically updated (unless they're also game components).
- A way to share resources across different parts of your code.

While these are different concepts, they often work together in practice. Many game systems are implemented as game components that also register themselves as services.

### When to Use Game Components vs Services

Use **Game Components** when you need:

- Functionality that requires regular updates each frame.
- Self-contained systems with their own update and/or draw logic.
- Organization of different game subsystems.

Use **Services** when you need:

- Global access to a resource or system.
- To decouple components that need to interact.
- To share references without passing them explicitly through constructors.

### When to Register a Game Component as a Service

Not all game components need to be services, and not all services need to be game components. Here are some guidelines:

**Register a game component as a service when:**

- Multiple other components or systems need to access its functionality.
- You want the flexibility to swap implementations later.
- The component provides functionality that's useful throughout your game.

**Keep a game component without registering it as a service when:**

- It's self-contained and doesn't need to be accessed by other components.
- It only affects a specific part of your game.
- It doesn't provide functionality that other components need to use.

## The Game Services Property

At the core of MonoGame's service system is the [**Game.Services**](xref:Microsoft.Xna.Framework.Game.Services) property, which provides access to the game's [**GameServiceContainer**](xref:Microsoft.Xna.Framework.GameServiceContainer). This container has two primary methods:

1. [**AddService**](xref:Microsoft.Xna.Framework.GameServiceContainer.AddService(System.Type,System.Object)): Registers a service with the container
2. [**GetService**](xref:Microsoft.Xna.Framework.GameServiceContainer.GetService(System.Type)): Retrieves a service from the container.

### Adding a Service

When adding a service using the [**AddService**](xref:Microsoft.Xna.Framework.GameServiceContainer.AddService(System.Type,System.Object)) method, you need to specify:

1. The service type (usually an interface).
2. The service implementation (usually a concrete class).

While you can register services by their concrete type, it's considered best practice to register them by their interface instead. This allows you to swap implementations without changing the consuming code.

For example, you might register an `ExampleService` like this:

```cs
// Define an interface
public interface IExampleService
{
    void DoSomething();
    // Interface members ...
}

// Create a concrete implementation of the interface.
public class ExampleService : GameComponent, IExampleService
{
    // Implementation. ...
}

// Register it as a service
Game.Services.AddService<IExampleService>(new ExampleService(Game));
```

### Retrieving a Service

Once a service has been registered, you can retrieve it from anywhere that has access to the [**Game.Services**](xref:Microsoft.Xna.Framework.Game.Services) property using the [**GetService**](xref:Microsoft.Xna.Framework.GameServiceContainer.GetService(System.Type)) method.

For example:

```cs
// Retrieve the service from the container in another component somewhere else.
IExampleService exampleService = Game.Services.GetService<IExampleService>();

// Ensure that the service was returned before using it.
if(exampleService != null)
{
    exampleService.DoSomething();
}

```

> [!TIP]
> If the requested service has not been registered, [**GetService**](xref:Microsoft.Xna.Framework.GameServiceContainer.GetService(System.Type)) returns `null`, so it is good practice to check for `null` before using a returned service.

## When to Register Services

Services are typically registered:

1. During the initialization of a component.
2. In the game's constructor or [**Initialize**](xref:Microsoft.Xna.Framework.Game.Initialize) method.
3. When the component is added to the game.

For game components, a common pattern is to register the service in the constructor:

```cs
public class ExampleService : GameComponent, IExampleService
{
    public ExampleService(Game game) : base(game)
    {
        // Register this instance of the service in the game's services container.
        game.Services.AddService<IExampleService>(this);
    }

    // Implementation ....
}
```

This ensures the service is available as soon as the component is created.

## Accessing Services

There are two common approaches to accessing services in game components, each with different advantages:

### Direct Access via Game.Services

Components can access services directly through the [**Game.Services**](xref:Microsoft.Xna.Framework.Game.Services) property:

```cs
public class ExampleComponent : GameComponent
{
    // Field to store the service reference in.
    private IExampleService _exampleService;

    public ExampleComponent(Game game) : base(game) { }

    public override void Initialize()
    {
        base.Initialize();

        // Retrieve the service.
        _exampleService = Game.Services.GetService<IExampleService>();
    }

    public override void Update(GameTime gameTime)
    {
        // Use the service
        _exampleService.DoSomething();
    }
}
```

**Advantages of Direct Access**:

- Simpler setup - components only need the game reference.
- Services can be accessed whenever needed, even if they weren't available at construction time.
- Code is more concise with fewer constructor parameters.

**Disadvantages**:

- Creates "hidden dependencies" that aren't obvious from the component's interface.

### Constructor Injection

The alternative approach is to pass required services directly through the constructor:

```cs
public class ExampleComponent : GameComponent
{
    // Field to store the service reference in.
    private IExampleService _exampleService;

    public ExampleComponent(Game game, IExampleService exampleService) 
        : base(game) 
    {
        _exampleService = exampleService;
    }

    public override void Update(GameTime gameTime)
    {
        // Use the service
        _exampleService.DoSomething();
    }
}
```

When using constructor injection, you can still use the services container to get the services when creating the component:

```cs
ExampleComponent exampleComponent = new ExampleComponent(
    game,
    game.Services.GetService<IExampleService>()
);
```

**Advantages of Constructor Injection**:

- Explicit dependencies - The constructor clearly shows what services the component requires.

**Disadvantages**:

- More verbose constructor code, especially for components with many dependencies.
- All dependencies must be available when the component is constructed.

### When to Choose Each Approach

Choose Direct Access when:

- You need a simpler setup with fewer dependencies.
- Services might not be available when the component is constructed.

Choose Constructor Injection when:

- You want to make dependencies explicit and obvious.
- You want to ensure all required services are available immediately.

## Adding Services to Our Game

Let's take the existing game components we've created, the `InputManager`, and the `AudioManager`, and update them to services in our game. First, we'll need to define interfaces for the services.  This allows us to follow best practices by registering services by their interface.

### The IInputManager Interface

Create a new file in name *IInputManager.cs* in the *Input* directory of the *MonoGameLibrary* project.  Add the following code to the file:

```cs
using Microsoft.Xna.Framework;

namespace MonoGameLibrary.Input;

/// <summary>
/// Interface for the game's input manager that handles keyboard, mouse, and gamepad input.
/// </summary>
public interface IInputManager
{
    /// <summary>
    /// Gets the keyboard info component for checking keyboard state.
    /// </summary>
    KeyboardInfo Keyboard { get; }

    /// <summary>
    /// Gets the mouse info component for checking mouse state.
    /// </summary>
    MouseInfo Mouse { get; }

    /// <summary>
    /// Gets an array of gamepad info components, one for each player.
    /// </summary>
    GamePadInfo[] GamePads { get; }

    /// <summary>
    /// Updates the input states.
    /// </summary>
    /// <param name="gameTime">Time passed since the last update.</param>
    void Update(GameTime gameTime);
}
```

### The IAudioManager Interface

Create a new file in name *IAudioManager.cs* in the *Audio* directory of the *MonoGameLibrary* project.  Add the following code to the file:

```cs
using Microsoft.Xna.Framework;

namespace MonoGameLibrary.Audio
{
    /// <summary>
    /// Interface for the game's audio manager that handles sound effects and music.
    /// </summary>
    public interface IAudioManager
    {
        /// <summary>
        /// Gets whether audio is currently muted.
        /// </summary>
        bool IsMuted { get; }
        
        /// <summary>
        /// Adds a sound effect to the audio manager.
        /// </summary>
        /// <param name="assetName">The content asset name of the sound effect.</param>
        void AddSoundEffect(string assetName);
        
        /// <summary>
        /// Adds a song to the audio manager.
        /// </summary>
        /// <param name="assetName">The content asset name of the song.</param>
        void AddSong(string assetName);
        
        /// <summary>
        /// Plays a sound effect.
        /// </summary>
        /// <param name="assetName">The asset name of the sound effect to play.</param>
        /// <returns>The sound effect instance that's playing.</returns>
        Microsoft.Xna.Framework.Audio.SoundEffectInstance PlaySoundEffect(string assetName);
        
        /// <summary>
        /// Plays a sound effect with custom parameters.
        /// </summary>
        /// <param name="assetName">The asset name of the sound effect to play.</param>
        /// <param name="volume">The volume to play at (0.0f to 1.0f).</param>
        /// <param name="pitch">The pitch adjustment (-1.0f to 1.0f).</param>
        /// <param name="pan">The stereo panning (-1.0f for left to 1.0f for right).</param>
        /// <param name="isLooped">Whether the sound should loop.</param>
        /// <returns>The sound effect instance that's playing.</returns>
        Microsoft.Xna.Framework.Audio.SoundEffectInstance PlaySoundEffect(string assetName, float volume, float pitch, float pan, bool isLooped);
        
        /// <summary>
        /// Plays a song.
        /// </summary>
        /// <param name="assetName">The asset name of the song to play.</param>
        void PlaySong(string assetName);
        
        /// <summary>
        /// Pauses all currently playing audio.
        /// </summary>
        void PauseAudio();
        
        /// <summary>
        /// Resumes all paused audio.
        /// </summary>
        void ResumeAudio();
        
        /// <summary>
        /// Mutes all audio.
        /// </summary>
        void MuteAudio();
        
        /// <summary>
        /// Unmutes all audio, restoring previous volume levels.
        /// </summary>
        void UnmuteAudio();
        
        /// <summary>
        /// Toggles between muted and unmuted states.
        /// </summary>
        void ToggleMute();
        
        /// <summary>
        /// Increases the volume of all audio by the specified amount.
        /// </summary>
        /// <param name="amount">The amount to increase (0.0f to 1.0f).</param>
        void IncreaseVolume(float amount = 0.1f);
        
        /// <summary>
        /// Decreases the volume of all audio by the specified amount.
        /// </summary>
        /// <param name="amount">The amount to decrease (0.0f to 1.0f).</param>
        void DecreaseVolume(float amount = 0.1f);
    }
}
```

### Updating the InputManager Class

Next, open the *InputManager.cs* file and make the following changes:

[!code-csharp[](./snippets/inputmanager.cs?highlight=1,17-18)]

The key changes made here are:

1. The `InputManager` class now implements the `IInputManager` interface in the class signature.
2. The `Initialize` method was updated to add the input manager to the game's services container once it has been initialized.

### Updating the AudioManager Class

Next, open the *AudioManager.cs* file and make the following changes:

[!code-csharp[](./snippets/audiomanager.cs?highlight=1,11-12)]

The key changes made here are:

1. The `AudioManager` class now implements the `IAudioManager` interface in the class signature.
2. The `Initialize` method was updated to add the audio manager to the game's services container once it has been created.

## Conclusion

We aren't making any obvious changes to the *Game1.cs* file in this chapter.  However, with the changes we've made, the `InputManager` and `AudioManager` are now registered as services which will be important for the up coming chapters.

By converting our game components into services, we've established a foundation for a more modular and flexible game architecture. This separation of concerns through the service container will allow us to:

- Access our input and audio systems from anywhere in the game.
- More easily swap implementations if needed.
- Create new game components that can find and use existing services.
- Prepare for the more complex scenes and game states we'll implement in future chapters.

The service container pattern helps games scale in complexity without becoming unmanageable. As we add more features and systems to our game, this architecture will help keep our code organized and maintainable.

In the next chapter, we'll build on this foundation to create a scene management system that will allow us to organize our game into different screens, such as a title screen, game screen, and pause screen.

## Test Your Knowledge

1. What is the difference between a game component and a service in MonoGame?

    :::question-answer
    A game component is a modular piece of game functionality that is automatically updated and/or drawn each frame through the game loop. A service is a reference to functionality that can be accessed throughout your game via the service container. Game components focus on "how things work" while services focus on "finding things." A class can be both a game component and a service at the same time.
    :::

2. What are the two primary methods of the [**GameServiceContainer**](xref:Microsoft.Xna.Framework.GameServiceContainer) and what do they do?

    :::question-answer
    The two primary methods are:

    - [**AddService**](xref:Microsoft.Xna.Framework.GameServiceContainer.AddService``1(``0)): Registers a service with the container
    - [**GetService**](xref:Microsoft.Xna.Framework.GameServiceContainer.GetService``1): Retrieves a service from the container
    :::

3. Why is it considered best practice to register services by their interface rather than their concrete type?

    :::question-answer
    Registering services by their interface rather than concrete type allows you to swap implementations without changing consuming code. This provides more flexibility in your architecture and makes your code more maintainable.
    :::

4. When should you register a game component as a service?

    :::question-answer
    You should register a game component as a service when:

    - Multiple other components or systems need to access its functionality
    - You want the flexibility to swap implementations later
    - The component provides functionality that's useful throughout your game
    - You want to decouple the component's implementation from code that uses it
    :::

5. What should you check when retrieving a service from the service container?

    :::question-answer
    When retrieving a service, you should check if the returned value is `null` before using it. This is because the [**GetService**](xref:Microsoft.Xna.Framework.GameServiceContainer.GetService``1) method returns `null` if the requested service has not been registered with the container.
    :::