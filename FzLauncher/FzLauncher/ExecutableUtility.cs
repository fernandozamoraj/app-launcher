using System.Drawing;
using System.IO;
using Shell32;

namespace FzLauncher
{
    public class ExecutableUtility
    {
        public static Icon IconFromFilePath(string filePath)
        {
            Icon result = null;

            try
            {
                if(File.Exists(filePath))
                    result = Icon.ExtractAssociatedIcon(filePath);
            }
            catch 
            {
                //Not much to do just ignore
                //This is a know issue with Java files
            }

            return result;
        }

        public static string GetShortcutTargetFile(string shortcutFilename)
        {
            string pathOnly = System.IO.Path.GetDirectoryName(shortcutFilename);
            string filenameOnly = System.IO.Path.GetFileName(shortcutFilename);

            Shell shell = new Shell();
            Folder folder = shell.NameSpace(pathOnly);
            FolderItem folderItem = folder.ParseName(filenameOnly);

            if (folderItem != null)
            {
                Shell32.ShellLinkObject link = (Shell32.ShellLinkObject)folderItem.GetLink;
                return link.Path;
            }

            return string.Empty;
        }

    }
}