/// <summary>
/// Draws the FPS calculation to the window title.
/// </summary>
/// <param name="gameTime">A snapshot of the game's timing values.</param>
public override void Draw(GameTime gameTime)
{
    // Increment the frame counter only during draw.
    _frameCounter++;

    // Update the window title to show the frames per second.
    Game.Window.Title = $"FPS: {FramesPerSecond}";
}