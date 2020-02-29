using System;
using System.IO;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Xml;

namespace VTility.Pages
{
    /// <summary>
    /// Interaktionslogik für PageNotes.xaml
    /// </summary>
    public partial class PageNotes : BasePage
    {
        public PageNotes()
        {
            InitializeComponent();
        }

        public override void LoadSettings()
        {
            var rteContent = LoadPageSetting("rte_content");
            if (rteContent != null && !rteContent.Equals(false))
            {
                var daContent = Convert.ToString(rteContent);
                if (daContent == null)
                    daContent = "";

                XmlReader xmlReader = XmlReader.Create(new StringReader(daContent));
                richTextBox.Document = XamlReader.Load(xmlReader) as FlowDocument;
            }
        }

        public override void SaveSettings()
        {
            SavePageSetting("rte_content", FlowDocumentToXml(richTextBox.Document));
        }

        private static string FlowDocumentToXml(FlowDocument doc)
        {
            return XamlWriter.Save(doc);
        }

        /// <summary>
        /// Deprecated
        /// </summary>
        /// <param name="xamlString"></param>
        /// <returns></returns>
        private static FlowDocument XMLtoFlowDocument(string xamlString)
        {
            FlowDocument doc = new FlowDocument();
            try
            {
                StringReader stringReader = new StringReader(xamlString);
                XmlReader xmlReader = XmlReader.Create(stringReader);
                Section sec = XamlReader.Load(xmlReader) as Section;
                while (sec.Blocks.Count > 0)
                {
                    var block = sec.Blocks.FirstBlock;
                    sec.Blocks.Remove(block);
                    doc.Blocks.Add(block);
                }
                return doc;
            }
            catch (Exception e)
            {
                Console.WriteLine("error... " + e);
            }
            return doc;
        }

        private FlowDocument StringToFlowDocument(string s)
        {
            var xamlString = string.Format("<FlowDocument xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\"><Paragraph>{0}</Paragraph></FlowDocument>", s);
            return XamlReader.Parse(xamlString) as FlowDocument;
        }
    }
}