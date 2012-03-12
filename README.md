HtmlParserSharp
===============

This is a manual C# port of the [Validator.nu HTML Parser](http://about.validator.nu/htmlparser/), a HTML5 parser originally written in Java and (compiled to C++ using the Google Web Toolkit) used by Mozilla's Gecko rendering engine. The port uses the DOM implemented in [System.Xml](http://msdn.microsoft.com/en-us/library/system.xml.aspx).

Status
------

Currently the port seems to work (as far as I have tested it), but as there are no unit tests, I'm not sure how well it really works. Tests showed that it is quite fast (about 3-6 times slower than parsing XML using .NET's XDocument API, but I think XML parsing is easier to implement, so this is okay).

What's missing
--------------
If you want to contribute, maybe you can start here:

* Support for character encodings other than UTF-8
* Unit tests