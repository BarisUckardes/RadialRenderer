using Runtime.Rendering;
using Runtime.Windowing;
using System.Numerics;
using Tespia.Core.Rendering.Shaders;
using Vulkan;

namespace Demo
{
    internal class Program
    {
        private struct VertexData
        {
            public Vector2 Position;
            public Vector2 Uv;
        }
        static void Main(string[] args)
        {
            //Create window
            WindowDesc windowDesc = new WindowDesc()
            {
                Title = "RadianRenderer",
                InitialState = Veldrid.WindowState.Normal,
                Offset = new System.Numerics.Vector2(100, 100),
                Size = new System.Numerics.Vector2(1024, 1024)
            };

            Window window = new Window(windowDesc);

            //Create renderer
            Renderer renderer = new Renderer(window);

            //Create 2D scene
            Veldrid.Texture texture0 = GraphicsUtils.LoadTexture("D:\\Resources\\Textures\\SceneAspectIcon.png");
            Veldrid.Shader[] shaderSet = Renderer.CompileVertexFragmentShader(BatchlessSpriteShader.VertexShader,BatchlessSpriteShader.FragmentShader);
            ReadOnlySpan<VertexData> vertexes = stackalloc VertexData[4]
            {
                new VertexData()
                {
                    Position = new Vector2(-1.0f,1.0f),
                    Uv = new Vector2(0,0)
                },
                new VertexData()
                {
                    Position = new Vector2(1.0f,1.0f),
                    Uv = new Vector2(1,0)
                },
                new VertexData()
                {
                    Position = new Vector2(1.0f,-1.0f),
                    Uv = new Vector2(1,1)
                },
                new VertexData()
                {
                    Position = new Vector2(-1.0f,-1.0f),
                    Uv = new Vector2(0,1)
                }
            };
            Veldrid.DeviceBuffer vertexBuffer = Renderer.AllocateBuffer(new Veldrid.BufferDescription()
            {
                RawBuffer = false,
                SizeInBytes = (uint)(16 * vertexes.Length),
                StructureByteStride = 0,
                Usage = Veldrid.BufferUsage.VertexBuffer
            });
            Renderer.UpdateBuffer(vertexBuffer, vertexes, 0);
            ReadOnlySpan<ushort> indexes = stackalloc ushort[]
            {
                0,1,2,0,2,3
            };
            Veldrid.DeviceBuffer indexBuffer = Renderer.AllocateBuffer(new Veldrid.BufferDescription()
            {
                RawBuffer = false,
                SizeInBytes = (uint)(2 * indexes.Length),
                StructureByteStride = 0,
                Usage = Veldrid.BufferUsage.IndexBuffer
            });
            Renderer.UpdateBuffer(indexBuffer, indexes, 0);
            Veldrid.DeviceBuffer mvpBuffer = Renderer.AllocateBuffer(new Veldrid.BufferDescription()
            {
                RawBuffer = false,
                SizeInBytes = (uint)(64),
                StructureByteStride = 0,
                Usage = Veldrid.BufferUsage.UniformBuffer
            });
            Matrix4x4 mvp = Matrix4x4.Identity;
            Renderer.UpdateBuffer(mvpBuffer,mvp,0);
            Veldrid.Sampler sampler = Renderer.CreateSampler(new Veldrid.SamplerDescription()
            {
                AddressModeU = Veldrid.SamplerAddressMode.Wrap,
                AddressModeV = Veldrid.SamplerAddressMode.Wrap,
                AddressModeW = Veldrid.SamplerAddressMode.Wrap,
                MaximumAnisotropy = 1,
                BorderColor = Veldrid.SamplerBorderColor.TransparentBlack,
                ComparisonKind = Veldrid.ComparisonKind.Never,
                Filter = Veldrid.SamplerFilter.MinPoint_MagPoint_MipPoint,
                LodBias = 1,
                MaximumLod = 0,
                MinimumLod = 0
            });
            Veldrid.ResourceLayout mvpResourceLayout = Renderer.CreateResourceLayout(new Veldrid.ResourceLayoutDescription()
            {
                Elements = new[]
                {
                    new Veldrid.ResourceLayoutElementDescription()
                    {
                        Kind = Veldrid.ResourceKind.UniformBuffer,
                        Name = "MvpBuffer",
                        Options = Veldrid.ResourceLayoutElementOptions.None,
                        Stages = Veldrid.ShaderStages.Vertex
                    }
                }
            });
            Veldrid.ResourceLayout samplerResourceLayout = Renderer.CreateResourceLayout(new Veldrid.ResourceLayoutDescription()
            {
                Elements = new[]
               {
                    new Veldrid.ResourceLayoutElementDescription()
                    {
                        Kind = Veldrid.ResourceKind.Sampler,
                        Name = "SpriteSampler",
                        Options = Veldrid.ResourceLayoutElementOptions.None,
                        Stages = Veldrid.ShaderStages.Fragment
                    }
                }
            });
            Veldrid.ResourceLayout textureResourceLayout = Renderer.CreateResourceLayout(new Veldrid.ResourceLayoutDescription()
            {
                Elements = new[]
               {
                    new Veldrid.ResourceLayoutElementDescription()
                    {
                        Kind = Veldrid.ResourceKind.TextureReadOnly,
                        Name = "SpriteTexture",
                        Options = Veldrid.ResourceLayoutElementOptions.None,
                        Stages = Veldrid.ShaderStages.Fragment
                    }
                }
            });
            Veldrid.ResourceSet mvpBufferResourceSet = Renderer.CreateResourceSet(mvpBuffer, mvpResourceLayout);
            Veldrid.ResourceSet samplerResourceSet = Renderer.CreateResourceSet(sampler, samplerResourceLayout);
            Veldrid.ResourceSet textureResourceSet = Renderer.CreateResourceSet(texture0, textureResourceLayout);
            Veldrid.VertexLayoutDescription vertexLayout = new Veldrid.VertexLayoutDescription()
            {
                Elements = new Veldrid.VertexElementDescription[]
                    {
                        new Veldrid.VertexElementDescription()
                        {
                            Format = Veldrid.VertexElementFormat.Float2,
                            Name = "Position",
                            Offset = 0,
                            Semantic = Veldrid.VertexElementSemantic.Position
                        },
                        new Veldrid.VertexElementDescription()
                        {
                            Format = Veldrid.VertexElementFormat.Float2,
                            Name = "Uv",
                            Offset = 8,
                            Semantic = Veldrid.VertexElementSemantic.TextureCoordinate
                        }
                    },
                InstanceStepRate = 0,
                Stride = 16
            };

            //Demo loop
            bool bSwitch = false;
            while (window.IsAlive)
            {
                //Poll events
                window.PollEvents();
                if (!window.IsAlive)
                    break;

                Renderer.BeginRendering();

                Renderer.SetVertexLayout(vertexLayout);

                Renderer.SetShaders(shaderSet, new Veldrid.ResourceLayout[]
                {
                    mvpResourceLayout,
                    samplerResourceLayout,
                    textureResourceLayout
                });
                Renderer.SetShaderResource(0, mvpBufferResourceSet);
                Renderer.SetShaderResource(1,samplerResourceSet);
                Renderer.SetShaderResource(2, textureResourceSet);
                Renderer.SetFaceCullMode(bSwitch ? Veldrid.FaceCullMode.Front : Veldrid.FaceCullMode.Back);
                bSwitch = !bSwitch;
                Renderer.SetFramebuffer(Renderer.SwapchainFramebuffer);
                Renderer.ClearColor(0, Veldrid.RgbaFloat.CornflowerBlue);
                Renderer.SetViewport(0, new Veldrid.Viewport()
                {
                    X = 0,
                    Y = 0,
                    Width = 1024,
                    Height = 1024,
                    MinDepth = 0.0f,
                    MaxDepth = 1.0f
                });
                Renderer.SetVertexBuffer(vertexBuffer);
                Renderer.SetIndexBuffer(indexBuffer,Veldrid.IndexFormat.UInt16);

                Renderer.DrawInstanced(6, 1, 0, 0, 0);

                Renderer.EndRendering();

                //Present
                Renderer.Present();
            }
        }
    }
}
