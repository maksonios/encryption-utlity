using Ionic.Zip;
using Microsoft.Extensions.Caching.Memory;

namespace EncryptionUtility.Services;

public class ArchiveHelperService
{
    private const string ArchiveName = "archive.zip";
    
    private readonly IMemoryCache _memoryCache;

    public ArchiveHelperService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public FileNameInfo CreateArchive(FileInfo[] files)
    {
        var stream = new MemoryStream();
        using (var zip = new ZipFile())
        {
            zip.Password = "yourPassword";  // Set your password here
            zip.Encryption = EncryptionAlgorithm.WinZipAes256;

            foreach (var file in files) 
                zip.AddEntry(file.Name, file.File);

            zip.Save(stream);
        }
        
        var fileId = Guid.NewGuid().ToString();
        var fileInfo = new FileInfo(ArchiveName, stream.ToArray());
        _memoryCache.Set(fileId, fileInfo, TimeSpan.FromMinutes(5));
        return new FileNameInfo(fileId, ArchiveName);
    }

    public FileInfo? TryGetFile(string fileId)
    {
        return _memoryCache.TryGetValue(fileId, out FileInfo? file) ? file : null;
    }
}