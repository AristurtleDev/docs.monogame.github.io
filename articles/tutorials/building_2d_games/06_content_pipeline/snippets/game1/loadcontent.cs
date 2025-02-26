protected override void LoadContent()
{
    _spriteBatch = new SpriteBatch(GraphicsDevice);

    // Load the MonoGame logo asset using the ContentManager
    _logo = Content.Load<Texture2D>("images/logo");
}