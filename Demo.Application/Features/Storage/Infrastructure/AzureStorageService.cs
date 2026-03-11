using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;
using Demo.Application.Domain.Settings;
using Demo.Application.Features.Storage.Interfaces;
using System.Net;

namespace Demo.Application.Features.Storage.Infrastructure;

public class AzureStorageService(IOptions<DemoSettings> settings, ILogger<AzureStorageService> logger) : IStorageService
{
    private readonly DemoSettings _settings = settings.Value;

    /// <summary>
    /// Uploads a blob to a specified folder in the organization.
    /// </summary>
    /// <param name="organizationId">Owning Organization ID</param>
    /// <param name="folderName">Folder name to upload blob to</param>
    /// <param name="blobName">Name of the blob</param>
    /// <param name="contentType">Content type of the file</param>
    /// <param name="file">File stream with contents of blob</param>
    /// <returns>Result URL of uploaded blob</returns>
    public async Task<Result<string>> UploadBlobToOrganizationAsync(long organizationId, string folderName, string blobName, string contentType, Stream file)
    {
        logger.LogDebug($"Params: OrganizationId={organizationId}, FolderName={folderName}, BlobName={blobName}, ContentType={contentType}, FileLength={file.Length}");

        try
        {
            BlobContainerClient container = GetOrganizationContainer(organizationId);
            string policyName = GetOrganizationPolicyName(organizationId);

            Result<string> result = await UploadBlobAsync(container, folderName, blobName, policyName, contentType, file);

            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred uploading the file");
        }

        return Result.Failed<string>("An error occurred uploading the file");
    }

    /// <summary>
    /// Deletes a blob based on the url of the blob in storage
    /// </summary>
    /// <param name="organizationId">Owning Organization ID</param>
    /// <param name="folderName">Name of the folder the blob is in</param>
    /// <param name="url">URL of the blob</param>
    /// <returns>Flag if blob was deleted</returns>
    public async Task<Result<Empty>> DeleteBlobInOrganizationFromUrlAsync(long organizationId, string folderName, string url)
    {
        logger.LogDebug($"Params: OrganizationId={organizationId}, FolderName={folderName}, url={url}");

        try
        {
            BlobContainerClient container = GetOrganizationContainer(organizationId);
            Result<Empty> result = await DeleteBlobFromUrlAsync(container, folderName, url);

            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred deleting the file");
        }

        return Result.Failed<Empty>("An error occurred deleting the file");
    }

