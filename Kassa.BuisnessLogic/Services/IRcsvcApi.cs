using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Refit;

namespace Kassa.BuisnessLogic.Services;
public interface IRcsvcApi
{
    [Get("/rcsvc/install")]
    public Task<HttpContent> InstallLatest(string? version = null);

    [Get("/rcsvc/version")]
    public Task<Version> GetVersion();
}
