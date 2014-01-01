using System;
using System.Windows.Forms;
using System.IO;
using pdftron;
using pdftron.PDF;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Office.Interop.Excel;
using System.Drawing;

namespace PDFNavigator
{
    public partial class KeywordSearchForm : Form
    {
        private string sourceFilesPath;

        //用于存储“文件：文件内容”对
        Hashtable fileContentTable = new Hashtable();
        Hashtable keywordList = new Hashtable();
        Hashtable siftedKeywordList = new Hashtable();
        List<KeywordInfo> sortingList = new List<KeywordInfo>();
        //用于抽取用户提供的关键字的索引文件
        private string[] keywords;

        XmlDocument descDoc;
        private XmlNode rootNode;

        public KeywordSearchForm()
        {
            PDFNet.Initialize();
            init();

            InitializeComponent();
        }

        private void init()
        {
            //初始化XML描述文档
            descDoc = new XmlDocument();

            //创建文件头
            XmlDeclaration d = descDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
            descDoc.AppendChild(d);

            //创建根节点
            rootNode = descDoc.CreateElement("Keyword");
        }

        private void sourceBrowseButton_Click(object sender, EventArgs e)
        {
            DialogResult result = this.folderBrowserDialog1.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                this.sourcePathTextbox.Text = this.sourceFilesPath 
                    = this.folderBrowserDialog1.SelectedPath;
            }
        }

        private void executeButton_Click(object sender, EventArgs e)
        {
            if(this.sourcePathTextbox.Text != String.Empty && this.keywordTextbox.Text != String.Empty)
            this.keywords = this.keywordTextbox.Text.Trim().Split(' ');

            foreach (string keyword in this.keywords)
            { 
                //对每一个keyword，进行在文件中进行遍历搜索
                init();
                processSearch(keyword);
                saveDescFile(keyword);
            }
            MessageBox.Show("已完成");
        }

        private void saveDescFile(String keyword)
        {
            descDoc.AppendChild(rootNode);
            descDoc.Save(this.sourcePathTextbox.Text + "\\" + keyword + ".xml");
        }

        private void processSearch(string keyword)
        {
            DirectoryInfo dInfo = new DirectoryInfo(this.sourcePathTextbox.Text);
            foreach (FileInfo info in dInfo.GetFiles())
            {
                if (info.Extension == ".pdf" || info.Extension == ".PDF")
                searchInFile(keyword, info.FullName);
            }
        }

        private void searchInFile(string keyword, string fileName)
        {
            //首先初始化extractor
            TextExtractor extractor = new TextExtractor();

            PDFDoc doc = new PDFDoc(fileName);
            int pageCount = doc.GetPageCount();

            for (int index = 1; index <= pageCount; ++index)
            {
                pdftron.PDF.Page page = doc.GetPage(index);
                if (page != null)
                {

                    extractor.Begin(page);
                    //对于每一行
                    for (TextExtractor.Line line = extractor.GetFirstLine(); line.IsValid(); line = line.GetNextLine())
                    { 
                        String lineValue = String.Empty;
                        //对于每一个单词（用空格隔开）
                        for (TextExtractor.Word word = line.GetFirstWord(); word.IsValid(); word = word.GetNextWord())
                        {
                            lineValue += word.GetString() + " ";
                        }
                        //获取一行文字
                        lineValue = lineValue.TrimEnd();

                        //将这行文字进行搜索，检查里面是否有关键字，若有，获取其相关位置的数据
                        searchKeywordPerLine(keyword, lineValue, line, page.GetIndex(), fileName);
                    }
                }
            }
        }

        private void searchKeywordPerLine(string keyword, string lineValue, TextExtractor.Line line, int pageCount, String fileName)
        {
            //对于这一行将其用空格拆开
            for (TextExtractor.Word word = line.GetFirstWord(); word.IsValid(); word = word.GetNextWord())
            { 
                //如果某一个片段包含keyword，返回该keyword的相关信息
                Regex regex = new Regex(keyword);
                string value = word.GetString();
                MatchCollection collection = regex.Matches(value);
                if (collection.Count > 0)
                {
                    foreach (Match m in collection)
                    {
                        //负责存放一个匹配的每一个字符的坐标值
                        double[,] coordinates = new double[keyword.Length, 8];
                        int i = 0;
                        //对于关键字中的每一个字符
                        for (int index = m.Index; index < m.Index + keyword.Length; ++index)
                        { 
                            double[] c = word.GetGlyphQuad(index);
                            for (int count = 0; count < 8; ++count)
                            {
                                coordinates[i, count] = c[count];
                            }
                            ++i;
                        }

                        //对于每一个匹配，生成一个节点，包括：文档名称，页号，行号，每个字符的坐标值
                        createXMLNode(pageCount, line.GetCurrentNum(), coordinates, keyword, fileName);
                    }
                }
            }
        }

