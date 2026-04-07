using System.Text.Json;

namespace StudyManager.Repositories;

internal sealed class JsonStorage
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true
    };

    private readonly string _filePath;

    public JsonStorage(string? filePath = null)
    {
        var dataDirectory = Path.Combine(AppContext.BaseDirectory, "Data");
        Directory.CreateDirectory(dataDirectory);

        _filePath = filePath ?? Path.Combine(dataDirectory, "study-manager-storage.json");
    }

    public string FilePath => _filePath;

    public bool Exists()
    {
        return File.Exists(_filePath);
    }

    public async Task<StorageSnapshot> LoadAsync(CancellationToken cancellationToken = default)
    {
        if (!File.Exists(_filePath))
        {
            return new StorageSnapshot();
        }

        await using var stream = File.Open(_filePath, FileMode.Open, FileAccess.Read, FileShare.Read);

        var snapshot = await JsonSerializer.DeserializeAsync<StorageSnapshot>(
            stream,
            SerializerOptions,
            cancellationToken).ConfigureAwait(false);

        return snapshot ?? new StorageSnapshot();
    }

    public async Task SaveAsync(StorageSnapshot snapshot, CancellationToken cancellationToken = default)
    {
        var directory = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        await using var stream = File.Create(_filePath);

        await JsonSerializer.SerializeAsync(
            stream,
            snapshot,
            SerializerOptions,
            cancellationToken).ConfigureAwait(false);

        await stream.FlushAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task EnsureCreatedWithSeedAsync(
        Func<StorageSnapshot> seedFactory,
        CancellationToken cancellationToken = default)
    {
        if (File.Exists(_filePath))
        {
            return;
        }

        var snapshot = seedFactory();
        await SaveAsync(snapshot, cancellationToken).ConfigureAwait(false);
    }
}