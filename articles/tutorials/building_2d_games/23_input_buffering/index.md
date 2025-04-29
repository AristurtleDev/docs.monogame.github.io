---
title: "Chapter 21: Input Buffering"
description: "Learn how to implement input buffering for more responsive controls."
---

In the [previous chapter](../20_the_game/index.md) we completed our Dungeon Slime game with class snake-like mechanics.  While the game is now playable, you might have noticed an issue with the inputs.  In games where movement updates happen at fixed intervals that are less frequent than input polling, inputs can sometimes feel unresponsive, especially when trying to make multiple inputs in succession.

For instance, if a player wants to navigate a tight corner by pressing up and then immediately left, pressing these keys in rapid succession often results in only the second input being registered, causing the slime to not move as expected.  This happens because the second input overwrites the first one before the ame has a chance to process it, leading ot frustrating gameplay.

In this chapter, you will:

- Learn what input buffering is and why it is essential for responsive gameplay.
- Understand how to implement a queue-based input buffer system.
- Modify the game's input handling to store and process inputs in sequence.

Let's start by understanding input buffering.

## Understanding Input Buffering

Input buffering is a technique used in game development to temporarily store player inputs that cannot be immediately processed. Instead of discarding these inputs, they are placed in a queue and processed in order when the game is ready to handle them.

Games like snake often update movement at fixed intervals rather than continuously. This creates a disconnect between when the player presses a button (which is checked every frame) and when the game can actually respond to that input (which happens on a less frequent movement cycle). Without input buffering:

- Players must time their inputs perfectly to align with the game's update cycle.
- Rapid inputs are lost because only the last input is remembered.
- The game feels unresponsive and frustrating, especially during quick turns.

A well-implemented input buffer gives players a more forgiving and responsive experience by:

1. Storing inputs that arrive between movement updates.
2. Preserving the order of inputs for more predictable behavior.
3. Creating a sense that the game is actually listening to the player.

The size of an input buffer is an important design decision. If it is too small, players still might feel the game isn't responsive enough during complex sequences. If it is too large, the game might feel like it is playing itself as it works through the backlog of buffered commands. For a snake-style game, a buffer size of two is typically ideal:

- It allows for quick two-direction turns (like up-then-left to navigate a corner).
- It's small enough that players don't accidentally queue up too many moves.
- It keeps the game challenging while removing most frustration with input.

## Implementing an Input Buffer

Now that we understand why input buffering is important, let's implement it in our game. We'll create a queue-based system that stores the last two directional inputs from the player.

Our implementation will require three main changes to the existing code:

1. Adding new fields to the `GameScene` class to store our buffer.
2. Updating how we process input in the `CheckInput` method.
3. Modifying how we apply movement in the `UpdateSlimeMovement` method.

Let's start by modifying the `GameScene` class to include an input buffer. Open the *GameScene.cs* file and add the following fields:

[!code-csharp[](./snippets/game_scene_fields.cs)]

> [!NOTE]
> The [`Queue<T>`](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.queue-1?view=net-9.0>) is a First In, First Out (FIFO) collection in C#. When you add items with `Enqueue()`, they join the end of the line, and when you retrieve items with `Dequeue()`, you always get the oldest item (the one at the front of the line). Think of it like people waiting in line - the first person to arrive is the first one served.

This queue will store the directional vectors (up, down, left, right) that we will apply to the slime's movement in the order they were received.

Next, we need to initialize this queue in the `InitializeNewGame` method:

[!code-csharp[](./snippets/game_scene_initialize_new_game.cs?highlight=30-31)]

### Updating the CheckInput Method

Next, we need to update the `CheckInput` method.  Instead of immediately overwriting our next direction, we'll add valid input directions to the queue.  Update `CheckInput` to the following:

[!code-csharp[](./snippets/game_scene_check_input.cs?highlight=11,31-49)]

The key changes made here are:

1. The `potentialNewDirection` is now a nullable [**Vector2**](xref:Microsoft.Xna.Framework.Vector2) value.  We start it as null now so that we're not always enqueuing the already stored next direction every update.
2. A check is made to see if there is a new direction that has been input and if the buffer is not full.
3. If a new direction is input and the buffer is not full:
   1. The validation is made using [**Vector2.Dot**](xref:Microsoft.Xna.Framework.Vector2.Dot(Microsoft.Xna.Framework.Vector2,Microsoft.Xna.Framework.Vector2)) just like before to ensure it is a valid direction
   2. A final check is made to ensure that the direction being queued to the input buffer isn't the same direction as one that is already queued.  No need to queue the same direction twice.

### Updating the UpdateSlimeMovement Method

Finally, we need to modify how we apply the movement direction during the movement update cycle.  Make the following changes to the start of the `UpdateSlimeMovement` method:

```cs
 private void UpdateSlimeMovement()
{
    // Get the next direction from the input buffer if one is available
    if (_inputBuffer.Count > 0)
    {
        _nextDirection = _inputBuffer.Dequeue();
    }

    // Rest of existing implementation for this method doesn't change....
}
```

The key change here is that we now dequeue a direction from the input buffer rather than directly using the `_nextDirection` value. This ensures we process inputs in the order they were received, preserving the player's intent.

## Testing the Input Buffer

With these changes in place, our game now supports input buffering.  Let's see how it improves the gameplay experience, particularly for making tight turns.

| ![Figure 21-1: Gameplay demonstration showing improved responsiveness with input buffering, particularly when making tight turns](./videos/gameplay.webm) |
| :-------------------------------------------------------------------------------------------------------------------------------------------------------: |
|            **Figure 21-1: Gameplay demonstration showing improved responsiveness with input buffering, particularly when making tight turns**             |

1. When navigating a corner, players can now quickly press up followed by left (or any other combination), and both inputs will be respected.
2. The game feels more responsive since it remembers inputs between movement update cycles.
3. Complex maneuvers are easier to execute since timing is more forgiving.

The difference might seem subtle, but it significantly reduces frustration during gameplay, especially as the slime chain grows longer and navigation becomes more challenging.

## Conclusion

Let's review what you accomplished in this chapter:

- Learned about input buffering and how it addresses the timing disconnect between input polling and movement updates.
- Implemented a queue-based input buffer with a capacity of two inputs.
- Modified the input handling to store directional commands.
- Updated the movement system to process buffered input in sequence.
- Improved the game's responsiveness for quick turns and navigation.

This small additional enhances how the game feels to play, particularly when making rapid directional changes.  As the slime grows longer and navigation becomes more challenging, responsive controls become important to maintain a fair and enjoyable experience.

## Test Your Knowledge

1. What problem does input buffering solve in grid-based movement games?

    :::question-answer
    Input buffering solves the timing disconnect between when players press buttons and when the game can actually process those inputs in games with fixed movement cycles.  Without buffering, inputs that occur between movement cycles are lost, especially when players make rapid sequential inputs.  This makes the game feel unresponsive and frustrating.
    :::

2. Why did we choose a buffer size of two for our input queue?

    :::question-answer
    A buffer size of two provides an ideal balance for a snake-style game: it's large enough to allow complex two-direction turns (like up-then-left), but small enough that players don't accidentally queue too many moves ahead.  This keeps the game responsive to player intent while maintaining challenge and preventing the game from feeling like it is playing itself with too many buffered commands.
    :::

3. In what order are buffered inputs processed, and why is this important?

    :::question-answer
    Buffered inputs are processed in a First In, First Out (FIFO) order using a queue data structure.  This is important because it preserves the sequence of player inputs, ensuring the game responds in teh order the player intended.  Processing inputs int he correct sequence makes the control feel natural and predictable, especially during complex maneuvers where the timing and order of directional changes matter.
    :::