        void createXMLNode(int pageCount, int lineCount, double[,] coordinates, string keyword, string fileName)
        {
            XmlElement node = descDoc.CreateElement("case");
            string [] temp = fileName.Split(new string[] { "\\" }, StringSplitOptions.None);
            string shortFileName = temp[temp.Length - 1];
            node.SetAttribute("Document", shortFileName);
            node.SetAttribute("PageCount", pageCount.ToString());
            node.SetAttribute("lineCount", lineCount.ToString());

            //生成keyword的整体坐标（左下，右上）
            XmlElement keywordCor = descDoc.CreateElement("KeywordCorners");
            keywordCor.SetAttribute("Value", keyword);
            
            XmlElement left_down = descDoc.CreateElement("left-down");
            left_down.SetAttribute("x", coordinates[0, 0].ToString());
            left_down.SetAttribute("y", coordinates[0, 1].ToString());
            XmlElement upper_right = descDoc.CreateElement("upper-right");
            upper_right.SetAttribute("x", coordinates[keyword.Length - 1, 4].ToString());
            upper_right.SetAttribute("y", coordinates[keyword.Length - 1, 5].ToString());
            
            keywordCor.AppendChild(left_down);
            keywordCor.AppendChild(upper_right);
            
            node.AppendChild(keywordCor);
            
            //生成每个字符的坐标
            int index = 0;
            foreach (char c in keyword)
            {
                //坐标节点
                XmlElement cCornerNode = descDoc.CreateElement("WordCorners");
                cCornerNode.SetAttribute("Value", c.ToString());
                //左下
                XmlElement cCoordinateNode1 = descDoc.CreateElement("left-down");
                cCoordinateNode1.SetAttribute("x", coordinates[index, 0].ToString());
                cCoordinateNode1.SetAttribute("y", coordinates[index, 1].ToString());
                //右下
                XmlElement cCoordinateNode2 = descDoc.CreateElement("right-down");
                cCoordinateNode2.SetAttribute("x", coordinates[index, 2].ToString());
                cCoordinateNode2.SetAttribute("y", coordinates[index, 3].ToString());
                //右上
                XmlElement cCoordinateNode3 = descDoc.CreateElement("upper-right");
                cCoordinateNode3.SetAttribute("x", coordinates[index, 4].ToString());
                cCoordinateNode3.SetAttribute("y", coordinates[index, 5].ToString());
                //左上
                XmlElement cCoordinateNode4 = descDoc.CreateElement("upper-left");
                cCoordinateNode4.SetAttribute("x", coordinates[index, 6].ToString());
                cCoordinateNode4.SetAttribute("y", coordinates[index, 7].ToString());

                cCornerNode.AppendChild(cCoordinateNode1);
                cCornerNode.AppendChild(cCoordinateNode2);
                cCornerNode.AppendChild(cCoordinateNode3);
                cCornerNode.AppendChild(cCoordinateNode4);

                ++index;

                node.AppendChild(cCornerNode);
            }

            rootNode.AppendChild(node);
        }

        private void statisticButton_Click(object sender, EventArgs e)
        {
            //首先读入所有的文档，生成关键字序列
            initKeywordList();

            //统计关键词数量！！！
            statisticKeyword();

            //排序！！！！！！！
            sortKeywordList();

            //将关键词的统计结果放入文本文件中
            saveKeyword();
        }

