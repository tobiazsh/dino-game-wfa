namespace Flappy_Birds_WFA.Exceptions
{
    public class ResourceNotFoundException(string message, Exception? e) : FileNotFoundException(message, e)
    {

    }
}
