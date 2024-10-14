﻿using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Reactive;
using System.Reflection;
using DynamicData;
using FurnaceEditor.Attributes;
using FurnaceEditor.Models;
using FurnaceEditor.Services;
using FurnaceEditor.Utilities.Loggers;
using FurnaceEditor.Utilities.Providers;
using Microsoft.Extensions.Logging;
using ReactiveUI;

namespace FurnaceEditor.ViewModels
{
    public class EditorWindowViewModel : ViewModelBase
    {
        public class WindowItem
        {
            public string Name { get; set; }
            public Type Type { get; set; }
        }

        public class LayoutResource
        {
            public string FileName { get; set; }
            public string Path { get; set; }
        }

        private LogMessage _lastLog;
        private WindowsService _windowsService;
        private readonly string _layoutsPrefix;
        private string _sceneName = "";
        private string _projectName = "";

        public ObservableCollection<WindowItem> WindowItems { get; private set; }
        public ObservableCollection<LayoutResource> Layouts { get; private set; }
        public LogMessage LastLog
        {
            get => _lastLog;
            set => this.RaiseAndSetIfChanged(ref _lastLog, value);
        }

        #region Commands
        public ReactiveCommand<WindowItem, Unit> OpenWindowCommand { get; private set; }
        public ReactiveCommand<LayoutResource, Unit> SelectLayoutCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> SaveLayout { get; private set; }
        public ReactiveCommand<Unit, Unit> SaveCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> UndoCommand { get; private set; }
        #endregion

        #region Properties
        public string SceneName { get => _sceneName; set => this.RaiseAndSetIfChanged(ref _sceneName, value); }
        public string ProjectName { get => _projectName; set => this.RaiseAndSetIfChanged(ref _projectName, value); }
        #endregion

        public EditorWindowViewModel(WindowsService windowsService, ProjectService projectService, ILogger<EditorWindowViewModel> logger, IObservableLoggerProvider loggerProvider)
        {
            _layoutsPrefix = $"{Assembly.GetExecutingAssembly().GetName().Name}.Resources.Layouts.";
            WindowItems = new();
            Layouts = new();
            _windowsService = windowsService;

            OpenWindowCommand = ReactiveCommand.Create<WindowItem>(item => OpenWindow(item));
            SelectLayoutCommand = ReactiveCommand.Create<LayoutResource>(item => SelectLayout(item));
            SaveLayout = ReactiveCommand.Create(_windowsService.Save);
            SaveCommand = ReactiveCommand.Create(() => { });
            UndoCommand = ReactiveCommand.Create(() => { });

            _windowsService.LoadLayout($"{_layoutsPrefix}Default.xml");
            LoadWindowItems();
            LoadAllLayoutResourceNames(_layoutsPrefix);

            MessageBus.Current.Listen<Project>().Subscribe(project =>
            {
                ProjectName = project.Name;
            });

            ProjectName = projectService.GetProject().Name;

            loggerProvider.Logs.CollectionChanged += (object? sender, NotifyCollectionChangedEventArgs e) =>
            {
                var collection = sender as ObservableCollection<LogMessage>;

                if (collection != null && collection.Count > 0)
                {
                    LastLog = collection[collection.Count - 1];
                }
                else
                {
                    LastLog = null;
                }
            };
        }

        private void LoadAllLayoutResourceNames(string folderName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceNames = assembly.GetManifestResourceNames();

            // Filter resource names to match the folderName and XML files
            var layoutResources = resourceNames
                .Where(name => name.StartsWith(_layoutsPrefix) && name.EndsWith(".xml"))
                .Select(name => new LayoutResource
                {
                    FileName = name.Substring(_layoutsPrefix.Length).Replace(".xml", ""),
                    Path = name
                })
                .ToList();

            Layouts.AddRange(layoutResources);
        }

        private void LoadWindowItems()
        {
            var windowTypes = Assembly.GetExecutingAssembly()
                                      .GetTypes()
                                      .Where(t => t.GetCustomAttributes<WindowAttribute>().Any());

            foreach (var type in windowTypes)
            {
                var attribute = type.GetCustomAttribute<WindowAttribute>();
                if (attribute != null)
                {
                    WindowItems.Add(new WindowItem { Name = attribute.Name, Type = type });
                }
            }
        }

        private void OpenWindow(WindowItem windowItem)
        {
            _windowsService.ShowFloatingWindow(windowItem.Type);
        }

        private void SelectLayout(LayoutResource layoutResource)
        {
            _windowsService.LoadLayout(layoutResource.Path);
        }
    }
}
