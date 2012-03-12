/*
 * Copyright (c) 2005-2007 Henri Sivonen
 * Copyright (c) 2007-2010 Mozilla Foundation
 * Portions of comments Copyright 2004-2010 Apple Computer, Inc., Mozilla 
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
 * comment are quotes from the WHATWG HTML 5 spec as of 2 June 2007 
 * amended as of June 18 2008 and May 31 2010.
 * That document came with this statement:
 * © Copyright 2004-2010 Apple Computer, Inc., Mozilla Foundation, and 
 * Opera Software ASA. You are granted a license to use, reproduce and 
 * create derivative works of this document."
 */

using System;
using System.Diagnostics;
using HtmlParserSharp.Common;

namespace HtmlParserSharp
{
	/// <summary>
	/// An implementation of
	/// http://www.whatwg.org/specs/web-apps/current-work/multipage/tokenization.html
	/// This class implements the <code>Locator</code> interface. This is not an
	/// incidental implementation detail: Users of this class are encouraged to make
	/// use of the <code>Locator</code> nature.
	/// By default, the tokenizer may report data that XML 1.0 bans. The tokenizer
	/// can be configured to treat these conditions as fatal or to coerce the infoset
	/// to something that XML 1.0 allows.
	/// </summary>
	public class Tokenizer : ILocator
	{
		private const int DATA_AND_RCDATA_MASK = ~1;

		public const int DATA = 0;

		public const int RCDATA = 1;

		public const int SCRIPT_DATA = 2;

		public const int RAWTEXT = 3;

		public const int SCRIPT_DATA_ESCAPED = 4;

		public const int ATTRIBUTE_VALUE_DOUBLE_QUOTED = 5;

		public const int ATTRIBUTE_VALUE_SINGLE_QUOTED = 6;

		public const int ATTRIBUTE_VALUE_UNQUOTED = 7;

		public const int PLAINTEXT = 8;

		public const int TAG_OPEN = 9;

		public const int CLOSE_TAG_OPEN = 10;

		public const int TAG_NAME = 11;

		public const int BEFORE_ATTRIBUTE_NAME = 12;

		public const int ATTRIBUTE_NAME = 13;

		public const int AFTER_ATTRIBUTE_NAME = 14;

		public const int BEFORE_ATTRIBUTE_VALUE = 15;

		public const int AFTER_ATTRIBUTE_VALUE_QUOTED = 16;

		public const int BOGUS_COMMENT = 17;

		public const int MARKUP_DECLARATION_OPEN = 18;

		public const int DOCTYPE = 19;

		public const int BEFORE_DOCTYPE_NAME = 20;

		public const int DOCTYPE_NAME = 21;

		public const int AFTER_DOCTYPE_NAME = 22;

		public const int BEFORE_DOCTYPE_PUBLIC_IDENTIFIER = 23;

		public const int DOCTYPE_PUBLIC_IDENTIFIER_DOUBLE_QUOTED = 24;

		public const int DOCTYPE_PUBLIC_IDENTIFIER_SINGLE_QUOTED = 25;

		public const int AFTER_DOCTYPE_PUBLIC_IDENTIFIER = 26;

		public const int BEFORE_DOCTYPE_SYSTEM_IDENTIFIER = 27;

		public const int DOCTYPE_SYSTEM_IDENTIFIER_DOUBLE_QUOTED = 28;

		public const int DOCTYPE_SYSTEM_IDENTIFIER_SINGLE_QUOTED = 29;

		public const int AFTER_DOCTYPE_SYSTEM_IDENTIFIER = 30;

		public const int BOGUS_DOCTYPE = 31;

		public const int COMMENT_START = 32;

		public const int COMMENT_START_DASH = 33;

		public const int COMMENT = 34;

		public const int COMMENT_END_DASH = 35;

		public const int COMMENT_END = 36;

		public const int COMMENT_END_BANG = 37;

		public const int NON_DATA_END_TAG_NAME = 38;

		public const int MARKUP_DECLARATION_HYPHEN = 39;

		public const int MARKUP_DECLARATION_OCTYPE = 40;

		public const int DOCTYPE_UBLIC = 41;

		public const int DOCTYPE_YSTEM = 42;

		public const int AFTER_DOCTYPE_PUBLIC_KEYWORD = 43;

		public const int BETWEEN_DOCTYPE_PUBLIC_AND_SYSTEM_IDENTIFIERS = 44;

		public const int AFTER_DOCTYPE_SYSTEM_KEYWORD = 45;

		public const int CONSUME_CHARACTER_REFERENCE = 46;

		public const int CONSUME_NCR = 47;

		public const int CHARACTER_REFERENCE_TAIL = 48;

		public const int HEX_NCR_LOOP = 49;

		public const int DECIMAL_NRC_LOOP = 50;

		public const int HANDLE_NCR_VALUE = 51;

		public const int HANDLE_NCR_VALUE_RECONSUME = 52;

		public const int CHARACTER_REFERENCE_HILO_LOOKUP = 53;

		public const int SELF_CLOSING_START_TAG = 54;

		public const int CDATA_START = 55;

		public const int CDATA_SECTION = 56;

		public const int CDATA_RSQB = 57;

		public const int CDATA_RSQB_RSQB = 58;

		public const int SCRIPT_DATA_LESS_THAN_SIGN = 59;

		public const int SCRIPT_DATA_ESCAPE_START = 60;

		public const int SCRIPT_DATA_ESCAPE_START_DASH = 61;

		public const int SCRIPT_DATA_ESCAPED_DASH = 62;

		public const int SCRIPT_DATA_ESCAPED_DASH_DASH = 63;

		public const int BOGUS_COMMENT_HYPHEN = 64;

		public const int RAWTEXT_RCDATA_LESS_THAN_SIGN = 65;

		public const int SCRIPT_DATA_ESCAPED_LESS_THAN_SIGN = 66;

		public const int SCRIPT_DATA_DOUBLE_ESCAPE_START = 67;

		public const int SCRIPT_DATA_DOUBLE_ESCAPED = 68;

		public const int SCRIPT_DATA_DOUBLE_ESCAPED_LESS_THAN_SIGN = 69;

		public const int SCRIPT_DATA_DOUBLE_ESCAPED_DASH = 70;

		public const int SCRIPT_DATA_DOUBLE_ESCAPED_DASH_DASH = 71;

		public const int SCRIPT_DATA_DOUBLE_ESCAPE_END = 72;

		/// <summary>
		/// Magic value for UTF-16 operations.
		/// </summary>
		private const int LEAD_OFFSET = (0xD800 - (0x10000 >> 10));

		/// <summary>
		/// UTF-16 code unit array containing less than and greater than for emitting
		/// those characters on certain parse errors.
		/// </summary>
		private static readonly char[] LT_GT = { '<', '>' };

		/// <summary>
		/// UTF-16 code unit array containing less than and solidus for emitting
		/// those characters on certain parse errors.
		/// </summary>
		private static readonly char[] LT_SOLIDUS = { '<', '/' };

		/// <summary>
		/// UTF-16 code unit array containing ]] for emitting those characters on
		/// state transitions.
		/// </summary>
		private static readonly char[] RSQB_RSQB = { ']', ']' };

		/// <summary>
		/// Array version of U+FFFD.
		/// </summary>
		private static readonly char[] REPLACEMENT_CHARACTER = { '\uFFFD' };

		// [NOCPP[

		/// <summary>
		/// Array version of space.
		/// </summary>
		private static readonly char[] SPACE = { ' ' };

		// ]NOCPP]

		/// <summary>
		/// Array version of line feed.
		/// </summary>
		private static readonly char[] LF = { '\n' };

		/// <summary>
		/// Buffer growth parameter.
		/// </summary>
		private const int BUFFER_GROW_BY = 1024;

		/// <summary>
		/// "CDATA[" as <code>char[]</code>
		/// </summary>
		private static readonly char[] CDATA_LSQB = "CDATA[".ToCharArray();

		/// <summary>
		/// "octype" as <code>char[]</code>
		/// </summary>
		private static readonly char[] OCTYPE = "octype".ToCharArray();

		/// <summary>
		/// "ublic" as <code>char[]</code>
		/// </summary>
		private static readonly char[] UBLIC = "ublic".ToCharArray();

		/// <summary>
		/// "ystem" as  <code>char[]</code>
		/// </summary>
		private static readonly char[] YSTEM = "ystem".ToCharArray();

		private static readonly char[] TITLE_ARR = { 't', 'i', 't', 'l', 'e' };

		private static readonly char[] SCRIPT_ARR = { 's', 'c', 'r', 'i', 'p', 't' };

		private static readonly char[] STYLE_ARR = { 's', 't', 'y', 'l', 'e' };

		private static readonly char[] PLAINTEXT_ARR = { 'p', 'l', 'a', 'i', 'n', 't',
				'e', 'x', 't' };

		private static readonly char[] XMP_ARR = { 'x', 'm', 'p' };

		private static readonly char[] TEXTAREA_ARR = { 't', 'e', 'x', 't', 'a', 'r',
				'e', 'a' };

		private static readonly char[] IFRAME_ARR = { 'i', 'f', 'r', 'a', 'm', 'e' };

		private static readonly char[] NOEMBED_ARR = { 'n', 'o', 'e', 'm', 'b', 'e',
				'd' };

		private static readonly char[] NOSCRIPT_ARR = { 'n', 'o', 's', 'c', 'r', 'i',
				'p', 't' };

		private static readonly char[] NOFRAMES_ARR = { 'n', 'o', 'f', 'r', 'a', 'm',
				'e', 's' };

		public ITokenHandler TokenHandler { get; private set; }

		//public IEncodingDeclarationHandler EncodingDeclarationHandler { get; set; }
		public event EventHandler<EncodingDetectedEventArgs> EncodingDeclared;

		// [NOCPP[

		public event EventHandler<ParserErrorEventArgs> ErrorEvent;

		// ]NOCPP]

		/**
		 * Whether the previous char read was CR.
		 */
		protected bool lastCR;

		protected int stateSave;

		private int returnStateSave;

		protected int index;

		private bool forceQuirks;

		private char additional;

		private int entCol;

		private int firstCharKey;

		private int lo;

		private int hi;

		private int candidate;

		private int strBufMark;

		private int prevValue;

		protected int value;

		private bool seenDigits;

		protected int cstart;

		/**
		 * Buffer for short identifiers.
		 */
		private char[] strBuf;

		/**
		 * Number of significant <code>char</code>s in <code>strBuf</code>.
		 */
		private int strBufLen;

		/**
		 * <code>-1</code> to indicate that <code>strBuf</code> is used or otherwise
		 * an offset to the main buffer.
		 */
		// private int strBufOffset = -1;
		/**
		 * Buffer for long strings.
		 */
		private char[] longStrBuf;

		/**
		 * Number of significant <code>char</code>s in <code>longStrBuf</code>.
		 */
		private int longStrBufLen;

		/**
		 * <code>-1</code> to indicate that <code>longStrBuf</code> is used or
		 * otherwise an offset to the main buffer.
		 */
		// private int longStrBufOffset = -1;

		/**
		 * Buffer for expanding NCRs falling into the Basic Multilingual Plane.
		 */
		private readonly char[] bmpChar;

		/**
		 * Buffer for expanding astral NCRs.
		 */
		private readonly char[] astralChar;

		/**
		 * The element whose end tag closes the current CDATA or RCDATA element.
		 */
		protected ElementName endTagExpectation = null;

		private char[] endTagExpectationAsArray; // not @Auto!

		/**
		 * <code>true</code> if tokenizing an end tag
		 */
		protected bool endTag;

		/**
		 * The current tag token name.
		 */
		private ElementName tagName = null;

		/**
		 * The current attribute name.
		 */
		protected AttributeName attributeName = null;

		// [NOCPP[

		/**
		 * Whether comment tokens are emitted.
		 */
		private bool wantsComments = false;

		/**
		 * <code>true</code> when HTML4-specific additional errors are requested.
		 */
		protected bool html4;

		/**
		 * Whether the stream is past the first 512 bytes.
		 */
		private bool metaBoundaryPassed;

		// ]NOCPP]

		/**
		 * The name of the current doctype token.
		 */
		[Local]
		private string doctypeName;

		/**
		 * The public id of the current doctype token.
		 */
		private string publicIdentifier;

		/**
		 * The system id of the current doctype token.
		 */
		private string systemIdentifier;

		/**
		 * The attribute holder.
		 */
		private HtmlAttributes attributes;

		// [NOCPP[

		/**
		 * The policy for vertical tab and form feed.
		 */
		private XmlViolationPolicy contentSpacePolicy = XmlViolationPolicy.AlterInfoset;

		/**
		 * The policy for comments.
		 */
		private XmlViolationPolicy commentPolicy = XmlViolationPolicy.AlterInfoset;

		private XmlViolationPolicy xmlnsPolicy = XmlViolationPolicy.AlterInfoset;

		private XmlViolationPolicy namePolicy = XmlViolationPolicy.AlterInfoset;

		private bool html4ModeCompatibleWithXhtml1Schemata;

		private readonly bool newAttributesEachTime;

		// ]NOCPP]

		private int mappingLangToXmlLang;

		private bool shouldSuspend;

		protected bool confident;

		private int line;

		// [NOCPP[

		protected Locator ampersandLocation;

		public Tokenizer(ITokenHandler tokenHandler, bool newAttributesEachTime)
		{
			this.TokenHandler = tokenHandler;
			this.newAttributesEachTime = newAttributesEachTime;
			this.bmpChar = new char[1];
			this.astralChar = new char[2];
			this.tagName = null;
			this.attributeName = null;
			this.doctypeName = null;
			this.publicIdentifier = null;
			this.systemIdentifier = null;
			this.attributes = null;
		}

		// ]NOCPP]

		/**
		 * The constructor.
		 * 
		 * @param tokenHandler
		 *            the handler for receiving tokens
		 */
		public Tokenizer(ITokenHandler tokenHandler)
		{
			this.TokenHandler = tokenHandler;
			// [NOCPP[
			this.newAttributesEachTime = false;
			// ]NOCPP]
			this.bmpChar = new char[1];
			this.astralChar = new char[2];
			this.tagName = null;
			this.attributeName = null;
			this.doctypeName = null;
			this.publicIdentifier = null;
			this.systemIdentifier = null;
			this.attributes = null;
		}

		// [NOCPP[

		/**
		 * Returns the mappingLangToXmlLang.
		 * 
		 * @return the mappingLangToXmlLang
		 */
		public bool isMappingLangToXmlLang()
		{
			return mappingLangToXmlLang == AttributeName.HTML_LANG;
		}

		/**
		 * Sets the mappingLangToXmlLang.
		 * 
		 * @param mappingLangToXmlLang
		 *            the mappingLangToXmlLang to set
		 */
		public void setMappingLangToXmlLang(bool mappingLangToXmlLang)
		{
			this.mappingLangToXmlLang = mappingLangToXmlLang ? AttributeName.HTML_LANG
					: AttributeName.HTML;
		}


		/**
		 * Sets the commentPolicy.
		 * 
		 * @param commentPolicy
		 *            the commentPolicy to set
		 */
		public void setCommentPolicy(XmlViolationPolicy commentPolicy)
		{
			this.commentPolicy = commentPolicy;
		}

		/**
		 * Sets the contentNonXmlCharPolicy.
		 * 
		 * @param contentNonXmlCharPolicy
		 *            the contentNonXmlCharPolicy to set
		 */
		public void setContentNonXmlCharPolicy(XmlViolationPolicy contentNonXmlCharPolicy)
		{
			if (contentNonXmlCharPolicy != XmlViolationPolicy.Allow)
			{
				throw new ArgumentException("Must use ErrorReportingTokenizer to set contentNonXmlCharPolicy to non-ALLOW.");
			}
		}

		/**
		 * Sets the contentSpacePolicy.
		 * 
		 * @param contentSpacePolicy
		 *            the contentSpacePolicy to set
		 */
		public void setContentSpacePolicy(XmlViolationPolicy contentSpacePolicy)
		{
			this.contentSpacePolicy = contentSpacePolicy;
		}

		/**
		 * Sets the xmlnsPolicy.
		 * 
		 * @param xmlnsPolicy
		 *            the xmlnsPolicy to set
		 */
		public void setXmlnsPolicy(XmlViolationPolicy xmlnsPolicy)
		{
			if (xmlnsPolicy == XmlViolationPolicy.Fatal)
			{
				throw new ArgumentException("Can't use FATAL here.");
			}
			this.xmlnsPolicy = xmlnsPolicy;
		}

		public void setNamePolicy(XmlViolationPolicy namePolicy)
		{
			this.namePolicy = namePolicy;
		}

		/**
		 * Sets the html4ModeCompatibleWithXhtml1Schemata.
		 * 
		 * @param html4ModeCompatibleWithXhtml1Schemata
		 *            the html4ModeCompatibleWithXhtml1Schemata to set
		 */
		public void setHtml4ModeCompatibleWithXhtml1Schemata(
				bool html4ModeCompatibleWithXhtml1Schemata)
		{
			this.html4ModeCompatibleWithXhtml1Schemata = html4ModeCompatibleWithXhtml1Schemata;
		}

		// ]NOCPP]

		// For the token handler to call
		/**
		 * Sets the tokenizer state and the associated element name. This should 
		 * only ever used to put the tokenizer into one of the states that have
		 * a special end tag expectation.
		 * 
		 * @param specialTokenizerState
		 *            the tokenizer state to set
		 * @param endTagExpectation
		 *            the expected end tag for transitioning back to normal
		 */
		public void setStateAndEndTagExpectation(int specialTokenizerState,
				[Local] String endTagExpectation)
		{
			this.stateSave = specialTokenizerState;
			if (specialTokenizerState == Tokenizer.DATA)
			{
				return;
			}
			char[] asArray = Portability.NewCharArrayFromLocal(endTagExpectation);
			this.endTagExpectation = ElementName.ElementNameByBuffer(asArray, 0, asArray.Length);
			endTagExpectationToArray();
		}

		/**
		 * Sets the tokenizer state and the associated element name. This should 
		 * only ever used to put the tokenizer into one of the states that have
		 * a special end tag expectation.
		 * 
		 * @param specialTokenizerState
		 *            the tokenizer state to set
		 * @param endTagExpectation
		 *            the expected end tag for transitioning back to normal
		 */
		public void setStateAndEndTagExpectation(int specialTokenizerState,
				ElementName endTagExpectation)
		{
			this.stateSave = specialTokenizerState;
			this.endTagExpectation = endTagExpectation;
			endTagExpectationToArray();
		}

		private void endTagExpectationToArray()
		{
			switch (endTagExpectation.Group)
			{
				case TreeBuilderConstants.TITLE:
					endTagExpectationAsArray = TITLE_ARR;
					return;
				case TreeBuilderConstants.SCRIPT:
					endTagExpectationAsArray = SCRIPT_ARR;
					return;
				case TreeBuilderConstants.STYLE:
					endTagExpectationAsArray = STYLE_ARR;
					return;
				case TreeBuilderConstants.PLAINTEXT:
					endTagExpectationAsArray = PLAINTEXT_ARR;
					return;
				case TreeBuilderConstants.XMP:
					endTagExpectationAsArray = XMP_ARR;
					return;
				case TreeBuilderConstants.TEXTAREA:
					endTagExpectationAsArray = TEXTAREA_ARR;
					return;
				case TreeBuilderConstants.IFRAME:
					endTagExpectationAsArray = IFRAME_ARR;
					return;
				case TreeBuilderConstants.NOEMBED:
					endTagExpectationAsArray = NOEMBED_ARR;
					return;
				case TreeBuilderConstants.NOSCRIPT:
					endTagExpectationAsArray = NOSCRIPT_ARR;
					return;
				case TreeBuilderConstants.NOFRAMES:
					endTagExpectationAsArray = NOFRAMES_ARR;
					return;
				default:
					Debug.Assert(false, "Bad end tag expectation.");
					return;
			}
		}

		/**
		 * For C++ use only.
		 */
		public void setLineNumber(int line)
		{
			this.line = line;
		}

		#region Locator implementation

		/**
		 * @see org.xml.sax.Locator#getLineNumber()
		 */
		public int LineNumber
		{
			get
			{
				return line;
			}
		}

		// [NOCPP[

		/**
		 * @see org.xml.sax.Locator#getColumnNumber()
		 */
		public int ColumnNumber
		{
			get
			{
				return -1;
			}
		}

		#endregion // locator implementation

		// end of public API

		public void notifyAboutMetaBoundary()
		{
			metaBoundaryPassed = true;
		}

		internal void turnOnAdditionalHtml4Errors()
		{
			html4 = true;
		}

		// ]NOCPP]

		internal HtmlAttributes emptyAttributes()
		{
			// [NOCPP[
			if (newAttributesEachTime)
			{
				return new HtmlAttributes(mappingLangToXmlLang);
			}
			else
			{
				// ]NOCPP]
				return HtmlAttributes.EMPTY_ATTRIBUTES;
				// [NOCPP[
			}
			// ]NOCPP]
		}

		/*@Inline*/
		private void clearStrBufAndAppend(char c)
		{
			strBuf[0] = c;
			strBufLen = 1;
		}

		/*@Inline*/
		private void clearStrBuf()
		{
			strBufLen = 0;
		}

		/**
		 * Appends to the smaller buffer.
		 * 
		 * @param c
		 *            the UTF-16 code unit to append
		 */
		private void appendStrBuf(char c)
		{
			if (strBufLen == strBuf.Length)
			{
				char[] newBuf = new char[strBuf.Length + Tokenizer.BUFFER_GROW_BY];
				Array.Copy(strBuf, newBuf, strBuf.Length);
				strBuf = newBuf;
			}
			strBuf[strBufLen++] = c;
		}

		/**
		 * The smaller buffer as a String. Currently only used for error reporting.
		 * 
		 * <p>
		 * C++ memory note: The return value must be released.
		 * 
		 * @return the smaller buffer as a string
		 */
		protected String strBufToString()
		{
			return Portability.NewStringFromBuffer(strBuf, 0, strBufLen);
		}

		/**
		 * Returns the short buffer as a local name. The return value is released in
		 * emitDoctypeToken().
		 * 
		 * @return the smaller buffer as local name
		 */
		private void strBufToDoctypeName()
		{
			doctypeName = Portability.NewLocalNameFromBuffer(strBuf, 0, strBufLen);
		}

		/**
		 * Emits the smaller buffer as character tokens.
		 * 
		 * @throws SAXException
		 *             if the token handler threw
		 */
		private void emitStrBuf()
		{
			if (strBufLen > 0)
			{
				TokenHandler.Characters(strBuf, 0, strBufLen);
			}
		}

		/*@Inline*/
		private void clearLongStrBuf()
		{
			longStrBufLen = 0;
		}

		/*@Inline*/
		private void clearLongStrBufAndAppend(char c)
		{
			longStrBuf[0] = c;
			longStrBufLen = 1;
		}

		/**
		 * Appends to the larger buffer.
		 * 
		 * @param c
		 *            the UTF-16 code unit to append
		 */
		private void appendLongStrBuf(char c)
		{
			if (longStrBufLen == longStrBuf.Length)
			{
				char[] newBuf = new char[longStrBufLen + (longStrBufLen >> 1)];
				Array.Copy(longStrBuf, newBuf, longStrBuf.Length);
				longStrBuf = newBuf;
			}
			longStrBuf[longStrBufLen++] = c;
		}

		/*@Inline*/
		private void appendSecondHyphenToBogusComment()
		{
			// [NOCPP[
			switch (commentPolicy)
			{
				case XmlViolationPolicy.AlterInfoset:
					// detachLongStrBuf();
					appendLongStrBuf(' ');
					// FALLTHROUGH
					goto case XmlViolationPolicy.Allow;
				case XmlViolationPolicy.Allow:
					Warn("The document is not mappable to XML 1.0 due to two consecutive hyphens in a comment.");
					// ]NOCPP]
					appendLongStrBuf('-');
					// [NOCPP[
					break;
				case XmlViolationPolicy.Fatal:
					Fatal("The document is not mappable to XML 1.0 due to two consecutive hyphens in a comment.");
					break;
			}
			// ]NOCPP]
		}

		// [NOCPP[
		private void maybeAppendSpaceToBogusComment()
		{
			switch (commentPolicy)
			{
				case XmlViolationPolicy.AlterInfoset:
					// detachLongStrBuf();
					appendLongStrBuf(' ');
					// FALLTHROUGH
					goto case XmlViolationPolicy.Allow;
				case XmlViolationPolicy.Allow:
					Warn("The document is not mappable to XML 1.0 due to a trailing hyphen in a comment.");
					break;
				case XmlViolationPolicy.Fatal:
					Fatal("The document is not mappable to XML 1.0 due to a trailing hyphen in a comment.");
					break;
			}
		}

		// ]NOCPP]

		/*@Inline*/
		private void adjustDoubleHyphenAndAppendToLongStrBufAndErr(char c)
		{
			errConsecutiveHyphens();
			// [NOCPP[
			switch (commentPolicy)
			{
				case XmlViolationPolicy.AlterInfoset:
					// detachLongStrBuf();
					longStrBufLen--;
					appendLongStrBuf(' ');
					appendLongStrBuf('-');
					// FALLTHROUGH
					goto case XmlViolationPolicy.AlterInfoset;
				case XmlViolationPolicy.Allow:
					Warn("The document is not mappable to XML 1.0 due to two consecutive hyphens in a comment.");
					// ]NOCPP]
					appendLongStrBuf(c);
					// [NOCPP[
					break;
				case XmlViolationPolicy.Fatal:
					Fatal("The document is not mappable to XML 1.0 due to two consecutive hyphens in a comment.");
					break;
			}
			// ]NOCPP]
		}

		private void appendLongStrBuf(char[] buffer, int offset, int length)
		{
			int reqLen = longStrBufLen + length;
			if (longStrBuf.Length < reqLen)
			{
				char[] newBuf = new char[reqLen + (reqLen >> 1)];
				Array.Copy(longStrBuf, newBuf, longStrBuf.Length);
				longStrBuf = newBuf;
			}
			Array.Copy(buffer, offset, longStrBuf, longStrBufLen, length);
			longStrBufLen = reqLen;
		}

		/**
		 * Append the contents of the smaller buffer to the larger one.
		 */
		/*@Inline*/
		private void appendStrBufToLongStrBuf()
		{
			appendLongStrBuf(strBuf, 0, strBufLen);
		}

		/**
		 * The larger buffer as a string.
		 * 
		 * <p>
		 * C++ memory note: The return value must be released.
		 * 
		 * @return the larger buffer as a string
		 */
		private string LongStrBufToString()
		{
			return Portability.NewStringFromBuffer(longStrBuf, 0, longStrBufLen);
		}

