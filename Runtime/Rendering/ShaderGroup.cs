using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veldrid;

namespace Runtime.Rendering
{
    public sealed class ShaderGroup
    {
        public ShaderGroup(Shader[] shaders,in ResourceLayoutDescription[] resourceLayouts)
        {
            if(shaders == null)
            {
                Shaders = Array.Empty<Shader>();
            }
            else
            {
                Shaders = shaders;
            }

            if (resourceLayouts == null)
            {
                Layouts = Array.Empty<ResourceLayout>();
                Resources = new List<string>();
            }
            else
            {
                Layouts = new ResourceLayout[resourceLayouts.Length];
                Resources = new List<string>(resourceLayouts.Length);

                for (int i = 0; i < resourceLayouts.Length; i++)
                {
                    ResourceLayoutDescription desc = resourceLayouts[i];
                    Layouts[i] = Renderer.CreateResourceLayout(desc);
                    Resources.Add(desc.Elements[0].Name);
                }
            }
        }

        public Shader[] Shaders { get; private init; }
        public ResourceLayout[] Layouts { get; private init; }
        public List<string> Resources { get; private init; }
    }
}
