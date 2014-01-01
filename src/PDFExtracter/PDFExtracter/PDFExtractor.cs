using System;
using System.Collections;
using System.Text.RegularExpressions;
using System.Xml;

using pdftron;
using pdftron.PDF;
using pdftron.Common;

namespace PDFExtractor
{
    public class PDFExtractor
    {
        //PDF文档解析器
        private TextExtractor extractor;

        private XmlDocument descDoc;
        private String pdfDescName;
        private XmlNode rootNode;
        //PDF文档
        private PDFDoc doc;


        static void Main(string[] args)
        {
            PDFExtractor pdfExtractor = new PDFExtractor();
            pdfExtractor.init(@"D:\Xia's Research\Search Platform\Resources\1.pdf");

            //开始分析PDF文档
            pdfExtractor.processPDF();

            //结束分析，关闭文档
            pdfExtractor.endProcess();
        }

        public void endProcess(String path = "")
        {
            if (path.Equals(""))
                descDoc.Save(this.pdfDescName);
            else
                descDoc.Save(path);
            extractor.Dispose();
            doc.Close();
        }

        public void processPDF()
        {
            //读入文件的基本信息
            readPDFinfo();

            //4.2.2011添加，读入定位点
            readLocationPoint();
            //读入内容
            readPDFContent();
        }

        private void readLocationPoint()
        {
            XmlElement locationPointsNode = descDoc.CreateElement("LocationPoints");

            ElementReader reader = new ElementReader();
            reader.Begin(this.doc.GetPage(1));
            Element element;
            int count = 0;
            while ((element = reader.Next()) != null)
            {
                switch (element.GetType())
                {
                    case Element.Type.e_image:
                    {
                        //Console.WriteLine("--> Image: {0}", ++image_counter);
                        //Console.WriteLine("    Width: {0}", element.GetImageWidth());
                        //Console.WriteLine("    Height: {0}", element.GetImageHeight());
                        //Console.WriteLine("    BPC: {0}", element.GetBitsPerComponent());

                        Matrix2D ctm = element.GetCTM();
                        double x2 = 1, y2 = 1;
                        ctm.Mult(ref x2, ref y2);
                        //分别是定位点的中点
                        Console.WriteLine("    Coords: x1={0}, y1={1}, x2={2}, y2={3}", ctm.m_h, ctm.m_v, x2, y2);

                        ///重要标注：ctm.m_h，ctm.m_v其实是贴图左下角的坐标，而非中心坐标
                        ///转化为中心坐标
                        double center_x = ctm.m_h + element.GetImageWidth() / 2;
                        double center_y = ctm.m_v + element.GetImageHeight() / 2;
                        createLocationDescNode(locationPointsNode, count, center_x, center_y);

                        if (++count == 4)
                        {
                            rootNode.AppendChild(locationPointsNode);
                            return;
                        }
                        break;
                    }
                }
            }
        }

        private void createLocationDescNode(XmlElement locationPointsNode, int count, double mid_x, double mid_y)
        {

            XmlElement locationPointNode = descDoc.CreateElement("LocationPoint");
            locationPointNode.SetAttribute("index", count.ToString());
            locationPointNode.SetAttribute("x", mid_x.ToString());
            locationPointNode.SetAttribute("y", mid_y.ToString());

            locationPointsNode.AppendChild(locationPointNode);
        }

