namespace Elster.Common.Folders;

public class FoldersUtils
{
    /// <summary>
    /// Returns the path to the user's configuration folder.
    /// No program should be writing to this folder. Use the <see cref="GetElsterCoreConfigPath"/> method instead.
    /// This will not validate if the directory exists or not.
    /// </summary>
    /// <returns>The full path to the user's config folder.</returns>
    public static string GetConfigPath()
    {
        return Path.GetFullPath(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
        );
    }

    /// <summary>
    /// Return the path to Elster's core configuration folder.
    /// This is where the core settings should be stored.
    /// </summary>
    /// <param name="ensureExistence">Whether to ensure the folder exists.</param>
    /// <returns>The full path to the Elster's config folder.</returns>
    public static string GetElsterCoreConfigPath(bool ensureExistence)
    {
        string path = Path.GetFullPath(Path.Combine(GetConfigPath(), "Elster"));
        if (ensureExistence) {
            Directory.CreateDirectory(path);
        }
        return path;
    }
}
