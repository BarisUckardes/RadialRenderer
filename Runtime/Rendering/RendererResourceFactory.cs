﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veldrid;
using Veldrid.SPIRV;

namespace Runtime.Rendering
{
    public partial class Renderer
    {
        public static DeviceBuffer AllocateBuffer(in BufferDescription desc)
        {
            return Instance._device.ResourceFactory.CreateBuffer(desc);
        }
        public static Texture AllocateTexture(in TextureDescription desc)
        {
            return Instance._device.ResourceFactory.CreateTexture(desc);
        }
        public static TextureView CreateTextureView(in TextureViewDescription desc)
        {
            return Instance._device.ResourceFactory.CreateTextureView(desc);
        }
        public static Sampler CreateSampler(in SamplerDescription desc)
        {
            return Instance._device.ResourceFactory.CreateSampler(desc);
        }
        public static CommandList AllocateCommandList()
        {
            return Instance._device.ResourceFactory.CreateCommandList();
        }
        public static Fence CreateFence()
        {
            return Instance._device.ResourceFactory.CreateFence(true);
        }
        public static Framebuffer CreateFramebuffer(in FramebufferDescription desc)
        {
            return Instance._device.ResourceFactory.CreateFramebuffer(desc);
        }
        public static ResourceLayout CreateResourceLayout(in ResourceLayoutDescription desc)
        {
            return Instance._device.ResourceFactory.CreateResourceLayout(desc);
        }
        public static ResourceSet CreateResourceSet(BindableResource resource, ResourceLayout resourceLayout)
        {
            return Instance._device.ResourceFactory.CreateResourceSet(
            new ResourceSetDescription()
            {
                BoundResources = new[] { resource },
                Layout = resourceLayout
            });
        }
        public static Pipeline CreateGraphicsPipeline(in GraphicsPipelineDescription desc)
        {
            return Instance._device.ResourceFactory.CreateGraphicsPipeline(desc);
        }
        public static Pipeline CreateComputePipeline(in ComputePipelineDescription desc)
        {
            return Instance._device.ResourceFactory.CreateComputePipeline(desc);
        }
        public static Shader[] CompileVertexFragmentShader(string vertexSource,string fragmentSource)
        {
            ShaderDescription vertexDesc = new ShaderDescription()
            {
                Debug = false,
                EntryPoint = "main",
                ShaderBytes = Encoding.UTF8.GetBytes(vertexSource),
                Stage = ShaderStages.Vertex
            };
            ShaderDescription fragmentDesc = new ShaderDescription()
            {
                Debug = false,
                EntryPoint = "main",
                ShaderBytes = Encoding.UTF8.GetBytes(fragmentSource),
                Stage = ShaderStages.Fragment
            };


            return Instance._device.ResourceFactory.CreateFromSpirv(vertexDesc, fragmentDesc);
        }
        
    
        public static Framebuffer SwapchainFramebuffer => Instance._device.SwapchainFramebuffer;
    }
}
