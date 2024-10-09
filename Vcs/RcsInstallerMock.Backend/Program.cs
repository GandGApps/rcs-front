using System.IO.Compression;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using RcsInstallerMock.Backend.Models;
using RcsInstallerMock.Backend.Services;
using RcsVersionControlMock;
using RcsVersionControlMock.DataAccess;
using RcsVersionControlMock.Json;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

builder.Services.AddAntiforgery(options =>
{
    options.SuppressXFrameOptionsHeader = true;
});

var rcsBinName = builder.Configuration["RcsBinName"];

if (string.IsNullOrWhiteSpace(rcsBinName))
{
    throw new NotSupportedException("RcsBinName is not set");
}

builder.Services.AddJsonRcsVersionControl(Path.Combine(builder.Environment.WebRootPath, "RcsBin", rcsBinName));
builder.Services.AddSingleton<ICachedZips>(provider =>
{
    var webHostEnvironment = provider.GetRequiredService<IWebHostEnvironment>();
    var cacheFilePath = Path.Combine(webHostEnvironment.WebRootPath, "versions_zippath_pair.json");
    return new CachedZips(cacheFilePath);
});

var app = builder.Build();

app.UseAntiforgery();

var rcsvcApi = app.MapGroup("api/rcsvc");

rcsvcApi.MapPost("/update", [RequestSizeLimit(250_000_000), IgnoreAntiforgeryToken] async Task<IResult> (
    [FromServices] IRcsVersionControl rcsVersionControl,
    [FromServices] IWebHostEnvironment webHostEnvironment,
    [FromServices] ILogger<Program> logger,
    IFormFile zipFile) =>
{
    if (zipFile is null || zipFile.Length == 0)
    {
        logger.LogWarning("No file was uploaded or the file is empty.");
        return Results.BadRequest();
    }

    if (!zipFile.FileName.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
    {
        logger.LogWarning("The uploaded file is not a ZIP archive.");
        return Results.BadRequest(new BadRequestWithMessage("File must be a ZIP archive."));
    }

    var zipFileName = $"{Guid.NewGuid()}.zip";

    var rcsBinDirectory = Path.Combine(webHostEnvironment.WebRootPath, "RcsBin");
    var zipFilePath = Path.Combine(rcsBinDirectory, zipFileName);

    try
    {
        logger.LogInformation("Saving uploaded ZIP file to temporary path: {ZipFilePath}", zipFilePath);

        // Ensure the RcsBin directory exists
        if (!Directory.Exists(rcsBinDirectory))
        {
            Directory.CreateDirectory(rcsBinDirectory);
            logger.LogInformation("Created directory: {RcsBinDirectory}", rcsBinDirectory);
        }

        // Save the uploaded file to a temporary location
        using (var stream = new FileStream(zipFilePath, FileMode.Create))
        {
            await zipFile.CopyToAsync(stream);
        }
        logger.LogInformation("ZIP file saved: {ZipFilePath}", zipFilePath);

        // Extract the ZIP file to the RcsBin directory
        logger.LogInformation("Extracting ZIP file {ZipFilePath} to directory {RcsBinDirectory}", zipFilePath, rcsBinDirectory);
        ZipFile.ExtractToDirectory(zipFilePath, rcsBinDirectory, overwriteFiles: true);
        logger.LogInformation("Extraction completed.");

        // Delete the temporary ZIP file
        File.Delete(zipFilePath);
        logger.LogInformation("Temporary ZIP file deleted: {ZipFilePath}", zipFilePath);

        // Update the version control
        logger.LogInformation("Updating version control via IRcsVersionControl.");
        await rcsVersionControl.Update();
        logger.LogInformation("Version control update completed.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while processing the uploaded ZIP file.");
        return Results.StatusCode(500);
    }
    finally
    {
        // Delete the temporary ZIP file if it still exists
        if (File.Exists(zipFilePath))
        {
            File.Delete(zipFilePath);
            logger.LogInformation("Temporary ZIP file deleted in finally block: {ZipFilePath}", zipFilePath);
        }
    }

    logger.LogInformation("Update process completed successfully.");
    return Results.Ok();
}).DisableAntiforgery();

rcsvcApi.MapGet("/install", [RequestSizeLimit(250_000_000), IgnoreAntiforgeryToken] async Task<IResult> (
    [FromServices] IRcsVersionControl rcsVersionControl,
    [FromServices] IWebHostEnvironment webHostEnvironment,
    [FromServices] ILogger<Program> logger,
    [FromServices] ICachedZips cachedZips,
    string? version) =>
{
    IEnumerable<VersionChangeNode> versionChanges;
    Version a;
    Version b;

    if (string.IsNullOrWhiteSpace(version))
    {
        versionChanges = await rcsVersionControl.GetAllValidChanges();

        if (!versionChanges.Any())
        {
            return Results.BadRequest(new BadRequestWithMessage("No valid versions found"));
        }

        a = new(0, 0);
        b = (await rcsVersionControl.VersionStore.GetCurrentVersion())!;
    }
    else
    {
        if (!Version.TryParse(version, out var parsedVersion))
        {
            return Results.BadRequest(new BadRequestWithMessage("Invalid version"));
        }

        versionChanges = await rcsVersionControl.GetChangesFromCurrent(parsedVersion);

        a = parsedVersion;
        b = (await rcsVersionControl.VersionStore.GetCurrentVersion())!;
    }

    var zipStream = await PrepareZipFile(versionChanges, a, b);

    return Results.File(zipStream, contentType: "application/zip", fileDownloadName: $"{a}_to_{b}.zip");

    // Локальная функция для подготовки ZIP-файла
    async Task<FileStream> PrepareZipFile(IEnumerable<VersionChangeNode> versionChangeNodes, Version a, Version b)
    {
        if (a > b)
        {
            (a, b) = (b, a);
        }

        logger.LogInformation("Preparing zip file for versions {VersionA} and {VersionB}", a, b);

        var cachedZipStream = await cachedZips.GetCachedZipAsync(a, b);

        if (cachedZipStream != null)
        {
            logger.LogInformation("Using cached zip file for versions {VersionA} and {VersionB}", a, b);
            return cachedZipStream;
        }

        var folderName = Guid.NewGuid().ToString();
        var tempFolderPath = Path.Combine(webHostEnvironment.WebRootPath, folderName);

        logger.LogDebug("Creating temporary directory {TempFolderPath}", tempFolderPath);

        Directory.CreateDirectory(tempFolderPath);

        using (var changesJsonFile = File.Create(Path.Combine(tempFolderPath, "changes.json")))
        {
            await JsonSerializer.SerializeAsync(changesJsonFile, versionChangeNodes, RcsVCConstants.JsonSerializerOptions);
        }

        logger.LogInformation("Copying files to temporary directory {TempFolderPath}", tempFolderPath);

        foreach (var versionChangeNode in versionChangeNodes)
        {
            if (versionChangeNode.ChangeType == VersionChangeNodeAction.Delete)
            {
                continue;
            }

            var jsonRepresentation = JsonSerializer.Serialize(versionChangeNode, RcsVCConstants.WriteIndentededJsonSerializerOptions);
            logger.LogDebug($"Processing {nameof(VersionChangeNode)}: \n {{{nameof(VersionChangeNode)}}}", jsonRepresentation);

            var filePath = Path.Combine(webHostEnvironment.WebRootPath, "RcsBin", versionChangeNode.Path);
            logger.LogDebug("Check Path Behavior: \n Path.Combine(\"{WebHostEnvironment.WebRootPath}\", \"{RcsBin}\", \"{VersionChangeNode.Path}\") = {PathCombineResult}",
                webHostEnvironment.WebRootPath,
                "RcsBin",
                versionChangeNode.Path,
                filePath);

            var tempFilePath = Path.Combine(tempFolderPath, versionChangeNode.Path);
            var directory = Path.GetDirectoryName(tempFilePath) ?? throw new InvalidOperationException($"Directory not found for path: {tempFolderPath}");

            if (!string.IsNullOrWhiteSpace(directory) && !Directory.Exists(directory))
            {
                logger.LogDebug("Creating directory {Directory}", directory);
                Directory.CreateDirectory(directory);
            }

            logger.LogDebug("Copying file {SourceFile} to {DestinationFile}", filePath, tempFilePath);

            using var sourceFile = File.OpenRead(filePath);
            using var destinationFile = File.Create(tempFilePath);

            await sourceFile.CopyToAsync(destinationFile);

            logger.LogDebug("File copied: {DestinationFile}", tempFilePath);
        }

        var zipFilePath = Path.Combine(webHostEnvironment.WebRootPath, $"{folderName}.zip");

        logger.LogInformation("Creating zip file {ZipFilePath}", zipFilePath);
        ZipFile.CreateFromDirectory(tempFolderPath, zipFilePath);
        logger.LogInformation("Zip file created at {ZipFilePath}", zipFilePath);

        await cachedZips.SaveZip(a, b, zipFilePath, TimeSpan.FromDays(1));

        Directory.Delete(tempFolderPath, true);

        return File.OpenRead(zipFilePath);
    }
}).DisableAntiforgery();

rcsvcApi.MapGet("/version", async Task<IResult>(
    [FromServices] IRcsVersionControl rcsVersionControl
    ) =>
{
    var current = await rcsVersionControl.GetCurrentVersion();


    return Results.Ok(current);
}).DisableAntiforgery();

await using (var scope = app.Services.CreateAsyncScope())
{
    var rcsvc = scope.ServiceProvider.GetRequiredService<IRcsVersionControl>();

    if (await rcsvc.GetCurrentVersion() is null)
    {
        await rcsvc.Update();
    }
}

app.Run();


[JsonSerializable(typeof(List<ZipCacheEntry>))]
[JsonSerializable(typeof(List<BadRequestWithMessage>))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{

}

internal sealed record BadRequestWithMessage(string Message);
