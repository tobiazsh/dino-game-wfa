namespace Flappy_Birds_WFA.Utils
{

    /// <summary>
    /// Used to identify resources uniquely
    /// </summary>
    public record Identifier (string Namespace, string Source)
    {
        public static Identifier Of(string ns, string src) => new Identifier(ns, src);
    }
}
