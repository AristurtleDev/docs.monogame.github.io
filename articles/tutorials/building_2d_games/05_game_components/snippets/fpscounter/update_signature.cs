/// <summary>
/// Updates the FPS calculation based on elapsed game time.
/// </summary>
/// <param name="gameTime">A snapshot of the game's timing values.</param>
public override void Update(GameTime gameTime)
{
    _elapsedTime += gameTime.ElapsedGameTime;

    if (_elapsedTime > s_oneSecond)
    {
        FramesPerSecond = _frameCounter;
        _frameCounter = 0;
        _elapsedTime -= s_oneSecond;
    }
}