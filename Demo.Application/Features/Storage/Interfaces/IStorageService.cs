namespace Demo.Application.Features.Storage.Interfaces;

public interface IStorageService
{
    /// <summary>
    /// Uploads a blob to a specified folder in the organization.
    /// </summary>
    /// <param name="organizationId">Owning Organization ID</param>
    /// <param name="folderName">Folder name to upload blob to</param>
    /// <param name="blobName">Name of the blob</param>
    /// <param name="contentType">Content type of the file</param>
    /// <param name="file">File stream with contents of blob</param>
    /// <returns>Result URL of uploaded blob</returns>
    Task<Result<string>> UploadBlobToOrganizationAsync(long organizationId, string folderName, string blobName, string contentType, Stream file);

    /// <summary>
    /// Deletes a blob based on the url of the blob in storage
    /// </summary>
    /// <param name="organizationId">Owning Organization ID</param>
    /// <param name="folderName">Name of the folder the blob is in</param>
    /// <param name="url">URL of the blob</param>
    /// <returns>Flag if blob was deleted</returns>
    Task<Result<Empty>> DeleteBlobInOrganizationFromUrlAsync(long organizationId, string folderName, string url);
}
