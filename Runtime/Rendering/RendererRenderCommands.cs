using System;
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
        public static void SetFramebuffer(Veldrid.Framebuffer framebuffer)
        {
            Instance._cmdList.SetFramebuffer(framebuffer);
            Instance._currentPipelineCache.Desc.Outputs = framebuffer.OutputDescription;
            MarkPipelineDirty();
        }
        public static void ClearColor(uint index,in RgbaFloat clearColor)
        {
            Instance._cmdList.ClearColorTarget(index, clearColor);
        }
        public static void ClearDepth(float depth)
        {
            Instance._cmdList.ClearDepthStencil(depth);
        }
        public static void ClearDepthStencil(float depth,byte stencil)
        {
            Instance._cmdList.ClearDepthStencil(depth, stencil);
        }
        public static void Present()
        {
            Instance._device.SwapBuffers();

        }
        public static void BeginRendering()
        {
            Instance._cmdList.Begin();
        }

        public static void EndRendering(bool bWaitForFinish = true)
        {
            Instance._cmdList.End();

            if (bWaitForFinish)
            {
                Instance._device.SubmitCommands(Instance._cmdList, Instance._fence);
                Instance._device.WaitForFence(Instance._fence);
                Instance._device.ResetFence(Instance._fence);
            }
            else
            {
                Instance._device.SubmitCommands(Instance._cmdList, Instance._fence);
            }


            Instance._pipelineActive = false;
        }

        public static void UpdateBuffer(DeviceBuffer buffer, nuint data, uint sizeInBytes, uint offset)
        {
            Instance._device.UpdateBuffer(buffer, offset, (nint)data, sizeInBytes);
        }
        public static void UpdateBuffer<T>(DeviceBuffer buffer, T data, uint offset) where T: unmanaged
        {
            Instance._device.UpdateBuffer(buffer,offset,data);
        }
        public static void UpdateBuffer<T>(DeviceBuffer buffer, T[] data,uint offsetInBytes) where T: unmanaged
        {
            Instance._device.UpdateBuffer<T>(buffer, offsetInBytes, data);
        }
        public static void UpdateBuffer<T>(DeviceBuffer buffer, ReadOnlySpan<T> data, uint offsetInBytes) where T : unmanaged
        {
            Instance._device.UpdateBuffer<T>(buffer, offsetInBytes, data);
        }
        public static void UpdateTexture(Texture texture, nuint data, uint sizeInBytes, uint x, uint y, uint z, uint width, uint height, uint depth, uint arrayIndex, uint mipIndex)
        {
            Instance._device.UpdateTexture(texture, (nint)data, sizeInBytes, x, y, z, width, height, depth, arrayIndex, mipIndex);
        }
     
        //Shaders
        public static void SetGraphicsShaderGroup(ShaderGroup group)
        {
            if(Instance._currentPipelineCache.Desc.ShaderSet.Shaders == null)
            {
                Instance._currentPipelineCache.Desc.ShaderSet.Shaders = new Shader[2];
            }

            Instance._currentPipelineCache.Desc.ShaderSet.Shaders = group.Shaders;

            Instance._currentPipelineCache.ShaderGroup = group;
            Instance._currentPipelineCache.Desc.ResourceLayouts = group.Layouts;

            MarkPipelineDirty();
        }
        //Rasterizer
        public static void SetFaceCullMode(FaceCullMode faceCullMode)
        {
            Instance._currentPipelineCache.Desc.RasterizerState.CullMode = faceCullMode;

            MarkPipelineDirty();
        }
        public static void SetPolygonFillMode(PolygonFillMode polygonFillMode)
        {
            Instance._currentPipelineCache.Desc.RasterizerState.FillMode = polygonFillMode;

            MarkPipelineDirty();
        }
        public static void EnableDepthClip()
        {
            Instance._currentPipelineCache.Desc.RasterizerState.DepthClipEnabled = true;

            MarkPipelineDirty();
        }
        public static void DisableDepthClip()
        {
            Instance._currentPipelineCache.Desc.RasterizerState.DepthClipEnabled = false;

            MarkPipelineDirty();
        }
        public static void EnableScissorTest()
        {
            Instance._currentPipelineCache.Desc.RasterizerState.ScissorTestEnabled = true;

            MarkPipelineDirty();
        }
        public static void DisableScissorTest()
        {
            Instance._currentPipelineCache.Desc.RasterizerState.ScissorTestEnabled = false;

            MarkPipelineDirty();
        }

        //Depth stencil state
        public static void EnableDepthTest()
        {
            Instance._currentPipelineCache.Desc.DepthStencilState.DepthTestEnabled = true;

            MarkPipelineDirty();
        }
        public static void DisableDepthTest()
        {
            Instance._currentPipelineCache.Desc.DepthStencilState.DepthTestEnabled = false;

            MarkPipelineDirty();
        }
        public static void EnableDepthWrite()
        {
            Instance._currentPipelineCache.Desc.DepthStencilState.DepthWriteEnabled = true;

            MarkPipelineDirty();
        }
        public static void DisableDepthWrite()
        {
            Instance._currentPipelineCache.Desc.DepthStencilState.DepthWriteEnabled = false;

            MarkPipelineDirty();
        }
        public static void SetDepthComparisionMethod(ComparisonKind kind)
        {
            Instance._currentPipelineCache.Desc.DepthStencilState.DepthComparison = kind;

            MarkPipelineDirty();
        }
        public static void EnableStencilTest()
        {
            Instance._currentPipelineCache.Desc.DepthStencilState.StencilTestEnabled = true;

            MarkPipelineDirty();
        }
        public static void DisableStencilTest()
        {
            Instance._currentPipelineCache.Desc.DepthStencilState.StencilTestEnabled = false;

            MarkPipelineDirty();
        }
        public static void SetStencilFrontFace(in StencilBehaviorDescription behaviour)
        {
            Instance._currentPipelineCache.Desc.DepthStencilState.StencilFront = behaviour;

            MarkPipelineDirty();
        }
        public static void SetStencilBackFace(in StencilBehaviorDescription behaviour)
        {
            Instance._currentPipelineCache.Desc.DepthStencilState.StencilBack = behaviour;

            MarkPipelineDirty();
        }
        public static void SetStencilReadMask(byte mask)
        {
            Instance._currentPipelineCache.Desc.DepthStencilState.StencilReadMask = mask;

            MarkPipelineDirty();
        }
        public static void SetStencilWritemask(byte mask)
        {
            Instance._currentPipelineCache.Desc.DepthStencilState.StencilWriteMask = mask;

            MarkPipelineDirty();
        }
        public static void SetStencilReference(uint reference)
        {
            Instance._currentPipelineCache.Desc.DepthStencilState.StencilReference = reference;

            MarkPipelineDirty();
        }
        
        public static void CopyBuffer(DeviceBuffer sourceBuffer,DeviceBuffer destinationBuffer,uint sourceOffsetInBytes,uint destinationOffsetInBytes,uint sizeInBytes)
        {
            Instance._cmdList.CopyBuffer(sourceBuffer, sourceOffsetInBytes, destinationBuffer, destinationOffsetInBytes, sizeInBytes);
            if (!destinationBuffer.Usage.HasFlag(BufferUsage.Staging))
                Renderer.WaitLastCommand();
        }
        public static void CopyBuffer(DeviceBuffer sourceBuffer, DeviceBuffer destinationBuffer)
        {
            Instance._cmdList.CopyBuffer(sourceBuffer,0, destinationBuffer,0,sourceBuffer.SizeInBytes);
            if (!destinationBuffer.Usage.HasFlag(BufferUsage.Staging))
                Renderer.WaitLastCommand();
        }
        public static void CopyTexture(Texture source,Texture destination)
        {
            Instance._cmdList.CopyTexture(source, destination);
            if (!destination.Usage.HasFlag(BufferUsage.Staging))
                Renderer.WaitLastCommand();
        }
        public static void CopyTexture(Texture source,uint srcX,uint srcY,uint srcZ,uint srcMipLevel,uint srcArrayLevel,Texture destination,uint dstX,uint dstY,uint dstZ,uint dstMipLevel,uint dstArrayLevel,uint width,uint height,uint depth,uint layerCount)
        {
            Instance._cmdList.CopyTexture(source, srcX, srcY, srcZ, srcMipLevel, srcArrayLevel, destination, dstX, dstY, dstZ, dstMipLevel, dstArrayLevel, width, height, depth, layerCount);
            if (!destination.Usage.HasFlag(BufferUsage.Staging))
                Renderer.WaitLastCommand();
        }
        //Blending state
        public static void EnableBlend(int attachmentIndex)
        {
            if(Instance._currentPipelineCache.Desc.BlendState.AttachmentStates == null)
            {
                Instance._currentPipelineCache.Desc.BlendState.AttachmentStates = new BlendAttachmentDescription[8];
            }
            Instance._currentPipelineCache.Desc.BlendState.AttachmentStates[attachmentIndex].BlendEnabled = true;

            MarkPipelineDirty();
        }
        public static void DisableBlend(int attachmentIndex)
        {
            if (Instance._currentPipelineCache.Desc.BlendState.AttachmentStates == null)
            {
                Instance._currentPipelineCache.Desc.BlendState.AttachmentStates = new BlendAttachmentDescription[8];
            }
            Instance._currentPipelineCache.Desc.BlendState.AttachmentStates[attachmentIndex].BlendEnabled = false;

            MarkPipelineDirty();
        }
        public static void SetBlendColorMask(ColorWriteMask mask,int attachmentIndex)
        {
            if (Instance._currentPipelineCache.Desc.BlendState.AttachmentStates == null)
            {
                Instance._currentPipelineCache.Desc.BlendState.AttachmentStates = new BlendAttachmentDescription[8];
            }
            Instance._currentPipelineCache.Desc.BlendState.AttachmentStates[attachmentIndex].ColorWriteMask = mask;

            MarkPipelineDirty();
        }
        public static void SetBlendColorFactors(BlendFactor source,BlendFactor destination,int attachmentIndex)
        {
            if (Instance._currentPipelineCache.Desc.BlendState.AttachmentStates == null)
            {
                Instance._currentPipelineCache.Desc.BlendState.AttachmentStates = new BlendAttachmentDescription[8];
            }
            Instance._currentPipelineCache.Desc.BlendState.AttachmentStates[attachmentIndex].SourceColorFactor = source;
            Instance._currentPipelineCache.Desc.BlendState.AttachmentStates[attachmentIndex].DestinationColorFactor = destination;

            MarkPipelineDirty();
        }
        public static void SetBlendColorFunction(BlendFunction function,int attachmentIndex)
        {
            if (Instance._currentPipelineCache.Desc.BlendState.AttachmentStates == null)
            {
                Instance._currentPipelineCache.Desc.BlendState.AttachmentStates = new BlendAttachmentDescription[8];
            }
            Instance._currentPipelineCache.Desc.BlendState.AttachmentStates[attachmentIndex].ColorFunction = function;

            MarkPipelineDirty();
        }
        public static void SetBlendAlphaFactors(BlendFactor source,BlendFactor destination,int attachmentIndex)
        {
            if (Instance._currentPipelineCache.Desc.BlendState.AttachmentStates == null)
            {
                Instance._currentPipelineCache.Desc.BlendState.AttachmentStates = new BlendAttachmentDescription[8];
            }
            Instance._currentPipelineCache.Desc.BlendState.AttachmentStates[attachmentIndex].SourceAlphaFactor = source;
            Instance._currentPipelineCache.Desc.BlendState.AttachmentStates[attachmentIndex].DestinationAlphaFactor = destination;

            MarkPipelineDirty();
        }
        public static void SetBlendAlphaFunction(BlendFunction function,int attachmentIndex)
        {
            if (Instance._currentPipelineCache.Desc.BlendState.AttachmentStates == null)
            {
                Instance._currentPipelineCache.Desc.BlendState.AttachmentStates = new BlendAttachmentDescription[8];
            }
            Instance._currentPipelineCache.Desc.BlendState.AttachmentStates[attachmentIndex].AlphaFunction = function;

            MarkPipelineDirty();
        }

        //IA state
        public static void SetVertexLayout(in VertexLayoutDescription vertexLayout)
        {
            if (Instance._currentPipelineCache.Desc.ShaderSet.VertexLayouts == null)
            {
                Instance._currentPipelineCache.Desc.ShaderSet.VertexLayouts = new VertexLayoutDescription[1];
            }
            Instance._currentPipelineCache.Desc.ShaderSet.VertexLayouts[0] = vertexLayout;

            MarkPipelineDirty();
        }

        public static void SetVertexBuffer(DeviceBuffer buffer)
        {
            Instance._cmdList.SetVertexBuffer(0,buffer);
        }
        public static void SetIndexBuffer(DeviceBuffer buffer,IndexFormat format)
        {
            Instance._cmdList.SetIndexBuffer(buffer, format);
        }
        public static void SetShaderResource(uint index,ResourceSet resourceSet)
        {
            //Invalidate the pipeline
            InvalidateAndPlacePipeline();

            Instance._cmdList.SetGraphicsResourceSet(index, resourceSet);
        }
        public static void SetGraphicsResource(string name,BindableResource resource)
        {
            //Get and check resource slot index
            int resourceSlotIndex = Instance._currentPipelineCache.ShaderGroup.Resources.IndexOf(name);
            if (resourceSlotIndex == -1)
                throw new Exception("Invalid shader group");

            //Invalidate the pipeline
            InvalidateAndPlacePipeline();

            //Check for the resource set
            ResourceSet resourceSet = null;
            if(!Instance._resourceSetCache.TryGetValue(resource, out resourceSet)) // found resource set
            {
                resourceSet = Renderer.CreateResourceSet(resource, Instance._currentPipelineCache.ShaderGroup.Layouts[resourceSlotIndex]);
                Instance._resourceSetCache.Add(resource, resourceSet);
            }

            Instance._cmdList.SetGraphicsResourceSet((uint)resourceSlotIndex, resourceSet);
        }
        public static void DrawInstanced(uint indexCount,uint instanceCount,uint indexOffset,int vertexOffset,uint instanceOffset)
        {
            //Invalidate the pipeline
            InvalidateAndPlacePipeline();

            //Draw
            Instance._cmdList.DrawIndexed(indexCount, instanceCount, indexOffset,vertexOffset, instanceOffset);
        }
        public static void SetViewport(uint index,in Viewport viewport)
        {
            Instance._cmdList.SetViewport(index, viewport);
        }
        public static void ClearCachedResourceSets()
        {

        }
        public static void ClearCachedPipelines()
        {

        }
        private static void InvalidateAndPlacePipeline()
        {
            //Check if a pipeline is already active
            if (Instance._pipelineActive)
                return;

            //Check if dirty
            if (!Instance._pipelineDirty)
            {
                Instance._cmdList.SetPipeline(Instance._currentPipelineCache.Pipeline);
                Instance._pipelineActive = true;
                return;
            }

            //Check the pipeline settings againts the others
            PipelineStateCache? foundMatchCache = null;
            foreach(PipelineStateCache other in Instance._pipelineCache)
            {
                //Check primitive topology
                if (Instance._currentPipelineCache.Desc.PrimitiveTopology != other.Desc.PrimitiveTopology)
                {
                    continue;
                }

                //Check output desc
                if (!CheckOutputDesc(Instance._currentPipelineCache.Desc.Outputs, other.Desc.Outputs))
                {
                    continue;
                }

                //Check rasterizer
                if (!CheckRasterizerState(Instance._currentPipelineCache.Desc.RasterizerState, other.Desc.RasterizerState))
                {
                    continue;
                }

                //Check depth stencil state
                if (!CheckDepthStencilState(Instance._currentPipelineCache.Desc.DepthStencilState, other.Desc.DepthStencilState))
                {
                    continue;
                }

                //Check blending desc
                if(!CheckBlendingState(Instance._currentPipelineCache.Desc.BlendState,other.Desc.BlendState))
                {
                    continue;
                }

                //Check shader group
                if (!CheckShaderGroup(Instance._currentPipelineCache.ShaderGroup, other.ShaderGroup))
                {
                    continue;
                }

                //Check shader set
                if (!CheckVertexLayout(Instance._currentPipelineCache.Desc.ShaderSet.VertexLayouts[0], other.Desc.ShaderSet.VertexLayouts[0]))
                {
                    continue;
                }

                //Set found match cache
                foundMatchCache = other;
                break;
            }

            //Generate new pipeline if failed to find new one
            if(foundMatchCache == null)
            {
                foundMatchCache = new PipelineStateCache()
                {
                    Desc = Instance._currentPipelineCache.Desc,
                    ShaderGroup = Instance._currentPipelineCache.ShaderGroup,
                    Pipeline = GenerateCurrentPipeline()
                };

                Instance._pipelineCache.Add(foundMatchCache.Value);
            }

            //Set pipeline
            Instance._cmdList.SetPipeline(foundMatchCache.Value.Pipeline);

            //Set current
            Instance._currentPipelineCache = foundMatchCache.Value;

            //Clear dirty state
            Instance._pipelineDirty = false;

            //Set pipeline active
            Instance._pipelineActive = true;
        }

        private static void MarkPipelineDirty()
        {
            Instance._pipelineDirty = true;
        }

        private static bool CheckOutputDesc(in OutputDescription a,in OutputDescription b)
        {
            if ((a.DepthAttachment == null && b.DepthAttachment != null) || (a.DepthAttachment != null && b.DepthAttachment == null))
                return false;

            if (a.ColorAttachments.Length != b.ColorAttachments.Length)
                return false;

            for(int i = 0;i< a.ColorAttachments.Length;i++)
            {
                if (a.ColorAttachments[i].Format != b.ColorAttachments[i].Format)
                    return false;
            }

            return true;
        }
        private static bool CheckRasterizerState(in RasterizerStateDescription a,in RasterizerStateDescription b)
        {
            return
                a.ScissorTestEnabled == b.ScissorTestEnabled &&
                a.DepthClipEnabled == b.DepthClipEnabled &&
                a.CullMode == b.CullMode &&
                a.FillMode == b.FillMode &&
                a.FrontFace == b.FrontFace;
                
        }
        private static bool CheckDepthStencilState(in DepthStencilStateDescription a,in DepthStencilStateDescription b)
        {
            return
                a.DepthComparison == b.DepthComparison &&
                a.DepthTestEnabled == b.DepthTestEnabled &&
                a.StencilTestEnabled == b.StencilTestEnabled &&
                a.DepthWriteEnabled == b.DepthWriteEnabled &&
                a.StencilReadMask == b.StencilReadMask &&
                a.StencilReference == b.StencilReference &&
                a.StencilWriteMask == b.StencilWriteMask &&
                CheckStencilBehaviour(a.StencilFront, b.StencilFront) &&
                CheckStencilBehaviour(a.StencilBack, b.StencilBack);

        }
        private static bool CheckBlendingState(in BlendStateDescription a,in BlendStateDescription b)
        {
            if (a.AttachmentStates.Length != b.AttachmentStates.Length)
                return false;

            for(int i = 0;i< a.AttachmentStates.Length;i++)
            {
                if (a.AttachmentStates[i].AlphaFunction != b.AttachmentStates[i].AlphaFunction)
                    return false;
                if (a.AttachmentStates[i].ColorFunction != b.AttachmentStates[i].ColorFunction)
                    return false;
                if (a.AttachmentStates[i].SourceColorFactor != b.AttachmentStates[i].SourceColorFactor)
                    return false;
                if (a.AttachmentStates[i].DestinationColorFactor != b.AttachmentStates[i].DestinationColorFactor)
                    return false;
                if (a.AttachmentStates[i].DestinationAlphaFactor != b.AttachmentStates[i].DestinationAlphaFactor)
                    return false;
                if (a.AttachmentStates[i].BlendEnabled != b.AttachmentStates[i].BlendEnabled)
                    return false;
                if (a.AttachmentStates[i].ColorWriteMask != b.AttachmentStates[i].ColorWriteMask)
                    return false;
            }

            return true;
        }
        private static bool CheckStencilBehaviour(in StencilBehaviorDescription a,in StencilBehaviorDescription b)
        {
            return
                a.Comparison == b.Comparison &&
                a.DepthFail == b.DepthFail &&
                a.Fail == b.Fail &&
                a.Pass == b.Pass;
        }
        private static bool CheckVertexLayout(in VertexLayoutDescription a,in VertexLayoutDescription b)
        {
            if(a.InstanceStepRate != b.InstanceStepRate)
                return false;
            if (a.Stride != b.Stride)
                return false;
            if(a.Elements.Length != b.Elements.Length)
                return false;

            for(int i = 0;i< a.Elements.Length;i++)
            {
                if (a.Elements[i].Name != b.Elements[i].Name)
                    return false;
                if (a.Elements[i].Format != b.Elements[i].Format)
                    return false;
                if (a.Elements[i].Semantic != b.Elements[i].Semantic)
                    return false;
                if (a.Elements[i].Offset != b.Elements[i].Offset)
                    return false;
            }

            return true;
        }
        private static bool CheckShaderGroup(ShaderGroup a,ShaderGroup b)
        {
            if (a.Shaders.Length != b.Shaders.Length)
                return false;
            if (a.Layouts.Length != b.Layouts.Length)
                return false;

            for (int i = 0; i < a.Layouts.Length; i++)
            {
                if (a.Layouts[i] != b.Layouts[i])
                    return false;
            }

            return true;
        }
        private static Pipeline GenerateCurrentPipeline()
        {
            return Instance._device.ResourceFactory.CreateGraphicsPipeline(Instance._currentPipelineCache.Desc);
        }
        private static void WaitLastCommand()
        {
            Instance._cmdList.End();
            Instance._device.SubmitCommands(Instance._cmdList, Instance._fence);
            Instance._device.WaitForFence(Instance._fence);
            Instance._device.ResetFence(Instance._fence);
            Instance._cmdList.Begin();
            Instance._pipelineActive = false;
        }

        private bool _pipelineDirty = true;
        private bool _pipelineActive = false;
        private PipelineStateCache _currentPipelineCache;
        private Dictionary<BindableResource, ResourceSet> _resourceSetCache;
    }
}
