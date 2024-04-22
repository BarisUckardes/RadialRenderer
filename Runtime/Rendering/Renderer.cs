using Runtime.Windowing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Veldrid;
using Veldrid.StartupUtilities;

namespace Runtime.Rendering
{
    public sealed partial class Renderer
    {
        private struct PipelineStateCache
        {
            public GraphicsPipelineDescription Desc;
            public List<ResourceLayout> Layouts;
            public Pipeline Pipeline;
        }

        private static Renderer Instance { get; set; }

        public Renderer(Window window, Veldrid.GraphicsBackend backend)
        {
            //Create graphics
#if DEBUG
            bool bDebug = false;
#else
            bool bDebug = false;
#endif
            Veldrid.GraphicsDeviceOptions options = new Veldrid.GraphicsDeviceOptions()
            {
                Debug = bDebug,
                HasMainSwapchain = true,
                PreferDepthRangeZeroToOne = true,
                PreferStandardClipSpaceYDirection = true,
                ResourceBindingModel = Veldrid.ResourceBindingModel.Default,
                SwapchainDepthFormat = Veldrid.PixelFormat.D24_UNorm_S8_UInt,
                SwapchainSrgbFormat = false,
                SyncToVerticalBlank = true
            };

            _device = VeldridStartup.CreateGraphicsDevice(window.UnderlyingWindow, options, (Veldrid.GraphicsBackend)backend);

            //Create internal resources
            CreateInternalResources();

            //Set default pipeline
            PipelineStateCache defaultCache = new PipelineStateCache()
            {
                Pipeline = null,
                Layouts = new List<ResourceLayout>(100),
                Desc = new GraphicsPipelineDescription()
                {
                    BlendState = BlendStateDescription.SingleOverrideBlend,
                    DepthStencilState = new DepthStencilStateDescription()
                    {
                        DepthComparison = ComparisonKind.Never,
                        DepthTestEnabled = false,
                        DepthWriteEnabled = false,
                        StencilBack = new StencilBehaviorDescription(),
                        StencilFront = new StencilBehaviorDescription(),
                        StencilReadMask = 0,
                        StencilReference = 0,
                        StencilTestEnabled = false,
                        StencilWriteMask = 0
                    },
                    Outputs = new OutputDescription(),
                    PrimitiveTopology = PrimitiveTopology.TriangleList,
                    RasterizerState = new RasterizerStateDescription()
                    {
                        CullMode = FaceCullMode.Back,
                        DepthClipEnabled = false,
                        FillMode = PolygonFillMode.Solid,
                        FrontFace = FrontFace.CounterClockwise,
                        ScissorTestEnabled = false
                    },
                    ResourceBindingModel = ResourceBindingModel.Default,
                    ResourceLayouts = null,
                    ShaderSet = new ShaderSetDescription()
                    {
                        Shaders = new Shader[2],
                        Specializations = null,
                        VertexLayouts = new VertexLayoutDescription[1]
                    }
                }
            };
            _currentPipelineCache = defaultCache;

            //Set backend
            Backend = backend;

            //Set instance
            Instance = this;
        }

        public Renderer(Window window)
        {
            //Create graphics
#if DEBUG
            bool bDebug = false;
#else
            bool bDebug = false;
#endif
            Veldrid.GraphicsDeviceOptions options = new Veldrid.GraphicsDeviceOptions()
            {
                Debug = bDebug,
                HasMainSwapchain = true,
                PreferDepthRangeZeroToOne = true,
                PreferStandardClipSpaceYDirection = true,
                ResourceBindingModel = Veldrid.ResourceBindingModel.Default,
                SwapchainDepthFormat = Veldrid.PixelFormat.D24_UNorm_S8_UInt,
                SwapchainSrgbFormat = false,
                SyncToVerticalBlank = true
            };

            Veldrid.GraphicsBackend backend = GetOptimalBackend();
            _device = VeldridStartup.CreateGraphicsDevice(window.UnderlyingWindow, options, (Veldrid.GraphicsBackend)backend);

            //Create internal resources
            CreateInternalResources();

            //Set default pipeline
            PipelineStateCache defaultCache = new PipelineStateCache()
            {
                Pipeline = null,
                Layouts = new List<ResourceLayout>(100),
                Desc = new GraphicsPipelineDescription()
                {
                    BlendState = BlendStateDescription.SingleAlphaBlend,
                    DepthStencilState = new DepthStencilStateDescription()
                    {
                        DepthComparison = ComparisonKind.Always,
                        DepthTestEnabled = false,
                        DepthWriteEnabled = false,
                        StencilBack = new StencilBehaviorDescription(),
                        StencilFront = new StencilBehaviorDescription(),
                        StencilReadMask = 0,
                        StencilReference = 0,
                        StencilTestEnabled = false,
                        StencilWriteMask = 0
                    },
                    Outputs = _device.MainSwapchain.Framebuffer.OutputDescription,
                    PrimitiveTopology = PrimitiveTopology.TriangleList,
                    RasterizerState = new RasterizerStateDescription()
                    {
                        CullMode = FaceCullMode.None,
                        DepthClipEnabled = false,
                        FillMode = PolygonFillMode.Solid,
                        FrontFace = FrontFace.CounterClockwise,
                        ScissorTestEnabled = false
                    },
                    ResourceBindingModel = ResourceBindingModel.Default,
                    ResourceLayouts = null,
                    ShaderSet = new ShaderSetDescription()
                    {
                        Shaders = new Shader[2],
                        Specializations = null,
                        VertexLayouts = new VertexLayoutDescription[1]
                    }
                },
            };
            _currentPipelineCache = defaultCache;

            //Set backend
            Backend = backend;

            //Set instance
            Instance = this;
        }

        public Veldrid.GraphicsBackend Backend { get; private init; }

        private Veldrid.GraphicsBackend GetOptimalBackend()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? Veldrid.GraphicsBackend.Metal : Veldrid.GraphicsBackend.Vulkan;
        }
        private void CreateInternalResources()
        {
            _cmdList = _device.ResourceFactory.CreateCommandList();
            _fence = _device.ResourceFactory.CreateFence(false);
            _pipelineCache = new List<PipelineStateCache>(1000);
        }

        private Veldrid.GraphicsDevice _device;
        private Veldrid.CommandList _cmdList;
        private Veldrid.Fence _fence;
        private List<PipelineStateCache> _pipelineCache;
    }
}