		/// <summary>
		/// Emits the current comment token.
		/// </summary>
		/// <param name="provisionalHyphens">The provisional hyphens.</param>
		/// <param name="pos">The position.</param>
		private void EmitComment(int provisionalHyphens, int pos)
		{
			// [NOCPP[
			if (wantsComments)
			{
				// ]NOCPP]
				// if (longStrBufOffset != -1) {
				// tokenHandler.comment(buf, longStrBufOffset, longStrBufLen
				// - provisionalHyphens);
				// } else {
				TokenHandler.Comment(longStrBuf, 0, longStrBufLen - provisionalHyphens);
				// }
				// [NOCPP[
			}
			// ]NOCPP]
			cstart = pos + 1;
		}

		/// <summary>
		/// Flushes coalesced character tokens.
		/// </summary>
		/// <param name="buf">The buffer.</param>
		/// <param name="pos">The position.</param>
		protected void FlushChars(char[] buf, int pos)
		{
			if (pos > cstart)
			{
				TokenHandler.Characters(buf, cstart, pos - cstart);
			}
			cstart = int.MaxValue;
		}

		/**
		 * Reports an condition that would make the infoset incompatible with XML
		 * 1.0 as fatal.
		 * 
		 * @param message
		 *            the message
		 * @throws SAXException
		 * @throws SAXParseException
		 */
		public void Fatal(string message)
		{
			/*SAXParseException spe = new SAXParseException(message, this);
			if (errorHandler != null) {
				errorHandler.fatalError(spe);
			}
			throw spe;*/
			throw new Exception(message); // TODO
		}

		/**
		 * Reports a Parse Error.
		 * 
		 * @param message
		 *            the message
		 * @throws SAXException
		 */
		public void Err(string message)
		{
			if (ErrorEvent == null)
			{
				return;
			}
			ErrorEvent(this, new ParserErrorEventArgs(message, false));
		}

		public void ErrTreeBuilder(string message)
		{
			/*ErrorHandler eh = null;
			if (tokenHandler is TreeBuilder<T>) {
				TreeBuilder<?> treeBuilder = (TreeBuilder<?>) tokenHandler;
				eh = treeBuilder.getErrorHandler();
			}
			if (eh == null) {
				eh = errorHandler;
			}
			if (eh == null) {
				return;
			}
			SAXParseException spe = new SAXParseException(message, this);
			eh.error(spe);*/
			Err(message); // TODO
		}

		/**
		 * Reports a warning
		 * 
		 * @param message
		 *            the message
		 * @throws SAXException
		 */
		public void Warn(string message)
		{
			if (ErrorEvent == null)
			{
				return;
			}
			ErrorEvent(this, new ParserErrorEventArgs(message, true));
		}

		/**
		 * 
		 */
		private void resetAttributes()
		{
			// [NOCPP[
			if (newAttributesEachTime)
			{
				// ]NOCPP]
				attributes = null;
				// [NOCPP[
			}
			else
			{
				attributes.Clear(mappingLangToXmlLang);
			}
			// ]NOCPP]
		}

		private void strBufToElementNameString()
		{
			// if (strBufOffset != -1) {
			// return ElementName.elementNameByBuffer(buf, strBufOffset, strBufLen);
			// } else {
			tagName = ElementName.ElementNameByBuffer(strBuf, 0, strBufLen);
			// }
		}

		private int emitCurrentTagToken(bool selfClosing, int pos)
		{
			cstart = pos + 1;
			maybeErrSlashInEndTag(selfClosing);
			stateSave = Tokenizer.DATA;
			HtmlAttributes attrs = (attributes == null ? HtmlAttributes.EMPTY_ATTRIBUTES
					: attributes);
			if (endTag)
			{
				/*
				 * When an end tag token is emitted, the content model flag must be
				 * switched to the PCDATA state.
				 */
				maybeErrAttributesOnEndTag(attrs);
				TokenHandler.EndTag(tagName);
			}
			else
			{
				TokenHandler.StartTag(tagName, attrs, selfClosing);
			}
			tagName = null;
			resetAttributes();
			/*
			 * The token handler may have called setStateAndEndTagExpectation
			 * and changed stateSave since the start of this method.
			 */
			return stateSave;
		}

		private void attributeNameComplete()
		{
			// if (strBufOffset != -1) {
			// attributeName = AttributeName.nameByBuffer(buf, strBufOffset,
			// strBufLen, namePolicy != XmlViolationPolicy.ALLOW);
			// } else {
			attributeName = AttributeName.NameByBuffer(strBuf, 0, strBufLen
				// [NOCPP[
					, namePolicy != XmlViolationPolicy.Allow
				// ]NOCPP]
					);
			// }

			if (attributes == null)
			{
				attributes = new HtmlAttributes(mappingLangToXmlLang);
			}

			/*
			 * When the user agent leaves the attribute name state (and before
			 * emitting the tag token, if appropriate), the complete attribute's
			 * name must be compared to the other attributes on the same token; if
			 * there is already an attribute on the token with the exact same name,
			 * then this is a parse error and the new attribute must be dropped,
			 * along with the value that gets associated with it (if any).
			 */
			if (attributes.Contains(attributeName))
			{
				errDuplicateAttribute();
				attributeName = null;
			}
		}

		private void addAttributeWithoutValue()
		{
			noteAttributeWithoutValue();

			// [NOCPP[
			if (metaBoundaryPassed && AttributeName.CHARSET == attributeName
					&& ElementName.META == tagName)
			{
				Err("A \u201Ccharset\u201D attribute on a \u201Cmeta\u201D element found after the first 512 bytes.");
			}
			// ]NOCPP]
			if (attributeName != null)
			{
				// [NOCPP[
				if (html4)
				{
					if (attributeName.IsBoolean())
					{
						if (html4ModeCompatibleWithXhtml1Schemata)
						{
							attributes.AddAttribute(attributeName,
									attributeName.GetLocal(AttributeName.HTML),
									xmlnsPolicy);
						}
						else
						{
							attributes.AddAttribute(attributeName, "", xmlnsPolicy);
						}
					}
					else
					{
						Err("Attribute value omitted for a non-bool attribute. (HTML4-only error.)");
						attributes.AddAttribute(attributeName, "", xmlnsPolicy);
					}
				}
				else
				{
					if (AttributeName.SRC == attributeName
							|| AttributeName.HREF == attributeName)
					{
						Warn("Attribute \u201C"
								+ attributeName.GetLocal(AttributeName.HTML)
								+ "\u201D without an explicit value seen. The attribute may be dropped by IE7.");
					}
					// ]NOCPP]
					attributes.AddAttribute(attributeName,
							Portability.NewEmptyString()
						// [NOCPP[
							, xmlnsPolicy
						// ]NOCPP]
					);
					// [NOCPP[
				}
				// ]NOCPP]
				attributeName = null; // attributeName has been adopted by the
				// |attributes| object
			}
		}

		private void addAttributeWithValue()
		{
			// [NOCPP[
			if (metaBoundaryPassed && ElementName.META == tagName
					&& AttributeName.CHARSET == attributeName)
			{
				Err("A \u201Ccharset\u201D attribute on a \u201Cmeta\u201D element found after the first 512 bytes.");
			}
			// ]NOCPP]
			if (attributeName != null)
			{
				String val = LongStrBufToString(); // Ownership transferred to
				// HtmlAttributes
				// [NOCPP[
				if (!endTag && html4 && html4ModeCompatibleWithXhtml1Schemata
						&& attributeName.IsCaseFolded())
				{
					val = newAsciiLowerCaseStringFromString(val);
				}
				// ]NOCPP]
				attributes.AddAttribute(attributeName, val
					// [NOCPP[
						, xmlnsPolicy
					// ]NOCPP]
				);
				attributeName = null; // attributeName has been adopted by the
				// |attributes| object
			}
		}

		// [NOCPP[

		private static String newAsciiLowerCaseStringFromString(String str)
		{
			if (str == null)
			{
				return null;
			}
			char[] buf = new char[str.Length];
			for (int i = 0; i < str.Length; i++)
			{
				char c = str[i];
				if (c >= 'A' && c <= 'Z')
				{
					c += (char)0x20;
				}
				buf[i] = c;
			}
			return new String(buf);
		}

		protected void startErrorReporting()
		{

		}

		// ]NOCPP]

		public void start()
		{
			initializeWithoutStarting();
			TokenHandler.StartTokenization(this);
			// [NOCPP[
			startErrorReporting();
			// ]NOCPP]
		}

		public bool tokenizeBuffer(UTF16Buffer buffer)
		{
			int state = stateSave;
			int returnState = returnStateSave;
			char c = '\u0000';
			shouldSuspend = false;
			lastCR = false;

			int start = buffer.Start;
			/**
			 * The index of the last <code>char</code> read from <code>buf</code>.
			 */
			int pos = start - 1;

			/**
			 * The index of the first <code>char</code> in <code>buf</code> that is
			 * part of a coalesced run of character tokens or
			 * <code>Integer.MAX_VALUE</code> if there is not a current run being
			 * coalesced.
			 */
			switch (state)
			{
				case DATA:
				case RCDATA:
				case SCRIPT_DATA:
				case PLAINTEXT:
				case RAWTEXT:
				case CDATA_SECTION:
				case SCRIPT_DATA_ESCAPED:
				case SCRIPT_DATA_ESCAPE_START:
				case SCRIPT_DATA_ESCAPE_START_DASH:
				case SCRIPT_DATA_ESCAPED_DASH:
				case SCRIPT_DATA_ESCAPED_DASH_DASH:
				case SCRIPT_DATA_DOUBLE_ESCAPE_START:
				case SCRIPT_DATA_DOUBLE_ESCAPED:
				case SCRIPT_DATA_DOUBLE_ESCAPED_LESS_THAN_SIGN:
				case SCRIPT_DATA_DOUBLE_ESCAPED_DASH:
				case SCRIPT_DATA_DOUBLE_ESCAPED_DASH_DASH:
				case SCRIPT_DATA_DOUBLE_ESCAPE_END:
					cstart = start;
					break;
				default:
					cstart = int.MaxValue;
					break;
			}

			/**
			 * The number of <code>char</code>s in <code>buf</code> that have
			 * meaning. (The rest of the array is garbage and should not be
			 * examined.)
			 */
			pos = StateLoop(state, c, pos, buffer.Buffer, false, returnState, buffer.End);
			if (pos == buffer.End)
			{
				// exiting due to end of buffer
				buffer.Start = pos;
			}
			else
			{
				buffer.Start = pos + 1;
			}
			return lastCR;
		}

