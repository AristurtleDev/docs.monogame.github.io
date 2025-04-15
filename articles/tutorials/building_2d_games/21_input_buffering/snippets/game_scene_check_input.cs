private void CheckInput()
{
    // Check for pause action
    if (_controller.Pause())
    {
        _pauseMenu.IsEnabled = _pauseMenu.IsVisible = _pauseMenu.IsSelected = true;
        return;
    }

    // Store the potential direction change
    Vector2? potentialNextDirection = null;

    // Check movement actions
    if (_controller.MoveUp())
    {
        potentialNextDirection = -Vector2.UnitY;
    }
    else if (_controller.MoveDown())
    {
        potentialNextDirection = Vector2.UnitY;
    }
    else if (_controller.MoveLeft())
    {
        potentialNextDirection = -Vector2.UnitX;
    }
    else if (_controller.MoveRight())
    {
        potentialNextDirection = Vector2.UnitX;
    }

    // If a new direction was input, consider adding it to the buffer.
    if (potentialNextDirection.HasValue && _inputBuffer.Count < MAX_BUFFER_SIZE)
    {
        // If the buffer is empty, validate against the current direction;
        // otherwise, validate against the last buffered direction
        Vector2 validateAgainst = _inputBuffer.Count > 0 ?
                                    _inputBuffer.Last() :
                                    _slimes[0].Direction;

        // Check if this is a valid direction change (not reversed).
        if (Vector2.Dot(potentialNextDirection.Value, validateAgainst) >= 0)
        {
            // Only add if it is different from the las buffered direction.
            if (_inputBuffer.Count == 0 || _inputBuffer.Last() != potentialNextDirection.Value)
            {
                _inputBuffer.Enqueue(potentialNextDirection.Value);
            }
        }
    }
}