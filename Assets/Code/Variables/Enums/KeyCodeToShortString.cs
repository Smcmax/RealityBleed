﻿using UnityEngine;
using System.Collections.Generic;

public static class KeyCodeToShortString {
	public static readonly Dictionary<KeyCode, string> Mapping = new Dictionary<KeyCode, string>() {
		{KeyCode.A, "A"},
		{KeyCode.B, "B"},
		{KeyCode.C, "C"},
		{KeyCode.D, "D"},
		{KeyCode.E, "E"},
		{KeyCode.F, "F"},
		{KeyCode.G, "G"},
		{KeyCode.H, "H"},
		{KeyCode.I, "I"},
		{KeyCode.J, "J"},
		{KeyCode.K, "K"},
		{KeyCode.L, "L"},
		{KeyCode.M, "M"},
		{KeyCode.N, "N"},
		{KeyCode.O, "O"},
		{KeyCode.P, "P"},
		{KeyCode.Q, "Q"},
		{KeyCode.R, "R"},
		{KeyCode.S, "S"},
		{KeyCode.T, "T"},
		{KeyCode.U, "U"},
		{KeyCode.V, "V"},
		{KeyCode.W, "W"},
		{KeyCode.X, "X"},
		{KeyCode.Y, "Y"},
		{KeyCode.Z, "Z"},

		{KeyCode.Alpha0, "0"},
		{KeyCode.Alpha1, "1"},
		{KeyCode.Alpha2, "2"},
		{KeyCode.Alpha3, "3"},
		{KeyCode.Alpha4, "4"},
		{KeyCode.Alpha5, "5"},
		{KeyCode.Alpha6, "6"},
		{KeyCode.Alpha7, "7"},
		{KeyCode.Alpha8, "8"},
		{KeyCode.Alpha9, "9"},

		{KeyCode.Keypad0, "0"},
		{KeyCode.Keypad1, "1"},
		{KeyCode.Keypad2, "2"},
		{KeyCode.Keypad3, "3"},
		{KeyCode.Keypad4, "4"},
		{KeyCode.Keypad5, "5"},
		{KeyCode.Keypad6, "6"},
		{KeyCode.Keypad7, "7"},
		{KeyCode.Keypad8, "8"},
		{KeyCode.Keypad9, "9"},

		{KeyCode.Exclaim, "!"},
		{KeyCode.DoubleQuote, "\""},
		{KeyCode.Hash, "#"},
		{KeyCode.Dollar, "$"},
		{KeyCode.Ampersand, "&"},
		{KeyCode.Quote, "'"},
		{KeyCode.LeftParen, "("},
		{KeyCode.RightParen, ")"},
		{KeyCode.Asterisk, "*"},
		{KeyCode.Plus, "+"},
		{KeyCode.Comma, ","},
		{KeyCode.Minus, "-"},
		{KeyCode.Period, "."},
		{KeyCode.Slash, "/"},
		{KeyCode.Colon, ":"},
		{KeyCode.Semicolon, ";"},
		{KeyCode.Less, "<"},
		{KeyCode.Equals, "="},
		{KeyCode.Greater, ">"},
		{KeyCode.Question, "?"},
		{KeyCode.At, "@"},
		{KeyCode.LeftBracket, "["},
		{KeyCode.Backslash, "\\"},
		{KeyCode.RightBracket, "]"},
		{KeyCode.Caret, "^"},
		{KeyCode.Underscore, "_"},
		{KeyCode.BackQuote, "`"},
		{KeyCode.AltGr, "Alt"},
		{KeyCode.Backspace, "Back"},
		{KeyCode.Break, "Break"},
		{KeyCode.CapsLock, "Caps"},
		{KeyCode.Clear, "Clear"},
		{KeyCode.Delete, "Del"},
		{KeyCode.DownArrow, "Down"},
		{KeyCode.End, "End"},
		{KeyCode.Escape, "Esc"},
		{KeyCode.F1, "F1"},
		{KeyCode.F2, "F2"},
		{KeyCode.F3, "F3"},
		{KeyCode.F4, "F4"},
		{KeyCode.F5, "F5"},
		{KeyCode.F6, "F6"},
		{KeyCode.F7, "F7"},
		{KeyCode.F8, "F8"},
		{KeyCode.F9, "F9"},
		{KeyCode.F10, "F10"},
		{KeyCode.F11, "F11"},
		{KeyCode.F12, "F12"},
		{KeyCode.F13, "F13"},
		{KeyCode.F14, "F14"},
		{KeyCode.F15, "F15"},
		{KeyCode.Help, "Help"},
		{KeyCode.Home, "Home"},
		{KeyCode.Insert, "Ins"},
		{KeyCode.LeftAlt, "Alt"},
		{KeyCode.LeftApple, "Cmd"},
		{KeyCode.LeftArrow, "Left"},
		{KeyCode.LeftControl, "Ctrl"},
		{KeyCode.LeftShift, "Shift"},
		{KeyCode.LeftWindows, "Win"},
		{KeyCode.Menu, "Menu"},
		{KeyCode.Mouse0, "Ms 0"},
		{KeyCode.Mouse1, "Ms 1"},
		{KeyCode.Mouse2, "Ms 2"},
		{KeyCode.Mouse3, "Ms 3"},
		{KeyCode.Mouse4, "Ms 4"},
		{KeyCode.Mouse5, "Ms 5"},
		{KeyCode.Mouse6, "Ms 6"},
		{KeyCode.None, "None"},
		{KeyCode.Numlock, "Numlk"},
		{KeyCode.PageDown, "PgDn"},
		{KeyCode.PageUp, "PgUp"},
		{KeyCode.Print, "Prtsc"},
		{KeyCode.Return, "Enter"},
		{KeyCode.RightAlt, "Alt"},
		{KeyCode.RightApple, "Cmd"},
		{KeyCode.RightArrow, "Right"},
		{KeyCode.RightControl, "Ctrl"},
		{KeyCode.RightShift, "Shift"},
		{KeyCode.RightWindows, "Win"},
		{KeyCode.ScrollLock, "Scrlk"},
		{KeyCode.Space, "Space"},
		{KeyCode.SysReq, "Sysrq"},
		{KeyCode.Tab, "Tab"},
		{KeyCode.UpArrow, "Up"},

		{KeyCode.KeypadDivide, "/"},
		{KeyCode.KeypadEquals, "="},
		{KeyCode.KeypadMinus, "-"},
		{KeyCode.KeypadMultiply, "*"},
		{KeyCode.KeypadPeriod, "."},
		{KeyCode.KeypadPlus, "+"},
	};
}