		/// <summary>
		/// States the loop.
		/// </summary>
		/// <param name="state">The state.</param>
		/// <param name="c">The c.</param>
		/// <param name="pos">The pos.</param>
		/// <param name="buf">The buf.</param>
		/// <param name="reconsume">if set to <c>true</c> [reconsume].</param>
		/// <param name="returnState">State of the return.</param>
		/// <param name="endPos">The end pos.</param>
		/// <returns></returns>
		private int StateLoop(int state, char c,
				int pos, char[] buf, bool reconsume, int returnState,
				int endPos)
		{
			/*
			 * Idioms used in this code:
			 * 
			 * 
			 * Consuming the next input character
			 * 
			 * To consume the next input character, the code does this: if (++pos ==
			 * endPos) { goto breakStateloop; } c = checkChar(buf, pos);
			 * 
			 * 
			 * Staying in a state
			 * 
			 * When there's a state that the tokenizer may stay in over multiple
			 * input characters, the state has a wrapper |for(;;)| loop and staying
			 * in the state continues the loop.
			 * 
			 * 
			 * Switching to another state
			 * 
			 * To switch to another state, the code sets the state variable to the
			 * magic number of the new state. Then it either continues stateloop or
			 * breaks out of the state's own wrapper loop if the target state is
			 * right after the current state in source order. (This is a partial
			 * workaround for Java's lack of goto.)
			 * 
			 * 
			 * Reconsume support
			 * 
			 * The spec sometimes says that an input character is reconsumed in
			 * another state. If a state can ever be entered so that an input
			 * character can be reconsumed in it, the state's code starts with an
			 * |if (reconsume)| that sets reconsume to false and skips over the
			 * normal code for consuming a new character.
			 * 
			 * To reconsume the current character in another state, the code sets
			 * |reconsume| to true and then switches to the other state.
			 * 
			 * 
			 * Emitting character tokens
			 * 
			 * This method emits character tokens lazily. Whenever a new range of
			 * character tokens starts, the field cstart must be set to the start
			 * index of the range. The flushChars() method must be called at the end
			 * of a range to flush it.
			 * 
			 * 
			 * U+0000 handling
			 * 
			 * The various states have to handle the replacement of U+0000 with
			 * U+FFFD. However, if U+0000 would be reconsumed in another state, the
			 * replacement doesn't need to happen, because it's handled by the
			 * reconsuming state.
			 * 
			 * 
			 * LF handling
			 * 
			 * Every state needs to increment the line number upon LF unless the LF
			 * gets reconsumed by another state which increments the line number.
			 * 
			 * 
			 * CR handling
			 * 
			 * Every state needs to handle CR unless the CR gets reconsumed and is
			 * handled by the reconsuming state. The CR needs to be handled as if it
			 * were and LF, the lastCR field must be set to true and then this
			 * method must return. The IO driver will then swallow the next
			 * character if it is an LF to coalesce CRLF.
			 */
			/*stateloop:*/
			for (; ; )
			{
			continueStateloop:

				switch (state)
				{
					case DATA:
						/*dataloop:*/
						for (; ; )
						{
							if (reconsume)
							{
								reconsume = false;
							}
							else
							{
								if (++pos == endPos)
								{
									goto breakStateloop;
								}
								c = checkChar(buf, pos);
							}
							switch (c)
							{
								case '&':
									/*
									 * U+0026 AMPERSAND (&) Switch to the character
									 * reference in data state.
									 */
									FlushChars(buf, pos);
									clearStrBufAndAppend(c);
									setAdditionalAndRememberAmpersandLocation('\u0000');
									returnState = state;
									state = transition(state, Tokenizer.CONSUME_CHARACTER_REFERENCE, reconsume, pos);
									goto continueStateloop;
								case '<':
									/*
									 * U+003C LESS-THAN SIGN (<) Switch to the tag
									 * open state.
									 */
									FlushChars(buf, pos);

									state = transition(state, Tokenizer.TAG_OPEN, reconsume, pos);
									goto breakDataloop; // FALL THROUGH continue
								// stateloop;
								case '\u0000':
									emitReplacementCharacter(buf, pos);
									continue;
								case '\r':
									emitCarriageReturn(buf, pos);
									goto breakStateloop;
								case '\n':
									silentLineFeed();
									goto default;
								default:
									/*
									 * Anything else Emit the input character as a
									 * character token.
									 * 
									 * Stay in the data state.
									 */
									continue;
							}
						}
					breakDataloop:
						goto case TAG_OPEN;
					// WARNING FALLTHRU CASE TRANSITION: DON'T REORDER
					case TAG_OPEN:
						/*tagopenloop:*/
						for (; ; )
						{
							/*
							 * The behavior of this state depends on the content
							 * model flag.
							 */
							if (++pos == endPos)
							{
								goto breakStateloop;
							}
							c = checkChar(buf, pos);
							/*
							 * If the content model flag is set to the PCDATA state
							 * Consume the next input character:
							 */
							if (c >= 'A' && c <= 'Z')
							{
								/*
								 * U+0041 LATIN CAPITAL LETTER A through to U+005A
								 * LATIN CAPITAL LETTER Z Create a new start tag
								 * token,
								 */
								endTag = false;
								/*
								 * set its tag name to the lowercase version of the
								 * input character (add 0x0020 to the character's
								 * code point),
								 */
								clearStrBufAndAppend((char)(c + 0x20));
								/* then switch to the tag name state. */
								state = transition(state, Tokenizer.TAG_NAME, reconsume, pos);
								/*
								 * (Don't emit the token yet; further details will
								 * be filled in before it is emitted.)
								 */
								goto breakTagopenloop;
								// goto continueStateloop;
							}
							else if (c >= 'a' && c <= 'z')
							{
								/*
								 * U+0061 LATIN SMALL LETTER A through to U+007A
								 * LATIN SMALL LETTER Z Create a new start tag
								 * token,
								 */
								endTag = false;
								/*
								 * set its tag name to the input character,
								 */
								clearStrBufAndAppend(c);
								/* then switch to the tag name state. */
								state = transition(state, Tokenizer.TAG_NAME, reconsume, pos);
								/*
								 * (Don't emit the token yet; further details will
								 * be filled in before it is emitted.)
								 */
								goto breakTagopenloop;
								// goto continueStateloop;
							}
							switch (c)
							{
								case '!':
									/*
									 * U+0021 EXCLAMATION MARK (!) Switch to the
									 * markup declaration open state.
									 */
									state = transition(state, Tokenizer.MARKUP_DECLARATION_OPEN, reconsume, pos);
									goto continueStateloop;
								case '/':
									/*
									 * U+002F SOLIDUS (/) Switch to the close tag
									 * open state.
									 */
									state = transition(state, Tokenizer.CLOSE_TAG_OPEN, reconsume, pos);
									goto continueStateloop;
								case '?':
									/*
									 * U+003F QUESTION MARK (?) Parse error.
									 */
									errProcessingInstruction();
									/*
									 * Switch to the bogus comment state.
									 */
									clearLongStrBufAndAppend(c);
									state = transition(state, Tokenizer.BOGUS_COMMENT, reconsume, pos);
									goto continueStateloop;
								case '>':
									/*
									 * U+003E GREATER-THAN SIGN (>) Parse error.
									 */
									errLtGt();
									/*
									 * Emit a U+003C LESS-THAN SIGN character token
									 * and a U+003E GREATER-THAN SIGN character
									 * token.
									 */
									TokenHandler.Characters(Tokenizer.LT_GT, 0, 2);
									/* Switch to the data state. */
									cstart = pos + 1;
									state = transition(state, Tokenizer.DATA, reconsume, pos);
									goto continueStateloop;
								default:
									/*
									 * Anything else Parse error.
									 */
									errBadCharAfterLt(c);
									/*
									 * Emit a U+003C LESS-THAN SIGN character token
									 */
									TokenHandler.Characters(Tokenizer.LT_GT, 0, 1);
									/*
									 * and reconsume the current input character in
									 * the data state.
									 */
									cstart = pos;
									state = transition(state, Tokenizer.DATA, reconsume, pos);
									reconsume = true;
									goto continueStateloop;
							}
						}
					breakTagopenloop:
						goto case TAG_NAME;
					// FALL THROUGH DON'T REORDER
					case TAG_NAME:
						/*tagnameloop:*/
						for (; ; )
						{
							if (++pos == endPos)
							{
								goto breakStateloop;
							}
							c = checkChar(buf, pos);
							/*
							 * Consume the next input character:
							 */
							switch (c)
							{
								case '\r':
									silentCarriageReturn();
									strBufToElementNameString();
									state = transition(state, Tokenizer.BEFORE_ATTRIBUTE_NAME, reconsume, pos);
									goto breakStateloop;
								case '\n':
									silentLineFeed();
									goto case ' ';
								case ' ':
								case '\t':
								case '\u000C':
									/*
									 * U+0009 CHARACTER TABULATION U+000A LINE FEED
									 * (LF) U+000C FORM FEED (FF) U+0020 SPACE
									 * Switch to the before attribute name state.
									 */
									strBufToElementNameString();
									state = transition(state, Tokenizer.BEFORE_ATTRIBUTE_NAME, reconsume, pos);
									goto breakTagnameloop;
								// goto continueStateloop;
								case '/':
									/*
									 * U+002F SOLIDUS (/) Switch to the self-closing
									 * start tag state.
									 */
									strBufToElementNameString();
									state = transition(state, Tokenizer.SELF_CLOSING_START_TAG, reconsume, pos);
									goto continueStateloop;
								case '>':
									/*
									 * U+003E GREATER-THAN SIGN (>) Emit the current
									 * tag token.
									 */
									strBufToElementNameString();
									state = transition(state, emitCurrentTagToken(false, pos), reconsume, pos);
									if (shouldSuspend)
									{
										goto breakStateloop;
									}
									/*
									 * Switch to the data state.
									 */
									goto continueStateloop;
								case '\u0000':
									c = '\uFFFD';
									goto default;
								// fall thru
								default:
									if (c >= 'A' && c <= 'Z')
									{
										/*
										 * U+0041 LATIN CAPITAL LETTER A through to
										 * U+005A LATIN CAPITAL LETTER Z Append the
										 * lowercase version of the current input
										 * character (add 0x0020 to the character's
										 * code point) to the current tag token's
										 * tag name.
										 */
										c += (char)0x20;
									}
									/*
									 * Anything else Append the current input
									 * character to the current tag token's tag
									 * name.
									 */
									appendStrBuf(c);
									/*
									 * Stay in the tag name state.
									 */
									continue;
							}
						}
					breakTagnameloop:
						goto case BEFORE_ATTRIBUTE_NAME;
					// FALLTHRU DON'T REORDER
					case BEFORE_ATTRIBUTE_NAME:
						/*beforeattributenameloop:*/
						for (; ; )
						{
							if (reconsume)
							{
								reconsume = false;
							}
							else
							{
								if (++pos == endPos)
								{
									goto breakStateloop;
								}
								c = checkChar(buf, pos);
							}
							/*
							 * Consume the next input character:
							 */
							switch (c)
							{
								case '\r':
									silentCarriageReturn();
									goto breakStateloop;
								case '\n':
									silentLineFeed();
									// fall thru
									goto case ' ';
								case ' ':
								case '\t':
								case '\u000C':
									/*
									 * U+0009 CHARACTER TABULATION U+000A LINE FEED
									 * (LF) U+000C FORM FEED (FF) U+0020 SPACE Stay
									 * in the before attribute name state.
									 */
									continue;
								case '/':
									/*
									 * U+002F SOLIDUS (/) Switch to the self-closing
									 * start tag state.
									 */
									state = transition(state, Tokenizer.SELF_CLOSING_START_TAG, reconsume, pos);
									goto continueStateloop;
								case '>':
									/*
									 * U+003E GREATER-THAN SIGN (>) Emit the current
									 * tag token.
									 */
									state = transition(state, emitCurrentTagToken(false, pos), reconsume, pos);
									if (shouldSuspend)
									{
										goto breakStateloop;
									}
									/*
									 * Switch to the data state.
									 */
									goto continueStateloop;
								case '\u0000':
									c = '\uFFFD';
									// fall thru
									goto case '\"';
								case '\"':
								case '\'':
								case '<':
								case '=':
									/*
									 * U+0022 QUOTATION MARK (") U+0027 APOSTROPHE
									 * (') U+003C LESS-THAN SIGN (<) U+003D EQUALS
									 * SIGN (=) Parse error.
									 */
									errBadCharBeforeAttributeNameOrNull(c);
									/*
									 * Treat it as per the "anything else" entry
									 * below.
									 */
									goto default;
								default:
									/*
									 * Anything else Start a new attribute in the
									 * current tag token.
									 */
									if (c >= 'A' && c <= 'Z')
									{
										/*
										 * U+0041 LATIN CAPITAL LETTER A through to
										 * U+005A LATIN CAPITAL LETTER Z Set that
										 * attribute's name to the lowercase version
										 * of the current input character (add
										 * 0x0020 to the character's code point)
										 */
										c += (char)0x20;
									}
									/*
									 * Set that attribute's name to the current
									 * input character,
									 */
									clearStrBufAndAppend(c);
									/*
									 * and its value to the empty string.
									 */
									// Will do later.
									/*
									 * Switch to the attribute name state.
									 */
									state = transition(state, Tokenizer.ATTRIBUTE_NAME, reconsume, pos);
									goto breakBeforeattributenameloop;
								// goto continueStateloop;
							}
						}
					breakBeforeattributenameloop:
						goto case ATTRIBUTE_NAME;
					// FALLTHRU DON'T REORDER
					case ATTRIBUTE_NAME:
						/*attributenameloop:*/
						for (; ; )
						{
							if (++pos == endPos)
							{
								goto breakStateloop;
							}
							c = checkChar(buf, pos);
							/*
							 * Consume the next input character:
							 */
							switch (c)
							{
								case '\r':
									silentCarriageReturn();
									attributeNameComplete();
									state = transition(state, Tokenizer.AFTER_ATTRIBUTE_NAME, reconsume, pos);
									goto breakStateloop;
								case '\n':
									silentLineFeed();
									goto case ' ';
								// fall thru
								case ' ':
								case '\t':
								case '\u000C':
									/*
									 * U+0009 CHARACTER TABULATION U+000A LINE FEED
									 * (LF) U+000C FORM FEED (FF) U+0020 SPACE
									 * Switch to the after attribute name state.
									 */
									attributeNameComplete();
									state = transition(state, Tokenizer.AFTER_ATTRIBUTE_NAME, reconsume, pos);
									goto continueStateloop;
								case '/':
									/*
									 * U+002F SOLIDUS (/) Switch to the self-closing
									 * start tag state.
									 */
									attributeNameComplete();
									addAttributeWithoutValue();
									state = transition(state, Tokenizer.SELF_CLOSING_START_TAG, reconsume, pos);
									goto continueStateloop;
								case '=':
									/*
									 * U+003D EQUALS SIGN (=) Switch to the before
									 * attribute value state.
									 */
									attributeNameComplete();
									state = transition(state, Tokenizer.BEFORE_ATTRIBUTE_VALUE, reconsume, pos);
									goto breakAttributenameloop;
								// goto continueStateloop;
								case '>':
									/*
									 * U+003E GREATER-THAN SIGN (>) Emit the current
									 * tag token.
									 */
									attributeNameComplete();
									addAttributeWithoutValue();
									state = transition(state, emitCurrentTagToken(false, pos), reconsume, pos);
									if (shouldSuspend)
									{
										goto breakStateloop;
									}
									/*
									 * Switch to the data state.
									 */
									goto continueStateloop;
								case '\u0000':
									c = '\uFFFD';
									// fall thru
									goto case '\"';
								case '\"':
								case '\'':
								case '<':
									/*
									 * U+0022 QUOTATION MARK (") U+0027 APOSTROPHE
									 * (') U+003C LESS-THAN SIGN (<) Parse error.
									 */
									errQuoteOrLtInAttributeNameOrNull(c);
									/*
									 * Treat it as per the "anything else" entry
									 * below.
									 */
									goto default;
								default:
									if (c >= 'A' && c <= 'Z')
									{
										/*
										 * U+0041 LATIN CAPITAL LETTER A through to
										 * U+005A LATIN CAPITAL LETTER Z Append the
										 * lowercase version of the current input
										 * character (add 0x0020 to the character's
										 * code point) to the current attribute's
										 * name.
										 */
										c += (char)0x20;
									}
									/*
									 * Anything else Append the current input
									 * character to the current attribute's name.
									 */
									appendStrBuf(c);
									/*
									 * Stay in the attribute name state.
									 */
									continue;
							}
						}
					breakAttributenameloop:
						goto case BEFORE_ATTRIBUTE_VALUE;
					// FALLTHRU DON'T REORDER
					case BEFORE_ATTRIBUTE_VALUE:
						/*beforeattributevalueloop:*/
						for (; ; )
						{
							if (++pos == endPos)
							{
								goto breakStateloop;
							}
							c = checkChar(buf, pos);
							/*
							 * Consume the next input character:
							 */
							switch (c)
							{
								case '\r':
									silentCarriageReturn();
									goto breakStateloop;
								case '\n':
									silentLineFeed();
									goto case ' ';
								// fall thru
								case ' ':
								case '\t':
								case '\u000C':
									/*
									 * U+0009 CHARACTER TABULATION U+000A LINE FEED
									 * (LF) U+000C FORM FEED (FF) U+0020 SPACE Stay
									 * in the before attribute value state.
									 */
									continue;
								case '"':
									/*
									 * U+0022 QUOTATION MARK (") Switch to the
									 * attribute value (double-quoted) state.
									 */
									clearLongStrBuf();
									state = transition(state, Tokenizer.ATTRIBUTE_VALUE_DOUBLE_QUOTED, reconsume, pos);
									goto breakBeforeattributevalueloop;
								// goto continueStateloop;
								case '&':
									/*
									 * U+0026 AMPERSAND (&) Switch to the attribute
									 * value (unquoted) state and reconsume this
									 * input character.
									 */
									clearLongStrBuf();
									state = transition(state, Tokenizer.ATTRIBUTE_VALUE_UNQUOTED, reconsume, pos);
									noteUnquotedAttributeValue();
									reconsume = true;
									goto continueStateloop;
								case '\'':
									/*
									 * U+0027 APOSTROPHE (') Switch to the attribute
									 * value (single-quoted) state.
									 */
									clearLongStrBuf();
									state = transition(state, Tokenizer.ATTRIBUTE_VALUE_SINGLE_QUOTED, reconsume, pos);
									goto continueStateloop;
								case '>':
									/*
									 * U+003E GREATER-THAN SIGN (>) Parse error.
									 */
									errAttributeValueMissing();
									/*
									 * Emit the current tag token.
									 */
									addAttributeWithoutValue();
									state = transition(state, emitCurrentTagToken(false, pos), reconsume, pos);
									if (shouldSuspend)
									{
										goto breakStateloop;
									}
									/*
									 * Switch to the data state.
									 */
									goto continueStateloop;
								case '\u0000':
									c = '\uFFFD';
									// fall thru
									goto case '<';
								case '<':
								case '=':
								case '`':
									/*
									 * U+003C LESS-THAN SIGN (<) U+003D EQUALS SIGN
									 * (=) U+0060 GRAVE ACCENT (`)
									 */
									errLtOrEqualsOrGraveInUnquotedAttributeOrNull(c);
									/*
									 * Treat it as per the "anything else" entry
									 * below.
									 */
									goto default;
								default:
									// [NOCPP[
									errHtml4NonNameInUnquotedAttribute(c);
									// ]NOCPP]
									/*
									 * Anything else Append the current input
									 * character to the current attribute's value.
									 */
									clearLongStrBufAndAppend(c);
									/*
									 * Switch to the attribute value (unquoted)
									 * state.
									 */

									state = transition(state, Tokenizer.ATTRIBUTE_VALUE_UNQUOTED, reconsume, pos);
									noteUnquotedAttributeValue();
									goto continueStateloop;
							}
						}
					breakBeforeattributevalueloop:
						goto case ATTRIBUTE_VALUE_DOUBLE_QUOTED;
					// FALLTHRU DON'T REORDER
					case ATTRIBUTE_VALUE_DOUBLE_QUOTED:
						/*attributevaluedoublequotedloop:*/
						for (; ; )
						{
							if (reconsume)
							{
								reconsume = false;
							}
							else
							{
								if (++pos == endPos)
								{
									goto breakStateloop;
								}
								c = checkChar(buf, pos);
							}
							/*
							 * Consume the next input character:
							 */
							switch (c)
							{
								case '"':
									/*
									 * U+0022 QUOTATION MARK (") Switch to the after
									 * attribute value (quoted) state.
									 */
									addAttributeWithValue();

									state = transition(state, Tokenizer.AFTER_ATTRIBUTE_VALUE_QUOTED, reconsume, pos);
									goto breakAttributevaluedoublequotedloop;
								// goto continueStateloop;
								case '&':
									/*
									 * U+0026 AMPERSAND (&) Switch to the character
									 * reference in attribute value state, with the
									 * additional allowed character being U+0022
									 * QUOTATION MARK (").
									 */
									clearStrBufAndAppend(c);
									setAdditionalAndRememberAmpersandLocation('\"');
									returnState = state;
									state = transition(state, Tokenizer.CONSUME_CHARACTER_REFERENCE, reconsume, pos);
									goto continueStateloop;
								case '\r':
									appendLongStrBufCarriageReturn();
									goto breakStateloop;
								case '\n':
									appendLongStrBufLineFeed();
									continue;
								case '\u0000':
									c = '\uFFFD';
									// fall thru
									goto default;
								default:
									/*
									 * Anything else Append the current input
									 * character to the current attribute's value.
									 */
									appendLongStrBuf(c);
									/*
									 * Stay in the attribute value (double-quoted)
									 * state.
									 */
									continue;
							}
						}
					breakAttributevaluedoublequotedloop:
						goto case AFTER_ATTRIBUTE_VALUE_QUOTED;
					// FALLTHRU DON'T REORDER
					case AFTER_ATTRIBUTE_VALUE_QUOTED:
						/*afterattributevaluequotedloop:*/
						for (; ; )
						{
							if (++pos == endPos)
							{
								goto breakStateloop;
							}
							c = checkChar(buf, pos);
							/*
							 * Consume the next input character:
							 */
							switch (c)
							{
								case '\r':
									silentCarriageReturn();
									state = transition(state, Tokenizer.BEFORE_ATTRIBUTE_NAME, reconsume, pos);
									goto breakStateloop;
								case '\n':
									silentLineFeed();
									// fall thru
									goto case ' ';
								case ' ':
								case '\t':
								case '\u000C':
									/*
									 * U+0009 CHARACTER TABULATION U+000A LINE FEED
									 * (LF) U+000C FORM FEED (FF) U+0020 SPACE
									 * Switch to the before attribute name state.
									 */
									state = transition(state, Tokenizer.BEFORE_ATTRIBUTE_NAME, reconsume, pos);
									goto continueStateloop;
								case '/':
									/*
									 * U+002F SOLIDUS (/) Switch to the self-closing
									 * start tag state.
									 */
									state = transition(state, Tokenizer.SELF_CLOSING_START_TAG, reconsume, pos);
									goto breakAfterattributevaluequotedloop;
								// goto continueStateloop;
								case '>':
									/*
									 * U+003E GREATER-THAN SIGN (>) Emit the current
									 * tag token.
									 */
									state = transition(state, emitCurrentTagToken(false, pos), reconsume, pos);
									if (shouldSuspend)
									{
										goto breakStateloop;
									}
									/*
									 * Switch to the data state.
									 */
									goto continueStateloop;
								default:
									/*
									 * Anything else Parse error.
									 */
									errNoSpaceBetweenAttributes();
									/*
									 * Reconsume the character in the before
									 * attribute name state.
									 */
									state = transition(state, Tokenizer.BEFORE_ATTRIBUTE_NAME, reconsume, pos);
									reconsume = true;
									goto continueStateloop;
							}
						}
					breakAfterattributevaluequotedloop:
						goto case SELF_CLOSING_START_TAG;
					// FALLTHRU DON'T REORDER
					case SELF_CLOSING_START_TAG:
						if (++pos == endPos)
						{
							goto breakStateloop;
						}
						c = checkChar(buf, pos);
						/*
						 * Consume the next input character:
						 */
						switch (c)
						{
							case '>':
								/*
								 * U+003E GREATER-THAN SIGN (>) Set the self-closing
								 * flag of the current tag token. Emit the current
								 * tag token.
								 */
								// [NOCPP[
								errHtml4XmlVoidSyntax();
								// ]NOCPP]
								state = transition(state, emitCurrentTagToken(true, pos), reconsume, pos);
								if (shouldSuspend)
								{
									goto breakStateloop;
								}
								/*
								 * Switch to the data state.
								 */
								goto continueStateloop;
							default:
								/* Anything else Parse error. */
								errSlashNotFollowedByGt();
								/*
								 * Reconsume the character in the before attribute
								 * name state.
								 */
								state = transition(state, Tokenizer.BEFORE_ATTRIBUTE_NAME, reconsume, pos);
								reconsume = true;
								goto continueStateloop;
						}
					// XXX reorder point
					case ATTRIBUTE_VALUE_UNQUOTED:
						for (; ; )
						{
							if (reconsume)
							{
								reconsume = false;
							}
							else
							{
								if (++pos == endPos)
								{
									goto breakStateloop;
								}
								c = checkChar(buf, pos);
							}
							/*
							 * Consume the next input character:
							 */
							switch (c)
							{
								case '\r':
									silentCarriageReturn();
									addAttributeWithValue();
									state = transition(state, Tokenizer.BEFORE_ATTRIBUTE_NAME, reconsume, pos);
									goto breakStateloop;
								case '\n':
									silentLineFeed();
									// fall thru
									goto case ' ';
								case ' ':
								case '\t':
								case '\u000C':
									/*
									 * U+0009 CHARACTER TABULATION U+000A LINE FEED
									 * (LF) U+000C FORM FEED (FF) U+0020 SPACE
									 * Switch to the before attribute name state.
									 */
									addAttributeWithValue();
									state = transition(state, Tokenizer.BEFORE_ATTRIBUTE_NAME, reconsume, pos);
									goto continueStateloop;
								case '&':
									/*
									 * U+0026 AMPERSAND (&) Switch to the character
									 * reference in attribute value state, with the
									 * additional allowed character being U+003E
									 * GREATER-THAN SIGN (>)
									 */
									clearStrBufAndAppend(c);
									setAdditionalAndRememberAmpersandLocation('>');
									returnState = state;
									state = transition(state, Tokenizer.CONSUME_CHARACTER_REFERENCE, reconsume, pos);
									goto continueStateloop;
								case '>':
									/*
									 * U+003E GREATER-THAN SIGN (>) Emit the current
									 * tag token.
									 */
									addAttributeWithValue();
									state = transition(state, emitCurrentTagToken(false, pos), reconsume, pos);
									if (shouldSuspend)
									{
										goto breakStateloop;
									}
									/*
									 * Switch to the data state.
									 */
									goto continueStateloop;
								case '\u0000':
									c = '\uFFFD';
									goto case '<';
								// fall thru
								case '<':
								case '\"':
								case '\'':
								case '=':
								case '`':
									/*
									 * U+0022 QUOTATION MARK (") U+0027 APOSTROPHE
									 * (') U+003C LESS-THAN SIGN (<) U+003D EQUALS
									 * SIGN (=) U+0060 GRAVE ACCENT (`) Parse error.
									 */
									errUnquotedAttributeValOrNull(c);
									/*
									 * Treat it as per the "anything else" entry
									 * below.
									 */
									// fall through
									goto default;
								default:
									// [NOCPP]
									errHtml4NonNameInUnquotedAttribute(c);
									// ]NOCPP]
									/*
									 * Anything else Append the current input
									 * character to the current attribute's value.
									 */
									appendLongStrBuf(c);
									/*
									 * Stay in the attribute value (unquoted) state.
									 */
									continue;
							}
						}
					// XXX reorder point
					case AFTER_ATTRIBUTE_NAME:
						for (; ; )
						{
							if (++pos == endPos)
							{
								goto breakStateloop;
							}
							c = checkChar(buf, pos);
							/*
							 * Consume the next input character:
							 */
							switch (c)
							{
								case '\r':
									silentCarriageReturn();
									goto breakStateloop;
								case '\n':
									silentLineFeed();
									// fall thru
									goto case ' ';
								case ' ':
								case '\t':
								case '\u000C':
									/*
									 * U+0009 CHARACTER TABULATION U+000A LINE FEED
									 * (LF) U+000C FORM FEED (FF) U+0020 SPACE Stay
									 * in the after attribute name state.
									 */
									continue;
								case '/':
									/*
									 * U+002F SOLIDUS (/) Switch to the self-closing
									 * start tag state.
									 */
									addAttributeWithoutValue();
									state = transition(state, Tokenizer.SELF_CLOSING_START_TAG, reconsume, pos);
									goto continueStateloop;
								case '=':
									/*
									 * U+003D EQUALS SIGN (=) Switch to the before
									 * attribute value state.
									 */
									state = transition(state, Tokenizer.BEFORE_ATTRIBUTE_VALUE, reconsume, pos);
									goto continueStateloop;
								case '>':
									/*
									 * U+003E GREATER-THAN SIGN (>) Emit the current
									 * tag token.
									 */
									addAttributeWithoutValue();
									state = transition(state, emitCurrentTagToken(false, pos), reconsume, pos);
									if (shouldSuspend)
									{
										goto breakStateloop;
									}
									/*
									 * Switch to the data state.
									 */
									goto continueStateloop;
								case '\u0000':
									c = '\uFFFD';
									goto case '\"';
								// fall thru
								case '\"':
								case '\'':
								case '<':
									errQuoteOrLtInAttributeNameOrNull(c);
									/*
									 * Treat it as per the "anything else" entry
									 * below.
									 */
									goto default;
								default:
									addAttributeWithoutValue();
									/*
									 * Anything else Start a new attribute in the
									 * current tag token.
									 */
									if (c >= 'A' && c <= 'Z')
									{
										/*
										 * U+0041 LATIN CAPITAL LETTER A through to
										 * U+005A LATIN CAPITAL LETTER Z Set that
										 * attribute's name to the lowercase version
										 * of the current input character (add
										 * 0x0020 to the character's code point)
										 */
										c += (char)0x20;
									}
									/*
									 * Set that attribute's name to the current
									 * input character,
									 */
									clearStrBufAndAppend(c);
									/*
									 * and its value to the empty string.
									 */
									// Will do later.
									/*
									 * Switch to the attribute name state.
									 */
									state = transition(state, Tokenizer.ATTRIBUTE_NAME, reconsume, pos);
									goto continueStateloop;
							}
						}
					// XXX reorder point
					case MARKUP_DECLARATION_OPEN:
						/*markupdeclarationopenloop:*/
						for (; ; )
						{
							if (++pos == endPos)
							{
								goto breakStateloop;
							}
							c = checkChar(buf, pos);
							/*
							 * If the next two characters are both U+002D
							 * HYPHEN-MINUS characters (-), consume those two
							 * characters, create a comment token whose data is the
							 * empty string, and switch to the comment start state.
							 * 
							 * Otherwise, if the next seven characters are an ASCII
							 * case-insensitive match for the word "DOCTYPE", then
							 * consume those characters and switch to the DOCTYPE
							 * state.
							 * 
							 * Otherwise, if the insertion mode is
							 * "in foreign content" and the current node is not an
							 * element in the HTML namespace and the next seven
							 * characters are an case-sensitive match for the string
							 * "[CDATA[" (the five uppercase letters "CDATA" with a
							 * U+005B LEFT SQUARE BRACKET character before and
							 * after), then consume those characters and switch to
							 * the CDATA section state.
							 * 
							 * Otherwise, is is a parse error. Switch to the bogus
							 * comment state. The next character that is consumed,
							 * if any, is the first character that will be in the
							 * comment.
							 */
							switch (c)
							{
								case '-':
									clearLongStrBufAndAppend(c);
									state = transition(state, Tokenizer.MARKUP_DECLARATION_HYPHEN, reconsume, pos);
									goto breakMarkupdeclarationopenloop;
								// goto continueStateloop;
								case 'd':
								case 'D':
									clearLongStrBufAndAppend(c);
									index = 0;
									state = transition(state, Tokenizer.MARKUP_DECLARATION_OCTYPE, reconsume, pos);
									goto continueStateloop;
								case '[':
									if (TokenHandler.IsCDataSectionAllowed)
									{
										clearLongStrBufAndAppend(c);
										index = 0;
										state = transition(state, Tokenizer.CDATA_START, reconsume, pos);
										goto continueStateloop;
									}
									else
									{
										// else fall through
										goto default;
									}
								default:
									errBogusComment();
									clearLongStrBuf();
									state = transition(state, Tokenizer.BOGUS_COMMENT, reconsume, pos);
									reconsume = true;
									goto continueStateloop;
							}
						}
					breakMarkupdeclarationopenloop:
						goto case MARKUP_DECLARATION_HYPHEN;
					// FALLTHRU DON'T REORDER
					case MARKUP_DECLARATION_HYPHEN:
						/*markupdeclarationhyphenloop:*/
						for (; ; )
						{
							if (++pos == endPos)
							{
								goto breakStateloop;
							}
							c = checkChar(buf, pos);
							switch (c)
							{
								case '\u0000':
									goto breakStateloop;
								case '-':
									clearLongStrBuf();
									state = transition(state, Tokenizer.COMMENT_START, reconsume, pos);
									goto breakMarkupdeclarationhyphenloop;
								// goto continueStateloop;
								default:
									errBogusComment();
									state = transition(state, Tokenizer.BOGUS_COMMENT, reconsume, pos);
									reconsume = true;
									goto continueStateloop;
							}
						}
					breakMarkupdeclarationhyphenloop:
						goto case COMMENT_START;
					// FALLTHRU DON'T REORDER
					case COMMENT_START:
						/*commentstartloop:*/
						for (; ; )
						{
							if (++pos == endPos)
							{
								goto breakStateloop;
							}
							c = checkChar(buf, pos);
							/*
							 * Comment start state
							 * 
							 * 
							 * Consume the next input character:
							 */
							switch (c)
							{
								case '-':
									/*
									 * U+002D HYPHEN-MINUS (-) Switch to the comment
									 * start dash state.
									 */
									appendLongStrBuf(c);
									state = transition(state, Tokenizer.COMMENT_START_DASH, reconsume, pos);
									goto continueStateloop;
								case '>':
									/*
									 * U+003E GREATER-THAN SIGN (>) Parse error.
									 */
									errPrematureEndOfComment();
									/* Emit the comment token. */
									EmitComment(0, pos);
									/*
									 * Switch to the data state.
									 */
									state = transition(state, Tokenizer.DATA, reconsume, pos);
									goto continueStateloop;
								case '\r':
									appendLongStrBufCarriageReturn();
									state = transition(state, Tokenizer.COMMENT, reconsume, pos);
									goto breakStateloop;
								case '\n':
									appendLongStrBufLineFeed();
									state = transition(state, Tokenizer.COMMENT, reconsume, pos);
									goto breakCommentstartloop;
								case '\u0000':
									c = '\uFFFD';
									// fall thru
									goto default;
								default:
									/*
									 * Anything else Append the input character to
									 * the comment token's data.
									 */
									appendLongStrBuf(c);
									/*
									 * Switch to the comment state.
									 */
									state = transition(state, Tokenizer.COMMENT, reconsume, pos);
									goto breakCommentstartloop;
								// goto continueStateloop;
							}
						}
					breakCommentstartloop:
						goto case COMMENT;
					// FALLTHRU DON'T REORDER
					case COMMENT:
						/*commentloop:*/
						for (; ; )
						{
							if (++pos == endPos)
							{
								goto breakStateloop;
							}
							c = checkChar(buf, pos);
							/*
							 * Comment state Consume the next input character:
							 */
							switch (c)
							{
								case '-':
									/*
									 * U+002D HYPHEN-MINUS (-) Switch to the comment
									 * end dash state
									 */
									appendLongStrBuf(c);
									state = transition(state, Tokenizer.COMMENT_END_DASH, reconsume, pos);
									goto breakCommentloop;
								// goto continueStateloop;
								case '\r':
									appendLongStrBufCarriageReturn();
									goto breakStateloop;
								case '\n':
									appendLongStrBufLineFeed();
									continue;
								case '\u0000':
									c = '\uFFFD';
									// fall thru
									goto default;
								default:
									/*
									 * Anything else Append the input character to
									 * the comment token's data.
									 */
									appendLongStrBuf(c);
									/*
									 * Stay in the comment state.
									 */
									continue;
							}
						}
					breakCommentloop:
						goto case COMMENT_END_DASH;
					// FALLTHRU DON'T REORDER
					case COMMENT_END_DASH:
						/*commentenddashloop:*/
						for (; ; )
						{
							if (++pos == endPos)
							{
								goto breakStateloop;
							}
							c = checkChar(buf, pos);
							/*
							 * Comment end dash state Consume the next input
							 * character:
							 */
							switch (c)
							{
								case '-':
									/*
									 * U+002D HYPHEN-MINUS (-) Switch to the comment
									 * end state
									 */
									appendLongStrBuf(c);
									state = transition(state, Tokenizer.COMMENT_END, reconsume, pos);
									goto breakCommentenddashloop;
								// goto continueStateloop;
								case '\r':
									appendLongStrBufCarriageReturn();
									state = transition(state, Tokenizer.COMMENT, reconsume, pos);
									goto breakStateloop;
								case '\n':
									appendLongStrBufLineFeed();
									state = transition(state, Tokenizer.COMMENT, reconsume, pos);
									goto continueStateloop;
								case '\u0000':
									c = '\uFFFD';
									goto default;
								// fall thru
								default:
									/*
									 * Anything else Append a U+002D HYPHEN-MINUS
									 * (-) character and the input character to the
									 * comment token's data.
									 */
									appendLongStrBuf(c);
									/*
									 * Switch to the comment state.
									 */
									state = transition(state, Tokenizer.COMMENT, reconsume, pos);
									goto continueStateloop;
							}
						}
					breakCommentenddashloop:
						goto case COMMENT_END;
					// FALLTHRU DON'T REORDER
					case COMMENT_END:
						/*commentendloop:*/
						for (; ; )
						{
							if (++pos == endPos)
							{
								goto breakStateloop;
							}
							c = checkChar(buf, pos);
							/*
							 * Comment end dash state Consume the next input
							 * character:
							 */
							switch (c)
							{
								case '>':
									/*
									 * U+003E GREATER-THAN SIGN (>) Emit the comment
									 * token.
									 */
									EmitComment(2, pos);
									/*
									 * Switch to the data state.
									 */
									state = transition(state, Tokenizer.DATA, reconsume, pos);
									goto continueStateloop;
								case '-':
									/* U+002D HYPHEN-MINUS (-) Parse error. */
									/*
									 * Append a U+002D HYPHEN-MINUS (-) character to
									 * the comment token's data.
									 */
									adjustDoubleHyphenAndAppendToLongStrBufAndErr(c);
									/*
									 * Stay in the comment end state.
									 */
									continue;
								case '\r':
									adjustDoubleHyphenAndAppendToLongStrBufCarriageReturn();
									state = transition(state, Tokenizer.COMMENT, reconsume, pos);
									goto breakStateloop;
								case '\n':
									adjustDoubleHyphenAndAppendToLongStrBufLineFeed();
									state = transition(state, Tokenizer.COMMENT, reconsume, pos);
									goto continueStateloop;
								case '!':
									errHyphenHyphenBang();
									appendLongStrBuf(c);
									state = transition(state, Tokenizer.COMMENT_END_BANG, reconsume, pos);
									goto continueStateloop;
								case '\u0000':
									c = '\uFFFD';
									// fall thru
									goto default;
								default:
									/*
									 * Append two U+002D HYPHEN-MINUS (-) characters
									 * and the input character to the comment
									 * token's data.
									 */
									adjustDoubleHyphenAndAppendToLongStrBufAndErr(c);
									/*
									 * Switch to the comment state.
									 */
									state = transition(state, Tokenizer.COMMENT, reconsume, pos);
									goto continueStateloop;
							}
						}
					// XXX reorder point
					case COMMENT_END_BANG:
						for (; ; )
						{
							if (++pos == endPos)
							{
								goto breakStateloop;
							}
							c = checkChar(buf, pos);
							/*
							 * Comment end bang state
							 * 
							 * Consume the next input character:
							 */
							switch (c)
							{
								case '>':
									/*
									 * U+003E GREATER-THAN SIGN (>) Emit the comment
									 * token.
									 */
									EmitComment(3, pos);
									/*
									 * Switch to the data state.
									 */
									state = transition(state, Tokenizer.DATA, reconsume, pos);
									goto continueStateloop;
								case '-':
									/*
									 * Append two U+002D HYPHEN-MINUS (-) characters
									 * and a U+0021 EXCLAMATION MARK (!) character
									 * to the comment token's data.
									 */
									appendLongStrBuf(c);
									/*
									 * Switch to the comment end dash state.
									 */
									state = transition(state, Tokenizer.COMMENT_END_DASH, reconsume, pos);
									goto continueStateloop;
								case '\r':
									appendLongStrBufCarriageReturn();
									goto breakStateloop;
								case '\n':
									appendLongStrBufLineFeed();
									continue;
								case '\u0000':
									c = '\uFFFD';
									// fall thru
									goto default;
								default:
									/*
									 * Anything else Append two U+002D HYPHEN-MINUS
									 * (-) characters, a U+0021 EXCLAMATION MARK (!)
									 * character, and the input character to the
									 * comment token's data. Switch to the comment
									 * state.
									 */
									appendLongStrBuf(c);
									/*
									 * Switch to the comment state.
									 */
									state = transition(state, Tokenizer.COMMENT, reconsume, pos);
									goto continueStateloop;
							}
						}
					// XXX reorder point
					case COMMENT_START_DASH:
						if (++pos == endPos)
						{
							goto breakStateloop;
						}
						c = checkChar(buf, pos);
						/*
						 * Comment start dash state
						 * 
						 * Consume the next input character:
						 */
						switch (c)
						{
							case '-':
								/*
								 * U+002D HYPHEN-MINUS (-) Switch to the comment end
								 * state
								 */
								appendLongStrBuf(c);
								state = transition(state, Tokenizer.COMMENT_END, reconsume, pos);
								goto continueStateloop;
							case '>':
								errPrematureEndOfComment();
								/* Emit the comment token. */
								EmitComment(1, pos);
								/*
								 * Switch to the data state.
								 */
								state = transition(state, Tokenizer.DATA, reconsume, pos);
								goto continueStateloop;
							case '\r':
								appendLongStrBufCarriageReturn();
								state = transition(state, Tokenizer.COMMENT, reconsume, pos);
								goto breakStateloop;
							case '\n':
								appendLongStrBufLineFeed();
								state = transition(state, Tokenizer.COMMENT, reconsume, pos);
								goto continueStateloop;
							case '\u0000':
								c = '\uFFFD';
								// fall thru
								goto default;
							default:
								/*
								 * Append a U+002D HYPHEN-MINUS character (-) and
								 * the current input character to the comment
								 * token's data.
								 */
								appendLongStrBuf(c);
								/*
								 * Switch to the comment state.
								 */
								state = transition(state, Tokenizer.COMMENT, reconsume, pos);
								goto continueStateloop;
						}
					// XXX reorder point
					case CDATA_START:
						for (; ; )
						{
							if (++pos == endPos)
							{
								goto breakStateloop;
							}
							c = checkChar(buf, pos);
							if (index < 6)
							{ // CDATA_LSQB.Length
								if (c == Tokenizer.CDATA_LSQB[index])
								{
									appendLongStrBuf(c);
								}
								else
								{
									errBogusComment();
									state = transition(state, Tokenizer.BOGUS_COMMENT, reconsume, pos);
									reconsume = true;
									goto continueStateloop;
								}
								index++;
								continue;
							}
							else
							{
								cstart = pos; // start coalescing
								state = transition(state, Tokenizer.CDATA_SECTION, reconsume, pos);
								reconsume = true;
								break; // FALL THROUGH goto continueStateloop;
							}
						}
						goto case CDATA_SECTION;
					// WARNING FALLTHRU CASE TRANSITION: DON'T REORDER
					case CDATA_SECTION:
						/*cdatasectionloop:*/
						for (; ; )
						{
							if (reconsume)
							{
								reconsume = false;
							}
							else
							{
								if (++pos == endPos)
								{
									goto breakStateloop;
								}
								c = checkChar(buf, pos);
							}
							switch (c)
							{
								case ']':
									FlushChars(buf, pos);
									state = transition(state, Tokenizer.CDATA_RSQB, reconsume, pos);
									goto breakCdatasectionloop; // FALL THROUGH
								case '\u0000':
									emitReplacementCharacter(buf, pos);
									continue;
								case '\r':
									emitCarriageReturn(buf, pos);
									goto breakStateloop;
								case '\n':
									silentLineFeed();
									goto default;
								// fall thru
								default:
									continue;
							}
						}
					breakCdatasectionloop:
						goto case CDATA_RSQB;
					// WARNING FALLTHRU CASE TRANSITION: DON'T REORDER
					case CDATA_RSQB:
						/*cdatarsqb:*/
						for (; ; )
						{
							if (++pos == endPos)
							{
								goto breakStateloop;
							}
							c = checkChar(buf, pos);
							switch (c)
							{
								case ']':
									state = transition(state, Tokenizer.CDATA_RSQB_RSQB, reconsume, pos);
									goto breakCdatarsqb;
								default:
									TokenHandler.Characters(Tokenizer.RSQB_RSQB, 0,
											1);
									cstart = pos;
									state = transition(state, Tokenizer.CDATA_SECTION, reconsume, pos);
									reconsume = true;
									goto continueStateloop;
							}
						}
					breakCdatarsqb:
						goto case CDATA_RSQB_RSQB;
					// WARNING FALLTHRU CASE TRANSITION: DON'T REORDER
					case CDATA_RSQB_RSQB:
						if (++pos == endPos)
						{
							goto breakStateloop;
						}
						c = checkChar(buf, pos);
						switch (c)
						{
							case '>':
								cstart = pos + 1;
								state = transition(state, Tokenizer.DATA, reconsume, pos);
								goto continueStateloop;
							default:
								TokenHandler.Characters(Tokenizer.RSQB_RSQB, 0, 2);
								cstart = pos;
								state = transition(state, Tokenizer.CDATA_SECTION, reconsume, pos);
								reconsume = true;
								goto continueStateloop;

						}
					// XXX reorder point
					case ATTRIBUTE_VALUE_SINGLE_QUOTED:
						/*attributevaluesinglequotedloop:*/
						for (; ; )
						{
							if (reconsume)
							{
								reconsume = false;
							}
							else
							{
								if (++pos == endPos)
								{
									goto breakStateloop;
								}
								c = checkChar(buf, pos);
							}
							/*
							 * Consume the next input character:
							 */
							switch (c)
							{
								case '\'':
									/*
									 * U+0027 APOSTROPHE (') Switch to the after
									 * attribute value (quoted) state.
									 */
									addAttributeWithValue();

									state = transition(state, Tokenizer.AFTER_ATTRIBUTE_VALUE_QUOTED, reconsume, pos);
									goto continueStateloop;
								case '&':
									/*
									 * U+0026 AMPERSAND (&) Switch to the character
									 * reference in attribute value state, with the
									 * + additional allowed character being U+0027
									 * APOSTROPHE (').
									 */
									clearStrBufAndAppend(c);
									setAdditionalAndRememberAmpersandLocation('\'');
									returnState = state;
									state = transition(state, Tokenizer.CONSUME_CHARACTER_REFERENCE, reconsume, pos);
									goto breakAttributevaluesinglequotedloop;
								// goto continueStateloop;
								case '\r':
									appendLongStrBufCarriageReturn();
									goto breakStateloop;
								case '\n':
									appendLongStrBufLineFeed();
									continue;
								case '\u0000':
									c = '\uFFFD';
									goto default;
								// fall thru
								default:
									/*
									 * Anything else Append the current input
									 * character to the current attribute's value.
									 */
									appendLongStrBuf(c);
									/*
									 * Stay in the attribute value (double-quoted)
									 * state.
									 */
									continue;
							}
						}
					breakAttributevaluesinglequotedloop:
						goto case CONSUME_CHARACTER_REFERENCE;
					// FALLTHRU DON'T REORDER
					case CONSUME_CHARACTER_REFERENCE:
						if (++pos == endPos)
						{
							goto breakStateloop;
						}
						c = checkChar(buf, pos);
						if (c == '\u0000')
						{
							goto breakStateloop;
						}
						/*
						 * Unlike the definition is the spec, this state does not
						 * return a value and never requires the caller to
						 * backtrack. This state takes care of emitting characters
						 * or appending to the current attribute value. It also
						 * takes care of that in the case when consuming the
						 * character reference fails.
						 */
						/*
						 * This section defines how to consume a character
						 * reference. This definition is used when parsing character
						 * references in text and in attributes.
						 * 
						 * The behavior depends on the identity of the next
						 * character (the one immediately after the U+0026 AMPERSAND
						 * character):
						 */
						switch (c)
						{
							case ' ':
							case '\t':
							case '\n':
							case '\r': // we'll reconsume!
							case '\u000C':
							case '<':
							case '&':
								emitOrAppendStrBuf(returnState);
								if ((returnState & DATA_AND_RCDATA_MASK) == 0)
								{
									cstart = pos;
								}
								state = transition(state, returnState, reconsume, pos);
								reconsume = true;
								goto continueStateloop;
							case '#':
								/*
								 * U+0023 NUMBER SIGN (#) Consume the U+0023 NUMBER
								 * SIGN.
								 */
								appendStrBuf('#');
								state = transition(state, Tokenizer.CONSUME_NCR, reconsume, pos);
								goto continueStateloop;
							default:
								if (c == additional)
								{
									emitOrAppendStrBuf(returnState);
									state = transition(state, returnState, reconsume, pos);
									reconsume = true;
									goto continueStateloop;
								}
								if (c >= 'a' && c <= 'z')
								{
									firstCharKey = c - 'a' + 26;
								}
								else if (c >= 'A' && c <= 'Z')
								{
									firstCharKey = c - 'A';
								}
								else
								{
									// No match
									/*
									 * If no match can be made, then this is a parse
									 * error.
									 */
									errNoNamedCharacterMatch();
									emitOrAppendStrBuf(returnState);
									if ((returnState & DATA_AND_RCDATA_MASK) == 0)
									{
										cstart = pos;
									}
									state = transition(state, returnState, reconsume, pos);
									reconsume = true;
									goto continueStateloop;
								}
								// Didn't fail yet
								appendStrBuf(c);
								state = transition(state, Tokenizer.CHARACTER_REFERENCE_HILO_LOOKUP, reconsume, pos);
								// FALL THROUGH goto continueStateloop;
								break;
						}
						goto case CHARACTER_REFERENCE_HILO_LOOKUP;
					// WARNING FALLTHRU CASE TRANSITION: DON'T REORDER
					case CHARACTER_REFERENCE_HILO_LOOKUP:
						{
							if (++pos == endPos)
							{
								goto breakStateloop;
							}
							c = checkChar(buf, pos);
							if (c == '\u0000')
							{
								goto breakStateloop;
							}
							/*
							 * The data structure is as follows:
							 * 
							 * HILO_ACCEL is a two-dimensional int array whose major
							 * index corresponds to the second character of the
							 * character reference (code point as index) and the
							 * minor index corresponds to the first character of the
							 * character reference (packed so that A-Z runs from 0
							 * to 25 and a-z runs from 26 to 51). This layout makes
							 * it easier to use the sparseness of the data structure
							 * to omit parts of it: The second dimension of the
							 * table is null when no character reference starts with
							 * the character corresponding to that row.
							 * 
							 * The int value HILO_ACCEL (by these indeces) is zero
							 * if there exists no character reference starting with
							 * that two-letter prefix. Otherwise, the value is an
							 * int that packs two shorts so that the higher short is
							 * the index of the highest character reference name
							 * with that prefix in NAMES and the lower short
							 * corresponds to the index of the lowest character
							 * reference name with that prefix. (It happens that the
							 * first two character reference names share their
							 * prefix so the packed int cannot be 0 by packing the
							 * two shorts.)
							 * 
							 * NAMES is an array of byte arrays where each byte
							 * array encodes the name of a character references as
							 * ASCII. The names omit the first two letters of the
							 * name. (Since storing the first two letters would be
							 * redundant with the data contained in HILO_ACCEL.) The
							 * entries are lexically sorted.
							 * 
							 * For a given index in NAMES, the same index in VALUES
							 * contains the corresponding expansion as an array of
							 * two UTF-16 code units (either the character and
							 * U+0000 or a suggogate pair).
							 */
							int hilo = 0;
							if (c <= 'z')
							{
								int[] row = NamedCharactersAccel.HILO_ACCEL[c];
								if (row != null)
								{
									hilo = row[firstCharKey];
								}
							}
							if (hilo == 0)
							{
								/*
								 * If no match can be made, then this is a parse
								 * error.
								 */
								errNoNamedCharacterMatch();
								emitOrAppendStrBuf(returnState);
								if ((returnState & DATA_AND_RCDATA_MASK) == 0)
								{
									cstart = pos;
								}
								state = transition(state, returnState, reconsume, pos);
								reconsume = true;
								goto continueStateloop;
							}
							// Didn't fail yet
							appendStrBuf(c);
							lo = hilo & 0xFFFF;
							hi = hilo >> 16;
							entCol = -1;
							candidate = -1;
							strBufMark = 0;
							state = transition(state, Tokenizer.CHARACTER_REFERENCE_TAIL, reconsume, pos);
							// FALL THROUGH goto continueStateloop;
							goto case CHARACTER_REFERENCE_TAIL;
						}
					case CHARACTER_REFERENCE_TAIL:
						/*outer:*/
						for (; ; )
						{
							if (++pos == endPos)
							{
								goto breakStateloop;
							}
							c = checkChar(buf, pos);
							if (c == '\u0000')
							{
								goto breakStateloop;
							}
							entCol++;
							/*
							 * Consume the maximum number of characters possible,
							 * with the consumed characters matching one of the
							 * identifiers in the first column of the named
							 * character references table (in a case-sensitive
							 * manner).
							 */
							/*loloop:*/
							for (; ; )
							{
								if (hi < lo)
								{
									goto breakOuter;
								}
								if (entCol == NamedCharacters.NAMES[lo].Length)
								{
									candidate = lo;
									strBufMark = strBufLen;
									lo++;
								}
								else if (entCol > NamedCharacters.NAMES[lo].Length)
								{
									goto breakOuter;
								}
								else if (c > NamedCharacters.NAMES[lo][entCol])
								{
									lo++;
								}
								else
								{
									goto breakLoloop;
								}
							}

						breakLoloop:

							/*hiloop:*/
							for (; ; )
							{
								if (hi < lo)
								{
									goto breakOuter;
								}
								if (entCol == NamedCharacters.NAMES[hi].Length)
								{
									goto breakHiloop;
								}
								if (entCol > NamedCharacters.NAMES[hi].Length)
								{
									goto breakOuter;
								}
								else if (c < NamedCharacters.NAMES[hi][entCol])
								{
									hi--;
								}
								else
								{
									goto breakHiloop;
								}
							}

						breakHiloop:

							if (hi < lo)
							{
								goto breakOuter;
							}
							appendStrBuf(c);
							continue;
						}

					breakOuter:

						if (candidate == -1)
						{
							// reconsume deals with CR, LF or nul
							/*
							 * If no match can be made, then this is a parse error.
							 */
							errNoNamedCharacterMatch();
							emitOrAppendStrBuf(returnState);
							if ((returnState & DATA_AND_RCDATA_MASK) == 0)
							{
								cstart = pos;
							}
							state = transition(state, returnState, reconsume, pos);
							reconsume = true;
							goto continueStateloop;
						}
						else
						{
							// c can't be CR, LF or nul if we got here
							string candidateName = NamedCharacters.NAMES[candidate];
							if (candidateName.Length == 0
									|| candidateName[candidateName.Length - 1] != ';')
							{
								/*
								 * If the last character matched is not a U+003B
								 * SEMICOLON (;), there is a parse error.
								 */
								if ((returnState & DATA_AND_RCDATA_MASK) != 0)
								{
									/*
									 * If the entity is being consumed as part of an
									 * attribute, and the last character matched is
									 * not a U+003B SEMICOLON (;),
									 */
									char ch;
									if (strBufMark == strBufLen)
									{
										ch = c;
									}
									else
									{
										// if (strBufOffset != -1) {
										// ch = buf[strBufOffset + strBufMark];
										// } else {
										ch = strBuf[strBufMark];
										// }
									}
									if (ch == '=' || (ch >= '0' && ch <= '9')
											|| (ch >= 'A' && ch <= 'Z')
											|| (ch >= 'a' && ch <= 'z'))
									{
										/*
										 * and the next character is either a U+003D
										 * EQUALS SIGN character (=) or in the range
										 * U+0030 DIGIT ZERO to U+0039 DIGIT NINE,
										 * U+0041 LATIN CAPITAL LETTER A to U+005A
										 * LATIN CAPITAL LETTER Z, or U+0061 LATIN
										 * SMALL LETTER A to U+007A LATIN SMALL
										 * LETTER Z, then, for historical reasons,
										 * all the characters that were matched
										 * after the U+0026 AMPERSAND (&) must be
										 * unconsumed, and nothing is returned.
										 */
										errNoNamedCharacterMatch();
										appendStrBufToLongStrBuf();
										state = transition(state, returnState, reconsume, pos);
										reconsume = true;
										goto continueStateloop;
									}
								}
								if ((returnState & DATA_AND_RCDATA_MASK) != 0)
								{
									errUnescapedAmpersandInterpretedAsCharacterReference();
								}
								else
								{
									errNotSemicolonTerminated();
								}
							}

							/*
							 * Otherwise, return a character token for the character
							 * corresponding to the entity name (as given by the
							 * second column of the named character references
							 * table).
							 */
							char[] val = NamedCharacters.VALUES[candidate];
							if (
								// [NOCPP[
							val.Length == 1
								// ]NOCPP]
								// CPPONLY: val[1] == 0
							)
							{
								emitOrAppendOne(val, returnState);
							}
							else
							{
								emitOrAppendTwo(val, returnState);
							}
							// this is so complicated!
							if (strBufMark < strBufLen)
							{
								// if (strBufOffset != -1) {
								// if ((returnState & (~1)) != 0) {
								// for (int i = strBufMark; i < strBufLen; i++) {
								// appendLongStrBuf(buf[strBufOffset + i]);
								// }
								// } else {
								// tokenHandler.Characters(buf, strBufOffset
								// + strBufMark, strBufLen
								// - strBufMark);
								// }
								// } else {
								if ((returnState & DATA_AND_RCDATA_MASK) != 0)
								{
									for (int i = strBufMark; i < strBufLen; i++)
									{
										appendLongStrBuf(strBuf[i]);
									}
								}
								else
								{
									TokenHandler.Characters(strBuf, strBufMark,
											strBufLen - strBufMark);
								}
								// }
							}
							if ((returnState & DATA_AND_RCDATA_MASK) == 0)
							{
								cstart = pos;
							}
							state = transition(state, returnState, reconsume, pos);
							reconsume = true;
							goto continueStateloop;
							/*
							 * If the markup contains I'm &notit; I tell you, the
							 * entity is parsed as "not", as in, I'm Â¬it; I tell
							 * you. But if the markup was I'm &notin; I tell you,
							 * the entity would be parsed as "notin;", resulting in
							 * I'm âˆ‰ I tell you.
							 */
						}
					// XXX reorder point
					case CONSUME_NCR:
						if (++pos == endPos)
						{
							goto breakStateloop;
						}
						c = checkChar(buf, pos);
						prevValue = -1;
						value = 0;
						seenDigits = false;
						/*
						 * The behavior further depends on the character after the
						 * U+0023 NUMBER SIGN:
						 */
						switch (c)
						{
							case 'x':
							case 'X':

								/*
								 * U+0078 LATIN SMALL LETTER X U+0058 LATIN CAPITAL
								 * LETTER X Consume the X.
								 * 
								 * Follow the steps below, but using the range of
								 * characters U+0030 DIGIT ZERO through to U+0039
								 * DIGIT NINE, U+0061 LATIN SMALL LETTER A through
								 * to U+0066 LATIN SMALL LETTER F, and U+0041 LATIN
								 * CAPITAL LETTER A, through to U+0046 LATIN CAPITAL
								 * LETTER F (in other words, 0-9, A-F, a-f).
								 * 
								 * When it comes to interpreting the number,
								 * interpret it as a hexadecimal number.
								 */
								appendStrBuf(c);
								state = transition(state, Tokenizer.HEX_NCR_LOOP, reconsume, pos);
								goto continueStateloop;
							default:
								/*
								 * Anything else Follow the steps below, but using
								 * the range of characters U+0030 DIGIT ZERO through
								 * to U+0039 DIGIT NINE (i.e. just 0-9).
								 * 
								 * When it comes to interpreting the number,
								 * interpret it as a decimal number.
								 */
								state = transition(state, Tokenizer.DECIMAL_NRC_LOOP, reconsume, pos);
								reconsume = true;
								// FALL THROUGH goto continueStateloop;
								break;
						}
						// WARNING FALLTHRU CASE TRANSITION: DON'T REORDER
						goto case DECIMAL_NRC_LOOP;
					case DECIMAL_NRC_LOOP:
						/*decimalloop:*/
						for (; ; )
						{
							if (reconsume)
							{
								reconsume = false;
							}
							else
							{
								if (++pos == endPos)
								{
									goto breakStateloop;
								}
								c = checkChar(buf, pos);
							}
							// Deal with overflow gracefully
							if (value < prevValue)
							{
								value = 0x110000; // Value above Unicode range but
								// within int
								// range
							}
							prevValue = value;
							/*
							 * Consume as many characters as match the range of
							 * characters given above.
							 */
							if (c >= '0' && c <= '9')
							{
								seenDigits = true;
								value *= 10;
								value += c - '0';
								continue;
							}
							else if (c == ';')
							{
								if (seenDigits)
								{
									if ((returnState & DATA_AND_RCDATA_MASK) == 0)
									{
										cstart = pos + 1;
									}
									state = transition(state, Tokenizer.HANDLE_NCR_VALUE, reconsume, pos);
									// FALL THROUGH goto continueStateloop;
									goto breakDecimalloop;
								}
								else
								{
									errNoDigitsInNCR();
									appendStrBuf(';');
									emitOrAppendStrBuf(returnState);
									if ((returnState & DATA_AND_RCDATA_MASK) == 0)
									{
										cstart = pos + 1;
									}
									state = transition(state, returnState, reconsume, pos);
									goto continueStateloop;
								}
							}
							else
							{
								/*
								 * If no characters match the range, then don't
								 * consume any characters (and unconsume the U+0023
								 * NUMBER SIGN character and, if appropriate, the X
								 * character). This is a parse error; nothing is
								 * returned.
								 * 
								 * Otherwise, if the next character is a U+003B
								 * SEMICOLON, consume that too. If it isn't, there
								 * is a parse error.
								 */
								if (!seenDigits)
								{
									errNoDigitsInNCR();
									emitOrAppendStrBuf(returnState);
									if ((returnState & DATA_AND_RCDATA_MASK) == 0)
									{
										cstart = pos;
									}
									state = transition(state, returnState, reconsume, pos);
									reconsume = true;
									goto continueStateloop;
								}
								else
								{
									errCharRefLacksSemicolon();
									if ((returnState & DATA_AND_RCDATA_MASK) == 0)
									{
										cstart = pos;
									}
									state = transition(state, Tokenizer.HANDLE_NCR_VALUE, reconsume, pos);
									reconsume = true;
									// FALL THROUGH goto continueStateloop;
									goto breakDecimalloop;
								}
							}
						}
					breakDecimalloop:
						goto case HANDLE_NCR_VALUE;
					// WARNING FALLTHRU CASE TRANSITION: DON'T REORDER
					case HANDLE_NCR_VALUE:
						// WARNING previous state sets reconsume
						// XXX inline this case if the method size can take it
						handleNcrValue(returnState);
						state = transition(state, returnState, reconsume, pos);
						goto continueStateloop;
					// XXX reorder point
					case HEX_NCR_LOOP:
						for (; ; )
						{
							if (++pos == endPos)
							{
								goto breakStateloop;
							}
							c = checkChar(buf, pos);
							// Deal with overflow gracefully
							if (value < prevValue)
							{
								value = 0x110000; // Value above Unicode range but
								// within int
								// range
							}
							prevValue = value;
							/*
							 * Consume as many characters as match the range of
							 * characters given above.
							 */
							if (c >= '0' && c <= '9')
							{
								seenDigits = true;
								value *= 16;
								value += c - '0';
								continue;
							}
							else if (c >= 'A' && c <= 'F')
							{
								seenDigits = true;
								value *= 16;
								value += c - 'A' + 10;
								continue;
							}
							else if (c >= 'a' && c <= 'f')
							{
								seenDigits = true;
								value *= 16;
								value += c - 'a' + 10;
								continue;
							}
							else if (c == ';')
							{
								if (seenDigits)
								{
									if ((returnState & DATA_AND_RCDATA_MASK) == 0)
									{
										cstart = pos + 1;
									}
									state = transition(state, Tokenizer.HANDLE_NCR_VALUE, reconsume, pos);
									goto continueStateloop;
								}
								else
								{
									errNoDigitsInNCR();
									appendStrBuf(';');
									emitOrAppendStrBuf(returnState);
									if ((returnState & DATA_AND_RCDATA_MASK) == 0)
									{
										cstart = pos + 1;
									}
									state = transition(state, returnState, reconsume, pos);
									goto continueStateloop;
								}
							}
							else
							{
								/*
								 * If no characters match the range, then don't
								 * consume any characters (and unconsume the U+0023
								 * NUMBER SIGN character and, if appropriate, the X
								 * character). This is a parse error; nothing is
								 * returned.
								 * 
								 * Otherwise, if the next character is a U+003B
								 * SEMICOLON, consume that too. If it isn't, there
								 * is a parse error.
								 */
								if (!seenDigits)
								{
									errNoDigitsInNCR();
									emitOrAppendStrBuf(returnState);
									if ((returnState & DATA_AND_RCDATA_MASK) == 0)
									{
										cstart = pos;
									}
									state = transition(state, returnState, reconsume, pos);
									reconsume = true;
									goto continueStateloop;
								}
								else
								{
									errCharRefLacksSemicolon();
									if ((returnState & DATA_AND_RCDATA_MASK) == 0)
									{
										cstart = pos;
									}
									state = transition(state, Tokenizer.HANDLE_NCR_VALUE, reconsume, pos);
									reconsume = true;
									goto continueStateloop;
								}
							}
						}
					// XXX reorder point
					case PLAINTEXT:
						/*plaintextloop:*/
						for (; ; )
						{
							if (reconsume)
							{
								reconsume = false;
							}
							else
							{
								if (++pos == endPos)
								{
									goto breakStateloop;
								}
								c = checkChar(buf, pos);
							}
							switch (c)
							{
								case '\u0000':
									emitPlaintextReplacementCharacter(buf, pos);
									continue;
								case '\r':
									emitCarriageReturn(buf, pos);
									goto breakStateloop;
								case '\n':
									silentLineFeed();
									goto default;
								default:
									/*
									 * Anything else Emit the current input
									 * character as a character token. Stay in the
									 * RAWTEXT state.
									 */
									continue;
							}
						}
					// XXX reorder point
					case CLOSE_TAG_OPEN:
						if (++pos == endPos)
						{
							goto breakStateloop;
						}
						c = checkChar(buf, pos);
						/*
						 * Otherwise, if the content model flag is set to the PCDATA
						 * state, or if the next few characters do match that tag
						 * name, consume the next input character:
						 */
						switch (c)
						{
							case '>':
								/* U+003E GREATER-THAN SIGN (>) Parse error. */
								errLtSlashGt();
								/*
								 * Switch to the data state.
								 */
								cstart = pos + 1;
								state = transition(state, Tokenizer.DATA, reconsume, pos);
								goto continueStateloop;
							case '\r':
								silentCarriageReturn();
								/* Anything else Parse error. */
								errGarbageAfterLtSlash();
								/*
								 * Switch to the bogus comment state.
								 */
								clearLongStrBufAndAppend('\n');
								state = transition(state, Tokenizer.BOGUS_COMMENT, reconsume, pos);
								goto breakStateloop;
							case '\n':
								silentLineFeed();
								/* Anything else Parse error. */
								errGarbageAfterLtSlash();
								/*
								 * Switch to the bogus comment state.
								 */
								clearLongStrBufAndAppend('\n');
								state = transition(state, Tokenizer.BOGUS_COMMENT, reconsume, pos);
								goto continueStateloop;
							case '\u0000':
								c = '\uFFFD';
								// fall thru
								goto default;
							default:
								if (c >= 'A' && c <= 'Z')
								{
									c += (char)0x20;
								}
								if (c >= 'a' && c <= 'z')
								{
									/*
									 * U+0061 LATIN SMALL LETTER A through to U+007A
									 * LATIN SMALL LETTER Z Create a new end tag
									 * token,
									 */
									endTag = true;
									/*
									 * set its tag name to the input character,
									 */
									clearStrBufAndAppend(c);
									/*
									 * then switch to the tag name state. (Don't
									 * emit the token yet; further details will be
									 * filled in before it is emitted.)
									 */
									state = transition(state, Tokenizer.TAG_NAME, reconsume, pos);
									goto continueStateloop;
								}
								else
								{
									/* Anything else Parse error. */
									errGarbageAfterLtSlash();
									/*
									 * Switch to the bogus comment state.
									 */
									clearLongStrBufAndAppend(c);
									state = transition(state, Tokenizer.BOGUS_COMMENT, reconsume, pos);
									goto continueStateloop;
								}
						}
					// XXX reorder point
					case RCDATA:
						/*rcdataloop:*/
						for (; ; )
						{
							if (reconsume)
							{
								reconsume = false;
							}
							else
							{
								if (++pos == endPos)
								{
									goto breakStateloop;
								}
								c = checkChar(buf, pos);
							}
							switch (c)
							{
								case '&':
									/*
									 * U+0026 AMPERSAND (&) Switch to the character
									 * reference in RCDATA state.
									 */
									FlushChars(buf, pos);
									clearStrBufAndAppend(c);
									additional = '\u0000';
									returnState = state;
									state = transition(state, Tokenizer.CONSUME_CHARACTER_REFERENCE, reconsume, pos);
									goto continueStateloop;
								case '<':
									/*
									 * U+003C LESS-THAN SIGN (<) Switch to the
									 * RCDATA less-than sign state.
									 */
									FlushChars(buf, pos);

									returnState = state;
									state = transition(state, Tokenizer.RAWTEXT_RCDATA_LESS_THAN_SIGN, reconsume, pos);
									goto continueStateloop;
								case '\u0000':
									emitReplacementCharacter(buf, pos);
									continue;
								case '\r':
									emitCarriageReturn(buf, pos);
									goto breakStateloop;
								case '\n':
									silentLineFeed();
									goto default;
								default:
									/*
									 * Emit the current input character as a
									 * character token. Stay in the RCDATA state.
									 */
									continue;
							}
						}
					// XXX reorder point
					case RAWTEXT:
						/*rawtextloop:*/
						for (; ; )
						{
							if (reconsume)
							{
								reconsume = false;
							}
							else
							{
								if (++pos == endPos)
								{
									goto breakStateloop;
								}
								c = checkChar(buf, pos);
							}
							switch (c)
							{
								case '<':
									/*
									 * U+003C LESS-THAN SIGN (<) Switch to the
									 * RAWTEXT less-than sign state.
									 */
									FlushChars(buf, pos);

									returnState = state;
									state = transition(state, Tokenizer.RAWTEXT_RCDATA_LESS_THAN_SIGN, reconsume, pos);
									goto breakRawtextloop;
								// FALL THRU goto continueStateloop;
								case '\u0000':
									emitReplacementCharacter(buf, pos);
									continue;
								case '\r':
									emitCarriageReturn(buf, pos);
									goto breakStateloop;
								case '\n':
									silentLineFeed();
									goto default;
								default:
									/*
									 * Emit the current input character as a
									 * character token. Stay in the RAWTEXT state.
									 */
									continue;
							}
						}
					breakRawtextloop:
						goto case RAWTEXT_RCDATA_LESS_THAN_SIGN;
					// XXX fallthru don't reorder
					case RAWTEXT_RCDATA_LESS_THAN_SIGN:
						/*rawtextrcdatalessthansignloop:*/
						for (; ; )
						{
							if (++pos == endPos)
							{
								goto breakStateloop;
							}
							c = checkChar(buf, pos);
							switch (c)
							{
								case '/':
									/*
									 * U+002F SOLIDUS (/) Set the temporary buffer
									 * to the empty string. Switch to the script
									 * data end tag open state.
									 */
									index = 0;
									clearStrBuf();
									state = transition(state, Tokenizer.NON_DATA_END_TAG_NAME, reconsume, pos);
									goto breakRawtextrcdatalessthansignloop;
								// FALL THRU goto continueStateloop;
								default:
									/*
									 * Otherwise, emit a U+003C LESS-THAN SIGN
									 * character token
									 */
									TokenHandler.Characters(Tokenizer.LT_GT, 0, 1);
									/*
									 * and reconsume the current input character in
									 * the data state.
									 */
									cstart = pos;
									state = transition(state, returnState, reconsume, pos);
									reconsume = true;
									goto continueStateloop;
							}
						}
					breakRawtextrcdatalessthansignloop:
						goto case NON_DATA_END_TAG_NAME;
					// XXX fall thru. don't reorder.
					case NON_DATA_END_TAG_NAME:
						for (; ; )
						{
							if (++pos == endPos)
							{
								goto breakStateloop;
							}
							c = checkChar(buf, pos);
							/*
							 * ASSERT! when entering this state, set index to 0 and
							 * call clearStrBuf() assert (contentModelElement !=
							 * null); Let's implement the above without lookahead.
							 * strBuf is the 'temporary buffer'.
							 */
							if (index < endTagExpectationAsArray.Length)
							{
								char e = endTagExpectationAsArray[index];
								char folded = c;
								if (c >= 'A' && c <= 'Z')
								{
									folded += (char)0x20;
								}
								if (folded != e)
								{
									// [NOCPP[
									errHtml4LtSlashInRcdata(folded);
									// ]NOCPP]
									TokenHandler.Characters(Tokenizer.LT_SOLIDUS,
											0, 2);
									emitStrBuf();
									cstart = pos;
									state = transition(state, returnState, reconsume, pos);
									reconsume = true;
									goto continueStateloop;
								}
								appendStrBuf(c);
								index++;
								continue;
							}
							else
							{
								endTag = true;
								// XXX replace contentModelElement with different
								// type
								tagName = endTagExpectation;
								switch (c)
								{
									case '\r':
										silentCarriageReturn();
										state = transition(state, Tokenizer.BEFORE_ATTRIBUTE_NAME, reconsume, pos);
										goto breakStateloop;
									case '\n':
										silentLineFeed();
										goto case ' ';
									// fall thru
									case ' ':
									case '\t':
									case '\u000C':
										/*
										 * U+0009 CHARACTER TABULATION U+000A LINE
										 * FEED (LF) U+000C FORM FEED (FF) U+0020
										 * SPACE If the current end tag token is an
										 * appropriate end tag token, then switch to
										 * the before attribute name state.
										 */
										state = transition(state, Tokenizer.BEFORE_ATTRIBUTE_NAME, reconsume, pos);
										goto continueStateloop;
									case '/':
										/*
										 * U+002F SOLIDUS (/) If the current end tag
										 * token is an appropriate end tag token,
										 * then switch to the self-closing start tag
										 * state.
										 */
										state = transition(state, Tokenizer.SELF_CLOSING_START_TAG, reconsume, pos);
										goto continueStateloop;
									case '>':
										/*
										 * U+003E GREATER-THAN SIGN (>) If the
										 * current end tag token is an appropriate
										 * end tag token, then emit the current tag
										 * token and switch to the data state.
										 */
										state = transition(state, emitCurrentTagToken(false, pos), reconsume, pos);
										if (shouldSuspend)
										{
											goto breakStateloop;
										}
										goto continueStateloop;
									default:
										/*
										 * Emit a U+003C LESS-THAN SIGN character
										 * token, a U+002F SOLIDUS character token,
										 * a character token for each of the
										 * characters in the temporary buffer (in
										 * the order they were added to the buffer),
										 * and reconsume the current input character
										 * in the RAWTEXT state.
										 */
										// [NOCPP[
										errWarnLtSlashInRcdata();
										// ]NOCPP]
										TokenHandler.Characters(
												Tokenizer.LT_SOLIDUS, 0, 2);
										emitStrBuf();
										if (c == '\u0000')
										{
											emitReplacementCharacter(buf, pos);
										}
										else
										{
											cstart = pos; // don't drop the
											// character
										}
										state = transition(state, returnState, reconsume, pos);
										goto continueStateloop;
								}
							}
						}
					// XXX reorder point
					// BEGIN HOTSPOT WORKAROUND
					case BOGUS_COMMENT:
						/*boguscommentloop:*/
						for (; ; )
						{
							if (reconsume)
							{
								reconsume = false;
							}
							else
							{
								if (++pos == endPos)
								{
									goto breakStateloop;
								}
								c = checkChar(buf, pos);
							}
							/*
							 * Consume every character up to and including the first
							 * U+003E GREATER-THAN SIGN character (>) or the end of
							 * the file (EOF), whichever comes first. Emit a comment
							 * token whose data is the concatenation of all the
							 * characters starting from and including the character
							 * that caused the state machine to switch into the
							 * bogus comment state, up to and including the
							 * character immediately before the last consumed
							 * character (i.e. up to the character just before the
							 * U+003E or EOF character). (If the comment was started
							 * by the end of the file (EOF), the token is empty.)
							 * 
							 * Switch to the data state.
							 * 
							 * If the end of the file was reached, reconsume the EOF
							 * character.
							 */
							switch (c)
							{
								case '>':
									EmitComment(0, pos);
									state = transition(state, Tokenizer.DATA, reconsume, pos);
									goto continueStateloop;
								case '-':
									appendLongStrBuf(c);
									state = transition(state, Tokenizer.BOGUS_COMMENT_HYPHEN, reconsume, pos);
									goto breakBoguscommentloop;
								case '\r':
									appendLongStrBufCarriageReturn();
									goto breakStateloop;
								case '\n':
									appendLongStrBufLineFeed();
									continue;
								case '\u0000':
									c = '\uFFFD';
									// fall thru
									goto default;
								default:
									appendLongStrBuf(c);
									continue;
							}
						}
					breakBoguscommentloop:
						goto case BOGUS_COMMENT_HYPHEN;
					// FALLTHRU DON'T REORDER
					case BOGUS_COMMENT_HYPHEN:
						/*boguscommenthyphenloop:*/
						for (; ; )
						{
							if (++pos == endPos)
							{
								goto breakStateloop;
							}
							c = checkChar(buf, pos);
							switch (c)
							{
								case '>':
									// [NOCPP[
									maybeAppendSpaceToBogusComment();
									// ]NOCPP]
									EmitComment(0, pos);
									state = transition(state, Tokenizer.DATA, reconsume, pos);
									goto continueStateloop;
								case '-':
									appendSecondHyphenToBogusComment();
									goto continueBoguscommenthyphenloop;
								case '\r':
									appendLongStrBufCarriageReturn();
									state = transition(state, Tokenizer.BOGUS_COMMENT, reconsume, pos);
									goto breakStateloop;
								case '\n':
									appendLongStrBufLineFeed();
									state = transition(state, Tokenizer.BOGUS_COMMENT, reconsume, pos);
									goto continueStateloop;
								case '\u0000':
									c = '\uFFFD';
									// fall thru
									goto default;
								default:
									appendLongStrBuf(c);
									state = transition(state, Tokenizer.BOGUS_COMMENT, reconsume, pos);
									goto continueStateloop;
							}
						continueBoguscommenthyphenloop:
							continue;
						}

					// XXX reorder point
					case SCRIPT_DATA:
						/*scriptdataloop:*/
						for (; ; )
						{
							if (reconsume)
							{
								reconsume = false;
							}
							else
							{
								if (++pos == endPos)
								{
									goto breakStateloop;
								}
								c = checkChar(buf, pos);
							}
							switch (c)
							{
								case '<':
									/*
									 * U+003C LESS-THAN SIGN (<) Switch to the
									 * script data less-than sign state.
									 */
									FlushChars(buf, pos);
									returnState = state;
									state = transition(state, Tokenizer.SCRIPT_DATA_LESS_THAN_SIGN, reconsume, pos);
									goto breakScriptdataloop; // FALL THRU continue
								// stateloop;
								case '\u0000':
									emitReplacementCharacter(buf, pos);
									continue;
								case '\r':
									emitCarriageReturn(buf, pos);
									goto breakStateloop;
								case '\n':
									silentLineFeed();
									goto default;
								default:
									/*
									 * Anything else Emit the current input
									 * character as a character token. Stay in the
									 * script data state.
									 */
									continue;
							}
						}
					breakScriptdataloop:
						goto case SCRIPT_DATA_LESS_THAN_SIGN;
					// WARNING FALLTHRU CASE TRANSITION: DON'T REORDER
					case SCRIPT_DATA_LESS_THAN_SIGN:
						/*scriptdatalessthansignloop:*/
						for (; ; )
						{
							if (++pos == endPos)
							{
								goto breakStateloop;
							}
							c = checkChar(buf, pos);
							switch (c)
							{
								case '/':
									/*
									 * U+002F SOLIDUS (/) Set the temporary buffer
									 * to the empty string. Switch to the script
									 * data end tag open state.
									 */
									index = 0;
									clearStrBuf();
									state = transition(state, Tokenizer.NON_DATA_END_TAG_NAME, reconsume, pos);
									goto continueStateloop;
								case '!':
									TokenHandler.Characters(Tokenizer.LT_GT, 0, 1);
									cstart = pos;
									state = transition(state, Tokenizer.SCRIPT_DATA_ESCAPE_START, reconsume, pos);
									goto breakScriptdatalessthansignloop; // FALL THRU
								// continue
								// stateloop;
								default:
									/*
									 * Otherwise, emit a U+003C LESS-THAN SIGN
									 * character token
									 */
									TokenHandler.Characters(Tokenizer.LT_GT, 0, 1);
									/*
									 * and reconsume the current input character in
									 * the data state.
									 */
									cstart = pos;
									state = transition(state, Tokenizer.SCRIPT_DATA, reconsume, pos);
									reconsume = true;
									goto continueStateloop;
							}
						}
					breakScriptdatalessthansignloop:
						goto case SCRIPT_DATA_ESCAPE_START;
					// WARNING FALLTHRU CASE TRANSITION: DON'T REORDER
					case SCRIPT_DATA_ESCAPE_START:
						/*scriptdataescapestartloop:*/
						for (; ; )
						{
							if (++pos == endPos)
							{
								goto breakStateloop;
							}
							c = checkChar(buf, pos);
							/*
							 * Consume the next input character:
							 */
							switch (c)
							{
								case '-':
									/*
									 * U+002D HYPHEN-MINUS (-) Emit a U+002D
									 * HYPHEN-MINUS character token. Switch to the
									 * script data escape start dash state.
									 */
									state = transition(state, Tokenizer.SCRIPT_DATA_ESCAPE_START_DASH, reconsume, pos);
									goto breakScriptdataescapestartloop; // FALL THRU
								// continue
								// stateloop;
								default:
									/*
									 * Anything else Reconsume the current input
									 * character in the script data state.
									 */
									state = transition(state, Tokenizer.SCRIPT_DATA, reconsume, pos);
									reconsume = true;
									goto continueStateloop;
							}
						}
					breakScriptdataescapestartloop:
						goto case SCRIPT_DATA_ESCAPE_START_DASH;
					// WARNING FALLTHRU CASE TRANSITION: DON'T REORDER
					case SCRIPT_DATA_ESCAPE_START_DASH:
						/*scriptdataescapestartdashloop:*/
						for (; ; )
						{
							if (++pos == endPos)
							{
								goto breakStateloop;
							}
							c = checkChar(buf, pos);
							/*
							 * Consume the next input character:
							 */
							switch (c)
							{
								case '-':
									/*
									 * U+002D HYPHEN-MINUS (-) Emit a U+002D
									 * HYPHEN-MINUS character token. Switch to the
									 * script data escaped dash dash state.
									 */
									state = transition(state, Tokenizer.SCRIPT_DATA_ESCAPED_DASH_DASH, reconsume, pos);
									goto breakScriptdataescapestartdashloop;
								// goto continueStateloop;
								default:
									/*
									 * Anything else Reconsume the current input
									 * character in the script data state.
									 */
									state = transition(state, Tokenizer.SCRIPT_DATA, reconsume, pos);
									reconsume = true;
									goto continueStateloop;
							}
						}
					breakScriptdataescapestartdashloop:
						goto case SCRIPT_DATA_ESCAPED_DASH_DASH;
					// WARNING FALLTHRU CASE TRANSITION: DON'T REORDER
					case SCRIPT_DATA_ESCAPED_DASH_DASH:
						/*scriptdataescapeddashdashloop:*/
						for (; ; )
						{
							if (++pos == endPos)
							{
								goto breakStateloop;
							}
							c = checkChar(buf, pos);
							/*
							 * Consume the next input character:
							 */
							switch (c)
							{
								case '-':
									/*
									 * U+002D HYPHEN-MINUS (-) Emit a U+002D
									 * HYPHEN-MINUS character token. Stay in the
									 * script data escaped dash dash state.
									 */
									continue;
								case '<':
									/*
									 * U+003C LESS-THAN SIGN (<) Switch to the
									 * script data escaped less-than sign state.
									 */
									FlushChars(buf, pos);
									state = transition(state, Tokenizer.SCRIPT_DATA_ESCAPED_LESS_THAN_SIGN, reconsume, pos);
									goto continueStateloop;
								case '>':
									/*
									 * U+003E GREATER-THAN SIGN (>) Emit a U+003E
									 * GREATER-THAN SIGN character token. Switch to
									 * the script data state.
									 */
									state = transition(state, Tokenizer.SCRIPT_DATA, reconsume, pos);
									goto continueStateloop;
								case '\u0000':
									emitReplacementCharacter(buf, pos);
									state = transition(state, Tokenizer.SCRIPT_DATA_ESCAPED, reconsume, pos);
									goto breakScriptdataescapeddashdashloop;
								case '\r':
									emitCarriageReturn(buf, pos);
									state = transition(state, Tokenizer.SCRIPT_DATA_ESCAPED, reconsume, pos);
									goto breakStateloop;
								case '\n':
									silentLineFeed();
									goto default;
								default:
									/*
									 * Anything else Emit the current input
									 * character as a character token. Switch to the
									 * script data escaped state.
									 */
									state = transition(state, Tokenizer.SCRIPT_DATA_ESCAPED, reconsume, pos);
									goto breakScriptdataescapeddashdashloop;
								// goto continueStateloop;
							}
						}
					breakScriptdataescapeddashdashloop:
						goto case SCRIPT_DATA_ESCAPED;
					// WARNING FALLTHRU CASE TRANSITION: DON'T REORDER
					case SCRIPT_DATA_ESCAPED:
						/*scriptdataescapedloop:*/
						for (; ; )
						{
							if (reconsume)
							{
								reconsume = false;
							}
							else
							{
								if (++pos == endPos)
								{
									goto breakStateloop;
								}
								c = checkChar(buf, pos);
							}
							/*
							 * Consume the next input character:
							 */
							switch (c)
							{
								case '-':
									/*
									 * U+002D HYPHEN-MINUS (-) Emit a U+002D
									 * HYPHEN-MINUS character token. Switch to the
									 * script data escaped dash state.
									 */
									state = transition(state, Tokenizer.SCRIPT_DATA_ESCAPED_DASH, reconsume, pos);
									goto breakScriptdataescapedloop; // FALL THRU
								// continue
								// stateloop;
								case '<':
									/*
									 * U+003C LESS-THAN SIGN (<) Switch to the
									 * script data escaped less-than sign state.
									 */
									FlushChars(buf, pos);
									state = transition(state, Tokenizer.SCRIPT_DATA_ESCAPED_LESS_THAN_SIGN, reconsume, pos);
									goto continueStateloop;
								case '\u0000':
									emitReplacementCharacter(buf, pos);
									continue;
								case '\r':
									emitCarriageReturn(buf, pos);
									goto breakStateloop;
								case '\n':
									silentLineFeed();
									goto default;
								default:
									/*
									 * Anything else Emit the current input
									 * character as a character token. Stay in the
									 * script data escaped state.
									 */
									continue;
							}
						}
					breakScriptdataescapedloop:
						goto case SCRIPT_DATA_ESCAPED_DASH;
					// WARNING FALLTHRU CASE TRANSITION: DON'T REORDER
					case SCRIPT_DATA_ESCAPED_DASH:
						/*scriptdataescapeddashloop:*/
						for (; ; )
						{
							if (++pos == endPos)
							{
								goto breakStateloop;
							}
							c = checkChar(buf, pos);
							/*
							 * Consume the next input character:
							 */
							switch (c)
							{
								case '-':
									/*
									 * U+002D HYPHEN-MINUS (-) Emit a U+002D
									 * HYPHEN-MINUS character token. Switch to the
									 * script data escaped dash dash state.
									 */
									state = transition(state, Tokenizer.SCRIPT_DATA_ESCAPED_DASH_DASH, reconsume, pos);
									goto continueStateloop;
								case '<':
									/*
									 * U+003C LESS-THAN SIGN (<) Switch to the
									 * script data escaped less-than sign state.
									 */
									FlushChars(buf, pos);
									state = transition(state, Tokenizer.SCRIPT_DATA_ESCAPED_LESS_THAN_SIGN, reconsume, pos);
									goto breakScriptdataescapeddashloop;
								// goto continueStateloop;
								case '\u0000':
									emitReplacementCharacter(buf, pos);
									state = transition(state, Tokenizer.SCRIPT_DATA_ESCAPED, reconsume, pos);
									goto continueStateloop;
								case '\r':
									emitCarriageReturn(buf, pos);
									state = transition(state, Tokenizer.SCRIPT_DATA_ESCAPED, reconsume, pos);
									goto breakStateloop;
								case '\n':
									silentLineFeed();
									goto default;
								default:
									/*
									 * Anything else Emit the current input
									 * character as a character token. Switch to the
									 * script data escaped state.
									 */
									state = transition(state, Tokenizer.SCRIPT_DATA_ESCAPED, reconsume, pos);
									goto continueStateloop;
							}
						}
					breakScriptdataescapeddashloop:
						goto case SCRIPT_DATA_ESCAPED_LESS_THAN_SIGN;
					// WARNING FALLTHRU CASE TRANSITION: DON'T REORDER
					case SCRIPT_DATA_ESCAPED_LESS_THAN_SIGN:
						/*scriptdataescapedlessthanloop:*/
						for (; ; )
						{
							if (++pos == endPos)
							{
								goto breakStateloop;
							}
							c = checkChar(buf, pos);
							/*
							 * Consume the next input character:
							 */
							switch (c)
							{
								case '/':
									/*
									 * U+002F SOLIDUS (/) Set the temporary buffer
									 * to the empty string. Switch to the script
									 * data escaped end tag open state.
									 */
									index = 0;
									clearStrBuf();
									returnState = Tokenizer.SCRIPT_DATA_ESCAPED;
									state = transition(state, Tokenizer.NON_DATA_END_TAG_NAME, reconsume, pos);
									goto continueStateloop;
								case 'S':
								case 's':
									/*
									 * U+0041 LATIN CAPITAL LETTER A through to
									 * U+005A LATIN CAPITAL LETTER Z Emit a U+003C
									 * LESS-THAN SIGN character token and the
									 * current input character as a character token.
									 */
									TokenHandler.Characters(Tokenizer.LT_GT, 0, 1);
									cstart = pos;
									index = 1;
									/*
									 * Set the temporary buffer to the empty string.
									 * Append the lowercase version of the current
									 * input character (add 0x0020 to the
									 * character's code point) to the temporary
									 * buffer. Switch to the script data double
									 * escape start state.
									 */
									state = transition(state, Tokenizer.SCRIPT_DATA_DOUBLE_ESCAPE_START, reconsume, pos);
									goto breakScriptdataescapedlessthanloop;
								// goto continueStateloop;
								default:
									/*
									 * Anything else Emit a U+003C LESS-THAN SIGN
									 * character token and reconsume the current
									 * input character in the script data escaped
									 * state.
									 */
									TokenHandler.Characters(Tokenizer.LT_GT, 0, 1);
									cstart = pos;
									reconsume = true;
									state = transition(state, Tokenizer.SCRIPT_DATA_ESCAPED, reconsume, pos);
									goto continueStateloop;
							}
						}
					breakScriptdataescapedlessthanloop:
						goto case SCRIPT_DATA_DOUBLE_ESCAPE_START;
					// WARNING FALLTHRU CASE TRANSITION: DON'T REORDER
					case SCRIPT_DATA_DOUBLE_ESCAPE_START:
						/*scriptdatadoubleescapestartloop:*/
						for (; ; )
						{
							if (++pos == endPos)
							{
								goto breakStateloop;
							}
							c = checkChar(buf, pos);
							Debug.Assert(index > 0);
							if (index < 6)
							{ // SCRIPT_ARR.Length
								char folded = c;
								if (c >= 'A' && c <= 'Z')
								{
									folded += (char)0x20;
								}
								if (folded != Tokenizer.SCRIPT_ARR[index])
								{
									reconsume = true;
									state = transition(state, Tokenizer.SCRIPT_DATA_ESCAPED, reconsume, pos);
									goto continueStateloop;
								}
								index++;
								continue;
							}
							switch (c)
							{
								case '\r':
									emitCarriageReturn(buf, pos);
									state = transition(state, Tokenizer.SCRIPT_DATA_DOUBLE_ESCAPED, reconsume, pos);
									goto breakStateloop;
								case '\n':
									silentLineFeed();
									goto case ' ';
								case ' ':
								case '\t':
								case '\u000C':
								case '/':
								case '>':
									/*
									 * U+0009 CHARACTER TABULATION U+000A LINE FEED
									 * (LF) U+000C FORM FEED (FF) U+0020 SPACE
									 * U+002F SOLIDUS (/) U+003E GREATER-THAN SIGN
									 * (>) Emit the current input character as a
									 * character token. If the temporary buffer is
									 * the string "script", then switch to the
									 * script data double escaped state.
									 */
									state = transition(state, Tokenizer.SCRIPT_DATA_DOUBLE_ESCAPED, reconsume, pos);
									goto breakScriptdatadoubleescapestartloop;
								// goto continueStateloop;
								default:
									/*
									 * Anything else Reconsume the current input
									 * character in the script data escaped state.
									 */
									reconsume = true;
									state = transition(state, Tokenizer.SCRIPT_DATA_ESCAPED, reconsume, pos);
									goto continueStateloop;
							}
						}
					breakScriptdatadoubleescapestartloop:
						goto case SCRIPT_DATA_DOUBLE_ESCAPED;
					// WARNING FALLTHRU CASE TRANSITION: DON'T REORDER
					case SCRIPT_DATA_DOUBLE_ESCAPED:
						/*scriptdatadoubleescapedloop:*/
						for (; ; )
						{
							if (reconsume)
							{
								reconsume = false;
							}
							else
							{
								if (++pos == endPos)
								{
									goto breakStateloop;
								}
								c = checkChar(buf, pos);
							}
							/*
							 * Consume the next input character:
							 */
							switch (c)
							{
								case '-':
									/*
									 * U+002D HYPHEN-MINUS (-) Emit a U+002D
									 * HYPHEN-MINUS character token. Switch to the
									 * script data double escaped dash state.
									 */
									state = transition(state, Tokenizer.SCRIPT_DATA_DOUBLE_ESCAPED_DASH, reconsume, pos);
									goto breakScriptdatadoubleescapedloop; // FALL THRU
								// continue
								// stateloop;
								case '<':
									/*
									 * U+003C LESS-THAN SIGN (<) Emit a U+003C
									 * LESS-THAN SIGN character token. Switch to the
									 * script data double escaped less-than sign
									 * state.
									 */
									state = transition(state, Tokenizer.SCRIPT_DATA_DOUBLE_ESCAPED_LESS_THAN_SIGN, reconsume, pos);
									goto continueStateloop;
								case '\u0000':
									emitReplacementCharacter(buf, pos);
									continue;
								case '\r':
									emitCarriageReturn(buf, pos);
									goto breakStateloop;
								case '\n':
									silentLineFeed();
									goto default;
								default:
									/*
									 * Anything else Emit the current input
									 * character as a character token. Stay in the
									 * script data double escaped state.
									 */
									continue;
							}
						}
					breakScriptdatadoubleescapedloop:
						goto case SCRIPT_DATA_DOUBLE_ESCAPED_DASH;
					// WARNING FALLTHRU CASE TRANSITION: DON'T REORDER
					case SCRIPT_DATA_DOUBLE_ESCAPED_DASH:
						/*scriptdatadoubleescapeddashloop:*/
						for (; ; )
						{
							if (++pos == endPos)
							{
								goto breakStateloop;
							}
							c = checkChar(buf, pos);
							/*
							 * Consume the next input character:
							 */
							switch (c)
							{
								case '-':
									/*
									 * U+002D HYPHEN-MINUS (-) Emit a U+002D
									 * HYPHEN-MINUS character token. Switch to the
									 * script data double escaped dash dash state.
									 */
									state = transition(state, Tokenizer.SCRIPT_DATA_DOUBLE_ESCAPED_DASH_DASH, reconsume, pos);
									goto breakScriptdatadoubleescapeddashloop;
								// goto continueStateloop;
								case '<':
									/*
									 * U+003C LESS-THAN SIGN (<) Emit a U+003C
									 * LESS-THAN SIGN character token. Switch to the
									 * script data double escaped less-than sign
									 * state.
									 */
									state = transition(state, Tokenizer.SCRIPT_DATA_DOUBLE_ESCAPED_LESS_THAN_SIGN, reconsume, pos);
									goto continueStateloop;
								case '\u0000':
									emitReplacementCharacter(buf, pos);
									state = transition(state, Tokenizer.SCRIPT_DATA_DOUBLE_ESCAPED, reconsume, pos);
									goto continueStateloop;
								case '\r':
									emitCarriageReturn(buf, pos);
									state = transition(state, Tokenizer.SCRIPT_DATA_DOUBLE_ESCAPED, reconsume, pos);
									goto breakStateloop;
								case '\n':
									silentLineFeed();
									goto default;
								default:
									/*
									 * Anything else Emit the current input
									 * character as a character token. Switch to the
									 * script data double escaped state.
									 */
									state = transition(state, Tokenizer.SCRIPT_DATA_DOUBLE_ESCAPED, reconsume, pos);
									goto continueStateloop;
							}
						}
					breakScriptdatadoubleescapeddashloop:
						goto case SCRIPT_DATA_DOUBLE_ESCAPED_DASH_DASH;
					// WARNING FALLTHRU CASE TRANSITION: DON'T REORDER
					case SCRIPT_DATA_DOUBLE_ESCAPED_DASH_DASH:
						/*scriptdatadoubleescapeddashdashloop:*/
						for (; ; )
						{
							if (++pos == endPos)
							{
								goto breakStateloop;
							}
							c = checkChar(buf, pos);
							/*
							 * Consume the next input character:
							 */
							switch (c)
							{
								case '-':
									/*
									 * U+002D HYPHEN-MINUS (-) Emit a U+002D
									 * HYPHEN-MINUS character token. Stay in the
									 * script data double escaped dash dash state.
									 */
									continue;
								case '<':
									/*
									 * U+003C LESS-THAN SIGN (<) Emit a U+003C
									 * LESS-THAN SIGN character token. Switch to the
									 * script data double escaped less-than sign
									 * state.
									 */
									state = transition(state, Tokenizer.SCRIPT_DATA_DOUBLE_ESCAPED_LESS_THAN_SIGN, reconsume, pos);
									goto breakScriptdatadoubleescapeddashdashloop;
								case '>':
									/*
									 * U+003E GREATER-THAN SIGN (>) Emit a U+003E
									 * GREATER-THAN SIGN character token. Switch to
									 * the script data state.
									 */
									state = transition(state, Tokenizer.SCRIPT_DATA, reconsume, pos);
									goto continueStateloop;
								case '\u0000':
									emitReplacementCharacter(buf, pos);
									state = transition(state, Tokenizer.SCRIPT_DATA_DOUBLE_ESCAPED, reconsume, pos);
									goto continueStateloop;
								case '\r':
									emitCarriageReturn(buf, pos);
									state = transition(state, Tokenizer.SCRIPT_DATA_DOUBLE_ESCAPED, reconsume, pos);
									goto breakStateloop;
								case '\n':
									silentLineFeed();
									goto default;
								default:
									/*
									 * Anything else Emit the current input
									 * character as a character token. Switch to the
									 * script data double escaped state.
									 */
									state = transition(state, Tokenizer.SCRIPT_DATA_DOUBLE_ESCAPED, reconsume, pos);
									goto continueStateloop;
							}
						}
					breakScriptdatadoubleescapeddashdashloop:
						goto case SCRIPT_DATA_DOUBLE_ESCAPED_LESS_THAN_SIGN;
					// WARNING FALLTHRU CASE TRANSITION: DON'T REORDER
					case SCRIPT_DATA_DOUBLE_ESCAPED_LESS_THAN_SIGN:
						/*scriptdatadoubleescapedlessthanloop:*/
						for (; ; )
						{
							if (++pos == endPos)
							{
								goto breakStateloop;
							}
							c = checkChar(buf, pos);
							/*
							 * Consume the next input character:
							 */
							switch (c)
							{
								case '/':
									/*
									 * U+002F SOLIDUS (/) Emit a U+002F SOLIDUS
									 * character token. Set the temporary buffer to
									 * the empty string. Switch to the script data
									 * double escape end state.
									 */
									index = 0;
									state = transition(state, Tokenizer.SCRIPT_DATA_DOUBLE_ESCAPE_END, reconsume, pos);
									goto breakScriptdatadoubleescapedlessthanloop;
								default:
									/*
									 * Anything else Reconsume the current input
									 * character in the script data double escaped
									 * state.
									 */
									reconsume = true;
									state = transition(state, Tokenizer.SCRIPT_DATA_DOUBLE_ESCAPED, reconsume, pos);
									goto continueStateloop;
							}
						}
					breakScriptdatadoubleescapedlessthanloop:
						goto case SCRIPT_DATA_DOUBLE_ESCAPE_END;
					// WARNING FALLTHRU CASE TRANSITION: DON'T REORDER
					case SCRIPT_DATA_DOUBLE_ESCAPE_END:
						/*scriptdatadoubleescapeendloop:*/
						for (; ; )
						{
							if (++pos == endPos)
							{
								goto breakStateloop;
							}
							c = checkChar(buf, pos);
							if (index < 6)
							{ // SCRIPT_ARR.Length
								char folded = c;
								if (c >= 'A' && c <= 'Z')
								{
									folded += (char)0x20;
								}
								if (folded != Tokenizer.SCRIPT_ARR[index])
								{
									reconsume = true;
									state = transition(state, Tokenizer.SCRIPT_DATA_DOUBLE_ESCAPED, reconsume, pos);
									goto continueStateloop;
								}
								index++;
								continue;
							}
							switch (c)
							{
								case '\r':
									emitCarriageReturn(buf, pos);
									state = transition(state, Tokenizer.SCRIPT_DATA_ESCAPED, reconsume, pos);
									goto breakStateloop;
								case '\n':
									silentLineFeed();
									goto case ' ';
								case ' ':
								case '\t':
								case '\u000C':
								case '/':
								case '>':
									/*
									 * U+0009 CHARACTER TABULATION U+000A LINE FEED
									 * (LF) U+000C FORM FEED (FF) U+0020 SPACE
									 * U+002F SOLIDUS (/) U+003E GREATER-THAN SIGN
									 * (>) Emit the current input character as a
									 * character token. If the temporary buffer is
									 * the string "script", then switch to the
									 * script data escaped state.
									 */
									state = transition(state, Tokenizer.SCRIPT_DATA_ESCAPED, reconsume, pos);
									goto continueStateloop;
								default:
									/*
									 * Reconsume the current input character in the
									 * script data double escaped state.
									 */
									reconsume = true;
									state = transition(state, Tokenizer.SCRIPT_DATA_DOUBLE_ESCAPED, reconsume, pos);
									goto continueStateloop;
							}
						}

					// XXX reorder point
					case MARKUP_DECLARATION_OCTYPE:
						/*markupdeclarationdoctypeloop:*/
						for (; ; )
						{
							if (++pos == endPos)
							{
								goto breakStateloop;
							}
							c = checkChar(buf, pos);
							if (index < 6)
							{ // OCTYPE.Length
								char folded = c;
								if (c >= 'A' && c <= 'Z')
								{
									folded += (char)0x20;
								}
								if (folded == Tokenizer.OCTYPE[index])
								{
									appendLongStrBuf(c);
								}
								else
								{
									errBogusComment();
									state = transition(state, Tokenizer.BOGUS_COMMENT, reconsume, pos);
									reconsume = true;
									goto continueStateloop;
								}
								index++;
								continue;
							}
							else
							{
								state = transition(state, Tokenizer.DOCTYPE, reconsume, pos);
								reconsume = true;
								goto breakMarkupdeclarationdoctypeloop;
								// goto continueStateloop;
							}
						}
					breakMarkupdeclarationdoctypeloop:
						goto case DOCTYPE;
					// FALLTHRU DON'T REORDER
					case DOCTYPE:
						/*doctypeloop:*/
						for (; ; )
						{
							if (reconsume)
							{
								reconsume = false;
							}
							else
							{
								if (++pos == endPos)
								{
									goto breakStateloop;
								}
								c = checkChar(buf, pos);
							}
							initDoctypeFields();
							/*
							 * Consume the next input character:
							 */
							switch (c)
							{
								case '\r':
									silentCarriageReturn();
									state = transition(state, Tokenizer.BEFORE_DOCTYPE_NAME, reconsume, pos);
									goto breakStateloop;
								case '\n':
									silentLineFeed();
									goto case ' ';
								// fall thru
								case ' ':
								case '\t':
								case '\u000C':
									/*
									 * U+0009 CHARACTER TABULATION U+000A LINE FEED
									 * (LF) U+000C FORM FEED (FF) U+0020 SPACE
									 * Switch to the before DOCTYPE name state.
									 */
									state = transition(state, Tokenizer.BEFORE_DOCTYPE_NAME, reconsume, pos);
									goto breakDoctypeloop;
								// goto continueStateloop;
								default:
									/*
									 * Anything else Parse error.
									 */
									errMissingSpaceBeforeDoctypeName();
									/*
									 * Reconsume the current character in the before
									 * DOCTYPE name state.
									 */
									state = transition(state, Tokenizer.BEFORE_DOCTYPE_NAME, reconsume, pos);
									reconsume = true;
									goto breakDoctypeloop;
								// goto continueStateloop;
							}
						}
					breakDoctypeloop:
						goto case BEFORE_DOCTYPE_NAME;
					// FALLTHRU DON'T REORDER
					case BEFORE_DOCTYPE_NAME:
						/*beforedoctypenameloop:*/
						for (; ; )
						{
							if (reconsume)
							{
								reconsume = false;
							}
							else
							{
								if (++pos == endPos)
								{
									goto breakStateloop;
								}
								c = checkChar(buf, pos);
							}
							/*
							 * Consume the next input character:
							 */
							switch (c)
							{
								case '\r':
									silentCarriageReturn();
									goto breakStateloop;
								case '\n':
									silentLineFeed();
									// fall thru
									goto case ' ';
								case ' ':
								case '\t':
								case '\u000C':
									/*
									 * U+0009 CHARACTER TABULATION U+000A LINE FEED
									 * (LF) U+000C FORM FEED (FF) U+0020 SPACE Stay
									 * in the before DOCTYPE name state.
									 */
									continue;
								case '>':
									/*
									 * U+003E GREATER-THAN SIGN (>) Parse error.
									 */
									errNamelessDoctype();
									/*
									 * Create a new DOCTYPE token. Set its
									 * force-quirks flag to on.
									 */
									forceQuirks = true;
									/*
									 * Emit the token.
									 */
									EmitDoctypeToken(pos);
									/*
									 * Switch to the data state.
									 */
									state = transition(state, Tokenizer.DATA, reconsume, pos);
									goto continueStateloop;
								case '\u0000':
									c = '\uFFFD';
									// fall thru
									goto default;
								default:
									if (c >= 'A' && c <= 'Z')
									{
										/*
										 * U+0041 LATIN CAPITAL LETTER A through to
										 * U+005A LATIN CAPITAL LETTER Z Create a
										 * new DOCTYPE token. Set the token's name
										 * to the lowercase version of the input
										 * character (add 0x0020 to the character's
										 * code point).
										 */
										c += (char)0x20;
									}
									/* Anything else Create a new DOCTYPE token. */
									/*
									 * Set the token's name name to the current
									 * input character.
									 */
									clearStrBufAndAppend(c);
									/*
									 * Switch to the DOCTYPE name state.
									 */
									state = transition(state, Tokenizer.DOCTYPE_NAME, reconsume, pos);
									goto breakBeforedoctypenameloop;
								// goto continueStateloop;
							}
						}
					breakBeforedoctypenameloop:
						goto case DOCTYPE_NAME;
					// FALLTHRU DON'T REORDER
					case DOCTYPE_NAME:
						/*doctypenameloop:*/
						for (; ; )
						{
							if (++pos == endPos)
							{
								goto breakStateloop;
							}
							c = checkChar(buf, pos);
							/*
							 * Consume the next input character:
							 */
							switch (c)
							{
								case '\r':
									silentCarriageReturn();
									strBufToDoctypeName();
									state = transition(state, Tokenizer.AFTER_DOCTYPE_NAME, reconsume, pos);
									goto breakStateloop;
								case '\n':
									silentLineFeed();
									// fall thru
									goto case ' ';
								case ' ':
								case '\t':
								case '\u000C':
									/*
									 * U+0009 CHARACTER TABULATION U+000A LINE FEED
									 * (LF) U+000C FORM FEED (FF) U+0020 SPACE
									 * Switch to the after DOCTYPE name state.
									 */
									strBufToDoctypeName();
									state = transition(state, Tokenizer.AFTER_DOCTYPE_NAME, reconsume, pos);
									goto breakDoctypenameloop;
								// goto continueStateloop;
								case '>':
									/*
									 * U+003E GREATER-THAN SIGN (>) Emit the current
									 * DOCTYPE token.
									 */
									strBufToDoctypeName();
									EmitDoctypeToken(pos);
									/*
									 * Switch to the data state.
									 */
									state = transition(state, Tokenizer.DATA, reconsume, pos);
									goto continueStateloop;
								case '\u0000':
									c = '\uFFFD';
									// fall thru
									goto default;
								default:
									/*
									 * U+0041 LATIN CAPITAL LETTER A through to
									 * U+005A LATIN CAPITAL LETTER Z Append the
									 * lowercase version of the input character (add
									 * 0x0020 to the character's code point) to the
									 * current DOCTYPE token's name.
									 */
									if (c >= 'A' && c <= 'Z')
									{
										c += (char)0x0020;
									}
									/*
									 * Anything else Append the current input
									 * character to the current DOCTYPE token's
									 * name.
									 */
									appendStrBuf(c);
									/*
									 * Stay in the DOCTYPE name state.
									 */
									continue;
							}
						}
					breakDoctypenameloop:
						goto case AFTER_DOCTYPE_NAME;
					// FALLTHRU DON'T REORDER
					case AFTER_DOCTYPE_NAME:
						/*afterdoctypenameloop:*/
						for (; ; )
						{
							if (++pos == endPos)
							{
								goto breakStateloop;
							}
							c = checkChar(buf, pos);
							/*
							 * Consume the next input character:
							 */
							switch (c)
							{
								case '\r':
									silentCarriageReturn();
									goto breakStateloop;
								case '\n':
									silentLineFeed();
									// fall thru
									goto case ' ';
								case ' ':
								case '\t':
								case '\u000C':
									/*
									 * U+0009 CHARACTER TABULATION U+000A LINE FEED
									 * (LF) U+000C FORM FEED (FF) U+0020 SPACE Stay
									 * in the after DOCTYPE name state.
									 */
									continue;
								case '>':
									/*
									 * U+003E GREATER-THAN SIGN (>) Emit the current
									 * DOCTYPE token.
									 */
									EmitDoctypeToken(pos);
									/*
									 * Switch to the data state.
									 */
									state = transition(state, Tokenizer.DATA, reconsume, pos);
									goto continueStateloop;
								case 'p':
								case 'P':
									index = 0;
									state = transition(state, Tokenizer.DOCTYPE_UBLIC, reconsume, pos);
									goto breakAfterdoctypenameloop;
								// goto continueStateloop;
								case 's':
								case 'S':
									index = 0;
									state = transition(state, Tokenizer.DOCTYPE_YSTEM, reconsume, pos);
									goto continueStateloop;
								default:
									/*
									 * Otherwise, this is the parse error.
									 */
									bogusDoctype();

									/*
									 * Set the DOCTYPE token's force-quirks flag to
									 * on.
									 */
									// done by bogusDoctype();
									/*
									 * Switch to the bogus DOCTYPE state.
									 */
									state = transition(state, Tokenizer.BOGUS_DOCTYPE, reconsume, pos);
									goto continueStateloop;
							}
						}
					breakAfterdoctypenameloop:
						goto case DOCTYPE_UBLIC;
					// FALLTHRU DON'T REORDER
					case DOCTYPE_UBLIC:
						/*doctypeublicloop:*/
						for (; ; )
						{
							if (++pos == endPos)
							{
								goto breakStateloop;
							}
							c = checkChar(buf, pos);
							/*
							 * If the six characters starting from the current input
							 * character are an ASCII case-insensitive match for the
							 * word "PUBLIC", then consume those characters and
							 * switch to the before DOCTYPE public identifier state.
							 */
							if (index < 5)
							{ // UBLIC.Length
								char folded = c;
								if (c >= 'A' && c <= 'Z')
								{
									folded += (char)0x20;
								}
								if (folded != Tokenizer.UBLIC[index])
								{
									bogusDoctype();
									// forceQuirks = true;
									state = transition(state, Tokenizer.BOGUS_DOCTYPE, reconsume, pos);
									reconsume = true;
									goto continueStateloop;
								}
								index++;
								continue;
							}
							else
							{
								state = transition(state, Tokenizer.AFTER_DOCTYPE_PUBLIC_KEYWORD, reconsume, pos);
								reconsume = true;
								goto breakDoctypeublicloop;
								// goto continueStateloop;
							}
						}
					breakDoctypeublicloop:
						goto case AFTER_DOCTYPE_PUBLIC_KEYWORD;
					// FALLTHRU DON'T REORDER
					case AFTER_DOCTYPE_PUBLIC_KEYWORD:
						/*afterdoctypepublickeywordloop:*/
						for (; ; )
						{
							if (reconsume)
							{
								reconsume = false;
							}
							else
							{
								if (++pos == endPos)
								{
									goto breakStateloop;
								}
								c = checkChar(buf, pos);
							}
							/*
							 * Consume the next input character:
							 */
							switch (c)
							{
								case '\r':
									silentCarriageReturn();
									state = transition(state, Tokenizer.BEFORE_DOCTYPE_PUBLIC_IDENTIFIER, reconsume, pos);
									goto breakStateloop;
								case '\n':
									silentLineFeed();
									// fall thru
									goto case ' ';
								case ' ':
								case '\t':
								case '\u000C':
									/*
									 * U+0009 CHARACTER TABULATION U+000A LINE FEED
									 * (LF) U+000C FORM FEED (FF) U+0020 SPACE
									 * Switch to the before DOCTYPE public
									 * identifier state.
									 */
									state = transition(state, Tokenizer.BEFORE_DOCTYPE_PUBLIC_IDENTIFIER, reconsume, pos);
									goto breakAfterdoctypepublickeywordloop;
								// FALL THROUGH continue stateloop
								case '"':
									/*
									 * U+0022 QUOTATION MARK (") Parse Error.
									 */
									errNoSpaceBetweenDoctypePublicKeywordAndQuote();
									/*
									 * Set the DOCTYPE token's public identifier to
									 * the empty string (not missing),
									 */
									clearLongStrBuf();
									/*
									 * then switch to the DOCTYPE public identifier
									 * (double-quoted) state.
									 */
									state = transition(state, Tokenizer.DOCTYPE_PUBLIC_IDENTIFIER_DOUBLE_QUOTED, reconsume, pos);
									goto continueStateloop;
								case '\'':
									/*
									 * U+0027 APOSTROPHE (') Parse Error.
									 */
									errNoSpaceBetweenDoctypePublicKeywordAndQuote();
									/*
									 * Set the DOCTYPE token's public identifier to
									 * the empty string (not missing),
									 */
									clearLongStrBuf();
									/*
									 * then switch to the DOCTYPE public identifier
									 * (single-quoted) state.
									 */
									state = transition(state, Tokenizer.DOCTYPE_PUBLIC_IDENTIFIER_SINGLE_QUOTED, reconsume, pos);
									goto continueStateloop;
								case '>':
									/* U+003E GREATER-THAN SIGN (>) Parse error. */
									errExpectedPublicId();
									/*
									 * Set the DOCTYPE token's force-quirks flag to
									 * on.
									 */
									forceQuirks = true;
									/*
									 * Emit that DOCTYPE token.
									 */
									EmitDoctypeToken(pos);
									/*
									 * Switch to the data state.
									 */
									state = transition(state, Tokenizer.DATA, reconsume, pos);
									goto continueStateloop;
								default:
									bogusDoctype();
									/*
									 * Set the DOCTYPE token's force-quirks flag to
									 * on.
									 */
									// done by bogusDoctype();
									/*
									 * Switch to the bogus DOCTYPE state.
									 */
									state = transition(state, Tokenizer.BOGUS_DOCTYPE, reconsume, pos);
									goto continueStateloop;
							}
						}
					breakAfterdoctypepublickeywordloop:
						goto case BEFORE_DOCTYPE_PUBLIC_IDENTIFIER;
					// FALLTHRU DON'T REORDER
					case BEFORE_DOCTYPE_PUBLIC_IDENTIFIER:
						/*beforedoctypepublicidentifierloop:*/
						for (; ; )
						{
							if (++pos == endPos)
							{
								goto breakStateloop;
							}
							c = checkChar(buf, pos);
							/*
							 * Consume the next input character:
							 */
							switch (c)
							{
								case '\r':
									silentCarriageReturn();
									goto breakStateloop;
								case '\n':
									silentLineFeed();
									// fall thru
									goto case ' ';
								case ' ':
								case '\t':
								case '\u000C':
									/*
									 * U+0009 CHARACTER TABULATION U+000A LINE FEED
									 * (LF) U+000C FORM FEED (FF) U+0020 SPACE Stay
									 * in the before DOCTYPE public identifier
									 * state.
									 */
									continue;
								case '"':
									/*
									 * U+0022 QUOTATION MARK (") Set the DOCTYPE
									 * token's public identifier to the empty string
									 * (not missing),
									 */
									clearLongStrBuf();
									/*
									 * then switch to the DOCTYPE public identifier
									 * (double-quoted) state.
									 */
									state = transition(state, Tokenizer.DOCTYPE_PUBLIC_IDENTIFIER_DOUBLE_QUOTED, reconsume, pos);
									goto breakBeforedoctypepublicidentifierloop;
								// goto continueStateloop;
								case '\'':
									/*
									 * U+0027 APOSTROPHE (') Set the DOCTYPE token's
									 * public identifier to the empty string (not
									 * missing),
									 */
									clearLongStrBuf();
									/*
									 * then switch to the DOCTYPE public identifier
									 * (single-quoted) state.
									 */
									state = transition(state, Tokenizer.DOCTYPE_PUBLIC_IDENTIFIER_SINGLE_QUOTED, reconsume, pos);
									goto continueStateloop;
								case '>':
									/* U+003E GREATER-THAN SIGN (>) Parse error. */
									errExpectedPublicId();
									/*
									 * Set the DOCTYPE token's force-quirks flag to
									 * on.
									 */
									forceQuirks = true;
									/*
									 * Emit that DOCTYPE token.
									 */
									EmitDoctypeToken(pos);
									/*
									 * Switch to the data state.
									 */
									state = transition(state, Tokenizer.DATA, reconsume, pos);
									goto continueStateloop;
								default:
									bogusDoctype();
									/*
									 * Set the DOCTYPE token's force-quirks flag to
									 * on.
									 */
									// done by bogusDoctype();
									/*
									 * Switch to the bogus DOCTYPE state.
									 */
									state = transition(state, Tokenizer.BOGUS_DOCTYPE, reconsume, pos);
									goto continueStateloop;
							}
						}
					breakBeforedoctypepublicidentifierloop:
						goto case DOCTYPE_PUBLIC_IDENTIFIER_DOUBLE_QUOTED;
					// FALLTHRU DON'T REORDER
					case DOCTYPE_PUBLIC_IDENTIFIER_DOUBLE_QUOTED:
						/*doctypepublicidentifierdoublequotedloop:*/
						for (; ; )
						{
							if (++pos == endPos)
							{
								goto breakStateloop;
							}
							c = checkChar(buf, pos);
							/*
							 * Consume the next input character:
							 */
							switch (c)
							{
								case '"':
									/*
									 * U+0022 QUOTATION MARK (") Switch to the after
									 * DOCTYPE public identifier state.
									 */
									publicIdentifier = LongStrBufToString();
									state = transition(state, Tokenizer.AFTER_DOCTYPE_PUBLIC_IDENTIFIER, reconsume, pos);
									goto breakDoctypepublicidentifierdoublequotedloop;
								// goto continueStateloop;
								case '>':
									/*
									 * U+003E GREATER-THAN SIGN (>) Parse error.
									 */
									errGtInPublicId();
									/*
									 * Set the DOCTYPE token's force-quirks flag to
									 * on.
									 */
									forceQuirks = true;
									/*
									 * Emit that DOCTYPE token.
									 */
									publicIdentifier = LongStrBufToString();
									EmitDoctypeToken(pos);
									/*
									 * Switch to the data state.
									 */
									state = transition(state, Tokenizer.DATA, reconsume, pos);
									goto continueStateloop;
								case '\r':
									appendLongStrBufCarriageReturn();
									goto breakStateloop;
								case '\n':
									appendLongStrBufLineFeed();
									continue;
								case '\u0000':
									c = '\uFFFD';
									// fall thru
									goto default;
								default:
									/*
									 * Anything else Append the current input
									 * character to the current DOCTYPE token's
									 * public identifier.
									 */
									appendLongStrBuf(c);
									/*
									 * Stay in the DOCTYPE public identifier
									 * (double-quoted) state.
									 */
									continue;
							}
						}
					breakDoctypepublicidentifierdoublequotedloop:
						goto case AFTER_DOCTYPE_PUBLIC_IDENTIFIER;
					// FALLTHRU DON'T REORDER
					case AFTER_DOCTYPE_PUBLIC_IDENTIFIER:
						/*afterdoctypepublicidentifierloop:*/
						for (; ; )
						{
							if (++pos == endPos)
							{
								goto breakStateloop;
							}
							c = checkChar(buf, pos);
							/*
							 * Consume the next input character:
							 */
							switch (c)
							{
								case '\r':
									silentCarriageReturn();
									state = transition(state, Tokenizer.BETWEEN_DOCTYPE_PUBLIC_AND_SYSTEM_IDENTIFIERS, reconsume, pos);
									goto breakStateloop;
								case '\n':
									silentLineFeed();
									// fall thru
									goto case ' ';
								case ' ':
								case '\t':
								case '\u000C':
									/*
									 * U+0009 CHARACTER TABULATION U+000A LINE FEED
									 * (LF) U+000C FORM FEED (FF) U+0020 SPACE
									 * Switch to the between DOCTYPE public and
									 * system identifiers state.
									 */
									state = transition(state, Tokenizer.BETWEEN_DOCTYPE_PUBLIC_AND_SYSTEM_IDENTIFIERS, reconsume, pos);
									goto breakAfterdoctypepublicidentifierloop;
								// goto continueStateloop;
								case '>':
									/*
									 * U+003E GREATER-THAN SIGN (>) Emit the current
									 * DOCTYPE token.
									 */
									EmitDoctypeToken(pos);
									/*
									 * Switch to the data state.
									 */
									state = transition(state, Tokenizer.DATA, reconsume, pos);
									goto continueStateloop;
								case '"':
									/*
									 * U+0022 QUOTATION MARK (") Parse error.
									 */
									errNoSpaceBetweenPublicAndSystemIds();
									/*
									 * Set the DOCTYPE token's system identifier to
									 * the empty string (not missing),
									 */
									clearLongStrBuf();
									/*
									 * then switch to the DOCTYPE system identifier
									 * (double-quoted) state.
									 */
									state = transition(state, Tokenizer.DOCTYPE_SYSTEM_IDENTIFIER_DOUBLE_QUOTED, reconsume, pos);
									goto continueStateloop;
								case '\'':
									/*
									 * U+0027 APOSTROPHE (') Parse error.
									 */
									errNoSpaceBetweenPublicAndSystemIds();
									/*
									 * Set the DOCTYPE token's system identifier to
									 * the empty string (not missing),
									 */
									clearLongStrBuf();
									/*
									 * then switch to the DOCTYPE system identifier
									 * (single-quoted) state.
									 */
									state = transition(state, Tokenizer.DOCTYPE_SYSTEM_IDENTIFIER_SINGLE_QUOTED, reconsume, pos);
									goto continueStateloop;
								default:
									bogusDoctype();
									/*
									 * Set the DOCTYPE token's force-quirks flag to
									 * on.
									 */
									// done by bogusDoctype();
									/*
									 * Switch to the bogus DOCTYPE state.
									 */
									state = transition(state, Tokenizer.BOGUS_DOCTYPE, reconsume, pos);
									goto continueStateloop;
							}
						}
					breakAfterdoctypepublicidentifierloop:
						goto case BETWEEN_DOCTYPE_PUBLIC_AND_SYSTEM_IDENTIFIERS;
					// FALLTHRU DON'T REORDER
					case BETWEEN_DOCTYPE_PUBLIC_AND_SYSTEM_IDENTIFIERS:
						/*betweendoctypepublicandsystemidentifiersloop:*/
						for (; ; )
						{
							if (++pos == endPos)
							{
								goto breakStateloop;
							}
							c = checkChar(buf, pos);
							/*
							 * Consume the next input character:
							 */
							switch (c)
							{
								case '\r':
									silentCarriageReturn();
									goto breakStateloop;
								case '\n':
									silentLineFeed();
									goto case ' ';
								// fall thru
								case ' ':
								case '\t':
								case '\u000C':
									/*
									 * U+0009 CHARACTER TABULATION U+000A LINE FEED
									 * (LF) U+000C FORM FEED (FF) U+0020 SPACE Stay
									 * in the between DOCTYPE public and system
									 * identifiers state.
									 */
									continue;
								case '>':
									/*
									 * U+003E GREATER-THAN SIGN (>) Emit the current
									 * DOCTYPE token.
									 */
									EmitDoctypeToken(pos);
									/*
									 * Switch to the data state.
									 */
									state = transition(state, Tokenizer.DATA, reconsume, pos);
									goto continueStateloop;
								case '"':
									/*
									 * U+0022 QUOTATION MARK (") Set the DOCTYPE
									 * token's system identifier to the empty string
									 * (not missing),
									 */
									clearLongStrBuf();
									/*
									 * then switch to the DOCTYPE system identifier
									 * (double-quoted) state.
									 */
									state = transition(state, Tokenizer.DOCTYPE_SYSTEM_IDENTIFIER_DOUBLE_QUOTED, reconsume, pos);
									goto breakBetweendoctypepublicandsystemidentifiersloop;
								// goto continueStateloop;
								case '\'':
									/*
									 * U+0027 APOSTROPHE (') Set the DOCTYPE token's
									 * system identifier to the empty string (not
									 * missing),
									 */
									clearLongStrBuf();
									/*
									 * then switch to the DOCTYPE system identifier
									 * (single-quoted) state.
									 */
									state = transition(state, Tokenizer.DOCTYPE_SYSTEM_IDENTIFIER_SINGLE_QUOTED, reconsume, pos);
									goto continueStateloop;
								default:
									bogusDoctype();
									/*
									 * Set the DOCTYPE token's force-quirks flag to
									 * on.
									 */
									// done by bogusDoctype();
									/*
									 * Switch to the bogus DOCTYPE state.
									 */
									state = transition(state, Tokenizer.BOGUS_DOCTYPE, reconsume, pos);
									goto continueStateloop;
							}
						}
					breakBetweendoctypepublicandsystemidentifiersloop:
						goto case DOCTYPE_SYSTEM_IDENTIFIER_DOUBLE_QUOTED;
					// FALLTHRU DON'T REORDER
					case DOCTYPE_SYSTEM_IDENTIFIER_DOUBLE_QUOTED:
						/*doctypesystemidentifierdoublequotedloop:*/
						for (; ; )
						{
							if (++pos == endPos)
							{
								goto breakStateloop;
							}
							c = checkChar(buf, pos);
							/*
							 * Consume the next input character:
							 */
							switch (c)
							{
								case '"':
									/*
									 * U+0022 QUOTATION MARK (") Switch to the after
									 * DOCTYPE system identifier state.
									 */
									systemIdentifier = LongStrBufToString();
									state = transition(state, Tokenizer.AFTER_DOCTYPE_SYSTEM_IDENTIFIER, reconsume, pos);
									goto continueStateloop;
								case '>':
									/*
									 * U+003E GREATER-THAN SIGN (>) Parse error.
									 */
									errGtInSystemId();
									/*
									 * Set the DOCTYPE token's force-quirks flag to
									 * on.
									 */
									forceQuirks = true;
									/*
									 * Emit that DOCTYPE token.
									 */
									systemIdentifier = LongStrBufToString();
									EmitDoctypeToken(pos);
									/*
									 * Switch to the data state.
									 */
									state = transition(state, Tokenizer.DATA, reconsume, pos);
									goto continueStateloop;
								case '\r':
									appendLongStrBufCarriageReturn();
									goto breakStateloop;
								case '\n':
									appendLongStrBufLineFeed();
									continue;
								case '\u0000':
									c = '\uFFFD';
									// fall thru
									goto default;
								default:
									/*
									 * Anything else Append the current input
									 * character to the current DOCTYPE token's
									 * system identifier.
									 */
									appendLongStrBuf(c);
									/*
									 * Stay in the DOCTYPE system identifier
									 * (double-quoted) state.
									 */
									continue;
							}
						}
					breakDoctypesystemidentifierdoublequotedloop:
						goto case AFTER_DOCTYPE_SYSTEM_IDENTIFIER;
					// FALLTHRU DON'T REORDER
					case AFTER_DOCTYPE_SYSTEM_IDENTIFIER:
						/*afterdoctypesystemidentifierloop:*/
						for (; ; )
						{
							if (++pos == endPos)
							{
								goto breakStateloop;
							}
							c = checkChar(buf, pos);
							/*
							 * Consume the next input character:
							 */
							switch (c)
							{
								case '\r':
									silentCarriageReturn();
									goto breakStateloop;
								case '\n':
									silentLineFeed();
									goto case ' ';
								// fall thru
								case ' ':
								case '\t':
								case '\u000C':
									/*
									 * U+0009 CHARACTER TABULATION U+000A LINE FEED
									 * (LF) U+000C FORM FEED (FF) U+0020 SPACE Stay
									 * in the after DOCTYPE system identifier state.
									 */
									continue;
								case '>':
									/*
									 * U+003E GREATER-THAN SIGN (>) Emit the current
									 * DOCTYPE token.
									 */
									EmitDoctypeToken(pos);
									/*
									 * Switch to the data state.
									 */
									state = transition(state, Tokenizer.DATA, reconsume, pos);
									goto continueStateloop;
								default:
									/*
									 * Switch to the bogus DOCTYPE state. (This does
									 * not set the DOCTYPE token's force-quirks flag
									 * to on.)
									 */
									bogusDoctypeWithoutQuirks();
									state = transition(state, Tokenizer.BOGUS_DOCTYPE, reconsume, pos);
									goto breakAfterdoctypesystemidentifierloop;
								// goto continueStateloop;
							}
						}
					breakAfterdoctypesystemidentifierloop:
						goto case BOGUS_DOCTYPE;
					// FALLTHRU DON'T REORDER
					case BOGUS_DOCTYPE:
						for (; ; )
						{
							if (reconsume)
							{
								reconsume = false;
							}
							else
							{
								if (++pos == endPos)
								{
									goto breakStateloop;
								}
								c = checkChar(buf, pos);
							}
							/*
							 * Consume the next input character:
							 */
							switch (c)
							{
								case '>':
									/*
									 * U+003E GREATER-THAN SIGN (>) Emit that
									 * DOCTYPE token.
									 */
									EmitDoctypeToken(pos);
									/*
									 * Switch to the data state.
									 */
									state = transition(state, Tokenizer.DATA, reconsume, pos);
									goto continueStateloop;
								case '\r':
									silentCarriageReturn();
									goto breakStateloop;
								case '\n':
									silentLineFeed();
									// fall thru
									goto default;
								default:
									/*
									 * Anything else Stay in the bogus DOCTYPE
									 * state.
									 */
									continue;
							}
						}
					// XXX reorder point
					case DOCTYPE_YSTEM:
						/*doctypeystemloop:*/
						for (; ; )
						{
							if (++pos == endPos)
							{
								goto breakStateloop;
							}
							c = checkChar(buf, pos);
							/*
							 * Otherwise, if the six characters starting from the
							 * current input character are an ASCII case-insensitive
							 * match for the word "SYSTEM", then consume those
							 * characters and switch to the before DOCTYPE system
							 * identifier state.
							 */
							if (index < 5)
							{ // YSTEM.Length
								char folded = c;
								if (c >= 'A' && c <= 'Z')
								{
									folded += (char)0x20;
								}
								if (folded != Tokenizer.YSTEM[index])
								{
									bogusDoctype();
									state = transition(state, Tokenizer.BOGUS_DOCTYPE, reconsume, pos);
									reconsume = true;
									goto continueStateloop;
								}
								index++;
								goto continueStateloop;
							}
							else
							{
								state = transition(state, Tokenizer.AFTER_DOCTYPE_SYSTEM_KEYWORD, reconsume, pos);
								reconsume = true;
								goto breakDoctypeystemloop;
								// goto continueStateloop;
							}
						}
					breakDoctypeystemloop:
						goto case AFTER_DOCTYPE_SYSTEM_KEYWORD;
					// FALLTHRU DON'T REORDER
					case AFTER_DOCTYPE_SYSTEM_KEYWORD:
						/*afterdoctypesystemkeywordloop:*/
						for (; ; )
						{
							if (reconsume)
							{
								reconsume = false;
							}
							else
							{
								if (++pos == endPos)
								{
									goto breakStateloop;
								}
								c = checkChar(buf, pos);
							}
							/*
							 * Consume the next input character:
							 */
							switch (c)
							{
								case '\r':
									silentCarriageReturn();
									state = transition(state, Tokenizer.BEFORE_DOCTYPE_SYSTEM_IDENTIFIER, reconsume, pos);
									goto breakStateloop;
								case '\n':
									silentLineFeed();
									// fall thru
									goto case ' ';
								case ' ':
								case '\t':
								case '\u000C':
									/*
									 * U+0009 CHARACTER TABULATION U+000A LINE FEED
									 * (LF) U+000C FORM FEED (FF) U+0020 SPACE
									 * Switch to the before DOCTYPE public
									 * identifier state.
									 */
									state = transition(state, Tokenizer.BEFORE_DOCTYPE_SYSTEM_IDENTIFIER, reconsume, pos);
									goto breakAfterdoctypesystemkeywordloop;
								// FALL THROUGH continue stateloop
								case '"':
									/*
									 * U+0022 QUOTATION MARK (") Parse Error.
									 */
									errNoSpaceBetweenDoctypeSystemKeywordAndQuote();
									/*
									 * Set the DOCTYPE token's system identifier to
									 * the empty string (not missing),
									 */
									clearLongStrBuf();
									/*
									 * then switch to the DOCTYPE public identifier
									 * (double-quoted) state.
									 */
									state = transition(state, Tokenizer.DOCTYPE_SYSTEM_IDENTIFIER_DOUBLE_QUOTED, reconsume, pos);
									goto continueStateloop;
								case '\'':
									/*
									 * U+0027 APOSTROPHE (') Parse Error.
									 */
									errNoSpaceBetweenDoctypeSystemKeywordAndQuote();
									/*
									 * Set the DOCTYPE token's public identifier to
									 * the empty string (not missing),
									 */
									clearLongStrBuf();
									/*
									 * then switch to the DOCTYPE public identifier
									 * (single-quoted) state.
									 */
									state = transition(state, Tokenizer.DOCTYPE_SYSTEM_IDENTIFIER_SINGLE_QUOTED, reconsume, pos);
									goto continueStateloop;
								case '>':
									/* U+003E GREATER-THAN SIGN (>) Parse error. */
									errExpectedPublicId();
									/*
									 * Set the DOCTYPE token's force-quirks flag to
									 * on.
									 */
									forceQuirks = true;
									/*
									 * Emit that DOCTYPE token.
									 */
									EmitDoctypeToken(pos);
									/*
									 * Switch to the data state.
									 */
									state = transition(state, Tokenizer.DATA, reconsume, pos);
									goto continueStateloop;
								default:
									bogusDoctype();
									/*
									 * Set the DOCTYPE token's force-quirks flag to
									 * on.
									 */
									// done by bogusDoctype();
									/*
									 * Switch to the bogus DOCTYPE state.
									 */
									state = transition(state, Tokenizer.BOGUS_DOCTYPE, reconsume, pos);
									goto continueStateloop;
							}
						}
					breakAfterdoctypesystemkeywordloop:
						goto case BEFORE_DOCTYPE_SYSTEM_IDENTIFIER;
					// FALLTHRU DON'T REORDER
					case BEFORE_DOCTYPE_SYSTEM_IDENTIFIER:
						/*beforedoctypesystemidentifierloop:*/
						for (; ; )
						{
							if (++pos == endPos)
							{
								goto breakStateloop;
							}
							c = checkChar(buf, pos);
							/*
							 * Consume the next input character:
							 */
							switch (c)
							{
								case '\r':
									silentCarriageReturn();
									goto breakStateloop;
								case '\n':
									silentLineFeed();
									// fall thru
									goto case ' ';
								case ' ':
								case '\t':
								case '\u000C':
									/*
									 * U+0009 CHARACTER TABULATION U+000A LINE FEED
									 * (LF) U+000C FORM FEED (FF) U+0020 SPACE Stay
									 * in the before DOCTYPE system identifier
									 * state.
									 */
									continue;
								case '"':
									/*
									 * U+0022 QUOTATION MARK (") Set the DOCTYPE
									 * token's system identifier to the empty string
									 * (not missing),
									 */
									clearLongStrBuf();
									/*
									 * then switch to the DOCTYPE system identifier
									 * (double-quoted) state.
									 */
									state = transition(state, Tokenizer.DOCTYPE_SYSTEM_IDENTIFIER_DOUBLE_QUOTED, reconsume, pos);
									goto continueStateloop;
								case '\'':
									/*
									 * U+0027 APOSTROPHE (') Set the DOCTYPE token's
									 * system identifier to the empty string (not
									 * missing),
									 */
									clearLongStrBuf();
									/*
									 * then switch to the DOCTYPE system identifier
									 * (single-quoted) state.
									 */
									state = transition(state, Tokenizer.DOCTYPE_SYSTEM_IDENTIFIER_SINGLE_QUOTED, reconsume, pos);
									goto breakBeforedoctypesystemidentifierloop;
								// goto continueStateloop;
								case '>':
									/* U+003E GREATER-THAN SIGN (>) Parse error. */
									errExpectedSystemId();
									/*
									 * Set the DOCTYPE token's force-quirks flag to
									 * on.
									 */
									forceQuirks = true;
									/*
									 * Emit that DOCTYPE token.
									 */
									EmitDoctypeToken(pos);
									/*
									 * Switch to the data state.
									 */
									state = transition(state, Tokenizer.DATA, reconsume, pos);
									goto continueStateloop;
								default:
									bogusDoctype();
									/*
									 * Set the DOCTYPE token's force-quirks flag to
									 * on.
									 */
									// done by bogusDoctype();
									/*
									 * Switch to the bogus DOCTYPE state.
									 */
									state = transition(state, Tokenizer.BOGUS_DOCTYPE, reconsume, pos);
									goto continueStateloop;
							}
						}
					breakBeforedoctypesystemidentifierloop:
						goto case DOCTYPE_SYSTEM_IDENTIFIER_SINGLE_QUOTED;
					// FALLTHRU DON'T REORDER
					case DOCTYPE_SYSTEM_IDENTIFIER_SINGLE_QUOTED:
						for (; ; )
						{
							if (++pos == endPos)
							{
								goto breakStateloop;
							}
							c = checkChar(buf, pos);
							/*
							 * Consume the next input character:
							 */
							switch (c)
							{
								case '\'':
									/*
									 * U+0027 APOSTROPHE (') Switch to the after
									 * DOCTYPE system identifier state.
									 */
									systemIdentifier = LongStrBufToString();
									state = transition(state, Tokenizer.AFTER_DOCTYPE_SYSTEM_IDENTIFIER, reconsume, pos);
									goto continueStateloop;
								case '>':
									errGtInSystemId();
									/*
									 * Set the DOCTYPE token's force-quirks flag to
									 * on.
									 */
									forceQuirks = true;
									/*
									 * Emit that DOCTYPE token.
									 */
									systemIdentifier = LongStrBufToString();
									EmitDoctypeToken(pos);
									/*
									 * Switch to the data state.
									 */
									state = transition(state, Tokenizer.DATA, reconsume, pos);
									goto continueStateloop;
								case '\r':
									appendLongStrBufCarriageReturn();
									goto breakStateloop;
								case '\n':
									appendLongStrBufLineFeed();
									continue;
								case '\u0000':
									c = '\uFFFD';
									// fall thru
									goto default;
								default:
									/*
									 * Anything else Append the current input
									 * character to the current DOCTYPE token's
									 * system identifier.
									 */
									appendLongStrBuf(c);
									/*
									 * Stay in the DOCTYPE system identifier
									 * (double-quoted) state.
									 */
									continue;
							}
						}
					// XXX reorder point

					case DOCTYPE_PUBLIC_IDENTIFIER_SINGLE_QUOTED:
						for (; ; )
						{
							if (++pos == endPos)
							{
								goto breakStateloop;
							}
							c = checkChar(buf, pos);
							/*
							 * Consume the next input character:
							 */
							switch (c)
							{
								case '\'':
									/*
									 * U+0027 APOSTROPHE (') Switch to the after
									 * DOCTYPE public identifier state.
									 */
									publicIdentifier = LongStrBufToString();
									state = transition(state, Tokenizer.AFTER_DOCTYPE_PUBLIC_IDENTIFIER, reconsume, pos);
									goto continueStateloop;
								case '>':
									errGtInPublicId();
									/*
									 * Set the DOCTYPE token's force-quirks flag to
									 * on.
									 */
									forceQuirks = true;
									/*
									 * Emit that DOCTYPE token.
									 */
									publicIdentifier = LongStrBufToString();
									EmitDoctypeToken(pos);
									/*
									 * Switch to the data state.
									 */
									state = transition(state, Tokenizer.DATA, reconsume, pos);
									goto continueStateloop;
								case '\r':
									appendLongStrBufCarriageReturn();
									goto breakStateloop;
								case '\n':
									appendLongStrBufLineFeed();
									continue;
								case '\u0000':
									c = '\uFFFD';
									// fall thru
									goto default;
								default:
									/*
									 * Anything else Append the current input
									 * character to the current DOCTYPE token's
									 * public identifier.
									 */
									appendLongStrBuf(c);
									/*
									 * Stay in the DOCTYPE public identifier
									 * (single-quoted) state.
									 */
									continue;
							}
						}
					// END HOTSPOT WORKAROUND
				}
			} // stateloop


			breakStateloop:

			FlushChars(buf, pos);
			/*
			 * if (prevCR && pos != endPos) { // why is this needed? pos--; col--; }
			 */
			// Save locals
			stateSave = state;
			returnStateSave = returnState;
			return pos;
		}

