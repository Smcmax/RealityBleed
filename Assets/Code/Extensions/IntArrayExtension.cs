using UnityEngine;

public static class IntArrayExtension {

	public static int[] Compare(this int[] p_first, int[] p_second) {
		int longest = p_first.Length > p_second.Length ? p_first.Length : p_second.Length;
		int[] result = new int[longest];

		for(int i = 0; i < longest; i++) {
			int res = 0;

			if(p_first.Length > i && p_second.Length > i)
				res = p_first[i] - p_second[i];
			else if(p_first.Length > i) res = p_first[i];
			else if(p_second.Length > i) res = p_second[i];

			result[i] = res;
		}

		return result;
	}
}