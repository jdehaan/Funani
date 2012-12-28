﻿/*
 * Copyright (c) 2008-2013, Jaap de Haan <jaap.dehaan@color-of-code.de>
 * All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions
 * are met:
 * 
 *   * Redistributions of source code must retain the above copyright notice,
 *     this list of conditions and the following disclaimer.
 *   * Redistributions in binary form must reproduce the above copyright
 *     notice, this list of conditions and the following disclaimer in the
 *     documentation and/or other materials provided with the distribution.
 *   * Neither the name of the "Color-Of-Code" nor the names of its
 *     contributors may be used to endorse or promote products derived from
 *     this software without specific prior written permission.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
 * ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE
 * LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
 * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
 * SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
 * CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF
 * THE POSSIBILITY OF SUCH DAMAGE.
 */

namespace Funani.Thumbnailer
{
    using System;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    public static class Thumbnail
    {
        /// <summary>
        /// Create a thumbnail of the given max size for the resource specified by the uri
        /// assuming the mime type is correctly specified too.
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="mime"></param>
        /// <param name="thumbnailSize"></param>
        /// <returns></returns>
        public static BitmapSource Extract(Uri uri, String mime, int thumbnailSize)
        {
            //TODO: write thumbnailers depending on the mime type
            // this works for WPF supported image formats only
            Orientation orientation = Orientation.Normal;
            BitmapFrame frame = BitmapFrame.Create(
                uri, BitmapCreateOptions.DelayCreation, BitmapCacheOption.None);
            BitmapSource ret = null;
            BitmapMetadata meta = frame.Metadata as BitmapMetadata;
            if (frame.PixelHeight < thumbnailSize && frame.PixelWidth < thumbnailSize)
            {
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.UriSource = uri;
                image.CacheOption = BitmapCacheOption.None;
                image.CreateOptions = BitmapCreateOptions.DelayCreation;
                image.EndInit();

                if (image.CanFreeze)
                    image.Freeze();

                ret = image;
            }
            else
            {
                if (frame.Thumbnail == null)
                {
                    BitmapImage image = new BitmapImage();
                    if (frame.PixelHeight >= frame.PixelWidth)
                        image.DecodePixelHeight = thumbnailSize;
                    else
                        image.DecodePixelWidth = thumbnailSize;
                    image.BeginInit();
                    image.UriSource = uri;
                    image.CacheOption = BitmapCacheOption.None;
                    image.CreateOptions = BitmapCreateOptions.DelayCreation;
                    image.EndInit();

                    if (image.CanFreeze)
                        image.Freeze();

                    ret = image;
                }
                else
                {
                    ret = frame.Thumbnail;
                }
            }

            if ((meta != null) && (ret != null))
            {
                double angle = 0;
                if (meta.GetQuery("/app1/ifd/{ushort=274}") != null)
                {
                    orientation = (Orientation)Enum.Parse(typeof(Orientation),
                        meta.GetQuery("/app1/ifd/{ushort=274}").ToString());
                }

                switch (orientation)
                {
                    case Orientation.Rotate90:
                        angle = -90;
                        break;
                    case Orientation.Rotate180:
                        angle = 180;
                        break;
                    case Orientation.Rotate270:
                        angle = 90;
                        break;
                }

                if (angle != 0)
                {
                    ret = new TransformedBitmap(ret.Clone(), new RotateTransform(angle));
                    ret.Freeze();
                }
            }

            return ret;
        }

    }
}