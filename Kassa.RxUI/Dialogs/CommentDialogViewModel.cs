﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using Kassa.RxUI.Pages;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Kassa.RxUI.Dialogs;
public class CommentDialogViewModel : DialogViewModel
{
    public CommentDialogViewModel(IOrderEditVm orderEditVm) : base()
    {
        OrderEditVm = orderEditVm;


        PublishCommentCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            IsPublished = true;

            await CloseAsync();
            
            return Comment;
        });
    }

    public IOrderEditVm OrderEditVm
    {
        get;
    }

    [Reactive]
    public string? Comment
    {
        get; set;
    }

    [Reactive]
    public bool IsKeyboardVisible
    {
        get; set;
    }

    public bool IsPublished
    {
        get; set;
    }

    public ReactiveCommand<Unit, string?> PublishCommentCommand
    {
        get;
    }
}
