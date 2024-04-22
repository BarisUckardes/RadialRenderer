using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tespia.Core.Rendering.Shaders
{
    public static class BatchlessSpriteShader
    {
        public const string VertexShader =@"
        #version 450

        layout(location = 0) in vec2 Position;
        layout(location = 1) in vec2 Uv;
        
        layout(location = 0) out vec2 UvOut;
        
        layout(set = 0,binding = 0) uniform TransformBuffer
        {
            mat4 Mvp;
        };
        void main()
        {
            gl_Position = Mvp*vec4(Position,0,1);
            UvOut = Uv;
        }
    ";

        public const string FragmentShader = @"
        #version 450

        layout(location = 0) in vec2 Uv;
   
        layout(location = 0) out vec4 ColorOUt;


        layout(set = 1,binding = 0) uniform sampler SpriteSampler;
        layout(set = 2,binding = 0) uniform texture2D SpriteTexture;

        void main()
        {
            ColorOUt = texture(sampler2D(SpriteTexture,SpriteSampler),Uv);
        }
    ";


    }
}