    /// <summary>
    /// Uploads a blob to a folder in a container.
    /// </summary>
    /// <param name="container">Container to load the blob in</param>
    /// <param name="folderName">Optional folder name to upload blob to</param>
    /// <param name="blobName">Name of the blob</param>
    /// <param name="policyName">Name of the policy to apply</param>
    /// <param name="contentType">Content type of the file</param>
    /// <param name="file">File stream with contents of blob</param>
    /// <returns>Result URL of uploaded blob</returns>
    private async Task<Result<string>> UploadBlobAsync(BlobContainerClient container, string folderName, string blobName, string policyName, string contentType, Stream file)
    {
        logger.LogDebug($"Params: FolderName={folderName}, BlobName={blobName}, ContentType={contentType}, FileLength={file?.Length}");

        try
        {
            string name = GenerateBlobName(folderName, blobName);
            BlobClient blob = container.GetBlobClient(name);
            if (blob is not null)
            {
                await blob.UploadAsync(file);
                await SetBlobPropertiesAsync(blob, contentType);
                Uri uri = GetServiceSasUriForBlob(blob, policyName);

                return Result.Success(uri.AbsoluteUri);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred uploading the file");
        }

        return Result.Failed<string>("An error occurred uploading the file");
    }

    /// <summary>
    /// Deletes a blob based on the url of the blob in storage
    /// </summary>
    /// <param name="container">Container the blob is in</param>
    /// <param name="folderName">Name of the folder the blob is in</param>
    /// <param name="url">URL of the blob</param>
    /// <returns>Result with Empty</returns>
    private async Task<Result<Empty>> DeleteBlobFromUrlAsync(BlobContainerClient container, string folderName, string url)
    {
        logger.LogDebug($"Params: FolderName={folderName}, url={url}");

        try
        {
            string blobName = GetBlobNameFromUrl(url);

            return await DeleteBlobsWithPrefixAsync(container, $"{folderName}/{blobName}");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred deleting the file");

            return Result.Failed<Empty>("An error occurred deleting the file");
        }
    }

    /// <summary>
    /// Deletes blobs in a container matching the prefix
    /// </summary>
    /// <param name="container">Container</param>
    /// <param name="prefix">Prefix of the blobs in the container to delete</param>
    /// <returns>Result with Empty</returns>
    private async Task<Result<Empty>> DeleteBlobsWithPrefixAsync(BlobContainerClient container, string prefix)
    {
        logger.LogDebug($"Params: prefix={prefix}");

        try
        {
            GetBlobsByHierarchyOptions options = new()
            {
                Delimiter = "/",
                Prefix = prefix
            };
            var pages = container.GetBlobsByHierarchyAsync(options).AsPages();

            await foreach (var page in pages)
            {
                foreach (var blob in page.Values.Where(item => item.IsBlob).Select(item => item.Blob))
                {
                    await container.DeleteBlobIfExistsAsync(blob.Name);
                }
            }

            return Result.Success<Empty>();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred deleting the blobs with a prefix");

            return Result.Failed<Empty>("An error occurred deleting the blobs with a prefix");
        }
    }

    /// <summary>
    /// Generates a unique name the blob will be saved as.
    /// </summary>
    /// <param name="folderName">Optional name of the folder to store the blob in</param>
    /// <param name="blobName">Original name of the blob</param>
    /// <returns>Blob name to use</returns>
    private string GenerateBlobName(string folderName, string blobName)
    {
        string name = !string.IsNullOrWhiteSpace(folderName) ? $"{folderName}/{blobName}" : blobName;
        string filename = Path.GetFileNameWithoutExtension(name).Replace(".", "_").Replace(" ", "-");
        string extension = Path.GetExtension(name);
        name = !string.IsNullOrWhiteSpace(folderName) ? $"{folderName}/{filename}-{DateTime.Now.Ticks}{extension}" : $"{filename}-{DateTime.Now.Ticks}{extension}";

        return name;
    }

    /// <summary>
    /// Gets the name of the blob from the URL
    /// </summary>
    /// <param name="url">URL of the blob</param>
    /// <returns>Blob name</returns>
    private string GetBlobNameFromUrl(string url)
    {
        int questionMarkIndex = url.LastIndexOf('?');
        int lastSlashIndex = url.LastIndexOf('/', questionMarkIndex);
        string blobName = url.Substring(lastSlashIndex + 1, questionMarkIndex - lastSlashIndex - 1);
        blobName = WebUtility.UrlDecode(blobName);

        return blobName;
    }

    /// <summary>
    /// Gets the storage container for the organization
    /// </summary>
    /// <param name="organizationId">Id of the organization</param>
    /// <returns>BlobContainerClient</returns>
    private BlobContainerClient GetOrganizationContainer(long organizationId)
    {
        var containerName = GetOrganizationContainerName(organizationId);
        var container = new BlobContainerClient(_settings.Storage.ConnectionString, containerName);

        return container;
    }

    /// <summary>
    /// Gets the name of the container for the organization
    /// </summary>
    /// <param name="id">ID of the organization</param>
    /// <returns>Name of the container</returns>
    private string GetOrganizationContainerName(long id) => $"organization-{id}";

    /// <summary>
    /// Gets the name of the access policy that will be applied to the organization's storage container
    /// </summary>
    /// <param name="id">ID of the organization</param>
    /// <returns>Name of the policy</returns>
    private string GetOrganizationPolicyName(long id) => $"organization-{id}-policy";

    /// <summary>
    /// Sets the blob properties
    /// </summary>
    /// <param name="blob">Blob</param>
    /// <param name="contentType">Content type of the blob</param>
    private async Task SetBlobPropertiesAsync(BlobClient blob, string contentType)
    {
        logger.LogDebug("Setting blob properties");

        try
        {
            // Get the existing properties
            BlobProperties properties = await blob.GetPropertiesAsync();

            BlobHttpHeaders headers = new()
            {
                // Set the MIME ContentType
                ContentType = contentType,
                ContentLanguage = "en-us",

                // Populate remaining headers with the pre-existing properties
                CacheControl = properties.CacheControl,
                ContentDisposition = properties.ContentDisposition,
                ContentEncoding = properties.ContentEncoding,
                ContentHash = properties.ContentHash
            };

            // Set the blob's properties.
            await blob.SetHttpHeadersAsync(headers);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred setting the blob properties");
        }
    }

    /// <summary>
    /// Gets the SAS URI for the blob.
    /// </summary>
    /// <param name="blob">Blob</param>
    /// <param name="storedPolicyName">Policy name</param>
    /// <returns>Uri</returns>
    private Uri GetServiceSasUriForBlob(BlobClient blob, string storedPolicyName)
    {
        logger.LogDebug("Get SAS URI for blob");

        // Check whether this BlobClient object has been authorized with Shared Key.
        if (blob.CanGenerateSasUri)
        {
            // Create a SAS token that's valid for one hour.
            var sasBuilder = new BlobSasBuilder()
            {
                BlobContainerName = blob.GetParentBlobContainerClient().Name,
                BlobName = blob.Name,
                Resource = "b",
                Identifier = storedPolicyName
            };

            Uri sasUri = blob.GenerateSasUri(sasBuilder);

            return sasUri;
        }
        else
        {
            logger.LogDebug("Blob cannot generate SAS Uri");
            return blob.Uri;
        }
    }
}