		// HOTSPOT WORKAROUND INSERTION POINT

		// [NOCPP[

		protected int transition(int from, int to, bool reconsume, int pos)
		{
			return to;
		}

		// ]NOCPP]

		private void initDoctypeFields()
		{
			doctypeName = "";
			if (systemIdentifier != null)
			{
				systemIdentifier = null;
			}
			if (publicIdentifier != null)
			{
				publicIdentifier = null;
			}
			forceQuirks = false;
		}

		/*@Inline*/
		private void adjustDoubleHyphenAndAppendToLongStrBufCarriageReturn()
		{
			silentCarriageReturn();
			adjustDoubleHyphenAndAppendToLongStrBufAndErr('\n');
		}

		/*@Inline*/
		private void adjustDoubleHyphenAndAppendToLongStrBufLineFeed()
		{
			silentLineFeed();
			adjustDoubleHyphenAndAppendToLongStrBufAndErr('\n');
		}

		/*@Inline*/
		private void appendLongStrBufLineFeed()
		{
			silentLineFeed();
			appendLongStrBuf('\n');
		}

		/*@Inline*/
		private void appendLongStrBufCarriageReturn()
		{
			silentCarriageReturn();
			appendLongStrBuf('\n');
		}

		/*@Inline*/
		protected void silentCarriageReturn()
		{
			++line;
			lastCR = true;
		}

