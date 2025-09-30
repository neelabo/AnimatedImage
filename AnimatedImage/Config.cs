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
    }
}
