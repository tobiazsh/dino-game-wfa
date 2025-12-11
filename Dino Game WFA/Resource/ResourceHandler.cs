using Flappy_Birds_WFA.Utils;
using System.Collections.Immutable;

namespace Flappy_Birds_WFA.Resource
{
    public class ResourceHandler
    {
        public static ImmutableArray<Resource> Resources = ImmutableArray<Resource>.Empty;

        public static void LoadDefaultResources()
        {
            Resources = Resources.AddRange(new Resource[]
            {
                new Picture(Identifier.Of(Globals.NamespaceName, "bird"), "Resources/bird.png").Load(),
                new Picture(Identifier.Of(Globals.NamespaceName, "pipe"), "Resources/pipe.png").Load(),
                new Picture(Identifier.Of(Globals.NamespaceName, "ground"), "Resources/ground.png").Load()
            });
        }

        public static Resource GetResource(Identifier id)
        {
            return Resources.FirstOrDefault(r => r.Id == id) 
                ?? throw new KeyNotFoundException($"Resource with ID {id.Namespace}:{id.Source} not found.");
        }
    }
}
