/*
 * Copyright (c) 2008-2011 Mozilla Foundation
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

using System;
using HtmlParserSharp.Common;

namespace HtmlParserSharp.Core
{

	public sealed class ElementName
	// uncomment when regenerating self
	//        implements Comparable<ElementName> 
	{

		/// <summary>
		/// The mask for extracting the dispatch group.
		/// </summary>
		public const int GROUP_MASK = 127;

		/// <summary>
		/// Indicates that the element is not a pre-interned element. Forbidden
		/// on preinterned elements.
		/// </summary>
		public const int CUSTOM = (1 << 30);

		/// <summary>
		/// Indicates that the element is in the "special" category. This bit
		/// should not be pre-set on MathML or SVG specials--only on HTML specials.
		/// </summary>
		public const int SPECIAL = (1 << 29);

		/// <summary>
		/// The element is foster-parenting. This bit should be pre-set on elements
		/// that are foster-parenting as HTML.
		/// </summary>
		public const int FOSTER_PARENTING = (1 << 28);

		/// <summary>
		/// The element is scoping. This bit should be pre-set on elements
		/// that are scoping as HTML.
		/// </summary>
		public const int SCOPING = (1 << 27);

		/// <summary>
		/// The element is scoping as SVG.
		/// </summary>
		public const int SCOPING_AS_SVG = (1 << 26);

		/// <summary>
		/// The element is scoping as MathML.
		/// </summary>
		public const int SCOPING_AS_MATHML = (1 << 25);

		/// <summary>
		/// The element is an HTML integration point.
		/// </summary>
		public const int HTML_INTEGRATION_POINT = (1 << 24);

		/// <summary>
		/// The element has an optional end tag.
		/// </summary>
		public const int OPTIONAL_END_TAG = (1 << 23);

		public static readonly ElementName NULL_ELEMENT_NAME = new ElementName(null);

		[Local]
		public readonly string name;

		[Local]
		public readonly string camelCaseName;

		/// <summary>
		/// The lowest 7 bits are the dispatch group. The high bits are flags.
		/// </summary>
		public readonly int flags;

		public int Flags
		{
			get
			{
				return flags;
			}
		}

		public int Group
		{
			get
			{
				return flags & GROUP_MASK;
			}
		}

		// [NOCPP[

		public bool IsCustom
		{
			get
			{
				return (flags & CUSTOM) != 0;
			}
		}

		// ]NOCPP]

		internal static ElementName ElementNameByBuffer(char[] buf, int offset, int length)
		{
			int hash = ElementName.BufToHash(buf, length);
			int index = Array.BinarySearch<int>(ElementName.ELEMENT_HASHES, hash);
			if (index < 0)
			{
				return new ElementName(Portability.NewLocalNameFromBuffer(buf, offset, length));
			}
			else
			{
				ElementName elementName = ElementName.ELEMENT_NAMES[index];
				/*[Local]*/
				string name = elementName.name;
				if (!Portability.LocalEqualsBuffer(name, buf, offset, length))
				{
					return new ElementName(Portability.NewLocalNameFromBuffer(buf,
							offset, length));
				}
				return elementName;
			}
		}

		/// <summary>
		/// This method has to return a unique integer for each well-known
		/// lower-cased element name.
		/// </summary>
		private static int BufToHash(char[] buf, int len)
		{
			int hash = len;
			hash <<= 5;
			hash += buf[0] - 0x60;
			int j = len;
			for (int i = 0; i < 4 && j > 0; i++)
			{
				j--;
				hash <<= 5;
				hash += buf[j] - 0x60;
			}
			return hash;
		}

		private ElementName([Local] string name, [Local] string camelCaseName, int flags)
		{
			this.name = name;
			this.camelCaseName = camelCaseName;
			this.flags = flags;
		}

		internal ElementName([Local] string name)
		{
			this.name = name;
			this.camelCaseName = name;
			this.flags = TreeBuilderConstants.OTHER | CUSTOM;
		}

		/*virtual*/	public ElementName CloneElementName()
		{
			return this;
		}

		// START CODE ONLY USED FOR GENERATING CODE uncomment and run to regenerate

		///// <summary>
		///// Returns a <see cref="System.String"/> that represents this instance.
		///// </summary>
		///// <returns>
		///// A <see cref="System.String"/> that represents this instance.
		///// </returns>
		//override public string ToString() {
		//    return "(\"" + name + "\", \"" + camelCaseName + "\", " + decomposedFlags() + ")";
		//}

		//private string DecomposedFlags() {
		//    StringBuilder buf = new StringBuilder("TreeBuilderConstants.");
		//    buf.Append(treeBuilderGroupToName());
		//    if ((flags & SPECIAL) != 0) {
		//        buf.Append(" | SPECIAL");
		//    }
		//    if ((flags & FOSTER_PARENTING) != 0) {
		//        buf.Append(" | FOSTER_PARENTING");
		//    }
		//    if ((flags & SCOPING) != 0) {
		//        buf.Append(" | SCOPING");
		//    }        
		//    if ((flags & SCOPING_AS_MATHML) != 0) {
		//        buf.Append(" | SCOPING_AS_MATHML");
		//    }
		//    if ((flags & SCOPING_AS_SVG) != 0) {
		//        buf.Append(" | SCOPING_AS_SVG");
		//    }
		//    if ((flags & OPTIONAL_END_TAG) != 0) {
		//        buf.Append(" | OPTIONAL_END_TAG");
		//    }
		//    return buf.ToString();
		//}

		//private string constName() {
		//    char[] buf = new char[name.Length];
		//    for (int i = 0; i < name.Length; i++) {
		//        char c = name[i];
		//        if (c == '-') {
		//            buf[i] = '_';
		//        } else if (c >= '0' && c <= '9') {
		//            buf[i] = c;
		//        } else {
		//            buf[i] = (char) (c - 0x20);
		//        }
		//    }
		//    return new String(buf);
		//}

		//private int hash() {
		//    return BufToHash(name.ToCharArray(), name.Length);
		//}

		//public int CompareTo(ElementName other) {
		//    int thisHash = this.hash();
		//    int otherHash = other.hash();
		//    if (thisHash < otherHash) {
		//        return -1;
		//    } else if (thisHash == otherHash) {
		//        return 0;
		//    } else {
		//        return 1;
		//    }
		//}

		//private string TreeBuilderGroupToName() {
		//    switch (GetGroup()) {
		//        case TreeBuilderConstants.OTHER:
		//            return "OTHER";
		//        case TreeBuilderConstants.A:
		//            return "A";
		//        case TreeBuilderConstants.BASE:
		//            return "BASE";
		//        case TreeBuilderConstants.BODY:
		//            return "BODY";
		//        case TreeBuilderConstants.BR:
		//            return "BR";
		//        case TreeBuilderConstants.BUTTON:
		//            return "BUTTON";
		//        case TreeBuilderConstants.CAPTION:
		//            return "CAPTION";
		//        case TreeBuilderConstants.COL:
		//            return "COL";
		//        case TreeBuilderConstants.COLGROUP:
		//            return "COLGROUP";
		//        case TreeBuilderConstants.FONT:
		//            return "FONT";
		//        case TreeBuilderConstants.FORM:
		//            return "FORM";
		//        case TreeBuilderConstants.FRAME:
		//            return "FRAME";
		//        case TreeBuilderConstants.FRAMESET:
		//            return "FRAMESET";
		//        case TreeBuilderConstants.IMAGE:
		//            return "IMAGE";
		//        case TreeBuilderConstants.INPUT:
		//            return "INPUT";
		//        case TreeBuilderConstants.ISINDEX:
		//            return "ISINDEX";
		//        case TreeBuilderConstants.LI:
		//            return "LI";
		//        case TreeBuilderConstants.LINK_OR_BASEFONT_OR_BGSOUND:
		//            return "LINK_OR_BASEFONT_OR_BGSOUND";
		//        case TreeBuilderConstants.MATH:
		//            return "MATH";
		//        case TreeBuilderConstants.META:
		//            return "META";
		//        case TreeBuilderConstants.SVG:
		//            return "SVG";
		//        case TreeBuilderConstants.HEAD:
		//            return "HEAD";
		//        case TreeBuilderConstants.HR:
		//            return "HR";
		//        case TreeBuilderConstants.HTML:
		//            return "HTML";
		//        case TreeBuilderConstants.KEYGEN:
		//            return "KEYGEN";
		//        case TreeBuilderConstants.NOBR:
		//            return "NOBR";
		//        case TreeBuilderConstants.NOFRAMES:
		//            return "NOFRAMES";
		//        case TreeBuilderConstants.NOSCRIPT:
		//            return "NOSCRIPT";
		//        case TreeBuilderConstants.OPTGROUP:
		//            return "OPTGROUP";
		//        case TreeBuilderConstants.OPTION:
		//            return "OPTION";
		//        case TreeBuilderConstants.P:
		//            return "P";
		//        case TreeBuilderConstants.PLAINTEXT:
		//            return "PLAINTEXT";
		//        case TreeBuilderConstants.SCRIPT:
		//            return "SCRIPT";
		//        case TreeBuilderConstants.SELECT:
		//            return "SELECT";
		//        case TreeBuilderConstants.STYLE:
		//            return "STYLE";
		//        case TreeBuilderConstants.TABLE:
		//            return "TABLE";
		//        case TreeBuilderConstants.TEXTAREA:
		//            return "TEXTAREA";
		//        case TreeBuilderConstants.TITLE:
		//            return "TITLE";
		//        case TreeBuilderConstants.TR:
		//            return "TR";
		//        case TreeBuilderConstants.XMP:
		//            return "XMP";
		//        case TreeBuilderConstants.TBODY_OR_THEAD_OR_TFOOT:
		//            return "TBODY_OR_THEAD_OR_TFOOT";
		//        case TreeBuilderConstants.TD_OR_TH:
		//            return "TD_OR_TH";
		//        case TreeBuilderConstants.DD_OR_DT:
		//            return "DD_OR_DT";
		//        case TreeBuilderConstants.H1_OR_H2_OR_H3_OR_H4_OR_H5_OR_H6:
		//            return "H1_OR_H2_OR_H3_OR_H4_OR_H5_OR_H6";
		//        case TreeBuilderConstants.OBJECT:
		//            return "OBJECT";
		//        case TreeBuilderConstants.OUTPUT_OR_LABEL:
		//            return "OUTPUT_OR_LABEL";
		//        case TreeBuilderConstants.MARQUEE_OR_APPLET:
		//            return "MARQUEE_OR_APPLET";
		//        case TreeBuilderConstants.PRE_OR_LISTING:
		//            return "PRE_OR_LISTING";
		//        case TreeBuilderConstants.B_OR_BIG_OR_CODE_OR_EM_OR_I_OR_S_OR_SMALL_OR_STRIKE_OR_STRONG_OR_TT_OR_U:
		//            return "B_OR_BIG_OR_CODE_OR_EM_OR_I_OR_S_OR_SMALL_OR_STRIKE_OR_STRONG_OR_TT_OR_U";
		//        case TreeBuilderConstants.UL_OR_OL_OR_DL:
		//            return "UL_OR_OL_OR_DL";
		//        case TreeBuilderConstants.IFRAME:
		//            return "IFRAME";
		//        case TreeBuilderConstants.NOEMBED:
		//            return "NOEMBED";
		//        case TreeBuilderConstants.EMBED_OR_IMG:
		//            return "EMBED_OR_IMG";
		//        case TreeBuilderConstants.AREA_OR_WBR:
		//            return "AREA_OR_WBR";
		//        case TreeBuilderConstants.DIV_OR_BLOCKQUOTE_OR_CENTER_OR_MENU:
		//            return "DIV_OR_BLOCKQUOTE_OR_CENTER_OR_MENU";
		//        case TreeBuilderConstants.FIELDSET:
		//            return "FIELDSET";
		//        case TreeBuilderConstants.ADDRESS_OR_ARTICLE_OR_ASIDE_OR_DETAILS_OR_DIR_OR_FIGCAPTION_OR_FIGURE_OR_FOOTER_OR_HEADER_OR_HGROUP_OR_NAV_OR_SECTION_OR_SUMMARY:
		//            return "ADDRESS_OR_ARTICLE_OR_ASIDE_OR_DETAILS_OR_DIR_OR_FIGCAPTION_OR_FIGURE_OR_FOOTER_OR_HEADER_OR_HGROUP_OR_NAV_OR_SECTION_OR_SUMMARY";
		//        case TreeBuilderConstants.RUBY_OR_SPAN_OR_SUB_OR_SUP_OR_VAR:
		//            return "RUBY_OR_SPAN_OR_SUB_OR_SUP_OR_VAR";
		//        case TreeBuilderConstants.RT_OR_RP:
		//            return "RT_OR_RP";
		//        case TreeBuilderConstants.COMMAND:
		//            return "COMMAND";
		//        case TreeBuilderConstants.PARAM_OR_SOURCE_OR_TRACK:
		//            return "PARAM_OR_SOURCE_OR_TRACK";
		//        case TreeBuilderConstants.MGLYPH_OR_MALIGNMARK:
		//            return "MGLYPH_OR_MALIGNMARK";
		//        case TreeBuilderConstants.MI_MO_MN_MS_MTEXT:
		//            return "MI_MO_MN_MS_MTEXT";
		//        case TreeBuilderConstants.ANNOTATION_XML:
		//            return "ANNOTATION_XML";
		//        case TreeBuilderConstants.FOREIGNOBJECT_OR_DESC:
		//            return "FOREIGNOBJECT_OR_DESC";
		//    }
		//    return null;
		//}

		///**
		// * Regenerate self
		// * 
		// * @param args
		// */
		//public static void main(String[] args) {
		//    Arrays.sort(ELEMENT_NAMES);
		//    for (int i = 1; i < ELEMENT_NAMES.length; i++) {
		//        if (ELEMENT_NAMES[i].hash() == ELEMENT_NAMES[i - 1].hash()) {
		//            System.err.println("Hash collision: " + ELEMENT_NAMES[i].name
		//                    + ", " + ELEMENT_NAMES[i - 1].name);
		//            return;
		//        }
		//    }
		//    for (int i = 0; i < ELEMENT_NAMES.length; i++) {
		//        ElementName el = ELEMENT_NAMES[i];
		//        System.out.println("public static readonly ElementName "
		//                + el.constName() + " = new ElementName" + el.toString()
		//                + ";");
		//    }
		//    System.out.println("private final static @NoLength ElementName[] ELEMENT_NAMES = {");
		//    for (int i = 0; i < ELEMENT_NAMES.length; i++) {
		//        ElementName el = ELEMENT_NAMES[i];
		//        System.out.println(el.constName() + ",");
		//    }
		//    System.out.println("};");
		//    System.out.println("private final static int[] ELEMENT_HASHES = {");
		//    for (int i = 0; i < ELEMENT_NAMES.length; i++) {
		//        ElementName el = ELEMENT_NAMES[i];
		//        System.out.println(Integer.toString(el.hash()) + ",");
		//    }
		//    System.out.println("};");
		//}

		// START GENERATED CODE
		public static readonly ElementName A = new ElementName("a", "a", TreeBuilderConstants.A);
		public static readonly ElementName B = new ElementName("b", "b", TreeBuilderConstants.B_OR_BIG_OR_CODE_OR_EM_OR_I_OR_S_OR_SMALL_OR_STRIKE_OR_STRONG_OR_TT_OR_U);
		public static readonly ElementName G = new ElementName("g", "g", TreeBuilderConstants.OTHER);
		public static readonly ElementName I = new ElementName("i", "i", TreeBuilderConstants.B_OR_BIG_OR_CODE_OR_EM_OR_I_OR_S_OR_SMALL_OR_STRIKE_OR_STRONG_OR_TT_OR_U);
		public static readonly ElementName P = new ElementName("p", "p", TreeBuilderConstants.P | SPECIAL | OPTIONAL_END_TAG);
		public static readonly ElementName Q = new ElementName("q", "q", TreeBuilderConstants.OTHER);
		public static readonly ElementName S = new ElementName("s", "s", TreeBuilderConstants.B_OR_BIG_OR_CODE_OR_EM_OR_I_OR_S_OR_SMALL_OR_STRIKE_OR_STRONG_OR_TT_OR_U);
		public static readonly ElementName U = new ElementName("u", "u", TreeBuilderConstants.B_OR_BIG_OR_CODE_OR_EM_OR_I_OR_S_OR_SMALL_OR_STRIKE_OR_STRONG_OR_TT_OR_U);
		public static readonly ElementName BR = new ElementName("br", "br", TreeBuilderConstants.BR | SPECIAL);
		public static readonly ElementName CI = new ElementName("ci", "ci", TreeBuilderConstants.OTHER);
		public static readonly ElementName CN = new ElementName("cn", "cn", TreeBuilderConstants.OTHER);
		public static readonly ElementName DD = new ElementName("dd", "dd", TreeBuilderConstants.DD_OR_DT | SPECIAL | OPTIONAL_END_TAG);
		public static readonly ElementName DL = new ElementName("dl", "dl", TreeBuilderConstants.UL_OR_OL_OR_DL | SPECIAL);
		public static readonly ElementName DT = new ElementName("dt", "dt", TreeBuilderConstants.DD_OR_DT | SPECIAL | OPTIONAL_END_TAG);
		public static readonly ElementName EM = new ElementName("em", "em", TreeBuilderConstants.B_OR_BIG_OR_CODE_OR_EM_OR_I_OR_S_OR_SMALL_OR_STRIKE_OR_STRONG_OR_TT_OR_U);
		public static readonly ElementName EQ = new ElementName("eq", "eq", TreeBuilderConstants.OTHER);
		public static readonly ElementName FN = new ElementName("fn", "fn", TreeBuilderConstants.OTHER);
		public static readonly ElementName H1 = new ElementName("h1", "h1", TreeBuilderConstants.H1_OR_H2_OR_H3_OR_H4_OR_H5_OR_H6 | SPECIAL);
		public static readonly ElementName H2 = new ElementName("h2", "h2", TreeBuilderConstants.H1_OR_H2_OR_H3_OR_H4_OR_H5_OR_H6 | SPECIAL);
		public static readonly ElementName H3 = new ElementName("h3", "h3", TreeBuilderConstants.H1_OR_H2_OR_H3_OR_H4_OR_H5_OR_H6 | SPECIAL);
		public static readonly ElementName H4 = new ElementName("h4", "h4", TreeBuilderConstants.H1_OR_H2_OR_H3_OR_H4_OR_H5_OR_H6 | SPECIAL);
		public static readonly ElementName H5 = new ElementName("h5", "h5", TreeBuilderConstants.H1_OR_H2_OR_H3_OR_H4_OR_H5_OR_H6 | SPECIAL);
		public static readonly ElementName H6 = new ElementName("h6", "h6", TreeBuilderConstants.H1_OR_H2_OR_H3_OR_H4_OR_H5_OR_H6 | SPECIAL);
		public static readonly ElementName GT = new ElementName("gt", "gt", TreeBuilderConstants.OTHER);
		public static readonly ElementName HR = new ElementName("hr", "hr", TreeBuilderConstants.HR | SPECIAL);
		public static readonly ElementName IN = new ElementName("in", "in", TreeBuilderConstants.OTHER);
		public static readonly ElementName LI = new ElementName("li", "li", TreeBuilderConstants.LI | SPECIAL | OPTIONAL_END_TAG);
		public static readonly ElementName LN = new ElementName("ln", "ln", TreeBuilderConstants.OTHER);
		public static readonly ElementName LT = new ElementName("lt", "lt", TreeBuilderConstants.OTHER);
		public static readonly ElementName MI = new ElementName("mi", "mi", TreeBuilderConstants.MI_MO_MN_MS_MTEXT | SCOPING_AS_MATHML);
		public static readonly ElementName MN = new ElementName("mn", "mn", TreeBuilderConstants.MI_MO_MN_MS_MTEXT | SCOPING_AS_MATHML);
		public static readonly ElementName MO = new ElementName("mo", "mo", TreeBuilderConstants.MI_MO_MN_MS_MTEXT | SCOPING_AS_MATHML);
		public static readonly ElementName MS = new ElementName("ms", "ms", TreeBuilderConstants.MI_MO_MN_MS_MTEXT | SCOPING_AS_MATHML);
		public static readonly ElementName OL = new ElementName("ol", "ol", TreeBuilderConstants.UL_OR_OL_OR_DL | SPECIAL);
		public static readonly ElementName OR = new ElementName("or", "or", TreeBuilderConstants.OTHER);
		public static readonly ElementName PI = new ElementName("pi", "pi", TreeBuilderConstants.OTHER);
		public static readonly ElementName RP = new ElementName("rp", "rp", TreeBuilderConstants.RT_OR_RP | OPTIONAL_END_TAG);
		public static readonly ElementName RT = new ElementName("rt", "rt", TreeBuilderConstants.RT_OR_RP | OPTIONAL_END_TAG);
		public static readonly ElementName TD = new ElementName("td", "td", TreeBuilderConstants.TD_OR_TH | SPECIAL | SCOPING | OPTIONAL_END_TAG);
		public static readonly ElementName TH = new ElementName("th", "th", TreeBuilderConstants.TD_OR_TH | SPECIAL | SCOPING | OPTIONAL_END_TAG);
		public static readonly ElementName TR = new ElementName("tr", "tr", TreeBuilderConstants.TR | SPECIAL | FOSTER_PARENTING | OPTIONAL_END_TAG);
		public static readonly ElementName TT = new ElementName("tt", "tt", TreeBuilderConstants.B_OR_BIG_OR_CODE_OR_EM_OR_I_OR_S_OR_SMALL_OR_STRIKE_OR_STRONG_OR_TT_OR_U);
		public static readonly ElementName UL = new ElementName("ul", "ul", TreeBuilderConstants.UL_OR_OL_OR_DL | SPECIAL);
		public static readonly ElementName AND = new ElementName("and", "and", TreeBuilderConstants.OTHER);
		public static readonly ElementName ARG = new ElementName("arg", "arg", TreeBuilderConstants.OTHER);
		public static readonly ElementName ABS = new ElementName("abs", "abs", TreeBuilderConstants.OTHER);
		public static readonly ElementName BIG = new ElementName("big", "big", TreeBuilderConstants.B_OR_BIG_OR_CODE_OR_EM_OR_I_OR_S_OR_SMALL_OR_STRIKE_OR_STRONG_OR_TT_OR_U);
		public static readonly ElementName BDO = new ElementName("bdo", "bdo", TreeBuilderConstants.OTHER);
		public static readonly ElementName CSC = new ElementName("csc", "csc", TreeBuilderConstants.OTHER);
		public static readonly ElementName COL = new ElementName("col", "col", TreeBuilderConstants.COL | SPECIAL);
		public static readonly ElementName COS = new ElementName("cos", "cos", TreeBuilderConstants.OTHER);
		public static readonly ElementName COT = new ElementName("cot", "cot", TreeBuilderConstants.OTHER);
		public static readonly ElementName DEL = new ElementName("del", "del", TreeBuilderConstants.OTHER);
		public static readonly ElementName DFN = new ElementName("dfn", "dfn", TreeBuilderConstants.OTHER);
		public static readonly ElementName DIR = new ElementName("dir", "dir", TreeBuilderConstants.ADDRESS_OR_ARTICLE_OR_ASIDE_OR_DETAILS_OR_DIR_OR_FIGCAPTION_OR_FIGURE_OR_FOOTER_OR_HEADER_OR_HGROUP_OR_NAV_OR_SECTION_OR_SUMMARY | SPECIAL);
		public static readonly ElementName DIV = new ElementName("div", "div", TreeBuilderConstants.DIV_OR_BLOCKQUOTE_OR_CENTER_OR_MENU | SPECIAL);
		public static readonly ElementName EXP = new ElementName("exp", "exp", TreeBuilderConstants.OTHER);
		public static readonly ElementName GCD = new ElementName("gcd", "gcd", TreeBuilderConstants.OTHER);
		public static readonly ElementName GEQ = new ElementName("geq", "geq", TreeBuilderConstants.OTHER);
		public static readonly ElementName IMG = new ElementName("img", "img", TreeBuilderConstants.EMBED_OR_IMG | SPECIAL);
		public static readonly ElementName INS = new ElementName("ins", "ins", TreeBuilderConstants.OTHER);
		public static readonly ElementName INT = new ElementName("int", "int", TreeBuilderConstants.OTHER);
		public static readonly ElementName KBD = new ElementName("kbd", "kbd", TreeBuilderConstants.OTHER);
		public static readonly ElementName LOG = new ElementName("log", "log", TreeBuilderConstants.OTHER);
		public static readonly ElementName LCM = new ElementName("lcm", "lcm", TreeBuilderConstants.OTHER);
		public static readonly ElementName LEQ = new ElementName("leq", "leq", TreeBuilderConstants.OTHER);
		public static readonly ElementName MTD = new ElementName("mtd", "mtd", TreeBuilderConstants.OTHER);
		public static readonly ElementName MIN = new ElementName("min", "min", TreeBuilderConstants.OTHER);
		public static readonly ElementName MAP = new ElementName("map", "map", TreeBuilderConstants.OTHER);
		public static readonly ElementName MTR = new ElementName("mtr", "mtr", TreeBuilderConstants.OTHER);
		public static readonly ElementName MAX = new ElementName("max", "max", TreeBuilderConstants.OTHER);
		public static readonly ElementName NEQ = new ElementName("neq", "neq", TreeBuilderConstants.OTHER);
		public static readonly ElementName NOT = new ElementName("not", "not", TreeBuilderConstants.OTHER);
		public static readonly ElementName NAV = new ElementName("nav", "nav", TreeBuilderConstants.ADDRESS_OR_ARTICLE_OR_ASIDE_OR_DETAILS_OR_DIR_OR_FIGCAPTION_OR_FIGURE_OR_FOOTER_OR_HEADER_OR_HGROUP_OR_NAV_OR_SECTION_OR_SUMMARY | SPECIAL);
		public static readonly ElementName PRE = new ElementName("pre", "pre", TreeBuilderConstants.PRE_OR_LISTING | SPECIAL);
		public static readonly ElementName REM = new ElementName("rem", "rem", TreeBuilderConstants.OTHER);
		public static readonly ElementName SUB = new ElementName("sub", "sub", TreeBuilderConstants.RUBY_OR_SPAN_OR_SUB_OR_SUP_OR_VAR);
		public static readonly ElementName SEC = new ElementName("sec", "sec", TreeBuilderConstants.OTHER);
		public static readonly ElementName SVG = new ElementName("svg", "svg", TreeBuilderConstants.SVG);
		public static readonly ElementName SUM = new ElementName("sum", "sum", TreeBuilderConstants.OTHER);
		public static readonly ElementName SIN = new ElementName("sin", "sin", TreeBuilderConstants.OTHER);
		public static readonly ElementName SEP = new ElementName("sep", "sep", TreeBuilderConstants.OTHER);
		public static readonly ElementName SUP = new ElementName("sup", "sup", TreeBuilderConstants.RUBY_OR_SPAN_OR_SUB_OR_SUP_OR_VAR);
		public static readonly ElementName SET = new ElementName("set", "set", TreeBuilderConstants.OTHER);
		public static readonly ElementName TAN = new ElementName("tan", "tan", TreeBuilderConstants.OTHER);
		public static readonly ElementName USE = new ElementName("use", "use", TreeBuilderConstants.OTHER);
		public static readonly ElementName VAR = new ElementName("var", "var", TreeBuilderConstants.RUBY_OR_SPAN_OR_SUB_OR_SUP_OR_VAR);
		public static readonly ElementName WBR = new ElementName("wbr", "wbr", TreeBuilderConstants.AREA_OR_WBR | SPECIAL);
		public static readonly ElementName XMP = new ElementName("xmp", "xmp", TreeBuilderConstants.XMP);
		public static readonly ElementName XOR = new ElementName("xor", "xor", TreeBuilderConstants.OTHER);
		public static readonly ElementName AREA = new ElementName("area", "area", TreeBuilderConstants.AREA_OR_WBR | SPECIAL);
		public static readonly ElementName ABBR = new ElementName("abbr", "abbr", TreeBuilderConstants.OTHER);
		public static readonly ElementName BASE = new ElementName("base", "base", TreeBuilderConstants.BASE | SPECIAL);
		public static readonly ElementName BVAR = new ElementName("bvar", "bvar", TreeBuilderConstants.OTHER);
		public static readonly ElementName BODY = new ElementName("body", "body", TreeBuilderConstants.BODY | SPECIAL | OPTIONAL_END_TAG);
		public static readonly ElementName CARD = new ElementName("card", "card", TreeBuilderConstants.OTHER);
		public static readonly ElementName CODE = new ElementName("code", "code", TreeBuilderConstants.B_OR_BIG_OR_CODE_OR_EM_OR_I_OR_S_OR_SMALL_OR_STRIKE_OR_STRONG_OR_TT_OR_U);
		public static readonly ElementName CITE = new ElementName("cite", "cite", TreeBuilderConstants.OTHER);
		public static readonly ElementName CSCH = new ElementName("csch", "csch", TreeBuilderConstants.OTHER);
		public static readonly ElementName COSH = new ElementName("cosh", "cosh", TreeBuilderConstants.OTHER);
		public static readonly ElementName COTH = new ElementName("coth", "coth", TreeBuilderConstants.OTHER);
		public static readonly ElementName CURL = new ElementName("curl", "curl", TreeBuilderConstants.OTHER);
		public static readonly ElementName DESC = new ElementName("desc", "desc", TreeBuilderConstants.FOREIGNOBJECT_OR_DESC | SCOPING_AS_SVG);
		public static readonly ElementName DIFF = new ElementName("diff", "diff", TreeBuilderConstants.OTHER);
		public static readonly ElementName DEFS = new ElementName("defs", "defs", TreeBuilderConstants.OTHER);
		public static readonly ElementName FORM = new ElementName("form", "form", TreeBuilderConstants.FORM | SPECIAL);
		public static readonly ElementName FONT = new ElementName("font", "font", TreeBuilderConstants.FONT);
		public static readonly ElementName GRAD = new ElementName("grad", "grad", TreeBuilderConstants.OTHER);
		public static readonly ElementName HEAD = new ElementName("head", "head", TreeBuilderConstants.HEAD | SPECIAL | OPTIONAL_END_TAG);
		public static readonly ElementName HTML = new ElementName("html", "html", TreeBuilderConstants.HTML | SPECIAL | SCOPING | OPTIONAL_END_TAG);
		public static readonly ElementName LINE = new ElementName("line", "line", TreeBuilderConstants.OTHER);
		public static readonly ElementName LINK = new ElementName("link", "link", TreeBuilderConstants.LINK_OR_BASEFONT_OR_BGSOUND | SPECIAL);
		public static readonly ElementName LIST = new ElementName("list", "list", TreeBuilderConstants.OTHER);
		public static readonly ElementName META = new ElementName("meta", "meta", TreeBuilderConstants.META | SPECIAL);
		public static readonly ElementName MSUB = new ElementName("msub", "msub", TreeBuilderConstants.OTHER);
		public static readonly ElementName MODE = new ElementName("mode", "mode", TreeBuilderConstants.OTHER);
		public static readonly ElementName MATH = new ElementName("math", "math", TreeBuilderConstants.MATH);
		public static readonly ElementName MARK = new ElementName("mark", "mark", TreeBuilderConstants.OTHER);
		public static readonly ElementName MASK = new ElementName("mask", "mask", TreeBuilderConstants.OTHER);
		public static readonly ElementName MEAN = new ElementName("mean", "mean", TreeBuilderConstants.OTHER);
		public static readonly ElementName MSUP = new ElementName("msup", "msup", TreeBuilderConstants.OTHER);
		public static readonly ElementName MENU = new ElementName("menu", "menu", TreeBuilderConstants.DIV_OR_BLOCKQUOTE_OR_CENTER_OR_MENU | SPECIAL);
		public static readonly ElementName MROW = new ElementName("mrow", "mrow", TreeBuilderConstants.OTHER);
		public static readonly ElementName NONE = new ElementName("none", "none", TreeBuilderConstants.OTHER);
		public static readonly ElementName NOBR = new ElementName("nobr", "nobr", TreeBuilderConstants.NOBR);
		public static readonly ElementName NEST = new ElementName("nest", "nest", TreeBuilderConstants.OTHER);
		public static readonly ElementName PATH = new ElementName("path", "path", TreeBuilderConstants.OTHER);
		public static readonly ElementName PLUS = new ElementName("plus", "plus", TreeBuilderConstants.OTHER);
		public static readonly ElementName RULE = new ElementName("rule", "rule", TreeBuilderConstants.OTHER);
		public static readonly ElementName REAL = new ElementName("real", "real", TreeBuilderConstants.OTHER);
		public static readonly ElementName RELN = new ElementName("reln", "reln", TreeBuilderConstants.OTHER);
		public static readonly ElementName RECT = new ElementName("rect", "rect", TreeBuilderConstants.OTHER);
		public static readonly ElementName ROOT = new ElementName("root", "root", TreeBuilderConstants.OTHER);
		public static readonly ElementName RUBY = new ElementName("ruby", "ruby", TreeBuilderConstants.RUBY_OR_SPAN_OR_SUB_OR_SUP_OR_VAR);
		public static readonly ElementName SECH = new ElementName("sech", "sech", TreeBuilderConstants.OTHER);
		public static readonly ElementName SINH = new ElementName("sinh", "sinh", TreeBuilderConstants.OTHER);
		public static readonly ElementName SPAN = new ElementName("span", "span", TreeBuilderConstants.RUBY_OR_SPAN_OR_SUB_OR_SUP_OR_VAR);
		public static readonly ElementName SAMP = new ElementName("samp", "samp", TreeBuilderConstants.OTHER);
		public static readonly ElementName STOP = new ElementName("stop", "stop", TreeBuilderConstants.OTHER);
		public static readonly ElementName SDEV = new ElementName("sdev", "sdev", TreeBuilderConstants.OTHER);
		public static readonly ElementName TIME = new ElementName("time", "time", TreeBuilderConstants.OTHER);
		public static readonly ElementName TRUE = new ElementName("true", "true", TreeBuilderConstants.OTHER);
		public static readonly ElementName TREF = new ElementName("tref", "tref", TreeBuilderConstants.OTHER);
		public static readonly ElementName TANH = new ElementName("tanh", "tanh", TreeBuilderConstants.OTHER);
		public static readonly ElementName TEXT = new ElementName("text", "text", TreeBuilderConstants.OTHER);
		public static readonly ElementName VIEW = new ElementName("view", "view", TreeBuilderConstants.OTHER);
		public static readonly ElementName ASIDE = new ElementName("aside", "aside", TreeBuilderConstants.ADDRESS_OR_ARTICLE_OR_ASIDE_OR_DETAILS_OR_DIR_OR_FIGCAPTION_OR_FIGURE_OR_FOOTER_OR_HEADER_OR_HGROUP_OR_NAV_OR_SECTION_OR_SUMMARY | SPECIAL);
		public static readonly ElementName AUDIO = new ElementName("audio", "audio", TreeBuilderConstants.OTHER);
		public static readonly ElementName APPLY = new ElementName("apply", "apply", TreeBuilderConstants.OTHER);
		public static readonly ElementName EMBED = new ElementName("embed", "embed", TreeBuilderConstants.EMBED_OR_IMG | SPECIAL);
		public static readonly ElementName FRAME = new ElementName("frame", "frame", TreeBuilderConstants.FRAME | SPECIAL);
		public static readonly ElementName FALSE = new ElementName("false", "false", TreeBuilderConstants.OTHER);
		public static readonly ElementName FLOOR = new ElementName("floor", "floor", TreeBuilderConstants.OTHER);
		public static readonly ElementName GLYPH = new ElementName("glyph", "glyph", TreeBuilderConstants.OTHER);
		public static readonly ElementName HKERN = new ElementName("hkern", "hkern", TreeBuilderConstants.OTHER);
		public static readonly ElementName IMAGE = new ElementName("image", "image", TreeBuilderConstants.IMAGE | SPECIAL);
		public static readonly ElementName IDENT = new ElementName("ident", "ident", TreeBuilderConstants.OTHER);
		public static readonly ElementName INPUT = new ElementName("input", "input", TreeBuilderConstants.INPUT | SPECIAL);
		public static readonly ElementName LABEL = new ElementName("label", "label", TreeBuilderConstants.OUTPUT_OR_LABEL);
		public static readonly ElementName LIMIT = new ElementName("limit", "limit", TreeBuilderConstants.OTHER);
		public static readonly ElementName MFRAC = new ElementName("mfrac", "mfrac", TreeBuilderConstants.OTHER);
		public static readonly ElementName MPATH = new ElementName("mpath", "mpath", TreeBuilderConstants.OTHER);
		public static readonly ElementName METER = new ElementName("meter", "meter", TreeBuilderConstants.OTHER);
		public static readonly ElementName MOVER = new ElementName("mover", "mover", TreeBuilderConstants.OTHER);
		public static readonly ElementName MINUS = new ElementName("minus", "minus", TreeBuilderConstants.OTHER);
		public static readonly ElementName MROOT = new ElementName("mroot", "mroot", TreeBuilderConstants.OTHER);
		public static readonly ElementName MSQRT = new ElementName("msqrt", "msqrt", TreeBuilderConstants.OTHER);
		public static readonly ElementName MTEXT = new ElementName("mtext", "mtext", TreeBuilderConstants.MI_MO_MN_MS_MTEXT | SCOPING_AS_MATHML);
		public static readonly ElementName NOTIN = new ElementName("notin", "notin", TreeBuilderConstants.OTHER);
		public static readonly ElementName PIECE = new ElementName("piece", "piece", TreeBuilderConstants.OTHER);
		public static readonly ElementName PARAM = new ElementName("param", "param", TreeBuilderConstants.PARAM_OR_SOURCE_OR_TRACK | SPECIAL);
		public static readonly ElementName POWER = new ElementName("power", "power", TreeBuilderConstants.OTHER);
		public static readonly ElementName REALS = new ElementName("reals", "reals", TreeBuilderConstants.OTHER);
		public static readonly ElementName STYLE = new ElementName("style", "style", TreeBuilderConstants.STYLE | SPECIAL);
		public static readonly ElementName SMALL = new ElementName("small", "small", TreeBuilderConstants.B_OR_BIG_OR_CODE_OR_EM_OR_I_OR_S_OR_SMALL_OR_STRIKE_OR_STRONG_OR_TT_OR_U);
		public static readonly ElementName THEAD = new ElementName("thead", "thead", TreeBuilderConstants.TBODY_OR_THEAD_OR_TFOOT | SPECIAL | FOSTER_PARENTING | OPTIONAL_END_TAG);
		public static readonly ElementName TABLE = new ElementName("table", "table", TreeBuilderConstants.TABLE | SPECIAL | FOSTER_PARENTING | SCOPING);
		public static readonly ElementName TITLE = new ElementName("title", "title", TreeBuilderConstants.TITLE | SPECIAL | SCOPING_AS_SVG);
		public static readonly ElementName TRACK = new ElementName("track", "track", TreeBuilderConstants.PARAM_OR_SOURCE_OR_TRACK);
		public static readonly ElementName TSPAN = new ElementName("tspan", "tspan", TreeBuilderConstants.OTHER);
		public static readonly ElementName TIMES = new ElementName("times", "times", TreeBuilderConstants.OTHER);
		public static readonly ElementName TFOOT = new ElementName("tfoot", "tfoot", TreeBuilderConstants.TBODY_OR_THEAD_OR_TFOOT | SPECIAL | FOSTER_PARENTING | OPTIONAL_END_TAG);
		public static readonly ElementName TBODY = new ElementName("tbody", "tbody", TreeBuilderConstants.TBODY_OR_THEAD_OR_TFOOT | SPECIAL | FOSTER_PARENTING | OPTIONAL_END_TAG);
		public static readonly ElementName UNION = new ElementName("union", "union", TreeBuilderConstants.OTHER);
		public static readonly ElementName VKERN = new ElementName("vkern", "vkern", TreeBuilderConstants.OTHER);
		public static readonly ElementName VIDEO = new ElementName("video", "video", TreeBuilderConstants.OTHER);
		public static readonly ElementName ARCSEC = new ElementName("arcsec", "arcsec", TreeBuilderConstants.OTHER);
		public static readonly ElementName ARCCSC = new ElementName("arccsc", "arccsc", TreeBuilderConstants.OTHER);
		public static readonly ElementName ARCTAN = new ElementName("arctan", "arctan", TreeBuilderConstants.OTHER);
		public static readonly ElementName ARCSIN = new ElementName("arcsin", "arcsin", TreeBuilderConstants.OTHER);
		public static readonly ElementName ARCCOS = new ElementName("arccos", "arccos", TreeBuilderConstants.OTHER);
		public static readonly ElementName APPLET = new ElementName("applet", "applet", TreeBuilderConstants.MARQUEE_OR_APPLET | SPECIAL | SCOPING);
		public static readonly ElementName ARCCOT = new ElementName("arccot", "arccot", TreeBuilderConstants.OTHER);
		public static readonly ElementName APPROX = new ElementName("approx", "approx", TreeBuilderConstants.OTHER);
		public static readonly ElementName BUTTON = new ElementName("button", "button", TreeBuilderConstants.BUTTON | SPECIAL);
		public static readonly ElementName CIRCLE = new ElementName("circle", "circle", TreeBuilderConstants.OTHER);
		public static readonly ElementName CENTER = new ElementName("center", "center", TreeBuilderConstants.DIV_OR_BLOCKQUOTE_OR_CENTER_OR_MENU | SPECIAL);
		public static readonly ElementName CURSOR = new ElementName("cursor", "cursor", TreeBuilderConstants.OTHER);
		public static readonly ElementName CANVAS = new ElementName("canvas", "canvas", TreeBuilderConstants.OTHER);
		public static readonly ElementName DIVIDE = new ElementName("divide", "divide", TreeBuilderConstants.OTHER);
		public static readonly ElementName DEGREE = new ElementName("degree", "degree", TreeBuilderConstants.OTHER);
		public static readonly ElementName DOMAIN = new ElementName("domain", "domain", TreeBuilderConstants.OTHER);
		public static readonly ElementName EXISTS = new ElementName("exists", "exists", TreeBuilderConstants.OTHER);
		public static readonly ElementName FETILE = new ElementName("fetile", "feTile", TreeBuilderConstants.OTHER);
		public static readonly ElementName FIGURE = new ElementName("figure", "figure", TreeBuilderConstants.ADDRESS_OR_ARTICLE_OR_ASIDE_OR_DETAILS_OR_DIR_OR_FIGCAPTION_OR_FIGURE_OR_FOOTER_OR_HEADER_OR_HGROUP_OR_NAV_OR_SECTION_OR_SUMMARY | SPECIAL);
		public static readonly ElementName FORALL = new ElementName("forall", "forall", TreeBuilderConstants.OTHER);
		public static readonly ElementName FILTER = new ElementName("filter", "filter", TreeBuilderConstants.OTHER);
		public static readonly ElementName FOOTER = new ElementName("footer", "footer", TreeBuilderConstants.ADDRESS_OR_ARTICLE_OR_ASIDE_OR_DETAILS_OR_DIR_OR_FIGCAPTION_OR_FIGURE_OR_FOOTER_OR_HEADER_OR_HGROUP_OR_NAV_OR_SECTION_OR_SUMMARY | SPECIAL);
		public static readonly ElementName HGROUP = new ElementName("hgroup", "hgroup", TreeBuilderConstants.ADDRESS_OR_ARTICLE_OR_ASIDE_OR_DETAILS_OR_DIR_OR_FIGCAPTION_OR_FIGURE_OR_FOOTER_OR_HEADER_OR_HGROUP_OR_NAV_OR_SECTION_OR_SUMMARY | SPECIAL);
		public static readonly ElementName HEADER = new ElementName("header", "header", TreeBuilderConstants.ADDRESS_OR_ARTICLE_OR_ASIDE_OR_DETAILS_OR_DIR_OR_FIGCAPTION_OR_FIGURE_OR_FOOTER_OR_HEADER_OR_HGROUP_OR_NAV_OR_SECTION_OR_SUMMARY | SPECIAL);
		public static readonly ElementName IFRAME = new ElementName("iframe", "iframe", TreeBuilderConstants.IFRAME | SPECIAL);
		public static readonly ElementName KEYGEN = new ElementName("keygen", "keygen", TreeBuilderConstants.KEYGEN | SPECIAL);
		public static readonly ElementName LAMBDA = new ElementName("lambda", "lambda", TreeBuilderConstants.OTHER);
		public static readonly ElementName LEGEND = new ElementName("legend", "legend", TreeBuilderConstants.OTHER);
		public static readonly ElementName MSPACE = new ElementName("mspace", "mspace", TreeBuilderConstants.OTHER);
		public static readonly ElementName MTABLE = new ElementName("mtable", "mtable", TreeBuilderConstants.OTHER);
		public static readonly ElementName MSTYLE = new ElementName("mstyle", "mstyle", TreeBuilderConstants.OTHER);
		public static readonly ElementName MGLYPH = new ElementName("mglyph", "mglyph", TreeBuilderConstants.MGLYPH_OR_MALIGNMARK);
		public static readonly ElementName MEDIAN = new ElementName("median", "median", TreeBuilderConstants.OTHER);
		public static readonly ElementName MUNDER = new ElementName("munder", "munder", TreeBuilderConstants.OTHER);
		public static readonly ElementName MARKER = new ElementName("marker", "marker", TreeBuilderConstants.OTHER);
		public static readonly ElementName MERROR = new ElementName("merror", "merror", TreeBuilderConstants.OTHER);
		public static readonly ElementName MOMENT = new ElementName("moment", "moment", TreeBuilderConstants.OTHER);
		public static readonly ElementName MATRIX = new ElementName("matrix", "matrix", TreeBuilderConstants.OTHER);
		public static readonly ElementName OPTION = new ElementName("option", "option", TreeBuilderConstants.OPTION | OPTIONAL_END_TAG);
		public static readonly ElementName OBJECT = new ElementName("object", "object", TreeBuilderConstants.OBJECT | SPECIAL | SCOPING);
		public static readonly ElementName OUTPUT = new ElementName("output", "output", TreeBuilderConstants.OUTPUT_OR_LABEL);
		public static readonly ElementName PRIMES = new ElementName("primes", "primes", TreeBuilderConstants.OTHER);
		public static readonly ElementName SOURCE = new ElementName("source", "source", TreeBuilderConstants.PARAM_OR_SOURCE_OR_TRACK);
		public static readonly ElementName STRIKE = new ElementName("strike", "strike", TreeBuilderConstants.B_OR_BIG_OR_CODE_OR_EM_OR_I_OR_S_OR_SMALL_OR_STRIKE_OR_STRONG_OR_TT_OR_U);
		public static readonly ElementName STRONG = new ElementName("strong", "strong", TreeBuilderConstants.B_OR_BIG_OR_CODE_OR_EM_OR_I_OR_S_OR_SMALL_OR_STRIKE_OR_STRONG_OR_TT_OR_U);
		public static readonly ElementName SWITCH = new ElementName("switch", "switch", TreeBuilderConstants.OTHER);
		public static readonly ElementName SYMBOL = new ElementName("symbol", "symbol", TreeBuilderConstants.OTHER);
		public static readonly ElementName SELECT = new ElementName("select", "select", TreeBuilderConstants.SELECT | SPECIAL);
		public static readonly ElementName SUBSET = new ElementName("subset", "subset", TreeBuilderConstants.OTHER);
		public static readonly ElementName SCRIPT = new ElementName("script", "script", TreeBuilderConstants.SCRIPT | SPECIAL);
		public static readonly ElementName TBREAK = new ElementName("tbreak", "tbreak", TreeBuilderConstants.OTHER);
		public static readonly ElementName VECTOR = new ElementName("vector", "vector", TreeBuilderConstants.OTHER);
		public static readonly ElementName ARTICLE = new ElementName("article", "article", TreeBuilderConstants.ADDRESS_OR_ARTICLE_OR_ASIDE_OR_DETAILS_OR_DIR_OR_FIGCAPTION_OR_FIGURE_OR_FOOTER_OR_HEADER_OR_HGROUP_OR_NAV_OR_SECTION_OR_SUMMARY | SPECIAL);
		public static readonly ElementName ANIMATE = new ElementName("animate", "animate", TreeBuilderConstants.OTHER);
		public static readonly ElementName ARCSECH = new ElementName("arcsech", "arcsech", TreeBuilderConstants.OTHER);
		public static readonly ElementName ARCCSCH = new ElementName("arccsch", "arccsch", TreeBuilderConstants.OTHER);
		public static readonly ElementName ARCTANH = new ElementName("arctanh", "arctanh", TreeBuilderConstants.OTHER);
		public static readonly ElementName ARCSINH = new ElementName("arcsinh", "arcsinh", TreeBuilderConstants.OTHER);
		public static readonly ElementName ARCCOSH = new ElementName("arccosh", "arccosh", TreeBuilderConstants.OTHER);
		public static readonly ElementName ARCCOTH = new ElementName("arccoth", "arccoth", TreeBuilderConstants.OTHER);
		public static readonly ElementName ACRONYM = new ElementName("acronym", "acronym", TreeBuilderConstants.OTHER);
		public static readonly ElementName ADDRESS = new ElementName("address", "address", TreeBuilderConstants.ADDRESS_OR_ARTICLE_OR_ASIDE_OR_DETAILS_OR_DIR_OR_FIGCAPTION_OR_FIGURE_OR_FOOTER_OR_HEADER_OR_HGROUP_OR_NAV_OR_SECTION_OR_SUMMARY | SPECIAL);
		public static readonly ElementName BGSOUND = new ElementName("bgsound", "bgsound", TreeBuilderConstants.LINK_OR_BASEFONT_OR_BGSOUND | SPECIAL);
		public static readonly ElementName COMMAND = new ElementName("command", "command", TreeBuilderConstants.COMMAND | SPECIAL);
		public static readonly ElementName COMPOSE = new ElementName("compose", "compose", TreeBuilderConstants.OTHER);
		public static readonly ElementName CEILING = new ElementName("ceiling", "ceiling", TreeBuilderConstants.OTHER);
		public static readonly ElementName CSYMBOL = new ElementName("csymbol", "csymbol", TreeBuilderConstants.OTHER);
		public static readonly ElementName CAPTION = new ElementName("caption", "caption", TreeBuilderConstants.CAPTION | SPECIAL | SCOPING);
		public static readonly ElementName DISCARD = new ElementName("discard", "discard", TreeBuilderConstants.OTHER);
		public static readonly ElementName DECLARE = new ElementName("declare", "declare", TreeBuilderConstants.OTHER);
		public static readonly ElementName DETAILS = new ElementName("details", "details", TreeBuilderConstants.ADDRESS_OR_ARTICLE_OR_ASIDE_OR_DETAILS_OR_DIR_OR_FIGCAPTION_OR_FIGURE_OR_FOOTER_OR_HEADER_OR_HGROUP_OR_NAV_OR_SECTION_OR_SUMMARY | SPECIAL);
		public static readonly ElementName ELLIPSE = new ElementName("ellipse", "ellipse", TreeBuilderConstants.OTHER);
		public static readonly ElementName FEFUNCA = new ElementName("fefunca", "feFuncA", TreeBuilderConstants.OTHER);
		public static readonly ElementName FEFUNCB = new ElementName("fefuncb", "feFuncB", TreeBuilderConstants.OTHER);
		public static readonly ElementName FEBLEND = new ElementName("feblend", "feBlend", TreeBuilderConstants.OTHER);
		public static readonly ElementName FEFLOOD = new ElementName("feflood", "feFlood", TreeBuilderConstants.OTHER);
		public static readonly ElementName FEIMAGE = new ElementName("feimage", "feImage", TreeBuilderConstants.OTHER);
		public static readonly ElementName FEMERGE = new ElementName("femerge", "feMerge", TreeBuilderConstants.OTHER);
		public static readonly ElementName FEFUNCG = new ElementName("fefuncg", "feFuncG", TreeBuilderConstants.OTHER);
		public static readonly ElementName FEFUNCR = new ElementName("fefuncr", "feFuncR", TreeBuilderConstants.OTHER);
		public static readonly ElementName HANDLER = new ElementName("handler", "handler", TreeBuilderConstants.OTHER);
		public static readonly ElementName INVERSE = new ElementName("inverse", "inverse", TreeBuilderConstants.OTHER);
		public static readonly ElementName IMPLIES = new ElementName("implies", "implies", TreeBuilderConstants.OTHER);
		public static readonly ElementName ISINDEX = new ElementName("isindex", "isindex", TreeBuilderConstants.ISINDEX | SPECIAL);
		public static readonly ElementName LOGBASE = new ElementName("logbase", "logbase", TreeBuilderConstants.OTHER);
		public static readonly ElementName LISTING = new ElementName("listing", "listing", TreeBuilderConstants.PRE_OR_LISTING | SPECIAL);
		public static readonly ElementName MFENCED = new ElementName("mfenced", "mfenced", TreeBuilderConstants.OTHER);
		public static readonly ElementName MPADDED = new ElementName("mpadded", "mpadded", TreeBuilderConstants.OTHER);
		public static readonly ElementName MARQUEE = new ElementName("marquee", "marquee", TreeBuilderConstants.MARQUEE_OR_APPLET | SPECIAL | SCOPING);
		public static readonly ElementName MACTION = new ElementName("maction", "maction", TreeBuilderConstants.OTHER);
		public static readonly ElementName MSUBSUP = new ElementName("msubsup", "msubsup", TreeBuilderConstants.OTHER);
		public static readonly ElementName NOEMBED = new ElementName("noembed", "noembed", TreeBuilderConstants.NOEMBED | SPECIAL);
		public static readonly ElementName POLYGON = new ElementName("polygon", "polygon", TreeBuilderConstants.OTHER);
		public static readonly ElementName PATTERN = new ElementName("pattern", "pattern", TreeBuilderConstants.OTHER);
		public static readonly ElementName PRODUCT = new ElementName("product", "product", TreeBuilderConstants.OTHER);
		public static readonly ElementName SETDIFF = new ElementName("setdiff", "setdiff", TreeBuilderConstants.OTHER);
		public static readonly ElementName SECTION = new ElementName("section", "section", TreeBuilderConstants.ADDRESS_OR_ARTICLE_OR_ASIDE_OR_DETAILS_OR_DIR_OR_FIGCAPTION_OR_FIGURE_OR_FOOTER_OR_HEADER_OR_HGROUP_OR_NAV_OR_SECTION_OR_SUMMARY | SPECIAL);
		public static readonly ElementName SUMMARY = new ElementName("summary", "summary", TreeBuilderConstants.ADDRESS_OR_ARTICLE_OR_ASIDE_OR_DETAILS_OR_DIR_OR_FIGCAPTION_OR_FIGURE_OR_FOOTER_OR_HEADER_OR_HGROUP_OR_NAV_OR_SECTION_OR_SUMMARY | SPECIAL);
		public static readonly ElementName TENDSTO = new ElementName("tendsto", "tendsto", TreeBuilderConstants.OTHER);
		public static readonly ElementName UPLIMIT = new ElementName("uplimit", "uplimit", TreeBuilderConstants.OTHER);
		public static readonly ElementName ALTGLYPH = new ElementName("altglyph", "altGlyph", TreeBuilderConstants.OTHER);
		public static readonly ElementName BASEFONT = new ElementName("basefont", "basefont", TreeBuilderConstants.LINK_OR_BASEFONT_OR_BGSOUND | SPECIAL);
		public static readonly ElementName CLIPPATH = new ElementName("clippath", "clipPath", TreeBuilderConstants.OTHER);
		public static readonly ElementName CODOMAIN = new ElementName("codomain", "codomain", TreeBuilderConstants.OTHER);
		public static readonly ElementName COLGROUP = new ElementName("colgroup", "colgroup", TreeBuilderConstants.COLGROUP | SPECIAL | OPTIONAL_END_TAG);
		public static readonly ElementName EMPTYSET = new ElementName("emptyset", "emptyset", TreeBuilderConstants.OTHER);
		public static readonly ElementName FACTOROF = new ElementName("factorof", "factorof", TreeBuilderConstants.OTHER);
		public static readonly ElementName FIELDSET = new ElementName("fieldset", "fieldset", TreeBuilderConstants.FIELDSET | SPECIAL);
		public static readonly ElementName FRAMESET = new ElementName("frameset", "frameset", TreeBuilderConstants.FRAMESET | SPECIAL);
		public static readonly ElementName FEOFFSET = new ElementName("feoffset", "feOffset", TreeBuilderConstants.OTHER);
		public static readonly ElementName GLYPHREF = new ElementName("glyphref", "glyphRef", TreeBuilderConstants.OTHER);
		public static readonly ElementName INTERVAL = new ElementName("interval", "interval", TreeBuilderConstants.OTHER);
		public static readonly ElementName INTEGERS = new ElementName("integers", "integers", TreeBuilderConstants.OTHER);
		public static readonly ElementName INFINITY = new ElementName("infinity", "infinity", TreeBuilderConstants.OTHER);
		public static readonly ElementName LISTENER = new ElementName("listener", "listener", TreeBuilderConstants.OTHER);
		public static readonly ElementName LOWLIMIT = new ElementName("lowlimit", "lowlimit", TreeBuilderConstants.OTHER);
		public static readonly ElementName METADATA = new ElementName("metadata", "metadata", TreeBuilderConstants.OTHER);
		public static readonly ElementName MENCLOSE = new ElementName("menclose", "menclose", TreeBuilderConstants.OTHER);
		public static readonly ElementName MPHANTOM = new ElementName("mphantom", "mphantom", TreeBuilderConstants.OTHER);
		public static readonly ElementName NOFRAMES = new ElementName("noframes", "noframes", TreeBuilderConstants.NOFRAMES | SPECIAL);
		public static readonly ElementName NOSCRIPT = new ElementName("noscript", "noscript", TreeBuilderConstants.NOSCRIPT | SPECIAL);
		public static readonly ElementName OPTGROUP = new ElementName("optgroup", "optgroup", TreeBuilderConstants.OPTGROUP | SPECIAL | OPTIONAL_END_TAG);
		public static readonly ElementName POLYLINE = new ElementName("polyline", "polyline", TreeBuilderConstants.OTHER);
		public static readonly ElementName PREFETCH = new ElementName("prefetch", "prefetch", TreeBuilderConstants.OTHER);
		public static readonly ElementName PROGRESS = new ElementName("progress", "progress", TreeBuilderConstants.OTHER);
		public static readonly ElementName PRSUBSET = new ElementName("prsubset", "prsubset", TreeBuilderConstants.OTHER);
		public static readonly ElementName QUOTIENT = new ElementName("quotient", "quotient", TreeBuilderConstants.OTHER);
		public static readonly ElementName SELECTOR = new ElementName("selector", "selector", TreeBuilderConstants.OTHER);
		public static readonly ElementName TEXTAREA = new ElementName("textarea", "textarea", TreeBuilderConstants.TEXTAREA | SPECIAL);
		public static readonly ElementName TEXTPATH = new ElementName("textpath", "textPath", TreeBuilderConstants.OTHER);
		public static readonly ElementName VARIANCE = new ElementName("variance", "variance", TreeBuilderConstants.OTHER);
		public static readonly ElementName ANIMATION = new ElementName("animation", "animation", TreeBuilderConstants.OTHER);
		public static readonly ElementName CONJUGATE = new ElementName("conjugate", "conjugate", TreeBuilderConstants.OTHER);
		public static readonly ElementName CONDITION = new ElementName("condition", "condition", TreeBuilderConstants.OTHER);
		public static readonly ElementName COMPLEXES = new ElementName("complexes", "complexes", TreeBuilderConstants.OTHER);
		public static readonly ElementName FONT_FACE = new ElementName("font-face", "font-face", TreeBuilderConstants.OTHER);
		public static readonly ElementName FACTORIAL = new ElementName("factorial", "factorial", TreeBuilderConstants.OTHER);
		public static readonly ElementName INTERSECT = new ElementName("intersect", "intersect", TreeBuilderConstants.OTHER);
		public static readonly ElementName IMAGINARY = new ElementName("imaginary", "imaginary", TreeBuilderConstants.OTHER);
		public static readonly ElementName LAPLACIAN = new ElementName("laplacian", "laplacian", TreeBuilderConstants.OTHER);
		public static readonly ElementName MATRIXROW = new ElementName("matrixrow", "matrixrow", TreeBuilderConstants.OTHER);
		public static readonly ElementName NOTSUBSET = new ElementName("notsubset", "notsubset", TreeBuilderConstants.OTHER);
		public static readonly ElementName OTHERWISE = new ElementName("otherwise", "otherwise", TreeBuilderConstants.OTHER);
		public static readonly ElementName PIECEWISE = new ElementName("piecewise", "piecewise", TreeBuilderConstants.OTHER);
		public static readonly ElementName PLAINTEXT = new ElementName("plaintext", "plaintext", TreeBuilderConstants.PLAINTEXT | SPECIAL);
		public static readonly ElementName RATIONALS = new ElementName("rationals", "rationals", TreeBuilderConstants.OTHER);
		public static readonly ElementName SEMANTICS = new ElementName("semantics", "semantics", TreeBuilderConstants.OTHER);
		public static readonly ElementName TRANSPOSE = new ElementName("transpose", "transpose", TreeBuilderConstants.OTHER);
		public static readonly ElementName ANNOTATION = new ElementName("annotation", "annotation", TreeBuilderConstants.OTHER);
		public static readonly ElementName BLOCKQUOTE = new ElementName("blockquote", "blockquote", TreeBuilderConstants.DIV_OR_BLOCKQUOTE_OR_CENTER_OR_MENU | SPECIAL);
		public static readonly ElementName DIVERGENCE = new ElementName("divergence", "divergence", TreeBuilderConstants.OTHER);
		public static readonly ElementName EULERGAMMA = new ElementName("eulergamma", "eulergamma", TreeBuilderConstants.OTHER);
		public static readonly ElementName EQUIVALENT = new ElementName("equivalent", "equivalent", TreeBuilderConstants.OTHER);
		public static readonly ElementName FIGCAPTION = new ElementName("figcaption", "figcaption", TreeBuilderConstants.ADDRESS_OR_ARTICLE_OR_ASIDE_OR_DETAILS_OR_DIR_OR_FIGCAPTION_OR_FIGURE_OR_FOOTER_OR_HEADER_OR_HGROUP_OR_NAV_OR_SECTION_OR_SUMMARY | SPECIAL);
		public static readonly ElementName IMAGINARYI = new ElementName("imaginaryi", "imaginaryi", TreeBuilderConstants.OTHER);
		public static readonly ElementName MALIGNMARK = new ElementName("malignmark", "malignmark", TreeBuilderConstants.MGLYPH_OR_MALIGNMARK);
		public static readonly ElementName MUNDEROVER = new ElementName("munderover", "munderover", TreeBuilderConstants.OTHER);
		public static readonly ElementName MLABELEDTR = new ElementName("mlabeledtr", "mlabeledtr", TreeBuilderConstants.OTHER);
		public static readonly ElementName NOTANUMBER = new ElementName("notanumber", "notanumber", TreeBuilderConstants.OTHER);
		public static readonly ElementName SOLIDCOLOR = new ElementName("solidcolor", "solidcolor", TreeBuilderConstants.OTHER);
		public static readonly ElementName ALTGLYPHDEF = new ElementName("altglyphdef", "altGlyphDef", TreeBuilderConstants.OTHER);
		public static readonly ElementName DETERMINANT = new ElementName("determinant", "determinant", TreeBuilderConstants.OTHER);
		public static readonly ElementName FEMERGENODE = new ElementName("femergenode", "feMergeNode", TreeBuilderConstants.OTHER);
		public static readonly ElementName FECOMPOSITE = new ElementName("fecomposite", "feComposite", TreeBuilderConstants.OTHER);
		public static readonly ElementName FESPOTLIGHT = new ElementName("fespotlight", "feSpotLight", TreeBuilderConstants.OTHER);
		public static readonly ElementName MALIGNGROUP = new ElementName("maligngroup", "maligngroup", TreeBuilderConstants.OTHER);
		public static readonly ElementName MPRESCRIPTS = new ElementName("mprescripts", "mprescripts", TreeBuilderConstants.OTHER);
		public static readonly ElementName MOMENTABOUT = new ElementName("momentabout", "momentabout", TreeBuilderConstants.OTHER);
		public static readonly ElementName NOTPRSUBSET = new ElementName("notprsubset", "notprsubset", TreeBuilderConstants.OTHER);
		public static readonly ElementName PARTIALDIFF = new ElementName("partialdiff", "partialdiff", TreeBuilderConstants.OTHER);
		public static readonly ElementName ALTGLYPHITEM = new ElementName("altglyphitem", "altGlyphItem", TreeBuilderConstants.OTHER);
		public static readonly ElementName ANIMATECOLOR = new ElementName("animatecolor", "animateColor", TreeBuilderConstants.OTHER);
		public static readonly ElementName DATATEMPLATE = new ElementName("datatemplate", "datatemplate", TreeBuilderConstants.OTHER);
		public static readonly ElementName EXPONENTIALE = new ElementName("exponentiale", "exponentiale", TreeBuilderConstants.OTHER);
		public static readonly ElementName FETURBULENCE = new ElementName("feturbulence", "feTurbulence", TreeBuilderConstants.OTHER);
		public static readonly ElementName FEPOINTLIGHT = new ElementName("fepointlight", "fePointLight", TreeBuilderConstants.OTHER);
		public static readonly ElementName FEMORPHOLOGY = new ElementName("femorphology", "feMorphology", TreeBuilderConstants.OTHER);
		public static readonly ElementName OUTERPRODUCT = new ElementName("outerproduct", "outerproduct", TreeBuilderConstants.OTHER);
		public static readonly ElementName ANIMATEMOTION = new ElementName("animatemotion", "animateMotion", TreeBuilderConstants.OTHER);
		public static readonly ElementName COLOR_PROFILE = new ElementName("color-profile", "color-profile", TreeBuilderConstants.OTHER);
		public static readonly ElementName FONT_FACE_SRC = new ElementName("font-face-src", "font-face-src", TreeBuilderConstants.OTHER);
		public static readonly ElementName FONT_FACE_URI = new ElementName("font-face-uri", "font-face-uri", TreeBuilderConstants.OTHER);
		public static readonly ElementName FOREIGNOBJECT = new ElementName("foreignobject", "foreignObject", TreeBuilderConstants.FOREIGNOBJECT_OR_DESC | SCOPING_AS_SVG);
		public static readonly ElementName FECOLORMATRIX = new ElementName("fecolormatrix", "feColorMatrix", TreeBuilderConstants.OTHER);
		public static readonly ElementName MISSING_GLYPH = new ElementName("missing-glyph", "missing-glyph", TreeBuilderConstants.OTHER);
		public static readonly ElementName MMULTISCRIPTS = new ElementName("mmultiscripts", "mmultiscripts", TreeBuilderConstants.OTHER);
		public static readonly ElementName SCALARPRODUCT = new ElementName("scalarproduct", "scalarproduct", TreeBuilderConstants.OTHER);
		public static readonly ElementName VECTORPRODUCT = new ElementName("vectorproduct", "vectorproduct", TreeBuilderConstants.OTHER);
		public static readonly ElementName ANNOTATION_XML = new ElementName("annotation-xml", "annotation-xml", TreeBuilderConstants.ANNOTATION_XML | SCOPING_AS_MATHML);
		public static readonly ElementName DEFINITION_SRC = new ElementName("definition-src", "definition-src", TreeBuilderConstants.OTHER);
		public static readonly ElementName FONT_FACE_NAME = new ElementName("font-face-name", "font-face-name", TreeBuilderConstants.OTHER);
		public static readonly ElementName FEGAUSSIANBLUR = new ElementName("fegaussianblur", "feGaussianBlur", TreeBuilderConstants.OTHER);
		public static readonly ElementName FEDISTANTLIGHT = new ElementName("fedistantlight", "feDistantLight", TreeBuilderConstants.OTHER);
		public static readonly ElementName LINEARGRADIENT = new ElementName("lineargradient", "linearGradient", TreeBuilderConstants.OTHER);
		public static readonly ElementName NATURALNUMBERS = new ElementName("naturalnumbers", "naturalnumbers", TreeBuilderConstants.OTHER);
		public static readonly ElementName RADIALGRADIENT = new ElementName("radialgradient", "radialGradient", TreeBuilderConstants.OTHER);
		public static readonly ElementName ANIMATETRANSFORM = new ElementName("animatetransform", "animateTransform", TreeBuilderConstants.OTHER);
		public static readonly ElementName CARTESIANPRODUCT = new ElementName("cartesianproduct", "cartesianproduct", TreeBuilderConstants.OTHER);
		public static readonly ElementName FONT_FACE_FORMAT = new ElementName("font-face-format", "font-face-format", TreeBuilderConstants.OTHER);
		public static readonly ElementName FECONVOLVEMATRIX = new ElementName("feconvolvematrix", "feConvolveMatrix", TreeBuilderConstants.OTHER);
		public static readonly ElementName FEDIFFUSELIGHTING = new ElementName("fediffuselighting", "feDiffuseLighting", TreeBuilderConstants.OTHER);
		public static readonly ElementName FEDISPLACEMENTMAP = new ElementName("fedisplacementmap", "feDisplacementMap", TreeBuilderConstants.OTHER);
		public static readonly ElementName FESPECULARLIGHTING = new ElementName("fespecularlighting", "feSpecularLighting", TreeBuilderConstants.OTHER);
		public static readonly ElementName DOMAINOFAPPLICATION = new ElementName("domainofapplication", "domainofapplication", TreeBuilderConstants.OTHER);
		public static readonly ElementName FECOMPONENTTRANSFER = new ElementName("fecomponenttransfer", "feComponentTransfer", TreeBuilderConstants.OTHER);
		private static readonly ElementName[] ELEMENT_NAMES = {
	A,
	B,
	G,
	I,
	P,
	Q,
	S,
	U,
	BR,
	CI,
	CN,
	DD,
	DL,
	DT,
	EM,
	EQ,
	FN,
	H1,
	H2,
	H3,
	H4,
	H5,
	H6,
	GT,
	HR,
	IN,
	LI,
	LN,
	LT,
	MI,
	MN,
	MO,
	MS,
	OL,
	OR,
	PI,
	RP,
	RT,
	TD,
	TH,
	TR,
	TT,
	UL,
	AND,
	ARG,
	ABS,
	BIG,
	BDO,
	CSC,
	COL,
	COS,
	COT,
	DEL,
	DFN,
	DIR,
	DIV,
	EXP,
	GCD,
	GEQ,
	IMG,
	INS,
	INT,
	KBD,
	LOG,
	LCM,
	LEQ,
	MTD,
	MIN,
	MAP,
	MTR,
	MAX,
	NEQ,
	NOT,
	NAV,
	PRE,
	REM,
	SUB,
	SEC,
	SVG,
	SUM,
	SIN,
	SEP,
	SUP,
	SET,
	TAN,
	USE,
	VAR,
	WBR,
	XMP,
	XOR,
	AREA,
	ABBR,
	BASE,
	BVAR,
	BODY,
	CARD,
	CODE,
	CITE,
	CSCH,
	COSH,
	COTH,
	CURL,
	DESC,
	DIFF,
	DEFS,
	FORM,
	FONT,
	GRAD,
	HEAD,
	HTML,
	LINE,
	LINK,
	LIST,
	META,
	MSUB,
	MODE,
	MATH,
	MARK,
	MASK,
	MEAN,
	MSUP,
	MENU,
	MROW,
	NONE,
	NOBR,
	NEST,
	PATH,
	PLUS,
	RULE,
	REAL,
	RELN,
	RECT,
	ROOT,
	RUBY,
	SECH,
	SINH,
	SPAN,
	SAMP,
	STOP,
	SDEV,
	TIME,
	TRUE,
	TREF,
	TANH,
	TEXT,
	VIEW,
	ASIDE,
	AUDIO,
	APPLY,
	EMBED,
	FRAME,
	FALSE,
	FLOOR,
	GLYPH,
	HKERN,
	IMAGE,
	IDENT,
	INPUT,
	LABEL,
	LIMIT,
	MFRAC,
	MPATH,
	METER,
	MOVER,
	MINUS,
	MROOT,
	MSQRT,
	MTEXT,
	NOTIN,
	PIECE,
	PARAM,
	POWER,
	REALS,
	STYLE,
	SMALL,
	THEAD,
	TABLE,
	TITLE,
	TRACK,
	TSPAN,
	TIMES,
	TFOOT,
	TBODY,
	UNION,
	VKERN,
	VIDEO,
	ARCSEC,
	ARCCSC,
	ARCTAN,
	ARCSIN,
	ARCCOS,
	APPLET,
	ARCCOT,
	APPROX,
	BUTTON,
	CIRCLE,
	CENTER,
	CURSOR,
	CANVAS,
	DIVIDE,
	DEGREE,
	DOMAIN,
	EXISTS,
	FETILE,
	FIGURE,
	FORALL,
	FILTER,
	FOOTER,
	HGROUP,
	HEADER,
	IFRAME,
	KEYGEN,
	LAMBDA,
	LEGEND,
	MSPACE,
	MTABLE,
	MSTYLE,
	MGLYPH,
	MEDIAN,
	MUNDER,
	MARKER,
	MERROR,
	MOMENT,
	MATRIX,
	OPTION,
	OBJECT,
	OUTPUT,
	PRIMES,
	SOURCE,
	STRIKE,
	STRONG,
	SWITCH,
	SYMBOL,
	SELECT,
	SUBSET,
	SCRIPT,
	TBREAK,
	VECTOR,
	ARTICLE,
	ANIMATE,
	ARCSECH,
	ARCCSCH,
	ARCTANH,
	ARCSINH,
	ARCCOSH,
	ARCCOTH,
	ACRONYM,
	ADDRESS,
	BGSOUND,
	COMMAND,
	COMPOSE,
	CEILING,
	CSYMBOL,
	CAPTION,
	DISCARD,
	DECLARE,
	DETAILS,
	ELLIPSE,
	FEFUNCA,
	FEFUNCB,
	FEBLEND,
	FEFLOOD,
	FEIMAGE,
	FEMERGE,
	FEFUNCG,
	FEFUNCR,
	HANDLER,
	INVERSE,
	IMPLIES,
	ISINDEX,
	LOGBASE,
	LISTING,
	MFENCED,
	MPADDED,
	MARQUEE,
	MACTION,
	MSUBSUP,
	NOEMBED,
	POLYGON,
	PATTERN,
	PRODUCT,
	SETDIFF,
	SECTION,
	SUMMARY,
	TENDSTO,
	UPLIMIT,
	ALTGLYPH,
	BASEFONT,
	CLIPPATH,
	CODOMAIN,
	COLGROUP,
	EMPTYSET,
	FACTOROF,
	FIELDSET,
	FRAMESET,
	FEOFFSET,
	GLYPHREF,
	INTERVAL,
	INTEGERS,
	INFINITY,
	LISTENER,
	LOWLIMIT,
	METADATA,
	MENCLOSE,
	MPHANTOM,
	NOFRAMES,
	NOSCRIPT,
	OPTGROUP,
	POLYLINE,
	PREFETCH,
	PROGRESS,
	PRSUBSET,
	QUOTIENT,
	SELECTOR,
	TEXTAREA,
	TEXTPATH,
	VARIANCE,
	ANIMATION,
	CONJUGATE,
	CONDITION,
	COMPLEXES,
	FONT_FACE,
	FACTORIAL,
	INTERSECT,
	IMAGINARY,
	LAPLACIAN,
	MATRIXROW,
	NOTSUBSET,
	OTHERWISE,
	PIECEWISE,
	PLAINTEXT,
	RATIONALS,
	SEMANTICS,
	TRANSPOSE,
	ANNOTATION,
	BLOCKQUOTE,
	DIVERGENCE,
	EULERGAMMA,
	EQUIVALENT,
	FIGCAPTION,
	IMAGINARYI,
	MALIGNMARK,
	MUNDEROVER,
	MLABELEDTR,
	NOTANUMBER,
	SOLIDCOLOR,
	ALTGLYPHDEF,
	DETERMINANT,
	FEMERGENODE,
	FECOMPOSITE,
	FESPOTLIGHT,
	MALIGNGROUP,
	MPRESCRIPTS,
	MOMENTABOUT,
	NOTPRSUBSET,
	PARTIALDIFF,
	ALTGLYPHITEM,
	ANIMATECOLOR,
	DATATEMPLATE,
	EXPONENTIALE,
	FETURBULENCE,
	FEPOINTLIGHT,
	FEMORPHOLOGY,
	OUTERPRODUCT,
	ANIMATEMOTION,
	COLOR_PROFILE,
	FONT_FACE_SRC,
	FONT_FACE_URI,
	FOREIGNOBJECT,
	FECOLORMATRIX,
	MISSING_GLYPH,
	MMULTISCRIPTS,
	SCALARPRODUCT,
	VECTORPRODUCT,
	ANNOTATION_XML,
	DEFINITION_SRC,
	FONT_FACE_NAME,
	FEGAUSSIANBLUR,
	FEDISTANTLIGHT,
	LINEARGRADIENT,
	NATURALNUMBERS,
	RADIALGRADIENT,
	ANIMATETRANSFORM,
	CARTESIANPRODUCT,
	FONT_FACE_FORMAT,
	FECONVOLVEMATRIX,
	FEDIFFUSELIGHTING,
	FEDISPLACEMENTMAP,
	FESPECULARLIGHTING,
	DOMAINOFAPPLICATION,
	FECOMPONENTTRANSFER,
	};
		private static readonly int[] ELEMENT_HASHES = {
	1057,
	1090,
	1255,
	1321,
	1552,
	1585,
	1651,
	1717,
	68162,
	68899,
	69059,
	69764,
	70020,
	70276,
	71077,
	71205,
	72134,
	72232,
	72264,
	72296,
	72328,
	72360,
	72392,
	73351,
	74312,
	75209,
	78124,
	78284,
	78476,
	79149,
	79309,
	79341,
	79469,
	81295,
	81487,
	82224,
	84498,
	84626,
	86164,
	86292,
	86612,
	86676,
	87445,
	3183041,
	3186241,
	3198017,
	3218722,
	3226754,
	3247715,
	3256803,
	3263971,
	3264995,
	3289252,
	3291332,
	3295524,
	3299620,
	3326725,
	3379303,
	3392679,
	3448233,
	3460553,
	3461577,
	3510347,
	3546604,
	3552364,
	3556524,
	3576461,
	3586349,
	3588141,
	3590797,
	3596333,
	3622062,
	3625454,
	3627054,
	3675728,
	3749042,
	3771059,
	3771571,
	3776211,
	3782323,
	3782963,
	3784883,
	3785395,
	3788979,
	3815476,
	3839605,
	3885110,
	3917911,
	3948984,
	3951096,
	135304769,
	135858241,
	136498210,
	136906434,
	137138658,
	137512995,
	137531875,
	137548067,
	137629283,
	137645539,
	137646563,
	137775779,
	138529956,
	138615076,
	139040932,
	140954086,
	141179366,
	141690439,
	142738600,
	143013512,
	146979116,
	147175724,
	147475756,
	147902637,
	147936877,
	148017645,
	148131885,
	148228141,
	148229165,
	148309165,
	148395629,
	148551853,
	148618829,
	149076462,
	149490158,
	149572782,
	151277616,
	151639440,
	153268914,
	153486514,
	153563314,
	153750706,
	153763314,
	153914034,
	154406067,
	154417459,
	154600979,
	154678323,
	154680979,
	154866835,
	155366708,
	155375188,
	155391572,
	155465780,
	155869364,
	158045494,
	168988979,
	169321621,
	169652752,
	173151309,
	174240818,
	174247297,
	174669292,
	175391532,
	176638123,
	177380397,
	177879204,
	177886734,
	180753473,
	181020073,
	181503558,
	181686320,
	181999237,
	181999311,
	182048201,
	182074866,
	182078003,
	182083764,
	182920847,
	184716457,
	184976961,
	185145071,
	187281445,
	187872052,
	188100653,
	188875944,
	188919873,
	188920457,
	189107250,
	189203987,
	189371817,
	189414886,
	189567458,
	190266670,
	191318187,
	191337609,
	202479203,
	202493027,
	202835587,
	202843747,
	203013219,
	203036048,
	203045987,
	203177552,
	203898516,
	204648562,
	205067918,
	205078130,
	205096654,
	205689142,
	205690439,
	205988909,
	207213161,
	207794484,
	207800999,
	208023602,
	208213644,
	208213647,
	210261490,
	210310273,
	210940978,
	213325049,
	213946445,
	214055079,
	215125040,
	215134273,
	215135028,
	215237420,
	215418148,
	215553166,
	215553394,
	215563858,
	215627949,
	215754324,
	217529652,
	217713834,
	217732628,
	218731945,
	221417045,
	221424946,
	221493746,
	221515401,
	221658189,
	221908140,
	221910626,
	221921586,
	222659762,
	225001091,
	236105833,
	236113965,
	236194995,
	236195427,
	236206132,
	236206387,
	236211683,
	236212707,
	236381647,
	236571826,
	237124271,
	238172205,
	238210544,
	238270764,
	238435405,
	238501172,
	239224867,
	239257644,
	239710497,
	240307721,
	241208789,
	241241557,
	241318060,
	241319404,
	241343533,
	241344069,
	241405397,
	241765845,
	243864964,
	244502085,
	244946220,
	245109902,
	247647266,
	247707956,
	248648814,
	248648836,
	248682161,
	248986932,
	249058914,
	249697357,
	252132601,
	252135604,
	252317348,
	255007012,
	255278388,
	255641645,
	256365156,
	257566121,
	269763372,
	271202790,
	271863856,
	272049197,
	272127474,
	274339449,
	274939471,
	275388004,
	275388005,
	275388006,
	275977800,
	278267602,
	278513831,
	278712622,
	281613765,
	281683369,
	282120228,
	282250732,
	282508942,
	283743649,
	283787570,
	284710386,
	285391148,
	285478533,
	285854898,
	285873762,
	286931113,
	288964227,
	289445441,
	289689648,
	291671489,
	303512884,
	305319975,
	305610036,
	305764101,
	308448294,
	308675890,
	312085683,
	312264750,
	315032867,
	316391000,
	317331042,
	317902135,
	318950711,
	319447220,
	321499182,
	322538804,
	323145200,
	337067316,
	337826293,
	339905989,
	340833697,
	341457068,
	342310196,
	345302593,
	349554733,
	349771471,
	349786245,
	350819405,
	356072847,
	370349192,
	373962798,
	375558638,
	375574835,
	376053993,
	383276530,
	383373833,
	383407586,
	384439906,
	386079012,
	404133513,
	404307343,
	407031852,
	408072233,
	409112005,
	409608425,
	409771500,
	419040932,
	437730612,
	439529766,
	442616365,
	442813037,
	443157674,
	443295316,
	450118444,
	450482697,
	456789668,
	459935396,
	471217869,
	474073645,
	476230702,
	476665218,
	476717289,
	483014825,
	485083298,
	489306281,
	538364390,
	540675748,
	543819186,
	543958612,
	576960820,
	577242548,
	610515252,
	642202932,
	644420819,
	};
	}

}
