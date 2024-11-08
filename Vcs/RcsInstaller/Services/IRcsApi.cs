using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using Refit;
using System.Threading.Tasks;

namespace RcsInstaller.Services;

public interface IRcsApi
{
    [Get("/api/rcsvc/install")]
    public Task<HttpContent> InstallLatest(string? version = null);

    [Get("/api/rcsvc/version")]
    public Task<Version> GetVersion();
}
