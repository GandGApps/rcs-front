using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic.Dto;
using Riok.Mapperly.Abstractions;

namespace Kassa.RxUI;

[Mapper]
public static partial class Mapper
{

    [MapperIgnoreSource(nameof(ClientViewModel.Changed))]
    [MapperIgnoreSource(nameof(ClientViewModel.Changing))]
    [MapperIgnoreSource(nameof(ClientViewModel.ThrownExceptions))]
    public static partial ClientDto ClientVmToDto(this ClientViewModel viewModel);
}
