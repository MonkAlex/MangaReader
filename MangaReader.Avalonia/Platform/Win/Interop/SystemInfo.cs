namespace MangaReader.Avalonia.Platform.Win.Interop
{
    /// <summary>
    /// This class is a helper for system information, currently to get the DPI factors
    /// </summary>
    public static class SystemInfo
    {
        private static readonly Point DpiFactors;

        static SystemInfo()
        {
#warning Fill DpiFactors
        }

        /// <summary>
        /// Returns the DPI X Factor
        /// </summary>
        public static double DpiFactorX => DpiFactors.X;

        /// <summary>
        /// Returns the DPI Y Factor
        /// </summary>
        public static double DpiFactorY => DpiFactors.Y;
    }
}
