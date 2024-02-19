using System.Runtime.CompilerServices;

// Generate paths
string filePathsString = "";
for (int i = 0; i < 100; i++)
{
    filePathsString += $"./files/file{i}.txt,";
}

string search = "*BGAA*";

int maxFilesPerBucket  = Environment.ProcessorCount;
string[] filePaths = filePathsString.Split(',');

var buckets = BucketizeFilePaths(filePaths, maxFilesPerBucket);

foreach (var bucket in buckets)
{
    List<Task<bool>> tasks = new();
    foreach (var filePath in bucket)
    {
        if (!string.IsNullOrEmpty(filePath))
            tasks.Add(Task.Run(() => SearchStringInFileAsync(filePath, search)));
    }

    await Task.WhenAll(tasks);

    for (int i = 0; i < tasks.Count; i++)
    {
        var bucketList = bucket.ToList();
        Console.WriteLine($"File path: {bucketList[i]} - Files contain string: {tasks[i].Result}");
    }

    Console.WriteLine($"NEXT BUCKET");
}

static async Task<bool> SearchStringInFileAsync(string filePath, string searchString)
{
    try
    {
        var content = await File.ReadAllTextAsync(filePath);
        return content.Contains(searchString);
    }
    catch (Exception ex)
    {
        // Console.WriteLine($"Error reading file {filePath}: {ex.Message}");
        return false;
    }
}

static IEnumerable<IEnumerable<string>> BucketizeFilePaths(string[] filePaths, int maxFilesPerBucket)
{
    int numBuckets = (int)Math.Ceiling((double)filePaths.Length / maxFilesPerBucket);

    for (int i = 0; i < numBuckets; i++)
    {
        yield return filePaths.Skip(i * maxFilesPerBucket).Take(maxFilesPerBucket);
    }
}