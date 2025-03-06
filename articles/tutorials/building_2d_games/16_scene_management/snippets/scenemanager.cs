#region declaration
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameLibrary.Scenes;

public class SceneManager : DrawableGameComponent, ISceneManager
{

}
#endregion declaration

#region fields
// Reference to the graphics device manager used to manage the presentation of graphics.
private GraphicsDeviceManager _graphics;

// Reference to the graphics device used for rendering.
private GraphicsDevice _graphicsDevice;

// The sprite batch used to draw the scenes.
private SpriteBatch _spriteBatch;

// Tracks the current active scene to be updated and drawn.
private Scene _currentScene;

// Tracks the next scene to switch to if there is one to switch to;
// otherwise, will be null.
private Scene _nextScene;
#endregion

#region ctors
/// <summary>
/// Creates a new SceneManager instance.
/// </summary>
/// <param name="game">The game this scene manager belongs to.</param>
public SceneManager(Game game) : base(game)
{
    // Add this scene manager to the game's service container.
    game.Services.AddService<ISceneManager>(this);
}
#endregion

#region methods
/// <summary>
/// Initialize this scene manager.
/// </summary>
public override void Initialize()
{
    // Get the GraphicsDeviceManager from the game's services container
    _graphics = (GraphicsDeviceManager)Game.Services.GetService<IGraphicsDeviceManager>();

    // Get the GraphicsDevice from the game's services container
    _graphicsDevice = (GraphicsDevice)Game.Services.GetService<IGraphicsDeviceService>();

    // Create the sprite batch that will be used to render scenes
    _spriteBatch = new SpriteBatch(_graphicsDevice);
}

/// <summary>
/// Updates this scene manager.
/// </summary>
/// <param name="gameTime">A snapshot of the timing values for the current frame.</param>
public override void Update(GameTime gameTime)
{
    // Check if there is a next scene to transition too
    if (_nextScene != null)
    {
        TransitionScene();
    }

    // If there is a current scene, update it
    if (_currentScene != null)
    {
        _currentScene.Update(gameTime);
    }
}

/// <summary>
/// Draws this scene.
/// </summary>
/// <param name="gameTime">A snapshot of the timing values for the current frame.</param>
public override void Draw(GameTime gameTime)
{
    // If there is a current scene, draw it
    if (_currentScene != null)
    {
        _currentScene.Draw(_spriteBatch, gameTime);
    }
}

/// <summary>
/// Informs the scene manager to change from the current scene to the scene specified.
/// </summary>
/// <param name="scene">The scene to change to.</param>
public void ChangeScene(Scene scene)
{
    // Only set the next scene if it is not the same instance as the
    // current scene
    if (ReferenceEquals(scene, _currentScene))
    {
        _nextScene = scene;
    }
}

private void TransitionScene()
{
    // If there is a current scene, dispose of it
    if (_currentScene != null)
    {
        _currentScene.Dispose();
    }

    // Perform a garbage clean up
    GC.Collect();

    // Swap the current scene with the next scene
    _currentScene = _nextScene;
    _nextScene = null;

    // Initialize the new scene
    _currentScene.Initialize();
}

protected override void Dispose(bool disposing)
{
    if (disposing)
    {
        if (_currentScene != null)
        {
            _currentScene.Dispose();
        }

        if (_nextScene != null)
        {
            _nextScene.Dispose();
        }
    }

    base.Dispose(disposing);
}
#endregion