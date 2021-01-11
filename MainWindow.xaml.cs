using System;
using System.Windows;
using System.Windows.Controls;
using System.Net;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Xml;
using System.Threading;
using System.Windows.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;


namespace HCMextract
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Dictionary<string, string> properties = new Dictionary<String, String>();
            foreach (String line in File.ReadAllLines(System.AppDomain.CurrentDomain.BaseDirectory + "properties.txt"))
            {
                properties.Add(line.Split('=')[0].Trim(), line.Substring(line.IndexOf('=') + 1).Trim());
            }

            textbox_environment.Text = properties["environment"];
            textbox_password.Password = properties["password"];
            textbox_username.Text = properties["username"];
            textBox_extractname.Text = properties["extract-name"];
            textbox_parameters_1.Text = properties["custom-parameters-1"];
            textbox_parameters_2.Text = properties["custom-parameters-2"];
            textbox_parameters_3.Text = properties["custom-parameters-3"];
            textbox_parameters_4.Text = properties["custom-parameters-4"];
            textbox_parameters_5.Text = properties["custom-parameters-5"];
            textbox_parameters_6.Text = properties["custom-parameters-6"];
            textbox_parameters_7.Text = properties["custom-parameters-7"];
            textbox_parameters_8.Text = properties["custom-parameters-8"];
            textbox_parameters_9.Text = properties["custom-parameters-9"];
            textbox_output_folder.Text = properties["output-folder"];
            if (properties["changes-only"].Equals("N"))
            {
                comboBox.SelectedIndex = 0;
            }
            else if (properties["changes-only"].Equals("Y"))
            {
                comboBox.SelectedIndex = 1;
            }
            else if (properties["changes-only"].Equals("ATTRIBUTE"))
            {
                comboBox.SelectedIndex = 2;
            }
            else if (properties["changes-only"].Equals("ATTRIB_OLD"))
            {
                comboBox.SelectedIndex = 3;
            }
            else if (properties["changes-only"].Equals("BLOCK"))
            {
                comboBox.SelectedIndex = 4;
            }
            else if (properties["changes-only"].Equals("BLOCK_OLD"))
            {
                comboBox.SelectedIndex = 5;
            }
            textbox_ldg.Text = properties["ldg"];
            datepicker.SelectedDate = DateTime.ParseExact(properties["effective-date"], "yyyy-MM-dd", CultureInfo.InvariantCulture);

        }

        private async Task<String> DownloadAttachment(String id)
        {
            String xmlInput = "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:typ=\"http://xmlns.oracle.com/apps/financials/commonModules/shared/financialUtilService/types/\"><soapenv:Header/><soapenv:Body><typ:downloadESSJobExecutionDetails><typ:requestId>789778</typ:requestId><typ:fileType>log</typ:fileType></typ:downloadESSJobExecutionDetails></soapenv:Body></soapenv:Envelope>";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://" + textbox_environment.Text + ":443/fscmService/FinancialUtilService");
            byte[] bytes = Encoding.ASCII.GetBytes(xmlInput);

            request.ContentLength = bytes.Length;
            request.ContentType = "text/xml; charset=utf-8";
            request.Headers.Add("SOAPAction", "http://xmlns.oracle.com/apps/financials/commonModules/shared/financialUtilService/downloadESSJobExecutionDetails");
            String encoded = System.Convert.ToBase64String(Encoding.ASCII.GetBytes(textbox_username.Text + ":" + textbox_password.Password));
            request.Headers.Add("Authorization", "Basic " + encoded);
            request.Method = "POST";
            Stream newStream = request.GetRequestStream();
            newStream.Write(bytes, 0, bytes.Length);
            newStream.Close();

            HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();
            Stream streamResponse = response.GetResponseStream();
            StreamReader streamRead = new StreamReader(streamResponse);
            Char[] readBuff = new Char[256];
            int count = streamRead.Read(readBuff, 0, 256);
            StringBuilder output = new StringBuilder();
            while (count > 0)
            {
                String outputData = new String(readBuff, 0, count);
                output.Append(outputData);
                count = streamRead.Read(readBuff, 0, 256);
            }
            // Close the Stream object.
            streamResponse.Close();
            streamRead.Close();
            // Release the resources held by response object.

            response.Close();

            //textbox_results.Text = output.ToString();
            //textbox_results.Text = output.ToString().Substring(output.ToString().IndexOf("PK"), output.Length - 48 - output.ToString().IndexOf("PK"));
            String PK = output.ToString().Substring(output.ToString().IndexOf("PK"), output.Length - 48 - output.ToString().IndexOf("PK"));
            //File.WriteAllText(textbox_output_folder.Text + "\\test.zip", PK);
            FileStream filestream = File.Create(textbox_output_folder.Text + "\\test.zip");

            byte[] byteArray = Encoding.ASCII.GetBytes(PK);
            MemoryStream stream = new MemoryStream(byteArray);

            /*
            MemoryStream PKstream = new MemoryStream(Encoding.ASCII.GetBytes(PK));

            GZipStream decompressionStream = new GZipStream(PKstream, CompressionMode.Decompress);
            MemoryStream memstream = new MemoryStream();
            decompressionStream.CopyTo(File.Create(textbox_output_folder.Text + "\\text.zip"));
            */

            /*
            StringBuilder memoutput = new StringBuilder();
            byte[] memBuff = new byte[256];
            while (count > 0)
            {
                String outputData = new String(readBuff, 0, count);
                memoutput.Append(outputData);
                count = memstream.Read(memBuff, 0, 256);
            }

            */
            textbox_results.Text = "Done";
            //textbox_results.Text = memoutput.ToString();

            //XmlDocument xml = new XmlDocument();
            //xml.LoadXml(output.ToString());

            //String result = xml.GetElementsByTagName("ns0:ProcessId")[0].InnerText;
            return "";
        }


        private async Task<String> GetJobId()
        {
            String xmlInput = "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:typ=\"http://xmlns.oracle.com/apps/hcm/processFlows/core/flowActionsService/types/\"><soapenv:Header/><soapenv:Body><typ:getFlowTaskRequestIdAndStatus><typ:flowInstanceName>Y_HCM_038_20190325_20190328175838</typ:flowInstanceName><typ:flowTaskInstanceName>Y_HCM_038_20190325</typ:flowTaskInstanceName><typ:legislativeDataGroupName>ARCADIS - US LDG</typ:legislativeDataGroupName></typ:getFlowTaskRequestIdAndStatus></soapenv:Body></soapenv:Envelope>";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://" + textbox_environment.Text + ":443/hcmService/FlowActionsService");
            byte[] bytes = Encoding.ASCII.GetBytes(xmlInput);

            request.ContentLength = bytes.Length;
            request.ContentType = "text/xml; charset=utf-8";
            request.Headers.Add("SOAPAction", "http://xmlns.oracle.com/apps/hcm/processFlows/core/flowActionService/getFlowTaskRequestIdAndStatus");
            String encoded = System.Convert.ToBase64String(Encoding.ASCII.GetBytes(textbox_username.Text + ":" + textbox_password.Password));
            request.Headers.Add("Authorization", "Basic " + encoded);
            request.Method = "POST";
            Stream newStream = request.GetRequestStream();
            newStream.Write(bytes, 0, bytes.Length);
            newStream.Close();

            HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();
            Stream streamResponse = response.GetResponseStream();
            StreamReader streamRead = new StreamReader(streamResponse);
            Char[] readBuff = new Char[256];
            int count = streamRead.Read(readBuff, 0, 256);
            StringBuilder output = new StringBuilder();
            while (count > 0)
            {
                String outputData = new String(readBuff, 0, count);
                output.Append(outputData);
                count = streamRead.Read(readBuff, 0, 256);
            }
            // Close the Stream object.
            streamResponse.Close();
            streamRead.Close();
            // Release the resources held by response object.
            response.Close();

            //textbox_results.Text = output.ToString();

            XmlDocument xml = new XmlDocument();
            xml.LoadXml(output.ToString());

            String result = xml.GetElementsByTagName("ns0:ProcessId")[0].InnerText;
            return result;
        }

        DateTime startTime = DateTime.Now;

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            TimeSpan time = DateTime.Now - startTime;

            label_timer.Content = time.ToString(@"hh\:mm\:ss");
        }

        private async void button_Click(object sender, RoutedEventArgs e)
        {
            await Run(1);
            if (!String.IsNullOrEmpty(textbox_parameters_2.Text)) { await Run(2); }
            if (!String.IsNullOrEmpty(textbox_parameters_3.Text)) { await Run(3); }
            if (!String.IsNullOrEmpty(textbox_parameters_4.Text)) { await Run(4); }
            if (!String.IsNullOrEmpty(textbox_parameters_5.Text)) { await Run(5); }
            if (!String.IsNullOrEmpty(textbox_parameters_6.Text)) { await Run(6); }
            if (!String.IsNullOrEmpty(textbox_parameters_7.Text)) { await Run(7); }
            if (!String.IsNullOrEmpty(textbox_parameters_8.Text)) { await Run(8); }
            if (!String.IsNullOrEmpty(textbox_parameters_9.Text)) { await Run(9); }
        }

        private async Task<String> Run(int instance)
        {
            String instanceName = textBox_extractname.Text.Replace(" ", "_") + "_" + String.Format("{0:yyyyMMddHHmmss}", DateTime.Now);
            String[] parameters = null;
            if (instance == 1) { textbox_instance_1.Text = instanceName; parameters = textbox_parameters_1.Text.Split(new String[] { ";" }, StringSplitOptions.RemoveEmptyEntries); }
            else if (instance == 2) { textbox_instance_2.Text = instanceName; parameters = textbox_parameters_2.Text.Split(new String[] { ";" }, StringSplitOptions.RemoveEmptyEntries); }
            else if (instance == 3) { textbox_instance_3.Text = instanceName; parameters = textbox_parameters_3.Text.Split(new String[] { ";" }, StringSplitOptions.RemoveEmptyEntries); }
            else if (instance == 4) { textbox_instance_4.Text = instanceName; parameters = textbox_parameters_4.Text.Split(new String[] { ";" }, StringSplitOptions.RemoveEmptyEntries); }
            else if (instance == 5) { textbox_instance_5.Text = instanceName; parameters = textbox_parameters_5.Text.Split(new String[] { ";" }, StringSplitOptions.RemoveEmptyEntries); }
            else if (instance == 6) { textbox_instance_6.Text = instanceName; parameters = textbox_parameters_6.Text.Split(new String[] { ";" }, StringSplitOptions.RemoveEmptyEntries); }
            else if (instance == 7) { textbox_instance_7.Text = instanceName; parameters = textbox_parameters_7.Text.Split(new String[] { ";" }, StringSplitOptions.RemoveEmptyEntries); }
            else if (instance == 8) { textbox_instance_8.Text = instanceName; parameters = textbox_parameters_8.Text.Split(new String[] { ";" }, StringSplitOptions.RemoveEmptyEntries); }
            else if (instance == 9) { textbox_instance_9.Text = instanceName; parameters = textbox_parameters_9.Text.Split(new String[] { ";" }, StringSplitOptions.RemoveEmptyEntries); }
            ThreadPool.QueueUserWorkItem(d => Dispatcher.Invoke(new Action(() => { label_status.Content = "Starting extract..."; })));
            DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            startTime = DateTime.Now;
            dispatcherTimer.Start();
            String startExtractResult = await RunExtract(instanceName, parameters);

            if (startExtractResult.Equals("true"))
            {
                String status = await WaitExtract(instanceName);
                ThreadPool.QueueUserWorkItem(d => Dispatcher.Invoke(new Action(() => { label_status.Content = "Waiting extract... " + status; })));
                while (!status.Equals("COMPLETED"))
                {
                    await Task.Delay(2000);
                    status = await WaitExtract(instanceName);
                    ThreadPool.QueueUserWorkItem(d => Dispatcher.Invoke(new Action(() => { label_status.Content = "Waiting extract... " + status; })));
                }
            }
            await ReadExtract(instanceName);
            ThreadPool.QueueUserWorkItem(d => Dispatcher.Invoke(new Action(() => { label_status.Content = "Finished"; })));
            dispatcherTimer.Stop();

            return "Done";
        }

        private async void button_ClickRollBack(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(textbox_instance_9.Text)) { await Rollback(9); textbox_instance_9.Text = ""; }
            if (!String.IsNullOrEmpty(textbox_instance_8.Text)) { await Rollback(8); textbox_instance_8.Text = ""; }
            if (!String.IsNullOrEmpty(textbox_instance_7.Text)) { await Rollback(7); textbox_instance_7.Text = ""; }
            if (!String.IsNullOrEmpty(textbox_instance_6.Text)) { await Rollback(6); textbox_instance_6.Text = ""; }
            if (!String.IsNullOrEmpty(textbox_instance_5.Text)) { await Rollback(5); textbox_instance_5.Text = ""; }
            if (!String.IsNullOrEmpty(textbox_instance_4.Text)) { await Rollback(4); textbox_instance_4.Text = ""; }
            if (!String.IsNullOrEmpty(textbox_instance_3.Text)) { await Rollback(3); textbox_instance_3.Text = ""; }
            if (!String.IsNullOrEmpty(textbox_instance_2.Text)) { await Rollback(2); textbox_instance_2.Text = ""; }
            if (!String.IsNullOrEmpty(textbox_instance_1.Text)) { await Rollback(1); textbox_instance_1.Text = ""; }
        }

        private async Task<String> Rollback(int instance)
        {
            String instanceName = "";
            if (instance == 1) { instanceName = textbox_instance_1.Text; }
            else if (instance == 2) { instanceName = textbox_instance_2.Text; }
            else if (instance == 3) { instanceName = textbox_instance_3.Text; }
            else if (instance == 4) { instanceName = textbox_instance_4.Text; }
            else if (instance == 5) { instanceName = textbox_instance_5.Text; }
            else if (instance == 6) { instanceName = textbox_instance_6.Text; }
            else if (instance == 7) { instanceName = textbox_instance_7.Text; }
            else if (instance == 8) { instanceName = textbox_instance_8.Text; }
            else if (instance == 9) { instanceName = textbox_instance_9.Text; }
            
            ThreadPool.QueueUserWorkItem(d => Dispatcher.Invoke(new Action(() => { label_status.Content = "Starting roll back..."; })));
            DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            startTime = DateTime.Now;
            dispatcherTimer.Start();
            String startExtractResult = await RunRollBack(instanceName);

            if (startExtractResult.Equals("true"))
            {
                String status = await WaitExtract(instanceName);
                ThreadPool.QueueUserWorkItem(d => Dispatcher.Invoke(new Action(() => { label_status.Content = "Waiting extract... " + status; })));
                while (!status.Equals("ROLLEDBACK"))
                {
                    await Task.Delay(2000);
                    status = await WaitExtract(instanceName);
                    ThreadPool.QueueUserWorkItem(d => Dispatcher.Invoke(new Action(() => { label_status.Content = "Waiting extract... " + status; })));
                }
            }
            ThreadPool.QueueUserWorkItem(d => Dispatcher.Invoke(new Action(() => { label_status.Content = "Finished"; })));
            dispatcherTimer.Stop();

            return "Done";
        }

        private async Task<String> RunExtract(String instanceName, String[] parameters)
        {
            StringBuilder xmlParameters = new StringBuilder();
            for (int i = 0; i < parameters.Length; i++)
            {
                String[] parameter = parameters[i].Trim().Split('=');
                xmlParameters.Append("<typ:parameterValues><flow:ParameterName>" + parameter[0] + "</flow:ParameterName><flow:ParameterValue>" + parameter[1] + "</flow:ParameterValue></typ:parameterValues>");
            }

            //DateTime selected_date = datepicker.SelectedDate.Value;
            //xmlParameters.Append("<typ:parameterValues><flow:ParameterName>Effective Date</flow:ParameterName><flow:ParameterValue>" + selected_date.ToString("yyyy-MM-dd") + "</flow:ParameterValue></typ:parameterValues>");
            String comboValue = ((ComboBoxItem)comboBox.SelectedValue).Content as String;
            if (comboValue != null)
            {
                xmlParameters.Append("<typ:parameterValues><flow:ParameterName>Changes Only</flow:ParameterName><flow:ParameterValue>" + comboValue + "</flow:ParameterValue></typ:parameterValues>");
            }

            StringBuilder ldgParameter = new StringBuilder();
            if (!textbox_ldg.Text.Equals(""))
            {
                ldgParameter.Append("<typ:legislativeDataGroupName>" + textbox_ldg.Text + "</typ:legislativeDataGroupName>");
            }

            String xmlInput = "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:typ=\"http://xmlns.oracle.com/apps/hcm/processFlows/core/flowActionsService/types/\" xmlns:flow=\"http://xmlns.oracle.com/apps/hcm/processFlows/core/flowControllerService/\"><soapenv:Header/><soapenv:Body><typ:submitFlow><typ:flowName>" + textBox_extractname.Text + "</typ:flowName><typ:flowInstanceName>" + instanceName + "</typ:flowInstanceName>" + xmlParameters + "<typ:scheduleFormulaTypeName> </typ:scheduleFormulaTypeName>" + ldgParameter + "<typ:scheduleFormulaName> </typ:scheduleFormulaName><typ:scheduledDate>2017-01-01T00:00:00.000000000</typ:scheduledDate><typ:recurringFlag>false</typ:recurringFlag><typ:endDate>2017-01-01T00:00:00.000000000</typ:endDate></typ:submitFlow></soapenv:Body></soapenv:Envelope>";

            //Old Web Service URL
            //HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://" + textbox_environment.Text + ":443/hcmProcFlowCoreController/FlowActionsService");

            //2020-02-06: New Web Service URL
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://" + textbox_environment.Text + ":443/hcmService/FlowActionsService");
        
            byte[] bytes = Encoding.ASCII.GetBytes(xmlInput);

            request.ContentLength = bytes.Length;
            request.ContentType = "text/xml; charset=utf-8";
            request.Headers.Add("SOAPAction", "http://xmlns.oracle.com/apps/hcm/processFlows/core/flowActionService/submitFlow");
            String encoded = System.Convert.ToBase64String(Encoding.ASCII.GetBytes(textbox_username.Text + ":" + textbox_password.Password));
            request.Headers.Add("Authorization", "Basic " + encoded);
            request.Method = "POST";
            Stream newStream = request.GetRequestStream();
            newStream.Write(bytes, 0, bytes.Length);
            newStream.Close();

            HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();
            Stream streamResponse = response.GetResponseStream();
            StreamReader streamRead = new StreamReader(streamResponse);
            Char[] readBuff = new Char[256];
            int count = streamRead.Read(readBuff, 0, 256);
            StringBuilder output = new StringBuilder();
            while (count > 0)
            {
                String outputData = new String(readBuff, 0, count);
                output.Append(outputData);
                count = streamRead.Read(readBuff, 0, 256);
            }
            // Close the Stream object.
            streamResponse.Close();
            streamRead.Close();
            // Release the resources held by response object.
            response.Close();

            textbox_results.Text = output.ToString();

            XmlDocument xml = new XmlDocument();
            xml.LoadXml(output.ToString());

            String result = xml.GetElementsByTagName("result")[0].InnerText;
            return result;
        }

        private async Task<String> RunRollBack(String instanceName)
        {
            //DateTime selected_date = datepicker.SelectedDate.Value;
            
            StringBuilder ldgParameter = new StringBuilder();
            if (!textbox_ldg.Text.Equals(""))
            {
                ldgParameter.Append("<typ:legislativeDataGroupName>" + textbox_ldg.Text + "</typ:legislativeDataGroupName>");
            }

            String xmlInput = "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:typ=\"http://xmlns.oracle.com/apps/hcm/processFlows/core/flowActionsService/types/\"><soapenv:Header/><soapenv:Body><typ:performAction><typ:flowInstanceName>" + instanceName + "</typ:flowInstanceName><typ:flowTaskInstanceName>" + textBox_extractname.Text + "</typ:flowTaskInstanceName><typ:actionName>roll back</typ:actionName>" + ldgParameter + "</typ:performAction></soapenv:Body></soapenv:Envelope>";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://" + textbox_environment.Text + ":443/hcmService/FlowActionsService");
            byte[] bytes = Encoding.ASCII.GetBytes(xmlInput);

            request.ContentLength = bytes.Length;
            request.ContentType = "text/xml; charset=utf-8";
            request.Headers.Add("SOAPAction", "http://xmlns.oracle.com/apps/hcm/processFlows/core/flowActionService/performAction");
            String encoded = System.Convert.ToBase64String(Encoding.ASCII.GetBytes(textbox_username.Text + ":" + textbox_password.Password));
            request.Headers.Add("Authorization", "Basic " + encoded);
            request.Method = "POST";
            Stream newStream = request.GetRequestStream();
            newStream.Write(bytes, 0, bytes.Length);
            newStream.Close();

            HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();
            Stream streamResponse = response.GetResponseStream();
            StreamReader streamRead = new StreamReader(streamResponse);
            Char[] readBuff = new Char[256];
            int count = streamRead.Read(readBuff, 0, 256);
            StringBuilder output = new StringBuilder();
            while (count > 0)
            {
                String outputData = new String(readBuff, 0, count);
                output.Append(outputData);
                count = streamRead.Read(readBuff, 0, 256);
            }
            // Close the Stream object.
            streamResponse.Close();
            streamRead.Close();
            // Release the resources held by response object.
            response.Close();

            textbox_results.Text = output.ToString();

            XmlDocument xml = new XmlDocument();
            xml.LoadXml(output.ToString());

            String result = xml.GetElementsByTagName("result")[0].InnerText;
            return result;
        }

        private async Task<String> WaitExtract(String instanceName)
        {
            StringBuilder ldgParameter = new StringBuilder();
            if (!textbox_ldg.Text.Equals(""))
            {
                ldgParameter.Append("<typ:legislativeDataGroupName>" + textbox_ldg.Text + "</typ:legislativeDataGroupName>");
            }

            String xmlInput = "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:typ=\"http://xmlns.oracle.com/apps/hcm/processFlows/core/flowActionsService/types/\"><soapenv:Header/><soapenv:Body><typ:getFlowTaskInstanceStatus><typ:flowInstanceName>" + instanceName + "</typ:flowInstanceName><typ:flowTaskInstanceName>" + textBox_extractname.Text + "</typ:flowTaskInstanceName>" + ldgParameter + "</typ:getFlowTaskInstanceStatus></soapenv:Body></soapenv:Envelope>";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://" + textbox_environment.Text + ":443/hcmService/FlowActionsService");
            byte[] bytes = Encoding.ASCII.GetBytes(xmlInput);

            request.ContentLength = bytes.Length;
            request.ContentType = "text/xml; charset=utf-8";
            request.Headers.Add("SOAPAction", "http://xmlns.oracle.com/apps/hcm/processFlows/core/flowActionService/getFlowTaskInstanceStatus");
            String encoded = System.Convert.ToBase64String(Encoding.ASCII.GetBytes(textbox_username.Text + ":" + textbox_password.Password));
            request.Headers.Add("Authorization", "Basic " + encoded);
            request.Method = "POST";
            Stream newStream = request.GetRequestStream();
            newStream.Write(bytes, 0, bytes.Length);
            newStream.Close();

            HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();
            Stream streamResponse = response.GetResponseStream();
            StreamReader streamRead = new StreamReader(streamResponse);
            Char[] readBuff = new Char[256];
            int count = streamRead.Read(readBuff, 0, 256);
            StringBuilder output = new StringBuilder();
            while (count > 0)
            {
                String outputData = new String(readBuff, 0, count);
                output.Append(outputData);
                count = streamRead.Read(readBuff, 0, 256);
            }
            // Close the Stream object.
            streamResponse.Close();
            streamRead.Close();
            // Release the resources held by response object.
            response.Close();

            textbox_results.Text = output.ToString();

            XmlDocument xml = new XmlDocument();
            xml.LoadXml(output.ToString());

            String result = xml.GetElementsByTagName("result")[0].InnerText;
            return result;
        }

        private async Task<String> ReadExtract(String instanceName)
        {
            StringBuilder ldgParameter = new StringBuilder();
            if (!textbox_ldg.Text.Equals(""))
            {
                ldgParameter.Append("<typ:legislativeDataGroupName>" + textbox_ldg.Text + "</typ:legislativeDataGroupName>");
            }

            String xmlInput = "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:typ=\"http://xmlns.oracle.com/apps/hcm/batchProcesses/core/batchProcessesCoreService/types/\"><soapenv:Header/><soapenv:Body><typ:fetchExtractOutput><typ:instanceName>" + instanceName + "</typ:instanceName><typ:taskName>" + textBox_extractname.Text + "</typ:taskName> " + ldgParameter + "<typ:mode>FLOW</typ:mode></typ:fetchExtractOutput></soapenv:Body></soapenv:Envelope>";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://" + textbox_environment.Text + ":443/hcmService/PayrollProcessingActionService");
            byte[] bytes = Encoding.ASCII.GetBytes(xmlInput);

            request.ContentLength = bytes.Length;
            request.ContentType = "text/xml; charset=utf-8";
            request.Headers.Add("SOAPAction", "http://xmlns.oracle.com/apps/hcm/batchProcesses/core/batchProcessesCoreService/fetchExtractOutput");
            String encoded = System.Convert.ToBase64String(Encoding.ASCII.GetBytes(textbox_username.Text + ":" + textbox_password.Password));
            request.Headers.Add("Authorization", "Basic " + encoded);
            request.Method = "POST";
            Stream newStream = request.GetRequestStream();
            newStream.Write(bytes, 0, bytes.Length);
            newStream.Close();

            HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();
            Stream streamResponse = response.GetResponseStream();
            StreamReader streamRead = new StreamReader(streamResponse);
            Char[] readBuff = new Char[256];
            int count = streamRead.Read(readBuff, 0, 256);
            StringBuilder output = new StringBuilder();
            while (count > 0)
            {
                String outputData = new String(readBuff, 0, count);
                output.Append(outputData);
                count = streamRead.Read(readBuff, 0, 256);
            }
            // Close the Stream object.
            streamResponse.Close();
            streamRead.Close();
            // Release the resources held by response object.
            response.Close();

            textbox_results.Text = output.ToString();

            XmlDocument xml = new XmlDocument();
            xml.LoadXml(output.ToString());

            String result = xml.GetElementsByTagName("result")[0].InnerText;
            
            // Format the XML text.
            StringWriter string_writer = new StringWriter();
            XmlTextWriter xml_text_writer = new XmlTextWriter(string_writer);
            xml_text_writer.Formatting = Formatting.Indented;
            XmlDocument xml_document = new XmlDocument();
            xml_document.LoadXml(result);
            xml_document.WriteTo(xml_text_writer);

            // Display the result.
            textbox_results.Text = string_writer.ToString();
            if (!String.IsNullOrEmpty(textbox_output_folder.Text))
            {
                File.WriteAllText(textbox_output_folder.Text + "\\" + instanceName + ".xml", string_writer.ToString());
            }

            return "Done";
        }
    }
}
