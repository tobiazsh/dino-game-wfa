namespace Dino_Game_WFA.Exceptions
{
    public class ResourceNotFoundException(string message, Exception? e) : FileNotFoundException(message, e)
    {

    }
}
