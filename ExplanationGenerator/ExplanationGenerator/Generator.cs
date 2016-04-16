using System;
using System.IO;

namespace ExplanationGenerator
{
    class Generator
    {
        string template = @"<!DOCTYPE html>
<html>
<head>
	<meta charset=""utf-8"">
	<title>Explanations</title>
	<link rel = ""stylesheet"" href=""./highlight/styles/dracula.css"">
	<script src = ""./highlight/highlight.pack.js"" ></script>

	<style media=""screen"">
	body {{
		background: #cecece;
		margin: 0;
		padding: 0;
	}}
	.hljs {{
		padding: 1em;
		overflow-y: auto;
		overflow-x: hidden;
		line-height: 1.5;
	}}
	pre {{
		margin: 0;
	}}
	pre code
	{{
		font: normal 10pt Consolas, Monaco, monospace;
		min-height: 100vh;
	}}
	.source_code {{
		width: 50%;
		float: left;
	}}
	.explanations {{
		width: 50%;
		float: right;
	}}
    .explanations pre {{
        white-space: normal;
    }}
	.explanations pre code {{
		background: #cecece;
		color: #000000;
		padding: 0 1em 0 1em;
	}}
	.indent {{
		margin-left: 2em;
		padding: 1px 1px 1px 2px;
		background-color: rgba(255,255,255,0.4);
	}}
	</style>

	<script>hljs.initHighlightingOnLoad();</script>
</head>
<body>
	<div class=""container"">
		<div class=""source_code"">
			<pre><code class=""cpp"">{0}</code></pre>
		</div>
		<div class=""explanations"">
			<pre><code>{1}</code></pre>
		</div>
	</div>
</body>
</html>";

        internal void generate(string source, string translation, string fileName)
        {
            translation = replaceTranslation(translation);
            string output = string.Format(template, source, translation);

            File.WriteAllText(fileName, output);
        }

        public string replaceTranslation(string translation)
        {
            string translationReplaced = translation;
            translationReplaced = translationReplaced.Replace("[", @"<div class=""indent"">");
            translationReplaced = translationReplaced.Replace("]", @"</div>");
            translationReplaced = translationReplaced.Replace("|", @"<br>");

            return translationReplaced;
        }
    }
}
