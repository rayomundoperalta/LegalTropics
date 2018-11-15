using Microsoft.Office.Interop.Word;

namespace LegalTropics
{
    public static class Utility
    {
        public static void InsertText(Selection currentSelection, string text)
        {
            if (currentSelection.Type == WdSelectionType.wdSelectionIP)
            {
                currentSelection.TypeText(text);
                currentSelection.TypeParagraph();
            }
            else
            {
                if (currentSelection.Type == WdSelectionType.wdSelectionNormal)
                {
                    if (Globals.ThisAddIn.Application.Options.ReplaceSelection)
                    {
                        object direction = WdCollapseDirection.wdCollapseStart;
                        currentSelection.Collapse(ref direction);
                    }
                    currentSelection.TypeText(text);
                    currentSelection.TypeParagraph();

                }
            }
        }
    }
}