		/*@Inline*/
		protected void silentLineFeed()
		{
			++line;
		}

		private void emitCarriageReturn(char[] buf, int pos)
		{
			silentCarriageReturn();
			FlushChars(buf, pos);
			TokenHandler.Characters(Tokenizer.LF, 0, 1);
			cstart = int.MaxValue;
		}

		private void emitReplacementCharacter(char[] buf, int pos)
		{
			FlushChars(buf, pos);
			TokenHandler.ZeroOriginatingReplacementCharacter();
			cstart = pos + 1;
		}

		private void emitPlaintextReplacementCharacter(char[] buf, int pos)
		{
			FlushChars(buf, pos);
			TokenHandler.Characters(REPLACEMENT_CHARACTER, 0, 1);
			cstart = pos + 1;
		}

		private void setAdditionalAndRememberAmpersandLocation(char add)
		{
			additional = add;
			// [NOCPP[
			ampersandLocation = new Locator(this);
			// ]NOCPP]
		}

		private void bogusDoctype()
		{
			errBogusDoctype();
			forceQuirks = true;
		}

		private void bogusDoctypeWithoutQuirks()
		{
			errBogusDoctype();
			forceQuirks = false;
		}

		private void emitOrAppendStrBuf(int returnState)
		{
			if ((returnState & DATA_AND_RCDATA_MASK) != 0)
			{
				appendStrBufToLongStrBuf();
			}
			else
			{
				emitStrBuf();
			}
		}

