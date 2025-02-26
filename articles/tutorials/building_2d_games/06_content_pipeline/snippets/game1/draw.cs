protected override void Draw(GameTime gameTime)
{
    GraphicsDevice.Clear(Color.CornflowerBlue);

    // Draw the MonoGame logo texture using the spritebatch
    _spriteBatch.Begin();
    _spriteBatch.Draw(_logo, Vector2.Zero, Color.White);
    _spriteBatch.End();

    base.Draw(gameTime);
}
