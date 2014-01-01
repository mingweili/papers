using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace PDFNavigator
{
    /// <summary>
    /// 用于记录关键词搜索结果
    /// </summary>
    public class KeywordInfo
    {
        public class Record
        {
            public Record(string fn, int p, int c)
            {
                this.fileName = fn;
                this.page = p;
                this.count = c;
            }
            public string fileName{set;get;}
            public int page{set;get;}
            public int count{set;get;}
        }

        public int sumCount { set; get; }
        public string keyword { set; get; }

        public List<Record> recordList = new List<Record>();

        public KeywordInfo(string kw)
        {
            this.keyword = kw;
            this.sumCount = 0;
        }
        public void addRecord(string fileName, int page, int count)
        { 
            this.recordList.Add(new Record(fileName, page, count));
            this.sumCount += count;
        }        
        
    }
}
