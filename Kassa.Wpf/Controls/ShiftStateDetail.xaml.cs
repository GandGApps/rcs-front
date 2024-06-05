﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
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
using Kassa.BuisnessLogic;
using Kassa.BuisnessLogic.ApplicationModelManagers;
using Kassa.BuisnessLogic.Dto;
using Kassa.BuisnessLogic.Services;
using ReactiveUI;
using Splat;

namespace Kassa.Wpf.Controls;
/// <summary>
/// Interaction logic for ShiftStateDetail.xaml
/// </summary>
public sealed partial class ShiftStateDetail : UserControl, IApplicationModelPresenter<ShiftDto>, IActivatableView
{
    public static readonly DependencyPropertyKey ShiftDtoProperty = DependencyProperty.RegisterReadOnly(
        "ShiftDto",
        typeof(ShiftDto),
        typeof(ShiftStateDetail),
        new PropertyMetadata(null, ShiftDtoPropertyChanged));

    private static void ShiftDtoPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is ShiftStateDetail shiftStateDetail)
        {
            ShiftDtoPropertyChanged(shiftStateDetail, (ShiftDto)e.NewValue);
        }
    }

    private static async void ShiftDtoPropertyChanged(ShiftStateDetail shiftStateDetail, ShiftDto dto)
    {
        shiftStateDetail._subcribeToDtoChanging?.Dispose();

        var shiftService = Locator.Current.GetNotInitializedService<IShiftService>();
        var memberService = Locator.Current.GetNotInitializedService<IMemberService>();

        if (dto is null)
        {
            shiftStateDetail.ShiftNumber.Text = string.Empty;
            shiftStateDetail.ShiftBegin.Text = string.Empty;
            shiftStateDetail.ManagerName.Text = string.Empty;
            shiftStateDetail.CashierName.Text = string.Empty;
            shiftStateDetail.ShiftState.Text = string.Empty;
            return;
        }

        shiftStateDetail.ShiftNumber.Text = dto.Number.ToString();
        shiftStateDetail.ShiftBegin.Text = dto.Start is null ? string.Empty : dto.Start.Value.ToString("dd.MM.yyyy | HH:mm");
        shiftStateDetail.ManagerName.Text = (await memberService.GetMember(dto.ManagerId ?? Guid.Empty))?.Name ?? "???";
        shiftStateDetail.CashierName.Text = (await memberService.GetMember(dto.MemberId))?.Name ?? "???";
        shiftStateDetail.ShiftState.Text = dto.Start is null ? " открыта " : " закрыта ";

        shiftStateDetail._subcribeToDtoChanging = shiftService.RuntimeShifts.AddPresenter(shiftStateDetail);
    }

    void IApplicationModelPresenter<ShiftDto>.ModelChanged(Change<ShiftDto> change)
    {
        var model = change.Current;

        ShiftDto = model;

        ShiftDtoPropertyChanged(this, model);
    }

    void IDisposable.Dispose()
    {

    }

    private IDisposable? _subcribeToDtoChanging;

    public ShiftDto? ShiftDto
    {

        get => (ShiftDto?)GetValue(ShiftDtoProperty.DependencyProperty);
        private set => SetValue(ShiftDtoProperty, value);
    }
    Guid IApplicationModelPresenter<ShiftDto>.Id => ShiftDto.Id;

    public ShiftStateDetail()
    {
        InitializeComponent();

        var shiftService = Locator.Current.GetNotInitializedService<IShiftService>();

        this.WhenActivated(disposables =>
        {

            shiftService.CurrentShift.Subscribe(async shift =>
            {
                if (shift is null)
                {
                    ShiftDto = null;
                    return;
                }

                ShiftDto = await shift.CreateDto();
            }).DisposeWith(disposables);

        });

        
    }
}
