﻿using Mvvm;
using Orimath.Core;
using Orimath.Plugins;
using Orimath.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Orimath
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Window_ContentRendered(object sender, EventArgs e)
        {
            await Dispatcher.Yield();
            var viewModel = (WorkspaceViewModel)DataContext;
            await Task.Run(viewModel.Initialize);

            foreach (var (viewModelType, (_, uiType)) in viewModel.ViewDefinitions)
            {
                var template = new DataTemplate(viewModelType)
                {
                    VisualTree = new FrameworkElementFactory(uiType),
                };
                Resources.Add(template.DataTemplateKey, template);
            }

            await Dispatcher.Yield();
            viewModel.LoadViewModels();

            foreach (var menuItem in viewModel.MenuItems)
                SetShortcutKey(menuItem);

            MainScrollViewer.ScrollToVerticalOffset((MainScrollViewer.ExtentHeight - MainScrollViewer.ActualHeight) / 2.0);
            MainScrollViewer.ScrollToHorizontalOffset((MainScrollViewer.ExtentWidth - MainScrollViewer.ActualWidth) / 2.0);

            void SetShortcutKey(MenuItemViewModel menuItem)
            {
                if (menuItem.ShortcutKey is { } gesture)
                    InputBindings.Add(new KeyBinding(menuItem.Command, gesture));

                foreach (var child in menuItem.Children)
                    SetShortcutKey(child);
            }
        }
    }
}
