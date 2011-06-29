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
			
		public void GetOrderQuote() 
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
			NewProductXML.GetElementsByTagName("Details")[0]["OrderQuantity"].InnerXml = "5";									
			
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
			
			// Set the Shipping Options
			ItemQuoteXML.GetElementsByTagName("RecipientAddress")[0]["CompanyName"].InnerXml = "Mimeo";			
			ItemQuoteXML.GetElementsByTagName("RecipientAddress")[0]["Name"].InnerXml = "Mimeo";
			ItemQuoteXML.GetElementsByTagName("RecipientAddress")[0]["FirstName"].InnerXml = "Kin";
			ItemQuoteXML.GetElementsByTagName("RecipientAddress")[0]["LastName"].InnerXml = "Lane";
			ItemQuoteXML.GetElementsByTagName("RecipientAddress")[0]["CareOf"].InnerXml = "Mimeo";
			ItemQuoteXML.GetElementsByTagName("RecipientAddress")[0]["Street"].InnerXml = "588 Sutter ";
			ItemQuoteXML.GetElementsByTagName("RecipientAddress")[0]["ApartmentOrSuite"].InnerXml = "Suite 203";
			ItemQuoteXML.GetElementsByTagName("RecipientAddress")[0]["City"].InnerXml = "San Francisco";
			ItemQuoteXML.GetElementsByTagName("RecipientAddress")[0]["StateOrProvince"].InnerXml = "CA";
			ItemQuoteXML.GetElementsByTagName("RecipientAddress")[0]["PostalCode"].InnerXml = "94012";
			ItemQuoteXML.GetElementsByTagName("RecipientAddress")[0]["Country"].InnerXml = "US";
			ItemQuoteXML.GetElementsByTagName("RecipientAddress")[0]["TelephoneNumber"].InnerXml = "541-913-2328";
			ItemQuoteXML.GetElementsByTagName("RecipientAddress")[0]["Email"].InnerXml = "kin.lane@mimeo.com";
			ItemQuoteXML.GetElementsByTagName("RecipientAddress")[0]["IsResidential"].InnerXml = "false";	
			
			// Put the Item Quote XML Object back to String
			StringWriter sw2 = new StringWriter();
			XmlTextWriter xw2 = new XmlTextWriter(sw2);
			ItemQuoteXML.WriteTo(xw2);
			
			string ShippingOptionsRequestXML = sw2.ToString();		
			
			// Mimeo Sandbox API - OrderService / GetShippingOptions
			string ShippingOptions_URL = "http://connect.sandbox.mimeo.com/2010/09/OrderService/GetShippingOptions";	
			
			HttpWebRequest ShippingOptions_Request = (HttpWebRequest) HttpWebRequest.Create (ShippingOptions_URL); 
			
			ShippingOptions_Request.ContentType = "text/xml";
			ShippingOptions_Request.Method = "POST";
			
         		//string s = "id="+Server.UrlEncode(xml);
            		byte[] requestBytes2 = System.Text.Encoding.ASCII.GetBytes(ShippingOptionsRequestXML);
            		ShippingOptions_Request.Method = "POST";
            		ShippingOptions_Request.ContentType = "text/xml;charset=utf-8";
            		ShippingOptions_Request.ContentLength = requestBytes2.Length;
            		Stream requestStream2 = ShippingOptions_Request.GetRequestStream();
            		requestStream2.Write(requestBytes2, 0, requestBytes2.Length);
            		requestStream2.Close();
            
			ShippingOptions_Request.Credentials = new NetworkCredential(userName, password);  
			
			string ShippingResponseXML = "";
			using (HttpWebResponse ShippingResponse = ShippingOptions_Request.GetResponse() as HttpWebResponse)  
			{  
			    // Get the response stream  
			    StreamReader reader = new StreamReader(ShippingResponse.GetResponseStream());  
			  
			    // Console application output  
			    ShippingResponseXML = reader.ReadToEnd();
			} 				
			
			// Create our XML Document From Shipping Options Response
			System.Xml.XmlDocument ShippingXML = new XmlDocument();
			ShippingXML.LoadXml(ShippingResponseXML);			
			
			// DISPLAY ONLY
			System.Text.StringBuilder sb2 = new System.Text.StringBuilder();			
			
			// Loop through the shipping options and set based upon UseShippingType
			string UseShippingType = "Ground";
			string Shipping_ID = "";
			string Shipping_Name = "";
			string Shipping_Charge = "";
			string Shipping_DeliveryDate = "";
			int i = 0;
			foreach (XmlNode option in ShippingXML.GetElementsByTagName("ShippingOptions")) // Loop through XML file
				{
					
				if(ShippingXML.GetElementsByTagName("ShippingOption")[i]["Name"].InnerText == UseShippingType)
					{
					Shipping_ID = ShippingXML.GetElementsByTagName("ShippingOption")[i]["Id"].InnerXml;
					Shipping_Name = ShippingXML.GetElementsByTagName("ShippingOption")[i]["Name"].InnerXml;
					Shipping_Charge = ShippingXML.GetElementsByTagName("ShippingOption")[i]["Charge"].InnerXml;
					Shipping_DeliveryDate = ShippingXML.GetElementsByTagName("ShippingOption")[i]["DeliveryDate"].InnerXml;
					}				
				i++;
				}			
			
			//Set the Shipping Choice witih the ID of the Shipping Option we went with above
			ShippingXML.GetElementsByTagName("Details")[0]["ShippingChoice"].InnerXml = Shipping_ID;
			
			// Zero Out the Shipping Options
			ShippingXML.GetElementsByTagName("Details")[0]["ShippingOptions"].InnerXml = "";			
			
			// Put the Item Quote XML Object back to String
			StringWriter sw3 = new StringWriter();
			XmlTextWriter xw3 = new XmlTextWriter(sw3);
			ShippingXML.WriteTo(xw3);
			
			string OrderQuoteRequestXML = sw3.ToString();		
			
			// Mimeo Sandbox API - OrderService / GetQuote
			string OrderQuote_URL = "http://connect.sandbox.mimeo.com/2010/09/OrderService/GetQuote";	
			
			HttpWebRequest OrderQuote_Request = (HttpWebRequest) HttpWebRequest.Create (OrderQuote_URL); 
			
			OrderQuote_Request.ContentType = "text/xml";
			OrderQuote_Request.Method = "POST";
			
         		//string s = "id="+Server.UrlEncode(xml);
            		byte[] requestBytes3 = System.Text.Encoding.ASCII.GetBytes(OrderQuoteRequestXML);
            		OrderQuote_Request.Method = "POST";
            		OrderQuote_Request.ContentType = "text/xml;charset=utf-8";
            		OrderQuote_Request.ContentLength = requestBytes3.Length;
            		Stream requestStream3 = OrderQuote_Request.GetRequestStream();
            		requestStream3.Write(requestBytes3, 0, requestBytes3.Length);
            		requestStream3.Close();
            
			OrderQuote_Request.Credentials = new NetworkCredential(userName, password);  
			
			string OrderQuoteResponseXML = "";
			using (HttpWebResponse OrderQuoteResponse = OrderQuote_Request.GetResponse() as HttpWebResponse)  
			{  
			    // Get the response stream  
			    StreamReader reader = new StreamReader(OrderQuoteResponse.GetResponseStream());  
			  
			    // Console application output  
			    OrderQuoteResponseXML = reader.ReadToEnd();
			} 			
			
			// Create our XML Document From New Product Template
			System.Xml.XmlDocument OrderQuoteXML = new XmlDocument();
			OrderQuoteXML.LoadXml(OrderQuoteResponseXML);			
			
			// Sets the Values for the Order Quote
			string ProductPrice = OrderQuoteXML.GetElementsByTagName("Details")[0]["ProductPrice"].InnerText;
			string ShippingPrice = OrderQuoteXML.GetElementsByTagName("Details")[0]["ShippingPrice"].InnerText;
			string TotalPrice = OrderQuoteXML.GetElementsByTagName("Details")[0]["TotalPrice"].InnerText;

						
			} 
		} 
	}