		private void handleNcrValue(int returnState)
		{
			/*
			 * If one or more characters match the range, then take them all and
			 * interpret the string of characters as a number (either hexadecimal or
			 * decimal as appropriate).
			 */
			if (value <= 0xFFFF)
			{
				if (value >= 0x80 && value <= 0x9f)
				{
					/*
					 * If that number is one of the numbers in the first column of
					 * the following table, then this is a parse error.
					 */
					errNcrInC1Range();
					/*
					 * Find the row with that number in the first column, and return
					 * a character token for the Unicode character given in the
					 * second column of that row.
					 */
					char[] val = NamedCharacters.WINDOWS_1252[value - 0x80];
					emitOrAppendOne(val, returnState);
					// [NOCPP[
				}
				else if (value == 0xC
					  && contentSpacePolicy != XmlViolationPolicy.Allow)
				{
					if (contentSpacePolicy == XmlViolationPolicy.AlterInfoset)
					{
						emitOrAppendOne(Tokenizer.SPACE, returnState);
					}
					else if (contentSpacePolicy == XmlViolationPolicy.Fatal)
					{
						Fatal("A character reference expanded to a form feed which is not legal XML 1.0 white space.");
					}
					// ]NOCPP]
				}
				else if (value == 0x0)
				{
					errNcrZero();
					emitOrAppendOne(Tokenizer.REPLACEMENT_CHARACTER, returnState);
				}
				else if ((value & 0xF800) == 0xD800)
				{
					errNcrSurrogate();
					emitOrAppendOne(Tokenizer.REPLACEMENT_CHARACTER, returnState);
				}
				else
				{
					/*
					 * Otherwise, return a character token for the Unicode character
					 * whose code point is that number.
					 */
					char ch = (char)value;
					// [NOCPP[
					if (value == 0x0D)
					{
						errNcrCr();
					}
					else if ((value <= 0x0008) || (value == 0x000B)
						  || (value >= 0x000E && value <= 0x001F))
					{
						ch = errNcrControlChar(ch);
					}
					else if (value >= 0xFDD0 && value <= 0xFDEF)
					{
						errNcrUnassigned();
					}
					else if ((value & 0xFFFE) == 0xFFFE)
					{
						ch = errNcrNonCharacter(ch);
					}
					else if (value >= 0x007F && value <= 0x009F)
					{
						errNcrControlChar();
					}
					else
					{
						maybeWarnPrivateUse(ch);
					}
					// ]NOCPP]
					bmpChar[0] = ch;
					emitOrAppendOne(bmpChar, returnState);
				}
			}
			else if (value <= 0x10FFFF)
			{
				// [NOCPP[
				maybeWarnPrivateUseAstral();
				if ((value & 0xFFFE) == 0xFFFE)
				{
					errAstralNonCharacter(value);
				}
				// ]NOCPP]
				astralChar[0] = (char)(Tokenizer.LEAD_OFFSET + (value >> 10));
				astralChar[1] = (char)(0xDC00 + (value & 0x3FF));
				emitOrAppendTwo(astralChar, returnState);
			}
			else
			{
				errNcrOutOfRange();
				emitOrAppendOne(Tokenizer.REPLACEMENT_CHARACTER, returnState);
			}
		}

