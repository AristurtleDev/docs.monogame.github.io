---
title: Adding Content
description: This tutorial will go over adding content such as images or sounds to your game.
---

> [!NOTE]
> For help with creating a project, please look at the [Creating a New Project](index.md) section of the Getting Started guide.

## MonoGame Content Builder Tool (MGCB Editor)

This tutorial assumes that you are using Visual Studio 2022 with its MonoGame extension installed. If you are not using the extension, you will need to manually install and use the [MGCB Editor](./tools/mgcb_editor.md).

> This is technically optional, since you can edit the .mgcb files manually if you wish, but the editor is highly recommended for ease of use.

## Adding content

First, you will need some content for your game. For this tutorial, use the following image of a ball:

![Open Content](images/ball.png)

Copy the image to your machine by using **right-click > Save Image As** and save it somewhere locally with the name “ball.png”.

Now open up your game project and look at the Solution Explorer window. Expand the **Content** folder and open up **Content.mgcb** file by double-clicking on it.

![Open Content](images/3_open_content.png)

You should now see the MGCB Editor window open up. If a text file opens instead, then right-click on **Content.mgcb** and select **Open With**, then select **MGCB Editor** in the list, click **Set as Default** and then click **OK**, then try again.

> If you do not see the **MGCB Editor** option when you right-click and select **Open With**, then please review the [Tools documentation](./tools/index.md) for installing the MGCB Editor tool for your operating system.

![MGCB Editor](images/3_mgcb_editor_tool.png)

Your game content is managed from this external tool. You can add content to your game in one of the following ways:

- **Add Existing Item** toolbar button
- **Edit > Add > Existing Item...** menu button
- **right-click > Add > Existing Item...** context menu

Make sure the "Content" MGCB file is selected to the left, then click the **Add Existing Item** toolbar button.

![Add Content](images/3_add_content.png)

You should now be prompted to select a file. Select the “**ball.png**” image that you downloaded a moment ago. Once you have confirmed your selection, you will be asked whether to copy the file, add a link to it, or skip it. Make sure "**Copy the file to the directory**" option is selected and click **Add**.

![Copy Content](images/3_copy_content.png)

Now click the **Save** toolbar button and close the MGCB Editor tool.

![Save Content](images/3_save_content.png)

---

## Adding the content in your game

Now that you have added the asset to the Content project, it is time to load it into your game. First, open up the **Game1.cs** class file and declare a new **ballTexture** variable of type **Texture2D** in the **Game1** class, so you can store the ball image into memory.

```csharp
public class Game1 : Game
{
    Texture2D ballTexture;

    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
```

Next, find the **LoadContent** method. Here, use **Content.Load()** function to load the "ball" sprite and store it in the **ballTexture** parameter. **Content.Load()** requires you to specify what type of content you are trying to load, in this case it is a **Texture2D**.

```csharp
protected override void LoadContent()
{
    // Create a new SpriteBatch, which can be used to draw textures.
    _spriteBatch = new SpriteBatch(GraphicsDevice);

    // TODO: use this.Content to load your game content here
    ballTexture = Content.Load<Texture2D>("ball");
}
```

Finally, find the Draw method to draw the ball onto the screen. This is done by:

- Opening a SpriteBatch (an image drawing collection function).

- Adding the images you want to draw and specifying where you want to draw them.

- Then finally closing the SpriteBatch to commit the textures you want drawn to the screen.

> **Note**: if you add multiple images, they will be drawn in the order you place them from back to front (each drawn on top of each other).

As shown below:

```csharp
protected override void Draw(GameTime gameTime)
{
    graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

    // TODO: Add your drawing code here
    _spriteBatch.Begin();
    _spriteBatch.Draw(ballTexture, new Vector2(0, 0), Color.White);
    _spriteBatch.End();

    base.Draw(gameTime);
}
```

Now run the game. You should get the following:

![Game](images/3_game.png)

**Next up:** [Adding Basic Code](5_adding_basic_code.md)
