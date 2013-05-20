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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;

using Catel.MVVM;

using Funani.Api;
using Funani.Engine.Commands;
using Catel.Data;
using Funani.Gui.Properties;

namespace Funani.Gui.ViewModels
{
    /// <summary>
    /// The top level filesystem drives
    /// </summary>
    public class DirectoryTreeViewModel : ViewModelBase
    {
        private readonly IEngine _engine;
        private readonly ObservableCollection<DirectoryViewModel> _firstGeneration;

        public DirectoryTreeViewModel()
        {
            _engine = GetService<IEngine>();

            // register commands
            UploadAllInThisDirectory = new Command(OnUploadAllInThisDirectoryExecute, OnUploadAllInThisDirectoryCanExecute);
            UploadAllRecursively = new Command(OnUploadAllRecursivelyExecute, OnUploadAllRecursivelyCanExecute);

            IEnumerable<DirectoryInfo> rootDirectories = Directory.GetLogicalDrives().Select(x => new DirectoryInfo(x));
            _firstGeneration = new ObservableCollection<DirectoryViewModel>();
            foreach (DirectoryInfo model in rootDirectories)
            {
                try
                {
                    _firstGeneration.Add(new DirectoryViewModel(model));
                }
                catch (Exception ex)
                {
                    _firstGeneration.Add(new DirectoryViewModel(model, null, ex));
                }
            }

            Settings settings = Settings.Default;
            if (!String.IsNullOrWhiteSpace(settings.LastDirectoryExplorerSelectedPath))
            {
                ExpandAndSelect(new DirectoryInfo(settings.LastDirectoryExplorerSelectedPath));
            }
        }

        public ObservableCollection<DirectoryViewModel> FirstGeneration
        {
            get { return _firstGeneration; }
        }

        #region Property: SelectedDirectory
        /// <summary>
        /// Gets or sets the property value.
        /// </summary>
        public DirectoryViewModel SelectedDirectory
        {
            get { return GetValue<DirectoryViewModel>(SelectedDirectoryProperty); }
            set { SetValue(SelectedDirectoryProperty, value); }
        }

        /// <summary>
        /// Register the SelectedDirectory property so it is known in the class.
        /// </summary>
        public static readonly PropertyData SelectedDirectoryProperty = RegisterProperty("SelectedDirectory",
            typeof(DirectoryViewModel), null, (sender, e) => ((DirectoryTreeViewModel)sender).OnSelectedDirectoryChanged());

        /// <summary>
        /// Called when the SelectedDirectory property has changed.
        /// </summary>
        private void OnSelectedDirectoryChanged()
        {
            DirectoryViewModel item = SelectedDirectory;
            if (item != null)
            {
                DirectoryInfo di = item.DirectoryInfo;
                if (di != null)
                {
                    ExpandAndSelect(di);
                    Settings settings = Settings.Default;
                    settings.LastDirectoryExplorerSelectedPath = di.FullName;
                    settings.Save();
                }
            }
        }
        #endregion

        #region Command: UploadAllInThisDirectory
        /// <summary>
        /// Gets the UploadAllInThisDirectory command.
        /// </summary>
        public Command UploadAllInThisDirectory { get; private set; }

        /// <summary>
        /// Method to check whether the UploadAllInThisDirectory command can be executed.
        /// </summary>
        /// <returns><c>true</c> if the command can be executed; otherwise <c>false</c></returns>
        private bool OnUploadAllInThisDirectoryCanExecute()
        {
            return SelectedDirectory != null;
        }

        /// <summary>
        /// Method to invoke when the UploadAllInThisDirectory command is executed.
        /// </summary>
        private void OnUploadAllInThisDirectoryExecute()
        {
            DirectoryViewModel dvm = SelectedDirectory;
            if (dvm == null)
                return;
            DirectoryInfo di = dvm.DirectoryInfo;
            var t = new Thread(() =>
                               AddFilesInDirectory(di, false)
                              );
            t.Start();
        }
        #endregion

        #region Command: UploadAllRecursively
        /// <summary>
        /// Gets the UploadAllRecursively command.
        /// </summary>
        public Command UploadAllRecursively { get; private set; }

        /// <summary>
        /// Method to check whether the UploadAllRecursively command can be executed.
        /// </summary>
        /// <returns><c>true</c> if the command can be executed; otherwise <c>false</c></returns>
        private bool OnUploadAllRecursivelyCanExecute()
        {
            return SelectedDirectory != null;
        }

        /// <summary>
        /// Method to invoke when the UploadAllRecursively command is executed.
        /// </summary>
        private void OnUploadAllRecursivelyExecute()
        {
            DirectoryViewModel dvm = SelectedDirectory;
            if (dvm == null)
                return;
            DirectoryInfo di = dvm.DirectoryInfo;
            var t = new Thread(() =>
                               AddFilesInDirectory(di, true)
                              );
            t.Start();
        }
        #endregion

        #region Helpers
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

        private void AddFilesInDirectory(DirectoryInfo di, bool recurse)
        {
            foreach (FileInfo fi in di.EnumerateFiles())
            {
                _engine.CommandQueue.AddCommand(new AddFileCommand(_engine, fi));
            }
            if (recurse)
            {
                foreach (DirectoryInfo sdi in di.EnumerateDirectories())
                {
                    AddFilesInDirectory(sdi, true);
                }
            }
        }

        private void ExpandAndSelect(DirectoryInfo path)
        {
            DirectoryViewModel vm = Lookup(path);
            if (vm != null)
            {
                vm.IsExpanded = true;
                vm.IsSelected = true;
            }
        }
        #endregion
    }
}