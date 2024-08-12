using ProductManagement.Tool.HtmlAnalysis.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductManagement.Tool.HtmlAnalysis
{
    public class NodeCompile
    {
        public string NodeString { get; set; } = "src=\"1 2\"  alt='测 \" 试'";

        public Node Node { get; set; }

        public void Compile()
        {
            if (string.IsNullOrWhiteSpace(NodeString))
            {
                return;
            }
            NodeString = NodeString.Trim();
            if (!NodeString.StartsWith("<"))
            {
                //return;
            }

            var attrs = new List<Attr>();
            var node = new Node()
            {
                OuterHtml = NodeString,
                Attrs = attrs
            };

            var length = NodeString.Length;
            var temp = new Stack<string>();
            var value = new List<char>();
            var current = string.Empty;

            //操作符
            var queueOp = new Stack<char>();
            char none = char.MinValue,
                currentOp = none,
                currentSeparater = none,
                prev = none,
                s = none;

            var dicSeparater = new Dictionary<char, char>();
            dicSeparater.Add('\'', '\'');
            dicSeparater.Add('"', '"');

            var dic = new Dictionary<char, char>();
            dic.Add('<', '>');
            dic.Add('=', '=');

            bool startStr = false;
            char[] express = new char[] { '=' };
            Attr attr = null;
            for (int i = 0; i < length; i++)
            {
                char c = NodeString[i];

                //不是字符串
                if (!startStr)
                {
                    //开始
                    if (dic.ContainsKey(c))
                    {
                        //符号之前的数据
                        if (value.Count > 0)
                        {
                            current = new string(value.ToArray());
                            value.Clear();
                            temp.Push(current);
                        }
                        currentOp = c;
                        temp.Push(c.ToString());
                    }
                    //结束符
                    else if (dic[currentOp] == c)
                    {
                        if (value.Count > 0)
                        {
                            current = new string(value.ToArray());
                            value.Clear();
                            temp.Push(current);
                        }
                        currentOp = none;
                        temp.Push(c.ToString());
                    }
                    else
                    {
                        //不在关健符之间的空格, 抛弃
                        if (c == ' ' && prev == ' ')
                        {
                            continue;
                        }
                        else if (c == ' ' && prev != ' ')
                        {
                            if (value.Count > 0)
                            {
                                current = new string(value.ToArray());
                                value.Clear();
                                temp.Push(current);
                            }
                        }
                        else
                        {
                            value.Add(c);
                        }
                    }
                }        

                //开始字符串符
                if (dicSeparater.ContainsKey(c) && !startStr)
                {
                    currentSeparater = c;
                    startStr = true;
                    continue;
                }
                //字符串符
                if (startStr)
                {
                    //结束字符串符
                    if (dicSeparater[currentSeparater] == c)
                    {
                        currentSeparater = none;
                        startStr = false;
                        if (value.Count > 0)
                        {
                            current = new string(value.ToArray());
                            value.Clear();
                            temp.Push(current); 
                        }
                    }
                    else
                    {
                        value.Add(c);
                    }
                }
                prev = c;
            }
            Node = node;
        }


        public void Compile2()
        {
            if (string.IsNullOrWhiteSpace(NodeString))
            {
                return;
            }
            NodeString = NodeString.Trim();
            if (NodeString.StartsWith("<"))
            {
                return;
            }

            var attrs = new List<Attr>();
            var node = new Node() { 
                OuterHtml = NodeString,
                Attrs = attrs
            };

            var length = NodeString.Length;
            var temp = new Stack<string>();
            var value = new List<char>();
            var current = string.Empty;

            //操作符
            var queueOp = new Stack<char>();
            char start = char.MinValue,
                currentOp = start,
                s = start;

            var dic = new Dictionary<char, char>();
            dic.Add('\'', '\'');
            dic.Add('"', '"');
            //dic.Add('<', '>');

            char[] express = new char []{ '=' };
            Attr attr = null;
            for (int i = 0; i < length; i++)
            {
                char c = NodeString[i];
                //开始符
                if (dic.ContainsKey(c) && currentOp == start)
                {
                    currentOp = c;
                    queueOp.Push(c);
                    continue;
                }
                //和开始匹配的结束符
                if (currentOp != start && dic[currentOp] == c)
                {
                    queueOp.Pop();
                    if (!queueOp.TryPeek(out currentOp))
                        currentOp = start;

                    //开始符之前的数据 value
                    if (value.Count > 0)
                    {
                        current = new string(value.ToArray());
                        value.Clear();

                        if (s == '=' && attr != null)
                        {
                            attr.Value = current;
                        }
                    }
                    continue;
                }


                switch (c)
                {
                    case '=':


                        break;
                    default:
                        break;
                }
                //不在关健符之间的=, 赋值
                if (c == '=' && currentOp == start)
                {
                    //开始符之前的数据, name
                    if (value.Count > 0)
                    {
                        s = '=';
                        current = new string(value.ToArray());
                        value.Clear();

                        attr = new Attr()
                        {
                            Name = current
                        };
                        attrs.Add(attr);
                        continue;
                    }
                }
                //不在关健符之间的空格, 抛弃
                if (c == ' ' && currentOp == start)
                {
                    continue;
                }

                value.Add(c);
            }
            Node = node;
        }
    }
}
