﻿using System;
using System.Runtime.InteropServices;
using SDL2;

namespace SdlExample
{
    //Texture wrapper class
    public class LTexture
    {
        //The actual hardware texture
        private IntPtr _Texture;

        //Image dimensions
        private int _Width;
        private int _Height;

        public LTexture()
        {
            //Initialize
            _Texture = IntPtr.Zero;
            _Width = 0;
            _Height = 0;
        }

        //Deallocates memory
        ~LTexture()
        {
            Free();
        }

        //Loads image at specified path
        public bool LoadFromFile(string path)
        {
            //Get rid of preexisting texture
            Free();

            //Load image at specified path
            var loadedSurface = SDL_image.IMG_Load(path);
            if (loadedSurface == IntPtr.Zero)
            {
                Console.WriteLine("Unable to load image {0}! SDL Error: {1}", path, SDL.SDL_GetError());
                return false;
            }

            var s = Marshal.PtrToStructure<SDL.SDL_Surface>(loadedSurface);

            //Color key image
            SDL.SDL_SetColorKey(loadedSurface, (int)SDL.SDL_bool.SDL_TRUE, SDL.SDL_MapRGB(s.format, 0, 0xFF, 0xFF));

            //Create texture from surface pixels
            var newTexture = SDL.SDL_CreateTextureFromSurface(Program.Renderer, loadedSurface);
            if (newTexture == IntPtr.Zero)
            {
                Console.WriteLine("Unable to create texture from {0}! SDL Error: {1}", path, SDL.SDL_GetError());
                return false;
            }

            //Get image dimensions
            _Width = s.w;
            _Height = s.h;

            //Get rid of old loaded surface
            SDL.SDL_FreeSurface(loadedSurface);

            //Return success
            _Texture = newTexture;
            return true;
        }

        //Deallocates texture
        public void Free()
        {
            //Free texture if it exists
            if (_Texture == IntPtr.Zero)
                return;

            SDL.SDL_DestroyTexture(_Texture);
            _Texture = IntPtr.Zero;
            _Width = 0;
            _Height = 0;
        }

        //Renders texture at given point
        public void Render(int x, int y)
        {
            //Set rendering space and render to screen
            var renderQuad = new SDL.SDL_Rect { x = x, y = y, w = _Width, h = _Height };
            SDL.SDL_RenderCopy(Program.Renderer, _Texture, IntPtr.Zero, ref renderQuad);
        }


        //Renders texture at given point
        public void Render(int x, int y, SDL.SDL_Rect? clip)
        {
            //Set rendering space and render to screen
            var renderQuad = new SDL.SDL_Rect { x = x, y = y, w = _Width, h = _Height };

            //Set clip rendering dimensions
            if (clip != null)
            {
                renderQuad.w = clip.Value.w;
                renderQuad.h = clip.Value.h;

                var myClip = clip.Value;

                SDL.SDL_RenderCopy(Program.Renderer, _Texture, ref myClip, ref renderQuad);
                return;
            }

            SDL.SDL_RenderCopy(Program.Renderer, _Texture, IntPtr.Zero, ref renderQuad);
        }
    }
}