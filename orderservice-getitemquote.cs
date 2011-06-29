using System; 
using System.Web.UI; 
using System.Web.UI.WebControls; 
using System.Web.UI.HtmlControls; 

using System.Data;
using System.IO;
using System.Net;
using System.Text;

using System.Xml;

namespace Mimeo 
	{ 
	public class OrderService
		{ 
			
		public void GetItemQuote() 
			{ 
			
			// Mimeo Account Credentials
			string userName = "";
			string password = "";
			
			System.Text.StringBuilder build_url = new System.Text.StringBuilder();

			//Sandbox Connection
			build_url("http://connect.sandbox.mimeo.com/2010/09/OrderService/NewProduct?");
			build_url("template=custom&");

			// Set up any document ID  you want
			build_url("&documentTemplateId=51d08dbb-9a60-4bc1-8843-1e4d0b24794d");
			string post_url = build_url();

			HttpWebRequest request = (HttpWebRequest) HttpWebRequest.Create (post_url); 
			
			request.Method = "GET";   
			request.Credentials = new NetworkCredential(userName, password);  
			
			string NewProductResponseXML = "";
			using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)  
			{  
			    // Get the response stream  
			    StreamReader reader = new StreamReader(response.GetResponseStream());  
			  
			    // Console application output  
			    NewProductResponseXML = reader.ReadToEnd();
			} 
			
			// Create our XML Document From New Product Template
			System.Xml.XmlDocument NewProductXML = new XmlDocument();
			NewProductXML.LoadXml(NewProductResponseXML);	
			
			// Product Object
			string ApplicationId = NewProductXML.GetElementsByTagName("Product")[0]["ApplicationId"].InnerText;
			string DocumentTemplateId = NewProductXML.GetElementsByTagName("Product")[0]["DocumentTemplateId"].InnerText;
			string DocumentTemplateName = NewProductXML.GetElementsByTagName("Product")[0]["DocumentTemplateName"].InnerText;							  	
			
			// Set the Source of Document with Our New Information
			//  URL - Source of PDF File
			XmlNode SourceNode = NewProductXML.GetElementsByTagName("DocumentSection")[0]["Source"];
			SourceNode.InnerXml = "http://kinlane-productions.s3.amazonaws.com/pdf-samples/The_Impact_of_Digital_Books_Upon_Print_Publishing.pdf";			

			// Range 1 through X number of PDF pages
			XmlNode SourceRange = NewProductXML.GetElementsByTagName("DocumentSection")[0]["Range"];
			SourceRange.InnerXml = "[1,5]";

			// Set Document Source
			string Source = NewProductXML.GetElementsByTagName("DocumentSection")[0]["Source"].InnerText;
			string Range = NewProductXML.GetElementsByTagName("DocumentSection")[0]["Range"].InnerText;
			
			// Set The Quantity for this Document
			XmlNode DetailsNode = NewProductXML.GetElementsByTagName("Details")[0]["OrderQuantity"];
			DetailsNode.InnerXml = "5";								
			
			// Put the New Product XML Object back to String
			StringWriter sw = new StringWriter();
			XmlTextWriter xw = new XmlTextWriter(sw);
			NewProductXML.WriteTo(xw);
			
			string ItemQuoteRequestXML = sw.ToString();
			
			// Mimeo Sandbox API - OrderService / GetItemQuote
			string ItemQuote_URL = "http://connect.sandbox.mimeo.com/2010/09/OrderService/GetItemQuote";	
			
			HttpWebRequest ItemQuote_Request = (HttpWebRequest) HttpWebRequest.Create (ItemQuote_URL); 
			
			ItemQuote_Request.ContentType = "text/xml";
			ItemQuote_Request.Method = "POST";
			
         		//string s = "id="+Server.UrlEncode(xml);
           		byte[] requestBytes = System.Text.Encoding.ASCII.GetBytes(ItemQuoteRequestXML);
            		ItemQuote_Request.Method = "POST";
            		ItemQuote_Request.ContentType = "text/xml;charset=utf-8";
            		ItemQuote_Request.ContentLength = requestBytes.Length;
            		Stream requestStream = ItemQuote_Request.GetRequestStream();
            		requestStream.Write(requestBytes, 0, requestBytes.Length);
            		requestStream.Close();
            
			ItemQuote_Request.Credentials = new NetworkCredential(userName, password);  
			
			string ItemQuoteResponseXML = "";
			using (HttpWebResponse ItemQuoteResponse = ItemQuote_Request.GetResponse() as HttpWebResponse)  
			{  
			    // Get the response stream  
			    StreamReader reader = new StreamReader(ItemQuoteResponse.GetResponseStream());  
			  
			    // Console application output  
			    ItemQuoteResponseXML = reader.ReadToEnd();
			} 	
			
			// Create our XML Document From New Product Template
			System.Xml.XmlDocument ItemQuoteXML = new XmlDocument();
			ItemQuoteXML.LoadXml(ItemQuoteResponseXML);			
			
			// Sets the Quote Price for the Order
			string ProductQuotePrice = ItemQuoteXML.GetElementsByTagName("Details")[0]["ProductPrice"].InnerText;	
					
			} 
		} 
	}