		public void Eof()
		{
			int state = stateSave;
			int returnState = returnStateSave;

			/*eofloop:*/
			for (; ; )
			{
				switch (state)
				{
					case SCRIPT_DATA_LESS_THAN_SIGN:
					case SCRIPT_DATA_ESCAPED_LESS_THAN_SIGN:
						/*
						 * Otherwise, emit a U+003C LESS-THAN SIGN character token
						 */
						TokenHandler.Characters(Tokenizer.LT_GT, 0, 1);
						/*
						 * and reconsume the current input character in the data
						 * state.
						 */
						goto breakEofloop;
					case TAG_OPEN:
						/*
						 * The behavior of this state depends on the content model
						 * flag.
						 */
						/*
						 * Anything else Parse error.
						 */
						errEofAfterLt();
						/*
						 * Emit a U+003C LESS-THAN SIGN character token
						 */
						TokenHandler.Characters(Tokenizer.LT_GT, 0, 1);
						/*
						 * and reconsume the current input character in the data
						 * state.
						 */
						goto breakEofloop;
					case RAWTEXT_RCDATA_LESS_THAN_SIGN:
						/*
						 * Emit a U+003C LESS-THAN SIGN character token
						 */
						TokenHandler.Characters(Tokenizer.LT_GT, 0, 1);
						/*
						 * and reconsume the current input character in the RCDATA
						 * state.
						 */
						goto breakEofloop;
					case NON_DATA_END_TAG_NAME:
						/*
						 * Emit a U+003C LESS-THAN SIGN character token, a U+002F
						 * SOLIDUS character token,
						 */
						TokenHandler.Characters(Tokenizer.LT_SOLIDUS, 0, 2);
						/*
						 * a character token for each of the characters in the
						 * temporary buffer (in the order they were added to the
						 * buffer),
						 */
						emitStrBuf();
						/*
						 * and reconsume the current input character in the RCDATA
						 * state.
						 */
						goto breakEofloop;
					case CLOSE_TAG_OPEN:
						/* EOF Parse error. */
						errEofAfterLt();
						/*
						 * Emit a U+003C LESS-THAN SIGN character token and a U+002F
						 * SOLIDUS character token.
						 */
						TokenHandler.Characters(Tokenizer.LT_SOLIDUS, 0, 2);
						/*
						 * Reconsume the EOF character in the data state.
						 */
						goto breakEofloop;
					case TAG_NAME:
						/*
						 * EOF Parse error.
						 */
						errEofInTagName();
						/*
						 * Reconsume the EOF character in the data state.
						 */
						goto breakEofloop;
					case BEFORE_ATTRIBUTE_NAME:
					case AFTER_ATTRIBUTE_VALUE_QUOTED:
					case SELF_CLOSING_START_TAG:
						/* EOF Parse error. */
						errEofWithoutGt();
						/*
						 * Reconsume the EOF character in the data state.
						 */
						goto breakEofloop;
					case ATTRIBUTE_NAME:
						/*
						 * EOF Parse error.
						 */
						errEofInAttributeName();
						/*
						 * Reconsume the EOF character in the data state.
						 */
						goto breakEofloop;
					case AFTER_ATTRIBUTE_NAME:
					case BEFORE_ATTRIBUTE_VALUE:
						/* EOF Parse error. */
						errEofWithoutGt();
						/*
						 * Reconsume the EOF character in the data state.
						 */
						goto breakEofloop;
					case ATTRIBUTE_VALUE_DOUBLE_QUOTED:
					case ATTRIBUTE_VALUE_SINGLE_QUOTED:
					case ATTRIBUTE_VALUE_UNQUOTED:
						/* EOF Parse error. */
						errEofInAttributeValue();
						/*
						 * Reconsume the EOF character in the data state.
						 */
						goto breakEofloop;
					case BOGUS_COMMENT:
						EmitComment(0, 0);
						goto breakEofloop;
					case BOGUS_COMMENT_HYPHEN:
						// [NOCPP[
						maybeAppendSpaceToBogusComment();
						// ]NOCPP]
						EmitComment(0, 0);
						goto breakEofloop;
					case MARKUP_DECLARATION_OPEN:
						errBogusComment();
						clearLongStrBuf();
						EmitComment(0, 0);
						goto breakEofloop;
					case MARKUP_DECLARATION_HYPHEN:
						errBogusComment();
						EmitComment(0, 0);
						goto breakEofloop;
					case MARKUP_DECLARATION_OCTYPE:
						if (index < 6)
						{
							errBogusComment();
							EmitComment(0, 0);
						}
						else
						{
							/* EOF Parse error. */
							errEofInDoctype();
							/*
							 * Create a new DOCTYPE token. Set its force-quirks flag
							 * to on.
							 */
							doctypeName = "";
							if (systemIdentifier != null)
							{
								systemIdentifier = null;
							}
							if (publicIdentifier != null)
							{
								publicIdentifier = null;
							}
							forceQuirks = true;
							/*
							 * Emit the token.
							 */
							EmitDoctypeToken(0);
							/*
							 * Reconsume the EOF character in the data state.
							 */
							goto breakEofloop;
						}
						goto breakEofloop;
					case COMMENT_START:
					case COMMENT:
						/*
						 * EOF Parse error.
						 */
						errEofInComment();
						/* Emit the comment token. */
						EmitComment(0, 0);
						/*
						 * Reconsume the EOF character in the data state.
						 */
						goto breakEofloop;
					case COMMENT_END:
						errEofInComment();
						/* Emit the comment token. */
						EmitComment(2, 0);
						/*
						 * Reconsume the EOF character in the data state.
						 */
						goto breakEofloop;
					case COMMENT_END_DASH:
					case COMMENT_START_DASH:
						errEofInComment();
						/* Emit the comment token. */
						EmitComment(1, 0);
						/*
						 * Reconsume the EOF character in the data state.
						 */
						goto breakEofloop;
					case COMMENT_END_BANG:
						errEofInComment();
						/* Emit the comment token. */
						EmitComment(3, 0);
						/*
						 * Reconsume the EOF character in the data state.
						 */
						goto breakEofloop;
					case DOCTYPE:
					case BEFORE_DOCTYPE_NAME:
						errEofInDoctype();
						/*
						 * Create a new DOCTYPE token. Set its force-quirks flag to
						 * on.
						 */
						forceQuirks = true;
						/*
						 * Emit the token.
						 */
						EmitDoctypeToken(0);
						/*
						 * Reconsume the EOF character in the data state.
						 */
						goto breakEofloop;
					case DOCTYPE_NAME:
						errEofInDoctype();
						strBufToDoctypeName();
						/*
						 * Set the DOCTYPE token's force-quirks flag to on.
						 */
						forceQuirks = true;
						/*
						 * Emit that DOCTYPE token.
						 */
						EmitDoctypeToken(0);
						/*
						 * Reconsume the EOF character in the data state.
						 */
						goto breakEofloop;
					case DOCTYPE_UBLIC:
					case DOCTYPE_YSTEM:
					case AFTER_DOCTYPE_NAME:
					case AFTER_DOCTYPE_PUBLIC_KEYWORD:
					case AFTER_DOCTYPE_SYSTEM_KEYWORD:
					case BEFORE_DOCTYPE_PUBLIC_IDENTIFIER:
						errEofInDoctype();
						/*
						 * Set the DOCTYPE token's force-quirks flag to on.
						 */
						forceQuirks = true;
						/*
						 * Emit that DOCTYPE token.
						 */
						EmitDoctypeToken(0);
						/*
						 * Reconsume the EOF character in the data state.
						 */
						goto breakEofloop;
					case DOCTYPE_PUBLIC_IDENTIFIER_DOUBLE_QUOTED:
					case DOCTYPE_PUBLIC_IDENTIFIER_SINGLE_QUOTED:
						/* EOF Parse error. */
						errEofInPublicId();
						/*
						 * Set the DOCTYPE token's force-quirks flag to on.
						 */
						forceQuirks = true;
						/*
						 * Emit that DOCTYPE token.
						 */
						publicIdentifier = LongStrBufToString();
						EmitDoctypeToken(0);
						/*
						 * Reconsume the EOF character in the data state.
						 */
						goto breakEofloop;
					case AFTER_DOCTYPE_PUBLIC_IDENTIFIER:
					case BEFORE_DOCTYPE_SYSTEM_IDENTIFIER:
					case BETWEEN_DOCTYPE_PUBLIC_AND_SYSTEM_IDENTIFIERS:
						errEofInDoctype();
						/*
						 * Set the DOCTYPE token's force-quirks flag to on.
						 */
						forceQuirks = true;
						/*
						 * Emit that DOCTYPE token.
						 */
						EmitDoctypeToken(0);
						/*
						 * Reconsume the EOF character in the data state.
						 */
						goto breakEofloop;
					case DOCTYPE_SYSTEM_IDENTIFIER_DOUBLE_QUOTED:
					case DOCTYPE_SYSTEM_IDENTIFIER_SINGLE_QUOTED:
						/* EOF Parse error. */
						errEofInSystemId();
						/*
						 * Set the DOCTYPE token's force-quirks flag to on.
						 */
						forceQuirks = true;
						/*
						 * Emit that DOCTYPE token.
						 */
						systemIdentifier = LongStrBufToString();
						EmitDoctypeToken(0);
						/*
						 * Reconsume the EOF character in the data state.
						 */
						goto breakEofloop;
					case AFTER_DOCTYPE_SYSTEM_IDENTIFIER:
						errEofInDoctype();
						/*
						 * Set the DOCTYPE token's force-quirks flag to on.
						 */
						forceQuirks = true;
						/*
						 * Emit that DOCTYPE token.
						 */
						EmitDoctypeToken(0);
						/*
						 * Reconsume the EOF character in the data state.
						 */
						goto breakEofloop;
					case BOGUS_DOCTYPE:
						/*
						 * Emit that DOCTYPE token.
						 */
						EmitDoctypeToken(0);
						/*
						 * Reconsume the EOF character in the data state.
						 */
						goto breakEofloop;
					case CONSUME_CHARACTER_REFERENCE:
						/*
						 * Unlike the definition is the spec, this state does not
						 * return a value and never requires the caller to
						 * backtrack. This state takes care of emitting characters
						 * or appending to the current attribute value. It also
						 * takes care of that in the case when consuming the entity
						 * fails.
						 */
						/*
						 * This section defines how to consume an entity. This
						 * definition is used when parsing entities in text and in
						 * attributes.
						 * 
						 * The behavior depends on the identity of the next
						 * character (the one immediately after the U+0026 AMPERSAND
						 * character):
						 */

						emitOrAppendStrBuf(returnState);
						state = returnState;
						continue;
					case CHARACTER_REFERENCE_HILO_LOOKUP:
						errNoNamedCharacterMatch();
						emitOrAppendStrBuf(returnState);
						state = returnState;
						continue;
					case CHARACTER_REFERENCE_TAIL:
						/*outer:*/
						for (; ; )
						{
							char c = '\u0000';
							entCol++;
							/*
							 * Consume the maximum number of characters possible,
							 * with the consumed characters matching one of the
							 * identifiers in the first column of the named
							 * character references table (in a case-sensitive
							 * manner).
							 */
							/*hiloop:*/
							for (; ; )
							{
								if (hi == -1)
								{
									goto breakHiloop;
								}
								if (entCol == NamedCharacters.NAMES[hi].Length)
								{
									goto breakHiloop;
								}
								if (entCol > NamedCharacters.NAMES[hi].Length)
								{
									goto breakOuter;
								}
								else if (c < NamedCharacters.NAMES[hi][entCol])
								{
									hi--;
								}
								else
								{
									goto breakHiloop;
								}
							}

						breakHiloop:

							/*loloop:*/
							for (; ; )
							{
								if (hi < lo)
								{
									goto breakOuter;
								}
								if (entCol == NamedCharacters.NAMES[lo].Length)
								{
									candidate = lo;
									strBufMark = strBufLen;
									lo++;
								}
								else if (entCol > NamedCharacters.NAMES[lo].Length)
								{
									goto breakOuter;
								}
								else if (c > NamedCharacters.NAMES[lo][entCol])
								{
									lo++;
								}
								else
								{
									goto breakLoloop;
								}
							}

						breakLoloop:

							if (hi < lo)
							{
								goto breakOuter;
							}
							continue;
						}

					breakOuter:

						if (candidate == -1)
						{
							/*
							 * If no match can be made, then this is a parse error.
							 */
							errNoNamedCharacterMatch();
							emitOrAppendStrBuf(returnState);
							state = returnState;
							goto continueEofloop;
						}
						else
						{
							string candidateName = NamedCharacters.NAMES[candidate];
							if (candidateName.Length == 0
									|| candidateName[candidateName.Length - 1] != ';')
							{
								/*
								 * If the last character matched is not a U+003B
								 * SEMICOLON (;), there is a parse error.
								 */
								if ((returnState & DATA_AND_RCDATA_MASK) != 0)
								{
									/*
									 * If the entity is being consumed as part of an
									 * attribute, and the last character matched is
									 * not a U+003B SEMICOLON (;),
									 */
									char ch;
									if (strBufMark == strBufLen)
									{
										ch = '\u0000';
									}
									else
									{
										ch = strBuf[strBufMark];
									}
									if ((ch >= '0' && ch <= '9')
											|| (ch >= 'A' && ch <= 'Z')
											|| (ch >= 'a' && ch <= 'z'))
									{
										/*
										 * and the next character is in the range
										 * U+0030 DIGIT ZERO to U+0039 DIGIT NINE,
										 * U+0041 LATIN CAPITAL LETTER A to U+005A
										 * LATIN CAPITAL LETTER Z, or U+0061 LATIN
										 * SMALL LETTER A to U+007A LATIN SMALL
										 * LETTER Z, then, for historical reasons,
										 * all the characters that were matched
										 * after the U+0026 AMPERSAND (&) must be
										 * unconsumed, and nothing is returned.
										 */
										errNoNamedCharacterMatch();
										appendStrBufToLongStrBuf();
										state = returnState;
										goto continueEofloop;
									}
								}
								if ((returnState & DATA_AND_RCDATA_MASK) != 0)
								{
									errUnescapedAmpersandInterpretedAsCharacterReference();
								}
								else
								{
									errNotSemicolonTerminated();
								}
							}

							/*
							 * Otherwise, return a character token for the character
							 * corresponding to the entity name (as given by the
							 * second column of the named character references
							 * table).
							 */
							char[] val = NamedCharacters.VALUES[candidate];
							if (
								// [NOCPP[
							val.Length == 1
								// ]NOCPP]
								// CPPONLY: val[1] == 0
							)
							{
								emitOrAppendOne(val, returnState);
							}
							else
							{
								emitOrAppendTwo(val, returnState);
							}
							// this is so complicated!
							if (strBufMark < strBufLen)
							{
								if ((returnState & DATA_AND_RCDATA_MASK) != 0)
								{
									for (int i = strBufMark; i < strBufLen; i++)
									{
										appendLongStrBuf(strBuf[i]);
									}
								}
								else
								{
									TokenHandler.Characters(strBuf, strBufMark,
											strBufLen - strBufMark);
								}
							}
							state = returnState;
							goto continueEofloop;
							/*
							 * If the markup contains I'm &notit; I tell you, the
							 * entity is parsed as "not", as in, I'm Â¬it; I tell
							 * you. But if the markup was I'm &notin; I tell you,
							 * the entity would be parsed as "notin;", resulting in
							 * I'm âˆ‰ I tell you.
							 */
						}
					case CONSUME_NCR:
					case DECIMAL_NRC_LOOP:
					case HEX_NCR_LOOP:
						/*
						 * If no characters match the range, then don't consume any
						 * characters (and unconsume the U+0023 NUMBER SIGN
						 * character and, if appropriate, the X character). This is
						 * a parse error; nothing is returned.
						 * 
						 * Otherwise, if the next character is a U+003B SEMICOLON,
						 * consume that too. If it isn't, there is a parse error.
						 */
						if (!seenDigits)
						{
							errNoDigitsInNCR();
							emitOrAppendStrBuf(returnState);
							state = returnState;
							continue;
						}
						else
						{
							errCharRefLacksSemicolon();
						}
						// WARNING previous state sets reconsume
						handleNcrValue(returnState);
						state = returnState;
						continue;
					case CDATA_RSQB:
						TokenHandler.Characters(Tokenizer.RSQB_RSQB, 0, 1);
						goto breakEofloop;
					case CDATA_RSQB_RSQB:
						TokenHandler.Characters(Tokenizer.RSQB_RSQB, 0, 2);
						goto breakEofloop;
					case DATA:
					default:
						goto breakEofloop;
				}

			continueEofloop:
				continue;
			} // eofloop

			breakEofloop:
			// case DATA:
			/*
			 * EOF Emit an end-of-file token.
			 */
			TokenHandler.Eof();
			return;
		}

