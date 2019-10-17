using System.Collections.Generic;
using System.IO;

public static class FileSearch {

    public static void RecursiveRetrieval(string p_path, ref List<string> p_files) {
        foreach(string file in Directory.GetFiles(p_path))
            if(!p_files.Contains(file))
                p_files.Add(file);

        foreach(string dir in Directory.GetDirectories(p_path))
            RecursiveRetrieval(dir, ref p_files);
    }

}
