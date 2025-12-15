using Dino_Game_WFA.Utils;
using System.Collections.Immutable;

namespace Dino_Game_WFA.Resource
{
    public class ResourceHandler
    {
        public static ImmutableArray<Resource> Resources = ImmutableArray<Resource>.Empty;

        public static void LoadDefaultResources()
        {
            Resources = Resources.AddRange(new Resource[]
            {
                new Picture(Identifier.Of(Globals.NamespaceName, "dead"), "Resources/dead.png").Load(),
                new Picture(Identifier.Of(Globals.NamespaceName, "cactus"), "Resources/cactus.gif").Load(),
                new Picture(Identifier.Of(Globals.NamespaceName, "cactus_multi"), "Resources/cactus_multi.gif").Load(),
                new Picture(Identifier.Of(Globals.NamespaceName, "running"), "Resources/running.gif").Load(),
            });
        }

        public static Resource GetResource(Identifier id)
        {
            return Resources.FirstOrDefault(r => r.Id == id) 
                ?? throw new KeyNotFoundException($"Resource with ID {id.Namespace}:{id.Source} not found.");
        }
    }
}
