/*
 * Copyright (c) 2007 Henri Sivonen
 * Copyright (c) 2007-2011 Mozilla Foundation
 * Portions of comments Copyright 2004-2008 Apple Computer, Inc., Mozilla 
 * Foundation, and Opera Software ASA.
 * Copyright (c) 2012 Patrick Reisert
 *
 * Permission is hereby granted, free of charge, to any person obtaining a 
 * copy of this software and associated documentation files (the "Software"), 
 * to deal in the Software without restriction, including without limitation 
 * the rights to use, copy, modify, merge, publish, distribute, sublicense, 
 * and/or sell copies of the Software, and to permit persons to whom the 
 * Software is furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in 
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
 * THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
 * DEALINGS IN THE SOFTWARE.
 */

/*
 * The comments following this one that use the same comment syntax as this 
 * comment are quotes from the WHATWG HTML 5 spec as of 27 June 2007 
 * amended as of June 28 2007.
 * That document came with this statement:
 * © Copyright 2004-2007 Apple Computer, Inc., Mozilla Foundation, and 
 * Opera Software ASA. You are granted a license to use, reproduce and 
 * create derivative works of this document."
 */

using HtmlParserSharp.Common;

namespace HtmlParserSharp.Core
{
	/// <summary>
	/// Moved the constants (and pseude-enums) out of the TreeBuilder class.
	/// TODO: Use real enums instead
	/// </summary>
	public class TreeBuilderConstants
	{
		/// <summary>
		/// Array version of U+FFFD.
		/// </summary>
		internal static readonly char[] REPLACEMENT_CHARACTER = { '\uFFFD' };

		// [NOCPP[

		internal readonly static string[] HTML4_PUBLIC_IDS = {
			"-//W3C//DTD HTML 4.0 Frameset//EN",
			"-//W3C//DTD HTML 4.0 Transitional//EN",
			"-//W3C//DTD HTML 4.0//EN", "-//W3C//DTD HTML 4.01 Frameset//EN",
			"-//W3C//DTD HTML 4.01 Transitional//EN",
			"-//W3C//DTD HTML 4.01//EN" };

		// ]NOCPP]

		internal readonly static string[] QUIRKY_PUBLIC_IDS = {
			"+//silmaril//dtd html pro v0r11 19970101//",
			"-//advasoft ltd//dtd html 3.0 aswedit + extensions//",
			"-//as//dtd html 3.0 aswedit + extensions//",
			"-//ietf//dtd html 2.0 level 1//",
			"-//ietf//dtd html 2.0 level 2//",
			"-//ietf//dtd html 2.0 strict level 1//",
			"-//ietf//dtd html 2.0 strict level 2//",
			"-//ietf//dtd html 2.0 strict//",
			"-//ietf//dtd html 2.0//",
			"-//ietf//dtd html 2.1e//",
			"-//ietf//dtd html 3.0//",
			"-//ietf//dtd html 3.2 final//",
			"-//ietf//dtd html 3.2//",
			"-//ietf//dtd html 3//",
			"-//ietf//dtd html level 0//",
			"-//ietf//dtd html level 1//",
			"-//ietf//dtd html level 2//",
			"-//ietf//dtd html level 3//",
			"-//ietf//dtd html strict level 0//",
			"-//ietf//dtd html strict level 1//",
			"-//ietf//dtd html strict level 2//",
			"-//ietf//dtd html strict level 3//",
			"-//ietf//dtd html strict//",
			"-//ietf//dtd html//",
			"-//metrius//dtd metrius presentational//",
			"-//microsoft//dtd internet explorer 2.0 html strict//",
			"-//microsoft//dtd internet explorer 2.0 html//",
			"-//microsoft//dtd internet explorer 2.0 tables//",
			"-//microsoft//dtd internet explorer 3.0 html strict//",
			"-//microsoft//dtd internet explorer 3.0 html//",
			"-//microsoft//dtd internet explorer 3.0 tables//",
			"-//netscape comm. corp.//dtd html//",
			"-//netscape comm. corp.//dtd strict html//",
			"-//o'reilly and associates//dtd html 2.0//",
			"-//o'reilly and associates//dtd html extended 1.0//",
			"-//o'reilly and associates//dtd html extended relaxed 1.0//",
			"-//softquad software//dtd hotmetal pro 6.0::19990601::extensions to html 4.0//",
			"-//softquad//dtd hotmetal pro 4.0::19971010::extensions to html 4.0//",
			"-//spyglass//dtd html 2.0 extended//",
			"-//sq//dtd html 2.0 hotmetal + extensions//",
			"-//sun microsystems corp.//dtd hotjava html//",
			"-//sun microsystems corp.//dtd hotjava strict html//",
			"-//w3c//dtd html 3 1995-03-24//", "-//w3c//dtd html 3.2 draft//",
			"-//w3c//dtd html 3.2 final//", "-//w3c//dtd html 3.2//",
			"-//w3c//dtd html 3.2s draft//", "-//w3c//dtd html 4.0 frameset//",
			"-//w3c//dtd html 4.0 transitional//",
			"-//w3c//dtd html experimental 19960712//",
			"-//w3c//dtd html experimental 970421//", "-//w3c//dtd w3 html//",
			"-//w3o//dtd w3 html 3.0//", "-//webtechs//dtd mozilla html 2.0//",
			"-//webtechs//dtd mozilla html//" };

