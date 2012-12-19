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

namespace Funani.Gui.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Text;

    public class DirectoryTreeViewModel
    {
        public DirectoryTreeViewModel()
		{
            var rootDirectories = Directory.GetLogicalDrives().Select(x => new DirectoryInfo(x));
            _firstGeneration = new ObservableCollection<DirectoryViewModel>();
            foreach (var model in rootDirectories)
            {
                try
                {
                    _firstGeneration.Add(new DirectoryViewModel(model));
                }
                catch
                {
                }
            }
		}

        public void ExpandAndSelect(DirectoryInfo path)
        {
            DirectoryViewModel vm = Lookup(path);
            if (vm != null)
            {
                vm.IsExpanded = true;
                vm.IsSelected = true;
            }
        }

        private DirectoryViewModel LookupRoot(DirectoryInfo path)
        {
            return FirstGeneration.FirstOrDefault(x => x.DirectoryInfo.Name == path.Root.Name);
        }

        private DirectoryViewModel Lookup(DirectoryInfo path)
        {
            DirectoryViewModel root = LookupRoot(path);
            if (root != null)
                return root.Lookup(path);
            return null;
        }

        public ObservableCollection<DirectoryViewModel> FirstGeneration
		{
			get
			{
				return _firstGeneration;
			}
		}

        readonly ObservableCollection<DirectoryViewModel> _firstGeneration;
    }
}
