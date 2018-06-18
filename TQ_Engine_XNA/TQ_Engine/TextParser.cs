using System;
using System.Linq;
using System.Collections.Generic;

namespace Tools
{
	public class TextParser
	{
		public static string[] ToLines (string text) {
			return CutAtSymbol (text, '\n');
		}
		public static string[] ToWords (string text) {
			return CutAtSymbol (text, ' ');
		}
		public static string PasteAs (char a, char b, string t) {
			char[] chs = t.Select ((char c) => c = c == a ? b : c).ToArray();
			t = "";
			foreach (var c in chs) {
				t = t + c;
			}
			return t;
		}
		public static float ParceFloat (string t) {
			t = PasteAs ('.', ',', t);
			return float.Parse (t);
		}
		public static string[] CutAtSymbol (string text, char symbol) {
			List<string> parts = new List<string> ();
			int sym = 0;
			string cur = "";
			while (true) {
				if (!(sym < text.Length)) {
					parts.Add (cur);
					break;
				}
				char ch = text [sym];
				if (ch == symbol) {
					if (cur.Length > 0) {
						parts.Add (cur);
						cur = "";
					}
				} else {
					cur = cur + ch;
				}
				sym++;
			}
			return parts.ToArray ();
		}
	}
}

