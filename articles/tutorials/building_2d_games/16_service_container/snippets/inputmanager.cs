public class InputManager : GameComponent, IInputManager
{
    /// <summary>
    /// Initializes this input manager.
    /// </summary>
    public override void Initialize()
    {
        Keyboard = new KeyboardInfo();
        Mouse = new MouseInfo();

        GamePads = new GamePadInfo[4];
        for (int i = 0; i < 4; i++)
        {
            GamePads[i] = new GamePadInfo((PlayerIndex)i);
        }

        // Add this instance to the game's services container.
        Game.Services.AddService<IInputManager>(this);
    }

    // Rest of class implementation remains the same.
}
