public Game1()
{
    _graphics = new GraphicsDeviceManager(this);
    Content.RootDirectory = "Content";
    IsMouseVisible = true;

    // Create a new FramesPerSecondCounter.
    _fpsCounter = new FramesPerSecondCounter(this);

    // Add the FramesPerSecondCounter ot the game's component collection
    Components.Add(_fpsCounter);
}