        private void readPDFContent()
        {
            extractor = new TextExtractor();
            //添加内容的节点
            XmlElement contentRoot = descDoc.CreateElement("Content");

            int pageCount = doc.GetPageCount();
            for (int index = 1; index <= pageCount; ++index)
            {
                //创建page的节点
                XmlElement pageNode = descDoc.CreateElement("Page");

                Page page = doc.GetPage(index);
                page.GetMediaBox();

                if (page != null)
                {
                    extractor.Begin(page);

                    //设置pageNode的基本属性
                    setPageAttribute(index, pageNode, page);

                    //对于每一行
                    for (TextExtractor.Line line = extractor.GetFirstLine(); line.IsValid(); line = line.GetNextLine())
                    {
                        XmlElement lineNode = descDoc.CreateElement("Line");

                        //lineNode加入了坐标节点
                        setLineAttribute(line, lineNode);

                        int lineCharCount = -1;
                        String lineValue = String.Empty;
                        //对于每一个单词（用空格隔开）
                        for (TextExtractor.Word word = line.GetFirstWord(); word.IsValid(); word = word.GetNextWord())
                        {
                            Hashtable repeatWordsTable = new Hashtable();
                            //首先将lineNode的属性累加，跳出循环后赋值
                            lineCharCount += word.GetStringLen() + 1;
                            lineValue += word.GetString() + " ";

                            //分割单词为中文片段和非中文片段
                            String wordString = word.GetString();
                            ArrayList list = splitCN_EN(wordString);
                            //对于每一个由单词分割出来的片段（中文或非中文）
                            foreach (String[] combintn in list)
                            {
                                int pos = wordString.IndexOf(combintn[1]);;
                                ///于4.25.2011晚上注释，将拆除中英文字符的正则表达式改为
                                ///识别汉字及汉字标点符号（没有考虑英文标点符号）
                                //Regex regex = new Regex(combintn[1]);
                                ////如果一句话中有多个字符串匹配
                                //MatchCollection mc = regex.Matches(wordString);
                                //if (mc.Count > 1)
                                //{
                                //    if (!repeatWordsTable.ContainsKey(combintn[1]))
                                //    {
                                //        int[] locs = new int[mc.Count];
                                //        int i = -1;
                                //        foreach (Match m in mc)
                                //            locs[++i] = m.Index;
                                //        repeatWordsTable.Add(combintn[1], new RepeatedWord(combintn[1], locs));
                                //    }
                                //    pos = ((RepeatedWord)repeatWordsTable[combintn[1]]).Loc;
                                //}
                                //else
                                //    pos = mc[0].Index;
                                //如果是中文片段
                                if (combintn[0].Equals("CN"))
                                {
                                    //对每一个中文字符
                                    for (int i = 0; i < combintn[1].Length; ++i)
                                    {
                                        XmlElement wordNode = descDoc.CreateElement("Word");
                                        char character = combintn[1][i];

                                        wordNode.SetAttribute("Value", character.ToString());

                                        //字体节点
                                        XmlElement fontNode = descDoc.CreateElement("Font");
                                        TextExtractor.Style style = word.GetCharStyle(i + pos);
                                        fontNode.SetAttribute("Name", style.GetFontName().Normalize());
                                        fontNode.SetAttribute("Size", style.GetFontSize().ToString());
                                        fontNode.SetAttribute("Color", style.GetColor().ToString());
                                        fontNode.SetAttribute("Weight", style.GetWeight().ToString());

                                        //坐标节点
                                        XmlElement cornerNode = descDoc.CreateElement("WordCorners");
                                        double[] coordinates = word.GetGlyphQuad(i + pos);
                                        //左下
                                        XmlElement coordinateNode1 = descDoc.CreateElement("left-down");
                                        coordinateNode1.SetAttribute("x", coordinates[0].ToString());
                                        coordinateNode1.SetAttribute("y", coordinates[1].ToString());
                                        //右下
                                        XmlElement coordinateNode2 = descDoc.CreateElement("right-down");
                                        coordinateNode2.SetAttribute("x", coordinates[2].ToString());
                                        coordinateNode2.SetAttribute("y", coordinates[3].ToString());
                                        //右上
                                        XmlElement coordinateNode3 = descDoc.CreateElement("upper-right");
                                        coordinateNode3.SetAttribute("x", coordinates[4].ToString());
                                        coordinateNode3.SetAttribute("y", coordinates[5].ToString());
                                        //左上
                                        XmlElement coordinateNode4 = descDoc.CreateElement("upper-left");
                                        coordinateNode4.SetAttribute("x", coordinates[6].ToString());
                                        coordinateNode4.SetAttribute("y", coordinates[7].ToString());

                                        cornerNode.AppendChild(coordinateNode1);
                                        cornerNode.AppendChild(coordinateNode2);
                                        cornerNode.AppendChild(coordinateNode3);
                                        cornerNode.AppendChild(coordinateNode4);

                                        wordNode.AppendChild(fontNode);
                                        wordNode.AppendChild(cornerNode);

                                        //lineNode加入了每一个汉字
                                        lineNode.AppendChild(wordNode);
                                    }
                                }
                                //是英语（非中文）片段，认为是一个单词
                                else
                                {
                                    //对于每一个单词
                                    //设置基本属性
                                    XmlElement wordNode = descDoc.CreateElement("Word");
                                    wordNode.SetAttribute("Length", combintn[1].Length.ToString());
                                    wordNode.SetAttribute("Value", combintn[1]);

                                    //字体节点,默认单词的第一个字母为该单词的字体属性。
                                    XmlElement fontNode = descDoc.CreateElement("Font");
                                    TextExtractor.Style style = word.GetCharStyle(pos);
                                    fontNode.SetAttribute("Name", style.GetFontName().Normalize());
                                    fontNode.SetAttribute("Size", style.GetFontSize().ToString());
                                    fontNode.SetAttribute("Color", style.GetColor().ToString());
                                    fontNode.SetAttribute("Weight", style.GetWeight().ToString());

                                    //坐标节点
                                    XmlElement cornerNode = descDoc.CreateElement("WordCorners");
                                    double[] coordinates = word.GetQuad();
                                    //左下：单词片段的第一个字母的左下坐标
                                    double[] coordinates1 = word.GetGlyphQuad(pos);
                                    XmlElement coordinateNode1 = descDoc.CreateElement("left-down");
                                    coordinateNode1.SetAttribute("x", coordinates1[0].ToString());
                                    coordinateNode1.SetAttribute("y", coordinates1[1].ToString());

                                    //右下：单词片段的最后一个字母的右下坐标
                                    double[] coordinates2 = word.GetGlyphQuad(pos + combintn[1].Length - 1);
                                    XmlElement coordinateNode2 = descDoc.CreateElement("right-down");
                                    coordinateNode2.SetAttribute("x", coordinates2[2].ToString());
                                    coordinateNode2.SetAttribute("y", coordinates2[3].ToString());

                                    //右上：单词片段的最后一个字母的右上坐标
                                    XmlElement coordinateNode3 = descDoc.CreateElement("upper-right");
                                    coordinateNode3.SetAttribute("x", coordinates2[4].ToString());
                                    coordinateNode3.SetAttribute("y", coordinates2[5].ToString());

                                    //左上：单词片段的第一个字母的左上坐标
                                    XmlElement coordinateNode4 = descDoc.CreateElement("upper-left");
                                    coordinateNode4.SetAttribute("x", coordinates1[6].ToString());
                                    coordinateNode4.SetAttribute("y", coordinates1[7].ToString());

                                    cornerNode.AppendChild(coordinateNode1);
                                    cornerNode.AppendChild(coordinateNode2);
                                    cornerNode.AppendChild(coordinateNode3);
                                    cornerNode.AppendChild(coordinateNode4);

                                    wordNode.AppendChild(fontNode);
                                    wordNode.AppendChild(cornerNode);

                                    //对于每一个字符
                                    String chars = combintn[1];
                                    for (int i = 0; i < chars.Length; ++i)
                                    {
                                        XmlElement charNode = descDoc.CreateElement("Char");
                                        char character = chars[i];

                                        charNode.SetAttribute("Value", character.ToString());

                                        //字体节点
                                        XmlElement cFontNode = descDoc.CreateElement("Font");
                                        TextExtractor.Style cStyle = word.GetCharStyle(i + pos);
                                        cFontNode.SetAttribute("Name", cStyle.GetFontName().Normalize());
                                        cFontNode.SetAttribute("Size", cStyle.GetFontSize().ToString());
                                        cFontNode.SetAttribute("Color", cStyle.GetColor().ToString());
                                        cFontNode.SetAttribute("Weight", cStyle.GetWeight().ToString());

                                        //坐标节点
                                        XmlElement cCornerNode = descDoc.CreateElement("CharCorners");
                                        double[] cCoordinates = word.GetGlyphQuad(i + pos);
                                        //左下
                                        XmlElement cCoordinateNode1 = descDoc.CreateElement("left-down");
                                        cCoordinateNode1.SetAttribute("x", cCoordinates[0].ToString());
                                        cCoordinateNode1.SetAttribute("y", cCoordinates[1].ToString());
                                        //右下
                                        XmlElement cCoordinateNode2 = descDoc.CreateElement("right-down");
                                        cCoordinateNode2.SetAttribute("x", cCoordinates[2].ToString());
                                        cCoordinateNode2.SetAttribute("y", cCoordinates[3].ToString());
                                        //右上
                                        XmlElement cCoordinateNode3 = descDoc.CreateElement("upper-right");
                                        cCoordinateNode3.SetAttribute("x", cCoordinates[4].ToString());
                                        cCoordinateNode3.SetAttribute("y", cCoordinates[5].ToString());
                                        //左上
                                        XmlElement cCoordinateNode4 = descDoc.CreateElement("upper-left");
                                        cCoordinateNode4.SetAttribute("x", cCoordinates[6].ToString());
                                        cCoordinateNode4.SetAttribute("y", cCoordinates[7].ToString());

                                        cCornerNode.AppendChild(cCoordinateNode1);
                                        cCornerNode.AppendChild(cCoordinateNode2);
                                        cCornerNode.AppendChild(cCoordinateNode3);
                                        cCornerNode.AppendChild(cCoordinateNode4);

                                        charNode.AppendChild(cFontNode);
                                        charNode.AppendChild(cCornerNode);

                                        wordNode.AppendChild(charNode);
                                    }

                                    //lineNode加入了每一个英文单词
                                    lineNode.AppendChild(wordNode);
                                }

                            }
                        }

                        //补齐lineNode的属性
                        lineNode.SetAttribute("CharCount", lineCharCount.ToString());
                        lineNode.SetAttribute("Value", lineValue.ToString().Trim());

                        //pageNode加入了每一行
                        pageNode.AppendChild(lineNode);
                    }

                    //对于contentNode，加入了每一页
                    contentRoot.AppendChild(pageNode);
                }
                else
                    break;
            }

            //在xml的根节点加入了contentNode
            rootNode.AppendChild(contentRoot);

            descDoc.AppendChild(rootNode);
        }

