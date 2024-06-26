﻿using StbiSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veldrid;

namespace Runtime.Rendering
{
    public static class GraphicsUtils
    {
        public unsafe static Texture LoadTexture(string path)
        {
            //Check
            if (!File.Exists(path))
                throw new FileNotFoundException("Texture file failed to found");

            //Load
            byte[] bytes = File.ReadAllBytes(path);

            //Create image from memory
            StbiImage image = Stbi.LoadFromMemory(bytes, 4);

            //Allocate texture
            TextureDescription desc = new TextureDescription()
            {
                Width = (uint)image.Width,
                Height = (uint)image.Height,
                Depth = 1,
                ArrayLayers = 1,
                MipLevels = 1,
                Format = PixelFormat.R8_G8_B8_A8_UNorm,
                SampleCount = TextureSampleCount.Count1,
                Type = TextureType.Texture2D,
                Usage = TextureUsage.Sampled
            };
            Texture texture = Renderer.AllocateTexture(desc);

            //Update texture JIT
            fixed (byte* pByte = image.Data)
            {
                Renderer.UpdateTexture(texture, (nuint)pByte, (uint)(image.Width * image.Height * 4), 0, 0, 0, (uint)image.Width, (uint)image.Height, 1, 0, 0);
            }

            return texture;
        }
    }
}
