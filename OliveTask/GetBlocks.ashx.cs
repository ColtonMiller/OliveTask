using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using System.Web.Script.Serialization;
using System.Text;

namespace OliveTask
{
    /// <summary>
    /// Summary description for Handler1
    /// </summary>
    public class GetBlocks : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            //load in xml
            XDocument xmlDocument = XDocument.Load(context.Server.MapPath("~/Pg001.xml"));

            //list of blocks to hold
            List<Block> pageBlocks = new List<Block>();

            //grab height and width of page and set int values
            int pageHeight = int.Parse(xmlDocument.Element("Page").Attribute("HEIGHT").Value);
            int pageWidth = int.Parse(xmlDocument.Element("Page").Attribute("WIDTH").Value);

            foreach (var article in xmlDocument.Descendants("Entity"))
            {
                foreach (var block in article.Elements("Block"))
                {
                    //make a new block with the values and add to list
                    pageBlocks.Add(new Block(pageWidth, pageHeight, block.Attribute("ID").Value, block.Attribute("BOX").Value));
                }                
            }

            //convert list to JSON
            var jsonSerializer = new JavaScriptSerializer();
            var results = jsonSerializer.Serialize(pageBlocks);

            context.Response.ContentType = "application/json";
            context.Response.ContentEncoding = Encoding.UTF8;
            context.Response.Write(results);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
    /// <summary>
    /// class for blocks
    /// </summary>
    public class Block
    {
        //properties
        public string BlockId { get; set; }
        public float Left { get; private set; }
        public float Top { get; private set; }
        public float Width { get; private set; }
        public float Height { get; private set; }

        //constructor that takes in Box values as parameters
        public Block(int widthOfPage, int heightOfPage, string id, string boxValue)
        {
            this.BlockId = id;
            //split by space
            string[] boxValueArray = boxValue.Split(' ');
            this.Left = (float.Parse(boxValueArray[0]) * 100) / widthOfPage;
            this.Top = (float.Parse(boxValueArray[1]) * 100 / heightOfPage);
            this.Width = (float.Parse(boxValueArray[3]) - float.Parse(boxValueArray[0]) * 100 / widthOfPage);
            this.Height = (float.Parse(boxValueArray[4]) - float.Parse(boxValueArray[1]) * 100 / widthOfPage);
        }
    }
}