		private void EmitDoctypeToken(int pos)
		{
			cstart = pos + 1;
			TokenHandler.Doctype(doctypeName, publicIdentifier, systemIdentifier,
					forceQuirks);
			// It is OK and sufficient to release these here, since
			// there's no way out of the doctype states than through paths
			// that call this method.
			doctypeName = null;
			publicIdentifier = null;
			systemIdentifier = null;
		}

		/*@Inline*/
		protected char checkChar(char[] buf, int pos)
		{
			return buf[pos];
		}

		// [NOCPP[

		/**
		 * Returns the alreadyComplainedAboutNonAscii.
		 * 
		 * @return the alreadyComplainedAboutNonAscii
		 */
		public bool isAlreadyComplainedAboutNonAscii()
		{
			return true;
		}

		// ]NOCPP]

		public bool InternalEncodingDeclaration(string internalCharset)
		{
			bool accept = false;
			if (EncodingDeclared != null)
			{
				foreach (var inv in EncodingDeclared.GetInvocationList())
				{
					var args = new EncodingDetectedEventArgs(internalCharset);
					inv.DynamicInvoke(this, args);
					if (args.AcceptEncoding)
						accept = true;
				}
			}

			return accept;
		}

		/**
		 * @param val
		 * @throws SAXException
		 */
		private void emitOrAppendTwo(char[] val, int returnState)
		{
			if ((returnState & DATA_AND_RCDATA_MASK) != 0)
			{
				appendLongStrBuf(val[0]);
				appendLongStrBuf(val[1]);
			}
			else
			{
				TokenHandler.Characters(val, 0, 2);
			}
		}

		private void emitOrAppendOne(char[] val, int returnState)
		{
			if ((returnState & DATA_AND_RCDATA_MASK) != 0)
			{
				appendLongStrBuf(val[0]);
			}
			else
			{
				TokenHandler.Characters(val, 0, 1);
			}
		}

		public void End()
		{
			strBuf = null;
			longStrBuf = null;
			doctypeName = null;
			systemIdentifier = null;
			publicIdentifier = null;
			tagName = null;
			attributeName = null;
			TokenHandler.EndTokenization();
			if (attributes != null)
			{
				attributes.Clear(mappingLangToXmlLang);
				attributes = null;
			}
		}

		public void requestSuspension()
		{
			shouldSuspend = true;
		}

		// [NOCPP[

		public void becomeConfident()
		{
			confident = true;
		}

		/**
		 * Returns the nextCharOnNewLine.
		 * 
		 * @return the nextCharOnNewLine
		 */
		public bool isNextCharOnNewLine()
		{
			return false;
		}

		public bool isPrevCR()
		{
			return lastCR;
		}

		/**
		 * Returns the line.
		 * 
		 * @return the line
		 */
		public int getLine()
		{
			return -1;
		}

		/**
		 * Returns the col.
		 * 
		 * @return the col
		 */
		public int getCol()
		{
			return -1;
		}

		// ]NOCPP]

		public bool isInDataState()
		{
			return (stateSave == DATA);
		}

		public void resetToDataState()
		{
			strBufLen = 0;
			longStrBufLen = 0;
			stateSave = Tokenizer.DATA;
			// line = 1; XXX line numbers
			lastCR = false;
			index = 0;
			forceQuirks = false;
			additional = '\u0000';
			entCol = -1;
			firstCharKey = -1;
			lo = 0;
			hi = 0; // will always be overwritten before use anyway
			candidate = -1;
			strBufMark = 0;
			prevValue = -1;
			value = 0;
			seenDigits = false;
			endTag = false;
			shouldSuspend = false;
			initDoctypeFields();
			if (tagName != null)
			{
				tagName = null;
			}
			if (attributeName != null)
			{
				attributeName = null;
			}
			// [NOCPP[
			if (newAttributesEachTime)
			{
				// ]NOCPP]
				if (attributes != null)
				{
					attributes = null;
				}
				// [NOCPP[
			}
			// ]NOCPP]
		}

		public void loadState(Tokenizer other)
		{
			strBufLen = other.strBufLen;
			if (strBufLen > strBuf.Length)
			{
				strBuf = new char[strBufLen];
			}
			Array.Copy(other.strBuf, strBuf, strBufLen);

			longStrBufLen = other.longStrBufLen;
			if (longStrBufLen > longStrBuf.Length)
			{
				longStrBuf = new char[longStrBufLen];
			}
			Array.Copy(other.longStrBuf, longStrBuf, longStrBufLen);

			stateSave = other.stateSave;
			returnStateSave = other.returnStateSave;
			endTagExpectation = other.endTagExpectation;
			endTagExpectationAsArray = other.endTagExpectationAsArray;
			// line = 1; XXX line numbers
			lastCR = other.lastCR;
			index = other.index;
			forceQuirks = other.forceQuirks;
			additional = other.additional;
			entCol = other.entCol;
			firstCharKey = other.firstCharKey;
			lo = other.lo;
			hi = other.hi;
			candidate = other.candidate;
			strBufMark = other.strBufMark;
			prevValue = other.prevValue;
			value = other.value;
			seenDigits = other.seenDigits;
			endTag = other.endTag;
			shouldSuspend = false;

			if (other.doctypeName == null)
			{
				doctypeName = null;
			}
			else
			{
				doctypeName = Portability.NewLocalFromLocal(other.doctypeName);
			}

			if (other.systemIdentifier == null)
			{
				systemIdentifier = null;
			}
			else
			{
				systemIdentifier = Portability.NewStringFromString(other.systemIdentifier);
			}

			if (other.publicIdentifier == null)
			{
				publicIdentifier = null;
			}
			else
			{
				publicIdentifier = Portability.NewStringFromString(other.publicIdentifier);
			}

			if (other.tagName == null)
			{
				tagName = null;
			}
			else
			{
				tagName = other.tagName.CloneElementName();
			}

			if (other.attributeName == null)
			{
				attributeName = null;
			}
			else
			{
				attributeName = other.attributeName.CloneAttributeName();
			}

			if (other.attributes == null)
			{
				attributes = null;
			}
			else
			{
				attributes = other.attributes.CloneAttributes();
			}
		}

		public void initializeWithoutStarting()
		{
			confident = false;
			strBuf = new char[64];
			longStrBuf = new char[1024];
			line = 1;
			// [NOCPP[
			html4 = false;
			metaBoundaryPassed = false;
			wantsComments = TokenHandler.WantsComments;
			if (!newAttributesEachTime)
			{
				attributes = new HtmlAttributes(mappingLangToXmlLang);
			}
			// ]NOCPP]
			resetToDataState();
		}

		protected void errGarbageAfterLtSlash()
		{
		}

		protected void errLtSlashGt()
		{
		}

		protected void errWarnLtSlashInRcdata()
		{
		}

		protected void errHtml4LtSlashInRcdata(char folded)
		{
		}

		protected void errCharRefLacksSemicolon()
		{
		}

		protected void errNoDigitsInNCR()
		{
		}

		protected void errGtInSystemId()
		{
		}

		protected void errGtInPublicId()
		{
		}

		protected void errNamelessDoctype()
		{
		}

		protected void errConsecutiveHyphens()
		{
		}

		protected void errPrematureEndOfComment()
		{
		}

		protected void errBogusComment()
		{
		}

		protected void errUnquotedAttributeValOrNull(char c)
		{
		}

		protected void errSlashNotFollowedByGt()
		{
		}

		protected void errHtml4XmlVoidSyntax()
		{
		}

		protected void errNoSpaceBetweenAttributes()
		{
		}

		protected void errHtml4NonNameInUnquotedAttribute(char c)
		{
		}

		protected void errLtOrEqualsOrGraveInUnquotedAttributeOrNull(char c)
		{
		}

		protected void errAttributeValueMissing()
		{
		}

		protected void errBadCharBeforeAttributeNameOrNull(char c)
		{
		}

		protected void errEqualsSignBeforeAttributeName()
		{
		}

		protected void errBadCharAfterLt(char c)
		{
		}

		protected void errLtGt()
		{
		}

		protected void errProcessingInstruction()
		{
		}

		protected void errUnescapedAmpersandInterpretedAsCharacterReference()
		{
		}

		protected void errNotSemicolonTerminated()
		{
		}

		protected void errNoNamedCharacterMatch()
		{
		}

		protected void errQuoteBeforeAttributeName(char c)
		{
		}

		protected void errQuoteOrLtInAttributeNameOrNull(char c)
		{
		}

		protected void errExpectedPublicId()
		{
		}

		protected void errBogusDoctype()
		{
		}

		protected void maybeWarnPrivateUseAstral()
		{
		}

		protected void maybeWarnPrivateUse(char ch)
		{
		}

		protected void maybeErrAttributesOnEndTag(HtmlAttributes attrs)
		{
		}

		protected void maybeErrSlashInEndTag(bool selfClosing)
		{
		}

		protected char errNcrNonCharacter(char ch)
		{
			return ch;
		}

		protected void errAstralNonCharacter(int ch)
		{
		}

		protected void errNcrSurrogate()
		{
		}

		protected char errNcrControlChar(char ch)
		{
			return ch;
		}

		protected void errNcrCr()
		{
		}

		protected void errNcrInC1Range()
		{
		}

		protected void errEofInPublicId()
		{
		}

		protected void errEofInComment()
		{
		}

		protected void errEofInDoctype()
		{
		}

		protected void errEofInAttributeValue()
		{
		}

		protected void errEofInAttributeName()
		{
		}

		protected void errEofWithoutGt()
		{
		}

		protected void errEofInTagName()
		{
		}

		protected void errEofInEndTag()
		{
		}

		protected void errEofAfterLt()
		{
		}

		protected void errNcrOutOfRange()
		{
		}

		protected void errNcrUnassigned()
		{
		}

		protected void errDuplicateAttribute()
		{
		}

		protected void errEofInSystemId()
		{
		}

		protected void errExpectedSystemId()
		{
		}

		protected void errMissingSpaceBeforeDoctypeName()
		{
		}

		protected void errHyphenHyphenBang()
		{
		}

		protected void errNcrControlChar()
		{
		}

		protected void errNcrZero()
		{
		}

		protected void errNoSpaceBetweenDoctypeSystemKeywordAndQuote()
		{
		}

		protected void errNoSpaceBetweenPublicAndSystemIds()
		{
		}

		protected void errNoSpaceBetweenDoctypePublicKeywordAndQuote()
		{
		}

		protected void noteAttributeWithoutValue()
		{
		}

		protected void noteUnquotedAttributeValue()
		{
		}

		// [NOCPP[

		/// <summary>
		/// Sets an offset to be added to the position reported to
		/// <code>TransitionHandler</code>.
		/// </summary>
		/// <param name="offset">The offset.</param>
		public void SetTransitionBaseOffset(int offset)
		{
			// TODO: nothing done here??
		}

		// ]NOCPP]
	}
}