        private void sortKeywordList()
        {
            setProcessTip("正在排序");
            
            //先将不够搜索阈值的项去掉
            siftRedundantKeyword();
            
            //将其拷贝到一个普通的list中
            double count = 0;
            double sum = keywordList.Count;
            foreach (KeywordInfo ki in this.siftedKeywordList.Values)
            {
                sortingList.Add(ki);
                setProcessBar((++count) / sum * 0.25 + 0.5);
            }

            //用新定义的排序方法进行排序
            sortingList.Sort
                (
                    (x, y) => { return -(x.sumCount.CompareTo(y.sumCount)); }
                );

            setProcessBar(0.75);
        }

        private void siftRedundantKeyword()
        {
            int thresholdValue = getThresholdValue();
            foreach (string kw in this.keywordList.Keys)
            {
                //如果符合要求
                if (((KeywordInfo)this.keywordList[kw]).sumCount > thresholdValue)
                {
                    this.siftedKeywordList.Add(kw, this.keywordList[kw]);
                }
            }
            
        }

        private int getThresholdValue()
        {
            switch (this.ThresholdComboBox.SelectedItem.ToString())
            {
                case ">5":
                    return 5;
                case ">50":
                    return 50;
                case ">100":
                    return 100;
                case ">500":
                    return 500;
                case ">800":
                    return 800;
                case ">1000":
                    return 1000;
                case ">5000":
                    return 5000;
                case ">10000":
                    return 10000;
                case ">20000":
                    return 20000;
                case ">100000":
                    return 100000;                    
            }
            return -1;
        }

        //private void saveKeyword()
        //{
        //    setProcessTip("正在保存统计信息");
        //    FileStream statisticFile = new FileStream(
        //        this.sourceFilesPath + "\\" + "keyword" + this.keywordLengthNumericUpDown.Value.ToString() + ".txt", 
        //        FileMode.Create);

        //    using (StreamWriter writer = new StreamWriter(statisticFile))
        //    {
        //        writer.WriteLine("关键词\t总计\t\t出现文件\t\t出现页码\t\t每页出现");

        //        //对于每一个关键词
        //        int count = 0;
        //        int sum = this.sortingList.Count;
        //        foreach (KeywordInfo info in this.sortingList)
        //        {
        //            writer.WriteLine
        //                (info.keyword + "\t" + info.sumCount 
        //                + "\t\t    " + "\t\t    " + "\t\t    ");

        //            foreach (KeywordInfo.Record record in info.recordList)
        //            {
        //                writer.WriteLine("   \t  \t\t" + record.fileName + "\t\t" +　record.page + "\t\t" + record.count);
        //            }
        //            this.setProcessBar(++count / sum * 0.25 + 0.75);
        //        }
        //        writer.Close();
        //        statisticFile.Close();
        //    }

        //    this.setProcessBar(1);
        //    this.setProcessTip("已完成");
        //}

        //private void statisticKeyword()
        //{
        //    //新建一个搜索器
        //    TextSearch searcher = new TextSearch();
        //    int mode = (int)(TextSearch.SearchMode.e_whole_word | TextSearch.SearchMode.e_highlight);
        //    int pageNum = 0;
        //    string resultStr = string.Empty;
        //    string ambientStr = string.Empty;
        //    int count = 0;
        //    int sum = this.keywordList.Count;
        //    foreach (string keyword in this.keywordList.Keys)
        //    {
        //        //向前推进滚动条
        //        setProcessBar(count++ / sum * 0.25);

        //        //搜索每一个文件
        //        foreach (FileInfo fi in new DirectoryInfo(this.sourceFilesPath).GetFiles())
        //        {
        //            PDFDoc doc = new PDFDoc(fi.FullName);
        //            Highlights highlight = new Highlights();
        //            searcher.Begin(doc, keyword, mode, -1, -1);

