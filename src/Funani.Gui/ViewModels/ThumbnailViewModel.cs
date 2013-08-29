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

using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Catel.Data;
using Catel.MVVM;
using Funani.Gui.Converters;

namespace Funani.Gui.ViewModels
{
    /// <summary>
    ///     ThumbnailViewModel view model.
    /// </summary>
    public class ThumbnailViewModel : ViewModelBase
    {
        private const int MaxThumbnailSize = 256;
        private static readonly UriToThumbnailConverter Converter = new UriToThumbnailConverter(MaxThumbnailSize);

        private BitmapSource _thumbnail;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ThumbnailViewModel" /> class.
        /// </summary>
        public ThumbnailViewModel(string fullName)
        {
            FullName = fullName;
        }

        /// <summary>
        ///     Gets the title of the view model.
        /// </summary>
        /// <value>The title.</value>
        public override string Title
        {
            get { return "ThumbnailViewModel"; }
        }

        #region Properties

        #region Property: FullName

        public static readonly PropertyData FullNameProperty =
            RegisterProperty("FullName", typeof(String), null);

        public String FullName
        {
            get { return GetValue<String>(FullNameProperty); }
            set { SetValue(FullNameProperty, value); }
        }

        #endregion

        public double ThumbnailWidth
        {
            get
            {
                if (MaxThumbnailSize > Thumbnail.PixelWidth)
                    return double.NaN;
                return Thumbnail.PixelWidth;
            }
        }

        public double ThumbnailHeight
        {
            get
            {
                if (MaxThumbnailSize > Thumbnail.PixelHeight)
                    return double.NaN;
                return Thumbnail.PixelHeight;
            }
        }

        public BitmapSource Thumbnail
        {
            get
            {
                return _thumbnail ??
                       (_thumbnail = Converter.Convert(FullName, typeof(BitmapSource), null, null) as BitmapSource);
            }
        }

        public BitmapScalingMode ScalingMode
        {
            get
            {
                if (ThumbnailWidth < MaxThumbnailSize && ThumbnailHeight < MaxThumbnailSize)
                    return BitmapScalingMode.Linear;
                return BitmapScalingMode.HighQuality;
            }
        }

        #endregion

        #region Commands

        #endregion
    }
}