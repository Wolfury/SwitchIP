using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Management;
using System.Threading;
using System.IO;

namespace SwitchIP
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            ReportIP();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            curInfoTextBox.Text = "";
            string ipStr = listBox1.SelectedItem.ToString();
            int posi = ipStr.IndexOf("192.");
            ipStr = ipStr.Substring(posi, ipStr.Length - posi);
            button1.Enabled = false;
            button2.Enabled = false;
            ChangeTo(new string[] { ipStr }, new string[] { "255.255.255.0" }, new string[] { "192.168.1.1" }, new string[] { "192.168.1.1" });
            button1.Enabled = true;
            button2.Enabled = true;
            MessageBox.Show("修改完成！");
            ReportIP();
        }

        public void ChangeTo(string[] ipAddr, string[] subnetMask, string[] gateways, /*string[] gatewayCostMetric, */string[] dnsServer)
        {

            ManagementBaseObject iObj = null;
            ManagementBaseObject oObj = null;
            ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection moc = mc.GetInstances();

            foreach (ManagementObject mo in moc)
            {
                if (!(bool)mo["IPEnabled"]) continue;

                iObj = mo.GetMethodParameters("EnableStatic");
                iObj["IPAddress"] = ipAddr;
                iObj["SubnetMask"] = subnetMask;
                oObj = mo.InvokeMethod("EnableStatic", iObj, null);

                iObj = mo.GetMethodParameters("SetGateways");
                iObj["DefaultIPGateway"] = gateways;
                //iObj["GatewayCostMetric"] = gatewayCostMetric;
                oObj = mo.InvokeMethod("SetGateways", iObj, null);

                iObj = mo.GetMethodParameters("SetDNSServerSearchOrder");
                iObj["DNSServerSearchOrder"] = dnsServer;
                oObj = mo.InvokeMethod("SetDNSServerSearchOrder", iObj, null);
                break;
            }
        } 
        /// <summary>DHCPEnabled</summary> 
        //public void EnableDHCP()
        //{
        //    ManagementObjectCollection moc; 
        //    foreach (ManagementObject mo in moc)
        //    {
        //        if (!(bool)mo["IPEnabled"]) continue;

        //        if (!(bool)mo["DHCPEnabled"])
        //        {
        //            iObj = mo.GetMethodParameters("EnableDHCP");
        //            oObj = mo.InvokeMethod("EnableDHCP", iObj, null);
        //        }
        //    }
        //}

        void ReportIP()
        {
            curInfoTextBox.Text = "";

            ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementObject mo in moc)
            {
                if (!(bool)mo["IPEnabled"])
                    continue;

                //curInfoTextBox.Text += "{0}\n   SVC:   '{1}'   MAC:   [{2}]"+ (string)mo["Caption"]+(string)mo["ServiceName"]+ (string)mo["MACAddress"];
                string[] infoNameArr = new string[] { "IPAddress", "IPSubnet", "DefaultIPGateway", "DNSServerSearchOrder" };
                foreach (string name in infoNameArr)
                { 
                    string[] strArr;
                    strArr = (string[])mo[name];
                    curInfoTextBox.Text += name + ":";
                    if (strArr == null)
                    {
                        curInfoTextBox.Text += "该项值有问题！";
                        continue;
                    }
                    foreach (string str in strArr)
                    {
                        curInfoTextBox.Text += str;
                        curInfoTextBox.Text += "\r\n";
                    }
                    
                }


            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            StreamReader sr = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "ipAddress.txt",Encoding.Unicode);
            string str = sr.ReadLine();
            while (str != null)
            {
                listBox1.Items.Add(str);
                str = sr.ReadLine();
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string ipStr = listBox1.SelectedItem.ToString();
            int posi = ipStr.IndexOf("192.");
            string name = ipStr.Substring(0, posi-1);
            ipStr = ipStr.Substring(posi, ipStr.Length - posi);
            AdviceTextBox.Text = name + "，您好！\r\n   我们建议你的网络连接应做如下设置：\r\n";
            AdviceTextBox.Text += "   IPAddress:" + ipStr + "\r\n";
            AdviceTextBox.Text += "   IPSubnet:255.255.255.0" + "\r\n";
            AdviceTextBox.Text += "   DefaultIPGateway:192.168.1.1" + "\r\n";
            AdviceTextBox.Text += "   DNSServerSearchOrder:192.168.1.1" + "\r\n\r\n";
            AdviceTextBox.Text += "点下面按钮即可完成以上设置！" + "\r\n";

        }

        private void button2_Click(object sender, EventArgs e)
        {

            ReportIP();

        }

    }
}