        private void setLineAttribute(TextExtractor.Line line, XmlElement lineNode)
        {
            lineNode.SetAttribute("Index", line.GetCurrentNum().ToString());

            //加入坐标
            XmlElement cornerNode = descDoc.CreateElement("LineCorners");

            //左下
            XmlElement coordinateNode1 = descDoc.CreateElement("left-down");
            coordinateNode1.SetAttribute("x", line.GetBBox().x1.ToString());
            coordinateNode1.SetAttribute("y", line.GetBBox().y1.ToString());

            ///于7月6号添加，添加左上、右下两个坐标值
            //右下
            XmlElement coordinateNode2 = descDoc.CreateElement("right-down");
            coordinateNode2.SetAttribute("x", line.GetBBox().x2.ToString());
            coordinateNode2.SetAttribute("y", line.GetBBox().y1.ToString());

            //右上
            XmlElement coordinateNode3 = descDoc.CreateElement("upper-right");
            coordinateNode3.SetAttribute("x", line.GetBBox().x2.ToString());
            coordinateNode3.SetAttribute("y", line.GetBBox().y2.ToString());

            //左上
            XmlElement coordinateNode4 = descDoc.CreateElement("upper-left");
            coordinateNode4.SetAttribute("x", line.GetBBox().x1.ToString());
            coordinateNode4.SetAttribute("y", line.GetBBox().y2.ToString());

            cornerNode.AppendChild(coordinateNode1);
            cornerNode.AppendChild(coordinateNode2);
            cornerNode.AppendChild(coordinateNode3);
            cornerNode.AppendChild(coordinateNode4);

            lineNode.AppendChild(cornerNode);
        }

