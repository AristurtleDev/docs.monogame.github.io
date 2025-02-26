/// <summary>
/// The ContentManager is a run-time component which loads managed objects from .xnb binary files produced by the
/// design time MonoGame Content Builder.  It also manages the lifespan of the loaded objects, disposing the
/// content manager will also dispose any assets which are themselves IDisposable.
/// </summary>
public partial class ContentManager : IDisposable
{
    /// <summary>
    /// Gets or Sets the root directory that this ContentManager will search for assets in.
    /// </summary>
    public string RootDirectory { get; set; }

    /// <summary>
    /// Gets the service provider instance used by this ContentManager.
    /// </summary>
    public IServiceProvider ServiceProvider { get; }

    /// <summary>
    /// Initializes a new instance of the ContentManager.
    /// </summary>
    /// <remarks>
    /// By default, the ContentManager searches for content in the directory where the executable is located.
    /// <param name="serviceProvider">The service provider that the ContentManager should use to locate services.</param>
    public ContentManager(IServiceProvider serviceProvider) { }

    /// <summary>
    /// Initializes a new instance of the ContentManager.
    /// </summary>
    /// <param name="serviceProvider">The service provider that the ContentManager should use to locate services.</param>
    /// <param name="rootDirectory">The root directory the ContentManager will search for content in.</param>
    public ContentManager(IServiceProvider serviceProvider, string rootDirectory) { }

    /// <summary>
    /// Loads an asset that has been processed by the Content Pipeline.
    /// </summary>
    /// </remarks>
    /// <typeparam name="T">The type of asset to load.</typeparam>
    /// <param name="assetName">
    /// The asset name, relative to the RootDirectory property, and not including the .xnb extension.
    /// </param>
    /// <returns>
    /// The loaded asset. Repeated calls to load the same asset will return the same object instance.
    /// </returns>
    public virtual T Load<T>(string assetName) { }

    /// <summary>
    /// Loads an asset that has been processed by the Content Pipeline.
    /// </summary>
    /// <remarks>
    /// This method attempts to load the asset based on the CultureInfo.CurrentCulture
    /// searching for the asset by name and appending it with with the culture name (e.g. "assetName.en-US")
    /// or two letter ISO language name (e.g. "assetName.en"). If unsuccessful in finding the asset with
    /// the culture information appended, it will fall back to loading the default asset.
    /// </remarks>
    /// <typeparam name="T">The type of asset to load.</typeparam>
    /// <param name="assetName">
    /// The asset name, relative to the RootDirectory property, and not including the .xnb extension.
    /// </param>
    /// <returns>
    /// The loaded asset. Repeated calls to load the same asset will return the same object instance.
    /// </returns>
    public virtual T LoadLocalized<T>(string assetName) { }

    /// <summary>
    /// Unloads all assets that were loaded by this ContentManger.
    /// </summary>
    /// <remarks>
    /// If an asset being unloaded implements the IDisposable interface, then the Dispose method of that asset will be
    /// called before unloading.
    /// </remarks>
    public virtual void Unload() { }

    /// <summary>
    /// Unloads a single asset that was loaded by this ContentManager.
    /// </summary>
    /// <remarks>
    /// If an asset being unloaded implements the IDisposable interface, then the Dispose method of that asset will be
    /// called before unloading.
    /// </remarks>
    /// <param name="assetName">
    /// The asset name, relative to the RootDirectory property, and not including the .xnb extension.
    /// </param>
    public virtual void UnloadAsset(string assetName) { }

    /// <summary>
    /// Unloads a set of assets loaded by this ContentManager where each element in the provided collection
    /// represents the name of an asset to unload.
    /// </summary>
    /// <remarks>
    /// If an assets being unloaded implements the IDisposable interface, then the Dispose method of that asset will be
    /// called before unloading.
    /// </remarks>
    /// <param name="assetNames">The collection containing the names of assets to unload.</param>
    public virtual void UnloadAssets(IList<string> assetNames) { }

    /// <summary>
    /// Disposes of this ContentManager.
    /// </summary>
    public void Dispose() { }
}
