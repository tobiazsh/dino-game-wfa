using Dino_Game_WFA.Exceptions;

namespace Dino_Game_WFA.Utils
{
    /// <summary>
    /// Represents a picture resource loaded from a file.
    /// </summary>
    /// <param name="id">Identifier. Make sure that each identifier is unique!</param>
    /// <param name="filePath">Path to the image</param>
    public class Picture(Identifier id, string filePath) : Resource.Resource(id)
    {
        private Bitmap? _bitmap;

        /// <summary>
        /// Loads the picture resource from the specified file path.
        /// </summary>
        /// <exception cref="ResourceLoadException">Thrown when unable to load image</exception>
        public Picture Load()
        {
            try
            {
                _bitmap = new Bitmap(filePath);
            }
            catch (Exception ex)
            {
                throw new ResourceNotFoundException($"Failed to load picture resource from path: {filePath}", ex);
            }

            return this;
        }

        public Bitmap? Bitmap
        {
            get => _bitmap;
        }

        public Size Size
        {
            get {
                if (_bitmap == null)
                {
                    throw new InvalidOperationException("Picture resource is not loaded.");
                }
                return _bitmap.Size;
            }
        }
    }
}
