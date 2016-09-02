// OpenCppCoverage is an open source code coverage for C++.
// Copyright (C) 2016 OpenCppCoverage
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace OpenCppCoverage.VSPackage.Helper
{
    /// <summary>
    /// Interaction logic for FileSystemSelectionControl.xaml
    /// </summary>
    public partial class FileSystemSelectionControl : UserControl
    {
        //-----------------------------------------------------------------------
        public FileSystemSelectionControl()
        {
            InitializeComponent();

            this.BrowseCommand = new RelayCommand(() =>
            {
                switch (this.Mode)
                {
                    case SelectionMode.FileSelection: SelectFile(); break;
                    case SelectionMode.FolderSelection: SelectFolder(); break;
                }
                this.textBox.Focus();
            });
        }

        //-----------------------------------------------------------------------
        public static readonly DependencyProperty SelectedPathProperty =
            DependencyProperty.Register(nameof(SelectedPath), typeof(string), typeof(FileSystemSelectionControl),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        //-----------------------------------------------------------------------
        public string SelectedPath
        {
            get { return (string)GetValue(SelectedPathProperty); }
            set { SetValue(SelectedPathProperty, value); }
        }

        //-----------------------------------------------------------------------
        public string FileFilter { get; set; }
        public SelectionMode Mode { get; set; }

        //-----------------------------------------------------------------------
        public enum SelectionMode
        {
            FileSelection,
            FolderSelection
        }

        //-----------------------------------------------------------------------
        public ICommand BrowseCommand { get; private set; }

        //-----------------------------------------------------------------------
        void SelectFile()
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = this.FileFilter;
            bool? userClickedOK = dialog.ShowDialog();

            if (userClickedOK != null && userClickedOK.Value)
                this.SelectedPath = dialog.FileName;
        }

        //-----------------------------------------------------------------------
        void SelectFolder()
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            var result = dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
                this.SelectedPath = dialog.SelectedPath;
        }
    }
}