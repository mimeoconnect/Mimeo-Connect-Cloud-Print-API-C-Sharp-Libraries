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

		public void NewProduct() 
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

			string new_product_template = "";
			
			using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)  
			{  
			    // Get the response stream  
			    StreamReader reader = new StreamReader(response.GetResponseStream());  
			  
			    // Console application output  
			    new_product_template = reader.ReadToEnd();
			} 
			
			getTemplate.Text = new_product_template;			
			
						
			} 
		} 
	}