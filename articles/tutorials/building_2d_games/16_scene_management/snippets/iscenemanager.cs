namespace MonoGameLibrary.Scenes;

public interface ISceneManager
{
    /// <summary>
    /// Informs the scene manager to change from the current scene to the scene specified.
    /// </summary>
    /// <param name="scene">The scene to change to.</param>
    void ChangeScene(Scene scene);
}