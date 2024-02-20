using System.Runtime.CompilerServices;

// Generate paths
string filePathsString = "";
for (int i = 0; i < 8; i++)
{
    filePathsString += $"./files/file{i}.txt,";
}

string search = "*BGAA*";

int maxFilesPerBucket  = Environment.ProcessorCount;
// int maxFilesPerBucket  = 4;
string[] filePaths = filePathsString.Split(',');
filePaths = filePaths.Where(filePath => !string.IsNullOrEmpty(filePath)).ToArray();

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

    int remainingFiles = filePaths.Length % maxFilesPerBucket;
    bool lastBucket = false;

    for (int i = 0; i < numBuckets; i++)
    {
        if (remainingFiles > 0 && i == numBuckets - 2)
        {
            lastBucket = true;
            yield return filePaths.Skip(i * maxFilesPerBucket).Take(maxFilesPerBucket + remainingFiles);
        }
        else if (!lastBucket)
        {
            yield return filePaths.Skip(i * maxFilesPerBucket).Take(maxFilesPerBucket);
        }
    }
}