        private void setPageAttribute(int index, XmlElement pageNode, Page page)
        {
            //行数
            pageNode.SetAttribute("LineCount", extractor.GetNumLines().ToString());
            //高
            pageNode.SetAttribute("Height", page.GetPageHeight().ToString());
            //宽
            pageNode.SetAttribute("Width", page.GetPageWidth().ToString());
            //页码
            pageNode.SetAttribute("Index", index.ToString());
        }

        private void readPDFinfo()
        {
            PDFDocInfo info = doc.GetDocInfo();

            XmlElement infoRoot = descDoc.CreateElement("Info");

            //文件title
            String title = info.GetTitle();
            XmlElement titleNode = descDoc.CreateElement("Title");
            titleNode.SetAttribute("Value", title);

            //文件的subject
            String subject = info.GetSubject();
            XmlElement subjectNode = descDoc.CreateElement("Subject");
            subjectNode.SetAttribute("Value", subject);

            //文件的作者
            String author = info.GetAuthor();
            XmlElement authorNode = descDoc.CreateElement("Author");
            authorNode.SetAttribute("Value", author);

            //文件的创建时间
            String creationTime = info.GetCreationDate().year + "/" + info.GetCreationDate().month + "/" + info.GetCreationDate().day;
            XmlElement creationTimeNode = descDoc.CreateElement("CreationTime");
            creationTimeNode.SetAttribute("Value", creationTime);

            //文件的关键字
            String keyWords = info.GetKeywords();
            XmlElement keyWordsNode = descDoc.CreateElement("KeyWords");
            keyWordsNode.SetAttribute("Value", keyWords);

            //文件的页数
            String pageCount = doc.GetPageCount().ToString();
            XmlElement pageCountNode = descDoc.CreateElement("PageCount");
            pageCountNode.SetAttribute("Value", pageCount);

            //加入到infoRoot中
            infoRoot.AppendChild(titleNode);
            infoRoot.AppendChild(subjectNode);
            infoRoot.AppendChild(authorNode);
            infoRoot.AppendChild(creationTimeNode);
            infoRoot.AppendChild(keyWordsNode);
            infoRoot.AppendChild(pageCountNode);

            //加入到根节点
            rootNode.AppendChild(infoRoot);
        }

