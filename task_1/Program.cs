using System.Runtime.CompilerServices;

// paths separated by commas
string filePathsString = "./files/file1.txt,./files/file2.txt,./files/file3.txt,./files/file4.txt";

string search = "*BGAA*";

string[] filePaths = filePathsString.Split(',');
List<Task<bool>> tasks = new();

foreach (var filePath in filePaths)
{
    if (!string.IsNullOrEmpty(filePath))
        tasks.Add(Task.Run(() => SearchStringInFile(filePath, search)));
}

await Task.WhenAll(tasks);

for (int i = 0; i < tasks.Count; i++)
{
    Console.WriteLine($"File {filePaths[i]}: {tasks[i].Result}");
}

static bool SearchStringInFile(string filePath, string searchString)
{
    try
    {

        var content = File.ReadAllText(filePath);

        return content.Contains(searchString);

    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error reading file {filePath}: {ex.Message}");
        return false;
    }
}