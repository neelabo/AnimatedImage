namespace AnimatedImage
{
    /// <summary>
    /// AnimatedImage configuration.
    /// </summary>
    public static class Config
    {
        /// <summary>
        /// Native library folder path. If null, the default path is used.
        /// </summary>
        public static string? NativeLibraryPath { get; private set; }

        /// <summary>
        /// Changes the path to the native library folder.
        /// </summary>
        public static void SetNativeLibraryPath(string llibraryPath)
        {
            NativeLibraryPath = llibraryPath;
        }

        /// <summary>
        /// Determines whether the current environment supports WebP image rendering.
        /// </summary>
        /// <returns>true if WebP image rendering is supported; otherwise, false.</returns>
        public static bool CheckWebpSupport()
        {
            return Formats.WebpRenderer.CheckSupport();
        }
    }
}