        public void init(String pdfFileName)
        {
            //初始化PDF类库
            PDFNet.Initialize();

            //载入文档
            doc = new PDFDoc(pdfFileName);
            pdfDescName = pdfFileName.Substring(0, pdfFileName.IndexOf('.')) + ".xml";

            //初始化XML描述文档
            descDoc = new XmlDocument();

            //创建文件头
            XmlDeclaration d = descDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
            descDoc.AppendChild(d);

            //创建根节点
            rootNode = descDoc.CreateElement("Pdf");
        }

        private bool isChineseWord(String s)
        {
            Regex rx = new Regex(@"^[\u4e00-\u9fa5]$");
            if (rx.IsMatch(s))
                return true;
            else
                return false;
        }

        private ArrayList splitCN_EN(String source)
        {
            ArrayList list = new ArrayList();
            Regex rx = new Regex(@"[\u4e00-\u9fa5|，。？、“”；：！…‘’（）《》【】]+");
            MatchCollection mc = rx.Matches(source);

            for (int index = 0; index < mc.Count; ++index)
            {
                //根据索引生成一个中文字符段
                String[] CN = new String[2];
                CN[0] = "CN";
                CN[1] = mc[index].Value;

                list.Add(CN);

                //如果后边还有英文字符段
                if (source.Length > mc[index].Index + mc[index].Length)
                {
                    String[] EN = new String[2];
                    EN[0] = "EN";
                    int ENLength = (index + 1 >= mc.Count)
                        ?
                        source.Length - (mc[index].Index + mc[index].Length)
                        :
                        mc[index + 1].Index - (mc[index].Index + mc[index].Length);

                    EN[1] = source.Substring(mc[index].Index + mc[index].Length, ENLength);

                    list.Add(EN);
                }
            }

            return list;
        }

    }
}