        //            while (true)
        //            {
        //                TextSearch.ResultCode code = searcher.Run(ref pageNum, ref resultStr, ref ambientStr, highlight);
        //                //如果找到了
        //                if (code == TextSearch.ResultCode.e_found)
        //                {
        //                    if (this.keywordList[keyword] == null)
        //                    {
        //                        this.keywordList[keyword] = new KeywordInfo(keyword);
        //                    }
        //                    ((KeywordInfo)this.keywordList[keyword]).setPage(pageNum);
        //                }
        //                //否则，退出本文档的搜索
        //                else
        //                    break;
        //            }
        //        }
        //    }
        //}
        private void saveKeyword() 
        {
            setProcessTip("正在保存文件");

            object miss = Missing.Value;
            Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();
            Workbook book = excelApp.Workbooks.Add(true);

            Worksheet workSheet = book.Worksheets[1] as Worksheet;

            //先插入表头
            object[,] columnTitle = new object[1, 5];
            columnTitle[0, 0] = "关键词";
            columnTitle[0, 1] = "总计";
            columnTitle[0, 2] = "出现文件";
            columnTitle[0, 3] = "出现页码";
            columnTitle[0, 4] = "每页出现";

            //插入文本
            Range range = workSheet.Range[workSheet.Cells[1, 1], workSheet.Cells[1, 5]];
            range.Value2 = columnTitle;
            range.Interior.Color = Color.Blue;

            //插入统计内容
            int lineCount = 1;//用于excel表格的行号计数
            int startLineCount = 0;//用于标注每个关键字的起始行号
            
            double count = 0;
            double sum = this.sortingList.Count;
            foreach (KeywordInfo info in this.sortingList)
            {
                int index = 0;//用于在一个关键字内的计数
                ++lineCount;
                startLineCount = lineCount + 1;
                //共info.recordList.Count + 1行
                object[,] content = new object[info.recordList.Count + 1, 5];
                content[index, 0] = info.keyword;
                content[index, 1] = info.sumCount.ToString();
                ++index;
                foreach (KeywordInfo.Record record in info.recordList)
                {
                    ++lineCount;
                    content[index, 2] = record.fileName;
                    content[index, 3] = record.page;
                    content[index, 4] = record.count;
                    ++index;
                }

                Range r = workSheet.Range[workSheet.Cells[startLineCount, 1], workSheet.Cells[startLineCount + info.recordList.Count, 5]];
                r.Value2 = content;

                Range r2 = workSheet.Range[workSheet.Cells[startLineCount, 1], workSheet.Cells[startLineCount, 5]];
                r2.Interior.Color = Color.LightCyan;
                //workSheet.get_Range(workSheet.Cells[startLineCount, 1], workSheet.Cells[startLineCount + info.recordList.Count + 1, 5]).Value2 = content;

                setProcessBar((++count) / sum * 0.25 + 0.75);
            }

            //保存
            excelApp.Workbooks[1].SaveAs(this.sourcePathTextbox.Text + @"\" + "keyword" + this.keywordLengthNumericUpDown.Value + ".xls", miss, miss, miss, miss, miss, XlSaveAsAccessMode.xlShared, miss, miss, miss, miss, miss);
            setProcessBar(1);
            setProcessTip("已完成。已将结果保存至：" + this.sourcePathTextbox.Text + @"\" + "keyword" + this.keywordLengthNumericUpDown.Value + ".xls");
        }

        private void statisticKeyword()
        { 
            //主题思路是：一个文件一个文件的搜索，在每个文件中队所有的关键字遍历一遍
            setProcessTip("正在搜索关键字");
            double count = 0;
            DirectoryInfo di = new DirectoryInfo(this.sourcePathTextbox.Text);
            double sum = di.GetFiles().Length;
            
            foreach (FileInfo fi in di.GetFiles())
            {
                if (fi.Extension == ".pdf" || fi.Extension == ".PDF")
                {
                    searchTempKeywordInFile(fi.FullName, fi.Name);
                    setProcessBar((++count) / sum * 0.25 + 0.25);
                }
            }
            setProcessBar(0.5);
        }

        private void searchTempKeywordInFile(string fileName, string shortName)
        {
            //在每一个文件中，遍历所有的临时关键字
            #region 1.建立一个PDF doc,一个extractor
            PDFDoc doc = new PDFDoc(fileName);
            TextExtractor extractor = new TextExtractor();
            #endregion

            #region 2.开始进行逐页的搜索，提取出一页的所有文字，将所有的关键字检查一遍
            int pageCount = doc.GetPageCount();

            //对于每一页
            string pageValue = string.Empty;
            for (int pageIndex = 1; pageIndex <= pageCount; ++pageIndex)
            {
                pdftron.PDF.Page page = doc.GetPage(pageIndex);
                if (page != null)
                {
                    pageValue = string.Empty;
                    
                    extractor.Begin(page);
                    //对于每一行
                    String lineValue = String.Empty;
                    for (TextExtractor.Line line = extractor.GetFirstLine(); line.IsValid(); line = line.GetNextLine())
                    {
                        lineValue = string.Empty;
                        //对于每一个单词（用空格隔开）
                        for (TextExtractor.Word word = line.GetFirstWord(); word.IsValid(); word = word.GetNextWord())
                        {
                            lineValue += word.GetString() + " ";
                        }
                        //获取一行文字
                        lineValue = lineValue.TrimEnd();

                        //累积一页文字
                        pageValue += lineValue;

                        
                    }
                    #region 3.将这页文字进行搜索，逐一遍历关键字
                    foreach (string keyword in this.keywordList.Keys)
                    {
                        searchKeywordInPage(keyword, pageValue, pageIndex, shortName);
                    }
                    #endregion
                }
            }
            #endregion
        }

        private void searchKeywordInPage(string keyword, string pageValue, int pageNum, string fileName)
        {
            Regex regex = new Regex(keyword);

            MatchCollection collection = regex.Matches(pageValue);

            if(collection.Count > 0)
            {
                ((KeywordInfo)this.keywordList[keyword]).addRecord(fileName, pageNum, collection.Count);
            }
        }

        private void initKeywordList()
        {
            setProcessTip("正在读取文档");
            //循环读入所有的文件，将每一个文档练成一个字符串
            DirectoryInfo di = new DirectoryInfo(this.sourcePathTextbox.Text);
            double count = 0;
            double sum = di.GetFiles().Length;
            foreach (FileInfo fi in di.GetFiles())
            { 
                //提取其文档中的文字,将其加入hashtable
                if (fi.Extension == ".pdf" || fi.Extension == ".PDF")
                {
                    this.fileContentTable.Add(fi.Name, extractWords(fi.FullName));
                    setProcessBar((++count) / sum * 0.25);
                }
            }

            //生成关键字序列!!!
            setProcessTip("正在生成关键词");
            extractKeywords();
        }

        private void extractKeywords()
        {
            int keywordLength = (int)this.keywordLengthNumericUpDown.Value;

            double count = 0;
            double sum = this.fileContentTable.Count;

            foreach (string content in this.fileContentTable.Values)
            { 
                //对于每一篇文档，提取关键字
                for (int index = 0; index < content.Length - keywordLength + 1; ++index)
                {
                    string tempKeyword = content.Substring(index, keywordLength);
                    //如果关键字中不包含标点符号等无用信息（即纯文字）
                    if (isPureGlyph(tempKeyword))
                    {
                        //如果不包含该关键字，则加入其中，其中的value空缺，以供后来统计结果用
                        if (!this.keywordList.ContainsKey(tempKeyword))
                            this.keywordList.Add(tempKeyword, new KeywordInfo(tempKeyword));
                    }
                }

                setProcessBar(count / sum * 0.25 + 0.25);
            }
        }

        private bool isPureGlyph(string tempKeyword)
        {
            foreach (char c in tempKeyword)
            {
                if (Char.IsPunctuation(c))
                    return false;
            }

            return true;
        }

        private void setProcessBar(double value)
        {
            this.progressBar.Value = (int)(value * 100);
        }

        private string extractWords(string fileName)
        {
            TextExtractor extractor = new TextExtractor();

            //读入一个文件
            PDFDoc doc = new PDFDoc(fileName);
            int pageCount = doc.GetPageCount();

            //用于存放一个文档的所有文字
            string docStrContent = string.Empty;
            for (int index = 1; index <= pageCount; ++index)
            {
                pdftron.PDF.Page page = doc.GetPage(index);
                if (page != null)
                {

                    extractor.Begin(page);
                    //对于每一行
                    for (TextExtractor.Line line = extractor.GetFirstLine(); line.IsValid(); line = line.GetNextLine())
                    {
                        String lineValue = String.Empty;
                        //对于每一个单词（用空格隔开）
                        for (TextExtractor.Word word = line.GetFirstWord(); word.IsValid(); word = word.GetNextWord())
                        {
                            lineValue += word.GetString() + " ";
                        }
                        //获取一行文字
                        lineValue = lineValue.TrimEnd();

                        docStrContent += lineValue;                        
                    }
                }
            }

            return docStrContent;
        }

        private void setProcessTip(string p)
        {
            this.processTip.Text = "状态： " + p;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
