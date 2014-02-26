using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using NPOI.HSSF.UserModel;
using NPOI.HPSF;
using NPOI.POIFS.FileSystem;

namespace SiteWordsExtractor
{
    class ElementsCounter
    {
        public static readonly ILog log = LogManager.GetLogger(typeof(ElementsCounter));

        private HtmlProcessor m_processor;
        private WordsCounter m_wordsCounter;
        private Dictionary<string, int> m_elementsCount;
        private Dictionary<string, int> m_elementsWordCount;

        public ElementsCounter(HtmlProcessor processor)
        {
            m_wordsCounter = new WordsCounter(AppSettings.Settings.WordsCounter.RegEx);

            m_elementsCount = new Dictionary<string, int>();
            m_elementsWordCount = new Dictionary<string, int>();

            m_processor = processor;
            m_processor.OnAttribute += OnText;
            m_processor.OnText += OnText;
            m_processor.OnBoldText += OnText;
            m_processor.OnHyperlink += OnHyperlink;
        }

        private void OnText(object sender, string text)
        {
            if (m_elementsCount.ContainsKey(text))
            {
                m_elementsCount[text]++;
            }
            else
            {
                m_elementsCount.Add(text, 1);
                m_elementsWordCount.Add(text, m_wordsCounter.CountWords(text));
            }
        }

        private void OnHyperlink(object sender, HyperlinkEventArgs args)
        {
            OnText(sender, args.Text);
        }
        
        public void SaveAsXls(string xlsFilepath)
        {
            HSSFWorkbook workbook = new HSSFWorkbook();

            //create a entry of DocumentSummaryInformation
            DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            dsi.Company = "Hever Translations";
            workbook.DocumentSummaryInformation = dsi;

            //create a entry of SummaryInformation
            SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
            si.Subject = "Statistics - auto generated file";
            workbook.SummaryInformation = si;

            HSSFSheet sheet = (HSSFSheet)workbook.CreateSheet("statistics");

            NPOI.SS.UserModel.IRow headerRow = sheet.CreateRow(0);
            headerRow.CreateCell(0).SetCellValue("Text");
            headerRow.CreateCell(1).SetCellValue("Words in Text");
            headerRow.CreateCell(2).SetCellValue("Instances of Text");
            //HSSFFont headerFont = workbook.CreateFont();
            NPOI.SS.UserModel.IFont headerFont = workbook.CreateFont();
            headerFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;
            headerRow.Cells[0].CellStyle = workbook.CreateCellStyle();
            headerRow.Cells[0].CellStyle.SetFont(headerFont);
            headerRow.Cells[1].CellStyle = workbook.CreateCellStyle();
            headerRow.Cells[1].CellStyle.SetFont(headerFont);
            headerRow.Cells[2].CellStyle = workbook.CreateCellStyle();
            headerRow.Cells[2].CellStyle.SetFont(headerFont);

            int totalWordsInText = 0;
            int totalInstances = 0;
            int rowNumber = 0;
            foreach (KeyValuePair<string, int> kvp in m_elementsCount)
            {
                string text = kvp.Key;
                int count = kvp.Value;
                int wordsCount = m_elementsWordCount[kvp.Key];

                totalWordsInText += wordsCount;
                totalInstances += count;

                rowNumber++;
                NPOI.SS.UserModel.IRow row = sheet.CreateRow(rowNumber);
                row.CreateCell(0).SetCellValue(text);
                row.CreateCell(1).SetCellValue(wordsCount);
                row.CreateCell(2).SetCellValue(count);
            }
            rowNumber++;
            rowNumber++;
            sheet.CreateRow(rowNumber).CreateCell(0).SetCellValue("Total words: " + totalWordsInText.ToString());
            rowNumber++;
            sheet.CreateRow(rowNumber).CreateCell(0).SetCellValue("Total instances: " + totalInstances.ToString());

            sheet.CreateFreezePane(0, 1, 0, 1);

            FileStream file = new FileStream(xlsFilepath, FileMode.Create);
            workbook.Write(file);
            file.Close();
        }
    }
}
