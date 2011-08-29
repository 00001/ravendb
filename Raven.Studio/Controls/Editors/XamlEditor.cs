﻿namespace Raven.Studio.Controls.Editors
{
	using ActiproSoftware.Text;
	using ActiproSoftware.Windows.Controls.SyntaxEditor;

	public class XamlEditor : SyntaxEditor
	{
		private static readonly ISyntaxLanguage DefaultLanguage;

		static XamlEditor()
		{
			DefaultLanguage = SyntaxEditorHelper.LoadLanguageDefinitionFromResourceStream("Xaml.langdef");
		}

		public XamlEditor()
		{
			Document.Language = DefaultLanguage;
			IsTextDataBindingEnabled = true;
		}
	}
}