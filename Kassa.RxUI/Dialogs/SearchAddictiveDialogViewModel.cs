﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Kassa.RxUI.Dialogs;

public class SearchAddictiveDialogViewModel : DialogViewModel
{
    public SearchAddictiveDialogViewModel(MainViewModel mainViewModel) : base(mainViewModel)
    {
        IsKeyboardVisible = false;

        var data = new AddictiveViewModel[]
        {
            new() { Name = "Сыр чеддер", Count = 15, Measure = "гр", IsAvailable = true},
            new() { Name = "Сыр моцарелла", Count = 15, Measure = "гр", IsAvailable = true},
            new() { Name = "Сыр пармезан", Count = 15, Measure = "гр", IsAvailable = true},
            new() { Name = "Сыр дор блю", Count = 15, Measure = "гр", IsAvailable = true},
            new() { Name = "Сыр фета", Count = 15, Measure = "гр", IsAvailable = false},
            new() { Name = "Сыр гауда", Count = 15, Measure = "гр", IsAvailable = true},
            new() { Name = "Сыр бри", Count = 15, Measure = "гр", IsAvailable = true},
            new() { Name = "Сыр голландский", Count = 15, Measure = "гр", IsAvailable = true},
            new() { Name = "Сыр голландский", Count = 15, Measure = "гр", IsAvailable = true},
            new() { Name = "Онигиро", Count = 15, Measure = "гр", IsAvailable = true},
            new() { Name = "Кетчуп", Count = 25, Measure = "гр", IsAvailable = true},
            new() { Name = "Майонез", Count = 25, Measure = "гр", IsAvailable = true},
            new() { Name = "Соус терияки", Count = 10, Measure = "гр", IsAvailable = false},
        }
        .AsObservableChangeSet();

        data.Bind(out _addictives)
            .Subscribe();

        FilteredAddcitves = new(_addictives);

        this.WhenAnyValue(x => x.SearchedText)
            .Skip(1) // fixing first blinking
            .Throttle(TimeSpan.FromMilliseconds(500))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(x =>
            {
                if (string.IsNullOrWhiteSpace(x))
                {
                    FilteredAddcitves.Clear();
                    FilteredAddcitves.AddRange(Addictives);
                    return;
                }
                var filtered = Addictives.Where(x => x.Name.Contains(SearchedText, StringComparison.CurrentCultureIgnoreCase));
                FilteredAddcitves.Clear();
                FilteredAddcitves.AddRange(filtered);
            });
    }

    [Reactive]
    public string SearchedText
    {
        get; set;
    } = null!;

    [Reactive]
    public bool IsKeyboardVisible
    {
        get; set;
    }

    public ReadOnlyObservableCollection<AddictiveViewModel> Addictives => _addictives;
    private readonly ReadOnlyObservableCollection<AddictiveViewModel> _addictives;

    public ObservableCollection<AddictiveViewModel> FilteredAddcitves
    {
        get; 
    }
}
