using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public static class StringExtension {

    public static string[] Split(this string p_string, string p_delimiter, string p_exclusion) {
        List<string> split = new List<string>();
        string currentString = p_string;
        int lastExclusionIndex = 0;

        while(currentString.Contains(p_delimiter)) {
            int exclusionIndexMin = currentString.IndexOf(p_exclusion);
            int exclusionIndexMax = exclusionIndexMin + p_exclusion.Length - 1;
            int delimiterIndex = currentString.IndexOf(p_delimiter, lastExclusionIndex);

            if(delimiterIndex >= exclusionIndexMin && delimiterIndex <= exclusionIndexMax &&
               exclusionIndexMin != -1) {
                lastExclusionIndex = exclusionIndexMax + 1;
            } else if(delimiterIndex < 0) break;
            else {
                string next = currentString.Substring(0, delimiterIndex);

                if(next.Any(x => !char.IsWhiteSpace(x))) split.Add(next);
                
                currentString = currentString.Remove(0, next.Length + p_delimiter.Length);
                lastExclusionIndex = 0;
            }
        }

        if(currentString.Length > 0) split.Add(currentString);

        return split.ToArray();
    }
}