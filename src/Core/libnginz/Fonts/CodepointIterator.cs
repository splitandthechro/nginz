using System;

namespace nginz
{
	/// <summary>
	/// Iterates through a string properly for display.
	/// Handles double width codepoints
	/// </summary>
	struct CodepointIterator
	{
		readonly string str;

		int strIndex;
		public int Index;
		public int Count;
		public uint Codepoint;

		public uint PeekNext () {
			return Index >= Count - 1 ? 0 : (uint) char.ConvertToUtf32 (str, strIndex);
		}

		public bool Iterate () {
			if (Index >= Count)
				return false;
			Codepoint = (uint) char.ConvertToUtf32 (str, strIndex);
			if (char.IsHighSurrogate (str, strIndex))
				strIndex++;
			strIndex++;
			Index++;
			return true;
		}

		public CodepointIterator (string str) {
			this.str = str;
			Count = 0;
			for (int i = 0; i < str.Length; i++) {
				Count++;
				if (char.IsHighSurrogate (str, i))
					i++;
			}
			Index = 0;
			Codepoint = 0;
			strIndex = 0;
		}
	}
}