		internal const int NOT_FOUND_ON_STACK = int.MaxValue;

		// [NOCPP[

		[Local]
		internal const string HTML_LOCAL = "html";

		// ]NOCPP]

		// Start dispatch groups

		public const int OTHER = 0;

		public const int A = 1;

		public const int BASE = 2;

		public const int BODY = 3;

		public const int BR = 4;

		public const int BUTTON = 5;

		public const int CAPTION = 6;

		public const int COL = 7;

		public const int COLGROUP = 8;

		public const int FORM = 9;

		public const int FRAME = 10;

		public const int FRAMESET = 11;

		public const int IMAGE = 12;

		public const int INPUT = 13;

		public const int ISINDEX = 14;

		public const int LI = 15;

		public const int LINK_OR_BASEFONT_OR_BGSOUND = 16;

		public const int MATH = 17;

		public const int META = 18;

		public const int SVG = 19;

		public const int HEAD = 20;

		public const int HR = 22;

		public const int HTML = 23;

		public const int NOBR = 24;

		public const int NOFRAMES = 25;

		public const int NOSCRIPT = 26;

		public const int OPTGROUP = 27;

		public const int OPTION = 28;

		public const int P = 29;

		public const int PLAINTEXT = 30;

		public const int SCRIPT = 31;

		public const int SELECT = 32;

		public const int STYLE = 33;

		public const int TABLE = 34;

		public const int TEXTAREA = 35;

		public const int TITLE = 36;

		public const int TR = 37;

		public const int XMP = 38;

		public const int TBODY_OR_THEAD_OR_TFOOT = 39;

		public const int TD_OR_TH = 40;

		public const int DD_OR_DT = 41;

		public const int H1_OR_H2_OR_H3_OR_H4_OR_H5_OR_H6 = 42;

		public const int MARQUEE_OR_APPLET = 43;

		public const int PRE_OR_LISTING = 44;

		public const int B_OR_BIG_OR_CODE_OR_EM_OR_I_OR_S_OR_SMALL_OR_STRIKE_OR_STRONG_OR_TT_OR_U = 45;

		public const int UL_OR_OL_OR_DL = 46;

		public const int IFRAME = 47;

		public const int EMBED_OR_IMG = 48;

		public const int AREA_OR_WBR = 49;

		public const int DIV_OR_BLOCKQUOTE_OR_CENTER_OR_MENU = 50;

		public const int ADDRESS_OR_ARTICLE_OR_ASIDE_OR_DETAILS_OR_DIR_OR_FIGCAPTION_OR_FIGURE_OR_FOOTER_OR_HEADER_OR_HGROUP_OR_NAV_OR_SECTION_OR_SUMMARY = 51;

		public const int RUBY_OR_SPAN_OR_SUB_OR_SUP_OR_VAR = 52;

		public const int RT_OR_RP = 53;

		public const int COMMAND = 54;

		public const int PARAM_OR_SOURCE_OR_TRACK = 55;

		public const int MGLYPH_OR_MALIGNMARK = 56;

		public const int MI_MO_MN_MS_MTEXT = 57;

		public const int ANNOTATION_XML = 58;

		public const int FOREIGNOBJECT_OR_DESC = 59;

		public const int NOEMBED = 60;

		public const int FIELDSET = 61;

		public const int OUTPUT_OR_LABEL = 62;

		public const int OBJECT = 63;

		public const int FONT = 64;

		public const int KEYGEN = 65;

		// start insertion modes

		internal const int INITIAL = 0;

		internal const int BEFORE_HTML = 1;

		internal const int BEFORE_HEAD = 2;

		internal const int IN_HEAD = 3;

		internal const int IN_HEAD_NOSCRIPT = 4;

		internal const int AFTER_HEAD = 5;

		internal const int IN_BODY = 6;

		internal const int IN_TABLE = 7;

		internal const int IN_CAPTION = 8;

		internal const int IN_COLUMN_GROUP = 9;

		internal const int IN_TABLE_BODY = 10;

		internal const int IN_ROW = 11;

		internal const int IN_CELL = 12;

		internal const int IN_SELECT = 13;

		internal const int IN_SELECT_IN_TABLE = 14;

		internal const int AFTER_BODY = 15;

		internal const int IN_FRAMESET = 16;

		internal const int AFTER_FRAMESET = 17;

		internal const int AFTER_AFTER_BODY = 18;

		internal const int AFTER_AFTER_FRAMESET = 19;

		internal const int TEXT = 20;

		internal const int FRAMESET_OK = 21;

		// start charset states

		internal const int CHARSET_INITIAL = 0;

		internal const int CHARSET_C = 1;

		internal const int CHARSET_H = 2;

		internal const int CHARSET_A = 3;

		internal const int CHARSET_R = 4;

		internal const int CHARSET_S = 5;

		internal const int CHARSET_E = 6;

		internal const int CHARSET_T = 7;

		internal const int CHARSET_EQUALS = 8;

		internal const int CHARSET_SINGLE_QUOTED = 9;

		internal const int CHARSET_DOUBLE_QUOTED = 10;

		internal const int CHARSET_UNQUOTED = 11;

		// end pseudo enums
	}
}
